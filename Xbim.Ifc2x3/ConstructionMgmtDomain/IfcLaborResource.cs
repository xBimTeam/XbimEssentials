#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLaborResource.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcLaborResource : IfcConstructionResource
    {
        private IfcText? _skillSet;

        /// <summary>
        ///   The skill set required for this type of labor.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcText? SkillSet
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _skillSet;
            }
            set { this.SetModelValue(this, ref _skillSet, value, v => SkillSet = v, "SkillSet"); }
        }

        #region Part 21 Step file Parse routines

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _skillSet = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}