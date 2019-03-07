using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;
using Xbim.IfcRail.MeasureResource;

namespace Xbim.IfcRail.ProductExtension
{
    public partial class IfcBuildingStorey
    {
        internal static int CompareStoreysByElevation(IIfcBuildingStorey x, IIfcBuildingStorey y)
        {
            double a = x.Elevation ?? 0;
            double b = y.Elevation ?? 0;
            return a.CompareTo(b);
        }
       
        /// <summary>
        /// Returns all spaces that are sub-spaces of this storey
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IIfcSpace> Spaces
        {
            get
            {              
                return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IIfcSpace>();
            }
        }


        public IEnumerable<IIfcBuildingStorey> BuildingStoreys
        {
            get
            {
                var storeys = IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IIfcBuildingStorey>().ToList();         
                storeys.Sort(CompareStoreysByElevation);
                return storeys;
            }
        }
    }
}
