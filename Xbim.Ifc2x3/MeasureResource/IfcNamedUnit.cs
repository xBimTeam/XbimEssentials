#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcNamedUnit.cs
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
    public abstract class IfcNamedUnit : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity, IfcUnit,
                                         INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcNamedUnit root = (IfcNamedUnit)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcNamedUnit left, IfcNamedUnit right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcNamedUnit left, IfcNamedUnit right)
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

        private IfcDimensionalExponents _dimensions;
        private IfcUnitEnum _unitType;

        #endregion

       

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The dimensional exponents of the SI base units by which the named unit is defined.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public virtual IfcDimensionalExponents Dimensions
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dimensions;
            }
            set { this.SetModelValue(this, ref _dimensions, value, v => Dimensions = v, "Dimensions"); }
        }

        /// <summary>
        ///   The type of the unit
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcUnitEnum UnitType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unitType;
            }
            set { this.SetModelValue(this, ref _unitType, value, v => UnitType = v, "UnitType"); }
        }

       

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _dimensions = (IfcDimensionalExponents) value.EntityVal;
                    break;
                case 1:
                    _unitType = (IfcUnitEnum) Enum.Parse(typeof (IfcUnitEnum), value.EnumVal, true);
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

        public virtual string WhereRule()
        {
            if (!IfcDimensionalExponents.CorrectDimensions(_unitType, Dimensions))
                return string.Format("WR1 NamedUnit : The dimensions of the named unit {0} are not correct\n",
                                     _unitType.ToString());
            else
                return "";
        }

        #endregion
    }
}