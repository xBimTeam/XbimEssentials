#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDocumentElectronicFormat.cs
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
    ///   An IfcDocumentElectronicFormat captures the type of document being referenced as an external source,and for which metadata is specified by IfcDocumentInformation.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcDocumentElectronicFormat captures the type of document being referenced as an external source,and for which metadata is specified by IfcDocumentInformation. 
    ///   HISTORY: New entity in IFC 2x
    ///   Formal Propositions:
    ///   WR1   :   In order to specify a valid electronic document format either the file extension or the MIME type has to be given.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcDocumentElectronicFormat : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                               INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcDocumentElectronicFormat root = (IfcDocumentElectronicFormat)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcDocumentElectronicFormat left, IfcDocumentElectronicFormat right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcDocumentElectronicFormat left, IfcDocumentElectronicFormat right)
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

        private IfcLabel? _fileExtension;
        private IfcLabel? _mimeContentType;
        private IfcLabel? _mimeSubtype;

        #endregion

        /// <summary>
        ///   Optional. File extension of electronic document used by computer operating system.
        /// </summary>
        public IfcLabel? FileExtension
        {
            get { return _fileExtension; }
            set { this.SetModelValue(this, ref _fileExtension, value, v => FileExtension = v, "FileExtension"); }
        }

        /// <summary>
        ///   Optional. Main Mime type (as published by W3C or as user defined application type)
        /// </summary>
        public IfcLabel? MimeContentType
        {
            get { return _mimeContentType; }
            set
            {
                this.SetModelValue(this, ref _mimeContentType, value, v => MimeContentType = v,
                                           "MimeContentType");
            }
        }

        /// <summary>
        ///   Optional. Mime subtype information.
        /// </summary>
        public IfcLabel? MimeSubtype
        {
            get { return _mimeSubtype; }
            set { this.SetModelValue(this, ref _mimeSubtype, value, v => MimeSubtype = v, "MimeSubtype"); }
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
                    _fileExtension = value.StringVal;
                    break;
                case 1:
                    _mimeContentType = value.StringVal;
                    break;
                case 2:
                    _mimeSubtype = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public string WhereRule()
        {
            if (!_fileExtension.HasValue && !_mimeContentType.HasValue)
                return
                    "WR1 DocumentElectronicFormat : In order to specify a valid electronic document format either the file extension or the MIME type has to be given.\n";

            else
                return "";
        }

        #endregion
    }
}