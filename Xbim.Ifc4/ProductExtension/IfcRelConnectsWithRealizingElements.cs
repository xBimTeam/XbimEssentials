// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.MeasureResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProductExtension;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcRelConnectsWithRealizingElements
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcRelConnectsWithRealizingElements : IIfcRelConnectsElements
	{
		IEnumerable<IIfcElement> @RealizingElements { get; }
		IfcLabel? @ConnectionType { get; }
	
	}
}

namespace Xbim.Ifc4.ProductExtension
{
	[ExpressType("IfcRelConnectsWithRealizingElements", 313)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcRelConnectsWithRealizingElements : IfcRelConnectsElements, IInstantiableEntity, IIfcRelConnectsWithRealizingElements, IContainsEntityReferences, IContainsIndexedReferences, IEquatable<@IfcRelConnectsWithRealizingElements>
	{
		#region IIfcRelConnectsWithRealizingElements explicit implementation
		IEnumerable<IIfcElement> IIfcRelConnectsWithRealizingElements.RealizingElements { get { return @RealizingElements; } }	
		IfcLabel? IIfcRelConnectsWithRealizingElements.ConnectionType { get { return @ConnectionType; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcRelConnectsWithRealizingElements(IModel model) : base(model) 		{ 
			Model = model; 
			_realizingElements = new ItemSet<IfcElement>( this, 0,  8);
		}

		#region Explicit attribute fields
		private ItemSet<IfcElement> _realizingElements;
		private IfcLabel? _connectionType;
		#endregion
	
		#region Explicit attribute properties
		[IndexedProperty]
		[EntityAttribute(8, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 8)]
		public ItemSet<IfcElement> @RealizingElements 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _realizingElements;
				((IPersistEntity)this).Activate(false);
				return _realizingElements;
			} 
		}	
		[EntityAttribute(9, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 9)]
		public IfcLabel? @ConnectionType 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _connectionType;
				((IPersistEntity)this).Activate(false);
				return _connectionType;
			} 
			set
			{
				SetValue( v =>  _connectionType = v, _connectionType, value,  "ConnectionType", 9);
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
					_realizingElements.InternalAdd((IfcElement)value.EntityVal);
					return;
				case 8: 
					_connectionType = value.StringVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcRelConnectsWithRealizingElements other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcRelConnectsWithRealizingElements
            var root = (@IfcRelConnectsWithRealizingElements)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcRelConnectsWithRealizingElements left, @IfcRelConnectsWithRealizingElements right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcRelConnectsWithRealizingElements left, @IfcRelConnectsWithRealizingElements right)
        {
            return !(left == right);
        }

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@OwnerHistory != null)
					yield return @OwnerHistory;
				if (@ConnectionGeometry != null)
					yield return @ConnectionGeometry;
				if (@RelatingElement != null)
					yield return @RelatingElement;
				if (@RelatedElement != null)
					yield return @RelatedElement;
				foreach(var entity in @RealizingElements)
					yield return entity;
			}
		}
		#endregion


		#region IContainsIndexedReferences
        IEnumerable<IPersistEntity> IContainsIndexedReferences.IndexedReferences 
		{ 
			get
			{
				if (@RelatingElement != null)
					yield return @RelatingElement;
				if (@RelatedElement != null)
					yield return @RelatedElement;
				foreach(var entity in @RealizingElements)
					yield return entity;
				
			} 
		}
		#endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}