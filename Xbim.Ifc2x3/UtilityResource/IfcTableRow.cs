#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTableRow.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.UtilityResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcTableRow : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                               INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcTableRow root = (IfcTableRow)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcTableRow left, IfcTableRow right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcTableRow left, IfcTableRow right)
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

        public IfcTableRow()
        {
            _rowCells = new XbimList<IfcValue>(this);
        }

        #region Fields

        private XbimList<IfcValue> _rowCells;
        private IfcBoolean _isHeading;

        #endregion

        /// <summary>
        ///   The value of information by row and column using the units defined. NOTE - The row value identifies both the actual value and the units in which it is recorded. Each cell (unique row and column) may have a different value AND different units. If the row is a heading row, then the row values are strings defined by the IfcString.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public XbimList<IfcValue> RowCells
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rowCells;
            }
            set { this.SetModelValue(this, ref _rowCells, value, v => RowCells = v, "RowCells"); }
        }

        /// <summary>
        ///   Flag which identifies if the row is a heading row or a row which contains row values. NOTE - If the row is a heading, the flag takes the value = TRUE.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcBoolean IsHeading
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _isHeading;
            }
            set { this.SetModelValue(this, ref _isHeading, value, v => IsHeading = v, "IsHeading"); }
        }

        /// <summary>
        ///   Reference to the IfcTable, in which the IfcTableRow is defined (or contained).
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory)]
        public IfcTable OfTable
        {
            get { return ModelOf.Instances.Where<IfcTable>(t => t.Rows.Contains(this)).FirstOrDefault(); }
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    ((IXbimNoNotifyCollection)_rowCells).Add((IfcValue) value.EntityVal);
                    break;
                case 1:
                    _isHeading = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }

            #endregion
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

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}