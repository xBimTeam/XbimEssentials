using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;
using Xbim.IfcRail.Kernel;
using Xbim.IfcRail.RepresentationResource;


namespace Xbim.IfcRail.ProductExtension
{
    public partial class IfcSite
    {

        /// <summary>
        /// Returns all buildings at the highest level of spatial structural decomposition (i.e. root buildings for this site)
        /// </summary>
        public IEnumerable<IIfcBuilding> Buildings
        {
            get
            {               
                return IsDecomposedBy.SelectMany(rel => rel.RelatedObjects.OfType<IfcBuilding>());
            }
        }


        public IEnumerable<IfcSpace> Spaces
        {
            get
            {

                if (IsDecomposedBy != null)
                {
                    var decomp = IsDecomposedBy;
                    var objs = decomp.SelectMany(s => s.RelatedObjects);
                    return objs.OfType<IfcSpace>();
                }

                return Enumerable.Empty<IfcSpace>();
            }
        }

        #region Representation methods

        public IfcShapeRepresentation FootPrintRepresentation
        {
            get
            {
                if (Representation != null)
                    return
                        Representation.Representations.OfType<IfcShapeRepresentation>().FirstOrDefault(
                            r => string.Compare(r.RepresentationIdentifier.GetValueOrDefault(), "FootPrint", true) == 0);
                return null;
            }
        }

        public void AddBuilding(IfcBuilding building)
        {
            var decomposition = IsDecomposedBy.FirstOrDefault();
            if (decomposition == null) //none defined create the relationship
            {
                var relSub = Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = this;
                relSub.RelatedObjects.Add(building);
            }
            else
            {
                decomposition.RelatedObjects.Add(building);
            }
        }

        public void AddSite(IfcSite subSite)
        {
            var decomposition = IsDecomposedBy.FirstOrDefault();
            if (decomposition==null) //none defined create the relationship
            {
                var relSub = Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = this;
                relSub.RelatedObjects.Add(subSite);
            }
            else
            {
                decomposition.RelatedObjects.Add(subSite);
            }
        }

#endregion

        #region Property Values

        ///// <summary>
        ///// Returns the projected footprint are of the site, this value is derived and makes use of property sets not in the ifc schema
        ///// </summary>
        ///// <returns></returns>
        //public IfcAreaMeasure? FootprintArea
        //{
        //    get
        //    {
        //        var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossArea");
        //        if (qArea == null) qArea = GetQuantity<IfcQuantityArea>("GrossArea"); //just look for any area
        //        if (qArea != null) return qArea.AreaValue;
        //        //if revit try their value
        //        IfcAreaMeasure val = GetPropertySingleValue<IfcAreaMeasure>("PSet_Revit_Dimensions",
        //            "Projected Area");
        //        if (val != null) return val;

        //        return null;
        //    }
       // }

        #endregion
    }
}
  

