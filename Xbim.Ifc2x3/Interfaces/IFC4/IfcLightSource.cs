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
namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
	public partial class @IfcLightSource : IIfcLightSource
	{
		Ifc4.MeasureResource.IfcLabel? IIfcLightSource.Name 
		{ 
			get
			{
				if (!Name.HasValue) return null;
				return new Ifc4.MeasureResource.IfcLabel(Name.Value);
			} 
			set
			{
				Name = value.HasValue ? 
					new MeasureResource.IfcLabel(value.Value) :  
					 new MeasureResource.IfcLabel?() ;
				
			}
		}
		IIfcColourRgb IIfcLightSource.LightColour 
		{ 
			get
			{
				return LightColour;
			} 
			set
			{
				LightColour = value as PresentationResource.IfcColourRgb;
				
			}
		}
		Ifc4.MeasureResource.IfcNormalisedRatioMeasure? IIfcLightSource.AmbientIntensity 
		{ 
			get
			{
				if (!AmbientIntensity.HasValue) return null;
				return new Ifc4.MeasureResource.IfcNormalisedRatioMeasure(AmbientIntensity.Value);
			} 
			set
			{
				AmbientIntensity = value.HasValue ? 
					new MeasureResource.IfcNormalisedRatioMeasure(value.Value) :  
					 new MeasureResource.IfcNormalisedRatioMeasure?() ;
				
			}
		}
		Ifc4.MeasureResource.IfcNormalisedRatioMeasure? IIfcLightSource.Intensity 
		{ 
			get
			{
				if (!Intensity.HasValue) return null;
				return new Ifc4.MeasureResource.IfcNormalisedRatioMeasure(Intensity.Value);
			} 
			set
			{
				Intensity = value.HasValue ? 
					new MeasureResource.IfcNormalisedRatioMeasure(value.Value) :  
					 new MeasureResource.IfcNormalisedRatioMeasure?() ;
				
			}
		}
	//## Custom code
	//##
	}
}