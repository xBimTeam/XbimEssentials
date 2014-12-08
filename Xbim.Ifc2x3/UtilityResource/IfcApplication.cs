#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcApplication.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.UtilityResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcApplication : IPersistIfcEntity, ISupportChangeNotification, INotifyPropertyChanged,
                                  INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcApplication root = (IfcApplication)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcApplication left, IfcApplication right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcApplication left, IfcApplication right)
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


        #region Fields and Events

        private IfcOrganization _applicationDeveloper;
        private IfcLabel _version;
        private IfcLabel _applicationFullName;
        private IfcIdentifier _applicationIdentifier;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Name of the application developer, being requested to be member of the IAI.
        /// </summary>

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcOrganization ApplicationDeveloper
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _applicationDeveloper;
            }
            set { this.SetModelValue(this, ref _applicationDeveloper, value, v => ApplicationDeveloper = v, "ApplicationDeveloper"); }
        }

        /// <summary>
        ///   The version number of this software as specified by the developer of the application.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcLabel Version
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _version;
            }
            set { this.SetModelValue(this, ref _version, value, v => Version = v, "Version"); }
        }


        /// <summary>
        ///   The full name of the application as specified by the application developer.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>

        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcLabel ApplicationFullName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _applicationFullName;
            }
            set { this.SetModelValue(this, ref _applicationFullName, value, v => ApplicationFullName = v, "ApplicationFullName"); }
        }


        /// <summary>
        ///   Short identifying name for the application.
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcIdentifier ApplicationIdentifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _applicationIdentifier;
            }
            set { this.SetModelValue(this, ref _applicationIdentifier, value, v => ApplicationIdentifier = v, "ApplicationIdentifier"); }
        }

        public override string ToString()
        {
            return string.Format("{0}({1}) - v{2}",ApplicationFullName,ApplicationIdentifier, Version);
        }
        #endregion

        #region Methods

        public static IfcIdentifier XbimApplicationIdentier
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyProductAttribute attr =
                    Attribute.GetCustomAttribute(assembly, typeof (AssemblyProductAttribute), false) as
                    AssemblyProductAttribute;
                return attr.Product;
            }
        }

        public static IfcLabel XbimVersion
        {
            get { return string.Format("Xbim version {0}", Assembly.GetExecutingAssembly().GetName().Version); }
        }

        public static IfcLabel XbimApplicationFullName
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyTitleAttribute attr =
                    Attribute.GetCustomAttribute(assembly, typeof (AssemblyTitleAttribute), false) as
                    AssemblyTitleAttribute;
                return attr.Title;
            }
        }

        public static IfcLabel XbimApplicationDeveloper
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyCompanyAttribute attr =
                    Attribute.GetCustomAttribute(assembly, typeof (AssemblyCompanyAttribute), false) as
                    AssemblyCompanyAttribute;
                return attr.Company;
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

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _applicationDeveloper = value.EntityVal as IfcOrganization;
                    break;
                case 1:
                    _version = value.StringVal;
                    break;
                case 2:
                    _applicationFullName = value.StringVal;
                    break;
                case 3:
                    _applicationIdentifier = value.StringVal;
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