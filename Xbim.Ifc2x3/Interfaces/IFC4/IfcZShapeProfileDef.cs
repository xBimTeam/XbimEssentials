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

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class @IfcZShapeProfileDef : IIfcZShapeProfileDef
	{
		Ifc4.MeasureResource.IfcPositiveLengthMeasure IIfcZShapeProfileDef.Depth 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcPositiveLengthMeasure(Depth);
			} 
			set
			{
				Depth = new MeasureResource.IfcPositiveLengthMeasure(value);
				
			}
		}
		Ifc4.MeasureResource.IfcPositiveLengthMeasure IIfcZShapeProfileDef.FlangeWidth 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcPositiveLengthMeasure(FlangeWidth);
			} 
			set
			{
				FlangeWidth = new MeasureResource.IfcPositiveLengthMeasure(value);
				
			}
		}
		Ifc4.MeasureResource.IfcPositiveLengthMeasure IIfcZShapeProfileDef.WebThickness 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcPositiveLengthMeasure(WebThickness);
			} 
			set
			{
				WebThickness = new MeasureResource.IfcPositiveLengthMeasure(value);
				
			}
		}
		Ifc4.MeasureResource.IfcPositiveLengthMeasure IIfcZShapeProfileDef.FlangeThickness 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcPositiveLengthMeasure(FlangeThickness);
			} 
			set
			{
				FlangeThickness = new MeasureResource.IfcPositiveLengthMeasure(value);
				
			}
		}
		Ifc4.MeasureResource.IfcNonNegativeLengthMeasure? IIfcZShapeProfileDef.FilletRadius 
		{ 
			get
			{
				if (!FilletRadius.HasValue) return null;
				return new Ifc4.MeasureResource.IfcNonNegativeLengthMeasure(FilletRadius.Value);
			} 
			set
			{
				FilletRadius = value.HasValue ? 
					new MeasureResource.IfcPositiveLengthMeasure(value.Value) :  
					 new MeasureResource.IfcPositiveLengthMeasure?() ;
				
			}
		}
		Ifc4.MeasureResource.IfcNonNegativeLengthMeasure? IIfcZShapeProfileDef.EdgeRadius 
		{ 
			get
			{
				if (!EdgeRadius.HasValue) return null;
				return new Ifc4.MeasureResource.IfcNonNegativeLengthMeasure(EdgeRadius.Value);
			} 
			set
			{
				EdgeRadius = value.HasValue ? 
					new MeasureResource.IfcPositiveLengthMeasure(value.Value) :  
					 new MeasureResource.IfcPositiveLengthMeasure?() ;
				
			}
		}
	//## Custom code
	//##
	}
}