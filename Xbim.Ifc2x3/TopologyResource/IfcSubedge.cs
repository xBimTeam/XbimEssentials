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
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.TopologyResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcSubedge
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcSubedge : IIfcEdge
	{
		IIfcEdge @ParentEdge { get; }
	
	}
}

namespace Xbim.Ifc2x3.TopologyResource
{
	[ExpressType("IfcSubedge", 487)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcSubedge : IfcEdge, IInstantiableEntity, IIfcSubedge, IContainsEntityReferences, IEquatable<@IfcSubedge>
	{
		#region IIfcSubedge explicit implementation
		IIfcEdge IIfcSubedge.ParentEdge { get { return @ParentEdge; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcSubedge(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcEdge _parentEdge;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(3, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 5)]
		public IfcEdge @ParentEdge 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _parentEdge;
				((IPersistEntity)this).Activate(false);
				return _parentEdge;
			} 
			set
			{
				SetValue( v =>  _parentEdge = v, _parentEdge, value,  "ParentEdge", 3);
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
					_parentEdge = (IfcEdge)(value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcSubedge other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcSubedge
            var root = (@IfcSubedge)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcSubedge left, @IfcSubedge right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcSubedge left, @IfcSubedge right)
        {
            return !(left == right);
        }

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@EdgeStart != null)
					yield return @EdgeStart;
				if (@EdgeEnd != null)
					yield return @EdgeEnd;
				if (@ParentEdge != null)
					yield return @ParentEdge;
			}
		}
		#endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}