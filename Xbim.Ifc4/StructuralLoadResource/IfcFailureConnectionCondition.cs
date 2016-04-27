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
using Xbim.Ifc4.StructuralLoadResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcFailureConnectionCondition
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcFailureConnectionCondition : IIfcStructuralConnectionCondition
	{
		IfcForceMeasure? @TensionFailureX { get; }
		IfcForceMeasure? @TensionFailureY { get; }
		IfcForceMeasure? @TensionFailureZ { get; }
		IfcForceMeasure? @CompressionFailureX { get; }
		IfcForceMeasure? @CompressionFailureY { get; }
		IfcForceMeasure? @CompressionFailureZ { get; }
	
	}
}

namespace Xbim.Ifc4.StructuralLoadResource
{
	[ExpressType("IfcFailureConnectionCondition", 640)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcFailureConnectionCondition : IfcStructuralConnectionCondition, IInstantiableEntity, IIfcFailureConnectionCondition, IEquatable<@IfcFailureConnectionCondition>
	{
		#region IIfcFailureConnectionCondition explicit implementation
		IfcForceMeasure? IIfcFailureConnectionCondition.TensionFailureX { get { return @TensionFailureX; } }	
		IfcForceMeasure? IIfcFailureConnectionCondition.TensionFailureY { get { return @TensionFailureY; } }	
		IfcForceMeasure? IIfcFailureConnectionCondition.TensionFailureZ { get { return @TensionFailureZ; } }	
		IfcForceMeasure? IIfcFailureConnectionCondition.CompressionFailureX { get { return @CompressionFailureX; } }	
		IfcForceMeasure? IIfcFailureConnectionCondition.CompressionFailureY { get { return @CompressionFailureY; } }	
		IfcForceMeasure? IIfcFailureConnectionCondition.CompressionFailureZ { get { return @CompressionFailureZ; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcFailureConnectionCondition(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcForceMeasure? _tensionFailureX;
		private IfcForceMeasure? _tensionFailureY;
		private IfcForceMeasure? _tensionFailureZ;
		private IfcForceMeasure? _compressionFailureX;
		private IfcForceMeasure? _compressionFailureY;
		private IfcForceMeasure? _compressionFailureZ;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(2, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 2)]
		public IfcForceMeasure? @TensionFailureX 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _tensionFailureX;
				((IPersistEntity)this).Activate(false);
				return _tensionFailureX;
			} 
			set
			{
				SetValue( v =>  _tensionFailureX = v, _tensionFailureX, value,  "TensionFailureX", 2);
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 3)]
		public IfcForceMeasure? @TensionFailureY 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _tensionFailureY;
				((IPersistEntity)this).Activate(false);
				return _tensionFailureY;
			} 
			set
			{
				SetValue( v =>  _tensionFailureY = v, _tensionFailureY, value,  "TensionFailureY", 3);
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 4)]
		public IfcForceMeasure? @TensionFailureZ 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _tensionFailureZ;
				((IPersistEntity)this).Activate(false);
				return _tensionFailureZ;
			} 
			set
			{
				SetValue( v =>  _tensionFailureZ = v, _tensionFailureZ, value,  "TensionFailureZ", 4);
			} 
		}	
		[EntityAttribute(5, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public IfcForceMeasure? @CompressionFailureX 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _compressionFailureX;
				((IPersistEntity)this).Activate(false);
				return _compressionFailureX;
			} 
			set
			{
				SetValue( v =>  _compressionFailureX = v, _compressionFailureX, value,  "CompressionFailureX", 5);
			} 
		}	
		[EntityAttribute(6, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 6)]
		public IfcForceMeasure? @CompressionFailureY 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _compressionFailureY;
				((IPersistEntity)this).Activate(false);
				return _compressionFailureY;
			} 
			set
			{
				SetValue( v =>  _compressionFailureY = v, _compressionFailureY, value,  "CompressionFailureY", 6);
			} 
		}	
		[EntityAttribute(7, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 7)]
		public IfcForceMeasure? @CompressionFailureZ 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _compressionFailureZ;
				((IPersistEntity)this).Activate(false);
				return _compressionFailureZ;
			} 
			set
			{
				SetValue( v =>  _compressionFailureZ = v, _compressionFailureZ, value,  "CompressionFailureZ", 7);
			} 
		}	
		#endregion





		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 1: 
					_tensionFailureX = value.RealVal;
					return;
				case 2: 
					_tensionFailureY = value.RealVal;
					return;
				case 3: 
					_tensionFailureZ = value.RealVal;
					return;
				case 4: 
					_compressionFailureX = value.RealVal;
					return;
				case 5: 
					_compressionFailureY = value.RealVal;
					return;
				case 6: 
					_compressionFailureZ = value.RealVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcFailureConnectionCondition other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcFailureConnectionCondition
            var root = (@IfcFailureConnectionCondition)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcFailureConnectionCondition left, @IfcFailureConnectionCondition right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcFailureConnectionCondition left, @IfcFailureConnectionCondition right)
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