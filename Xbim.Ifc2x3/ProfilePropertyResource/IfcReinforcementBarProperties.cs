#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReinforcementBarProperties.cs
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

namespace Xbim.Ifc2x3.ProfilePropertyResource
{
    [IfcPersistedEntityAttribute]
    public class IfcReinforcementBarProperties : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                                 INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcReinforcementBarProperties root = (IfcReinforcementBarProperties)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcReinforcementBarProperties left, IfcReinforcementBarProperties right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcReinforcementBarProperties left, IfcReinforcementBarProperties right)
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

        private IfcAreaMeasure _totalCrossSectionArea;
        private IfcLabel _steelGrade;
        private IfcReinforcingBarSurfaceEnum? _barSurface;
        private IfcLengthMeasure? _effectiveDepth;
        private IfcPositiveLengthMeasure? _nominalBarDiameter;
        private IfcCountMeasure? _barCount;

        #endregion

        #region Properties

        /// <summary>
        ///   The total effective cross-section area of the reinforcement of a specific steel grade.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcAreaMeasure TotalCrossSectionArea
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _totalCrossSectionArea;
            }
            set
            {
                this.SetModelValue(this, ref _totalCrossSectionArea, value, v => TotalCrossSectionArea = v,
                                           "TotalCrossSectionArea");
            }
        }

        /// <summary>
        ///   The nominal steel grade defined according to local standards.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcLabel SteelGrade
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _steelGrade;
            }
            set { this.SetModelValue(this, ref _steelGrade, value, v => SteelGrade = v, "SteelGrade"); }
        }

        ///<summary>
        ///  Indicator for whether the bar surface is plain or textured.
        ///</summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcReinforcingBarSurfaceEnum? BarSurface
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _barSurface;
            }
            set { this.SetModelValue(this, ref _barSurface, value, v => BarSurface = v, "BarSurface"); }
        }

        /// <summary>
        ///   The effective depth, i.e. the distance of the specific reinforcement cross section area or reinforcement configuration in a row, counted from a common specific reference point. Usually 
        ///   the reference point is the upper surface (for beams and slabs) or a similar projection in a plane (for columns).
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLengthMeasure? EffectiveDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _effectiveDepth;
            }
            set { this.SetModelValue(this, ref _effectiveDepth, value, v => EffectiveDepth = v, "EffectiveDepth"); }
        }

        /// <summary>
        ///   The nominal diameter defining the cross-section size of the reinforcing bar. 
        ///   The bar diameter should be identical for all bars included in the specific reinforcement configuration.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? NominalBarDiameter
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _nominalBarDiameter;
            }
            set
            {
                this.SetModelValue(this, ref _nominalBarDiameter, value, v => NominalBarDiameter = v,
                                           "NominalBarDiameter");
            }
        }

        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcCountMeasure? BarCount
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _barCount;
            }
            set { this.SetModelValue(this, ref _barCount, value, v => BarCount = v, "BarCount"); }
        }

        #endregion

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _totalCrossSectionArea = value.RealVal;
                    break;
                case 1:
                    _steelGrade = value.StringVal;
                    break;
                case 2:
                    _barSurface =
                        (IfcReinforcingBarSurfaceEnum)
                        Enum.Parse(typeof (IfcReinforcingBarSurfaceEnum), value.EnumVal, true);
                    break;
                case 3:
                    _effectiveDepth = value.RealVal;
                    break;
                case 4:
                    _nominalBarDiameter = value.RealVal;
                    break;
                case 5:
                    _barCount = value.NumberVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
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

        #region IPersistIfc Members

        public virtual string WhereRule()
        {
            return "";
        }

        #endregion
    }
}