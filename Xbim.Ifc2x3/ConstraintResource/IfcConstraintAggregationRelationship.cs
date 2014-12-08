#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConstraintAggregationRelationship.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ConstraintResource
{
    /// <summary>
    ///   An IfcConstraintAggregationRelationship is an objectified relationship that enables instances
    ///   of IfcConstraint and its subtypes to be aggregated together logically.
    /// </summary>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcConstraintAggregationRelationship : INotifyPropertyChanged, ISupportChangeNotification,
                                                        IPersistIfcEntity, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcConstraintAggregationRelationship root = (IfcConstraintAggregationRelationship)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcConstraintAggregationRelationship left, IfcConstraintAggregationRelationship right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcConstraintAggregationRelationship left, IfcConstraintAggregationRelationship right)
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

        public IfcConstraintAggregationRelationship()
        {
            _relatedConstraints = new XbimListUnique<IfcConstraint>(this);
        }

        #region Fields

        private IfcLabel? _name;
        private IfcText? _description;
        private IfcConstraint _relatingConstraint;
        private XbimListUnique<IfcConstraint> _relatedConstraints;
        private IfcLogicalOperatorEnum _logicalAggregator;

        #endregion

        /// <summary>
        ///   A name used to identify or qualify the constraint aggregation.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
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
        ///   A description that may apply additional information about a constraint aggregation.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = v, "Description"); }
        }

        /// <summary>
        ///   Constraint to which the other Constraints are associated.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcConstraint RelatingConstraint
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingConstraint;
            }
            set
            {
                this.SetModelValue(this, ref _relatingConstraint, value, v => RelatingConstraint = v,
                                           "RelatingConstraint");
            }
        }

        /// <summary>
        ///   Constraints that are aggregated in using the LogicalAggregator.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.ListUnique, IfcAttributeType.Class, 1)]
        public XbimListUnique<IfcConstraint> RelatedConstraints
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedConstraints;
            }
            set
            {
                this.SetModelValue(this, ref _relatedConstraints, value, v => RelatedConstraints = v,
                                           "RelatedConstraints");
            }
        }

        /// <summary>
        ///   Enumeration that identifies the logical type of aggregation.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcLogicalOperatorEnum LogicalAggregator
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _logicalAggregator;
            }
            set
            {
                this.SetModelValue(this, ref _logicalAggregator, value, v => LogicalAggregator = v,
                                           "LogicalAggregator");
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

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _name = value.StringVal;
                    break;
                case 1:
                    _description = value.StringVal;
                    break;
                case 2:
                    _relatingConstraint = (IfcConstraint) value.EntityVal;
                    break;
                case 3:
                    ((IXbimNoNotifyCollection)_relatedConstraints).Add((IfcConstraint) value.EntityVal);
                    break;
                case 4:
                    _logicalAggregator =
                        (IfcLogicalOperatorEnum) Enum.Parse(typeof (IfcLogicalOperatorEnum), value.EnumVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public string WhereRule()
        {
            string err = "";
            if (_relatedConstraints.Contains(_relatingConstraint))
                err +=
                    "WR11 ConstraintAggregationRelationship : The instance to which the relation RelatingConstraint points shall not be the same as the RelatedConstraint.\n";
            if (_relatedConstraints.Distinct().Count() != _relatedConstraints.Count)
                err +=
                    "UNIQUE ConstraintAggregationRelationship : RelatedContraints contains duplicate values, only unique values are permitted\n";
            return err;
        }

        #endregion
    }
}