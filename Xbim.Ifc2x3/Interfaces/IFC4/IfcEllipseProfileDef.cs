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
	public partial class @IfcEllipseProfileDef : IIfcEllipseProfileDef
	{
		Ifc4.MeasureResource.IfcPositiveLengthMeasure IIfcEllipseProfileDef.SemiAxis1 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcPositiveLengthMeasure(SemiAxis1);
			} 
			set
			{
				SemiAxis1 = new MeasureResource.IfcPositiveLengthMeasure(value);
				
			}
		}
		Ifc4.MeasureResource.IfcPositiveLengthMeasure IIfcEllipseProfileDef.SemiAxis2 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcPositiveLengthMeasure(SemiAxis2);
			} 
			set
			{
				SemiAxis2 = new MeasureResource.IfcPositiveLengthMeasure(value);
				
			}
		}
	//## Custom code
	//##
	}
}