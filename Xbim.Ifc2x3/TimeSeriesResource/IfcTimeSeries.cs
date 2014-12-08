#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTimeSeries.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TimeSeriesResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcTimeSeries : IfcMetricValueSelect, INotifyPropertyChanged, ISupportChangeNotification,
                                          IPersistIfcEntity, IfcObjectReferenceSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcTimeSeries root = (IfcTimeSeries)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcTimeSeries left, IfcTimeSeries right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcTimeSeries left, IfcTimeSeries right)
        {
            return !(left == right);
        }
        #region IPersistIfcEntity Members

        private int _entityLabel;
		bool _activated;

        private IModel _model;

        public IModel ModelOf
        {
            get { return _model; }
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _activated=activated;
			_model = model;
            _entityLabel = entityLabel;
        }

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        public int EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock(this) { if (_model != null && !_activated) _activated = _model.Activate(this, false)>0;  }
            if (write) _model.Activate(this, write);
        }

        #endregion

        #region Fields

        private IfcLabel _name;
        private IfcText? _description;
        private IfcDateTimeSelect _startTime;
        private IfcDateTimeSelect _endTime;
        private IfcTimeSeriesDataTypeEnum _timeSeriesDataType;
        private IfcDataOriginEnum _dataOrigin;
        private IfcLabel? _userDefinedDataOrigin;
        private IfcUnit _unit;

        #endregion

        /// <summary>
        ///   A unique name for the time series.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        ///   text description of the data that the series represents.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = v, "Description"); }
        }

        /// <summary>
        ///   The start time of a time series.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcDateTimeSelect StartTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _startTime;
            }
            set { this.SetModelValue(this, ref _startTime, value, v => StartTime = v, "StartTime"); }
        }

        /// <summary>
        ///   The end time of a time series.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcDateTimeSelect EndTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _endTime;
            }
            set { this.SetModelValue(this, ref _endTime, value, v => EndTime = v, "EndTime"); }
        }

        /// <summary>
        ///   The time series data type.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcTimeSeriesDataTypeEnum TimeSeriesDataType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _timeSeriesDataType;
            }
            set
            {
                this.SetModelValue(this, ref _timeSeriesDataType, value, v => TimeSeriesDataType = v,
                                           "TimeSeriesDataType");
            }
        }

        /// <summary>
        ///   The orgin of a time series data.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcDataOriginEnum DataOrigin
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dataOrigin;
            }
            set { this.SetModelValue(this, ref _dataOrigin, value, v => DataOrigin = v, "DataOrigin"); }
        }

        /// <summary>
        ///   Optional. Value of the data origin if DataOrigin attribute is USERDEFINED.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLabel? UserDefinedDataOrigin
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedDataOrigin;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedDataOrigin, value, v => UserDefinedDataOrigin = v,
                                           "UserDefinedDataOrigin");
            }
        }

        /// <summary>
        ///   Optional. The unit to be assigned to all values within the time series. Note that mixing units is not allowed. If the value is not given, the global unit for the type of IfcValue, as defined at IfcProject.UnitsInContext is used.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcUnit Unit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unit;
            }
            set { this.SetModelValue(this, ref _unit, value, v => Unit = v, "Unit"); }
        }

        #region Inverses

        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcTimeSeriesReferenceRelationship> DocumentedBy
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcTimeSeriesReferenceRelationship>(
                        tr => tr.ReferencedTimeSeries == this);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        [field: NonSerialized] //don't serialize events
            private event PropertyChangingEventHandler PropertyChanging;

        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }

        #endregion

        #region ISupportChangeNotification Members

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _name = value.StringVal;
                    break;
                case 1:
                    _description = value.StringVal;
                    break;
                case 2:
                    _startTime = (IfcDateTimeSelect) value.EntityVal;
                    break;
                case 3:
                    _endTime = (IfcDateTimeSelect) value.EntityVal;
                    break;
                case 4:
                    _timeSeriesDataType =
                        (IfcTimeSeriesDataTypeEnum) Enum.Parse(typeof (IfcTimeSeriesDataTypeEnum), value.EnumVal, true);
                    break;
                case 5:
                    _dataOrigin = (IfcDataOriginEnum) Enum.Parse(typeof (IfcDataOriginEnum), value.EnumVal, true);
                    break;
                case 6:
                    _userDefinedDataOrigin = value.StringVal;
                    break;
                case 7:
                    _unit = (IfcUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}