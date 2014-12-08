#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProductRepresentation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcProductRepresentation defines a representation of a product, including its (geometric or topological) representation.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcProductRepresentation defines a representation of a product, including its (geometric or topological) representation. A product can have zero, one or many geometric representations, and a single geometric representation can be shared among various products using mapped representations.
    ///   NOTE: The definition of this entity relates to the STEP entity property_definition. The use of the term ‘property’ was avoided since it conflicts with the property, property type, and property set definitions elsewhere in the IFC model. 
    ///   HISTORY: New entity in IFC Release 2.0
    ///   IFC2x Edition 3 NOTE  Users should not instantiate the entity IfcProductRepresentation from IFC2x Edition 3 onwards. It will be changed into an ABSTRACT supertype in future releases of IFC
    /// </remarks>
    [IfcPersistedEntityAttribute]
    //  [IfcPersistedEntity]
    public class IfcProductRepresentation : ISupportChangeNotification, INotifyPropertyChanged, IPersistIfcEntity,
                                            INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcProductRepresentation root = (IfcProductRepresentation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcProductRepresentation left, IfcProductRepresentation right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcProductRepresentation left, IfcProductRepresentation right)
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

        public IfcProductRepresentation()
        {
            _representations = new XbimList<IfcRepresentation>(this);
        }

        #region Fields

        private IfcLabel? _name;
        private IfcText? _description;
        private XbimList<IfcRepresentation> _representations;

        #endregion

        #region Events

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. The word or group of words by which the product representation is known.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = value, "Name"); }
        }

        /// <summary>
        ///   Optional.   The word or group of words that characterize the product representation. It can be used to add additional meaning to the name of the product representation.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = value, "Description"); }
        }

        /// <summary>
        ///   Contained list of representations (including shape representations). Each member defines a valid representation of a particular type within a particular representation context.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.List, 1),IndexedProperty]
        public XbimList<IfcRepresentation> Representations
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representations;
            }
            set
            {
                this.SetModelValue(this, ref _representations, value, v => Representations = v,
                                           "Representations");
            }
        }


        public IfcRepresentation this[string identifier]
        {
            get { return this._representations.FirstOrDefault(item => identifier == item.RepresentationIdentifier); }
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
                case 2:
                    ((IXbimNoNotifyCollection)_representations).Add((IfcRepresentation) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
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