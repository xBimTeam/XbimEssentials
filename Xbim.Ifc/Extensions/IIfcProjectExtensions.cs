using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcProjectExtensions
    {
        public static IEnumerable<IIfcSpatialStructureElement> GetSpatialStructuralElements(this IIfcProject project)
        {
            return project.IsDecomposedBy.SelectMany(rel => rel.RelatedObjects.OfType<IIfcSpatialStructureElement>());
        }



        /// <summary>
        ///   Adds Site to the IsDecomposedBy Collection.
        /// </summary>
        public static void AddSite(this IIfcProject proj, IIfcSite site)
        {
            var decomposition = proj.IsDecomposedBy.FirstOrDefault();
            if (decomposition == null) //none defined create the relationship
            {
                var factory = new EntityCreator(proj.Model);
                var relSub = factory.RelAggregates(r =>
                {
                    r.RelatingObject = proj;
                    r.RelatedObjects.Add(site);
                });
            }
            else
                decomposition.RelatedObjects.Add(site);
        }

        /// <summary>
		///   Adds Building to the IsDecomposedBy Collection.
		/// </summary>
		public static void AddBuilding(this IIfcProject proj, IIfcBuilding building)
        {
            var decomposition = proj.IsDecomposedBy.FirstOrDefault();
            if (decomposition == null) //none defined create the relationship
            {
                var factory = new EntityCreator(proj.Model);
                var relSub = factory.RelAggregates(r =>
                {
                    r.RelatingObject = proj;
                    r.RelatedObjects.Add(building);
                });
            }
            else
                decomposition.RelatedObjects.Add(building);
        }

        /// <summary>
		///   Adds Ifc4x3 Facility to the IsDecomposedBy Collection.
		/// </summary>
		public static void AddFacility(this IIfcProject proj, Ifc4x3.ProductExtension.IfcFacility facility)
        {
            var decomposition = proj.IsDecomposedBy.FirstOrDefault();
            if (decomposition == null) //none defined create the relationship
            {
                var factory = new EntityCreator(proj.Model);
                var relSub = factory.RelAggregates(r =>
                {
                    r.RelatingObject = proj;
                    r.RelatedObjects.Add(facility);
                });
            }
            else
                decomposition.RelatedObjects.Add(facility);
        }
    }
}
