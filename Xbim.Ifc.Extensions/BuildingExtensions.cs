using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class BuildingExtensions
    {
        /// <summary>
        /// Returns the site (if any) that contains this building, null if the building is not decomposing a site
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        public static IfcSite GetSite(this IfcBuilding building)
        {
            return building.Decomposes.OfType<IfcRelAggregates>().Select(r => r.RelatedObjects).OfType<IfcSite>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the buidlings that decompose this building
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        public static IEnumerable<IfcBuilding> GetBuildings(this IfcBuilding building)
        {
            IEnumerable<IfcRelDecomposes> decomp = building.IsDecomposedBy;
            IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);
            return objs.OfType<IfcBuilding>();
        }

        /// <summary>
        /// Returns all spaces that are sub-spaces of this building
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IfcSpace> GetSpaces(this IfcBuilding building)
        {
            IEnumerable<IfcRelDecomposes> decomp = building.IsDecomposedBy;
            IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);
            return objs.OfType<IfcSpace>();
        }

        /// <summary>
        /// Gets the Gross Floor Area, if the element base quantity GrossFloorArea is defined this has precedence
        /// If no property is defined the GFA is returned as the sume of the building storeys GFA
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        public static IfcAreaMeasure? GetGrossFloorArea(this IfcBuilding building)
        {
            IfcQuantityArea qArea = building.GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea");
            if (qArea == null) qArea = building.GetQuantity<IfcQuantityArea>("GrossFloorArea"); //just look for any area
            if (qArea != null) return qArea.AreaValue;
            IfcAreaMeasure area = 0;
            foreach (IfcBuildingStorey buildingStorey in building.GetBuildingStoreys())
            {
                IfcAreaMeasure? bsArea = buildingStorey.GetGrossFloorArea();
                if (bsArea.HasValue) area += bsArea;
            }
            if (area != 0) return area;
            return null;
        }


        
        /// <summary>
        /// Returns the building storeys for this floor  
        /// </summary>
        /// <param name="building"></param>
        /// <param name="sortByElevation">If true and the building storey Elevation property has been set will sort by this value</param>
        /// <returns></returns>
        public static IEnumerable<IfcBuildingStorey> GetBuildingStoreys(this IfcBuilding building, bool sortByElevation = false)
        {
            IEnumerable<IfcRelDecomposes> decomp = building.IsDecomposedBy;
            IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);
            if (sortByElevation)
            {
                List<IfcBuildingStorey> storeys = objs.OfType<IfcBuildingStorey>().ToList();
                storeys.Sort(BuildingStoreyExtensions.CompareStoreysByElevation);
                return storeys;
            }
            else
                return objs.OfType<IfcBuildingStorey>();
        }

       
    }
}
