// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.MaterialResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcMaterialLayerSetUsage
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcMaterialLayerSetUsage : IIfcMaterialUsageDefinition
	{
		IIfcMaterialLayerSet @ForLayerSet { get; }
		IfcLayerSetDirectionEnum @LayerSetDirection { get; }
		IfcDirectionSenseEnum @DirectionSense { get; }
		IfcLengthMeasure @OffsetFromReferenceLine { get; }
		IfcPositiveLengthMeasure? @ReferenceExtent { get; }
	
	}
}

namespace Xbim.Ifc4.MaterialResource
{
	[ExpressType("IfcMaterialLayerSetUsage", 165)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcMaterialLayerSetUsage : IfcMaterialUsageDefinition, IInstantiableEntity, IIfcMaterialLayerSetUsage, IEquatable<@IfcMaterialLayerSetUsage>
	{
		#region IIfcMaterialLayerSetUsage explicit implementation
		IIfcMaterialLayerSet IIfcMaterialLayerSetUsage.ForLayerSet { get { return @ForLayerSet; } }	
		IfcLayerSetDirectionEnum IIfcMaterialLayerSetUsage.LayerSetDirection { get { return @LayerSetDirection; } }	
		IfcDirectionSenseEnum IIfcMaterialLayerSetUsage.DirectionSense { get { return @DirectionSense; } }	
		IfcLengthMeasure IIfcMaterialLayerSetUsage.OffsetFromReferenceLine { get { return @OffsetFromReferenceLine; } }	
		IfcPositiveLengthMeasure? IIfcMaterialLayerSetUsage.ReferenceExtent { get { return @ReferenceExtent; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcMaterialLayerSetUsage(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcMaterialLayerSet _forLayerSet;
		private IfcLayerSetDirectionEnum _layerSetDirection;
		private IfcDirectionSenseEnum _directionSense;
		private IfcLengthMeasure _offsetFromReferenceLine;
		private IfcPositiveLengthMeasure? _referenceExtent;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(1, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 2)]
		public IfcMaterialLayerSet @ForLayerSet 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _forLayerSet;
				((IPersistEntity)this).Activate(false);
				return _forLayerSet;
			} 
			set
			{
				SetValue( v =>  _forLayerSet = v, _forLayerSet, value,  "ForLayerSet", 1);
			} 
		}	
		[EntityAttribute(2, EntityAttributeState.Mandatory, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 3)]
		public IfcLayerSetDirectionEnum @LayerSetDirection 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _layerSetDirection;
				((IPersistEntity)this).Activate(false);
				return _layerSetDirection;
			} 
			set
			{
				SetValue( v =>  _layerSetDirection = v, _layerSetDirection, value,  "LayerSetDirection", 2);
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Mandatory, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 4)]
		public IfcDirectionSenseEnum @DirectionSense 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _directionSense;
				((IPersistEntity)this).Activate(false);
				return _directionSense;
			} 
			set
			{
				SetValue( v =>  _directionSense = v, _directionSense, value,  "DirectionSense", 3);
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public IfcLengthMeasure @OffsetFromReferenceLine 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _offsetFromReferenceLine;
				((IPersistEntity)this).Activate(false);
				return _offsetFromReferenceLine;
			} 
			set
			{
				SetValue( v =>  _offsetFromReferenceLine = v, _offsetFromReferenceLine, value,  "OffsetFromReferenceLine", 4);
			} 
		}	
		[EntityAttribute(5, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 6)]
		public IfcPositiveLengthMeasure? @ReferenceExtent 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _referenceExtent;
				((IPersistEntity)this).Activate(false);
				return _referenceExtent;
			} 
			set
			{
				SetValue( v =>  _referenceExtent = v, _referenceExtent, value,  "ReferenceExtent", 5);
			} 
		}	
		#endregion





		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					_forLayerSet = (IfcMaterialLayerSet)(value.EntityVal);
					return;
				case 1: 
                    _layerSetDirection = (IfcLayerSetDirectionEnum) System.Enum.Parse(typeof (IfcLayerSetDirectionEnum), value.EnumVal, true);
					return;
				case 2: 
                    _directionSense = (IfcDirectionSenseEnum) System.Enum.Parse(typeof (IfcDirectionSenseEnum), value.EnumVal, true);
					return;
				case 3: 
					_offsetFromReferenceLine = value.RealVal;
					return;
				case 4: 
					_referenceExtent = value.RealVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcMaterialLayerSetUsage other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcMaterialLayerSetUsage
            var root = (@IfcMaterialLayerSetUsage)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcMaterialLayerSetUsage left, @IfcMaterialLayerSetUsage right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcMaterialLayerSetUsage left, @IfcMaterialLayerSetUsage right)
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