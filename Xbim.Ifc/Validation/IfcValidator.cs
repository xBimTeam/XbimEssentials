using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.Metadata;

namespace Xbim.Ifc.Validation
{
    public class IfcValidator
    {
        private IfcStore _store;

        public IfcValidator(IfcStore store)
        {
            _store = store;
        }

        #region Validation

        /// <summary>
        /// Validates all entities in the model
        /// </summary>
        /// <param name="tw"></param>
        /// <param name="validateLevel"></param>
        /// <returns></returns>
        public int Validate(TextWriter tw, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            int errors = 0;
            foreach (var entity in _store.Instances)
            {
                errors += Validate(entity, tw, validateLevel);
            }
            return errors;
        }

        public int Validate(IEnumerable<IPersistEntity> entities, TextWriter tw, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            int errors = 0;
            foreach (var entity in entities)
            {
                errors += Validate(entity, tw, validateLevel);
            }
            return errors;
        }

        public int Validate(IPersistEntity ent, TextWriter tw, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            var itw = new IndentedTextWriter(tw);
            if (validateLevel == ValidationFlags.None) return 0; //nothing to do
            var ifcType = ent.ExpressType;
            var notIndented = true;
            var errors = 0;
            if (validateLevel == ValidationFlags.Properties || validateLevel == ValidationFlags.All)
            {
                foreach (var ifcProp in ifcType.Properties.Values)
                {
                    var err = GetIfcSchemaError(ent, ifcProp);
                    if (string.IsNullOrEmpty(err))
                        continue;
                    if (notIndented)
                    {
                        itw.WriteLine($"#{ent.EntityLabel} - {ifcType.Type.Name}");
                        itw.Indent++;
                        notIndented = false;
                    }
                    itw.WriteLine(err.Trim('\n'));
                    errors++;
                }
            }
            if (validateLevel == ValidationFlags.Inverses || validateLevel == ValidationFlags.All)
            {
                foreach (var ifcInv in ifcType.Inverses)
                {
                    var err = GetIfcSchemaError(ent, ifcInv);
                    if (string.IsNullOrEmpty(err))
                        continue;
                    if (notIndented)
                    {
                        itw.WriteLine($"#{ent.EntityLabel} - {ifcType.Type.Name}");
                        itw.Indent++;
                        notIndented = false;
                    }
                    itw.WriteLine(err.Trim('\n'));
                    errors++;
                }
            }

            if (ent is IPersistValidatable)
            {
                var str = ((IPersistValidatable)ent).WhereRule();
                if (!string.IsNullOrEmpty(str))
                {
                    if (notIndented)
                    {
                        itw.WriteLine($"#{ent.EntityLabel} - {ifcType.Type.Name}");
                        itw.Indent++;
                        notIndented = false;
                    }
                    itw.WriteLine(str.Trim('\n'));
                    errors++;
                }
            }

            
            if (!notIndented)
                itw.Indent--;
            return errors;
        }

        private static string GetIfcSchemaError(IPersistEntity instance, ExpressMetaProperty prop)
        {
            //IfcAttribute ifcAttr, object instance, object propVal, string propName

            var ifcAttr = prop.EntityAttribute;
            var propVal = prop.PropertyInfo.GetValue(instance, null);
            var propName = prop.PropertyInfo.Name;

            if (propVal is ExpressType)
            {
                var err = "";
                // todo: test if this behaves like toPart21
                var val = propVal.ToString(); // topart21
                if (ifcAttr.State == EntityAttributeState.Mandatory && val == "$")
                    err += $"{instance.GetType().Name}.{propName} is not optional";
                if (propVal is IPersistValidatable)
                    err += ((IPersistValidatable)propVal).WhereRule();
                if (!string.IsNullOrEmpty(err))
                    return err;
            }

            if (ifcAttr.State == EntityAttributeState.Mandatory && propVal == null)
                return $"{instance.GetType().Name}.{propName} is not optional";
            if (ifcAttr.State == EntityAttributeState.Optional && propVal == null)
                //if it is null and optional then it is ok
                return null;
            if (ifcAttr.EntityType == EntityAttributeType.Set 
                || ifcAttr.EntityType == EntityAttributeType.List 
                || ifcAttr.EntityType == EntityAttributeType.ListUnique)
            {
                if (ifcAttr.MinCardinality < 1 && ifcAttr.MaxCardinality < 0) //we don't care how many so don't check
                    return null;
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
                    return
                        $"{instance.GetType().Name}.{propName} must have at least {ifcAttr.MinCardinality} item(s). " +
                        $"It has {count}";
                }
                if (ifcAttr.MaxCardinality > -1 && count > ifcAttr.MaxCardinality)
                {
                    return
                        $"{instance.GetType().Name}.{propName} must have no more than {ifcAttr.MaxCardinality} item(s). " +
                        $"It has at least {count}";
                }
            }
            return null;
        }
        #endregion
    }
}
