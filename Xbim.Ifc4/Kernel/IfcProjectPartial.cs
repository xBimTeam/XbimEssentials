using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;

namespace Xbim.Ifc4.Interfaces
{
	public partial interface @IIfcProject
	{

		void Initialize(ProjectUnits units);
		IEnumerable<IIfcSite> Sites { get; }
		IEnumerable<IIfcBuilding> Buildings { get; }
		IEnumerable<IIfcSpatialStructureElement> SpatialStructuralElements { get; }
	}
}

namespace Xbim.Ifc4.Kernel
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
			var ua = model.Instances.New<IfcUnitAssignment>();

			if (units == ProjectUnits.SIUnitsUK)
			{
				ua.SetOrChangeSiUnit(IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE, IfcSIPrefix.MILLI);
				ua.SetOrChangeSiUnit(IfcUnitEnum.AREAUNIT, IfcSIUnitName.SQUARE_METRE, null);
				ua.SetOrChangeSiUnit(IfcUnitEnum.VOLUMEUNIT, IfcSIUnitName.CUBIC_METRE, null);
				ua.SetOrChangeSiUnit(IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM, IfcSIPrefix.KILO);
			}
			else if (units == ProjectUnits.ImperialUnits || units == ProjectUnits.USCustomaryUnits)
			{
				ua.SetOrChangeConversionUnit(IfcUnitEnum.LENGTHUNIT, ConversionBasedUnit.Foot);
				ua.SetOrChangeConversionUnit(IfcUnitEnum.AREAUNIT, ConversionBasedUnit.SquareFoot);
				ua.SetOrChangeConversionUnit(IfcUnitEnum.VOLUMEUNIT, ConversionBasedUnit.CubicFoot);
				ua.SetOrChangeConversionUnit(IfcUnitEnum.MASSUNIT, ConversionBasedUnit.Pound);
			}

			ua.SetOrChangeSiUnit(IfcUnitEnum.SOLIDANGLEUNIT, IfcSIUnitName.STERADIAN, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.PLANEANGLEUNIT, IfcSIUnitName.RADIAN, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.TIMEUNIT, IfcSIUnitName.SECOND, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT, IfcSIUnitName.KELVIN, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT, IfcSIUnitName.DEGREE_CELSIUS, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.POWERUNIT, IfcSIUnitName.WATT, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.FORCEUNIT, IfcSIUnitName.NEWTON, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.ILLUMINANCEUNIT, IfcSIUnitName.LUX, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.LUMINOUSFLUXUNIT, IfcSIUnitName.LUMEN, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.LUMINOUSINTENSITYUNIT, IfcSIUnitName.CANDELA, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.PRESSUREUNIT, IfcSIUnitName.PASCAL, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.ELECTRICCURRENTUNIT, IfcSIUnitName.AMPERE, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.ELECTRICVOLTAGEUNIT, IfcSIUnitName.VOLT, null);
			ua.SetOrChangeSiUnit(IfcUnitEnum.FREQUENCYUNIT, IfcSIUnitName.HERTZ, null);
			UnitsInContext = ua;

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

		#region Decomposition methods

		/// <summary>
		///   Adds Site to the IsDecomposedBy Collection.
		/// </summary>
		public void AddSite(IfcSite site)
		{
			var decomposition = IsDecomposedBy.FirstOrDefault();
			if (decomposition == null) //none defined create the relationship
			{
				var relSub = Model.Instances.New<IfcRelAggregates>();
				relSub.RelatingObject = this;
				relSub.RelatedObjects.Add(site);
			}
			else
				decomposition.RelatedObjects.Add(site);
		}

		public IEnumerable<IIfcSite> Sites
		{
			get
			{
				return IsDecomposedBy.SelectMany(rel => Enumerable.OfType<IfcSite>(rel.RelatedObjects));
			}
		}

		/// <summary>
		///   Adds Building to the IsDecomposedBy Collection.
		/// </summary>
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
				decomposition.RelatedObjects.Add(building);
		}

		/// <summary>
		/// Returns all buildings at the highest level of spatial structural decomposition (i.e. root buildings)
		/// </summary>
		public IEnumerable<IIfcBuilding> Buildings
		{
			get
			{
				foreach (var rel in IsDecomposedBy)
				{
					foreach (var definition in rel.RelatedObjects)
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

		public IEnumerable<IIfcSpatialStructureElement> SpatialStructuralElements
		{
			get
			{
				return IsDecomposedBy.SelectMany(rel => rel.RelatedObjects.OfType<IfcSpatialStructureElement>());
			}
		}
		#endregion
	}
}