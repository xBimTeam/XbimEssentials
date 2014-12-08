#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDerivedUnitElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    public class DerivedUnitElementSet : XbimSet<IfcDerivedUnitElement>
    {
        internal DerivedUnitElementSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    /// <summary>
    ///   A derived unit element is one of the unit quantities which makes up a derived unit. 
    ///   EXAMPLE: Newtons per square millimetre is a derived unit. It has two elements, Newton whose exponent has a value of 1 and millimetre whose exponent is -2. 
    ///   NOTE Corresponding STEP name: derived_unit_element, please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcDerivedUnitElement : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                         INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcDerivedUnitElement root = (IfcDerivedUnitElement)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcDerivedUnitElement left, IfcDerivedUnitElement right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcDerivedUnitElement left, IfcDerivedUnitElement right)
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

        private IfcNamedUnit _unit;
        private int _exponent;

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The fixed quantity which is used as the mathematical factor.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcNamedUnit Unit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unit;
            }
            set { this.SetModelValue(this, ref _unit, value, v => Unit = v, "Unit"); }
        }

        /// <summary>
        ///   The power that is applied to the unit attribute.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public int Exponent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _exponent;
            }
            set { this.SetModelValue(this, ref _exponent, value, v => Exponent = v, "Exponent"); }
        }


        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _unit = (IfcNamedUnit) value.EntityVal;
                    break;
                case 1:
                    _exponent = (int) value.IntegerVal;
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
            return "";
        }

        #endregion
    }
}