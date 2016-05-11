// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.GeometryResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.ProfileResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcArbitraryProfileDefWithVoids
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcArbitraryProfileDefWithVoids : IIfcArbitraryClosedProfileDef
	{
		IEnumerable<IIfcCurve> @InnerCurves { get; }
	
	}
}

namespace Xbim.Ifc2x3.ProfileResource
{
	[ExpressType("IfcArbitraryProfileDefWithVoids", 116)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcArbitraryProfileDefWithVoids : IfcArbitraryClosedProfileDef, IInstantiableEntity, IIfcArbitraryProfileDefWithVoids, IContainsEntityReferences, IEquatable<@IfcArbitraryProfileDefWithVoids>
	{
		#region IIfcArbitraryProfileDefWithVoids explicit implementation
		IEnumerable<IIfcCurve> IIfcArbitraryProfileDefWithVoids.InnerCurves { get { return @InnerCurves; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcArbitraryProfileDefWithVoids(IModel model) : base(model) 		{ 
			Model = model; 
			_innerCurves = new ItemSet<IfcCurve>( this, 0,  4);
		}

		#region Explicit attribute fields
		private ItemSet<IfcCurve> _innerCurves;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(4, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 4)]
		public ItemSet<IfcCurve> @InnerCurves 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _innerCurves;
				((IPersistEntity)this).Activate(false);
				return _innerCurves;
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
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 3: 
					_innerCurves.InternalAdd((IfcCurve)value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcArbitraryProfileDefWithVoids other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcArbitraryProfileDefWithVoids
            var root = (@IfcArbitraryProfileDefWithVoids)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcArbitraryProfileDefWithVoids left, @IfcArbitraryProfileDefWithVoids right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcArbitraryProfileDefWithVoids left, @IfcArbitraryProfileDefWithVoids right)
        {
            return !(left == right);
        }

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@OuterCurve != null)
					yield return @OuterCurve;
				foreach(var entity in @InnerCurves)
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