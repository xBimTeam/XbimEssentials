using System.Linq;
using Xbim.IfcRail.ProductExtension;

namespace Xbim.IfcRail.Kernel
{
    public abstract partial class @IfcProduct
    {
        public IIfcSpatialElement IsContainedIn
        {
            get
            {
                return Model.Instances.Where<IIfcRelContainedInSpatialStructure>(r => r.RelatedElements.Contains(this)).Select(s => s.RelatingStructure).FirstOrDefault();
            }
        }
    }

}
