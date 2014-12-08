#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcShapeAspect.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Common.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The shape aspect is an identifiable element of the shape of a product.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-41:1992: The shape aspect is an identifiable element of the shape of a product. 
    ///   Definition from IAI: The IfcShapeAspect allows for grouping of shape representation items that represent aspects (or components) of the shape of a product. Thereby shape representations of components of the product shape represent a distinctive part to a product that can be explicitly addressed. 
    ///   NOTE: The definition of this class relates to the STEP entity shape_aspect. Please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard. 
    ///   HISTORY: New Entity in IFC Release 2.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcShapeAspect : ISupportChangeNotification, INotifyPropertyChanged, IPersistIfcEntity,
                                  INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcShapeAspect root = (IfcShapeAspect)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcShapeAspect left, IfcShapeAspect right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcShapeAspect left, IfcShapeAspect right)
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

        public IfcShapeAspect()
        {
            _shapeRepresentations = new ShapeModelList(this);
        }

        #region Fields

        private ShapeModelList _shapeRepresentations;
        private IfcLabel? _name;
        private IfcText? _description;
        private IfcLogical _productDefinitional;
        private IfcProductDefinitionShape _partOfProductDefinitionShape;

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   List of shapeModel. Each member defines a valid representation of a particular type within a particular representation context
        ///   as being an aspect (or part) of a product definition.
        /// </summary>
        /// <remarks>
        ///   IFC2x Edition 3 CHANGE  The data type has been changed from IfcShapeRepresentation to IfcShapeModel with upward compatibility
        /// </remarks>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public ShapeModelList ShapeRepresentations
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _shapeRepresentations;
            }
            set
            {
                this.SetModelValue(this, ref _shapeRepresentations, value, v => ShapeRepresentations = v,
                                           "ShapeRepresentations");
            }
        }

        /// <summary>
        ///   Optional. The word or group of words by which the shape aspect is known. It is a tag to indicate the particular semantic of a component within the product definition shape, used to provide meaning. Example: use the tag "Glazing" to define which component of a window shape defines the glazing area.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
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
        ///   Optional. The word or group of words that characterize the shape aspect. It can be used to add additional meaning to the name of the aspect.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
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
        ///   An indication that the shape aspect is on the physical boundary of the product definition shape.
        /// </summary>
        /// <remarks>
        ///   If the value of this attribute is TRUE, it shall be asserted that the shape aspect being identified is on such a boundary. If the value is FALSE, it shall be asserted that the shape aspect being identified is not on such a boundary. If the value is UNKNOWN, it shall be asserted that it is not known whether or not the shape aspect being identified is on such a boundary. 
        ///   ---
        ///   EXAMPLE: Would be FALSE for a center line, identified as shape aspect; would be TRUE for a cantilever.
        ///   ---
        /// </remarks>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLogical ProductDefinitional
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _productDefinitional;
            }
            set
            {
                this.SetModelValue(this, ref _productDefinitional, value, v => ProductDefinitional = v,
                                           "ProductDefinitional");
            }
        }

        /// <summary>
        ///   Reference to the product definition shape of which this class is an aspect.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcProductDefinitionShape PartOfProductDefinitionShape
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _partOfProductDefinitionShape;
            }
            set
            {
                this.SetModelValue(this, ref _partOfProductDefinitionShape, value,
                                           v => PartOfProductDefinitionShape = v, "PartOfProductDefinitionShape");
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

        #region IPersistIfc Members

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    ((IXbimNoNotifyCollection)_shapeRepresentations).Add((IfcShapeModel) value.EntityVal);
                    break;
                case 1:
                    _name = value.StringVal;
                    break;
                case 2:
                    _description = value.StringVal;
                    break;
                case 3:
                    _productDefinitional = value.BooleanVal;
                    break;
                case 4:
                    _partOfProductDefinitionShape = (IfcProductDefinitionShape) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}