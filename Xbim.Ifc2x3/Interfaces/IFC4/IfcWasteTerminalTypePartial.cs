using Xbim.Ifc4.Interfaces;
using System;
using Xbim.Ifc4.MeasureResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.PlumbingFireProtectionDomain
{
    public partial class IfcWasteTerminalType
    {
        IfcLabel? IIfcElementType.ElementType
        {
            get
            {
                switch (PredefinedType)
                {
                    case IfcWasteTerminalTypeEnum.GREASEINTERCEPTOR:
                    case IfcWasteTerminalTypeEnum.OILINTERCEPTOR:
                    case IfcWasteTerminalTypeEnum.PETROLINTERCEPTOR:
                        return new IfcLabel(Enum.GetName(typeof(IfcWasteTerminalTypeEnum), PredefinedType));
                }
                return ElementType.HasValue ? new IfcLabel(ElementType) : null;
            }
            set
            {
                ElementType = value.HasValue
                    ? value.Value.ToString()
                    : null;

                if (!value.HasValue)
                    return;

                IfcWasteTerminalTypeEnum e;
                if (Enum.TryParse(value.Value.ToString(), true, out e))
                    PredefinedType = e;
            }
        }
    }
}
