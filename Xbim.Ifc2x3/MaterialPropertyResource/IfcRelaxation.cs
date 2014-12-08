using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;

using System.ComponentModel;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.MaterialPropertyResource
{
    /// <summary>
    /// Measure of the decrease in stress over long time interval resulting from plastic flow. 
    /// It describes the time dependent relative relaxation value for a given initial stress level at constant strain
    /// </summary>
    [IfcPersistedEntity]
    public class IfcRelaxation : IPersistIfcEntity, IPersistIfc, 
                                 INotifyPropertyChanged, ISupportChangeNotification, INotifyPropertyChanging
    {

        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcRelaxation root = (IfcRelaxation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcRelaxation left, IfcRelaxation right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcRelaxation left, IfcRelaxation right)
        {
            return !(left == right);
        }
        #region Fields

        private IfcNormalisedRatioMeasure _relaxationValue; 
        private IfcNormalisedRatioMeasure _initialStress;

        #endregion

        #region Properties

        /// <summary>
        ///  Time dependent loss of stress, relative to initial stress and therefore dimensionless.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcNormalisedRatioMeasure RelaxationValue
        {
            get
            {

                ((IPersistIfcEntity)this).Activate(false);

                return _relaxationValue;
            }
            set
            {
                this.SetModelValue(this, ref _relaxationValue, value, v => RelaxationValue = v, "RelaxationValue");
            }
        }

        /// <summary>
        ///  Stress at the beginning. Given as relative to the yield stress of the material and is therefore dimensionless.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcNormalisedRatioMeasure InitialStress
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _initialStress;
            }
            set
            {
                this.SetModelValue(this, ref _initialStress, value, v => InitialStress = v, "InitialStress");
            }
        }

        #endregion

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _relaxationValue = value.RealVal;
                    break;
                case 1:
                    _initialStress = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            return ""; 
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



    }
}
