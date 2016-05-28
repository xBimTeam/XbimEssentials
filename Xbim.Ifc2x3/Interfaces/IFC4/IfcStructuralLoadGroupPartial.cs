using System;
using Xbim.Ifc4.Interfaces;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    public partial class IfcStructuralLoadGroup
    {
        Xbim.Ifc4.MeasureResource.IfcLabel? IIfcObject.ObjectType
        {
            get
            {
                if (PredefinedType == IfcLoadGroupTypeEnum.LOAD_COMBINATION_GROUP)
                    return new Xbim.Ifc4.MeasureResource.IfcLabel("LOAD_COMBINATION_GROUP");
                return ObjectType.HasValue ? new Xbim.Ifc4.MeasureResource.IfcLabel(ObjectType) : null;
            }
            set
            {
                if (!value.HasValue)
                {
                    ObjectType = null;
                    return;
                }
                var str = value.Value.ToString();
                if (string.Equals(str, "LOAD_COMBINATION_GROUP", StringComparison.InvariantCultureIgnoreCase))
                {
                    PredefinedType = IfcLoadGroupTypeEnum.LOAD_COMBINATION_GROUP;
                    ObjectType = str;
                }
            }
        }
    }
}
