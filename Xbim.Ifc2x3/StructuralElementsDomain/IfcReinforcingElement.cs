#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReinforcingElement.cs
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

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    /// <summary>
    ///   Bars, wires, strands, and other slender members embedded in concrete in such a manner that the reinforcement and the concrete act together in resisting forces.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcReinforcingElement : IfcBuildingElementComponent
    {
        #region Fields

        private IfcLabel? _steelGrade;

        #endregion

        #region Properties

        /// <summary>
        ///   The nominal steel grade defined according to local standards.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcLabel? SteelGrade
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _steelGrade;
            }
            set { this.SetModelValue(this, ref _steelGrade, value, v => SteelGrade = v, "SteelGrade"); }
        }

        #endregion

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
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _steelGrade = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}