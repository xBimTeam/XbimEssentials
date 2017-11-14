#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    SpatialStructureElementExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class SpatialStructureElementExtensions
    {
        /// <summary>
        ///   Returns all the  elements that decomposes this
        /// </summary>
        /// <param name = "se"></param>
        /// <param name = "model"></param>
        /// <returns></returns>
        public static IEnumerable<IfcProduct> GetContainedElements(this IfcSpatialStructureElement se, IModel model)
        {
            return
                model.Instances.Where<IfcRelContainedInSpatialStructure>(r => r.RelatingStructure == se).SelectMany(
                    subrel => subrel.RelatedElements);
        }

        /// <summary>
        ///   Returns all the elements that decomposes this
        /// </summary>
        /// <param name = "se"></param>
        /// <returns></returns>
        public static IEnumerable<IfcProduct> GetContainedElements(this IfcSpatialStructureElement se)
        {
            return
                se.Model.Instances.Where<IfcRelContainedInSpatialStructure>(
                    r => r.RelatingStructure == se).SelectMany(subrel => subrel.RelatedElements);
        }

        /// <summary>
        ///   Returns  the first spatial structural element that this decomposes
        /// </summary>
        /// <param name = "se"></param>
        /// <returns></returns>
        public static IfcSpatialStructureElement GetContainingStructuralElement(this IfcSpatialStructureElement se)
        {
            IModel model = se.Model;
            IEnumerable<IfcRelContainedInSpatialStructure> rels =
                model.Instances.Where<IfcRelContainedInSpatialStructure>(r => r.RelatedElements.Contains(se));
            return rels.Select(r => r.RelatingStructure).FirstOrDefault();
            // return  se).Instances.Where<RelContainedInSpatialStructure>(r => r.RelatedElements.Contains(se)).Select(r=>r.RelatingStructure).FirstOrDefault(.Model;
        }

        /// <summary>
        ///   Returns  the spatial structural elements that this decomposes
        /// </summary>
        /// <param name = "se"></param>
        /// <returns></returns>
        public static IEnumerable<IfcSpatialStructureElement> GetContainingStructuralElements(
            this IfcSpatialStructureElement se)
        {
            IModel model = se.Model;
            IEnumerable<IfcRelContainedInSpatialStructure> rels =
                model.Instances.Where<IfcRelContainedInSpatialStructure>(r => r.RelatedElements.Contains(se));
            return rels.Select(r => r.RelatingStructure);
            // return  se).Instances.Where<RelContainedInSpatialStructure>(r => r.RelatedElements.Contains(se)).Select(r=>r.RelatingStructure).FirstOrDefault(.Model;
        }

        /// <summary>
        ///   Adds the  element to the set of  elements which are contained in this spatialstructure
        /// </summary>
        /// <param name = "se"></param>
        /// <param name = "prod"></param>
        public static void AddElement(this IfcSpatialStructureElement se, IfcProduct prod)
        {
            if (prod == null) return;

            IEnumerable<IfcRelContainedInSpatialStructure> relatedElements = se.ContainsElements;
            var ifcRelContainedInSpatialStructures = relatedElements as IList<IfcRelContainedInSpatialStructure> ?? relatedElements.ToList();
            if (!ifcRelContainedInSpatialStructures.Any()) //none defined create the relationship
            {
                IfcRelContainedInSpatialStructure relSe = se.Model.Instances.New<IfcRelContainedInSpatialStructure>();
                relSe.RelatingStructure = se;
                relSe.RelatedElements.Add(prod);
            }
            else
            {
                ifcRelContainedInSpatialStructures.First().RelatedElements.Add(prod);
            }
        }

        /// <summary>
        ///   Adds specified IfcSpatialStructureElement to the decomposition of this spatial structure element.
        /// </summary>
        /// <param name = "se"></param>
        /// <param name = "child">Child spatial structure element.</param>
        public static void AddToSpatialDecomposition(this IfcSpatialStructureElement se,
                                                     IfcSpatialStructureElement child)
        {
            IEnumerable<IfcRelDecomposes> decomposition = se.IsDecomposedBy;
            var ifcRelDecomposeses = decomposition as IList<IfcRelDecomposes> ?? decomposition.ToList();
            if (!ifcRelDecomposeses.Any()) //none defined create the relationship
            {
                IfcRelAggregates relSub = se.Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = se;
                relSub.RelatedObjects.Add(child);
            }
            else
            {
                ifcRelDecomposeses.First().RelatedObjects.Add(child);
            }
        }
    }
}
