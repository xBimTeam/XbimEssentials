using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcObject
    {
        public IEnumerable<IfcRelDefinesByProperties> IsDefinedByProperties
        {
            get
            {
                return
                    Model.Instances.Where<IfcRelDefinesByProperties>(r => r.RelatedObjects.Contains(this));
            }
        }
    }
}
