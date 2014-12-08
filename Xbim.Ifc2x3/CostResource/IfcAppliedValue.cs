#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAppliedValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   An IfcAppliedValue is an abstract supertype that specifies the common attributes for cost and environmental values that may be applied to objects within the IFC model.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcAppliedValue is an abstract supertype that specifies the common attributes for cost and environmental values that may be applied to objects within the IFC model. 
    ///   HISTORY: New Entity in IFC Release 2x2
    ///   Use Definitions
    ///   The extent of the IfcAppliedValue is determined by the AppliedValue attribute which may be defined either as an IfcMeasureWithUnit or as an IfcMonetaryMeasure or as an IfcRatioMeasure via the IfcAppliedValueSelect type. 
    ///   Optionally, an IfcAppliedValue may have an applicable date. This is intended to fix the date on which the value became relevant for use. It may be the date on which the value was set in the model or it may be a prior or future date when the value becomes operable. It should be noted that the datatype for IfcAppliedValue.ApplicableDate is IfcDateTimeSelect. This enables either a calendar date or a date and time to be selected. The option of selecting a time only without a date is also possible through this select mechanism but should not be used in the case of an applied value. 
    ///   Similarly, an IfcAppliedValue may have a 'fixed until' date. This is intended to fix the date on which the value ceases to be relevant for use. 
    ///   An instance of IfcAppliedValue may have a unit basis asserted. This is defined as an IfcMeasureWithUnit that determines the extent of the unit value for application purposes. It is assumed that when this attribute is asserted, then the value given to IfcAppliedValue is that for unit quantity. This is not enforced within the IFC schema and thus needs to be controlled within an application.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcAppliedValue : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                            IfcObjectReferenceSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcAppliedValue root = (IfcAppliedValue)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcAppliedValue left, IfcAppliedValue right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcAppliedValue left, IfcAppliedValue right)
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

        private IfcLabel? _name;
        private IfcText? _description;
        private IfcAppliedValueSelect _appliedValue;
        private IfcMeasureWithUnit _unitBasis;
        private IfcDateTimeSelect _applicableDate;
        private IfcDateTimeSelect _fixedUntilDate;

        #endregion

        /// <summary>
        ///   Optional. A name or additional clarification given to a cost (or impact) value.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        ///   Optional. The description that may apply additional information about a cost (or impact) value. The description may be from purpose generated text, specification libraries, standards etc.
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
        ///   Optional. The extent or quantity or amount of an applied value.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcAppliedValueSelect Value
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _appliedValue;
            }
            set { this.SetModelValue(this, ref _appliedValue, value, v => Value = v, "Value"); }
        }

        /// <summary>
        ///   Optional. The number and unit of measure on which the unit cost is based.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcMeasureWithUnit UnitBasis
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unitBasis;
            }
            set { this.SetModelValue(this, ref _unitBasis, value, v => UnitBasis = v, "UnitBasis"); }
        }

        /// <summary>
        ///   Optional. The date until which applied value is applicable.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcDateTimeSelect ApplicableDate
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _applicableDate;
            }
            set { this.SetModelValue(this, ref _applicableDate, value, v => ApplicableDate = v, "ApplicableDate"); }
        }

        /// <summary>
        ///   Optional. The date until which applied value is applicable.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcDateTimeSelect FixedUntilDate
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fixedUntilDate;
            }
            set { this.SetModelValue(this, ref _fixedUntilDate, value, v => FixedUntilDate = v, "FixedUntilDate"); }
        }


        /// <summary>
        ///   Inverse. Pointer to the IfcReferencesCostDocument relationship, which refer to a document from which the cost value is referenced.
        /// </summary>
        public IEnumerable<IfcReferencesValueDocument> ValuesReferenced
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcReferencesValueDocument>(
                        rv => rv.ReferencingValues.Contains(this));
            }
        }

        /// <summary>
        ///   Inverse. The total (or subtotal) value of the components within the applied value relationship expressed as a single applied value.
        /// </summary>
        public IEnumerable<IfcAppliedValueRelationship> ValueOfComponents
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcAppliedValueRelationship>(
                        av => av.ComponentOfTotal == this);
            }
        }

        /// <summary>
        ///   Inverse. The value of the single applied value which is used by the applied value relationship to express a complex applied value.
        /// </summary>
        public IEnumerable<IfcAppliedValueRelationship> IsComponentIn
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcAppliedValueRelationship>(
                        av => av.Components.Contains(this));
            }
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
                    _appliedValue = (IfcAppliedValueSelect) value.EntityVal;
                    break;
                case 3:
                    _unitBasis = (IfcMeasureWithUnit) value.EntityVal;
                    break;
                case 4:
                    _applicableDate = (IfcDateTimeSelect) value.EntityVal;
                    break;
                case 5:
                    _fixedUntilDate = (IfcDateTimeSelect) value.EntityVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public virtual string WhereRule()
        {
            if (_appliedValue == null && ValueOfComponents.Count() == 0)
                return "WR1 AppliedValue : Applied Value or ValueOfCommponents must be defined";
            else
                return "";
        }

        #endregion
    }
}