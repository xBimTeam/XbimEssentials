// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;

//## Custom using statements
//##

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc4x3.GeometricModelResource
{
	public partial class @IfcSurfaceCurveSweptAreaSolid : IIfcSurfaceCurveSweptAreaSolid
	{

		[CrossSchemaAttribute(typeof(IIfcSurfaceCurveSweptAreaSolid), 3)]
		IIfcCurve IIfcSurfaceCurveSweptAreaSolid.Directrix 
		{ 
			get
			{
				return Directrix;
			} 
			set
			{
				Directrix = value as GeometryResource.IfcCurve;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcSurfaceCurveSweptAreaSolid), 4)]
		Ifc4.MeasureResource.IfcParameterValue? IIfcSurfaceCurveSweptAreaSolid.StartParam 
		{ 
			get
			{
				//## Custom code
				return new Ifc4.MeasureResource.IfcParameterValue((MeasureResource.IfcParameterValue)StartParam);
				//##
			}
			set
			{
				//## Custom code
				StartParam = value.HasValue ? 
					new MeasureResource.IfcParameterValue(value.Value) :  
					 default(MeasureResource.IfcParameterValue);
				//##
			}
		}

		[CrossSchemaAttribute(typeof(IIfcSurfaceCurveSweptAreaSolid), 5)]
		Ifc4.MeasureResource.IfcParameterValue? IIfcSurfaceCurveSweptAreaSolid.EndParam 
		{
			get
			{
				//## Custom code
				return new Ifc4.MeasureResource.IfcParameterValue((MeasureResource.IfcParameterValue)EndParam);
				//##
			}
			set
			{
				//## Custom code
				EndParam = value.HasValue ?
					new MeasureResource.IfcParameterValue(value.Value) :
					 default(MeasureResource.IfcParameterValue);
				//##
			}
		}

		[CrossSchemaAttribute(typeof(IIfcSurfaceCurveSweptAreaSolid), 6)]
		IIfcSurface IIfcSurfaceCurveSweptAreaSolid.ReferenceSurface 
		{ 
			get
			{
				return ReferenceSurface;
			} 
			set
			{
				ReferenceSurface = value as GeometryResource.IfcSurface;
				
			}
		}
	//## Custom code
	//##
	}
}