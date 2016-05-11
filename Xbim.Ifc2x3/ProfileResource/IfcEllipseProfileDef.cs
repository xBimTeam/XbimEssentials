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
using Xbim.Ifc2x3.ProfileResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcEllipseProfileDef
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcEllipseProfileDef : IIfcParameterizedProfileDef
	{
		IfcPositiveLengthMeasure @SemiAxis1 { get; }
		IfcPositiveLengthMeasure @SemiAxis2 { get; }
	
	}
}

namespace Xbim.Ifc2x3.ProfileResource
{
	[ExpressType("IfcEllipseProfileDef", 285)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcEllipseProfileDef : IfcParameterizedProfileDef, IInstantiableEntity, IIfcEllipseProfileDef, IContainsEntityReferences, IEquatable<@IfcEllipseProfileDef>
	{
		#region IIfcEllipseProfileDef explicit implementation
		IfcPositiveLengthMeasure IIfcEllipseProfileDef.SemiAxis1 { get { return @SemiAxis1; } }	
		IfcPositiveLengthMeasure IIfcEllipseProfileDef.SemiAxis2 { get { return @SemiAxis2; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcEllipseProfileDef(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcPositiveLengthMeasure _semiAxis1;
		private IfcPositiveLengthMeasure _semiAxis2;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(4, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 4)]
		public IfcPositiveLengthMeasure @SemiAxis1 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _semiAxis1;
				((IPersistEntity)this).Activate(false);
				return _semiAxis1;
			} 
			set
			{
				SetValue( v =>  _semiAxis1 = v, _semiAxis1, value,  "SemiAxis1", 4);
			} 
		}	
		[EntityAttribute(5, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public IfcPositiveLengthMeasure @SemiAxis2 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _semiAxis2;
				((IPersistEntity)this).Activate(false);
				return _semiAxis2;
			} 
			set
			{
				SetValue( v =>  _semiAxis2 = v, _semiAxis2, value,  "SemiAxis2", 5);
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
					_semiAxis1 = value.RealVal;
					return;
				case 4: 
					_semiAxis2 = value.RealVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcEllipseProfileDef other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcEllipseProfileDef
            var root = (@IfcEllipseProfileDef)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcEllipseProfileDef left, @IfcEllipseProfileDef right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcEllipseProfileDef left, @IfcEllipseProfileDef right)
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