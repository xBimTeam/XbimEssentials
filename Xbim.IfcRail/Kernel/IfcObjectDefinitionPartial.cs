using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Xbim.IfcRail.ProductExtension;
using Xbim.IfcRail.PropertyResource;
using Xbim.IfcRail.QuantityResource;

namespace Xbim.IfcRail.Kernel
{
    public partial class IfcObjectDefinition
    {     
        public IIfcMaterialSelect Material
        {
            get
            {
                var relMat = HasAssociations.OfType<IIfcRelAssociatesMaterial>().FirstOrDefault();
                return relMat != null ? relMat.RelatingMaterial : null;
            }
        }

        public IIfcValue this[string property]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(property))
                    return null;

                List<IfcPropertySetDefinition> pSets = null;
                var obj = this as IfcObject;
                if (obj != null)
                    pSets = obj.IsDefinedBy.SelectMany(GetDefinitions).Where(d => d != null).ToList();

                var type = this as IfcTypeObject;
                if (type != null)
                    pSets = type.HasPropertySets.ToList();

                var ctx = this as IfcContext;
                if (ctx != null)
                    pSets = ctx.IsDefinedBy.SelectMany(GetDefinitions).Where(d => d != null).ToList();

                if (pSets == null || !pSets.Any())
                    return null;

                var parts = property.Split('.');
                if (parts.Length == 2)
                {
                    pSets = pSets.Where(p => p.Name == parts[0]).ToList();
                    property = parts[1];
                }

                var prop =
                    pSets.OfType<IfcPropertySet>()
                        .SelectMany(p => p.HasProperties.OfType<IfcPropertySingleValue>())
                        .FirstOrDefault(p => p.Name == property);
                if (prop != null)
                    return prop.NominalValue;

                var quant = pSets.OfType<IfcElementQuantity>()
                        .SelectMany(p => p.Quantities.OfType<IfcPhysicalSimpleQuantity>())
                        .FirstOrDefault(p => p.Name == property);
                if (quant == null)
                    return null;

                var area = quant as IfcQuantityArea;
                if (area != null)
                    return area.AreaValue;
                var count = quant as IfcQuantityCount;
                if (count != null)
                    return count.CountValue;
                var length = quant as IfcQuantityLength;
                if (length != null)
                    return length.LengthValue;
                var time = quant as IfcQuantityTime;
                if (time != null)
                    return time.TimeValue;
                var volume = quant as IfcQuantityVolume;
                if (volume != null)
                    return volume.VolumeValue;
                var weight = quant as IfcQuantityWeight;
                if (weight != null)
                    return weight.WeightValue;
                
                return null;
            }
        }

        private static IEnumerable<IfcPropertySetDefinition> GetDefinitions(IfcRelDefinesByProperties r)
        {
            if (r.RelatingPropertyDefinition == null)
                return null;
            var defSelect = r.RelatingPropertyDefinition;
            var def = defSelect as IfcPropertySetDefinition;
            if (def != null)
                return new[] { def };
            if (defSelect is IfcPropertySetDefinitionSet)
            {
                return
                    ((IfcPropertySetDefinitionSet)defSelect).Value as IEnumerable<IfcPropertySetDefinition>;
            }
            return null;
        }
    }
}
