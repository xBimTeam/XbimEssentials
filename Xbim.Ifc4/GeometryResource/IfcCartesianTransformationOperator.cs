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
using Xbim.Ifc4.GeometryResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcCartesianTransformationOperator
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcCartesianTransformationOperator : IIfcGeometricRepresentationItem
	{
		IIfcDirection @Axis1 { get; }
		IIfcDirection @Axis2 { get; }
		IIfcCartesianPoint @LocalOrigin { get; }
		IfcReal? @Scale { get; }
		IfcReal @Scl  { get ; }
		IfcDimensionCount @Dim  { get ; }
	
	}
}

namespace Xbim.Ifc4.GeometryResource
{
	[ExpressType("IfcCartesianTransformationOperator", 146)]
	// ReSharper disable once PartialTypeWithSinglePart
	public abstract partial class @IfcCartesianTransformationOperator : IfcGeometricRepresentationItem, IIfcCartesianTransformationOperator, IEquatable<@IfcCartesianTransformationOperator>
	{
		#region IIfcCartesianTransformationOperator explicit implementation
		IIfcDirection IIfcCartesianTransformationOperator.Axis1 { get { return @Axis1; } }	
		IIfcDirection IIfcCartesianTransformationOperator.Axis2 { get { return @Axis2; } }	
		IIfcCartesianPoint IIfcCartesianTransformationOperator.LocalOrigin { get { return @LocalOrigin; } }	
		IfcReal? IIfcCartesianTransformationOperator.Scale { get { return @Scale; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcCartesianTransformationOperator(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcDirection _axis1;
		private IfcDirection _axis2;
		private IfcCartesianPoint _localOrigin;
		private IfcReal? _scale;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(1, EntityAttributeState.Optional, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 3)]
		public IfcDirection @Axis1 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _axis1;
				((IPersistEntity)this).Activate(false);
				return _axis1;
			} 
			set
			{
				SetValue( v =>  _axis1 = v, _axis1, value,  "Axis1", 1);
			} 
		}	
		[EntityAttribute(2, EntityAttributeState.Optional, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 4)]
		public IfcDirection @Axis2 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _axis2;
				((IPersistEntity)this).Activate(false);
				return _axis2;
			} 
			set
			{
				SetValue( v =>  _axis2 = v, _axis2, value,  "Axis2", 2);
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 5)]
		public IfcCartesianPoint @LocalOrigin 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _localOrigin;
				((IPersistEntity)this).Activate(false);
				return _localOrigin;
			} 
			set
			{
				SetValue( v =>  _localOrigin = v, _localOrigin, value,  "LocalOrigin", 3);
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 6)]
		public IfcReal? @Scale 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _scale;
				((IPersistEntity)this).Activate(false);
				return _scale;
			} 
			set
			{
				SetValue( v =>  _scale = v, _scale, value,  "Scale", 4);
			} 
		}	
		#endregion


		#region Derived attributes
		[EntityAttribute(0, EntityAttributeState.Derived, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 0)]
		public IfcReal @Scl 
		{
			get 
			{
				//## Getter for Scl
                return Scale ?? 1.0;
				//##
			}
		}

		[EntityAttribute(0, EntityAttributeState.Derived, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 0)]
		public IfcDimensionCount @Dim 
		{
			get 
			{
				//## Getter for Dim
			    return LocalOrigin.Dim;
			    //##
			}
		}

		#endregion



		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					_axis1 = (IfcDirection)(value.EntityVal);
					return;
				case 1: 
					_axis2 = (IfcDirection)(value.EntityVal);
					return;
				case 2: 
					_localOrigin = (IfcCartesianPoint)(value.EntityVal);
					return;
				case 3: 
					_scale = value.RealVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcCartesianTransformationOperator other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcCartesianTransformationOperator
            var root = (@IfcCartesianTransformationOperator)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcCartesianTransformationOperator left, @IfcCartesianTransformationOperator right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcCartesianTransformationOperator left, @IfcCartesianTransformationOperator right)
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