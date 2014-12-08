#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOrganizationRelationship.cs
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

namespace Xbim.Ifc2x3.ActorResource
{
    [IfcPersistedEntityAttribute]
    public class OrganizationRelationshipCollection : XbimList<IfcOrganizationRelationship>
    {
        internal OrganizationRelationshipCollection(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcOrganizationRelationship : ISupportChangeNotification, INotifyPropertyChanged, IPersistIfcEntity,
                                               INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcOrganizationRelationship root = (IfcOrganizationRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcOrganizationRelationship left, IfcOrganizationRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcOrganizationRelationship left, IfcOrganizationRelationship right)
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




        public IfcOrganizationRelationship()
        {
            _relatingOrganizations = new OrganizationCollection(this);
        }

        #region Fields and Events

        private IfcLabel? _name;
        private IfcText? _description;
        private IfcOrganization _relatingOrganization;

        private OrganizationCollection _relatingOrganizations;

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   The word or group of words by which the relationship is referred to.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters</value>
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
        ///   Optional. Text that relates the nature of the relationship.
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
        ///   Organization which is the relating part of the relationship between organizations.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcOrganization RelatingOrganization
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingOrganization;
            }
            set
            {
                this.SetModelValue(this, ref _relatingOrganization, value, v => RelatingOrganization = v,
                                           "RelatingOrganization");
            }
        }

        /// <summary>
        ///   The other, possibly dependent, organizations which are the related parts of the relationship between organizations.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public OrganizationCollection RelatedOrganizations
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingOrganizations;
            }
            set
            {
                this.SetModelValue(this, ref _relatingOrganizations, value, v => RelatedOrganizations = v,
                                           "RelatedOrganizations");
            }
        }

        #endregion

        #region Properties

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

        public void IfcParse(int propIndex, IPropertyValue value)
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
                    _relatingOrganization = value.EntityVal as IfcOrganization;
                    break;
                case 3:
                    ((IXbimNoNotifyCollection)_relatingOrganizations).Add(value.EntityVal as IfcOrganization);
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