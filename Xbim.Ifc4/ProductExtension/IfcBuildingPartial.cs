using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4.Interfaces
{
    /// <summary>
    /// Readonly interface for IfcBuilding
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial interface @IIfcBuilding : IIfcSpatialStructureElement
    {
        IIfcSite Site { get; }
        IEnumerable<IIfcBuilding> Buildings { get; }
        IEnumerable<IIfcSpace> Spaces { get; }
        IEnumerable<IIfcBuildingStorey> BuildingStoreys { get; }
    }
}

namespace Xbim.Ifc4.ProductExtension
{

     public partial class IfcBuilding
     {
         
         /// <summary>
         /// Returns the site (if any) that contains this building, null if the building is not decomposing a site
         /// </summary>
         /// <returns></returns>
         public IIfcSite Site
         {
             get
             {
                 return Decomposes.SelectMany(r => r.RelatedObjects).OfType<IIfcSite>().FirstOrDefault();
             }
         }

         /// <summary>
         /// Returns the buidlings that decompose this building
         /// </summary>
         /// <returns></returns>
         public IEnumerable<IIfcBuilding> Buildings
         {
             get
             {
                 return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IIfcBuilding>();             
             }
         }

         /// <summary>
         /// Returns all spaces that are sub-spaces of this building
         /// </summary>
         /// <returns></returns>
         public IEnumerable<IIfcSpace> Spaces
         {
             get
             {
                 return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IIfcSpace>();         
             }
         }

        

         /// <summary>
         /// Returns the building storeys for this floor  
         /// </summary>
         /// <returns></returns>
         public  IEnumerable<IIfcBuildingStorey> BuildingStoreys
         {
             get
             {
                 var storeys = IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IIfcBuildingStorey>().ToList();
                 storeys.Sort(IfcBuildingStorey.CompareStoreysByElevation);
                 return storeys;               
             }
         }

         #region Properties
         ///// <summary>
         ///// Gets the Gross Floor Area, if the element base quantity GrossFloorArea is defined this has precedence
         ///// If no property is defined the GFA is returned as the sume of the building storeys GFA
         ///// </summary>
         ///// <returns></returns>
         //public IfcAreaMeasure? GrossFloorArea
         //{
         //    get
         //    {
         //        var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea") ??
         //                                GetQuantity<IfcQuantityArea>("GrossFloorArea");
         //        if (qArea != null)
         //            return qArea.AreaValue;
         //        IfcAreaMeasure area = 0;
         //        foreach (var buildingStorey in BuildingStoreys)
         //        {
         //            var bsArea = buildingStorey.GrossFloorArea ?? 0;
         //            area += bsArea;
         //        }
         //        if (area != 0) return area;
         //        return null;
         //    }
         //}

         #endregion

     }
}
