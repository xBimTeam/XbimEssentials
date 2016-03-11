using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }
    }
}
