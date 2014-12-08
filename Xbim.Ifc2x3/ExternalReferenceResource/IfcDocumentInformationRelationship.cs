#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDocumentInformationRelationship.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Common.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcDocumentInformationRelationship is a relationship class that enables a document to have the ability to reference other documents.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcDocumentInformationRelationship is a relationship class that enables a document to have the ability to reference other documents.
    ///   HISTORY: New entity in IFC Release 2x.
    ///   Use Definitions
    ///   This class can be used to describe relationships in which one document may reference one or more other sub documents or where a document is used as a replacement for another document (but where both the original and the replacing document need to be retained).
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcDocumentInformationRelationship : INotifyPropertyChanged, ISupportChangeNotification,
                                                      IPersistIfcEntity, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcDocumentInformationRelationship root = (IfcDocumentInformationRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcDocumentInformationRelationship left, IfcDocumentInformationRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcDocumentInformationRelationship left, IfcDocumentInformationRelationship right)
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


        public IfcDocumentInformationRelationship()
        {
            _relatedDocuments = new XbimSet<IfcDocumentInformation>(this);
        }

        #region Fields

        private IfcDocumentInformation _relatingDocument;
        private XbimSet<IfcDocumentInformation> _relatedDocuments;
        private IfcLabel? _relationshipType;

        #endregion

        /// <summary>
        ///   The document that acts as the parent, referencing or original document in a relationship.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcDocumentInformation RelatingDocument
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingDocument;
            }
            set
            {
                this.SetModelValue(this, ref _relatingDocument, value, v => RelatingDocument = v,
                                           "RelatingDocument");
            }
        }

        /// <summary>
        ///   The document that acts as the child, referenced or replacing document in a relationship.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcDocumentInformation> RelatedDocuments
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedDocuments;
            }
            set
            {
                this.SetModelValue(this, ref _relatedDocuments, value, v => RelatedDocuments = v,
                                           "RelatedDocuments");
            }
        }

        /// <summary>
        ///   Optional. Describes the type of relationship between documents. This could be sub-document, replacement etc. The interpretation has to be established in an application context.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? RelationshipType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relationshipType;
            }
            set
            {
                this.SetModelValue(this, ref _relationshipType, value, v => RelationshipType = v,
                                           "RelationshipType");
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
                    _relatingDocument = (IfcDocumentInformation) value.EntityVal;
                    break;
                case 1:
                    ((IXbimNoNotifyCollection)_relatedDocuments).Add((IfcDocumentInformation) value.EntityVal);
                    break;
                case 2:
                    _relationshipType = value.StringVal;
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