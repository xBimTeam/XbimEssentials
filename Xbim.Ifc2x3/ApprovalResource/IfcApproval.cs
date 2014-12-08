#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcApproval.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ApprovalResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcApproval : IPersistIfcEntity, ISupportChangeNotification, INotifyPropertyChanged,
                               INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcApproval root = (IfcApproval)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcApproval left, IfcApproval right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcApproval left, IfcApproval right)
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

        private IfcIdentifier? _identifier;
        private IfcLabel? _name;
        private IfcText? _description;
        private IfcDateTimeSelect _approvalDateTime;
        private IfcLabel? _approvalStatus;
        private IfcLabel? _approvalLevel;
        private IfcText? _approvalQualifier;

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   A general textual description of a design, work task, plan, etc. that is being approved for.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
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
        ///   Date and time when the result of the approval process is produced.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcDateTimeSelect ApprovalDateTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _approvalDateTime;
            }
            set
            {
                this.SetModelValue(this, ref _approvalDateTime, value, v => ApprovalDateTime = v,
                                           "ApprovalDateTime");
            }
        }
        

        /// <summary>
        ///   The result or current status of the approval, e.g. Requested, Processed, Approved, Not Approved.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? ApprovalStatus
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _approvalStatus;
            }
            set { this.SetModelValue(this, ref _approvalStatus, value, v => ApprovalStatus = v, "ApprovalStatus"); }
        }

        /// <summary>
        ///   Level of the approval e.g. Draft v.s. Completed design.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLabel? ApprovalLevel
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _approvalLevel;
            }
            set { this.SetModelValue(this, ref _approvalLevel, value, v => ApprovalLevel = v, "ApprovalLevel"); }
        }

        /// <summary>
        ///   Textual description of special constraints or conditions for the approval.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcText? ApprovalQualifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _approvalQualifier;
            }
            set
            {
                this.SetModelValue(this, ref _approvalQualifier, value, v => ApprovalQualifier = v,
                                           "ApprovalQualifier");
            }
        }

        /// <summary>
        ///   A human readable name given to an approval.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
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
        ///   A computer interpretable identifier by which the approval is known.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcIdentifier? Identifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _identifier;
            }
            set { this.SetModelValue(this, ref _identifier, value, v => Identifier = v, "Identifier"); }
        }

        #endregion

        #region Inverses

        /// <summary>
        ///   The set of relationships by which resource objects that are are approved by this approval are known.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcResourceApprovalRelationship> ApprovedResources
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcResourceApprovalRelationship>(r => r.Approval == this);
            }
        }

        /// <summary>
        ///   The set of relationships by which the actors acting in specified roles on this approval are known.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcApprovalActorRelationship> Actors
        {
            get { return ModelOf.Instances.Where<IfcApprovalActorRelationship>(r => r.Approval == this); }
        }

        /// <summary>
        ///   The set of relationships by which this approval is related to others.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcApprovalRelationship> IsRelatedWith
        {
            get { return ModelOf.Instances.Where<IfcApprovalRelationship>(r => r.RelatedApproval == this); }
        }

        /// <summary>
        ///   The set of relationships by which other approvals are related to this one.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcApprovalRelationship> Relates
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcApprovalRelationship>(r => r.RelatingApproval == this);
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
                    _description = value.StringVal;
                    break;
                case 1:
                    _approvalDateTime = (IfcDateTimeSelect) value.EntityVal;
                    break;
                case 2:
                    _approvalStatus = value.StringVal;
                    break;
                case 3:
                    _approvalLevel = value.StringVal;
                    break;
                case 4:
                    _approvalQualifier = value.StringVal;
                    break;
                case 5:
                    _name = value.StringVal;
                    break;
                case 6:
                    _identifier = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            if (_identifier == null && _name == null)
                return
                    "HasIdentifierOrName Person:   Either Identifier or Name (or both) by which the approval is known shall be given.\n";
            else
                return "";
        }

        #endregion
    }
}
