#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralPlanarAction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.StructuralLoadResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    public class IfcStructuralPlanarAction : IfcStructuralAction
    {
        private IfcProjectedOrTrueLengthEnum _projectedOrTrue;

        /// <summary>
        ///   Defines if the load values are given by using the length of the member on which they act (true length) or by using the projected length resulting from 
        ///   the loaded member and the global project coordinate system. It is only considered if the global project coordinate system is used, and if 
        ///   the action is of type IfcStructuralLinearAction or IfcStructuralPlanarAction.
        /// </summary>
        [Ifc(12, IfcAttributeState.Mandatory)]
        public IfcProjectedOrTrueLengthEnum ProjectedOrTrue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _projectedOrTrue;
            }
            set
            {
                this.SetModelValue(this, ref _projectedOrTrue, value, v => ProjectedOrTrue = v,
                                           "ProjectedOrTrue");
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
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    base.IfcParse(propIndex, value);
                    break;
                case 11:
                    _projectedOrTrue =
                        (IfcProjectedOrTrueLengthEnum)
                        Enum.Parse(typeof (IfcProjectedOrTrueLengthEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!(AppliedLoad is IfcStructuralLoadPlanarForce || AppliedLoad is IfcStructuralLoadTemperature))
                baseErr +=
                    "WR61 StructuralLinearAction : A linear action should place either a planar force or a temperature force.\n";
            return baseErr;
        }
    }
}