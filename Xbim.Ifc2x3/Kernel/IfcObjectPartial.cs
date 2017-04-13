using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.QuantityResource;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcObject
    {
        public IEnumerable<IfcRelDefinesByProperties> IsDefinedByProperties
        {
            get
            {
                return Model.Instances.Where<IfcRelDefinesByProperties>(r => r.RelatedObjects.Contains(this));
            }
        }

        /// <summary>
        /// Specific type information  that is common to all instances of IfcObject refering to the same type.
        /// </summary>
        /// <returns></returns>
        public IfcTypeObject IsTypedBy
        {
            get
            {
                var def = Model.Instances.OfType<IfcRelDefinesByType>().FirstOrDefault(rd => rd.RelatedObjects.Contains(this));
                return def != null ? def.RelatingType : null;
            }
            set
            {
                //divorce any exisitng related types
                var rels = Model.Instances.Where<IfcRelDefinesByType>(rd => rd.RelatedObjects.Contains(this));
                foreach (var rel in rels)
                {
                    rel.RelatedObjects.Remove(this);
                }
                //find any existing relationships to this type
                var typeRel = Model.Instances.OfType<IfcRelDefinesByType>().FirstOrDefault(rd => rd.RelatingType == value);
                if (typeRel == null) //none defined create the relationship
                {
                    var relSub = Model.Instances.New<IfcRelDefinesByType>();
                    relSub.RelatingType = value;
                    relSub.RelatedObjects.Add(this);
                }
                else //we have the type
                    typeRel.RelatedObjects.Add(this);

            }
        }

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
        /// Adds an existing property set to the objecty, NB no check is done for duplicate psets
        /// </summary>
        /// <param name="pSet"></param>
        public void AddPropertySet(IfcPropertySet pSet)
        {

            var relDef = Model.Instances.OfType<IfcRelDefinesByProperties>().FirstOrDefault(r => r.RelatingPropertyDefinition == pSet);
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
        public IfcPropertySet GetPropertySet(string pSetName, bool caseSensitive = true)
        {
            return PropertySets.FirstOrDefault(ps=>string.Compare(ps.Name,pSetName,!caseSensitive)==0);
        }

        public IfcPropertySingleValue GetPropertySingleValue(string pSetName, string propertyName)
        {
            var pset = GetPropertySet(pSetName);
            return pset != null ? pset.HasProperties.OfType<IfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName) : null;
        }
        public TValueType GetPropertySingleValue<TValueType>(string pSetName, string propertyName) where TValueType : Ifc4.Interfaces.IIfcValue
        {
            var pset = GetPropertySet(pSetName);
            if (pset == null) return default(TValueType);
            var pVal =
                pset.HasProperties.OfType<IfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName);
            if (pVal != null && pVal.NominalValue is TValueType) return (TValueType)pVal.NominalValue;
            return default(TValueType);
        }

        /// <summary>
        /// If the property value exists, returns the Nominal Value of the contents
        /// </summary>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IfcValue GetPropertySingleNominalValue(string pSetName, string propertyName)
        {
            var psv = GetPropertySingleValue(pSetName, propertyName);
            return psv == null ? null : psv.NominalValue;
        }

        public IEnumerable<IfcPropertySet> PropertySets
        {
            get
            {              
                return IsDefinedByProperties.Select(rel => rel.RelatingPropertyDefinition).OfType<IfcPropertySet>();
            }
        }
   

        /// <summary>
        /// Creates property single value with specified type and default value of this type (0 for numeric types, empty string tor string types and false for bool types)
        /// </summary>
        /// <param name="pSetName">Property set name</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="type">Type of the property</param>
        /// <returns>Property single value with default value of the specified type</returns>
        public IfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, Type type)
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

        public IfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, IfcValue value)
        {
            var pset = GetPropertySet(pSetName) as IfcPropertySet;
            if (pset == null)
            {
                pset = Model.Instances.New<IfcPropertySet>();
                pset.Name = pSetName;
                var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = pset;
                relDef.RelatedObjects.Add(this);
            }

            //change existing property of the same name from the property set
            var singleVal = GetPropertySingleValue(pSetName, propertyName) as IfcPropertySingleValue;
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
        static public IEnumerable<Ifc4.Interfaces.IIfcElement> GetExternalElements(IModel model)
        {
            return model.Instances.OfType<Ifc4.Interfaces.IIfcRelSpaceBoundary>().Where(r => r.InternalOrExternalBoundary == Ifc4.Interfaces.IfcInternalOrExternalEnum.EXTERNAL
                && r.PhysicalOrVirtualBoundary == Ifc4.Interfaces.IfcPhysicalOrVirtualEnum.PHYSICAL
                && r.RelatedBuildingElement != null).Select(rsb => rsb.RelatedBuildingElement).Distinct();
        }

        public Ifc4.Interfaces.IIfcElementQuantity GetElementQuantity(string pSetName, bool caseSensitive = true)
        {
            IfcRelDefinesByProperties rel = caseSensitive ?
                IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is Ifc4.Interfaces.IIfcElementQuantity)
                : IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name.ToString().ToLower() == pSetName.ToLower() && r.RelatingPropertyDefinition is Ifc4.Interfaces.IIfcElementQuantity);
            if (rel != null) return rel.RelatingPropertyDefinition as Ifc4.Interfaces.IIfcElementQuantity;
            return null;
        }

        /// <summary>
        /// Use this method to get all element quantities related to this object
        /// </summary>
        /// <returns>All related element quantities</returns>
        public IEnumerable<Ifc4.Interfaces.IIfcElementQuantity> ElementQuantities
        {
            get
            {
                var rels = IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition is Ifc4.Interfaces.IIfcElementQuantity);
                return rels.Select(rel => rel.RelatingPropertyDefinition as Ifc4.Interfaces.IIfcElementQuantity);
            }
        }

        /// <summary>
        /// Use this to get all physical simple quantities (like length, area, volume, count, etc.)
        /// </summary>
        /// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        public IEnumerable<Ifc4.Interfaces.IIfcPhysicalSimpleQuantity> PhysicalSimpleQuantities
        {
            get
            {
                return ElementQuantities.SelectMany(eq => eq.Quantities).OfType<Ifc4.Interfaces.IIfcPhysicalSimpleQuantity>();
            }
        }

        /// <summary>
        /// Returns the first quantity in the property set pSetName of name qName
        /// </summary>
        /// <typeparam name="TQType"></typeparam>
        /// <param name="pSetName"></param>
        /// <param name="qName"></param>
        /// <returns></returns>
        public TQType GetQuantity<TQType>(string pSetName, string qName) where TQType : Ifc4.Interfaces.IIfcPhysicalQuantity
        {
            var rel = IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is IfcElementQuantity);
            if (rel == null) return default(TQType);
            var eQ = rel.RelatingPropertyDefinition as Ifc4.Interfaces.IIfcElementQuantity;
            return eQ == null ? default(TQType) : eQ.Quantities.OfType<TQType>().FirstOrDefault(q => q.Name == qName);
        }

        /// <summary>
        /// Returns the first quantity that matches the quantity name
        /// </summary>
        /// <typeparam name="TQType"></typeparam>
        /// <param name="qName"></param>
        /// <returns></returns>
        public TQType GetQuantity<TQType>(string qName) where TQType : Ifc4.Interfaces.IIfcPhysicalQuantity
        {
            var rel = IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition is Ifc4.Interfaces.IIfcElementQuantity);
            if (rel == null) return default(TQType);
            var eQ = rel.RelatingPropertyDefinition as Ifc4.Interfaces.IIfcElementQuantity;
            return eQ != null ? eQ.Quantities.OfType<TQType>().FirstOrDefault(q => q.Name == qName) : default(TQType);
        }

        /// <summary>
        /// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        /// </summary>
        /// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        /// <param name="quantity">quantity to be added</param>
        /// <param name="methodOfMeasurement">Sets the method of measurement, if not null overrides previous value</param>
        public IfcElementQuantity AddQuantity(string propertySetName, IfcPhysicalQuantity quantity, string methodOfMeasurement)
        {
            var pset = GetElementQuantity(propertySetName) as IfcElementQuantity;

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
        public IfcElementQuantity AddQuantity(string propertySetName, IfcPhysicalQuantity quantity)
        {
            return AddQuantity(propertySetName, quantity, null);
        }

        /// <summary>
        /// Returns simple physical quality of the element.
        /// </summary>
        /// <param name="pSetName"></param>
        /// <param name="qualityName"></param>
        /// <returns></returns>
        public Xbim.Ifc4.Interfaces.IIfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(string pSetName, string qualityName)
        {
            var elementQuality = GetElementQuantity(pSetName) as IfcElementQuantity;
            if (elementQuality != null)
            {
                return elementQuality.Quantities.FirstOrDefault<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName);
            }
            return null;
        }

        public void SetElementPhysicalSimpleQuantity(string qSetName, string qualityName, double value, XbimQuantityTypeEnum quantityType, IfcNamedUnit unit)
        {


            var qset = GetElementQuantity(qSetName)as IfcElementQuantity;
            if (qset == null)
            {
                qset = Model.Instances.New<IfcElementQuantity>();
                qset.Name = qSetName;
                var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
                relDef.RelatingPropertyDefinition = qset;
                relDef.RelatedObjects.Add(this);
            }

            //remove existing simple quality
            var simpleQuantity = GetElementPhysicalSimpleQuantity(qSetName, qualityName) as IfcPhysicalSimpleQuantity;
            if (simpleQuantity == null)
            {
                switch (quantityType)
                {
                    case XbimQuantityTypeEnum.Area:
                        simpleQuantity = Model.Instances.New<IfcQuantityArea>(sq => sq.AreaValue = (IfcAreaMeasure)value);
                        break;
                    case XbimQuantityTypeEnum.Count:
                        simpleQuantity = Model.Instances.New<IfcQuantityCount>(sq => sq.CountValue = (IfcCountMeasure)value);
                        break;
                    case XbimQuantityTypeEnum.Length:
                        simpleQuantity = Model.Instances.New<IfcQuantityLength>(sq => sq.LengthValue = (IfcLengthMeasure)value);
                        break;
                    case XbimQuantityTypeEnum.Time:
                        simpleQuantity = Model.Instances.New<IfcQuantityTime>(sq => sq.TimeValue = (IfcTimeMeasure)value);
                        break;
                    case XbimQuantityTypeEnum.Volume:
                        simpleQuantity = Model.Instances.New<IfcQuantityVolume>(sq => sq.VolumeValue = (IfcVolumeMeasure)value);
                        break;
                    case XbimQuantityTypeEnum.Weight:
                        simpleQuantity = Model.Instances.New<IfcQuantityWeight>(sq => sq.WeightValue = (IfcMassMeasure)value);
                        break;
                    default:
                        return;
                }
            }
            else
            {
                switch (quantityType)
                {
                    case XbimQuantityTypeEnum.Area:
                        ((IfcQuantityArea)simpleQuantity).AreaValue = new IfcAreaMeasure(value);
                        break;
                    case XbimQuantityTypeEnum.Count:
                        ((IfcQuantityCount)simpleQuantity).CountValue = new IfcCountMeasure(value);
                        break;
                    case XbimQuantityTypeEnum.Length:
                        ((IfcQuantityLength)simpleQuantity).LengthValue = new IfcLengthMeasure(value);                       
                        break;
                    case XbimQuantityTypeEnum.Time:
                        ((IfcQuantityTime)simpleQuantity).TimeValue = new IfcTimeMeasure(value);                            
                        break;
                    case XbimQuantityTypeEnum.Volume:
                        ((IfcQuantityVolume)simpleQuantity).VolumeValue = new IfcVolumeMeasure(value);     
                        break;
                    case XbimQuantityTypeEnum.Weight:
                        ((IfcQuantityWeight)simpleQuantity).WeightValue = new IfcMassMeasure(value); 
                        break;
                    default:
                        return;
                }
            }
            simpleQuantity.Unit = unit;
            simpleQuantity.Name = qualityName;
            qset.Quantities.Add(simpleQuantity);
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
