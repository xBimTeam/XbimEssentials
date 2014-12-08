using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProductExtension;
using System.ComponentModel;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.GeometricConstraintResource
{


    [IfcPersistedEntityAttribute]
    public class IfcGridAxis : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                    INotifyPropertyChanging
    {

        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcGridAxis root = (IfcGridAxis)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcGridAxis left, IfcGridAxis right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcGridAxis left, IfcGridAxis right)
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

        IfcLabel? _axisTag;
        IfcCurve _axisCurve;
        bool _sameSense;
        /// <summary>
        ///   Optional. The tag or name for this grid axis.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? AxisTag
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _axisTag;
            }
            set
            {
                this.SetModelValue(this, ref _axisTag, value, v => AxisTag = v, "AxisTag");
            }
        }

        /// <summary>
        ///   Underlying curve which provides the geometry for this grid axis.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcCurve AxisCurve
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _axisCurve;
            }
            set
            {
                this.SetModelValue(this, ref _axisCurve, value, v => AxisCurve = v, "AxisCurve");
            }
        }

        /// <summary>
        ///   Defines whether the original sense of curve is used or whether it is reversed in the context of the grid axis.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public bool SameSense
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _sameSense;
            }
            set
            {
                this.SetModelValue(this, ref _sameSense, value, v => SameSense = v, "SameSense");
            }
        }



        /// <summary>
        ///   Inverse. If provided, the IfcGridAxis is part of the WAxes of IfcGrid.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcGrid> PartOfW
        {
            get
            {
                return ModelOf.Instances.Where<IfcGrid>(g => g.WAxes.Contains(this));
            }
        }

        /// <summary>
        ///   Inverse. If provided, the IfcGridAxis is part of the VAxes of IfcGrid.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcGrid> PartOfV
        {
            get
            {
                return ModelOf.Instances.Where<IfcGrid>(g => g.VAxes.Contains(this));
            }
        }

        /// <summary>
        ///   Inverse. If provided, the IfcGridAxis is part of the UAxes of IfcGrid.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcGrid> PartOfU
        {
            get
            {
                return ModelOf.Instances.Where<IfcGrid>(g => g.UAxes.Contains(this));
            }
        }
        /// <summary>
        ///   The reference to a set of 's, that connect other grid axes to this grid axis.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcVirtualGridIntersection> HasIntersections
        {
            get
            {
                return ModelOf.Instances.Where<IfcVirtualGridIntersection>(vg => vg.IntersectingAxes.Contains(this));
            }
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _axisTag = value.StringVal;
                    break;
                case 1:
                    _axisCurve = (IfcCurve)value.EntityVal;
                    break;
                case 2:
                    _sameSense = value.BooleanVal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("P21 index value out of range in {0}",
                                                                        this.GetType().Name));
            }
        }


        public string WhereRule()
        {
            string err = "";
            if (AxisCurve.Dim != 2)
                err += "WR1 IfcGridAxis: The dimensionality of the grid axis must be 2\n";
            bool u = PartOfU.Count() > 0;
            bool v = PartOfV.Count() > 0;
            bool w = PartOfW.Count() > 0;
            if (!(u ^ v ^ w))
                err += "WR2 IfcGridAxis: The IfcGridAxis needs to be used by exactly one of the three attributes of IfcGrid: i.e. it can only refer to a single instance of IfcGrid in one of the three list of axes.\n";
            return err;
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



    }
}
