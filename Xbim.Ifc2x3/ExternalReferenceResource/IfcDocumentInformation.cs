#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDocumentInformation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcDocumentInformation captures "metadata" of an external document.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcDocumentInformation captures "metadata" of an external document. The actual content of the document is 
    ///   not defined in IFC ; instead, it can be found following the reference given to IfcDocumentReference. 
    ///   HISTORY: New entity in IFC 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcDocumentInformation : IfcDocumentSelect, INotifyPropertyChanged, ISupportChangeNotification,
                                          IPersistIfcEntity, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcDocumentInformation root = (IfcDocumentInformation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcDocumentInformation left, IfcDocumentInformation right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcDocumentInformation left, IfcDocumentInformation right)
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

        private IfcIdentifier _documentId;
        private IfcLabel _name;
        private IfcText? _description;
        private XbimSet<IfcDocumentReference> _documentReferences;
        private IfcText? _purpose;
        private IfcText? _intendedUse;
        private IfcText? _scope;
        private IfcLabel? _revision;
        private IfcActorSelect _documentOwner;
        private XbimSet<IfcActorSelect> _editors;
        private IfcDateAndTime _creationTime;
        private IfcDateAndTime _lastRevisionTime;
        private IfcDocumentElectronicFormat _electronicFormat;
        private IfcCalendarDate _validFrom;
        private IfcCalendarDate _validUntil;
        private IfcDocumentConfidentialityEnum? _confidentiality;
        private IfcDocumentStatusEnum? _status;

        #endregion

        /// <summary>
        ///   Identifier that uniquely identifies a document.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcIdentifier DocumentId
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _documentId;
            }
            set { this.SetModelValue(this, ref _documentId, value, v => DocumentId = v, "DocumentId"); }
        }

        /// <summary>
        ///   File name or document name assigned by owner.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        ///   Optional. Description of document and its content.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
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
        ///   Optional. Information on the referenced document.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public XbimSet<IfcDocumentReference> DocumentReferences
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _documentReferences;
            }
            set
            {
                this.SetModelValue(this, ref _documentReferences, value, v => DocumentReferences = v,
                                           "DocumentReferences");
            }
        }

        /// <summary>
        ///   Optional. Purpose for this document.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcText? Purpose
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _purpose;
            }
            set { this.SetModelValue(this, ref _purpose, value, v => Purpose = v, "Purpose"); }
        }

        /// <summary>
        ///   Optional. Intended use for this document.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcText? IntendedUse
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _intendedUse;
            }
            set { this.SetModelValue(this, ref _intendedUse, value, v => IntendedUse = v, "IntendedUse"); }
        }

        /// <summary>
        ///   Optional. Scope for this document.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcText? Scope
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _scope;
            }
            set { this.SetModelValue(this, ref _scope, value, v => Scope = v, "Scope"); }
        }

        /// <summary>
        ///   Optional. Document revision designation
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel? Revision
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _revision;
            }
            set { this.SetModelValue(this, ref _revision, value, v => Revision = v, "Revision"); }
        }

        /// <summary>
        ///   Optional. Information about the person and/or organization acknowledged as the 'owner' of this document. In some contexts, the document owner determines who has access to or editing right to the document.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcActorSelect DocumentOwner
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _documentOwner;
            }
            set { this.SetModelValue(this, ref _documentOwner, value, v => DocumentOwner = v, "DocumentOwner"); }
        }

        /// <summary>
        ///   Optional. The persons and/or organizations who have created this document or contributed to it.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public XbimSet<IfcActorSelect> Editors
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _editors;
            }
            set { this.SetModelValue(this, ref _editors, value, v => Editors = v, "Editors"); }
        }

        /// <summary>
        ///   Optional. Date and time stamp when the document was originally created.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcDateAndTime CreationTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _creationTime;
            }
            set { this.SetModelValue(this, ref _creationTime, value, v => CreationTime = v, "CreationTime"); }
        }

        /// <summary>
        ///   Optional. Date and time stamp when this document version was created.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcDateAndTime LastRevisionTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _lastRevisionTime;
            }
            set
            {
                this.SetModelValue(this, ref _lastRevisionTime, value, v => LastRevisionTime = v,
                                           "LastRevisionTime");
            }
        }

        /// <summary>
        ///   Optional. Describes the electronic format of the document being referenced, providing the file extension and the manner in which the content is provided.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcDocumentElectronicFormat ElectronicFormat
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _electronicFormat;
            }
            set
            {
                this.SetModelValue(this, ref _electronicFormat, value, v => ElectronicFormat = v,
                                           "ElectronicFormat");
            }
        }

        /// <summary>
        ///   Optional. Date, when the document becomes valid.
        /// </summary>
        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcCalendarDate ValidFrom
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _validFrom;
            }
            set { this.SetModelValue(this, ref _validFrom, value, v => ValidFrom = v, "ValidFrom"); }
        }

        /// <summary>
        ///   Optional. Date until which the document remains valid.
        /// </summary>
        [IfcAttribute(15, IfcAttributeState.Optional)]
        public IfcCalendarDate ValidUntil
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _validUntil;
            }
            set { this.SetModelValue(this, ref _validUntil, value, v => ValidUntil = v, "ValidUntil"); }
        }

        /// <summary>
        ///   Optional. The level of confidentiality of the document.
        /// </summary>
        [IfcAttribute(16, IfcAttributeState.Optional)]
        public IfcDocumentConfidentialityEnum? Confidentiality
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _confidentiality;
            }
            set
            {
                this.SetModelValue(this, ref _confidentiality, value, v => Confidentiality = v,
                                           "Confidentiality");
            }
        }

        /// <summary>
        ///   Optional. The current status of the document. Examples of status values that might be used for a document information status include:
        /// </summary>
        [IfcAttribute(17, IfcAttributeState.Optional)]
        public IfcDocumentStatusEnum? Status
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _status;
            }
            set { this.SetModelValue(this, ref _status, value, v => Status = v, "Status"); }
        }

        /// <summary>
        ///   An inverse relationship from the IfcDocumentInformationRelationship to the related documents.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcDocumentInformationRelationship> IsPointedTo
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcDocumentInformationRelationship>(
                        di => di.RelatedDocuments.Contains(this));
            }
        }

        /// <summary>
        ///   An inverse relationship from the IfcDocumentInformationRelationship to the relating document.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcDocumentInformationRelationship> IsPointer
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcDocumentInformationRelationship>(
                        di => di.RelatingDocument == this);
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
                    _documentId = value.StringVal;
                    break;
                case 1:
                    _name = value.StringVal;
                    break;
                case 2:
                    _description = value.StringVal;
                    break;
                case 3:
                    if (_documentReferences == null) _documentReferences = new XbimSet<IfcDocumentReference>(this);
                    ((IXbimNoNotifyCollection)_documentReferences).Add((IfcDocumentReference) value.EntityVal);
                    break;
                case 4:
                    _purpose = value.StringVal;
                    break;
                case 5:
                    _intendedUse = value.StringVal;
                    break;
                case 6:
                    _scope = value.StringVal;
                    break;
                case 7:
                    _revision = value.StringVal;
                    break;
                case 8:
                    _documentOwner = (IfcActorSelect) value.EntityVal;
                    break;
                case 9:
                    if (_editors == null) _editors = new XbimSet<IfcActorSelect>(this);
                    ((IXbimNoNotifyCollection)_editors).Add((IfcActorSelect) value.EntityVal);
                    break;
                case 10:
                    _creationTime = (IfcDateAndTime) value.EntityVal;
                    break;
                case 11:
                    _lastRevisionTime = (IfcDateAndTime) value.EntityVal;
                    break;
                case 12:
                    _electronicFormat = (IfcDocumentElectronicFormat) value.EntityVal;
                    break;
                case 13:
                    _validFrom = (IfcCalendarDate) value.EntityVal;
                    break;
                case 14:
                    _validUntil = (IfcCalendarDate) value.EntityVal;
                    break;
                case 15:
                    _confidentiality =
                        (IfcDocumentConfidentialityEnum)
                        Enum.Parse(typeof (IfcDocumentConfidentialityEnum), value.EnumVal, true);
                    break;
                case 16:
                    _status = (IfcDocumentStatusEnum) Enum.Parse(typeof (IfcDocumentStatusEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
        #region Collection initialisation methods
        /// <summary>
        ///   Initialise the Document Reference list and add passed IfcDocumentReference objects to list
        /// </summary>  
        /// <param name="clear">if true will clear existing list if existing</param>
        /// <param name="ifcDocumentReferences">IfcDocumentReference objects as list or ',' separated parameters</param>
        public XbimSet<IfcDocumentReference> SetDocumentReferences(bool clear, params IfcDocumentReference[] ifcDocumentReferences )
        {

            if (_documentReferences == null) _documentReferences = new XbimSet<IfcDocumentReference>(this);
            else if (clear) _documentReferences.Clear();
            foreach (IfcDocumentReference ifcDocumentReference in ifcDocumentReferences)
            {
                _documentReferences.Add(ifcDocumentReference);
            }
            return _documentReferences;
        }

        /// <summary>
        ///   Initialise the Editors List and add passed IfcActorSelect objects to list
        /// </summary>  
        /// <param name="clear">if true will clear existing list if existing</param>
        /// <param name="ifcActorSelects">IfcActorSelect objects as list or ',' separated parameters</param>
        public XbimSet<IfcActorSelect> SetEditors(bool clear, params IfcActorSelect[] ifcActorSelects )
        {

            if (_editors == null) _editors = new XbimSet<IfcActorSelect>(this);
            else if (clear) _editors.Clear();
            foreach (IfcActorSelect ifcActorSelect in ifcActorSelects)
            {
                _editors.Add(ifcActorSelect);
            }
            return _editors;
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
