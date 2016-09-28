using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.QuantityResource;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcTypeObject
    {
        public void AddPropertySet(IfcPropertySetDefinition pSetDefinition)
        {
            HasPropertySets.Add(pSetDefinition);
        }

        /// <summary>
        /// Returns the propertyset of the specified name, null if it does not exist
        /// </summary>
        /// <param name="pSetName"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        public  IfcPropertySet GetPropertySet(string pSetName, bool caseSensitive = true)
        {
            if (HasPropertySets == null) return null;
            return caseSensitive ?
                HasPropertySets.FirstOrDefault<IfcPropertySet>(r => r.Name == pSetName) :
                HasPropertySets.FirstOrDefault<IfcPropertySet>(r => string.Equals(r.Name.ToString(), pSetName, StringComparison.CurrentCultureIgnoreCase));
        }

        public IfcPropertySingleValue GetPropertySingleValue(string pSetName, string propertyName)
        {
            var pset = GetPropertySet(pSetName);
            if (pset != null)
                return pset.HasProperties.FirstOrDefault<IfcPropertySingleValue>(p => p.Name == propertyName);
            return null;
        }

        public IfcValue GetPropertySingleValueValue(string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(pSetName, propertyName);
            return psv.NominalValue;
        }

        public IEnumerable<IfcPropertySet> PropertySets
        {
            get
            {                
                if (HasPropertySets != null) return HasPropertySets.OfType<IfcPropertySet>();               
                return Enumerable.Empty<IfcPropertySet>();
            }
        }

        public IDictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>> PropertySingleValues
        {
            get
            {
                var result = new Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>>();
                var pSets = HasPropertySets;
                if (pSets == null) return result;
                var pSetsPure = pSets.OfType<IfcPropertySet>();
                foreach (var pSet in pSetsPure)
                {
                    var value = new Dictionary<IfcIdentifier, IfcValue>();
                    IfcLabel psetName = pSet.Name ?? new IfcLabel("Undefined");
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
        } 

        public IfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, IfcValue value)
        {
            var pset = GetPropertySet(pSetName);
            IfcPropertySingleValue property;
            
            if (pset == null)
            {
                pset = Model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                AddPropertySet(pset);
            }

            //change existing property of the same name from the property set
            var singleVal = GetPropertySingleValue(pSetName, propertyName);
            if (singleVal != null)
            {
                property = singleVal;
                singleVal.NominalValue = value;
            }
            else
            {
                property = Model.Instances.New<IfcPropertySingleValue>(psv => { psv.Name = propertyName; psv.NominalValue = value; });
                pset.HasProperties.Add(property);
            }
            return property;
        }

        public IfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(pSetName);
            return elementQuality != null ? elementQuality.Quantities.FirstOrDefault<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName) : null;
        }


        /// <summary>
        /// Use this to get all physical simple quantities (like length, area, volume, count, etc.)
        /// </summary>
        /// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        public IEnumerable<IfcPhysicalSimpleQuantity> PhysicalSimpleQuantities
        {
            get 
            {
                return ElementQuantities.SelectMany(eq => eq.Quantities).OfType<IfcPhysicalSimpleQuantity>();
            }
        }

        /// <summary>
        /// Use this method to get all element quantities related to this object
        /// </summary>
        /// <returns>All related element quantities</returns>
        public IEnumerable<IfcElementQuantity> ElementQuantities
        {
            get
            {
                if (HasPropertySets != null) return HasPropertySets.OfType<IfcElementQuantity>();
                return Enumerable.Empty<IfcElementQuantity>();
            }
        }

        public  IfcElementQuantity GetElementQuantity(string pSetName, bool caseSensitive = true)
        {
            if (HasPropertySets == null) return null;

            return caseSensitive ?
                HasPropertySets.FirstOrDefault<IfcElementQuantity>(r => r.Name == pSetName) :
                HasPropertySets.FirstOrDefault<IfcElementQuantity>(r => r.Name.ToString().ToLower() == pSetName.ToLower());
        }


      
    }
}
