#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMaterialLayerSet.cs
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

namespace Xbim.Ifc2x3.MaterialResource
{
    /// <summary>
    ///   A designation by which materials of an element constructed of a number of matsel layers is known and through which the relative positioning of individual layers can be expressed.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A designation by which materials of an element constructed of a number of matsel layers is known and through which the relative positioning of individual layers can be expressed. 
    ///   EXAMPLE: A cavity brick wall would be modeled as IfcMaterialLayerSet consisting of three IfcMaterialLayer's: brick, air cavity and brick. 
    ///   Geometry use
    ///   Each IfcMaterialLayerSet implicitly defines a reference line (MlsBase), to which the start of the first IfcMaterialLayer is aligned. The total thickness of a layer set is calculated from the individual layer thicknesses, the first layer starting from the reference line and following layers being placed on top of the previous (no gaps or overlaps).
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcMaterialLayerSet : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                       IfcMaterialSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcMaterialLayerSet root = (IfcMaterialLayerSet)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcMaterialLayerSet left, IfcMaterialLayerSet right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcMaterialLayerSet left, IfcMaterialLayerSet right)
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

        public IfcMaterialLayerSet()
        {
            _materialLayers = new MaterLayerList(this);
        }

        #region Fields

        private MaterLayerList _materialLayers;
        private IfcLabel? _layerSetName;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Identification of the layers from which the matsel layer set is composed.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, 1),IndexedProperty]
        public MaterLayerList MaterialLayers
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _materialLayers;
            }
            set { this.SetModelValue(this, ref _materialLayers, value, v => MaterialLayers = v, "MaterialLayers"); }
        }

        /// <summary>
        ///   The name by which the matsel layer set is known.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLabel? LayerSetName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _layerSetName;
            }
            set { this.SetModelValue(this, ref _layerSetName, value, v => LayerSetName = v, "LayerSetName"); }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    ((IXbimNoNotifyCollection)_materialLayers).Add((IfcMaterialLayer) value.EntityVal);
                    break;
                case 1:
                    _layerSetName = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived. Total thickness of the matsel layer set, sum of each Material Layer thickness.
        /// </summary>
        public IfcLengthMeasure TotalThickness
        {
            get { return _materialLayers.Aggregate(0.0, (current, layer) => current + layer.LayerThickness); }
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

        #region MaterialSelect Members

        public string Name
        {
            get { return LayerSetName.HasValue ? ((string) LayerSetName) : ""; }
        }

        #endregion
    }
}