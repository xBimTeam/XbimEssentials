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
namespace Xbim.Ifc2x3.MeasureResource
{
	public partial class @IfcConversionBasedUnit : IIfcConversionBasedUnit
	{
		Ifc4.MeasureResource.IfcLabel IIfcConversionBasedUnit.Name 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcLabel(Name);
			} 
			set
			{
				Name = new IfcLabel(value);
				
			}
		}
		IIfcMeasureWithUnit IIfcConversionBasedUnit.ConversionFactor 
		{ 
			get
			{
				return ConversionFactor;
			} 
			set
			{
				ConversionFactor = value as IfcMeasureWithUnit;
				
			}
		}
		IEnumerable<IIfcExternalReferenceRelationship> IIfcConversionBasedUnit.HasExternalReference 
		{ 
			get
			{
				return Model.Instances.Where<IIfcExternalReferenceRelationship>(e => e.RelatedResourceObjects != null &&  e.RelatedResourceObjects.Contains(this), "RelatedResourceObjects", this);
			} 
		}
	//## Custom code
	//##
	}
}