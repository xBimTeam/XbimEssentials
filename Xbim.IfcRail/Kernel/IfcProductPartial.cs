using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.IfcRail.Kernel
{
    public abstract partial class @IfcProduct
    {
        public IIfcSpatialElement IsContainedIn
        {
            get
            {
                return Model.Instances.Where<IIfcRelContainedInSpatialStructure>(r => r.RelatedElements.Contains(this), nameof(IIfcRelContainedInSpatialStructure.RelatedElements), this)
                    .Select(s => s.RelatingStructure).FirstOrDefault();
            }
        }
    }

}
