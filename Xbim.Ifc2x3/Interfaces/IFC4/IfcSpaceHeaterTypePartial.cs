using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.HVACDomain
{
    public partial class IfcSpaceHeaterType
    {
        IfcLabel? IIfcElementType.ElementType
        {
            get
            {
                if (PredefinedType == IfcSpaceHeaterTypeEnum.BASEBOARDHEATER)
                    return new IfcLabel("BASEBOARDHEATER");
                if (PredefinedType == IfcSpaceHeaterTypeEnum.FINNEDTUBEUNIT)
                    return new IfcLabel("FINNEDTUBEUNIT");
                if (PredefinedType == IfcSpaceHeaterTypeEnum.PANELRADIATOR)
                    return new IfcLabel("PANELRADIATOR");
                if (PredefinedType == IfcSpaceHeaterTypeEnum.SECTIONALRADIATOR)
                    return new IfcLabel("SECTIONALRADIATOR");
                if (PredefinedType == IfcSpaceHeaterTypeEnum.TUBULARRADIATOR)
                    return new IfcLabel("TUBULARRADIATOR");
                if (PredefinedType == IfcSpaceHeaterTypeEnum.UNITHEATER)
                    return new IfcLabel("UNITHEATER");
                return ElementType.HasValue ? new IfcLabel(ElementType) : null;
            }
        }
    }
}
