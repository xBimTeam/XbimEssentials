#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcClassificationNotationFacet.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcClassificationNotationFacet is a group of alphanumeric characters used within a classification notation.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcClassificationNotationFacet is a group of alphanumeric characters used within a classification notation. 
    ///   HISTORY: New class in IFC Release 1.5. Modified in IFC 2x.
    ///   Use Definitions
    ///   An IfcClassificationNotationFacet object holds an individual classification value that is to be assigned to an object through IfcClassificationNotation and IfcRelAssociatesClassification objects.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcClassificationNotationFacet : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                                  INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcClassificationNotationFacet root = (IfcClassificationNotationFacet)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcClassificationNotationFacet left, IfcClassificationNotationFacet right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcClassificationNotationFacet left, IfcClassificationNotationFacet right)
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


        private IfcLabel _notationValue;

        /// <summary>
        ///   The notation value that specifies the classification e.g. 'L781'
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcLabel NotationValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _notationValue;
            }
            set { this.SetModelValue(this, ref _notationValue, value, v => NotationValue = v, "NotationValue"); }
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
                    _notationValue = value.StringVal;
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