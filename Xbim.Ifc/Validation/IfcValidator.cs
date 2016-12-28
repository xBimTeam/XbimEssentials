using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Common.Metadata;

namespace Xbim.Ifc.Validation
{
    /// <summary>
    /// This class provides basic POCO access to validation errors.
    /// Validation reporting should build upon this.
    /// </summary>
    public class IfcValidator
    {
        /// <summary>
        /// Validates all entities in the model
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(IfcStore store, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            foreach (var entity in store.Instances)
            {
                foreach (var result in Validate(entity, validateLevel))
                {
                    yield return result;
                }
            }            
        }

        public IEnumerable<ValidationResult> Validate(IEnumerable<IPersistEntity> entities, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            foreach (var entity in entities)
            {
                foreach (var result in Validate(entity, validateLevel))
                {
                    yield return result;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(IPersistEntity ent, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            if (validateLevel == ValidationFlags.None)
                yield break; //nothing to do
            var ifcType = ent.ExpressType;
            var notIndented = true;
            
            if (validateLevel.HasFlag(ValidationFlags.Properties))
            {
                foreach (var ifcProp in ifcType.Properties.Values)
                {
                    var errs = GetIfcSchemaError(ent, ifcProp, validateLevel);
                    foreach (var validationResult in errs)
                    {
                        yield return validationResult;
                    }
                }
            }
            if (validateLevel.HasFlag(ValidationFlags.Inverses))
            {
                foreach (var ifcInv in ifcType.Inverses)
                {
                    var errs = GetIfcSchemaError(ent, ifcInv, validateLevel);
                    foreach (var validationResult in errs)
                    {
                        yield return validationResult;
                    }
                }
            }

            if (ent is IExpressValidatable)
            {
                var errs = ((IExpressValidatable)ent).Validate();
                foreach (var validationResult in errs)
                {
                    yield return validationResult;
                }
            }
        }

        private static IEnumerable<ValidationResult> GetIfcSchemaError(IPersistEntity instance, ExpressMetaProperty prop, ValidationFlags validateLevel)
        {
            //IfcAttribute ifcAttr, object instance, object propVal, string propName

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
                        IssueSource = propName,
                        Message = $"{instance.GetType().Name}.{propName} is not optional"
                    };
                }

                if (validateLevel.HasFlag(ValidationFlags.TypeWhereClauses) && propVal is IExpressValidatable)
                {
                    foreach (var issue in ((IExpressValidatable)propVal).Validate())
                    {
                        // this should become hierarchical
                        issue.Item = instance;
                    }                    
                }
                yield break;
            }

            if (ifcAttr.State == EntityAttributeState.Mandatory && propVal == null)
                yield return new ValidationResult()
                    {
                        Item = instance,
                        IssueSource = propName,
                        Message = $"{instance.GetType().Name}.{propName} is not optional"
                };
            if (ifcAttr.State == EntityAttributeState.Optional && propVal == null)
                //if it is null and optional then it is ok
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
                                  $"It has at least {count}"
                    };
                    
                       
                }
            }
        }
    }
}
