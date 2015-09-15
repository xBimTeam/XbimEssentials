#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRoot.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.UtilityResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.Transactions;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   The IfcRoot is the most abstract and root class for all IFC entity definitions that roots in the kernel or in subsequent layers of the IFC object model.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcRoot is the most abstract and root class for all IFC entity definitions that roots in the kernel or in subsequent layers of the IFC object model. It is therefore the common supertype all all IFC entities, beside those defined in an IFC resource schema. All entities that are subtypes of IfcRoot can be used independently, whereas resource schema entities, that are not subtypes of IfcRoot, are not supposed to be independent entities. 
    ///   The IfcRoot assigns the globally unique ID, and the ownership and history information to the entity. In addition it may provide for a name and a description about the concepts.
    ///   HISTORY New entity in IFC Release 1.0
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcRoot : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                    INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcRoot root = (IfcRoot)obj;
            return this==root;
        }
        public override sealed int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcRoot left, IfcRoot right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

           return  (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);
          
        }

        public static bool operator !=(IfcRoot left, IfcRoot right)
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
            if (write)
            {
                _model.Activate(this, write);
                if (_model.AutoAddOwnerHistory &&   _ownerHistory != (_model.OwnerHistoryAddObject as IfcOwnerHistory)) //no need to do it if it is already set
                {
                    if (_ownerHistory != (_model.OwnerHistoryModifyObject as IfcOwnerHistory))
                    {
                        Transaction.AddPropertyChange(v => OwnerHistory = v, _ownerHistory, (IfcOwnerHistory)_model.OwnerHistoryModifyObject);
                        ((ISupportChangeNotification)this).NotifyPropertyChanging("OwnerHistory");
                        _ownerHistory = (IfcOwnerHistory)_model.OwnerHistoryModifyObject;
                        ((ISupportChangeNotification)this).NotifyPropertyChanged("OwnerHistory");
                    }
                }
            }
        }

        #endregion


        #region Fields

        private IfcGloballyUniqueId _globalId;
        private IfcOwnerHistory _ownerHistory;
        private IfcLabel? _name;
        private IfcText? _description;

        #endregion

        #region Constructors & Initialisers

        public IfcRoot()
        {
            _globalId = IfcGloballyUniqueId.NewGuid();
        }

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Assignment of a globally unique identifier within the entire software world
        /// </summary>
        [IdentityProperty]
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcGloballyUniqueId GlobalId
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _globalId;
            }
            set { this.SetModelValue(this, ref _globalId, value, v => GlobalId = v, "GlobalId"); }
        }


        /// <summary>
        ///   Assignment of the information about the current ownership of that object, including owning actor, application, local identification and information captured about the recent changes of the object, NOTE: only the last modification in stored.
        /// </summary>
        /// TODO: Resolve this
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcOwnerHistory OwnerHistory
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                
                return _ownerHistory;
            }
            set { this.SetModelValue(this, ref _ownerHistory, value, v => OwnerHistory = v, "OwnerHistory"); }
        }

        /// <summary>
        ///   Optional name for use by the participating software systems or users. For some subtypes of IfcRoot the insertion of the Name attribute may be required. This would be enforced by a where rule.
        /// </summary>
        [IdentityComponent]
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
        ///   Optional description, provided for exchanging informative comments.
        /// </summary>
        /// <remarks>
        /// </remarks>
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

        #endregion

        #region Part 21 Step file Parse routines

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _globalId = value.StringVal;
                    break;
                case 1:
                    _ownerHistory = (IfcOwnerHistory) value.EntityVal;
                    break;
                case 2:
                    _name = value.StringVal;
                    break;
                case 3:
                    _description = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("#{0} = {1}", _entityLabel, GetType().Name);
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
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

        public abstract string WhereRule();

        #endregion
    }
}