#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundaryFaceCondition.cs
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
    public class IfcBoundaryFaceCondition : IfcBoundaryCondition
    {
        #region Fields

        private IfcModulusOfSubgradeReactionMeasure? _linearStiffnessByAreaX;
        private IfcModulusOfSubgradeReactionMeasure? _linearStiffnessByAreaY;
        private IfcModulusOfSubgradeReactionMeasure? _linearStiffnessByAreaZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Linear stiffness value in x-direction of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcModulusOfSubgradeReactionMeasure? LinearStiffnessByAreaX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearStiffnessByAreaX;
            }
            set
            {
                this.SetModelValue(this, ref _linearStiffnessByAreaX, value, v => LinearStiffnessByAreaX = v,
                                           "LinearStiffnessByAreaX");
            }
        }

        /// <summary>
        ///   Linear stiffness value in y-direction of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcModulusOfSubgradeReactionMeasure? LinearStiffnessByAreaY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearStiffnessByAreaY;
            }
            set
            {
                this.SetModelValue(this, ref _linearStiffnessByAreaY, value, v => LinearStiffnessByAreaY = v,
                                           "LinearStiffnessByAreaY");
            }
        }

        /// <summary>
        ///   Linear stiffness value in z-direction of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcModulusOfSubgradeReactionMeasure? LinearStiffnessByAreaZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearStiffnessByAreaZ;
            }
            set
            {
                this.SetModelValue(this, ref _linearStiffnessByAreaZ, value, v => LinearStiffnessByAreaZ = v,
                                           "LinearStiffnessByAreaZ");
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _linearStiffnessByAreaX = value.RealVal;
                    break;
                case 2:
                    _linearStiffnessByAreaY = value.RealVal;
                    break;
                case 3:
                    _linearStiffnessByAreaZ = value.RealVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}