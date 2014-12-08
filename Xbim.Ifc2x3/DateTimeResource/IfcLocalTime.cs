#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLocalTime.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.DateTimeResource
{
    [IfcPersistedEntityAttribute]
    public class IfcLocalTime : IfcDateTimeSelect, IPersistIfcEntity, INotifyPropertyChanged, ISupportChangeNotification,
                                IfcObjectReferenceSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcLocalTime root = (IfcLocalTime)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcLocalTime left, IfcLocalTime right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcLocalTime left, IfcLocalTime right)
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

        private IfcHourInDay _hourComponent;
        private IfcMinuteInHour? _minuteComponent;
        private IfcSecondInMinute? _secondComponent;
        private IfcCoordinatedUniversalTimeOffset _zone;
        private IfcDaylightSavingHour? _daylightSavingOffset;

        #endregion

        /// <summary>
        ///   The number of hours of the local time.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcHourInDay HourComponent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hourComponent;
            }
            set { this.SetModelValue(this, ref _hourComponent, value, v => HourComponent = v, "HourComponent"); }
        }

        /// <summary>
        ///   The number of minutes of the local time.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcMinuteInHour? MinuteComponent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _minuteComponent;
            }
            set
            {
                this.SetModelValue(this, ref _minuteComponent, value, v => MinuteComponent = v,
                                           "MinuteComponent");
            }
        }

        /// <summary>
        ///   The number of seconds of the local time.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcSecondInMinute? SecondComponent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _secondComponent;
            }
            set
            {
                this.SetModelValue(this, ref _secondComponent, value, v => SecondComponent = v,
                                           "SecondComponent");
            }
        }

        /// <summary>
        ///   The relationship of the local time to coordinated universal time.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcCoordinatedUniversalTimeOffset Zone
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _zone;
            }
            set { this.SetModelValue(this, ref _zone, value, v => Zone = v, "Zone"); }
        }

        /// <summary>
        ///   The offset of daylight saving time from basis time.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcDaylightSavingHour? DaylightSavingOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _daylightSavingOffset;
            }
            set
            {
                this.SetModelValue(this, ref _daylightSavingOffset, value, v => DaylightSavingOffset = v,
                                           "DaylightSavingOffset");
            }
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _hourComponent = (int) value.IntegerVal;
                    break;
                case 1:
                    _minuteComponent = (int) value.IntegerVal;
                    break;
                case 2:
                    _secondComponent = value.RealVal;
                    break;
                case 3:
                    _zone = (IfcCoordinatedUniversalTimeOffset) value.EntityVal;
                    break;
                case 4:
                    _daylightSavingOffset = (int) value.IntegerVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
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

        public string WhereRule()
        {
            if (_secondComponent.HasValue && !_minuteComponent.HasValue)
                return "WR21 LocalTime : The seconds shall only exist if the minutes exists.\n";
            else
                return "";
        }

        #endregion
    }
}