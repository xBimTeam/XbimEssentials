using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// Definition from IAI: The draughting callout relationship establishes a logical relationship between two draughting callouts. The meaning of this relationship is given at the subtypes of this entity.
    ///
    /// NOTE: The IfcDraughtingCalloutRelationship is an entity that had been adopted from ISO 10303, Industrial automation systems and integration—Product data representation and exchange, Part 101: Integrated application resources: Draughting.
    /// 
    /// NOTE Corresponding STEP name: draughting_callout_relationship. Please refer to ISO/IS 10303-101:1994 page 21 for the final definition of the formal standard. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcDraughtingCalloutRelationship : IPersistIfcEntity, ISupportChangeNotification
    {
        //Name : OPTIONAL IfcLabel;
	    //Description : OPTIONAL IfcText;
	    //RelatingDraughtingCallout : IfcDraughtingCallout;
        //RelatedDraughtingCallout : IfcDraughtingCallout;

        #region fields
        private IfcLabel _name;
        private IfcText _description;
        private IfcDraughtingCallout _relatingDraughtingCallout;
        private IfcDraughtingCallout _relatedDraughtingCallout;
        #endregion

        /// <summary>
        /// The word or group of words by which the relationship is referred to. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        /// Additional informal description of the relationship. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcText Description
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = v, "Description"); }
        }

        /// <summary>
        /// One of the draughting callouts which is a part of the relationship. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcDraughtingCallout RelatingDraughtingCallout
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _relatingDraughtingCallout;
            }
            set { this.SetModelValue(this, ref _relatingDraughtingCallout, value, v => RelatingDraughtingCallout = v, "RelatingDraughtingCallout"); }
        }

        /// <summary>
        /// The other of the draughting callouts which is a part of the relationship. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcDraughtingCallout RelatedDraughtingCallout
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _relatedDraughtingCallout;
            }
            set { this.SetModelValue(this, ref _relatedDraughtingCallout, value, v => RelatedDraughtingCallout = v, "RelatedDraughtingCallout"); }
        }


        private IModel _model;
        private int _entityLabel;
        private bool _activated;

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock (this) { if (_model != null && !_activated) _activated = _model.Activate(this, false) > 0; }
            if (write) _model.Activate(this, write);
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _model = model;
            _entityLabel = entityLabel;
            _activated = activated;
        }

        IModel IPersistIfcEntity.ModelOf
        {
            get { return _model; }
        }

        int IPersistIfcEntity.EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfc.IfcParse(int propIndex, IPropertyValue value)
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
                    _relatingDraughtingCallout = (IfcDraughtingCallout)(value.EntityVal);
                    break;
                case 3:
                    _relatedDraughtingCallout = (IfcDraughtingCallout)(value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public virtual string WhereRule()
        {
            return "";
        }

        [field: NonSerialized] //don't serialize events
        private event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized] //don't serialize event
        private event PropertyChangingEventHandler PropertyChanging;

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

        event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        event System.ComponentModel.PropertyChangingEventHandler System.ComponentModel.INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }
    }
}
