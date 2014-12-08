#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPhysicalQuantity.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.QuantityResource
{
    [IfcPersistedEntityAttribute]
    public class PhysicalQuantitySet : XbimSet<IfcPhysicalQuantity>
    {
        internal PhysicalQuantitySet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    /// <summary>
    ///   The physical quantity, IfcPhysicalQuantity, is an abstract entity that holds a complex or simple quantity measure together with a semantic definition of the usage for the single or several measure value.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The physical quantity, IfcPhysicalQuantity, is an abstract entity that holds a complex or simple quantity measure together with a semantic definition of the usage for the single or several measure value.
    ///   The Name attribute defines the actual usage or kind of measure. The interpretation of the name label has to be established within the actual exchange context. In addition an informative text may be associated to each quantity by the Description attribute.
    ///   HISTORY New entity in IFC Release 2.x. It replaces the calcXxx attributes used in previous IFC Releases.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcPhysicalQuantity : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                                INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcPhysicalQuantity root = (IfcPhysicalQuantity)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcPhysicalQuantity left, IfcPhysicalQuantity right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcPhysicalQuantity left, IfcPhysicalQuantity right)
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

        #endregion

        /// <summary>
        ///   Optional. Name of the element quantity or measure. The name attribute has to be made recognizable by further agreements.
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
        ///   Optional. Further explanation that might be given to the quantity.
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

        /// <summary>
        ///   Inverse. Reference to a physical complex quantity in which the physical quantity may be contained.
        /// </summary>
        public IEnumerable<IfcPhysicalComplexQuantity> PartOfComplex
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcPhysicalComplexQuantity>(
                        pq => pq.HasQuantities.Contains(this));
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

        public virtual string WhereRule()
        {
            return "";
        }

        #endregion

        #region IPersistIfc Members

        string IPersistIfc.WhereRule()
        {
            return "";
        }

        #endregion
    }
}