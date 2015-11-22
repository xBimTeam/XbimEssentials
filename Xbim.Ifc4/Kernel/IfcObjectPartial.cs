namespace Xbim.Ifc4.Kernel
{
    public partial class IfcObject
    {

        #region Property Sets, this needs to be amended to wotk with Ifc4

        ///// <summary>
        ///// Adds an existing property set to the objecty, NB no check is done for duplicate psets
        ///// </summary>
        ///// <param name="pSet"></param>
        //public void AddPropertySet(IfcPropertySet pSet)
        //{

        //    var relDef = Model.Instances.OfType<IfcRelDefinesByProperties>().FirstOrDefault(r => r.RelatingPropertyDefinition == pSet);
        //    if (relDef == null)
        //    {

        //        relDef = Model.Instances.New<IfcRelDefinesByProperties>();
        //        relDef.RelatingPropertyDefinition = pSet;
        //    }
        //    relDef.RelatedObjects.Add(this);
        //}

        ///// <summary>
        ///// Returns the propertyset of the specified name, null if it does not exist
        ///// </summary>
        ///// <param name="pSetName"></param>
        ///// <param name="caseSensitive"></param>
        ///// <returns></returns>
        //public IfcPropertySet GetPropertySet(string pSetName)
        //{
        //    var propertySets = new List<IfcPropertySet>();
        //    foreach (var relDefinesByPorperties in IsDefinedByProperties)
        //    {
        //        var propertySet = relDefinesByPorperties.RelatingPropertyDefinition as IfcPropertySet;
        //        if( propertySet!=null)
        //    }
        //    var propSetSelect = IsDefinedByProperties.Select(r => r.RelatingPropertyDefinition).ToList();

        //    IfcRelDefinesByProperties rel = caseSensitive ?
        //        IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName)
        //        : IsDefinedByProperties.FirstOrDefault(r => string.Equals(r.RelatingPropertyDefinition.Name.ToString(), pSetName, StringComparison.CurrentCultureIgnoreCase));
        //    if (rel != null) return rel.RelatingPropertyDefinition as IfcPropertySet;
        //    return null;
        //}
        //public IfcPropertySingleValue GetPropertySingleValue(string pSetName, string propertyName)
        //{
        //    var pset = GetPropertySet(pSetName);
        //    return pset != null ? pset.HasProperties.OfType<IfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName) : null;
        //}
        //public TValueType GetPropertySingleValue<TValueType>(string pSetName, string propertyName) where TValueType : IfcValue
        //{
        //    var pset = GetPropertySet(pSetName);
        //    if (pset == null) return default(TValueType);
        //    var pVal =
        //        pset.HasProperties.OfType<IfcPropertySingleValue>().FirstOrDefault(p => p.Name == propertyName);
        //    if (pVal != null && pVal.NominalValue is TValueType) return (TValueType)pVal.NominalValue;
        //    return default(TValueType);
        //}

        ///// <summary>
        ///// If the property value exists, returns the Nominal Value of the contents
        ///// </summary>
        ///// <param name="pSetName"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //public IfcValue GetPropertySingleNominalValue(string pSetName, string propertyName)
        //{
        //    var psv = GetPropertySingleValue(pSetName, propertyName);
        //    return psv == null ? null : psv.NominalValue;
        //}

        //public IEnumerable<IfcPropertySet> PropertySets
        //{
        //    get
        //    {
        //        IEnumerable<IfcRelDefinesByProperties> rels = IsDefinedByProperties;
        //        return rels.Select(rel => rel.RelatingPropertyDefinition).OfType<IfcPropertySet>();
        //    }
        //}

        //public IDictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>> PropertySingleValues
        //{
        //    get
        //    {
        //        var result = new Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>>();
        //        foreach (var rel in IsDefinedByProperties)
        //        {

        //            var psetName = rel.RelatingPropertyDefinition.Name;
        //            var pSet = rel.RelatingPropertyDefinition as IfcPropertySet;
        //            if (pSet == null || psetName == null) continue;
        //            Dictionary<IfcIdentifier, IfcValue> values;
        //            if (!result.TryGetValue(psetName.Value, out values))
        //            {
        //                values = new Dictionary<IfcIdentifier, IfcValue>();
        //                result.Add(psetName.Value, values);
        //            }
        //            foreach (var prop in pSet.HasProperties)
        //            {
        //                var singleVal = prop as IfcPropertySingleValue;
        //                if (singleVal == null) continue;
        //                if (!values.ContainsKey(prop.Name))
        //                    values.Add(prop.Name, singleVal.NominalValue);
        //            }

        //        }
        //        return result;
        //    }
        //}


        ///// <summary>
        ///// Creates property single value with specified type and default value of this type (0 for numeric types, empty string tor string types and false for bool types)
        ///// </summary>
        ///// <param name="pSetName">Property set name</param>
        ///// <param name="propertyName">Property name</param>
        ///// <param name="type">Type of the property</param>
        ///// <returns>Property single value with default value of the specified type</returns>
        //public IfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, Type type)
        //{
        //    if (typeof(IfcValue).IsAssignableFrom(type))
        //    {
        //        IfcValue value;
        //        if (typeof(IfcPositiveLengthMeasure).IsAssignableFrom(type))
        //            value = Activator.CreateInstance(type, 1.0) as IfcValue;
        //        else
        //            value = Activator.CreateInstance(type) as IfcValue;

        //        if (value != null)
        //            return SetPropertySingleValue(pSetName, propertyName, value);
        //        throw new Exception("Type '" + type.Name + "' can't be initialized.");
        //    }
        //    throw new ArgumentException("Type '" + type.Name + "' is not compatible with IfcValue type.");
        //}

        //public IfcPropertySingleValue SetPropertySingleValue(string pSetName, string propertyName, IfcValue value)
        //{
        //    var pset = GetPropertySet(pSetName);
        //    if (pset == null)
        //    {
        //        pset = Model.Instances.New<IfcPropertySet>();
        //        pset.Name = pSetName;
        //        var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
        //        relDef.RelatingPropertyDefinition = pset;
        //        relDef.RelatedObjects.Add(this);
        //    }

        //    //change existing property of the same name from the property set
        //    var singleVal = GetPropertySingleValue(pSetName, propertyName);
        //    if (singleVal != null)
        //    {
        //        singleVal.NominalValue = value;
        //    }
        //    else
        //    {

        //        singleVal = Model.Instances.New<IfcPropertySingleValue>(psv => { psv.Name = propertyName; psv.NominalValue = value; });
        //        pset.HasProperties.Add(singleVal);
        //    }

        //    return singleVal;
        //}


        ///// <summary>
        ///// Returns a list of all the elements that bound the external of the building 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //static public IEnumerable<IfcElement> GetExternalElements(IModel model)
        //{
        //    return model.Instances.OfType<IfcRelSpaceBoundary>().Where(r => r.InternalOrExternalBoundary == IfcInternalOrExternalEnum.EXTERNAL
        //        && r.PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL
        //        && r.RelatedBuildingElement != null).Select(rsb => rsb.RelatedBuildingElement).Distinct();
        //}

        //public IfcElementQuantity GetElementQuantity(string pSetName, bool caseSensitive = true)
        //{
        //    IfcRelDefinesByProperties rel = caseSensitive ?
        //        IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is IfcElementQuantity)
        //        : IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name.ToString().ToLower() == pSetName.ToLower() && r.RelatingPropertyDefinition is IfcElementQuantity);
        //    if (rel != null) return rel.RelatingPropertyDefinition as IfcElementQuantity;
        //    return null;
        //}

        ///// <summary>
        ///// Use this method to get all element quantities related to this object
        ///// </summary>
        ///// <returns>All related element quantities</returns>
        //public IEnumerable<IfcElementQuantity> ElementQuantities
        //{
        //    get
        //    {
        //        var rels = IsDefinedByProperties.Where(r => r.RelatingPropertyDefinition is IfcElementQuantity);
        //        return rels.Select(rel => rel.RelatingPropertyDefinition as IfcElementQuantity);
        //    }
        //}

        ///// <summary>
        ///// Use this to get all physical simple quantities (like length, area, volume, count, etc.)
        ///// </summary>
        ///// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        //public IEnumerable<IfcPhysicalSimpleQuantity> PhysicalSimpleQuantities
        //{
        //    get
        //    {
        //        return ElementQuantities.SelectMany(eq => eq.Quantities).OfType<IfcPhysicalSimpleQuantity>();
        //    }
        //}

        ///// <summary>
        ///// Returns the first quantity in the property set pSetName of name qName
        ///// </summary>
        ///// <typeparam name="TQType"></typeparam>
        ///// <param name="pSetName"></param>
        ///// <param name="qName"></param>
        ///// <returns></returns>
        //public TQType GetQuantity<TQType>(string pSetName, string qName) where TQType : IfcPhysicalQuantity
        //{
        //    var rel = IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is IfcElementQuantity);
        //    if (rel == null) return default(TQType);
        //    var eQ = rel.RelatingPropertyDefinition as IfcElementQuantity;
        //    return eQ == null ? default(TQType) : eQ.Quantities.OfType<TQType>().FirstOrDefault(q => q.Name == qName);
        //}

        ///// <summary>
        ///// Returns the first quantity that matches the quantity name
        ///// </summary>
        ///// <typeparam name="TQType"></typeparam>
        ///// <param name="qName"></param>
        ///// <returns></returns>
        //public TQType GetQuantity<TQType>(string qName) where TQType : IfcPhysicalQuantity
        //{
        //    var rel = IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition is IfcElementQuantity);
        //    if (rel == null) return default(TQType);
        //    var eQ = rel.RelatingPropertyDefinition as IfcElementQuantity;
        //    return eQ != null ? eQ.Quantities.OfType<TQType>().FirstOrDefault(q => q.Name == qName) : default(TQType);
        //}

        ///// <summary>
        ///// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        ///// </summary>
        ///// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        ///// <param name="quantity">quantity to be added</param>
        ///// <param name="methodOfMeasurement">Sets the method of measurement, if not null overrides previous value</param>
        //public IfcElementQuantity AddQuantity(string propertySetName, IfcPhysicalQuantity quantity, string methodOfMeasurement)
        //{
        //    var pset = GetElementQuantity(propertySetName);

        //    if (pset == null)
        //    {
        //        pset = Model.Instances.New<IfcElementQuantity>();
        //        pset.Name = propertySetName;
        //        var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
        //        relDef.RelatingPropertyDefinition = pset;
        //        relDef.RelatedObjects.Add(this);
        //    }
        //    pset.Quantities.Add(quantity);
        //    if (!string.IsNullOrEmpty(methodOfMeasurement)) pset.MethodOfMeasurement = methodOfMeasurement;
        //    return pset;
        //}

        ///// <summary>
        ///// Adds a new IfcPhysicalQuantity to the IfcElementQuantity called propertySetName
        ///// </summary>
        ///// <param name="propertySetName">Name of the IfcElementQuantity property set</param>
        ///// <param name="quantity">quantity to be added</param>
        //public IfcElementQuantity AddQuantity(string propertySetName, IfcPhysicalQuantity quantity)
        //{
        //    return AddQuantity(propertySetName, quantity, null);
        //}

        ///// <summary>
        ///// Returns simple physical quality of the element.
        ///// </summary>
        ///// <param name="pSetName"></param>
        ///// <param name="qualityName"></param>
        ///// <returns></returns>
        //public IfcPhysicalSimpleQuantity GetElementPhysicalSimpleQuantity(string pSetName, string qualityName)
        //{
        //    var elementQuality = GetElementQuantity(pSetName);
        //    if (elementQuality != null)
        //    {
        //        return elementQuality.Quantities.FirstOrDefault<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName);
        //    }
        //    return null;
        //}

        //public void SetElementPhysicalSimpleQuantity(string qSetName, string qualityName, double value, XbimQuantityTypeEnum quantityType, IfcNamedUnit unit)
        //{


        //    var qset = GetElementQuantity(qSetName);
        //    if (qset == null)
        //    {
        //        qset = Model.Instances.New<IfcElementQuantity>();
        //        qset.Name = qSetName;
        //        var relDef = Model.Instances.New<IfcRelDefinesByProperties>();
        //        relDef.RelatingPropertyDefinition = qset;
        //        relDef.RelatedObjects.Add(this);
        //    }

        //    //remove existing simple quality
        //    var simpleQuality = GetElementPhysicalSimpleQuantity(qSetName, qualityName);
        //    if (simpleQuality != null)
        //    {
        //        var elementQuality = GetElementQuantity(qSetName);
        //        elementQuality.Quantities.Remove(simpleQuality);
        //        Model.Delete(simpleQuality);
        //    }

        //    switch (quantityType)
        //    {
        //        case XbimQuantityTypeEnum.Area:
        //            simpleQuality = Model.Instances.New<IfcQuantityArea>(sq => sq.AreaValue = (IfcAreaMeasure)value);
        //            break;
        //        case XbimQuantityTypeEnum.Count:
        //            simpleQuality = Model.Instances.New<IfcQuantityCount>(sq => sq.CountValue = (IfcCountMeasure)value);
        //            break;
        //        case XbimQuantityTypeEnum.Length:
        //            simpleQuality = Model.Instances.New<IfcQuantityLength>(sq => sq.LengthValue = (IfcLengthMeasure)value);
        //            break;
        //        case XbimQuantityTypeEnum.Time:
        //            simpleQuality = Model.Instances.New<IfcQuantityTime>(sq => sq.TimeValue = (IfcTimeMeasure)value);
        //            break;
        //        case XbimQuantityTypeEnum.Volume:
        //            simpleQuality = Model.Instances.New<IfcQuantityVolume>(sq => sq.VolumeValue = (IfcVolumeMeasure)value);
        //            break;
        //        case XbimQuantityTypeEnum.Weight:
        //            simpleQuality = Model.Instances.New<IfcQuantityWeight>(sq => sq.WeightValue = (IfcMassMeasure)value);
        //            break;
        //        default:
        //            return;
        //    }

        //    simpleQuality.Unit = unit;
        //    simpleQuality.Name = qualityName;

        //    qset.Quantities.Add(simpleQuality);
        //}

        //public void RemovePropertySingleValue(string pSetName, string propertyName)
        //{
        //    var pset = GetPropertySet(pSetName);
        //    if (pset != null)
        //    {
        //        var singleValue = pset.HasProperties.FirstOrDefault<IfcPropertySingleValue>(p => p.Name == propertyName);
        //        if (singleValue != null)
        //        {
        //            pset.HasProperties.Remove(singleValue);
        //        }
        //    }

        //}

        //public void RemoveElementPhysicalSimpleQuantity(string pSetName, string qualityName)
        //{
        //    var elementQuality = GetElementQuantity(pSetName);
        //    if (elementQuality != null)
        //    {
        //        var simpleQuality = elementQuality.Quantities.FirstOrDefault<IfcPhysicalSimpleQuantity>(sq => sq.Name == qualityName);
        //        if (simpleQuality != null)
        //        {
        //            elementQuality.Quantities.Remove(simpleQuality);
        //        }
        //    }
        //} 
        #endregion
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
