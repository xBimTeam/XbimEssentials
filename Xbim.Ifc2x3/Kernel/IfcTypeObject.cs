#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTypeObject.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public class IfcTypeObject : IfcObjectDefinition
    {
        #region Fields and Events

        private IfcLabel _applicableOccurrence;
        private PropertySetDefinitionSet _hasPropertySets;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. The attribute optionally defines the data type of the occurrence object, to which the assigned type object can relate. If not present, no instruction is given to which occurrence object the type object is applicable.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLabel ApplicableOccurrence
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _applicableOccurrence;
            }
            set
            {
                this.SetModelValue(this, ref _applicableOccurrence, value, v => ApplicableOccurrence = v,
                                           "ApplicableOccurrence");
            }
        }

        /// <summary>
        ///   Optional. Set list of unique property sets, that are associated with the object type and are common to all object occurrences referring to this object type.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional, IfcAttributeType.Set, 1)]
        public PropertySetDefinitionSet HasPropertySets
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hasPropertySets;
            }
            set
            {
                this.SetModelValue(this, ref _hasPropertySets, value, v => HasPropertySets = v,
                                           "HasPropertySets");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _applicableOccurrence = value.StringVal;
                    break;
                case 5:
                    if (_hasPropertySets == null)
                        _hasPropertySets = new PropertySetDefinitionSet(this);
                    _hasPropertySets.Add((IfcPropertySetDefinition)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public void CreateHasPropertySets()
        {
            if (_hasPropertySets == null) _hasPropertySets = new PropertySetDefinitionSet(this);
        }

        #region Inverse Relationships

        /// <summary>
        ///   Reference to the relationship IfcRelDefinedByType and thus to those occurrence objects, which are defined by this type.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelDefinesByType> ObjectTypeOf
        {
            get { return ModelOf.Instances.Where<IfcRelDefinesByType>(rt => rt.RelatingType == this); }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (!Name.HasValue)
                return "WR1 TypeObject: A Name attribute has to be provided.\n";
            return "";
        }

        #endregion

        #region optional sets and lists methods

        public void AddPropertySet(IfcPropertySetDefinition pSetDefinition)
        {
            if (this.HasPropertySets == null) this.HasPropertySets = new PropertySetDefinitionSet(this);
            this.HasPropertySets.Add(pSetDefinition);
        }

        #endregion
    }
}