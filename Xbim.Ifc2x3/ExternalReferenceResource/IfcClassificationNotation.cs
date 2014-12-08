#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcClassificationNotation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcClassificationNotation is a notation used from published reference (which may be either publicly available from a classification society or is published locally for the purposes of an organization, project or other purpose).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcClassificationNotation is a notation used from published reference (which may be either publicly available from a classification society or is published locally for the purposes of an organization, project or other purpose). 
    ///   HISTORY: New class in IFC Release 1.5. It has changed in IFC Release 2x. Documentation corrections made in IFC 2x Addendum 1
    ///   Use Definitions
    ///   A classification notation may be developed using various notation facets. A facet is a part of the actual notation but which has a specific meaning. For instance, it may be appropriate to classify an item by owning actor (represented by A=Architect) and by an entry from a classification table such as CI/SfB (represented by 210 for external wall). This gives a classification as A210.
    ///   All classifications of an object that are contained within the IFC model are made through the IfcClassificationNotation class. For a given object, the IfcRelAssociatesClassification class makes the connection between the IfcObject and the IfcClassificationNotation. 
    ///   It is a requirement that a classification notation can only bring together facets from the same classification system or source. Bringing together notation facets from different sources within the same classification notation is not allowed. However, multiple classifications can be applied to a single object through the use of more than one instance of IfcRelAssociatesClassification. In this way it is possible to define multiple classification notations where each notation contains facets from a single source.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcClassificationNotation : IPersistIfcEntity, INotifyPropertyChanged, ISupportChangeNotification,
                                             IfcClassificationNotationSelect, INotifyPropertyChanging
    {

        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcClassificationNotation root = (IfcClassificationNotation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcClassificationNotation left, IfcClassificationNotation right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcClassificationNotation left, IfcClassificationNotation right)
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

        public IfcClassificationNotation()
        {
            _notationFacets = new XbimSet<IfcClassificationNotationFacet>(this);
        }

        #region Fields

        private XbimSet<IfcClassificationNotationFacet> _notationFacets;

        #endregion

        /// <summary>
        ///   Alphanumeric characters in defined groups from which the classification notation is derived.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public XbimSet<IfcClassificationNotationFacet> NotationFacets
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _notationFacets;
            }
            set { this.SetModelValue(this, ref _notationFacets, value, v => NotationFacets = v, "NotationFacets"); }
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
                    ((IXbimNoNotifyCollection)_notationFacets).Add((IfcClassificationNotationFacet) value.EntityVal);
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