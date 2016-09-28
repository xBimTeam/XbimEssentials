using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.HVACDomain
{
    public partial class IfcSpaceHeaterType
    {
        IfcLabel? IIfcElementType.ElementType
        {
            get
            {
                switch (PredefinedType)
                {
                    case IfcSpaceHeaterTypeEnum.BASEBOARDHEATER:
                        return new IfcLabel("BASEBOARDHEATER");
                    case IfcSpaceHeaterTypeEnum.FINNEDTUBEUNIT:
                        return new IfcLabel("FINNEDTUBEUNIT");
                    case IfcSpaceHeaterTypeEnum.PANELRADIATOR:
                        return new IfcLabel("PANELRADIATOR");
                    case IfcSpaceHeaterTypeEnum.SECTIONALRADIATOR:
                        return new IfcLabel("SECTIONALRADIATOR");
                    case IfcSpaceHeaterTypeEnum.TUBULARRADIATOR:
                        return new IfcLabel("TUBULARRADIATOR");
                    case IfcSpaceHeaterTypeEnum.UNITHEATER:
                        return new IfcLabel("UNITHEATER");
                }
                return ElementType.HasValue ? new IfcLabel(ElementType) : null;
            }
            set
            {
                if (!value.HasValue)
                {
                    ElementType = null;
                    return;
                }
                var strVal = value.Value.ToString();
                ElementType = strVal;

                switch (strVal.ToUpperInvariant())
                {
                    case "BASEBOARDHEATER":
                        PredefinedType = IfcSpaceHeaterTypeEnum.BASEBOARDHEATER;
                        ElementType = null;
                        return;
                    case "FINNEDTUBEUNIT":
                        PredefinedType = IfcSpaceHeaterTypeEnum.FINNEDTUBEUNIT;
                        ElementType = null;
                        return;
                    case "PANELRADIATOR":
                        PredefinedType = IfcSpaceHeaterTypeEnum.PANELRADIATOR;
                        ElementType = null;
                        return;
                    case "SECTIONALRADIATOR":
                        PredefinedType = IfcSpaceHeaterTypeEnum.SECTIONALRADIATOR;
                        ElementType = null;
                        return;
                    case "TUBULARRADIATOR":
                        PredefinedType = IfcSpaceHeaterTypeEnum.TUBULARRADIATOR;
                        ElementType = null;
                        return;
                    case "UNITHEATER":
                        PredefinedType = IfcSpaceHeaterTypeEnum.UNITHEATER;
                        ElementType = null;
                        return;
                }
            }
        }
    }
}
