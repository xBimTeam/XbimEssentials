using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;

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
        }
    }
}
