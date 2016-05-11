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
using Xbim.Ifc2x3.TimeSeriesResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcRegularTimeSeries
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcRegularTimeSeries : IIfcTimeSeries
	{
		IfcTimeMeasure @TimeStep { get; }
		IEnumerable<IIfcTimeSeriesValue> @Values { get; }
	
	}
}

namespace Xbim.Ifc2x3.TimeSeriesResource
{
	[ExpressType("IfcRegularTimeSeries", 417)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcRegularTimeSeries : IfcTimeSeries, IInstantiableEntity, IIfcRegularTimeSeries, IContainsEntityReferences, IEquatable<@IfcRegularTimeSeries>
	{
		#region IIfcRegularTimeSeries explicit implementation
		IfcTimeMeasure IIfcRegularTimeSeries.TimeStep { get { return @TimeStep; } }	
		IEnumerable<IIfcTimeSeriesValue> IIfcRegularTimeSeries.Values { get { return @Values; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcRegularTimeSeries(IModel model) : base(model) 		{ 
			Model = model; 
			_values = new ItemSet<IfcTimeSeriesValue>( this, 0,  10);
		}

		#region Explicit attribute fields
		private IfcTimeMeasure _timeStep;
		private ItemSet<IfcTimeSeriesValue> _values;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(9, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 10)]
		public IfcTimeMeasure @TimeStep 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _timeStep;
				((IPersistEntity)this).Activate(false);
				return _timeStep;
			} 
			set
			{
				SetValue( v =>  _timeStep = v, _timeStep, value,  "TimeStep", 9);
			} 
		}	
		[EntityAttribute(10, EntityAttributeState.Mandatory, EntityAttributeType.List, EntityAttributeType.Class, 1, -1, 11)]
		public ItemSet<IfcTimeSeriesValue> @Values 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _values;
				((IPersistEntity)this).Activate(false);
				return _values;
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
					_timeStep = value.RealVal;
					return;
				case 9: 
					_values.InternalAdd((IfcTimeSeriesValue)value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcRegularTimeSeries other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcRegularTimeSeries
            var root = (@IfcRegularTimeSeries)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcRegularTimeSeries left, @IfcRegularTimeSeries right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcRegularTimeSeries left, @IfcRegularTimeSeries right)
        {
            return !(left == right);
        }

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@StartTime != null)
					yield return @StartTime;
				if (@EndTime != null)
					yield return @EndTime;
				if (@Unit != null)
					yield return @Unit;
				foreach(var entity in @Values)
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