#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDerivedUnit.cs
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
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcPersistedEntityAttribute]
    public class UnitSet : XbimSet<IfcUnit>
    {
        internal UnitSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    [IfcPersistedEntityAttribute]
    public class IfcDerivedUnit : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity, IfcUnit,
                                  INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcDerivedUnit root = (IfcDerivedUnit)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcDerivedUnit left, IfcDerivedUnit right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcDerivedUnit left, IfcDerivedUnit right)
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

        public IfcDerivedUnit()
        {
            _elements = new DerivedUnitElementSet(this);
        }

        #region Fields

        private DerivedUnitElementSet _elements;
        private IfcDerivedUnitEnum _unitType;
        private IfcLabel? _userDefinedType;

        #endregion

       

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The group of units and their exponents that define the derived unit.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public DerivedUnitElementSet Elements
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elements;
            }
            set
            {
                this.SetModelValue(this, ref _elements, value, v => Elements = v, "Elements");
               
            }
        }

        /// <summary>
        ///   Name of the derived unit chosen from an enumeration of derived unit types for use in IFC models.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcDerivedUnitEnum UnitType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unitType;
            }
            set
            {
                this.SetModelValue(this, ref _unitType, value, v => UnitType = v, "UnitType");
                if (value != IfcDerivedUnitEnum.USERDEFINED)
                    UserDefinedType = null;
            }
        }


        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? UserDefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedType;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedType, value, v => UserDefinedType = v,
                                           "UserDefinedType");
                UnitType = IfcDerivedUnitEnum.USERDEFINED;
            }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    ((IXbimNoNotifyCollection)_elements).Add((IfcDerivedUnitElement) value.EntityVal);
                    break;
                case 1:
                    _unitType = (IfcDerivedUnitEnum) Enum.Parse(typeof (IfcDerivedUnitEnum), value.EnumVal.ToUpper());
                    break;
                case 2:
                    _userDefinedType = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Dimensional exponents derived using the function IfcDerivedDimensionalExponents using (SELF) as the input value.
        /// </summary>
        public IfcDimensionalExponents Dimensions
        {
            get { return IfcDimensionalExponents.DeriveDimensionalExponents(this); }
        }

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
            if (_elements.Count > 1 || (_elements.Count == 1 && _elements.First.Exponent != 1))
            {
                if (
                    !(_unitType != IfcDerivedUnitEnum.USERDEFINED ||
                      (_unitType == IfcDerivedUnitEnum.USERDEFINED && _userDefinedType.HasValue)))

                    err +=
                        "WR2 DerivedUnit:   When attribute UnitType has enumeration value USERDEFINED then attribute UserDefinedType shall also have a value.";
            }
            else
                err += "WR1 DerivedUnit:   Units as such shall not be re-defined as derived units.";
            return err;
        }

        #endregion
    }
}