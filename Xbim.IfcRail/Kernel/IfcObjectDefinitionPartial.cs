using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;
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
                IIfcRelAssociatesMaterial relMat = HasAssociations.OfType<IIfcRelAssociatesMaterial>().FirstOrDefault();
                return relMat != null ? relMat.RelatingMaterial : null;
            }
        }

        public IIfcValue this[string property]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(property))
                    return null;

                List<IIfcPropertySetDefinition> pSets = null;
                if (this is IIfcObject obj)
                    pSets = obj.IsDefinedBy.SelectMany(GetDefinitions).Where(d => d != null).ToList();

                if (this is IIfcTypeObject type)
                    pSets = type.HasPropertySets.ToList();

                IfcContext ctx = this as IfcContext;
                if (ctx != null)
                    pSets = ctx.IsDefinedBy.SelectMany(GetDefinitions).Where(d => d != null).ToList();

                if (pSets == null || !pSets.Any())
                    return null;

                string[] parts = property.Split('.');
                if (parts.Length == 2)
                {
                    pSets = pSets.Where(p => p.Name == parts[0]).ToList();
                    property = parts[1];
                }

                IIfcPropertySingleValue prop =
                    pSets.OfType<IIfcPropertySet>()
                        .SelectMany(p => p.HasProperties.OfType<IIfcPropertySingleValue>())
                        .FirstOrDefault(p => p.Name == property);
                if (prop != null)
                    return prop.NominalValue;

                IfcPhysicalSimpleQuantity quant = pSets.OfType<IfcElementQuantity>()
                        .SelectMany(p => p.Quantities.OfType<IfcPhysicalSimpleQuantity>())
                        .FirstOrDefault(p => p.Name == property);
                if (quant == null)
                    return null;

                if (quant is IIfcQuantityArea area)
                    return area.AreaValue;
                if (quant is IIfcQuantityCount count)
                    return count.CountValue;
                if (quant is IIfcQuantityLength length)
                    return length.LengthValue;
                if (quant is IIfcQuantityTime time)
                    return time.TimeValue;
                if (quant is IIfcQuantityVolume volume)
                    return volume.VolumeValue;
                if (quant is IIfcQuantityWeight weight)
                    return weight.WeightValue;

                return null;
            }
        }

        private static IEnumerable<IIfcPropertySetDefinition> GetDefinitions(IIfcRelDefinesByProperties rel)
        {
            if (!(rel.RelatingPropertyDefinition is IfcRelDefinesByProperties r))
                return null;
            IfcPropertySetDefinitionSelect defSelect = r.RelatingPropertyDefinition;
            IfcPropertySetDefinition def = defSelect as IfcPropertySetDefinition;
            if (def != null)
                return new[] { def };
            if (defSelect is IfcPropertySetDefinitionSet)
            {
                return
                    ((IfcPropertySetDefinitionSet)defSelect).Value as IEnumerable<IIfcPropertySetDefinition>;
            }
            return null;
        }
    }
}
