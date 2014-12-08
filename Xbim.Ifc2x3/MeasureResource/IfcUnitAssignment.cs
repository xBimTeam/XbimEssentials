#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcUnitAssignment.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcUnitAssignment : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                     INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcUnitAssignment root = (IfcUnitAssignment)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcUnitAssignment left, IfcUnitAssignment right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcUnitAssignment left, IfcUnitAssignment right)
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

        public IfcUnitAssignment()
        {
            _units = new UnitSet(this);
        }

        #region Fields

        private UnitSet _units;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public UnitSet Units
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _units;
            }
            set { this.SetModelValue(this, ref _units, value, v => Units = v, "Units"); }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
            {
                ((IXbimNoNotifyCollection)_units).Add((IfcUnit) value.EntityVal);
            }
            else
                this.HandleUnexpectedAttribute(propIndex, value);
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
            string err = "";
            IEnumerable<IfcNamedUnit> namedUnits = _units.Where<IfcNamedUnit>(u => u.UnitType != IfcUnitEnum.USERDEFINED);
            IEnumerable<IfcDerivedUnit> derivedUnits =
                _units.Where<IfcDerivedUnit>(u => u.UnitType != IfcDerivedUnitEnum.USERDEFINED);
            IEnumerable<IfcMonetaryUnit> monetaryUnits = _units.OfType<IfcMonetaryUnit>();
            if (monetaryUnits.Count() > 1)
                err += "Only one Monetary Unit is allowed\n";
            HashSet<string> derivedUnitNames = new HashSet<string>();
            HashSet<string> namedUnitsNames = new HashSet<string>();
            foreach (IfcNamedUnit item in namedUnits)
            {
                if (namedUnitsNames.Contains(item.UnitType.ToString()))
                {
                    err += "Two named units of the same type are not allowed in UnitAssignment\n";
                }
                else
                    namedUnitsNames.Add(item.UnitType.ToString());
            }

            foreach (IfcDerivedUnit item in derivedUnits)
            {
                if (derivedUnitNames.Contains(item.UnitType.ToString()))
                {
                    err += "Two derived units of the same type are not allowed in UnitAssignment\n";
                }
                else
                    derivedUnitNames.Add(item.UnitType.ToString());
            }
            return err;
        }

        #endregion
    }

}