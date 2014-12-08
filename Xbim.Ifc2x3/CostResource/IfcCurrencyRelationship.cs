#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCurrencyRelationship.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   An IfcCurrencyRelationship defines the rate of exchange that applies between two designated currencies at a particular time and as published by a particular source.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI:An IfcCurrencyRelationship defines the rate of exchange that applies between two designated currencies at a particular time and as published by a particular source. 
    ///   HISTORY: New Entity in IFC 2x2. 
    ///   Use Definitions
    ///   An IfcCurrencyRelationship is used where there may be a need to reference an IfcCostValue in one currency to an IfcCostValue in another currency. It takes account of fact that currency exchange rates may vary by requiring the recording the date and time of the currency exchange rate used and the source that publishes the rate. There may be many sources and there are different strategies for currency conversion (spot rate, forward buying of currency at a fixed rate). 
    ///   The source for the currency exchange is defined as an instance of IfcLibraryInformation that includes a name and a location (typically a URL, since most rates are now published in reliable sources via the web, although it may be a string value defining a lication of any type).
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcCurrencyRelationship : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                           INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcCurrencyRelationship root = (IfcCurrencyRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcCurrencyRelationship left, IfcCurrencyRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcCurrencyRelationship left, IfcCurrencyRelationship right)
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

        private IfcMonetaryUnit _relatingMonetaryUnit;
        private IfcMonetaryUnit _relatedMonetaryUnit;
        private IfcPositiveRatioMeasure _exchangeRate;
        private IfcDateAndTime _rateDateTime;
        private IfcLibraryInformation _rateSource;

        #endregion

        /// <summary>
        ///   The monetary unit from which an exchange is derived. For instance, in the case of a conversion from GBP to USD, the relating monetary unit is GBP.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcMonetaryUnit RelatingMonetaryUnit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingMonetaryUnit;
            }
            set
            {
                this.SetModelValue(this, ref _relatingMonetaryUnit, value, v => RelatingMonetaryUnit = v,
                                           "RelatingMonetaryUnit");
            }
        }

        /// <summary>
        ///   The monetary unit to which an exchange results. For instance, in the case of a conversion from GBP to USD, the related monetary unit is USD.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcMonetaryUnit RelatedMonetaryUnit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedMonetaryUnit;
            }
            set
            {
                this.SetModelValue(this, ref _relatedMonetaryUnit, value, v => RelatedMonetaryUnit = v,
                                           "RelatedMonetaryUnit");
            }
        }

        /// <summary>
        ///   The currently agreed ratio of the amount of a related monetary unit that is equivalent to a unit amount of the relating monetary unit in a currency relationship. For instance, in the case of a conversion from GBP to USD, the value of the exchange rate may be 1.486 (USD) : 1 (GBP).
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcPositiveRatioMeasure ExchangeRate
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _exchangeRate;
            }
            set { this.SetModelValue(this, ref _exchangeRate, value, v => ExchangeRate = v, "ExchangeRate"); }
        }

        /// <summary>
        ///   The date and time at which an exchange rate applies.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcDateAndTime RateDateTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rateDateTime;
            }
            set { this.SetModelValue(this, ref _rateDateTime, value, v => RateDateTime = v, "RateDateTime"); }
        }

        /// <summary>
        ///   The source from which an exchange rate is obtained.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLibraryInformation RateSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rateSource;
            }
            set { this.SetModelValue(this, ref _rateSource, value, v => RateSource = v, "RateSource"); }
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
                    _relatingMonetaryUnit = (IfcMonetaryUnit) value.EntityVal;
                    break;
                case 1:
                    _relatedMonetaryUnit = (IfcMonetaryUnit) value.EntityVal;
                    break;
                case 2:
                    _exchangeRate = value.RealVal;
                    break;
                case 3:
                    _rateDateTime = (IfcDateAndTime) value.EntityVal;
                    break;
                case 4:
                    _rateSource = (IfcLibraryInformation) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
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