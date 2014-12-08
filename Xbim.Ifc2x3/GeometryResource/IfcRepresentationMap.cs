#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRepresentationMap.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class RepresentationMapList : XbimListUnique<IfcRepresentationMap>
    {
        internal RepresentationMapList(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }


    [IfcPersistedEntityAttribute]
    public class IfcRepresentationMap : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                        INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcRepresentationMap root = (IfcRepresentationMap)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcRepresentationMap left, IfcRepresentationMap right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcRepresentationMap left, IfcRepresentationMap right)
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

        private IfcAxis2Placement _mappingOrigin;
        private IfcRepresentation _mappedRepresentation;

        #endregion

        #region Events

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement MappingOrigin
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _mappingOrigin;
            }
            set
            {
                if (value is IfcAxis2Placement2D || value is IfcAxis2Placement3D || value == null)
                    this.SetModelValue(this, ref _mappingOrigin, value, v => _mappingOrigin = v, "MappingOrigin");
                else
                    throw new ArgumentException("Illegal Axis2Placement type passed to RepresentationMap.MappingOrigin");
            }
        }

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcRepresentation MappedRepresentation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _mappedRepresentation;
            }
            set
            {
                this.SetModelValue(this, ref _mappedRepresentation, value, v => _mappedRepresentation = v,
                                           "MappedRepresentation");
            }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _mappingOrigin = (IfcAxis2Placement) value.EntityVal;
                    break;
                case 1:
                    _mappedRepresentation = (IfcRepresentation) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverses

        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcMappedItem> MapUsage
        {
            get { return ModelOf.Instances.Where<IfcMappedItem>(m => m.MappingSource == this); }
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

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}