#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcContextDependentUnit.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    /// <summary>
    ///   A context dependent unit is a unit which is not related to the SI system. 
    ///   NOTE The number of parts in an assembly is a physical quantity measured in units that may be called "parts" but which cannot be related to an SI unit. 
    ///   NOTE Corresponding STEP name: context_dependent_unit, please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcContextDependentUnit : IfcNamedUnit
    {
        #region Fields

        private IfcLabel _name;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The word, or group of words, by which the context dependent unit is referred to.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _name = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}