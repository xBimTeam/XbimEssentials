#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConstraint.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ConstraintResource
{
    /// <summary>
    ///   Abstract class. An IfcConstraint is used to define a constraint or limiting value or boundary condition 
    ///   that may be applied to an object or to the value of a property.
    /// </summary>
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcConstraint : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                          INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcConstraint root = (IfcConstraint)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcConstraint left, IfcConstraint right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcConstraint left, IfcConstraint right)
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

        private IfcLabel _name;
        private IfcText? _description;
        private IfcConstraintEnum _constraintGrade;
        private IfcLabel? _constraintSource;
        private IfcActorSelect _creatingActor;
        private IfcDateTimeSelect _creationTime;
        private IfcLabel? _userDefinedGrade;

        #endregion

        /// <summary>
        ///   A name to be used for the constraint (e.g., ChillerCoefficientOfPerformance).
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        ///   A description that may apply additional information about a constraint.
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
        ///   Enumeration that qualifies the type of constraint.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcConstraintEnum ConstraintGrade
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _constraintGrade;
            }
            set
            {
                this.SetModelValue(this, ref _constraintGrade, value, v => ConstraintGrade = v,
                                           "ConstraintGrade");
            }
        }

        /// <summary>
        ///   Any source material, such as a code or standard, from which the constraint originated.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLabel? ConstraintSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _constraintSource;
            }
            set
            {
                this.SetModelValue(this, ref _constraintSource, value, v => ConstraintSource = v,
                                           "ConstraintSource");
            }
        }

        /// <summary>
        ///   Person and/or organization that has created the constraint.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcActorSelect CreatingActor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _creatingActor;
            }
            set { this.SetModelValue(this, ref _creatingActor, value, v => CreatingActor = v, "CreatingActor"); }
        }

        /// <summary>
        ///   Time when information specifying the constraint instance was created.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcDateTimeSelect CreationTime
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _creationTime;
            }
            set { this.SetModelValue(this, ref _creationTime, value, v => CreationTime = v, "CreationTime"); }
        }

        /// <summary>
        ///   Allows for specification of user defined grade of the constraint beyond the enumeration values (hard, soft, advisory) 
        ///   provided by ConstraintGrade attribute of type ConstraintEnum. 
        ///   When a value is provided for attribute UserDefinedGrade in parallel the attribute ConstraintGrade 
        ///   shall have enumeration value USERDEFINED.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLabel? UserDefinedGrade
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedGrade;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedGrade, value, v => UserDefinedGrade = v,
                                           "UserDefinedGrade");
            }
        }

        #region Inverses

        /// <summary>
        ///   Reference to the constraint classifications through objectified relationship
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcConstraintClassificationRelationship> ClassifiedAs
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcConstraintClassificationRelationship>(
                        c => c.ClassifiedConstraint == this);
            }
        }

        /// <summary>
        ///   References to the objectified relationships that relate other constraints with this constraint.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcConstraintRelationship> RelatesConstraints
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcConstraintRelationship>(
                        c => c.RelatingConstraint == this);
            }
        }

        /// <summary>
        ///   References to the objectified relationships that relate this constraint with other constraints.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcConstraintRelationship> IsRelatedWith
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcConstraintRelationship>(
                        c => c.RelatedConstraints.Contains(this));
            }
        }

        /// <summary>
        ///   Reference to the properties to which the constraint is applied.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcPropertyConstraintRelationship> PropertiesForConstraint
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcPropertyConstraintRelationship>(
                        c => c.RelatingConstraint == this);
            }
        }

        /// <summary>
        ///   Reference to the relationships that collect other constraints into this aggregate constraint.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcConstraintAggregationRelationship> Aggregates
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcConstraintAggregationRelationship>(
                        c => c.RelatingConstraint == this);
            }
        }

        /// <summary>
        ///   Reference to the relationships that relate this constraint into aggregate constraints.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcConstraintAggregationRelationship> IsAggregatedIn
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcConstraintAggregationRelationship>(
                        c => c.RelatedConstraints.Contains(this));
            }
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

        public virtual void IfcParse(int propIndex, IPropertyValue value)
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
                    _constraintGrade = (IfcConstraintEnum) Enum.Parse(typeof (IfcConstraintEnum), value.EnumVal);
                    break;
                case 3:
                    _constraintSource = value.StringVal;
                    break;
                case 4:
                    _creatingActor = (IfcActorSelect) value.EntityVal;
                    break;
                case 5:
                    _creationTime = (IfcDateTimeSelect) value.EntityVal;
                    break;
                case 6:
                    _userDefinedGrade = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public virtual string WhereRule()
        {
            if (_constraintGrade == IfcConstraintEnum.USERDEFINED &&
                string.IsNullOrEmpty(UserDefinedGrade.GetValueOrDefault()))
                return
                    "WR11 Constraint : The attribute UserDefinedGrade must be asserted when the value of the IfcConstraintGradeEnum is set to USERDEFINED.\n";
            else
                return "";
        }

        #endregion
    }
}