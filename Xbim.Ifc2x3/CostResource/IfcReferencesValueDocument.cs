#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReferencesValueDocument.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   An IfcReferencesValueDocument is a means of referencing many instances of IfcAppliedValue to a single document where the document is a price list, quotation, list of environmental impact values or other source of information.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcReferencesValueDocument is a means of referencing many instances of IfcAppliedValue to a single document where the document is a price list, quotation, list of environmental impact values or other source of information. 
    ///   HISTORY: New class in IFC Release 2x. Name changed from IfcReferencesCostDocument in IFC 2x2
    ///   Use Definitions
    ///   The purpose of this class is to be able to identify a reference source from which applied values are obtained. Since many objects may be obtain such values from the same referenced document, use of a relationship class allows the document to be identified once only when information is exchanged or shared rather than many times.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcReferencesValueDocument : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                              INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcReferencesValueDocument root = (IfcReferencesValueDocument)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcReferencesValueDocument left, IfcReferencesValueDocument right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcReferencesValueDocument left, IfcReferencesValueDocument right)
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

        public IfcReferencesValueDocument()
        {
            _referencingValues = new XbimSet<IfcAppliedValue>(this);
        }

        #region Fields

        private IfcDocumentSelect _referencedDocument;
        private XbimSet<IfcAppliedValue> _referencingValues;
        private IfcLabel? _name;
        private IfcText? _description;

        #endregion

        /// <summary>
        ///   A document such as a price list or quotation from which costs are obtained.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcDocumentSelect ReferencedDocument
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _referencedDocument;
            }
            set
            {
                this.SetModelValue(this, ref _referencedDocument, value, v => ReferencedDocument = v,
                                           "ReferencedDocument");
            }
        }

        /// <summary>
        ///   Costs obtained from a single document such as a price list or quotation.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public XbimSet<IfcAppliedValue> ReferencingValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _referencingValues;
            }
            set
            {
                this.SetModelValue(this, ref _referencingValues, value, v => ReferencingValues = v,
                                           "ReferencingValues");
            }
        }

        /// <summary>
        ///   Optional. A name used to identify or qualify the relationship to the document from which values may be referenced..
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
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
        ///   Optional. A description of the relationship to the document from which values may be referenced.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = v, "Description"); }
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        #endregion

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
                    _referencedDocument = (IfcDocumentSelect) value.EntityVal;
                    break;
                case 1:
                    ((IXbimNoNotifyCollection)_referencingValues).Add((IfcAppliedValue) value.EntityVal);
                    break;
                case 2:
                    _name = value.StringVal;
                    break;
                case 3:
                    _description = value.StringVal;
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