#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAppliedValueRelationship.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Common.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   An IfcAppliedValueRelationship is a relationship class that enables applied values of cost or environmental impact to be aggregated together as components of another applied value.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcAppliedValueRelationship is a relationship class that enables applied values of cost or environmental impact to be aggregated together as components of another applied value.
    ///   HISTORY: New Entity in IFC Release 2.0. Name changed in IFC 2x2 from IfcCostValueRelationship.
    ///   Use Definitions
    ///   Dependency relationships can exist between applied values on the basis that one particular value may be determined by operations on one or more other values. This is captured through the IfcAppliedValueRelationship entity. In this relationship, one instance of IfcAppliedValue acts as the principal (IfcAppliedValueRelationship.ComponentOf) whose value may be determined from the instances of IfcAppliedValue that are defined as its components (IfcAppliedValueRelationship.Components)
    ///   An IfcAppliedValueRelationship has an ArithmeticOperator attribute that specifies the form of arithmetical operation implied by the relationship.
    ///   A practical consideration when using the applied value relationship is that when the arithmetic operator is ADD, then the type of the IfcAppliedValue.AppliedValue attribute will be IfcMeasureWithUnit or IfcMonetaryMeasure whilst if the arithmetic operator is MULTIPLY, then the type of the IfcAppliedValue.AppliedValue attribute for one instance of IfcAppliedValue will be IfcMeasureWithUnit or IfcMonetaryMeasure whilst for other instances it will be IfcRatioMeasure.
    ///   Example
    ///   A relationship exists between applied value A and applied values B, C and D such that the value of A is determined by the addition of B, C and D such that:
    ///   A = B + C + D 
    ///  
    ///   It is possible to develop more complex applied value specifications by creating hierarchies of applied value relationships. In the diagram below, the development of a applied value is shown whereby, because B = E * F and D = G * H * J, then:
    ///   A = ((E * F) + C + (G * H * J)) 
    ///   EXPRESS specification:
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcAppliedValueRelationship : ISupportChangeNotification, INotifyPropertyChanged, IPersistIfcEntity,
                                               INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcAppliedValueRelationship root = (IfcAppliedValueRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcAppliedValueRelationship left, IfcAppliedValueRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcAppliedValueRelationship left, IfcAppliedValueRelationship right)
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

        public IfcAppliedValueRelationship()
        {
            _components = new XbimSet<IfcAppliedValue>(this);
        }

        #region Fields

        private IfcAppliedValue _componentOfTotal;
        private XbimSet<IfcAppliedValue> _components;
        private IfcArithmeticOperatorEnum _arithmeticOperator;
        private IfcLabel? _name;
        private IfcText? _description;

        #endregion

        /// <summary>
        ///   The applied value (total or subtotal) of which the value being considered is a component.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcAppliedValue ComponentOfTotal
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _componentOfTotal;
            }
            set
            {
                this.SetModelValue(this, ref _componentOfTotal, value, v => ComponentOfTotal = v,
                                           "ComponentOfTotal");
            }
        }

        /// <summary>
        ///   Applied values that are components of another applied value and from which that applied value may be deduced.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimSet<IfcAppliedValue> Components
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _components;
            }
            set { this.SetModelValue(this, ref _components, value, v => Components = v, "Components"); }
        }

        /// <summary>
        ///   The arithmetic operator applied in an applied value relationship.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcArithmeticOperatorEnum ArithmeticOperator
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _arithmeticOperator;
            }
            set
            {
                this.SetModelValue(this, ref _arithmeticOperator, value, v => ArithmeticOperator = v,
                                           "ArithmeticOperator");
            }
        }

        /// <summary>
        ///   A name used to identify or qualify the applied value relationship.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
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
        ///   A description that may apply additional information about an applied value relationship.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
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

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _componentOfTotal = (IfcAppliedValue) value.EntityVal;
                    break;
                case 1:
                    ((IXbimNoNotifyCollection)_components).Add((IfcAppliedValue) value.EntityVal);
                    break;
                case 2:
                    _arithmeticOperator =
                        (IfcArithmeticOperatorEnum)
                        Enum.Parse(typeof (IfcArithmeticOperatorEnum), value.EnumVal, true);
                    break;
                case 3:
                    _name = value.StringVal;
                    break;
                case 4:
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