﻿using System.Collections.Generic;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.RepresentationResource;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.ProductExtension;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcProject
    {
        public IfcGeometricRepresentationContext ModelContext
        {
            get
            {
                return 
                    RepresentationContexts.
                        FirstOrDefault<IfcGeometricRepresentationContext>(r => r.ContextType == "Model");    
            }
            
        }

        /// <summary>
        ///   Sets up the default units as SI
        ///   Creates the GeometricRepresentationContext for a Model view, required by Ifc compliance
        /// </summary>
        public void Initialize(ProjectUnits units)
        {
            var model = Model;
            if (units == ProjectUnits.SIUnitsUK)
            {
                var ua = model.Instances.New<IfcUnitAssignment>();
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
                UnitsInContext = ua;
            }
            //Create the Mandatory Model View
            if (ModelContext == null)
            {
                var origin = model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(0, 0, 0));
                var axis3D = model.Instances.New<IfcAxis2Placement3D>(a => a.Location = origin);
                var gc = model.Instances.New<IfcGeometricRepresentationContext>(c =>
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
                RepresentationContexts.Add(gc);

                var origin2D = model.Instances.New<IfcCartesianPoint>(p => p.SetXY(0, 0));
                var axis2D = model.Instances.New<IfcAxis2Placement2D>(a => a.Location = origin2D);
                var pc = model.Instances.New<IfcGeometricRepresentationContext>(c =>
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
                RepresentationContexts.Add(pc);

            }
        }

        /// <summary>
        /// Returns all buildings at the highest level of spatial structural decomposition (i.e. root buildings)
        /// </summary>
        public IEnumerable<Xbim.Ifc4.Interfaces.IIfcBuilding> Buildings
        {
            get
            {
                IEnumerable<IfcRelAggregates> aggregate = IsDecomposedBy.OfType<IfcRelAggregates>();
                foreach (IfcRelAggregates rel in aggregate)
                {
                    foreach (IfcObjectDefinition definition in rel.RelatedObjects)
                    {
                        var site = definition as IfcSite;
                        if (site != null)
                            foreach (var building in site.Buildings)
                                yield return building;
                        if (definition is IfcBuilding)
                            yield return (definition as IfcBuilding);
                    }
                }
            }
        }

        public IEnumerable<Xbim.Ifc4.Interfaces.IIfcSpatialStructureElement> SpatialStructuralElements
        {
            get
            {
                IEnumerable<IfcRelAggregates> aggregate = IsDecomposedBy.OfType<IfcRelAggregates>();
                return aggregate.SelectMany(rel => rel.RelatedObjects.OfType<IfcSpatialStructureElement>());
            }
        }

        /// <summary>
        /// Makes a name out of the fields of this project
        /// </summary>
        /// <returns></returns>
        public string BuildName()
        {
            List<string> tokens = new List<string>();
            if (!string.IsNullOrWhiteSpace(Name)) tokens.Add(Name);
            else if (!string.IsNullOrWhiteSpace(LongName)) tokens.Add(LongName);
            else if (!string.IsNullOrWhiteSpace(Description)) tokens.Add(Description);
            if (!string.IsNullOrWhiteSpace(Phase)) tokens.Add(Phase);
            return string.Join(", ", tokens);
        }


        public IfcNamedUnit AreaUnit
        {
            get
            {
                return UnitsInContext.AreaUnit;                 
            }
            
        }

        public void SetOrChangeSiUnit(IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                             IfcSIPrefix? siUnitPrefix)
        {
            if (UnitsInContext == null)
            {
                UnitsInContext = Model.Instances.New<IfcUnitAssignment>();
            }
            IfcUnitAssignment unitsAssignment = UnitsInContext;
            unitsAssignment.SetOrChangeSiUnit(unitType, siUnitName, siUnitPrefix);
        }

        public void SetOrChangeConversionUnit(IfcUnitEnum unitType, ConversionBasedUnit conversionUnit)
        {
           
            if (UnitsInContext == null)
                UnitsInContext = Model.Instances.New<IfcUnitAssignment>();
            IfcUnitAssignment unitsAssignment = UnitsInContext;
            unitsAssignment.SetOrChangeConversionUnit(unitType, conversionUnit);
        }


      

        public IfcGeometricRepresentationContext PlanContext
        {
            get
            {
                return RepresentationContexts.FirstOrDefault < IfcGeometricRepresentationContext>(r => r.ContextType == "Plan");
            }
            
        }


        #region Decomposition methods

        /// <summary>
        ///   Adds Site to the IsDecomposedBy Collection.
        /// </summary>
        public  void AddSite(IfcSite site)
        {
            var decomposition = IsDecomposedBy.FirstOrDefault();
            if (decomposition==null) //none defined create the relationship
            {
                var relSub = Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = this;
                relSub.RelatedObjects.Add(site);
            }
            else
                decomposition.RelatedObjects.Add(site);
        }

        public IEnumerable<Xbim.Ifc4.Interfaces.IIfcSite> Sites
        {
            get
            {
                var aggregate = IsDecomposedBy.OfType<IfcRelAggregates>();
                return from rel in aggregate from definition in Enumerable.OfType<IfcSite>(rel.RelatedObjects) select definition;
            }
        }

        /// <summary>
        ///   Adds Building to the IsDecomposedBy Collection.
        /// </summary>
        public  void AddBuilding( IfcBuilding building)
        {
            var decomposition = IsDecomposedBy.FirstOrDefault();
            if (decomposition==null) //none defined create the relationship
            {
                var relSub = Model.Instances.New<IfcRelAggregates>();
                relSub.RelatingObject = this;
                relSub.RelatedObjects.Add(building);
            }
            else
                decomposition.RelatedObjects.Add(building);
        }

        #endregion
   

    }

}
