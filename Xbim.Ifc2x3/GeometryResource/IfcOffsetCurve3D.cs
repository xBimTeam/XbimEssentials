// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.MeasureResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.GeometryResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcOffsetCurve3D
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcOffsetCurve3D : IIfcCurve
	{
		IIfcCurve @BasisCurve { get; }
		IfcLengthMeasure @Distance { get; }
		bool? @SelfIntersect { get; }
		IIfcDirection @RefDirection { get; }
	
	}
}

namespace Xbim.Ifc2x3.GeometryResource
{
	[ExpressType("IfcOffsetCurve3D", 67)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcOffsetCurve3D : IfcCurve, IInstantiableEntity, IIfcOffsetCurve3D, IEquatable<@IfcOffsetCurve3D>
	{
		#region IIfcOffsetCurve3D explicit implementation
		IIfcCurve IIfcOffsetCurve3D.BasisCurve { get { return @BasisCurve; } }	
		IfcLengthMeasure IIfcOffsetCurve3D.Distance { get { return @Distance; } }	
		bool? IIfcOffsetCurve3D.SelfIntersect { get { return @SelfIntersect; } }	
		IIfcDirection IIfcOffsetCurve3D.RefDirection { get { return @RefDirection; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcOffsetCurve3D(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcCurve _basisCurve;
		private IfcLengthMeasure _distance;
		private bool? _selfIntersect;
		private IfcDirection _refDirection;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(1, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 3)]
		public IfcCurve @BasisCurve 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _basisCurve;
				((IPersistEntity)this).Activate(false);
				return _basisCurve;
			} 
			set
			{
				SetValue( v =>  _basisCurve = v, _basisCurve, value,  "BasisCurve", 1);
			} 
		}	
		[EntityAttribute(2, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 4)]
		public IfcLengthMeasure @Distance 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _distance;
				((IPersistEntity)this).Activate(false);
				return _distance;
			} 
			set
			{
				SetValue( v =>  _distance = v, _distance, value,  "Distance", 2);
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public bool? @SelfIntersect 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _selfIntersect;
				((IPersistEntity)this).Activate(false);
				return _selfIntersect;
			} 
			set
			{
				SetValue( v =>  _selfIntersect = v, _selfIntersect, value,  "SelfIntersect", 3);
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 6)]
		public IfcDirection @RefDirection 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _refDirection;
				((IPersistEntity)this).Activate(false);
				return _refDirection;
			} 
			set
			{
				SetValue( v =>  _refDirection = v, _refDirection, value,  "RefDirection", 4);
			} 
		}	
		#endregion





		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					_basisCurve = (IfcCurve)(value.EntityVal);
					return;
				case 1: 
					_distance = value.RealVal;
					return;
				case 2: 
					_selfIntersect = value.BooleanVal;
					return;
				case 3: 
					_refDirection = (IfcDirection)(value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcOffsetCurve3D other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcOffsetCurve3D
            var root = (@IfcOffsetCurve3D)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcOffsetCurve3D left, @IfcOffsetCurve3D right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcOffsetCurve3D left, @IfcOffsetCurve3D right)
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