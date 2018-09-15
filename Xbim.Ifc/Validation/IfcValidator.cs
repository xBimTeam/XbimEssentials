using System.Collections;
using System.Collections.Generic;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Common.Metadata;

// todo: once c#6 is accepted to the solution these configurations should be removed.
// ReSharper disable ConvertToAutoProperty
// ReSharper disable UseStringInterpolation

namespace Xbim.Ifc.Validation
{
    /// <summary>
    /// This class provides basic POCO access to validation errors.
    /// Validation reporting should build upon this. For an example see <see cref="IfcValidationReporter"/>
    /// </summary>
    public class IfcValidator
    {
        private bool _createEntityHierarchy;
        public bool CreateEntityHierarchy
        {
            get { return _createEntityHierarchy; }
            set { _createEntityHierarchy = value; }
        }

        private int _entityCount;

        private int _resultCount;

        private int _entityCountLimit = -1;
        public int EntityCountLimit
        {
            get { return _entityCountLimit; }
            set { _entityCountLimit = value; }
        }

        private int _resultCountLimit = -1;
        public int ResultCountLimit
        {
            get { return _resultCountLimit; }
            set { _resultCountLimit = value; }
        }

        public ValidationFlags ValidateLevel = ValidationFlags.Properties;
        
        public bool LimitReached
        {
            get
            {
                return (EntityCountLimit >= 0 && _entityCount >= EntityCountLimit)
                       ||
                       (ResultCountLimit >= 0 && _resultCount >= ResultCountLimit);
            }
        }

        /// <summary>
        /// Validates all entities in the model, unless count limits are reached
        /// </summary>
        /// <returns>An enumerable of results</returns>
        public IEnumerable<ValidationResult> Validate(IModel model)
        {
            return Validate(model.Instances);
        }

        /// <summary>
        /// Validates all provided entities, unless count limits are reached
        /// </summary>
        /// <returns>An enumerable of results</returns>
        public IEnumerable<ValidationResult> Validate(IEnumerable<IPersistEntity> entities)
        {
            foreach (var entity in entities)
            {
                foreach (var result in Validate(entity))
                {
                    yield return result;
                    if (LimitReached)
                        yield break;
                }
            }
        }

        /// <summary>
        /// Validates the entities, unless count limits are reached 
        /// </summary>
        /// <returns>An enumerable of results</returns>
        public IEnumerable<ValidationResult> Validate(IPersistEntity entity)
        {
            foreach (var result in PerformValidation(entity, CreateEntityHierarchy, ValidateLevel))
            {
                yield return result;
                if (LimitReached)
                    yield break;
            }           
        }

        private IEnumerable<ValidationResult> PerformValidation(IPersistEntity ent, bool hierarchical, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            var thisEntAdded = false;
            var hierResult = new ValidationResult() {Item = ent, IssueType = ValidationFlags.None};
            if (validateLevel == ValidationFlags.None)
                yield break; //nothing to do
            var ifcType = ent.ExpressType;
            
            if (validateLevel.HasFlag(ValidationFlags.Properties))
            {
                foreach (var ifcProp in ifcType.Properties.Values)
                {
                    var errs = GetIfcSchemaErrors(ent, ifcProp, validateLevel, hierarchical);
                    foreach (var validationResult in errs)
                    {
                        validationResult.IssueType |= ValidationFlags.Properties;
                        thisEntAdded = UpdateCount(thisEntAdded);

                        if (hierarchical)
                        {
                            hierResult.AddDetail(validationResult);
                        }
                        else
                        {
                            validationResult.Item = ent;
                            yield return validationResult;
                        }
                    }
                }
            }
            if (validateLevel.HasFlag(ValidationFlags.Inverses))
            {
                foreach (var ifcInv in ifcType.Inverses)
                {
                    var errs = GetIfcSchemaErrors(ent, ifcInv, validateLevel, hierarchical);
                    foreach (var validationResult in errs)
                    {
                        validationResult.IssueType |= ValidationFlags.Inverses;
                        thisEntAdded = UpdateCount(thisEntAdded);
                        if (hierarchical)
                        {
                            hierResult.AddDetail(validationResult);
                        }
                        else
                        {
                            validationResult.Item = ent;
                            yield return validationResult;
                        }
                    }
                }
            }
            if (validateLevel.HasFlag(ValidationFlags.EntityWhereClauses) && ent is IExpressValidatable)
            {
                var errs = ((IExpressValidatable)ent).Validate();
                foreach (var validationResult in errs)
                {
                    thisEntAdded = UpdateCount(thisEntAdded);
                    if (hierarchical)
                    {
                        hierResult.AddDetail(validationResult);
                    }
                    else
                    {
                        yield return validationResult;
                    }
                }
            }

            if (hierarchical && hierResult.IssueType != ValidationFlags.None)
            {
                // the IssueType is populated if any children have been added.
                hierResult.Message = string.Format("Entity #{0} ({1}) has validation failures.", ent.EntityLabel,
                    ifcType.Name);
                yield return hierResult;
            }
        }

        private bool UpdateCount(bool thisEntAdded)
        {
            if (!thisEntAdded)
            {
                _entityCount++;
            }
            _resultCount++;
            return true;
        }

        private static IEnumerable<ValidationResult> GetIfcSchemaErrors(IPersist instance, ExpressMetaProperty prop, ValidationFlags validateLevel, bool hierarchical)
        {
            var ifcAttr = prop.EntityAttribute;
            var propVal = prop.PropertyInfo.GetValue(instance, null);
            var propName = prop.PropertyInfo.Name;

            if (propVal is IExpressValueType)
            {
                var val = ((IExpressValueType) propVal).Value;
                if (ifcAttr.State == EntityAttributeState.Mandatory && val == null)
                {
                    yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueType = ValidationFlags.Properties,
                        IssueSource = propName,
                        Message = string.Format("{0}.{1} is not optional.", instance.GetType().Name, propName)
                    };
                }
                if (validateLevel.HasFlag(ValidationFlags.TypeWhereClauses) && propVal is IExpressValidatable)
                {
                    var hierResult = new ValidationResult() { Item = instance, IssueType = ValidationFlags.None };
                    foreach (var issue in ((IExpressValidatable)propVal).Validate())
                    {
                        if (hierarchical)
                        {
                            hierResult.AddDetail(issue);  
                        }
                        else
                        {
                            yield return issue;
                        }
                    }
                    if (hierarchical && hierResult.IssueType != ValidationFlags.None)
                    {
                        // the IssueType is populated if any children have been added.
                        hierResult.Message = string.Format("Property {0} has validation failures.", prop.Name);
                        yield return hierResult;
                    }
                }
                yield break;
            }

            if (ifcAttr.State == EntityAttributeState.Mandatory && propVal == null)
                yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueType = ValidationFlags.Properties,
                    IssueSource = propName,
                        Message = string.Format("{0}.{1} is not optional.", instance.GetType().Name, propName)
                    };
            if (ifcAttr.State == EntityAttributeState.Optional && propVal == null)
                //if it is null and optional then it is ok
                yield break;
            if (ifcAttr.State == EntityAttributeState.Optional && propVal is IOptionalItemSet && !((IOptionalItemSet)propVal).Initialized)
                //if it is non-initialized list and optional then it is ok
                yield break;
            if (ifcAttr.EntityType == EntityAttributeType.Set 
                || ifcAttr.EntityType == EntityAttributeType.List 
                || ifcAttr.EntityType == EntityAttributeType.ListUnique)
            {
                if (ifcAttr.MinCardinality < 1 && ifcAttr.MaxCardinality < 0) //we don't care how many so don't check
                    yield break;
                var coll = propVal as ICollection;
                var count = 0;
                if (coll != null)
                    count = coll.Count;
                else
                {
                    var en = (IEnumerable)propVal;
                    if (en != null)
                    { 
                        // todo: ensure that this faster than count() given the conditionals in the loop
                        // ReSharper disable once UnusedVariable // the loop is only executed to count the item if needed.
                        foreach (var item in en)
                        {
                            count++;
                            if (count >= ifcAttr.MinCardinality && ifcAttr.MaxCardinality == -1)
                                //we have met the requirements
                                break;
                            if (ifcAttr.MaxCardinality > -1 && count > ifcAttr.MaxCardinality) //we are out of bounds
                                break;
                        }
                    }
                }

                if (count < ifcAttr.MinCardinality)
                {
                    yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueType = ValidationFlags.Properties,
                        IssueSource = propName,
                        Message =
                            string.Format("{0}.{1} must have at least {2} item(s). It has {3}.", instance.GetType().Name,
                                propName, ifcAttr.MinCardinality, count)
                    };
                }

                if (ifcAttr.MaxCardinality > -1 && count > ifcAttr.MaxCardinality)
                {
                    yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueType = ValidationFlags.Properties,
                        IssueSource = propName,
                        Message =
                            string.Format("{0}.{1} must have no more than {2} item(s). It has at least {3}.",
                                instance.GetType().Name, propName, ifcAttr.MaxCardinality, count)
                    };
                }
            }
        }
    }
}
