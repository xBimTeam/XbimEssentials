#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRepresentationContext.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    [IfcPersistedEntityAttribute]
    public class RepresentationContextSet : XbimSet<IfcRepresentationContext>
    {
        internal RepresentationContextSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        public override bool Remove(IfcRepresentationContext item)
        {
            if (item.RepresentationsInContext.Count() != 0)
                throw new Exception("RepresentationContext cannot be removed, Representations still reference it");
            return base.Remove(item);
        }

        public override void Clear()
        {
            foreach (IfcRepresentationContext rc in this)
            {
                if (rc.RepresentationsInContext.Count() != 0)
                    throw new Exception(
                        "RepresentationContext cannot be cleared, Representations still reference some of the contexts it");
            }
            base.Clear();
        }

        /// <summary>
        ///   Returns the Mandatory Model 3DView of ContextType = "Model"
        /// </summary>
        public IfcRepresentationContext ModelView
        {
            get { return this.FirstOrDefault(inst => inst.ContextType == "Model"); }
        }

        public IfcRepresentationContext this[string contextIdentifier]
        {
            get
            {
                foreach (IfcRepresentationContext context in this)
                {
                    if (context.ContextIdentifier == contextIdentifier)
                        return context;
                }
                return null;
            }
        }
    }

    /// <summary>
    ///   A representation context is a context in which a set of representation items are related.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A representation context is a context in which a set of representation items are related. 
    ///   Definition from IAI: The IfcRepresentationContext defines the context to which the IfcRepresentation of a product is related. 
    ///   NOTE  The definition of this class relates to the STEP entity representation_context. Please refer to ISO/IS 10303-43:1994 for the final definition of the formal standard.
    ///   HISTORY  New entity in IFC Release 1.5. 
    ///   IFC2x Edition 3 NOTE  Users should not instantiate the entity IfcRepresentationContext from IFC2x Edition 2 onwards. It will be changed into an ABSTRACT supertype in future releases of IFC.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcRepresentationContext : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                            INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcRepresentationContext root = (IfcRepresentationContext)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcRepresentationContext left, IfcRepresentationContext right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcRepresentationContext left, IfcRepresentationContext right)
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

        private IfcLabel? _contextIdentifier;
        private IfcLabel? _contextType;

        #endregion

        #region Events

        [field: NonSerialized] //don't serialize events
            public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The optional identifier of the representation context as used within a project.
        /// </summary>

        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? ContextIdentifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _contextIdentifier;
            }
            set
            {
                this.SetModelValue(this, ref _contextIdentifier, value, v => ContextIdentifier = v,
                                           "ContextIdentifier");
            }
        }

        /// <summary>
        ///   The description of the type of a representation context. The supported values for context type are to be specified by implementers agreements.
        /// </summary>

        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLabel? ContextType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _contextType;
            }
            set { this.SetModelValue(this, ref _contextType, value, v => ContextType = v, "ContextType"); }
        }

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _contextIdentifier = value.StringVal;
                    break;
                case 1:
                    _contextType = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverse functions

        /// <summary>
        ///   Inverse. All shape representations that are defined in the same representation context.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRepresentation> RepresentationsInContext
        {
            get { return ModelOf.Instances.Where<IfcRepresentation>(r => r.ContextOfItems == this); }
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

        #endregion

        #region ISupportIfcParser Members

        public virtual string WhereRule()
        {
            return "";
        }

        #endregion
    }
}