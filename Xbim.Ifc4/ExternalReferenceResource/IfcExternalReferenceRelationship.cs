// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ExternalReferenceResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcExternalReferenceRelationship
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcExternalReferenceRelationship : IIfcResourceLevelRelationship
	{
		IIfcExternalReference @RelatingReference { get;  set; }
		IEnumerable<IIfcResourceObjectSelect> @RelatedResourceObjects { get; }
	
	}
}

namespace Xbim.Ifc4.ExternalReferenceResource
{
	[ExpressType("IfcExternalReferenceRelationship", 1173)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcExternalReferenceRelationship : IfcResourceLevelRelationship, IInstantiableEntity, IIfcExternalReferenceRelationship, IContainsEntityReferences, IContainsIndexedReferences, IEquatable<@IfcExternalReferenceRelationship>
	{
		#region IIfcExternalReferenceRelationship explicit implementation
		IIfcExternalReference IIfcExternalReferenceRelationship.RelatingReference { 
			get { return @RelatingReference; } 
 
 
			set { RelatingReference = value as IfcExternalReference;}
		}	
		IEnumerable<IIfcResourceObjectSelect> IIfcExternalReferenceRelationship.RelatedResourceObjects { 
			get { return @RelatedResourceObjects; } 
		}	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcExternalReferenceRelationship(IModel model) : base(model) 		{ 
			Model = model; 
			_relatedResourceObjects = new ItemSet<IfcResourceObjectSelect>( this, 0,  4);
		}

		#region Explicit attribute fields
		private IfcExternalReference _relatingReference;
		private ItemSet<IfcResourceObjectSelect> _relatedResourceObjects;
		#endregion
	
		#region Explicit attribute properties
		[IndexedProperty]
		[EntityAttribute(3, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 3)]
		public IfcExternalReference @RelatingReference 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _relatingReference;
				((IPersistEntity)this).Activate(false);
				return _relatingReference;
			} 
			set
			{
				SetValue( v =>  _relatingReference = v, _relatingReference, value,  "RelatingReference", 3);
			} 
		}	
		[IndexedProperty]
		[EntityAttribute(4, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 4)]
		public ItemSet<IfcResourceObjectSelect> @RelatedResourceObjects 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _relatedResourceObjects;
				((IPersistEntity)this).Activate(false);
				return _relatedResourceObjects;
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
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 2: 
					_relatingReference = (IfcExternalReference)(value.EntityVal);
					return;
				case 3: 
					_relatedResourceObjects.InternalAdd((IfcResourceObjectSelect)value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcExternalReferenceRelationship other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcExternalReferenceRelationship
            var root = (@IfcExternalReferenceRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcExternalReferenceRelationship left, @IfcExternalReferenceRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcExternalReferenceRelationship left, @IfcExternalReferenceRelationship right)
        {
            return !(left == right);
        }

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@RelatingReference != null)
					yield return @RelatingReference;
				foreach(var entity in @RelatedResourceObjects)
					yield return entity;
			}
		}
		#endregion


		#region IContainsIndexedReferences
        IEnumerable<IPersistEntity> IContainsIndexedReferences.IndexedReferences 
		{ 
			get
			{
				if (@RelatingReference != null)
					yield return @RelatingReference;
				foreach(var entity in @RelatedResourceObjects)
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