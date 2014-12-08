#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProperty.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    public class UniquePropertyNameComparer : IEqualityComparer<IfcProperty>
    {
        #region IEqualityComparer<Property> Members

        public bool Equals(IfcProperty x, IfcProperty y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(IfcProperty obj)
        {
            return obj.Name.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    ///   Set Of Properties, the Name of each property in the set must be unique
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class SetOfProperty : XbimSet<IfcProperty>
    {
        internal SetOfProperty(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    /// <summary>
    ///   Definition from IAI: An abstract generalization for all types of properties that can be associated with IFC objects through the property set mechanism.
    /// </summary>
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcProperty : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                        INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcProperty root = (IfcProperty)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcProperty left, IfcProperty right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcProperty left, IfcProperty right)
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

        private IfcIdentifier _name;
        private IfcText? _description;

        #endregion

        #region Constructors

        internal IfcProperty(IfcIdentifier name)
        {
            _name = name;
        }

        public IfcProperty()
        {
        }

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Name for this property. This label is the significant name string that defines the semantic meaning for the property.
        /// </summary>

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcIdentifier Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        ///   Optional. Informative text to explain the property.
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

        #endregion

        /// <summary>
        ///   Inverse. The property on whose value that of another property depends.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcPropertyDependencyRelationship> PropertyForDependance
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcPropertyDependencyRelationship>(
                        p => p.DependingProperty == this);
            }
        }

        /// <summary>
        ///   Inverse. The relating property on which the value of the property depends.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcPropertyDependencyRelationship> PropertyDependsOn
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcPropertyDependencyRelationship>(
                        p => p.DependantProperty == this);
            }
        }

        /// <summary>
        ///   Inverse. Reference to the IfcComplexProperty in which the IfcProperty is contained.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcComplexProperty> PartOfComplex
        {
            get { return ModelOf.Instances.Where<IfcComplexProperty>(c => c.HasProperties.Contains(this)); }
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
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public abstract string WhereRule();
    }
}