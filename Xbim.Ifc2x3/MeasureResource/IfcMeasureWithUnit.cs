#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMeasureWithUnit.cs
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

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcPersistedEntityAttribute]
    public class IfcMeasureWithUnit : IPersistIfcEntity, IfcAppliedValueSelect, IfcMetricValueSelect,
                                      ISupportChangeNotification, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcMeasureWithUnit root = (IfcMeasureWithUnit)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcMeasureWithUnit left, IfcMeasureWithUnit right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcMeasureWithUnit left, IfcMeasureWithUnit right)
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

        #region Part 21 Step file Parse routines

        private IfcValue _valueComponent;
        private IfcUnit _unitComponent;

        /// <summary>
        ///   The value of the physical quantity when expressed in the specified units.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcValue ValueComponent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _valueComponent;
            }
            set { this.SetModelValue(this, ref _valueComponent, value, v => ValueComponent = v, "ValueComponent"); }
        }

        /// <summary>
        ///   The unit in which the physical quantity is expressed.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcUnit UnitComponent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unitComponent;
            }
            set { this.SetModelValue(this, ref _unitComponent, value, v => UnitComponent = v, "UnitComponent"); }
        }

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _valueComponent = (IfcValue) value.EntityVal;
                    break;
                case 1:
                    _unitComponent = (IfcUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public IfcMeasureWithUnit(IfcValue val, IfcUnit unit)
        {
            _valueComponent = val;
            _unitComponent = unit;
        }

        public IfcMeasureWithUnit()
        {
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
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

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}