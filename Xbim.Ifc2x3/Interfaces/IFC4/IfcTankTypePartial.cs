using System;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.HVACDomain
{
    public partial class IfcTankType
    {
        IfcLabel? IIfcElementType.ElementType
        {
            get
            {
                if (PredefinedType == IfcTankTypeEnum.SECTIONAL)
                    return new IfcLabel("SECTIONAL");
                if (PredefinedType == IfcTankTypeEnum.PREFORMED)
                    return new IfcLabel("PREFORMED");
                return ElementType.HasValue ? new IfcLabel(ElementType) : null;
            }
            set
            {
                ElementType = value.HasValue
                    ? value.Value.ToString()
                    : null;

                if (!value.HasValue)
                    return;

                IfcTankTypeEnum e;
                if (Enum.TryParse(value.Value.ToString(), true, out e))
                    PredefinedType = e;
            }
        }
    }
}
