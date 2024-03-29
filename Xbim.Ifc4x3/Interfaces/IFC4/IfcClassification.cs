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
namespace Xbim.Ifc4x3.ExternalReferenceResource
{
	public partial class @IfcClassification : IIfcClassification
	{

		[CrossSchemaAttribute(typeof(IIfcClassification), 1)]
		Ifc4.MeasureResource.IfcLabel? IIfcClassification.Source 
		{ 
			get
			{
				if (!Source.HasValue) return null;
				return new Ifc4.MeasureResource.IfcLabel(Source.Value);
			} 
			set
			{
				Source = value.HasValue ? 
					new MeasureResource.IfcLabel(value.Value) :  
					 new MeasureResource.IfcLabel?() ;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcClassification), 2)]
		Ifc4.MeasureResource.IfcLabel? IIfcClassification.Edition 
		{ 
			get
			{
				if (!Edition.HasValue) return null;
				return new Ifc4.MeasureResource.IfcLabel(Edition.Value);
			} 
			set
			{
				Edition = value.HasValue ? 
					new MeasureResource.IfcLabel(value.Value) :  
					 new MeasureResource.IfcLabel?() ;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcClassification), 3)]
		Ifc4.DateTimeResource.IfcDate? IIfcClassification.EditionDate 
		{ 
			get
			{
				if (!EditionDate.HasValue) return null;
				return new Ifc4.DateTimeResource.IfcDate(EditionDate.Value);
			} 
			set
			{
				EditionDate = value.HasValue ? 
					new DateTimeResource.IfcDate(value.Value) :  
					 new DateTimeResource.IfcDate?() ;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcClassification), 4)]
		Ifc4.MeasureResource.IfcLabel IIfcClassification.Name 
		{ 
			get
			{
				return new Ifc4.MeasureResource.IfcLabel(Name);
			} 
			set
			{
				Name = new MeasureResource.IfcLabel(value);
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcClassification), 5)]
		Ifc4.MeasureResource.IfcText? IIfcClassification.Description 
		{ 
			get
			{
				if (!Description.HasValue) return null;
				return new Ifc4.MeasureResource.IfcText(Description.Value);
			} 
			set
			{
				Description = value.HasValue ? 
					new MeasureResource.IfcText(value.Value) :  
					 new MeasureResource.IfcText?() ;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcClassification), 6)]
		Ifc4.ExternalReferenceResource.IfcURIReference? IIfcClassification.Location 
		{ 
			get
			{
				if (!Specification.HasValue) return null;
				return new Ifc4.ExternalReferenceResource.IfcURIReference(Specification.Value);
			} 
			set
			{
				Specification = value.HasValue ? 
					new MeasureResource.IfcURIReference(value.Value) :  
					 new MeasureResource.IfcURIReference?() ;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcClassification), 7)]
		IItemSet<Xbim.Ifc4.MeasureResource.IfcIdentifier> IIfcClassification.ReferenceTokens 
		{ 
			get
			{
			
				return new Common.Collections.ProxyValueSet<MeasureResource.IfcIdentifier, Ifc4.MeasureResource.IfcIdentifier>(ReferenceTokens, 
					s => new Ifc4.MeasureResource.IfcIdentifier(s), 
					t => new MeasureResource.IfcIdentifier(t));
			} 
		}
		IEnumerable<IIfcRelAssociatesClassification> IIfcClassification.ClassificationForObjects 
		{ 
			get
			{
				return Model.Instances.Where<IIfcRelAssociatesClassification>(e => (e.RelatingClassification as IfcClassification) == this, "RelatingClassification", this);
			} 
		}
		IEnumerable<IIfcClassificationReference> IIfcClassification.HasReferences 
		{ 
			get
			{
				return Model.Instances.Where<IIfcClassificationReference>(e => (e.ReferencedSource as IfcClassification) == this, "ReferencedSource", this);
			} 
		}
	//## Custom code
	//##
	}
}