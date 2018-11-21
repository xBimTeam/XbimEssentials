// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.TimeSeriesResource;
using Xbim.Ifc2x3.DateTimeResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.ControlExtension;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcTimeSeriesSchedule
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcTimeSeriesSchedule : IIfcControl
	{
		IItemSet<IIfcDateTimeSelect> @ApplicableDates { get; }
		IfcTimeSeriesScheduleTypeEnum @TimeSeriesScheduleType { get;  set; }
		IIfcTimeSeries @TimeSeries { get;  set; }
	
	}
}

namespace Xbim.Ifc2x3.ControlExtension
{
	[ExpressType("IfcTimeSeriesSchedule", 712)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcTimeSeriesSchedule : IfcControl, IInstantiableEntity, IIfcTimeSeriesSchedule, IContainsEntityReferences, IEquatable<@IfcTimeSeriesSchedule>
	{
		#region IIfcTimeSeriesSchedule explicit implementation
		IItemSet<IIfcDateTimeSelect> IIfcTimeSeriesSchedule.ApplicableDates { 
			get { return new Common.Collections.ProxyItemSet<IfcDateTimeSelect, IIfcDateTimeSelect>( @ApplicableDates); } 
		}	
		IfcTimeSeriesScheduleTypeEnum IIfcTimeSeriesSchedule.TimeSeriesScheduleType { 
 
			get { return @TimeSeriesScheduleType; } 
			set { TimeSeriesScheduleType = value;}
		}	
		IIfcTimeSeries IIfcTimeSeriesSchedule.TimeSeries { 
 
 
			get { return @TimeSeries; } 
			set { TimeSeries = value as IfcTimeSeries;}
		}	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcTimeSeriesSchedule(IModel model, int label, bool activated) : base(model, label, activated)  
		{
			_applicableDates = new OptionalItemSet<IfcDateTimeSelect>( this, 0,  6);
		}

		#region Explicit attribute fields
		private readonly OptionalItemSet<IfcDateTimeSelect> _applicableDates;
		private IfcTimeSeriesScheduleTypeEnum _timeSeriesScheduleType;
		private IfcTimeSeries _timeSeries;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(6, EntityAttributeState.Optional, EntityAttributeType.List, EntityAttributeType.Class, new int [] { 1 }, new int [] { -1 }, 12)]
		public IOptionalItemSet<IfcDateTimeSelect> @ApplicableDates 
		{ 
			get 
			{
				if(_activated) return _applicableDates;
				Activate();
				return _applicableDates;
			} 
		}	
		[EntityAttribute(7, EntityAttributeState.Mandatory, EntityAttributeType.Enum, EntityAttributeType.None, null, null, 13)]
		public IfcTimeSeriesScheduleTypeEnum @TimeSeriesScheduleType 
		{ 
			get 
			{
				if(_activated) return _timeSeriesScheduleType;
				Activate();
				return _timeSeriesScheduleType;
			} 
			set
			{
				SetValue( v =>  _timeSeriesScheduleType = v, _timeSeriesScheduleType, value,  "TimeSeriesScheduleType", 7);
			} 
		}	
		[EntityAttribute(8, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, null, null, 14)]
		public IfcTimeSeries @TimeSeries 
		{ 
			get 
			{
				if(_activated) return _timeSeries;
				Activate();
				return _timeSeries;
			} 
			set
			{
				if (value != null && !(ReferenceEquals(Model, value.Model)))
					throw new XbimException("Cross model entity assignment.");
				SetValue( v =>  _timeSeries = v, _timeSeries, value,  "TimeSeries", 8);
			} 
		}	
		#endregion




		#region IPersist implementation
		public override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
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
					_applicableDates.InternalAdd((IfcDateTimeSelect)value.EntityVal);
					return;
				case 6: 
                    _timeSeriesScheduleType = (IfcTimeSeriesScheduleTypeEnum) System.Enum.Parse(typeof (IfcTimeSeriesScheduleTypeEnum), value.EnumVal, true);
					return;
				case 7: 
					_timeSeries = (IfcTimeSeries)(value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcTimeSeriesSchedule other)
	    {
	        return this == other;
	    }
        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@OwnerHistory != null)
					yield return @OwnerHistory;
				foreach(var entity in @ApplicableDates)
					yield return entity;
				if (@TimeSeries != null)
					yield return @TimeSeries;
			}
		}
		#endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}