#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcExternalReference.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcExternalReference is the identification of information that is not explicitly represented in the current model or in the project database 
    ///   (as an implementation of the current model).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcExternalReference is the identification of information that is not explicitly represented in the current model 
    ///   or in the project database (as an implementation of the current model). Such information may be contained in classifications, documents or libraries. 
    ///   Only the Location (e.g. as an URL) is given to describe the place where the information can be found. 
    ///   Also an optional ItemReference as a key to allow more specific references (as to sections or tables) is provided. 
    ///   The ItemReference defines a system interpretable method to identify the relevant part of information at the data source (given by Location). 
    ///   In addition a human interpretable Name can be assigned to identify the information subject (e.g. classification code).
    ///   IfcExternalReference is an abstract supertype of all external reference classes.
    ///   HISTORY: New Class in IFC Release 2x to generalize means of referencing available in IFC Release 2.0. 
    ///   Formal Propositions:
    ///   WR1   :   One of the attributes of IfcExternalReference should have a value assigned.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcExternalReference : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                                 IfcObjectReferenceSelect, INotifyPropertyChanging, IfcLightDistributionDataSourceSelect
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcExternalReference root = (IfcExternalReference)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcExternalReference left, IfcExternalReference right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcExternalReference left, IfcExternalReference right)
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

        private IfcLabel? _location;
        private IfcIdentifier? _itemReference;
        private IfcLabel? _name;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Location, where the external source (classification, document or library). 
        ///   This can be either human readable or computer interpretable. 
        ///   For electronic location normally given as an URL location string, however other ways of accessing external references may be established in an application scenario.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? Location
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _location;
            }
            set { this.SetModelValue(this, ref _location, value, v => Location = v, "Location"); }
        }

        /// <summary>
        ///   Optional. Identifier for the referenced item in the external source (classification, document or library). 
        ///   The internal reference can provide a computer interpretable pointer into electronic source.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcIdentifier? ItemReference
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _itemReference;
            }
            set { this.SetModelValue(this, ref _itemReference, value, v => ItemReference = v, "ItemReference"); }
        }

        /// <summary>
        ///   Optional. Optional name to further specify the reference. 
        ///   It can provide a human readable identifier (which does not necessarily need to have a counterpart in the internal structure of the document).
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


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _location = value.StringVal;
                    break;
                case 1:
                    _itemReference = value.StringVal;
                    break;
                case 2:
                    _name = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
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

        #region ISupportIfcParser Members

        public virtual string WhereRule()
        {
            if (ItemReference == null && Location == null && Name == null)
                return
                    "WR1 ExternalReference : One of the attributes of ExternalReference should have a value assigned.\n";
            else
                return "";
        }

        #endregion
    }
}