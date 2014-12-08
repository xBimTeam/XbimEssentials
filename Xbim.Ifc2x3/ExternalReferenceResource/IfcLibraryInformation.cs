#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLibraryInformation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcLibraryInformation is a class that describes a library where a library is a structured store of information, normally organized in a manner which allows information lookup through an index or reference value.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcLibraryInformation is a class that describes a library where a library is a structured store of information, normally organized in a manner which allows information lookup through an index or reference value. IfcLibraryInformation provides the library name and optional version, version date and publisher attributes. 
    ///   NOTE: The complete definition of the information in an external library is out of scope in this IFC release. 
    ///   HISTORY: New Entity in IFC Release 2.0. Renamed from IfcLibrary to IfcLibraryInformation in IFC 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcLibraryInformation : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                         IfcLibrarySelect, INotifyPropertyChanging
    {

        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcLibraryInformation root = (IfcLibraryInformation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcLibraryInformation left, IfcLibraryInformation right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcLibraryInformation left, IfcLibraryInformation right)
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
        private IfcLabel? _version;
        private IfcOrganization _publisher;
        private IfcCalendarDate _versionDate;
        private XbimSet<IfcLibraryReference> _libraryReference;

        #endregion

        /// <summary>
        ///   The name which is used to identify the library.
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
        ///   Optional. Identifier for the library version used for reference.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLabel? Version
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _version;
            }
            set { this.SetModelValue(this, ref _version, value, v => Version = v, "Version"); }
        }

        /// <summary>
        ///   Optional. Information of the organization that acts as the library publisher.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcOrganization Publisher
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _publisher;
            }
            set { this.SetModelValue(this, ref _publisher, value, v => Publisher = v, "Publisher"); }
        }

        /// <summary>
        ///   Optional. Date of the referenced version of the library.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcCalendarDate VersionDate
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _versionDate;
            }
            set { this.SetModelValue(this, ref _versionDate, value, v => VersionDate = v, "VersionDate"); }
        }

        /// <summary>
        ///   Optional. Information on the library being referenced.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcLibraryReference> LibraryReference
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _libraryReference;
            }
            set
            {
                this.SetModelValue(this, ref _libraryReference, value, v => LibraryReference = v,
                                           "LibraryReference");
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

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _name = value.StringVal;
                    break;
                case 1:
                    _version = value.StringVal;
                    break;
                case 2:
                    _publisher = (IfcOrganization) value.EntityVal;
                    break;
                case 3:
                    _versionDate = (IfcCalendarDate) value.EntityVal;
                    break;
                case 4:
                    if (_libraryReference == null) _libraryReference = new XbimSet<IfcLibraryReference>(this);
                    ((IXbimNoNotifyCollection)_libraryReference).Add((IfcLibraryReference) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}