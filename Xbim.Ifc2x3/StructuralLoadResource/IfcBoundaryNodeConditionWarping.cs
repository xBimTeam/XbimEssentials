#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundaryNodeConditionWarping.cs
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

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    [IfcPersistedEntityAttribute]
    public class IfcBoundaryNodeConditionWarping : IfcBoundaryNodeCondition
    {
        #region Fields

        private IfcWarpingMomentMeasure? _warpingStiffness;

        #endregion

        #region Properties

        /// <summary>
        ///   The warping moment at the point load.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcWarpingMomentMeasure? WarpingStiffness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _warpingStiffness;
            }
            set
            {
                this.SetModelValue(this, ref _warpingStiffness, value, v => WarpingStiffness = v,
                                           "WarpingStiffness");
            }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _warpingStiffness = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}