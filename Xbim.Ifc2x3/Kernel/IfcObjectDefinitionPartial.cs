using System.Linq;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcObjectDefinition
    {     
        public IIfcMaterialSelect Material
        {
            get
            {
                var relMat = HasAssociations.OfType<IIfcRelAssociatesMaterial>().FirstOrDefault();
                if (relMat != null) return relMat.RelatingMaterial;
                return null;
            }
        }
    }
}
