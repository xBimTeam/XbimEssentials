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
using Xbim.Ifc4.ProfileResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcRectangleHollowProfileDef
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcRectangleHollowProfileDef : IIfcRectangleProfileDef
	{
		IfcPositiveLengthMeasure @WallThickness { get;  set; }
		IfcNonNegativeLengthMeasure? @InnerFilletRadius { get;  set; }
		IfcNonNegativeLengthMeasure? @OuterFilletRadius { get;  set; }
	
	}
}

namespace Xbim.Ifc4.ProfileResource
{
	[ExpressType("IfcRectangleHollowProfileDef", 562)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcRectangleHollowProfileDef : IfcRectangleProfileDef, IInstantiableEntity, IIfcRectangleHollowProfileDef, IContainsEntityReferences, IEquatable<@IfcRectangleHollowProfileDef>
	{
		#region IIfcRectangleHollowProfileDef explicit implementation
		IfcPositiveLengthMeasure IIfcRectangleHollowProfileDef.WallThickness { 
			get { return @WallThickness; } 
 
			set { WallThickness = value;}
		}	
		IfcNonNegativeLengthMeasure? IIfcRectangleHollowProfileDef.InnerFilletRadius { 
			get { return @InnerFilletRadius; } 
 
			set { InnerFilletRadius = value;}
		}	
		IfcNonNegativeLengthMeasure? IIfcRectangleHollowProfileDef.OuterFilletRadius { 
			get { return @OuterFilletRadius; } 
 
			set { OuterFilletRadius = value;}
		}	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcRectangleHollowProfileDef(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcPositiveLengthMeasure _wallThickness;
		private IfcNonNegativeLengthMeasure? _innerFilletRadius;
		private IfcNonNegativeLengthMeasure? _outerFilletRadius;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(6, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 8)]
		public IfcPositiveLengthMeasure @WallThickness 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _wallThickness;
				((IPersistEntity)this).Activate(false);
				return _wallThickness;
			} 
			set
			{
				SetValue( v =>  _wallThickness = v, _wallThickness, value,  "WallThickness", 6);
			} 
		}	
		[EntityAttribute(7, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 9)]
		public IfcNonNegativeLengthMeasure? @InnerFilletRadius 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _innerFilletRadius;
				((IPersistEntity)this).Activate(false);
				return _innerFilletRadius;
			} 
			set
			{
				SetValue( v =>  _innerFilletRadius = v, _innerFilletRadius, value,  "InnerFilletRadius", 7);
			} 
		}	
		[EntityAttribute(8, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 10)]
		public IfcNonNegativeLengthMeasure? @OuterFilletRadius 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _outerFilletRadius;
				((IPersistEntity)this).Activate(false);
				return _outerFilletRadius;
			} 
			set
			{
				SetValue( v =>  _outerFilletRadius = v, _outerFilletRadius, value,  "OuterFilletRadius", 8);
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
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 5: 
					_wallThickness = value.RealVal;
					return;
				case 6: 
					_innerFilletRadius = value.RealVal;
					return;
				case 7: 
					_outerFilletRadius = value.RealVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcRectangleHollowProfileDef other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcRectangleHollowProfileDef
            var root = (@IfcRectangleHollowProfileDef)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcRectangleHollowProfileDef left, @IfcRectangleHollowProfileDef right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcRectangleHollowProfileDef left, @IfcRectangleHollowProfileDef right)
        {
            return !(left == right);
        }

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@Position != null)
					yield return @Position;
			}
		}
		#endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}