#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ProjectExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Common;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    

    public static class ProjectExtensions
    {
      
        /// <summary>
        /// Returns all buildings at the highest level of spatial structural decomposition (i.e. root buildings)
        /// </summary>
        public static IEnumerable<IfcBuilding> GetBuildings(this IfcProject project)
        {
            if (project != null)
            {
                IEnumerable<IfcRelAggregates> aggregate = project.IsDecomposedBy.OfType<IfcRelAggregates>();
                foreach (IfcRelAggregates rel in aggregate)
                {
                    foreach (IfcObjectDefinition definition in rel.RelatedObjects)
                    {
                        if (definition is IfcSite)
                            foreach (IfcBuilding building in ((IfcSite)definition).GetBuildings())
                                yield return building;
                        if (definition is IfcBuilding)
                            yield return (definition as IfcBuilding);
                    }
                }
            }
        }

        public static IEnumerable<IfcSpatialStructureElement> GetSpatialStructuralElements(this IfcProject project)
        {
            IEnumerable<IfcRelAggregates> aggregate = project.IsDecomposedBy.OfType<IfcRelAggregates>();
            foreach (var rel in aggregate)
            {
                foreach (var item in rel.RelatedObjects.OfType<IfcSpatialStructureElement>())
                     yield return item;
            }
        }


        public static string BuildName(this IfcProject ifcProject)
        {
            List<string> tokens = new List<string>();
            if(!string.IsNullOrWhiteSpace(ifcProject.Name)) tokens.Add(ifcProject.Name); 
            else if(!string.IsNullOrWhiteSpace(ifcProject.LongName)) tokens.Add(ifcProject.LongName);
            else if(!string.IsNullOrWhiteSpace(ifcProject.Description)) tokens.Add(ifcProject.Description);
            if(!string.IsNullOrWhiteSpace(ifcProject.Phase)) tokens.Add(ifcProject.Phase);
            return string.Join(", ", tokens);
        }

       
        public static IfcNamedUnit GetAreaUnit(this IfcProject ifcProject)
        {
            return ifcProject.UnitsInContext.GetAreaUnit();
        }

        public static void SetOrChangeSIUnit(this IfcProject ifcProject, IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                             IfcSIPrefix? siUnitPrefix)
        {
            IModel model = ifcProject.Model;
            if (ifcProject.UnitsInContext == null)
            {
                ifcProject.UnitsInContext = model.Instances.New<IfcUnitAssignment>();
            }

            IfcUnitAssignment unitsAssignment = ifcProject.UnitsInContext;
            unitsAssignment.SetOrChangeSIUnit(unitType, siUnitName, siUnitPrefix);
        }

        public static void SetOrChangeConversionUnit(this IfcProject ifcProject, IfcUnitEnum unitType,
                                                     ConversionBasedUnit conversionUnit)
        {
            IModel model = ifcProject.Model;
            if (ifcProject.UnitsInContext == null)
            {
                ifcProject.UnitsInContext = model.Instances.New<IfcUnitAssignment>();
            }

            IfcUnitAssignment unitsAssignment = ifcProject.UnitsInContext;
            unitsAssignment.SetOrChangeConversionUnit(unitType, conversionUnit);
        }


        public static IfcGeometricRepresentationContext ModelContext(this IfcProject proj)
        {
            return
                proj.RepresentationContexts.Where<IfcGeometricRepresentationContext>(r => r.ContextType == "Model").
                    FirstOrDefault();
        }

        public static IfcGeometricRepresentationContext PlanContext(this IfcProject proj)
        {
            return
                proj.RepresentationContexts.Where<IfcGeometricRepresentationContext>(r => r.ContextType == "Plan").
                    FirstOrDefault();
        }


        #region Decomposition methods

        /// <summary>
        ///   Adds Site to the IsDecomposedBy Collection.
        /// </summary>
        public static void AddSite(this IfcProject ifcProject, IfcSite site)
        {
            IEnumerable<IfcRelDecomposes> decomposition = ifcProject.IsDecomposedBy;
            if (decomposition.Count() == 0) //none defined create the relationship
            {
                IfcRelAggregates relSub = ifcProject.Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = ifcProject;
                relSub.RelatedObjects.Add(site);
            }
            else
            {
                decomposition.First().RelatedObjects.Add(site);
            }
        }

        public static IEnumerable<IfcSite> GetSites(this IfcProject ifcProject)
        {
            IEnumerable<IfcRelAggregates> aggregate = ifcProject.IsDecomposedBy.OfType<IfcRelAggregates>();
            
            foreach (IfcRelAggregates rel in aggregate)
            {
                foreach (IfcObjectDefinition definition in rel.RelatedObjects)
                {
                    if (definition is IfcSite) yield return (definition as IfcSite);
                }
            }
            
        }

        /// <summary>
        ///   Adds Building to the IsDecomposedBy Collection.
        /// </summary>
        public static void AddBuilding(this IfcProject ifcProject, IfcBuilding building)
        {
            IEnumerable<IfcRelDecomposes> decomposition = ifcProject.IsDecomposedBy;
            if (decomposition.Count() == 0) //none defined create the relationship
            {
                IfcRelAggregates relSub = ifcProject.Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = ifcProject;
                relSub.RelatedObjects.Add(building);
            }
            else
            {
                decomposition.First().RelatedObjects.Add(building);
            }
        }

        #endregion
    }
}