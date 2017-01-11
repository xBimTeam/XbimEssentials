using System.Collections;
using System.Collections.Generic;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Common.Metadata;
using Xbim.Essentials.Tests;

namespace Xbim.Ifc.Validation
{
    /// <summary>
    /// This class provides basic POCO access to validation errors.
    /// Validation reporting should build upon this. For an example see <see cref="ValidationReporter"/>
    /// </summary>
    public class IfcValidator
    {
        public bool CreateEntityHierarchy { get; set; } = false;

        private int _entityCount;

        private int _resultCount;

        public int EntityCountLimit { get; set; } = -1;

        public int ResultCountLimit { get; set; } = -1;
        
        public ValidationFlags ValidateLevel = ValidationFlags.Properties;

        public bool LimitReached => (EntityCountLimit >= 0 && _entityCount >= EntityCountLimit)
                                    || 
                                    (ResultCountLimit >= 0 && _resultCount >= ResultCountLimit);

        /// <summary>
        /// Validates all entities in the model, unless count limits are reached
        /// </summary>
        /// <returns>An enumerable of results</returns>
        public IEnumerable<ValidationResult> Validate(IfcStore store)
        {
            return Validate(store.Instances);
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
                hierResult.Message = $"Entity #{ent.EntityLabel} ({ifcType.Name}) has validation failures.";
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
                        IssueType = ValidationFlags.TypeWhereClauses,
                        IssueSource = propName,
                        Message = $"{instance.GetType().Name}.{propName} is not optional."
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
                        hierResult.Message = $"Property {prop.Name} has validation failures.";
                        yield return hierResult;
                    }
                }
                yield break;
            }

            if (ifcAttr.State == EntityAttributeState.Mandatory && propVal == null)
                yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueSource = propName,
                        Message = $"{instance.GetType().Name}.{propName} is not optional."
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

                if (count < ifcAttr.MinCardinality)
                {
                    yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueSource = propName,
                        Message = $"{instance.GetType().Name}.{propName} must have at least {ifcAttr.MinCardinality} item(s). " +
                                  $"It has {count}."
                    };
                }

                if (ifcAttr.MaxCardinality > -1 && count > ifcAttr.MaxCardinality)
                {
                    yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueSource = propName,
                        Message = $"{instance.GetType().Name}.{propName} must have no more than {ifcAttr.MaxCardinality} item(s). " +
                                  $"It has at least {count}."
                    };
                }
            }
        }
    }
}
