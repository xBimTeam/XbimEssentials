#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRepresentation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PresentationOrganizationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   A representation is one or more representation items that are related in a specified representation context as the representation of some concept.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-43:1992: A representation is one or more representation items that are related in a specified representation context as the representation of some concept. 
    ///   Definition from IAI: The IfcRepresentation defines the general concept of representing product properties. 
    ///   NOTE  The definition of this entity relates to the STEP entity representation. Please refer to ISO/IS 10303-43:1994 for the final definition of the formal standard. 
    ///   HISTORY  New entity in IFC Release 2.0 
    ///   IFC2x Edition 3 NOTE  Users should not instantiate the entity IfcRepresentation from IFC2x Edition 3 onwards. It will be changed into an ABSTRACT supertype in future releases of IFC.
    ///   IFC2x Edition 3 CHANGE  The inverse attributes LayerAssignments and RepresentationMap have been added with upward compatibility.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcRepresentation : ISupportChangeNotification, INotifyPropertyChanged, IPersistIfcEntity,
                                     IfcLayeredItem, INotifyPropertyChanging
    {

        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcRepresentation root = (IfcRepresentation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcRepresentation left, IfcRepresentation right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcRepresentation left, IfcRepresentation right)
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

        public IfcRepresentation()
        {
            _items = new XbimSet<IfcRepresentationItem>(this);
        }

        #region Fields

        private IfcRepresentationContext _contextOfItems;
        private IfcLabel? _representationIdentifier;
        private IfcLabel? _representationType;
        private XbimSet<IfcRepresentationItem> _items;

        #endregion

        #region Events

        [field: NonSerialized] //don't serialize events
            public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Part 21 Step file representation

        /// <summary>
        ///   Definition of the representation context for which the different subtypes of representation are valid.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcRepresentationContext ContextOfItems
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _contextOfItems;
            }
            set { this.SetModelValue(this, ref _contextOfItems, value, v => ContextOfItems = v, "ContextOfItems"); }
        }

        /// <summary>
        ///   Optional identifier of the representation as used within a project.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLabel? RepresentationIdentifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representationIdentifier;
            }
            set
            {
                this.SetModelValue(this, ref _representationIdentifier, value, v => RepresentationIdentifier = v,
                                           "RepresentationIdentifier");
            }
        }


        /// <summary>
        ///   The description of the type of a representation context. 
        ///   The representation type defines the type of geometry or topology used for representing the product representation. 
        ///   More information is given at the subtypes ShapeRepresentation and TopologyRepresentation.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? RepresentationType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representationType;
            }
            set
            {
                this.SetModelValue(this, ref _representationType, value, v => RepresentationType = v,
                                           "RepresentationType");
            }
        }


        /// <summary>
        ///   Set of geometric representation items that are defined for this representation.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimSet<IfcRepresentationItem> Items
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _items;
            }
            set { this.SetModelValue(this, ref _items, value, v => Items = v, "Items"); }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _contextOfItems = (IfcRepresentationContext) value.EntityVal;
                    break;
                case 1:
                    _representationIdentifier = value.StringVal;
                    break;
                case 2:
                    _representationType = value.StringVal;
                    break;
                case 3:
                    ((IXbimNoNotifyCollection)_items).Add((IfcRepresentationItem) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Inverse. Use of the representation within an IfcRepresentationMap. If used, this IfcRepresentation may be assigned to many representations as one of its Items using an IfcMappedItem. Using IfcRepresentationMap is the way to share one representation (often of type IfcShapeRepresentation) by many products.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRepresentationMap> RepresentationMap
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRepresentationMap>(m => m.MappedRepresentation == this);
            }
        }

        /// <summary>
        ///   Inverse. Assignment of the whole representation to a single or multiple layer(s). The LayerAssigments can be overridden by LayerAssigments of the IfcRepresentationItem's within the list of Items.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcPresentationLayerAssignment> LayerAssignments
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcPresentationLayerAssignment>(
                        a => (a.AssignedItems != null && a.AssignedItems.Contains(this)));
            }
        }

        /// <summary>
        ///   Inverse. Reference to the product shape, for which it is the shape representation.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcProductRepresentation> OfProductRepresentation
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcProductRepresentation>(
                        p => ( p.Representations.Contains(this)));
            }
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

        #region INotifyPropertyChanged Members

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

        #region ISupportIfcParser Members

        public virtual string WhereRule()
        {
            return "";
        }

        #endregion
    }
}