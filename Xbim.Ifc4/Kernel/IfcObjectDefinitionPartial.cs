using System.Linq;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.ProductExtension;

namespace Xbim.Ifc4.Kernel
{
    public partial class IfcObjectDefinition
    {     
        public IfcMaterialSelect Material
        {
            get
            {
                var relMat = HasAssociations.OfType<IfcRelAssociatesMaterial>().FirstOrDefault();
                return relMat != null ? relMat.RelatingMaterial : null;
            }
        }
    }
}
