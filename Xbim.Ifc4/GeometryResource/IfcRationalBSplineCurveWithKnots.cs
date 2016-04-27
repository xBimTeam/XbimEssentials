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
    /// Readonly interface for IfcRationalBSplineCurveWithKnots
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcRationalBSplineCurveWithKnots : IIfcBSplineCurveWithKnots
	{
		IEnumerable<IfcReal> @WeightsData { get; }
		List<IfcReal> @Weights  { get ; }
	
	}
}

namespace Xbim.Ifc4.GeometryResource
{
	[ExpressType("IfcRationalBSplineCurveWithKnots", 1241)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcRationalBSplineCurveWithKnots : IfcBSplineCurveWithKnots, IInstantiableEntity, IIfcRationalBSplineCurveWithKnots, IEquatable<@IfcRationalBSplineCurveWithKnots>
	{
		#region IIfcRationalBSplineCurveWithKnots explicit implementation
		IEnumerable<IfcReal> IIfcRationalBSplineCurveWithKnots.WeightsData { get { return @WeightsData; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcRationalBSplineCurveWithKnots(IModel model) : base(model) 		{ 
			Model = model; 
			_weightsData = new ItemSet<IfcReal>( this, 0,  9);
		}

		#region Explicit attribute fields
		private ItemSet<IfcReal> _weightsData;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(9, EntityAttributeState.Mandatory, EntityAttributeType.List, EntityAttributeType.None, 2, -1, 11)]
		public ItemSet<IfcReal> @WeightsData 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _weightsData;
				((IPersistEntity)this).Activate(false);
				return _weightsData;
			} 
		}	
		#endregion


		#region Derived attributes
		[EntityAttribute(0, EntityAttributeState.Derived, EntityAttributeType.Array, EntityAttributeType.None, 0, -1, 0)]
		public List<IfcReal> @Weights 
		{
			get 
			{
				//## Getter for Weights
			    return WeightsData.ToList();
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
					_weightsData.InternalAdd(value.RealVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcRationalBSplineCurveWithKnots other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcRationalBSplineCurveWithKnots
            var root = (@IfcRationalBSplineCurveWithKnots)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcRationalBSplineCurveWithKnots left, @IfcRationalBSplineCurveWithKnots right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcRationalBSplineCurveWithKnots left, @IfcRationalBSplineCurveWithKnots right)
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