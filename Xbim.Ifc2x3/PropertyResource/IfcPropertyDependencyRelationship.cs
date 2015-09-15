#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyDependencyRelationship.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   An IfcPropertyDependencyRelationship describes an identified dependency between the value of one property and that of another.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcPropertyDependencyRelationship describes an identified dependency between the value of one property and that of another.
    ///   HISTORY: New entity in Release IFC2x Edition 2
    ///   Use Definition
    ///   Whilst the IfcPropertyDependencyRelationship may be used to describe the dependency, and it may do so in terms of the expression of how the dependency operates, it is not possible through the current IFC model for the value of the related property to be actually derived from the value of the relating property. The determination of value according to the dependency is required to be performed by an application that can then use the Expression attribute to flag the form of the dependency.
    ///   Formal Propositions:
    ///   WR1   :   The DependingProperty shall not point to the same instance as the DependantProperty
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcPropertyDependencyRelationship : ISupportChangeNotification, INotifyPropertyChanged,
                                                     IPersistIfcEntity, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcPropertyDependencyRelationship root = (IfcPropertyDependencyRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcPropertyDependencyRelationship left, IfcPropertyDependencyRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcPropertyDependencyRelationship left, IfcPropertyDependencyRelationship right)
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

        private IfcProperty _dependingProperty;
        private IfcProperty _dependantProperty;
        private IfcLabel? _name;
        private IfcText? _description;
        private IfcText? _expression;

        #endregion

        /// <summary>
        ///   The property on which the relationship depends.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcProperty DependingProperty
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dependingProperty;
            }
            set
            {
                this.SetModelValue(this, ref _dependingProperty, value, v => DependingProperty = v,
                                           "DependingProperty");
            }
        }

        /// <summary>
        ///   The dependant property
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcProperty DependantProperty
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dependantProperty;
            }
            set
            {
                this.SetModelValue(this, ref _dependantProperty, value, v => DependantProperty = v,
                                           "DependantProperty");
            }
        }


        /// <summary>
        ///   A name used to identify or qualify the applied value relationship.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
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
        ///   A description that may apply additional information about an applied value relationship.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
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
        ///   The arithmetic operator applied in an applied value relationship.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcText? Expression
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _expression;
            }
            set { this.SetModelValue(this, ref _expression, value, v => Expression = v, "Expression"); }
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

        #region IPersistIfc Members

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _dependingProperty = (IfcProperty) value.EntityVal;
                    break;
                case 1:
                    _dependantProperty = (IfcProperty) value.EntityVal;
                    break;
                case 2:
                    _name = value.StringVal;
                    break;
                case 3:
                    _description = value.StringVal;
                    break;
                case 4:
                    _expression = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            if (_dependantProperty == _dependingProperty)
                return
                    "WR1 PropertyDependencyRelationship : The DependingProperty shall not point to the same instance as the DependantProperty\n";
            else
                return "";
        }

        #endregion
    }
}