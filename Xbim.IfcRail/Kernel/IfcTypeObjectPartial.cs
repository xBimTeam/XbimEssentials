using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;

namespace Xbim.IfcRail.Kernel
{
    public partial class IfcTypeObject
    {
        public IEnumerable<IIfcRelDefinesByProperties> DefinedByProperties
        {
            get
            {
                return Model.Instances.Where<IfcRelDefinesByProperties>(e => e.RelatedObjects.Contains(this), "RelatedObjects", this);
            }
        }

    }
}
