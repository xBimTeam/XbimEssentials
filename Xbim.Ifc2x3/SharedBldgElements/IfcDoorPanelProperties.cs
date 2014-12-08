#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDoorPanelProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    [IfcPersistedEntityAttribute]
    public class IfcDoorPanelProperties : IfcPropertySetDefinition
    {
        #region Part 21 Step file Parse routines

        private IfcPositiveLengthMeasure? _panelDepth;
        private IfcDoorPanelOperationEnum _panelOperation = IfcDoorPanelOperationEnum.NOTDEFINED;
        private IfcNormalisedRatioMeasure? _panelWidth;
        private IfcDoorPanelPositionEnum _panelPosition = IfcDoorPanelPositionEnum.NOTDEFINED;
        private IfcShapeAspect _shapeAspectStyle;

        /// <summary>
        ///   Optional. Depth of the door panel, measured perpendicular to the plane of the door leaf.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? PanelDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _panelDepth;
            }
            set { this.SetModelValue(this, ref _panelDepth, value, v => PanelDepth = v, "PanelDepth"); }
        }

        /// <summary>
        ///   The PanelOperation defines the way of operation of that panel. The PanelOperation of the door panel has to correspond with the OperationType of the IfcDoorStyle.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcDoorPanelOperationEnum PanelOperation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _panelOperation;
            }
            set { this.SetModelValue(this, ref _panelOperation, value, v => PanelOperation = v, "PanelOperation"); }
        }

        /// <summary>
        ///   Optional. Width of this panel, given as ratio relative to the total clear opening width of the door.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? PanelWidth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _panelWidth;
            }
            set { this.SetModelValue(this, ref _panelWidth, value, v => PanelWidth = v, "PanelWidth"); }
        }

        /// <summary>
        ///   Position of this panel within the door.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcDoorPanelPositionEnum PanelPosition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _panelPosition;
            }
            set { this.SetModelValue(this, ref _panelPosition, value, v => PanelPosition = v, "PanelPosition"); }
        }

        /// <summary>
        ///   Optional. Pointer to the shape aspect, if given. The shape aspect reflects the part of the door shape, which represents the door panel.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcShapeAspect ShapeAspectStyle
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _shapeAspectStyle;
            }
            set
            {
                this.SetModelValue(this, ref _shapeAspectStyle, value, v => ShapeAspectStyle = v,
                                           "ShapeAspectStyle");
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
                    _panelDepth = value.RealVal;
                    break;
                case 5:
                    _panelOperation =
                        (IfcDoorPanelOperationEnum) Enum.Parse(typeof (IfcDoorPanelOperationEnum), value.EnumVal, true);
                    break;
                case 6:
                    _panelWidth = value.RealVal;
                    break;
                case 7:
                    _panelPosition =
                        (IfcDoorPanelPositionEnum) Enum.Parse(typeof (IfcDoorPanelPositionEnum), value.EnumVal, true);
                    break;
                case 8:
                    _shapeAspectStyle = (IfcShapeAspect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            IfcTypeObject defType = DefinesType.FirstOrDefault();
            if (defType != null && !(defType is IfcDoorStyle))
                return
                    "WR31 DoorPanelProperties: The DoorPanelProperties shall only be used in the context of an IfcDoorStyle. ";
            return "";
        }

        #endregion
    }
}