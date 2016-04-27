// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.StructuralAnalysisDomain;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.ProductExtension;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcElement
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcElement : IIfcProduct, IfcStructuralActivityAssignmentSelect
	{
		IfcIdentifier? @Tag { get; }
		IEnumerable<IIfcRelConnectsStructuralElement> @HasStructuralMember {  get; }
		IEnumerable<IIfcRelFillsElement> @FillsVoids {  get; }
		IEnumerable<IIfcRelConnectsElements> @ConnectedTo {  get; }
		IEnumerable<IIfcRelCoversBldgElements> @HasCoverings {  get; }
		IEnumerable<IIfcRelProjectsElement> @HasProjections {  get; }
		IEnumerable<IIfcRelReferencedInSpatialStructure> @ReferencedInStructures {  get; }
		IEnumerable<IIfcRelConnectsPortToElement> @HasPorts {  get; }
		IEnumerable<IIfcRelVoidsElement> @HasOpenings {  get; }
		IEnumerable<IIfcRelConnectsWithRealizingElements> @IsConnectionRealization {  get; }
		IEnumerable<IIfcRelSpaceBoundary> @ProvidesBoundaries {  get; }
		IEnumerable<IIfcRelConnectsElements> @ConnectedFrom {  get; }
		IEnumerable<IIfcRelContainedInSpatialStructure> @ContainedInStructure {  get; }
	
	}
}

namespace Xbim.Ifc2x3.ProductExtension
{
	[ExpressType("IfcElement", 19)]
	// ReSharper disable once PartialTypeWithSinglePart
	public abstract partial class @IfcElement : IfcProduct, IIfcElement, IEquatable<@IfcElement>
	{
		#region IIfcElement explicit implementation
		IfcIdentifier? IIfcElement.Tag { get { return @Tag; } }	
		 
		IEnumerable<IIfcRelConnectsStructuralElement> IIfcElement.HasStructuralMember {  get { return @HasStructuralMember; } }
		IEnumerable<IIfcRelFillsElement> IIfcElement.FillsVoids {  get { return @FillsVoids; } }
		IEnumerable<IIfcRelConnectsElements> IIfcElement.ConnectedTo {  get { return @ConnectedTo; } }
		IEnumerable<IIfcRelCoversBldgElements> IIfcElement.HasCoverings {  get { return @HasCoverings; } }
		IEnumerable<IIfcRelProjectsElement> IIfcElement.HasProjections {  get { return @HasProjections; } }
		IEnumerable<IIfcRelReferencedInSpatialStructure> IIfcElement.ReferencedInStructures {  get { return @ReferencedInStructures; } }
		IEnumerable<IIfcRelConnectsPortToElement> IIfcElement.HasPorts {  get { return @HasPorts; } }
		IEnumerable<IIfcRelVoidsElement> IIfcElement.HasOpenings {  get { return @HasOpenings; } }
		IEnumerable<IIfcRelConnectsWithRealizingElements> IIfcElement.IsConnectionRealization {  get { return @IsConnectionRealization; } }
		IEnumerable<IIfcRelSpaceBoundary> IIfcElement.ProvidesBoundaries {  get { return @ProvidesBoundaries; } }
		IEnumerable<IIfcRelConnectsElements> IIfcElement.ConnectedFrom {  get { return @ConnectedFrom; } }
		IEnumerable<IIfcRelContainedInSpatialStructure> IIfcElement.ContainedInStructure {  get { return @ContainedInStructure; } }
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcElement(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcIdentifier? _tag;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(8, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 14)]
		public IfcIdentifier? @Tag 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _tag;
				((IPersistEntity)this).Activate(false);
				return _tag;
			} 
			set
			{
				SetValue( v =>  _tag = v, _tag, value,  "Tag", 8);
			} 
		}	
		#endregion



		#region Inverse attributes
		[InverseProperty("RelatingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 15)]
		public IEnumerable<IfcRelConnectsStructuralElement> @HasStructuralMember 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelConnectsStructuralElement>(e => Equals(e.RelatingElement), "RelatingElement", this);
			} 
		}
		[InverseProperty("RelatedBuildingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, 1, 16)]
		public IEnumerable<IfcRelFillsElement> @FillsVoids 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelFillsElement>(e => Equals(e.RelatedBuildingElement), "RelatedBuildingElement", this);
			} 
		}
		[InverseProperty("RelatingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 17)]
		public IEnumerable<IfcRelConnectsElements> @ConnectedTo 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelConnectsElements>(e => Equals(e.RelatingElement), "RelatingElement", this);
			} 
		}
		[InverseProperty("RelatingBuildingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 18)]
		public IEnumerable<IfcRelCoversBldgElements> @HasCoverings 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelCoversBldgElements>(e => Equals(e.RelatingBuildingElement), "RelatingBuildingElement", this);
			} 
		}
		[InverseProperty("RelatingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 19)]
		public IEnumerable<IfcRelProjectsElement> @HasProjections 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelProjectsElement>(e => Equals(e.RelatingElement), "RelatingElement", this);
			} 
		}
		[InverseProperty("RelatedElements")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 20)]
		public IEnumerable<IfcRelReferencedInSpatialStructure> @ReferencedInStructures 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelReferencedInSpatialStructure>(e => e.RelatedElements != null &&  e.RelatedElements.Contains(this), "RelatedElements", this);
			} 
		}
		[InverseProperty("RelatedElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 21)]
		public IEnumerable<IfcRelConnectsPortToElement> @HasPorts 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelConnectsPortToElement>(e => Equals(e.RelatedElement), "RelatedElement", this);
			} 
		}
		[InverseProperty("RelatingBuildingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 22)]
		public IEnumerable<IfcRelVoidsElement> @HasOpenings 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelVoidsElement>(e => Equals(e.RelatingBuildingElement), "RelatingBuildingElement", this);
			} 
		}
		[InverseProperty("RealizingElements")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 23)]
		public IEnumerable<IfcRelConnectsWithRealizingElements> @IsConnectionRealization 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelConnectsWithRealizingElements>(e => e.RealizingElements != null &&  e.RealizingElements.Contains(this), "RealizingElements", this);
			} 
		}
		[InverseProperty("RelatedBuildingElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 24)]
		public IEnumerable<IfcRelSpaceBoundary> @ProvidesBoundaries 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelSpaceBoundary>(e => Equals(e.RelatedBuildingElement), "RelatedBuildingElement", this);
			} 
		}
		[InverseProperty("RelatedElement")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 25)]
		public IEnumerable<IfcRelConnectsElements> @ConnectedFrom 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelConnectsElements>(e => Equals(e.RelatedElement), "RelatedElement", this);
			} 
		}
		[InverseProperty("RelatedElements")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, 1, 26)]
		public IEnumerable<IfcRelContainedInSpatialStructure> @ContainedInStructure 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelContainedInSpatialStructure>(e => e.RelatedElements != null &&  e.RelatedElements.Contains(this), "RelatedElements", this);
			} 
		}
		#endregion


		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
				case 1: 
				case 2: 
				case 3: 
				case 4: 
				case 5: 
				case 6: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 7: 
					_tag = value.StringVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcElement other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcElement
            var root = (@IfcElement)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcElement left, @IfcElement right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcElement left, @IfcElement right)
        {
            return !(left == right);
        }

        public static bool operator ==(@IfcElement left, IfcStructuralActivityAssignmentSelect right)
		{
			return left == right as @IfcElement;
		}

		public static bool operator !=(@IfcElement left, IfcStructuralActivityAssignmentSelect right)
		{
			return !(left == right);
		}

        #endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}