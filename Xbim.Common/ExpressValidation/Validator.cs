using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.Metadata;

// todo: once c#6 is accepted to the solution these configurations should be removed.
// ReSharper disable ConvertToAutoProperty
// ReSharper disable UseStringInterpolation

namespace Xbim.Common.ExpressValidation
{
    /// <summary>
    /// This class provides basic POCO access to validation errors.
    /// Validation reporting should build upon this.
    /// </summary>
    public class Validator
    {

        protected int _entityCount;
        protected int _resultCount;

        public bool CreateEntityHierarchy { get; set; }
        public int EntityCountLimit { get; set; } = -1;
        public int ResultCountLimit { get; set; } = -1;

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
        public virtual IEnumerable<ValidationResult> Validate(IModel model)
        {
            return Validate(model.Instances);
        }

        /// <summary>
        /// Validates all provided entities, unless count limits are reached
        /// </summary>
        /// <returns>An enumerable of results</returns>
        public virtual IEnumerable<ValidationResult> Validate(IEnumerable<IPersistEntity> entities)
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
        public virtual IEnumerable<ValidationResult> Validate(IPersistEntity entity)
        {
            foreach (var result in PerformValidation(entity, CreateEntityHierarchy, ValidateLevel))
            {
                yield return result;
                if (LimitReached)
                    yield break;
            }           
        }

        protected IEnumerable<ValidationResult> PerformValidation(IPersistEntity ent, bool hierarchical, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            var thisEntAdded = false;
            var hierResult = new ValidationResult() {Item = ent, IssueType = ValidationFlags.None};
            if (validateLevel == ValidationFlags.None)
                yield break; //nothing to do
            var expType = ent.ExpressType;
            
            if (validateLevel.HasFlag(ValidationFlags.Properties))
            {
                foreach (var prop in expType.Properties.Values)
                {
                    var errs = GetSchemaErrors(ent, prop, validateLevel, hierarchical);
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
                foreach (var inv in expType.Inverses)
                {
                    var errs = GetSchemaErrors(ent, inv, validateLevel, hierarchical);
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
                    expType.Name);
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

        protected static IEnumerable<ValidationResult> GetSchemaErrors(IPersist instance, ExpressMetaProperty prop, ValidationFlags validateLevel, bool hierarchical)
        {
            var attr = prop.EntityAttribute;
            var propVal = prop.PropertyInfo.GetValue(instance, null);
            var propName = prop.PropertyInfo.Name;

            if (propVal is IExpressValueType)
            {
                var val = ((IExpressValueType) propVal).Value;
                var underlyingType = ((IExpressValueType)propVal).UnderlyingSystemType;
                if (attr.State == EntityAttributeState.Mandatory && val == null && underlyingType != typeof(bool?))
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

            if (attr.State == EntityAttributeState.Mandatory && propVal == null)
                yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueType = ValidationFlags.Properties,
                    IssueSource = propName,
                        Message = string.Format("{0}.{1} is not optional.", instance.GetType().Name, propName)
                    };
            if (attr.State == EntityAttributeState.Optional && propVal == null)
                //if it is null and optional then it is ok
                yield break;
            if (attr.State == EntityAttributeState.Optional && propVal is IOptionalItemSet && !((IOptionalItemSet)propVal).Initialized)
                //if it is non-initialized list and optional then it is ok
                yield break;
            if (attr.IsEnumerable)
            {
                if (
               (attr.MinCardinality == null || attr.MinCardinality.Length == 0 || attr.MinCardinality.All(c => c < 0)) &&
               (attr.MaxCardinality == null || attr.MaxCardinality.Length == 0 || attr.MaxCardinality.All(c => c < 1))) //we don't care how many so don't check
                    yield break;

                var depth = attr.MinCardinality.Length;
                if (depth != attr.MaxCardinality.Length)
                    throw new System.Exception("Inconsistent metadata: minimal and maximal cardinality has to have the same length.");

                var sb = new StringBuilder();
                var items = (IEnumerable)propVal;
                CheckCardinality(attr.MinCardinality, attr.MaxCardinality, items, 0, sb);
                var msg = sb.ToString();
                if (string.IsNullOrWhiteSpace(msg))
                    yield break;

                yield return new ValidationResult()
                {
                    Item = instance,
                    IssueType = ValidationFlags.Properties,
                    IssueSource = propName,
                    Message = string.Format("{0}.{1}: {2}", instance.GetType().Name, prop.Name, msg)
                };
            }
        }

        protected static void CheckCardinality(int[] minimums, int[] maximums, IEnumerable items, int depth, StringBuilder sb)
        {
            if (depth >= minimums.Length || items == null)
                return;

            var min = minimums[depth];
            var max = maximums[depth];

            if (min > 0 && max > 0)
            {
                var count = 0;
                var coll = items as ICollection;
                if (coll != null)
                    count = coll.Count;
                else
                {
                    // ReSharper disable once UnusedVariable
                    foreach (var item in items)
                    {
                        count++;
                        if (count >= min && max == -1)
                            //we have met the requirements
                            break;
                        if (max > -1 && count > max) //we are out of bounds
                            break;
                    }
                }

                if (count < min)
                {
                    sb.AppendFormat("Must have at least {0} item(s). It has {1} or more.", min, count);
                    sb.AppendLine();
                }
                if (count > max)
                {
                    sb.AppendFormat("Must have no more than {0} item(s). It has at least {1}.", max, count);
                    sb.AppendLine();
                }
            }

            if (depth + 1 == minimums.Length)
                return;

            foreach (var item in items)
            {
                CheckCardinality(minimums, maximums, item as IEnumerable, depth + 1, sb);
            }
        }
    }
}
