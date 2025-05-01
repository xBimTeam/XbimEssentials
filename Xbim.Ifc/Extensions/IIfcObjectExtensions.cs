using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcObjectExtensions
    {
        /// <summary>
        /// Adds an element type to the object if it doesn't already have one, return the new or existing relationship that holds the type and this element. 
        /// If there is a relationship for this type but this element is not related it adds it to the existing relationship
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static IIfcRelDefinesByType AddDefiningType(this IIfcObject obj, IIfcTypeObject theType)
        {
            
            var typedefs = obj.Model.Instances.Where<IIfcRelDefinesByType>(r => r.RelatingType == theType).ToList();
            var thisTypeDef = typedefs.FirstOrDefault(r => r.RelatedObjects.Contains(obj));
            if (thisTypeDef != null) return thisTypeDef; // it is already type related
            var anyTypeDef = typedefs.FirstOrDefault(); //take any one of the rels of the type
            if (anyTypeDef != null)
            {
                anyTypeDef.RelatedObjects.Add(obj);
                return anyTypeDef;
            }
            var factory = new EntityCreator(obj.Model);
            var newdef = factory.RelDefinesByType(r => r.RelatingType = theType);
            newdef.RelatedObjects.Add(obj);
            
            return newdef;
        }

        /// <summary>
        /// Adds an existing property set to the object, NB no check is done for duplicate psets
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSet"></param>
        public static void AddPropertySet(this IIfcObject obj, IIfcPropertySet pSet)
        {

            var relDef = obj.Model.Instances.OfType<IIfcRelDefinesByProperties>().FirstOrDefault(r => pSet.Equals(r.RelatingPropertyDefinition));
            if (relDef == null)
            {
                var factory = new EntityCreator(obj.Model);
                relDef = factory.RelDefinesByProperties(r => r.RelatingPropertyDefinition = pSet);
            }
            relDef.RelatedObjects.Add(obj);
        }

        private static IEnumerable<IIfcPropertySet> GetPropertySets(this IIfcObject obj)
        {
            var pSets = obj.IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions);
            return pSets.OfType<IIfcPropertySet>();
        }

        /// <summary>
        /// Returns the propertyset of the specified name, null if it does not exist
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        public static IIfcPropertySet GetPropertySet(this IIfcObject obj, string pSetName, bool caseSensitive = true)
        {
            return obj.GetPropertySets().FirstOrDefault(pset => string.Compare(pSetName, pset.Name, !caseSensitive) == 0);
        }

        /// <summary>
        /// Gets a PropertySet, creating one is does not exist matching the propertySet name
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertySetName"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        private static IIfcPropertySet GetOrCreatePropertySet(this IIfcObject obj, string propertySetName, EntityCreator factory)
        {
            var pset = obj.GetPropertySet(propertySetName);
            if (pset == null)
            {
                pset = factory.PropertySet();
                pset.Name = propertySetName;
                var relDef = factory.RelDefinesByProperties(r =>
                {
                    r.RelatingPropertyDefinition = pset;
                    r.RelatedObjects.Add(obj);
                });
            }

            return pset;
        }

        /// <summary>
        /// Returns the first property matching the pset and property name, null if none exists
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static TProp GetSimpleProperty<TProp>(this IIfcObject obj, string pSetName, string propertyName) where TProp : IIfcSimpleProperty
        {
            var pset = obj.GetPropertySet(pSetName);
            return pset != null ? pset.HasProperties.OfType<TProp>().FirstOrDefault(p => p.Name == propertyName) : default;
        }

        /// <summary>
        /// Returns the first single property matching the pset and property name, null if none exists
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IIfcPropertySingleValue GetPropertySingleValue(this IIfcObject obj, string pSetName, string propertyName)
        {
            return obj.GetSimpleProperty<IIfcPropertySingleValue>(pSetName, propertyName);
        }

        /// <summary>
        /// Returns the first Enumerated Property matching the pset and property name, null if none exists
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IIfcPropertyEnumeratedValue GetPropertyEnumeratedValue(this IIfcObject obj, string pSetName, string propertyName)
        {
            return obj.GetSimpleProperty<IIfcPropertyEnumeratedValue>(pSetName, propertyName);
        }

        /// <summary>
        /// Returns the first Bounded Property matching the pset and property name, null if none exists
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IIfcPropertyBoundedValue GetPropertyBoundedValue(this IIfcObject obj, string pSetName, string propertyName)
        {
            return obj.GetSimpleProperty<IIfcPropertyBoundedValue>(pSetName, propertyName);
        }

        /// <summary>
        /// Returns the first List Property matching the pset and property name, null if none exists
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IIfcPropertyListValue GetPropertyListValue(this IIfcObject obj, string pSetName, string propertyName)
        {
            return obj.GetSimpleProperty<IIfcPropertyListValue>(pSetName, propertyName);
        }


        /// <summary>
        /// Returns the value of the first single property matching the pset and property name, null if none exists
        /// </summary>
        /// <typeparam name="TValueType"></typeparam>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static TValueType GetPropertySingleValue<TValueType>(this IIfcObject obj, string pSetName, string propertyName) where TValueType : IIfcValue
        {
            var pset = obj.GetPropertySet(pSetName);
            if (pset == null) return default;
            var pVal =
                pset.HasProperties.OfType<IIfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName);
            if (pVal != null && pVal.NominalValue is TValueType) return (TValueType)pVal.NominalValue;
            return default;
        }

        /// <summary>
        /// If the property value exists, returns the Nominal Value of the contents
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IIfcValue GetPropertySingleNominalValue(this IIfcObject obj, string pSetName, string propertyName)
        {
            var psv = obj.GetPropertySingleValue(pSetName, propertyName);
            return psv == null ? null : psv.NominalValue;
        }


        /// <summary>
        /// Creates property single value with specified type and default value of this type (0 for numeric types, empty string or string types and false for bool types)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IIfcPropertySingleValue SetPropertySingleValue<T>(this IIfcObject obj, string pSetName, string propertyName) where T : IIfcValue
        {
            return obj.SetPropertySingleValue(pSetName, propertyName, typeof(T));
        }

        /// <summary>
        /// Creates property single value with specified type and default value of this type (0 for numeric types, empty string or string types and false for bool types)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName">Property set name</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="type">Type of the property</param>
        /// <returns>Property single value with default value of the specified type</returns>
        public static IIfcPropertySingleValue SetPropertySingleValue(this IIfcObject obj, string pSetName, string propertyName, Type type)
        {
            if (typeof(IIfcValue).GetTypeInfo().IsAssignableFrom(type))
            {
                IIfcValue value;
                if (typeof(Ifc4.MeasureResource.IfcPositiveLengthMeasure).GetTypeInfo().IsAssignableFrom(type))
                    value = Activator.CreateInstance(type, 1.0) as IIfcValue;
                else
                    value = Activator.CreateInstance(type) as IIfcValue;

                if (value != null)
                    return obj.SetPropertySingleValue(pSetName, propertyName, value);
                throw new Exception("Type '" + type.Name + "' can't be initialized.");
            }
            throw new ArgumentException("Type '" + type.Name + "' is not compatible with IfcValue type.");
        }

        /// <summary>
        /// Creates a property single, or updates an existing matching one with the supplied <see cref="IIfcValue"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IIfcPropertySingleValue SetPropertySingleValue(this IIfcObject obj, string pSetName, string propertyName, IIfcValue value)
        {
            var factory = new EntityCreator(obj.Model);
            IIfcPropertySet pset = obj.GetOrCreatePropertySet(pSetName, factory);

            //change existing property of the same name from the property set
            var singleVal = obj.GetPropertySingleValue(pSetName, propertyName);
            if (singleVal != null)
            {
                singleVal.NominalValue = value;
            }
            else
            {
                singleVal = factory.PropertySingleValue(psv => { psv.Name = propertyName; psv.NominalValue = value; });
                pset.HasProperties.Add(singleVal);
            }

            return singleVal;
        }

        /// <summary>
        /// Creates an enumerated Value property, or updates an existing matching one with the supplied <see cref="IIfcValue"/>s
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IIfcPropertyEnumeratedValue SetPropertyEnumeratedValue<TVal>(this IIfcObject obj, string pSetName, string propertyName, TVal[] values) where TVal : IIfcValue
        {
            var factory = new EntityCreator(obj.Model);
            IIfcPropertySet pset = obj.GetOrCreatePropertySet(pSetName, factory);

            //change existing property of the same name from the property set
            var enumProperty = obj.GetSimpleProperty<IIfcPropertyEnumeratedValue>(pSetName, propertyName);
            if (enumProperty == null)
            {
                enumProperty = factory.PropertyEnumeratedValue(psv => { psv.Name = propertyName; });
                pset.HasProperties.Add(enumProperty);
            }
            foreach (var value in values)
            {
                enumProperty.EnumerationValues.Add(value);
            }

            return enumProperty;
        }

        /// <summary>
        /// Creates an Bounded Value property, or updates an existing matching one with the supplied <see cref="IIfcValue"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <param name="lowerValue"></param>
        /// <param name="upperValue"></param>
        /// <param name="setPointValue"></param>
        /// <returns></returns>
        public static IIfcPropertyBoundedValue SetPropertyBoundedValue(this IIfcObject obj, string pSetName, string propertyName, 
            IIfcValue lowerValue, IIfcValue upperValue, IIfcValue setPointValue)
        {
            var factory = new EntityCreator(obj.Model);
            IIfcPropertySet pset = obj.GetOrCreatePropertySet(pSetName, factory);

            //change existing property of the same name from the property set
            var enumProperty = obj.GetSimpleProperty<IIfcPropertyBoundedValue>(pSetName, propertyName);
            if (enumProperty == null)
            {
                enumProperty = factory.PropertyBoundedValue(psv => { psv.Name = propertyName; });
                pset.HasProperties.Add(enumProperty);
            }
            enumProperty.LowerBoundValue = lowerValue;
            enumProperty.UpperBoundValue = upperValue;
            enumProperty.SetPointValue = setPointValue;

            return enumProperty;
        }

        /// <summary>
        /// Creates an enumerated Value property, or updates an existing matching one with the supplied <see cref="IIfcValue"/>s
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IIfcPropertyListValue SetPropertyListValue<TVal>(this IIfcObject obj, string pSetName, string propertyName, TVal[] values) where TVal : IIfcValue
        {
            var factory = new EntityCreator(obj.Model);
            IIfcPropertySet pset = obj.GetOrCreatePropertySet(pSetName, factory);

            //change existing property of the same name from the property set
            var listProperty = obj.GetSimpleProperty<IIfcPropertyListValue>(pSetName, propertyName);
            if (listProperty == null)
            {
                listProperty = factory.PropertyListValue(psv => { psv.Name = propertyName; });
                pset.HasProperties.Add(listProperty);
            }
            foreach (var value in values)
            {
                listProperty.ListValues.Add(value);
            }

            return listProperty;
        }

        //TODO: would this be more logical on IIfcBuilding not IIfcObject
        /// <summary>
        /// Returns a list of all the elements that bound the external of the building 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<IIfcElement> GetExternalElements(this IIfcObject obj)
        {
            return obj.Model.Instances.OfType<IIfcRelSpaceBoundary>().Where(r => r.InternalOrExternalBoundary == IfcInternalOrExternalEnum.EXTERNAL
                && r.PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL
                && r.RelatedBuildingElement != null).Select(rsb => rsb.RelatedBuildingElement).Distinct();
        }

        public static IIfcElementQuantity GetElementQuantity(this IIfcObject obj, string quantityName, bool caseSensitive = true)
        {
            var qSets = obj.IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions).OfType<IIfcElementQuantity>();
            return qSets.FirstOrDefault(qset => string.Compare(quantityName, qset.Name, !caseSensitive) == 0);
        }

        /// <summary>
        /// Returns the first quantity in the property set pSetName of name qName
        /// </summary>
        /// <typeparam name="TQType"></typeparam>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="quantityName"></param>
        /// <returns></returns>
        public static TQType GetQuantity<TQType>(this IIfcObject obj, string pSetName, string quantityName) where TQType : IIfcPhysicalQuantity
        {
            var propSets = obj.IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions);
            var rel = propSets.FirstOrDefault(r => r is IIfcElementQuantity && r.Name == pSetName);
            if (rel == null) return default;
            var eQ = rel as IIfcElementQuantity;
            return eQ == null ? default : eQ.Quantities.OfType<TQType>().FirstOrDefault(q => q.Name == quantityName);
        }

        /// <summary>
        /// Returns the first quantity that matches the quantity name
        /// </summary>
        /// <typeparam name="TQType"></typeparam>
        /// <param name="obj"></param>
        /// <param name="quantityName"></param>
        /// <returns></returns>
        public static TQType GetQuantity<TQType>(this IIfcObject obj, string quantityName) where TQType : IIfcPhysicalQuantity
        {
            var qSets = obj.IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions).OfType<IIfcElementQuantity>();
            return qSets.SelectMany(qset => qset.Quantities).OfType<TQType>().FirstOrDefault(q => q.Name == quantityName);
        }


        /// <summary>
        /// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="psetName">Name of the IfcElementQuantity property set</param>
        /// <param name="quantity">quantity to be added</param>
        /// <param name="methodOfMeasurement">Sets the method of measurement, if not null overrides previous value</param>
        public static IIfcElementQuantity AddQuantity(this IIfcObject obj, string psetName, IIfcPhysicalQuantity quantity, string methodOfMeasurement = null)
        {
            var pset = obj.GetElementQuantity(psetName);

            if (pset == null)
            {
                var factory = new EntityCreator(obj.Model);
                pset = factory.ElementQuantity(p => p.Name = psetName);
                var relDef = factory.RelDefinesByProperties(r =>
                {
                    r.RelatingPropertyDefinition = pset;
                    r.RelatedObjects.Add(obj);

                });
            }
            pset.Quantities.Add(quantity);
            if (!string.IsNullOrEmpty(methodOfMeasurement)) pset.MethodOfMeasurement = methodOfMeasurement;
            return pset;
        }


        /// <summary>
        /// Returns simple physical quantity of the element.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="quantityName"></param>
        /// <returns></returns>
        public static IIfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(this IIfcObject obj, string pSetName, string quantityName)
        {
            var elementQuality = obj.GetElementQuantity(pSetName);
            if (elementQuality != null)
            {
                return elementQuality.Quantities.FirstOrDefault<IIfcPhysicalSimpleQuantity>(sq => sq.Name == quantityName);
            }
            return null;
        }

        /// <summary>
        /// Removes a matching single value property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="propertyName"></param>
        public static void RemovePropertySingleValue(this IIfcObject obj, string pSetName, string propertyName)
        {
            var pset = obj.GetPropertySet(pSetName);
            if (pset != null)
            {
                var singleValue = pset.HasProperties.FirstOrDefault<IIfcPropertySingleValue>(p => p.Name == propertyName);
                if (singleValue != null)
                {
                    pset.HasProperties.Remove(singleValue);
                }
            }

        }

        /// <summary>
        /// Removes a matching simple quantity
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSetName"></param>
        /// <param name="qualityName"></param>
        public static void RemoveElementPhysicalSimpleQuantity(this IIfcObject obj, string pSetName, string qualityName)
        {
            var elementQuality = obj.GetElementQuantity(pSetName);
            if (elementQuality != null)
            {
                var simpleQuality = elementQuality.Quantities.FirstOrDefault<IIfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName);
                if (simpleQuality != null)
                {
                    elementQuality.Quantities.Remove(simpleQuality);
                }
            }
        }

        /// <summary>
        /// Sets a simple Physical Quantity and value on this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="qSetName"></param>
        /// <param name="quantityName"></param>
        /// <param name="value"></param>
        /// <param name="quantityType"></param>
        /// <param name="unit"></param>
        public static void SetElementPhysicalSimpleQuantity(this IIfcObject obj, string qSetName, string quantityName, double value, XbimQuantityTypeEnum quantityType, 
            IIfcNamedUnit unit)
        {

            var factory = new EntityCreator(obj.Model);
            var qset = obj.GetElementQuantity(qSetName);
            if (qset == null)
            {
                qset = factory.ElementQuantity(q =>
                {
                    q.Name = qSetName;
                });
                var relDef = factory.RelDefinesByProperties(prop =>
                {
                    prop.RelatingPropertyDefinition = qset;
                    prop.RelatedObjects.Add(obj);
                });
                
            }

            //remove any existing matching simple quantity if one exists
            var simpleQuantity = obj.GetElementPhysicalSimpleQuantity(qSetName, quantityName);
            if (simpleQuantity != null)
            {
                var elementQuality = obj.GetElementQuantity(qSetName);
                elementQuality.Quantities.Remove(simpleQuantity);
                obj.Model.Delete(simpleQuantity);
            }

            simpleQuantity = quantityType switch
            {
                XbimQuantityTypeEnum.Area => factory.QuantityArea(sq => sq.AreaValue = (Ifc4.MeasureResource.IfcAreaMeasure)value),
                XbimQuantityTypeEnum.Length => factory.QuantityLength(sq => sq.LengthValue = (Ifc4.MeasureResource.IfcLengthMeasure)value),
                XbimQuantityTypeEnum.Volume => factory.QuantityVolume(sq => sq.VolumeValue = (Ifc4.MeasureResource.IfcVolumeMeasure)value),
                XbimQuantityTypeEnum.Count => factory.QuantityCount(sq => sq.CountValue = (Ifc4.MeasureResource.IfcCountMeasure)value),
                XbimQuantityTypeEnum.Weight => factory.QuantityWeight(sq => sq.WeightValue = (Ifc4.MeasureResource.IfcMassMeasure)value),
                XbimQuantityTypeEnum.Time => factory.QuantityTime(sq => sq.TimeValue = (Ifc4.MeasureResource.IfcTimeMeasure)value),
                _ => default,
                
            };

            if (simpleQuantity == null)
                return;

            simpleQuantity.Unit = unit;
            simpleQuantity.Name = quantityName;

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
