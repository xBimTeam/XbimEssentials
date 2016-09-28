using System.Linq;
using System.Collections.Generic;
using System;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Common;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.QuantityResource;
namespace Xbim.Ifc4.Kernel
{
    public partial class IfcObject
    {

        /// <summary>
        /// Adds an element type to the object if it doesn't already have one, return the new or existing relationship that holds the type and this element. If there is a relationship for this type but this element is not related it adds it to the exosting relationship
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public IfcRelDefinesByType AddDefiningType(IfcTypeObject theType)
        {
            var typedefs = Model.Instances.Where<IfcRelDefinesByType>(r => r.RelatingType == theType).ToList();
            var thisTypeDef = typedefs.FirstOrDefault(r => r.RelatedObjects.Contains((this)));
            if (thisTypeDef != null) return thisTypeDef; // it is already type related
            var anyTypeDef = typedefs.FirstOrDefault(); //take any one of the rels of the type
            if (anyTypeDef != null)
            {
                anyTypeDef.RelatedObjects.Add(this);
                return anyTypeDef;
            }
            var newdef = Model.Instances.New<IfcRelDefinesByType>(); //create one
            newdef.RelatedObjects.Add(this);
            newdef.RelatingType = theType;
            return newdef;
        }
        /// <summary>
        /// Adds an existing property set to the object, NB no check is done for duplicate psets
        /// </summary>
        /// <param name="pSet"></param>
        public void AddPropertySet(IfcPropertySet pSet)
        {

            var relDef = Model.Instances.OfType<IfcRelDefinesByProperties>().FirstOrDefault(r => pSet.Equals(r.RelatingPropertyDefinition));
            if (relDef == null)
            {

                relDef = Model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pSet;
            }
            relDef.RelatedObjects.Add(this);
        }

        /// <summary>
        /// Returns the propertyset of the specified name, null if it does not exist
        /// </summary>
        /// <param name="pSetName"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        public IIfcPropertySet GetPropertySet(string pSetName, bool caseSensitive = true)
        {
            return PropertySets.FirstOrDefault(pset => string.Compare(pSetName, pset.Name, !caseSensitive) == 0);           
        }
        public IIfcPropertySingleValue GetPropertySingleValue(string pSetName, string propertyName)
        {
            var pset = GetPropertySet(pSetName);
            return pset != null ? pset.HasProperties.OfType<IIfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName) : null;
        }
        public TValueType GetPropertySingleValue<TValueType>(string pSetName, string propertyName) where TValueType : IIfcValue
        {
            var pset = GetPropertySet(pSetName);
            if (pset == null) return default(TValueType);
            var pVal =
                pset.HasProperties.OfType<IIfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName);
            if (pVal != null && pVal.NominalValue is TValueType) return (TValueType)pVal.NominalValue;
            return default(TValueType);
        }

        /// <summary>
        /// If the property value exists, returns the Nominal Value of the contents
        /// </summary>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IIfcValue GetPropertySingleNominalValue(string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(pSetName, propertyName);
            return psv == null ? null : psv.NominalValue;
        }

        public IEnumerable<IIfcPropertySet> PropertySets
        {
            get
            {
                var pSets = IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions);
                return pSets.OfType<IIfcPropertySet>();
               
            }
        }

        

        /// <summary>
        /// Creates property single value with specified type and default value of this type (0 for numeric types, empty string tor string types and false for bool types)
        /// </summary>
        /// <param name="pSetName">Property set name</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="type">Type of the property</param>
        /// <returns>Property single value with default value of the specified type</returns>
        public IIfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, Type type)
        {
            if (typeof(IfcValue).IsAssignableFrom(type))
            {
                IfcValue value;
                if (typeof(IfcPositiveLengthMeasure).IsAssignableFrom(type))
                    value = Activator.CreateInstance(type, 1.0) as IfcValue;
                else
                    value = Activator.CreateInstance(type) as IfcValue;

                if (value != null)
                    return SetPropertySingleValue(pSetName, propertyName, value);
                throw new Exception("Type '" + type.Name + "' can't be initialized.");
            }
            throw new ArgumentException("Type '" + type.Name + "' is not compatible with IfcValue type.");
        }

        public IIfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, IfcValue value)
        {
            var pset = GetPropertySet(pSetName);
            if (pset == null)
            {
                pset = Model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pset;
                relDef.RelatedObjects.Add(this);
            }

            //change existing property of the same name from the property set
            var singleVal = GetPropertySingleValue(pSetName, propertyName);
            if (singleVal != null)
            {
                singleVal.NominalValue = value;
            }
            else
            {

                singleVal = Model.Instances.New<IfcPropertySingleValue>(psv => { psv.Name = propertyName; psv.NominalValue = value; });
                pset.HasProperties.Add(singleVal);
            }

            return singleVal;
        }


        /// <summary>
        /// Returns a list of all the elements that bound the external of the building 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<IIfcElement> GetExternalElements(IModel model)
        {
            return model.Instances.OfType<IIfcRelSpaceBoundary>().Where(r => r.InternalOrExternalBoundary == IfcInternalOrExternalEnum.EXTERNAL
                && r.PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL
                && r.RelatedBuildingElement != null).Select(rsb => rsb.RelatedBuildingElement).Distinct();
        }

        public IIfcElementQuantity GetElementQuantity(string pSetName, bool caseSensitive = true)
        {
            var qSets = IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions).OfType<IfcElementQuantity>();
            return qSets.FirstOrDefault(qset=>string.Compare(pSetName,qset.Name,!caseSensitive)==0);         
        }

        /// <summary>
        /// Use this method to get all element quantities related to this object
        /// </summary>
        /// <returns>All related element quantities</returns>
        public IEnumerable<IIfcElementQuantity> ElementQuantities
        {
            get
            {
                var rels = IsDefinedBy.Where(r => r.RelatingPropertyDefinition is IIfcElementQuantity);
                return rels.Select(rel => rel.RelatingPropertyDefinition as IIfcElementQuantity);
            }
        }

        /// <summary>
        /// Use this to get all physical simple quantities (like length, area, volume, count, etc.)
        /// </summary>
        /// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        public IEnumerable<IIfcPhysicalSimpleQuantity> PhysicalSimpleQuantities
        {
            get
            {
                return ElementQuantities.SelectMany(eq => eq.Quantities).OfType<IIfcPhysicalSimpleQuantity>();
            }
        }

        /// <summary>
        /// Returns the first quantity in the property set pSetName of name qName
        /// </summary>
        /// <typeparam name="TQType"></typeparam>
        /// <param name="pSetName"></param>
        /// <param name="qName"></param>
        /// <returns></returns>
        public TQType GetQuantity<TQType>(string pSetName, string qName) where TQType : IIfcPhysicalQuantity
        {
            var propSets = IsDefinedBy.SelectMany(r=>r.RelatingPropertyDefinition.PropertySetDefinitions);
            var rel = propSets.FirstOrDefault(r =>  r is IfcElementQuantity && r.Name == pSetName);
            if (rel == null) return default(TQType);
            var eQ = rel as IfcElementQuantity;
            return eQ == null ? default(TQType) : eQ.Quantities.OfType<TQType>().FirstOrDefault(q => q.Name == qName);
        }

        /// <summary>
        /// Returns the first quantity that matches the quantity name
        /// </summary>
        /// <typeparam name="TQType"></typeparam>
        /// <param name="qName"></param>
        /// <returns></returns>
        public TQType GetQuantity<TQType>(string qName) where TQType : IIfcPhysicalQuantity
        {
            var qSets = IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions).OfType<IIfcElementQuantity>();
            return qSets.SelectMany(qset=>qset.Quantities).OfType<TQType>().FirstOrDefault(q => q.Name == qName);
        }

        /// <summary>
        /// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        /// </summary>
        /// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        /// <param name="quantity">quantity to be added</param>
        /// <param name="methodOfMeasurement">Sets the method of measurement, if not null overrides previous value</param>
        public IIfcElementQuantity AddQuantity(string propertySetName, IIfcPhysicalQuantity quantity, string methodOfMeasurement)
        {
            var pset = GetElementQuantity(propertySetName);

            if (pset == null)
            {
                pset = Model.Instances.New<IfcElementQuantity>();
                pset.Name = propertySetName;
                var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pset;
                relDef.RelatedObjects.Add(this);
            }
            pset.Quantities.Add(quantity);
            if (!string.IsNullOrEmpty(methodOfMeasurement)) pset.MethodOfMeasurement = methodOfMeasurement;
            return pset;
        }

        /// <summary>
        /// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        /// </summary>
        /// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        /// <param name="quantity">quantity to be added</param>
        public IIfcElementQuantity AddQuantity(string propertySetName, IIfcPhysicalQuantity quantity)
        {
            return AddQuantity(propertySetName, quantity, null);
        }

        /// <summary>
        /// Returns simple physical quality of the element.
        /// </summary>
        /// <param name="pSetName"></param>
        /// <param name="qualityName"></param>
        /// <returns></returns>
        public IIfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(pSetName);
            if (elementQuality != null)
            {
                return elementQuality.Quantities.FirstOrDefault<IIfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName);
            }
            return null;
        }

        public void SetElementPhysicalSimpleQuantity(string qSetName, string qualityName, double value, XbimQuantityTypeEnum quantityType, IIfcNamedUnit unit)
        {


            var qset = GetElementQuantity(qSetName);
            if (qset == null)
            {
                qset = Model.Instances.New<IfcElementQuantity>();
                qset.Name = qSetName;
                var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = qset;
                relDef.RelatedObjects.Add(this);
            }

            //remove existing simple quality
            var simpleQuality = GetElementPhysicalSimpleQuantity(qSetName, qualityName);
            if (simpleQuality != null)
            {
                var elementQuality = GetElementQuantity(qSetName);
                elementQuality.Quantities.Remove(simpleQuality);
                Model.Delete(simpleQuality);
            }

            switch (quantityType)
            {
                case XbimQuantityTypeEnum.Area:
                    simpleQuality = Model.Instances.New<IfcQuantityArea>(sq => sq.AreaValue = (IfcAreaMeasure)value);
                    break;
                case XbimQuantityTypeEnum.Count:
                    simpleQuality = Model.Instances.New<IfcQuantityCount>(sq => sq.CountValue = (IfcCountMeasure)value);
                    break;
                case XbimQuantityTypeEnum.Length:
                    simpleQuality = Model.Instances.New<IfcQuantityLength>(sq => sq.LengthValue = (IfcLengthMeasure)value);
                    break;
                case XbimQuantityTypeEnum.Time:
                    simpleQuality = Model.Instances.New<IfcQuantityTime>(sq => sq.TimeValue = (IfcTimeMeasure)value);
                    break;
                case XbimQuantityTypeEnum.Volume:
                    simpleQuality = Model.Instances.New<IfcQuantityVolume>(sq => sq.VolumeValue = (IfcVolumeMeasure)value);
                    break;
                case XbimQuantityTypeEnum.Weight:
                    simpleQuality = Model.Instances.New<IfcQuantityWeight>(sq => sq.WeightValue = (IfcMassMeasure)value);
                    break;
                default:
                    return;
            }

            simpleQuality.Unit = unit;
            simpleQuality.Name = qualityName;

            qset.Quantities.Add(simpleQuality);
        }

        public void RemovePropertySingleValue(string pSetName, string propertyName)
        {
            var pset = GetPropertySet(pSetName);
            if (pset != null)
            {
                var singleValue = pset.HasProperties.FirstOrDefault<IIfcPropertySingleValue>(p => p.Name == propertyName);
                if (singleValue != null)
                {
                    pset.HasProperties.Remove(singleValue);
                }
            }

        }

        public void RemoveElementPhysicalSimpleQuantity(string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(pSetName);
            if (elementQuality != null)
            {
                var simpleQuality = elementQuality.Quantities.FirstOrDefault<IIfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName);
                if (simpleQuality != null)
                {
                    elementQuality.Quantities.Remove(simpleQuality);
                }
            }
        }
    }

    public enum XbimQuantityTypeEnum
    {
        Length,
        Area,
        Volume,
        Count,
        Weight,
        Time
    }

}
