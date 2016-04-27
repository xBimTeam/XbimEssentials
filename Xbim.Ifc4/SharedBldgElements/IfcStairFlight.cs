// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.SharedBldgElements;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcStairFlight
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcStairFlight : IIfcBuildingElement
	{
		IfcInteger? @NumberOfRisers { get; }
		IfcInteger? @NumberOfTreads { get; }
		IfcPositiveLengthMeasure? @RiserHeight { get; }
		IfcPositiveLengthMeasure? @TreadLength { get; }
		IfcStairFlightTypeEnum? @PredefinedType { get; }
	
	}
}

namespace Xbim.Ifc4.SharedBldgElements
{
	[ExpressType("IfcStairFlight", 25)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcStairFlight : IfcBuildingElement, IInstantiableEntity, IIfcStairFlight, IEquatable<@IfcStairFlight>
	{
		#region IIfcStairFlight explicit implementation
		IfcInteger? IIfcStairFlight.NumberOfRisers { get { return @NumberOfRisers; } }	
		IfcInteger? IIfcStairFlight.NumberOfTreads { get { return @NumberOfTreads; } }	
		IfcPositiveLengthMeasure? IIfcStairFlight.RiserHeight { get { return @RiserHeight; } }	
		IfcPositiveLengthMeasure? IIfcStairFlight.TreadLength { get { return @TreadLength; } }	
		IfcStairFlightTypeEnum? IIfcStairFlight.PredefinedType { get { return @PredefinedType; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcStairFlight(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcInteger? _numberOfRisers;
		private IfcInteger? _numberOfTreads;
		private IfcPositiveLengthMeasure? _riserHeight;
		private IfcPositiveLengthMeasure? _treadLength;
		private IfcStairFlightTypeEnum? _predefinedType;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(9, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 33)]
		public IfcInteger? @NumberOfRisers 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _numberOfRisers;
				((IPersistEntity)this).Activate(false);
				return _numberOfRisers;
			} 
			set
			{
				SetValue( v =>  _numberOfRisers = v, _numberOfRisers, value,  "NumberOfRisers", 9);
			} 
		}	
		[EntityAttribute(10, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 34)]
		public IfcInteger? @NumberOfTreads 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _numberOfTreads;
				((IPersistEntity)this).Activate(false);
				return _numberOfTreads;
			} 
			set
			{
				SetValue( v =>  _numberOfTreads = v, _numberOfTreads, value,  "NumberOfTreads", 10);
			} 
		}	
		[EntityAttribute(11, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 35)]
		public IfcPositiveLengthMeasure? @RiserHeight 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _riserHeight;
				((IPersistEntity)this).Activate(false);
				return _riserHeight;
			} 
			set
			{
				SetValue( v =>  _riserHeight = v, _riserHeight, value,  "RiserHeight", 11);
			} 
		}	
		[EntityAttribute(12, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 36)]
		public IfcPositiveLengthMeasure? @TreadLength 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _treadLength;
				((IPersistEntity)this).Activate(false);
				return _treadLength;
			} 
			set
			{
				SetValue( v =>  _treadLength = v, _treadLength, value,  "TreadLength", 12);
			} 
		}	
		[EntityAttribute(13, EntityAttributeState.Optional, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 37)]
		public IfcStairFlightTypeEnum? @PredefinedType 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _predefinedType;
				((IPersistEntity)this).Activate(false);
				return _predefinedType;
			} 
			set
			{
				SetValue( v =>  _predefinedType = v, _predefinedType, value,  "PredefinedType", 13);
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
				case 7: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 8: 
					_numberOfRisers = value.IntegerVal;
					return;
				case 9: 
					_numberOfTreads = value.IntegerVal;
					return;
				case 10: 
					_riserHeight = value.RealVal;
					return;
				case 11: 
					_treadLength = value.RealVal;
					return;
				case 12: 
                    _predefinedType = (IfcStairFlightTypeEnum) System.Enum.Parse(typeof (IfcStairFlightTypeEnum), value.EnumVal, true);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcStairFlight other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcStairFlight
            var root = (@IfcStairFlight)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcStairFlight left, @IfcStairFlight right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcStairFlight left, @IfcStairFlight right)
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