#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTable.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Linq;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.UtilityResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcTable : IPersistIfcEntity, IfcMetricValueSelect, ISupportChangeNotification, INotifyPropertyChanged,
                            INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcTable root = (IfcTable)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcTable left, IfcTable right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcTable left, IfcTable right)
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

        public IfcTable()
        {
            _rows = new XbimList<IfcTableRow>(this);
        }

        #region Fields

        private string _name;
        private XbimList<IfcTableRow> _rows;

        #endregion

        /// <summary>
        ///   A unique name which is intended to describe the usage of the Table.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public string Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }


        /// <summary>
        ///   Reference to information content of rows.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public XbimList<IfcTableRow> Rows
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rows;
            }
            set { this.SetModelValue(this, ref _rows, value, v => Rows = v, "Rows"); }
        }

        /// <summary>
        ///   The number of rows in a table that contains data, i.e. total number of rows minus number of heading rows in table.
        /// </summary>
        public int NumberOfDataRows
        {
            get { return _rows.Count(tr => tr.IsHeading == false); }
        }

        /// <summary>
        ///   The number of cells in each row, this complies to the number of columns in a table. See WR2 that ensures that each row has the same number of cells. The actual value is derived from the first member of the Rows list.
        /// </summary>
        public int NumberOfCellsInRow
        {
            get
            {
                IfcTableRow row1 = Rows.FirstOrDefault();
                return row1 != null ? row1.RowCells.Count : 0;
            }
        }

        /// <summary>
        ///   The number of headings in a table. This is restricted by WR3 to max. one.
        /// </summary>
        public int NumberOfHeadings
        {
            get { return _rows.Count(tr => tr.IsHeading == true); }
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
                    _name = value.StringVal;
                    break;
                case 1:
                    ((IXbimNoNotifyCollection)_rows).Add((IfcTableRow) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public string WhereRule()
        {
            string err = "";
            bool first = true;
            int numCells = 0;
            foreach (IfcTableRow row in _rows)
            {
                if (first)
                {
                    numCells = row.RowCells.Count;
                    first = false;
                }
                else if (numCells != row.RowCells.Count)
                {
                    err += "WR1 Table : All Rows in a table must have the same number of cells\n";
                    break;
                }
            }

            if (NumberOfHeadings > 1)
                err += "WR3 Table : Only 0 or one heading row is permitted per table\n";
            return err;
        }

        #endregion
    }
}