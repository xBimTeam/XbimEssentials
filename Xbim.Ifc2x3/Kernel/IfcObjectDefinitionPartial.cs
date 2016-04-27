using System.Linq;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.ProductExtension;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcObjectDefinition
    {     
        public IfcMaterialSelect Material
        {
            get
            {
                var relMat = HasAssociations.OfType<IfcRelAssociatesMaterial>().FirstOrDefault();
                if (relMat != null) return relMat.RelatingMaterial;
                return null;
            }
        }
    }
}
