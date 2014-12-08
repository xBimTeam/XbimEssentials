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
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common;
using Xbim.Ifc.SelectTypes;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public enum ProjectUnits
    {
        SIUnitsUK
    }

    public static class ProjectExtensions
    {
        #region Unit Initialization

        /// <summary>
        ///   Sets up the default units as SI
        ///   Creates the GeometricRepresentationContext for a Model view, required by Ifc compliance
        /// </summary>
        /// <param name = "ifcProject"></param>
        public static void Initialize(this IfcProject ifcProject, ProjectUnits units)
        {
            IModel model = ifcProject.ModelOf;
            if (units == ProjectUnits.SIUnitsUK)
            {
                IfcUnitAssignment ua = model.Instances.New<IfcUnitAssignment>();
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.LENGTHUNIT;
                                                                     s.Name = IfcSIUnitName.METRE;
                                                                     s.Prefix = IfcSIPrefix.MILLI;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.AREAUNIT;
                                                                     s.Name = IfcSIUnitName.SQUARE_METRE;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.VOLUMEUNIT;
                                                                     s.Name = IfcSIUnitName.CUBIC_METRE;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.SOLIDANGLEUNIT;
                                                                     s.Name = IfcSIUnitName.STERADIAN;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.PLANEANGLEUNIT;
                                                                     s.Name = IfcSIUnitName.RADIAN;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.MASSUNIT;
                                                                     s.Name = IfcSIUnitName.GRAM;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.TIMEUNIT;
                                                                     s.Name = IfcSIUnitName.SECOND;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType =
                                                                         IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT;
                                                                     s.Name = IfcSIUnitName.DEGREE_CELSIUS;
                                                                 }));
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = IfcUnitEnum.LUMINOUSINTENSITYUNIT;
                                                                     s.Name = IfcSIUnitName.LUMEN;
                                                                 }));
                ifcProject.UnitsInContext = ua;
            }
            //Create the Mandatory Model View
            if (ModelContext(ifcProject) == null)
            {
                IfcCartesianPoint origin = model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(0, 0, 0));
                IfcAxis2Placement3D axis3D = model.Instances.New<IfcAxis2Placement3D>(a => a.Location = origin);
                IfcGeometricRepresentationContext gc = model.Instances.New<IfcGeometricRepresentationContext>(c =>
                                                                                                        {
                                                                                                            c.
                                                                                                                ContextType
                                                                                                                =
                                                                                                                "Model";
                                                                                                            c.
                                                                                                                ContextIdentifier
                                                                                                                =
                                                                                                                "Building Model";
                                                                                                            c.
                                                                                                                CoordinateSpaceDimension
                                                                                                                = 3;
                                                                                                            c.Precision
                                                                                                                =
                                                                                                                0.00001;
                                                                                                            c.
                                                                                                                WorldCoordinateSystem
                                                                                                                = axis3D;
                                                                                                        }
                    );
                ifcProject.RepresentationContexts.Add(gc);

                IfcCartesianPoint origin2D = model.Instances.New<IfcCartesianPoint>(p => p.SetXY(0, 0));
                IfcAxis2Placement2D axis2D = model.Instances.New<IfcAxis2Placement2D>(a => a.Location = origin2D);
                IfcGeometricRepresentationContext pc = model.Instances.New<IfcGeometricRepresentationContext>(c =>
                {
                    c.
                        ContextType
                        =
                        "Plan";
                    c.
                        ContextIdentifier
                        =
                        "Building Plan View";
                    c.
                        CoordinateSpaceDimension
                        = 2;
                    c.Precision
                        =
                        0.00001;
                    c.
                        WorldCoordinateSystem
                        = axis2D;
                }
                    );
                ifcProject.RepresentationContexts.Add(pc);

            }
        }

        #endregion
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
            IModel model = ifcProject.ModelOf;
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
            IModel model = ifcProject.ModelOf;
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
                IfcRelAggregates relSub = ifcProject.ModelOf.Instances.New<IfcRelAggregates>();
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
                IfcRelAggregates relSub = ifcProject.ModelOf.Instances.New<IfcRelAggregates>();
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