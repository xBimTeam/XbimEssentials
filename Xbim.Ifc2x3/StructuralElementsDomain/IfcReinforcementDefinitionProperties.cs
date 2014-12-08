#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReinforcementDefinitionProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcReinforcementDefinitionProperties : IfcPropertySetDefinition
    {
        public IfcReinforcementDefinitionProperties()
        {
            _reinforcementSectionDefinitions = new XbimList<IfcSectionReinforcementProperties>(this);
        }

        #region Fields

        private IfcLabel? _definitionType;
        private XbimList<IfcSectionReinforcementProperties> _reinforcementSectionDefinitions;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Descriptive type name applied to reinforcement definition properties.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLabel? DefinitionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _definitionType;
            }
            set { this.SetModelValue(this, ref _definitionType, value, v => DefinitionType = v, "DefinitionType"); }
        }

        /// <summary>
        ///   The list of section reinforcement properties attached to the reinforcement definition properties.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory, IfcAttributeType.List, 1)]
        public XbimList<IfcSectionReinforcementProperties> ReinforcementSectionDefinitions
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _reinforcementSectionDefinitions;
            }
            set
            {
                this.SetModelValue(this, ref _reinforcementSectionDefinitions, value,
                                           v => ReinforcementSectionDefinitions = v, "ReinforcementSectionDefinitions");
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
                    _definitionType = value.StringVal;
                    break;
                case 5:
                    _reinforcementSectionDefinitions.Add((IfcSectionReinforcementProperties) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}