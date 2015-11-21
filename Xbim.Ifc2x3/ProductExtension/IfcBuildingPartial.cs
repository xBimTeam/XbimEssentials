using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;

namespace Xbim.Ifc2x3.ProductExtension
{

     public partial class IfcBuilding
     {
         
         /// <summary>
         /// Returns the site (if any) that contains this building, null if the building is not decomposing a site
         /// </summary>
         /// <returns></returns>
         public IfcSite Site
         {
             get
             {
                 return Decomposes.OfType<IfcRelAggregates>().SelectMany(r => r.RelatedObjects).OfType<IfcSite>().FirstOrDefault();
             }
         }

         /// <summary>
         /// Returns the buidlings that decompose this building
         /// </summary>
         /// <returns></returns>
         public IEnumerable<IfcBuilding> Buildings
         {
             get
             {
                 return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IfcBuilding>();             
             }
         }

         /// <summary>
         /// Returns all spaces that are sub-spaces of this building
         /// </summary>
         /// <returns></returns>
         public IEnumerable<IfcSpace> Spaces
         {
             get
             {
                 return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IfcSpace>();         
             }
         }

         /// <summary>
         /// Gets the Gross Floor Area, if the element base quantity GrossFloorArea is defined this has precedence
         /// If no property is defined the GFA is returned as the sume of the building storeys GFA
         /// </summary>
         /// <returns></returns>
         public IfcAreaMeasure? GrossFloorArea
         {
             get
             {
                 var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea") ??
                                         GetQuantity<IfcQuantityArea>("GrossFloorArea");
                 if (qArea != null)
                     return qArea.AreaValue;
                 IfcAreaMeasure area = 0;
                 foreach (var buildingStorey in BuildingStoreys)
                 {
                     var bsArea = buildingStorey.GrossFloorArea ?? 0;
                     area += bsArea;
                 }
                 if (area != 0) return area;
                 return null;
             }
         }


         /// <summary>
         /// Returns the building storeys for this floor  
         /// </summary>
         /// <returns></returns>
         public  IEnumerable<IfcBuildingStorey> BuildingStoreys
         {
             get
             {
                 var storeys = IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IfcBuildingStorey>().ToList();
                 storeys.Sort(IfcBuildingStorey.CompareStoreysByElevation);
                 return storeys;               
             }
         }

    }
}
