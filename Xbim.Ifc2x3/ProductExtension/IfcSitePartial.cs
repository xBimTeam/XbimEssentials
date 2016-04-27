using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc2x3.ProductExtension
{
    public partial class IfcSite
    {


        #region Property Values

        /// <summary>
        /// Returns the projected footprint are of the site, this value is derived and makes use of property sets not in the ifc schema
        /// </summary>
        /// <returns></returns>
        public Ifc4.MeasureResource.IfcAreaMeasure? FootprintArea
        {
            get
            {
                var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossArea");
                if (qArea == null) qArea = GetQuantity<IfcQuantityArea>("GrossArea"); //just look for any area
                if (qArea != null) return new Ifc4.MeasureResource.IfcAreaMeasure(qArea.AreaValue);
                //if revit try their value
                var val = GetPropertySingleValue<Ifc4.MeasureResource.IfcAreaMeasure>("PSet_Revit_Dimensions",
                    "Projected Area");
                if (val != null) return val;

                return null;
            }
        }

        /// <summary>
        /// Returns all buildings at the highest level of spatial structural decomposition (i.e. root buildings for this site)
        /// </summary>
        public IEnumerable<IIfcBuilding> Buildings
        {
            get
            {
                IEnumerable<IfcRelAggregates> aggregate = IsDecomposedBy.OfType<IfcRelAggregates>();
                return aggregate.SelectMany(rel => rel.RelatedObjects.OfType<IfcBuilding>());
            }
        }

        #endregion

        public IEnumerable<IIfcSpace> Spaces
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

        public void AddElement(IfcProduct element)
        {
            var spatialStructure = ContainsElements.FirstOrDefault();
            if (spatialStructure==null) //none defined create the relationship
            {
                var relSe = Model.Instances.New<IfcRelContainedInSpatialStructure>();
                relSe.RelatingStructure = this;
                relSe.RelatedElements.Add(element);
            }
            else
                spatialStructure.RelatedElements.Add(element);
        }
        #endregion
    }
}
  

