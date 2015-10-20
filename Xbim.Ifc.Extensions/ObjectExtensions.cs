#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ObjectExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Specific type information  that is common to all instances of IfcObject refering to the same type.
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IfcTypeObject GetDefiningType(this IfcObject tObj, IModel model)
        {
            var def =  model.Instances.Where<IfcRelDefinesByType>(rd => rd.RelatedObjects.Contains(tObj)).FirstOrDefault();
            if (def != null)
                return def.RelatingType;
            else
                return null;
        }

        public static IfcTypeObject GetDefiningType(this IfcObject tObj)
        {
            var def = tObj.Model.Instances.Where<IfcRelDefinesByType>(rd => rd.RelatedObjects.Contains(tObj)).FirstOrDefault();
            if (def != null)
                return def.RelatingType;
            else
                return null;
           
        }
        //Removes the current object from any RelDefinesByType relationships and adds a relationship to the specified Type 
        public static void SetDefiningType(this IfcObject obj, IfcTypeObject typeObj, IModel model)
        {

            //divorce any exisitng related types
            var rels = model.Instances.Where<IfcRelDefinesByType>(rd => rd.RelatedObjects.Contains(obj));
            foreach (var rel in rels)
            {
                rel.RelatedObjects.Remove(obj);
            }
            //find any existing relationships to this type
            var typeRel = model.Instances.Where<IfcRelDefinesByType>(rd => rd.RelatingType==typeObj).FirstOrDefault();
            if (typeRel == null) //none defined create the relationship
            {
                var relSub = model.Instances.New<IfcRelDefinesByType>();
                relSub.RelatingType = typeObj;
                relSub.RelatedObjects.Add(obj);
            }
            else //we have the type
            {

                typeRel.RelatedObjects.Add(obj);
            }
        }

        /// <summary>
        /// Adds an existing property set to the objecty, NB no check is done for duplicate psets
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSet"></param>
        public static void AddPropertySet(this IfcObject obj, IfcPropertySet pSet)
        {
            var model = obj.Model;
            var relDef = model.Instances.OfType<IfcRelDefinesByProperties>().Where(r => r.RelatingPropertyDefinition==pSet).FirstOrDefault(); ;
            if (relDef==null)
            {
                
                relDef = model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pSet;
            }
            relDef.RelatedObjects.Add(obj);
        }

        /// <summary>
        /// Returns the propertyset of the specified name, null if it does not exist
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <returns></returns>
        public static IfcPropertySet GetPropertySet(this IfcObject obj, string pSetName, bool caseSensitive = true)
        {
            IfcRelDefinesByProperties rel = caseSensitive ?
                obj.IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName)
                : obj.IsDefinedByProperties.FirstOrDefault(r => string.Equals(r.RelatingPropertyDefinition.Name.ToString(), pSetName, StringComparison.CurrentCultureIgnoreCase));
            if (rel != null) return rel.RelatingPropertyDefinition as IfcPropertySet;
            return null;
        }
        public static IfcPropertySingleValue GetPropertySingleValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName)
        {
            var pset = GetPropertySet(obj, pSetName);
            if (pset != null)
                return pset.HasProperties.Where<IfcPropertySingleValue>(p => p.Name == propertyName).FirstOrDefault();
            return null;
        }
        public static VType GetPropertySingleValue<VType>(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName) where VType : IfcValue
        {
            var pset = GetPropertySet(obj, pSetName);
            if (pset != null)
            {
                var pVal = pset.HasProperties.Where<IfcPropertySingleValue>(p => p.Name == propertyName).FirstOrDefault();
                if (pVal != null && typeof(VType).IsAssignableFrom(pVal.NominalValue.GetType())) return (VType)pVal.NominalValue;
            }
            return default(VType);
        }

        /// <summary>
        /// If the property value exists, returns the Nominal Value of the contents
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IfcValue GetPropertySingleNominalValue (this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(obj, pSetName, propertyName);
            return psv == null ? null : psv.NominalValue;
        }

        public static List<IfcPropertySet> GetAllPropertySets(this Xbim.Ifc2x3.Kernel.IfcObject obj)
        {
            var result = new List<IfcPropertySet>();
            IEnumerable<IfcRelDefinesByProperties> rels = obj.IsDefinedByProperties;
            foreach (var rel in rels)
            {
                var pSet = rel.RelatingPropertyDefinition as IfcPropertySet;
                if (pSet != null) result.Add(pSet);
            }

            return result;
        }

        public static Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>> GetAllPropertySingleValues(this Xbim.Ifc2x3.Kernel.IfcObject obj)
        {
            var result = new Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>>();
            IEnumerable<IfcRelDefinesByProperties> relations = obj.IsDefinedByProperties.OfType<IfcRelDefinesByProperties>();
            foreach (var rel in relations)
            {
                var value = new Dictionary<IfcIdentifier, IfcValue>();
                var psetName = rel.RelatingPropertyDefinition.Name??null;
                var pSet = rel.RelatingPropertyDefinition as IfcPropertySet;
                if (pSet == null) continue;
                foreach (var prop in pSet.HasProperties)
                {
                    var singleVal = prop as IfcPropertySingleValue;
                    if (singleVal == null) continue;
                    value.Add(prop.Name, singleVal.NominalValue);
                }
                if (!result.ContainsKey(psetName))
                    result.Add(psetName, value);
            }
            return result;
        }

        public static void DeletePropertySingleValueValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(obj, pSetName, propertyName);
            if (psv == null) return;
            psv.NominalValue = null;
        }

        public static IfcPropertyTableValue GetPropertyTableValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyTableName)
        {
            var pset = GetPropertySet(obj, pSetName);
            if (pset != null)
                return pset.HasProperties.Where<IfcPropertyTableValue>(p => p.Name == propertyTableName).FirstOrDefault();
            return null;
        }

        public static IfcValue GetPropertyTableItemValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyTableName, IfcValue definingValue)
        {
            var table = GetPropertyTableValue(obj, pSetName, propertyTableName);
            if (table == null) return null;
            var definingValues = table.DefiningValues;
            if (definingValues == null) return null;
            if (!definingValues.Contains(definingValue)) return null;
            int index = definingValues.IndexOf(definingValue);

            if (table.DefinedValues.Count < index + 1) return null;
            return table.DefinedValues[index];
        }

        public static void SetPropertyTableItemValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyTableName, IfcValue definingValue, IfcValue definedValue)
        {
            SetPropertyTableItemValue(obj, pSetName, propertyTableName, definingValue, definedValue, null, null);
        }

        public static void SetPropertyTableItemValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyTableName, IfcValue definingValue, IfcValue definedValue, IfcUnit definingUnit, IfcUnit definedUnit)
        {
            var pset = GetPropertySet(obj, pSetName);
            IModel model = null;
            if (pset == null)
            {
                model = obj.Model;
                pset = model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                var relDef = model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pset;
                relDef.RelatedObjects.Add(obj);
            }
            var table = GetPropertyTableValue(obj, pSetName, propertyTableName);
            if (table == null)
            {
                model = obj.Model;
                table = model.Instances.New<IfcPropertyTableValue>(tb => { tb.Name = propertyTableName; });
                pset.HasProperties.Add(table);
                table.DefinedUnit = definedUnit;
                table.DefiningUnit = definingUnit;
            }
            if (table.DefiningUnit != definingUnit || table.DefinedUnit != definedUnit)
                throw new Exception("Inconsistent definition of the units in the property table.");

            var itemValue = GetPropertyTableItemValue(obj, pSetName, propertyTableName, definingValue);
            if (itemValue != null)
            {
                itemValue = definedValue;
            }
            else
            {
                //if (table.DefiningValues == null) table.DefiningValues = new XbimList<IfcValue>();
                table.DefiningValues.Add(definingValue);
                //if (table.DefinedValues == null) table.DefinedValues = new XbimList<IfcValue>();
                table.DefinedValues.Add(definedValue);

                //check of integrity
                if (table.DefinedValues.Count != table.DefiningValues.Count)
                    throw new Exception("Inconsistent state of the property table. Number of defined and defining values are not the same.");
            }
        }

        /// <summary>
        /// Creates property single value with specified type and default value of this type (0 for numeric types, empty string tor string types and false for bool types)
        /// </summary>
        /// <param name="pSetName">Property set name</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="type">Type of the property</param>
        /// <returns>Property single value with default value of the specified type</returns>
        public static IfcPropertySingleValue SetPropertySingleValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName, Type type)
        {
            if (typeof(IfcValue).IsAssignableFrom(type))
            {
                IfcValue value;
                if (typeof(IfcPositiveLengthMeasure).IsAssignableFrom(type))
                    value = Activator.CreateInstance(type, 1.0) as IfcValue;
                else
                    value = Activator.CreateInstance(type) as IfcValue;

                if (value != null)
                    return SetPropertySingleValue(obj, pSetName, propertyName, value);
                else
                    throw new Exception("Type '" + type.Name + "' can't be initialized.");
            }
            else
                throw new ArgumentException("Type '" + type.Name + "' is not compatible with IfcValue type.");
        }

        public static IfcPropertySingleValue SetPropertySingleValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName, IfcValue value)
        {
            var pset = GetPropertySet(obj, pSetName);
            IModel model = null;
            if (pset == null)    
            {
                model = obj.Model;
                pset = model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                var relDef = model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pset;
                relDef.RelatedObjects.Add(obj);
            }

            //change existing property of the same name from the property set
            var singleVal = GetPropertySingleValue(obj, pSetName, propertyName);
            if (singleVal != null)
            {
                singleVal.NominalValue = value;
            }
            else
            {
                model = obj.Model;
                singleVal = model.Instances.New<IfcPropertySingleValue>(psv => { psv.Name = propertyName; psv.NominalValue = value; });
                pset.HasProperties.Add(singleVal);
            }

            return singleVal;
        }


        /// <summary>
        /// Returns a list of all the elements that bound the external of the building 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        static public IEnumerable<IfcElement> GetExternalElements(IModel model)
        {
            return model.Instances.OfType<IfcRelSpaceBoundary>().Where(r => r.InternalOrExternalBoundary == IfcInternalOrExternalEnum.EXTERNAL
                && r.PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL
                && r.RelatedBuildingElement != null).Select(rsb => rsb.RelatedBuildingElement).Distinct();
        }

        public static IfcElementQuantity GetElementQuantity(this IfcObject elem, string pSetName, bool caseSensitive = true)
        {
            IfcRelDefinesByProperties rel = caseSensitive ?
                elem.IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is IfcElementQuantity).FirstOrDefault()
                : elem.IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition.Name.ToString().ToLower() == pSetName.ToLower() && r.RelatingPropertyDefinition is IfcElementQuantity).FirstOrDefault();
            if (rel != null) return rel.RelatingPropertyDefinition as IfcElementQuantity;
            else return null;
        }

        /// <summary>
        /// Use this method to get all element quantities related to this object
        /// </summary>
        /// <returns>All related element quantities</returns>
        public static IEnumerable<IfcElementQuantity> GetAllElementQuantities(this IfcObject elem)
        {
            var rels = elem.IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition is IfcElementQuantity);
            foreach (var rel in rels)
            {
                yield return rel.RelatingPropertyDefinition as IfcElementQuantity;
            }
        }

        /// <summary>
        /// Use this to get all physical simple quantities (like length, area, volume, count, etc.)
        /// </summary>
        /// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        public static IEnumerable<IfcPhysicalSimpleQuantity> GetAllPhysicalSimpleQuantities(this IfcObject elem)
        {
            foreach (var eq in elem.GetAllElementQuantities())
            {
                foreach (var q in eq.Quantities)
                {
                    var psq = q as IfcPhysicalSimpleQuantity;
                    if (psq != null) yield return psq;
                }
            }
        }

        /// <summary>
        /// Returns the first quantity in the property set pSetName of name qName
        /// </summary>
        /// <typeparam name="QType"></typeparam>
        /// <param name="elem"></param>
        /// <param name="pSetName"></param>
        /// <param name="qName"></param>
        /// <returns></returns>
        public static QType GetQuantity<QType>(this IfcObject elem, string pSetName, string qName) where QType : IfcPhysicalQuantity
        {
            IfcRelDefinesByProperties rel = elem.IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is IfcElementQuantity).FirstOrDefault();
            if (rel != null)
            {
                var eQ =  rel.RelatingPropertyDefinition as IfcElementQuantity;
                if (eQ != null)
                {
                    var result = eQ.Quantities.Where<QType>(q => q.Name == qName).FirstOrDefault();
                    return result;
                }
            }
            return default(QType);
        }

        /// <summary>
        /// Returns the first quantity that matches the quantity name
        /// </summary>
        /// <typeparam name="QType"></typeparam>
        /// <param name="elem"></param>
        /// <param name="qName"></param>
        /// <returns></returns>
        public static QType GetQuantity<QType>(this IfcObject elem, string qName) where QType : IfcPhysicalQuantity
        {
            IfcRelDefinesByProperties rel = elem.IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition is IfcElementQuantity).FirstOrDefault();
            if (rel != null)
            {
                var eQ = rel.RelatingPropertyDefinition as IfcElementQuantity;
                if (eQ != null)
                {
                    var result = eQ.Quantities.Where<QType>(q => q.Name == qName).FirstOrDefault();
                    return result;
                }
            }
            return default(QType);
        }

        /// <summary>
        /// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        /// <param name="quantity">quantity to be added</param>
        /// <param name="methodOfMeasurement">Sets the method of measurement, if not null overrides previous value</param>
        public static IfcElementQuantity AddQuantity(this IfcObject elem, string propertySetName, IfcPhysicalQuantity quantity, string methodOfMeasurement)
        {
            var pset = elem.GetElementQuantity(propertySetName);

            if (pset == null)
            {
                var model = elem.Model;
                pset = model.Instances.New<IfcElementQuantity>();
                pset.Name = propertySetName;
                var relDef = model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pset;
                relDef.RelatedObjects.Add(elem);
            }
            pset.Quantities.Add(quantity);
            if (!string.IsNullOrEmpty(methodOfMeasurement)) pset.MethodOfMeasurement = methodOfMeasurement;
            return pset;
        }
        /// <summary>
        /// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        /// <param name="quantity">quantity to be added</param>
        static public IfcElementQuantity AddQuantity(this IfcObject elem, string propertySetName, IfcPhysicalQuantity quantity)
        {
            return AddQuantity(elem, propertySetName, quantity, null);
        }

        /// <summary>
        /// Returns simple physical quality of the element.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="pSetName"></param>
        /// <param name="qualityName"></param>
        /// <returns></returns>
        public static IfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(this IfcObject elem, string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(elem, pSetName);
            if (elementQuality != null)
            {
                return elementQuality.Quantities.Where<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName).FirstOrDefault();
            }
            else
                return null;
        }

        public static void SetElementPhysicalSimpleQuantity(this IfcObject elem, string qSetName, string qualityName, double value, XbimQuantityTypeEnum quantityType,IfcNamedUnit unit)
        {
            var model = elem.Model;

            var qset = GetElementQuantity(elem, qSetName);
            if (qset == null)
            {
                qset = model.Instances.New<IfcElementQuantity>();
                qset.Name = qSetName;
                var relDef = model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = qset;
                relDef.RelatedObjects.Add(elem);
            }

            //remove existing simple quality
            var simpleQuality = GetElementPhysicalSimpleQuantity(elem, qSetName, qualityName);
            if (simpleQuality != null)
            {
                var elementQuality = GetElementQuantity(elem, qSetName);
                elementQuality.Quantities.Remove(simpleQuality);
                model.Delete(simpleQuality);
            }

            switch (quantityType)
            {
                case XbimQuantityTypeEnum.AREA:
                    simpleQuality = model.Instances.New<IfcQuantityArea>(sq => sq.AreaValue = (IfcAreaMeasure)value);
                    break;
                case XbimQuantityTypeEnum.COUNT:
                    simpleQuality = model.Instances.New<IfcQuantityCount>(sq => sq.CountValue = (IfcCountMeasure)value);
                    break;
                case XbimQuantityTypeEnum.LENGTH:
                    simpleQuality = model.Instances.New<IfcQuantityLength>(sq => sq.LengthValue = (IfcLengthMeasure)value);
                    break;
                case XbimQuantityTypeEnum.TIME:
                    simpleQuality = model.Instances.New<IfcQuantityTime>(sq => sq.TimeValue = (IfcTimeMeasure)value);
                    break;
                case XbimQuantityTypeEnum.VOLUME:
                    simpleQuality = model.Instances.New<IfcQuantityVolume>(sq => sq.VolumeValue = (IfcVolumeMeasure)value);
                    break;
                case XbimQuantityTypeEnum.WEIGHT:
                    simpleQuality = model.Instances.New<IfcQuantityWeight>(sq => sq.WeightValue = (IfcMassMeasure)value);
                    break;
                default:
                    return;
            }

            simpleQuality.Unit = unit;
            simpleQuality.Name = qualityName;

            qset.Quantities.Add(simpleQuality);
        }

        public static void RemovePropertySingleValue(this Xbim.Ifc2x3.Kernel.IfcObject obj, string pSetName, string propertyName)
        {
            var pset = GetPropertySet(obj, pSetName);
            if (pset != null)
            {
                var singleValue = pset.HasProperties.Where<IfcPropertySingleValue>(p => p.Name == propertyName).FirstOrDefault();
                if (singleValue != null)
                {
                    pset.HasProperties.Remove(singleValue);
                }
            }
               
        }

        public static void RemoveElementPhysicalSimpleQuantity(this IfcObject elem, string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(elem, pSetName);
            if (elementQuality != null)
            {
                var simpleQuality = elementQuality.Quantities.Where<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName).FirstOrDefault();
                if (simpleQuality != null)
                {
                    elementQuality.Quantities.Remove(simpleQuality);
                }
            }
        }
    }

    public enum XbimQuantityTypeEnum
    {
        LENGTH,
        AREA,
        VOLUME,
        COUNT,
        WEIGHT,
        TIME
    }
}

