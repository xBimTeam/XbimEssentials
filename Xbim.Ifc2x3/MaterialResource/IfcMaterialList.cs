#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMaterialList.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MaterialResource
{
    /// <summary>
    ///   A list of the different materials that are used in an element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A list of the different materials that are used in an element. 
    ///   NOTE: The class IfcMaterialList will normally be used where an element is described at a more abstract level. For example, for an architectural specification writer, the only information that may be needed about a concrete column is that it contains concrete, reinforcing steel and mild steel ligatures. It shall not be used for elements consisting of matsel layers when the different layers can be defined and the class IfcMaterialLayerSet can be used. Also, IfcMaterialList shall not be used for elements consisting of a single identifiable matsel, (e.g. to represent anisotropic matsel).
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcMaterialList : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                   IfcMaterialSelect, IfcObjectReferenceSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcMaterialList root = (IfcMaterialList)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcMaterialList left, IfcMaterialList right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcMaterialList left, IfcMaterialList right)
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

        public IfcMaterialList()
        {
            _materials = new XbimList<IfcMaterial>(this);
        }

        #region Fields

        private XbimList<IfcMaterial> _materials;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Materials used in a composition of substances.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, 1)]
        public XbimList<IfcMaterial> Materials
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _materials;
            }
            set { this.SetModelValue(this, ref _materials, value, v => Materials = v, "Materials"); }
        }

        #endregion

        #region ISupportIfcParser Members

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
            {
                ((IXbimNoNotifyCollection)_materials).Add((IfcMaterial)value.EntityVal);
            }
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

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

        #region MaterialSelect Members

        public string Name
        {
            get
            {
                StringBuilder str = new StringBuilder();
                foreach (IfcMaterial mat in Materials)
                {
                    str.AppendFormat("{0} ", mat.Name);
                }
                return str.ToString();
            }
        }

        #endregion
    }
}