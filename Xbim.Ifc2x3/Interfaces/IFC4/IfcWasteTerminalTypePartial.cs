using Xbim.Ifc4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.PlumbingFireProtectionDomain
{
    public partial class IfcWasteTerminalType
    {
        IfcLabel? IIfcElementType.ElementType
        {
            get
            {
                if (PredefinedType == IfcWasteTerminalTypeEnum.GREASEINTERCEPTOR)
                    return new IfcLabel("GREASEINTERCEPTOR");
                if (PredefinedType == IfcWasteTerminalTypeEnum.OILINTERCEPTOR)
                    return new IfcLabel("OILINTERCEPTOR");
                if (PredefinedType == IfcWasteTerminalTypeEnum.PETROLINTERCEPTOR)
                    return new IfcLabel("PETROLINTERCEPTOR");
                return ElementType.HasValue ? new IfcLabel(ElementType) : null;
            }
        }
    }
}
