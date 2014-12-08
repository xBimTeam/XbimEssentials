#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcApprovalActorRelationship.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.ActorResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ApprovalResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcApprovalActorRelationship : IPersistIfcEntity, ISupportChangeNotification, INotifyPropertyChanged,
                                                INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcApprovalActorRelationship root = (IfcApprovalActorRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcApprovalActorRelationship left, IfcApprovalActorRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcApprovalActorRelationship left, IfcApprovalActorRelationship right)
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

        private IfcActorSelect _actor;
        private IfcApproval _approval;
        private IfcActorRole _role;

        #endregion

        #region Properties

        /// <summary>
        ///   The reference to the actor who is acting in the given role on the approval specified in this relationship.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcActorSelect Actor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _actor;
            }
            set { this.SetModelValue(this, ref _actor, value, v => Actor = v, "Actor"); }
        }

        /// <summary>
        ///   The approval on which the actor is acting in the role specified in this relationship.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcApproval Approval
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _approval;
            }
            set { this.SetModelValue(this, ref _approval, value, v => Approval = v, "Approval"); }
        }

        /// <summary>
        ///   The role of the actor w.r.t the approval. May be omitted, if the actor's general role implies also the role w.r.t the approval and does not need more detailed definition.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcActorRole Role
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _role;
            }
            set { this.SetModelValue(this, ref _role, value, v => Role = v, "Role"); }
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
                    _actor = (IfcActorSelect) value.EntityVal;
                    break;
                case 1:
                    _approval = (IfcApproval) value.EntityVal;
                    break;
                case 2:
                    _role = (IfcActorRole) value.EntityVal;
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