#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    TypeObjectExtensions.cs
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

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class TypeObjectExtensions
    {
        /// <summary>
        /// Returns the propertyset of the specified name, null if it does not exist
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <returns></returns>
        public static IfcPropertySet GetPropertySet(this IfcTypeObject obj, string pSetName, bool caseSensitive = true)
        {
            if (obj.HasPropertySets == null) return null;
            else return caseSensitive ? 
                obj.HasPropertySets.Where<IfcPropertySet>(r => r.Name == pSetName).FirstOrDefault() :
                obj.HasPropertySets.Where<IfcPropertySet>(r => r.Name.ToString().ToLower() == pSetName.ToLower()).FirstOrDefault();
        }

        public static IfcPropertySingleValue GetPropertySingleValue(this IfcTypeObject obj, string pSetName, string propertyName)
        {
            var pset = GetPropertySet(obj, pSetName);
            if (pset != null)
                return pset.HasProperties.Where<IfcPropertySingleValue>(p => p.Name == propertyName).FirstOrDefault();
            return null;
        }

        public static IfcValue GetPropertySingleValueValue(this IfcTypeObject obj, string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(obj, pSetName, propertyName);
            return psv.NominalValue;
        }

        public static List<IfcPropertySet> GetAllPropertySets(this IfcTypeObject obj)
        {
            var result = new List<IfcPropertySet>();
            if (obj.HasPropertySets != null)
            {
                foreach (var def in obj.HasPropertySets)
                {
                    if (def is IfcPropertySet) result.Add(def as IfcPropertySet);
                }
            }
            return result;
        }

        public static Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>> GetAllPropertySingleValues(this IfcTypeObject obj)
        {
            var result = new Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>>();
            var pSets = obj.HasPropertySets;
            if (pSets == null) return result;
            var pSetsPure = pSets.OfType<IfcPropertySet>();
            foreach (var pSet in pSetsPure)
            {
                var value = new Dictionary<IfcIdentifier, IfcValue>();
                var psetName = pSet.Name ?? null;
                foreach (var prop in pSet.HasProperties)
                {
                    var singleVal = prop as IfcPropertySingleValue;
                    if (singleVal == null) continue;
                    value.Add(prop.Name, singleVal.NominalValue);
                }
                result.Add(psetName, value);
            }
            return result;
        }

        public static void DeletePropertySingleValueValue(this IfcTypeObject obj, string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(obj, pSetName, propertyName);
            if (psv != null) psv.NominalValue = null;
        }

        public static IfcPropertyTableValue GetPropertyTableValue(this IfcTypeObject obj, string pSetName, string propertyTableName)
        {
            var pset = GetPropertySet(obj, pSetName);
            if (pset != null)
                return pset.HasProperties.Where<IfcPropertyTableValue>(p => p.Name == propertyTableName).FirstOrDefault();
            return null;
        }

        public static IfcValue GetPropertyTableItemValue(this IfcTypeObject obj, string pSetName, string propertyTableName, IfcValue definingValue)
        {
            var table = GetPropertyTableValue(obj, pSetName, propertyTableName);
            if (table == null) return null;
            var definingValues = table.DefiningValues;
            if (definingValues == null) return null;
            if (!definingValues.Contains(definingValue)) return null;
            var index = definingValues.IndexOf(definingValue);

            if (table.DefinedValues.Count < index + 1) return null;
            return table.DefinedValues[index];
        }

        public static void SetPropertyTableItemValue(this IfcTypeObject obj, string pSetName, string propertyTableName, IfcValue definingValue, IfcValue definedValue)
        {
            SetPropertyTableItemValue(obj, pSetName, propertyTableName, definingValue, definedValue, null, null);
        }

        public static void SetPropertyTableItemValue(this IfcTypeObject obj, string pSetName, string propertyTableName, IfcValue definingValue, IfcValue definedValue, IfcUnit definingUnit, IfcUnit definedUnit)
        {
            var pset = GetPropertySet(obj, pSetName);
            IModel model;
            if (pset == null)
            {
                var ent = obj as IPersistEntity;
                model = ent != null ? ent.Model : obj.Model;
                pset = model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                obj.AddPropertySet(pset);
            }
            var table = GetPropertyTableValue(obj, pSetName, propertyTableName);
            if (table == null)
            {
                var ent = obj as IPersistEntity;
                model = ent != null ? ent.Model : obj.Model;
                table = model.Instances.New<IfcPropertyTableValue>(tb => { tb.Name = propertyTableName; });
                pset.HasProperties.Add(table);
                table.DefinedUnit = definedUnit;
                table.DefiningUnit = definingUnit;
            }
            if (table.DefiningUnit != definingUnit || table.DefinedUnit != definedUnit)
                throw new Exception("Inconsistent definition of the units in the property table.");

            var itemIndex = table.DefiningValues.IndexOf(definingValue);
            //check if defining value is not defined already
            if (itemIndex >= 0)
            {
                table.DefinedValues[itemIndex] = definedValue;
            }
            else
            {
                table.DefiningValues.Add(definingValue);
                table.DefinedValues.Add(definedValue);

                //check of integrity
                if (table.DefinedValues.Count != table.DefiningValues.Count)
                    throw new Exception("Inconsistent state of the property table. Number of defined and defining values are not the same.");
            }
        }

        public static IfcPropertySingleValue SetPropertySingleValue(this IfcTypeObject obj, string pSetName, string propertyName, IfcValue value)
        {
            var pset = GetPropertySet(obj, pSetName);
            IfcPropertySingleValue property;
            IModel model;
            if (pset == null)
            {
                //if (value == null) return;
                var ent = obj as IPersistEntity;
                model = ent!= null? ent.Model : obj.Model;
                pset = model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                obj.AddPropertySet(pset);
            }

            //change existing property of the same name from the property set
            var singleVal = GetPropertySingleValue(obj, pSetName, propertyName);
            if (singleVal != null)
            {
                property = singleVal;
                singleVal.NominalValue = value;
            }
            else
            {
                //if (value == null) return;
                var ent = obj as IPersistEntity;
                model = ent != null ? ent.Model : obj.Model;
                property = model.Instances.New<IfcPropertySingleValue>(psv => { psv.Name = propertyName; psv.NominalValue = value; });
                pset.HasProperties.Add(property);
            }
            return property;
        }

        public static IfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(this IfcTypeObject elem, string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(elem, pSetName);
            if (elementQuality != null)
            {
                return elementQuality.Quantities.Where<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName).FirstOrDefault();
            }
            else
                return null;
        }


        /// <summary>
        /// Use this to get all physical simple quantities (like length, area, volume, count, etc.)
        /// </summary>
        /// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        public static IEnumerable<IfcPhysicalSimpleQuantity> GetAllPhysicalSimpleQuantities(this IfcTypeObject elem)
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
        /// Use this method to get all element quantities related to this object
        /// </summary>
        /// <returns>All related element quantities</returns>
        public static IEnumerable<IfcElementQuantity> GetAllElementQuantities(this IfcTypeObject elem)
        {
            if (elem.HasPropertySets != null)
                return elem.HasPropertySets.OfType<IfcElementQuantity>();
            return new IfcElementQuantity[] { };
        }

        public static IfcElementQuantity GetElementQuantity(this IfcTypeObject elem, string pSetName, bool caseSensitive = true )
        {
            if (elem.HasPropertySets == null) return null;
            
            return caseSensitive ?
                elem.HasPropertySets.FirstOrDefault<IfcElementQuantity>(r => r.Name == pSetName) :
                elem.HasPropertySets.FirstOrDefault<IfcElementQuantity>(r => r.Name.ToString().ToLower() == pSetName.ToLower())
                ;
        }

        public static void RemoveElementPhysicalSimpleQuantity(this IfcTypeObject elem, string pSetName, string qualityName)
        {
            var elementQuantity = GetElementQuantity(elem, pSetName);
            if (elementQuantity != null)
            {
                var simpleQuantity = elementQuantity.Quantities.FirstOrDefault(sq => sq.Name == qualityName);
                if (simpleQuantity != null)
                {
                    elementQuantity.Quantities.Remove(simpleQuantity);
                }
            }
        }

        public static void SetElementPhysicalSimpleQuantity(this IfcTypeObject elem, string qSetName, string qualityName, double value, XbimQuantityTypeEnum quantityType, IfcNamedUnit unit)
        {
            var model = elem.Model;

            var qset = GetElementQuantity(elem, qSetName);
            if (qset == null)
            {
                qset = model.Instances.New<IfcElementQuantity>();
                qset.Name = qSetName;
                if (elem.HasPropertySets == null) 
                    throw new NullReferenceException("HasPropertySets is not initialized");
                    //elem.CreateHasPropertySets();
                elem.HasPropertySets.Add(qset);
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
                    simpleQuality = model.Instances.New<IfcQuantityArea>(sq => sq.AreaValue = value);
                    break;
                case XbimQuantityTypeEnum.COUNT:
                    simpleQuality = model.Instances.New<IfcQuantityCount>(sq => sq.CountValue = value);
                    break;
                case XbimQuantityTypeEnum.LENGTH:
                    simpleQuality = model.Instances.New<IfcQuantityLength>(sq => sq.LengthValue = value);
                    break;
                case XbimQuantityTypeEnum.TIME:
                    simpleQuality = model.Instances.New<IfcQuantityTime>(sq => sq.TimeValue = value);
                    break;
                case XbimQuantityTypeEnum.VOLUME:
                    simpleQuality = model.Instances.New<IfcQuantityVolume>(sq => sq.VolumeValue = value);
                    break;
                case XbimQuantityTypeEnum.WEIGHT:
                    simpleQuality = model.Instances.New<IfcQuantityWeight>(sq => sq.WeightValue = value);
                    break;
                default:
                    return;
            }

            simpleQuality.Unit = unit;
            simpleQuality.Name = qualityName;

            qset.Quantities.Add(simpleQuality);
        }
    }
}
