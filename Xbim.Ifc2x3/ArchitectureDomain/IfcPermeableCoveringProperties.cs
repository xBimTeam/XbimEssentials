using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ArchitectureDomain
{
    /// <summary>
    /// Definition from BS 6100: A permeable covering is a permeable cover for an opening which allows airflow .
    /// 
    /// Definition from IAI: A description of a panel within a door or window (as fillers for opening) which allows for air flow. It is given by its properties (IfcPermeableCoveringProperties). A permeable covering is a casement, i.e. a component, fixed or opening, consisting essentially of a frame and the infilling. The infilling is normally a grill, a louver or a screen. The way of operation is defined in the operation type.
    /// 
    /// The IfcPermeableCoveringProperties are included in the list of properties (HasPropertySets) of the IfcWindowStyle or the IfcDoorStyle. More information about the permeable covering can be included in the same list of the window or door style using the IfcPropertySet for dynamic extensions. This particularly applies for additional properties for the various operation types
    /// </summary>
    [IfcPersistedEntity]
    public class IfcPermeableCoveringProperties : IfcPropertySetDefinition
    {
        private IfcPermeableCoveringOperationEnum _OperationType;

        /// <summary>
        /// Types of permeable covering operations. Also used to assign standard symbolic presentations according to national building standards.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPermeableCoveringOperationEnum OperationType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _OperationType;
            }
            set { this.SetModelValue(this, ref _OperationType, value, v => OperationType = v, "OperationType"); }
        }

        private IfcWindowPanelPositionEnum _PanelPosition;

        /// <summary>
        ///  	Position of this permeable covering panel within the overall window or door type. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcWindowPanelPositionEnum PanelPosition
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PanelPosition;
            }
            set { this.SetModelValue(this, ref _PanelPosition, value, v => PanelPosition = v, "PanelPosition"); }
        }

        private IfcPositiveLengthMeasure? _FrameDepth;

        /// <summary>
        /// Depth of panel frame (used to include the permeable covering), measured from front face to back face horizontally (i.e. perpendicular to the window or door (elevation) plane. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? FrameDepth
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _FrameDepth;
            }
            set { this.SetModelValue(this, ref _FrameDepth, value, v => FrameDepth = v, "FrameDepth"); }
        }

        private IfcPositiveLengthMeasure? _FrameThickness;

        /// <summary>
        /// Width of panel frame (used to include the permeable covering), measured from inside of panel (at permeable covering) to outside of panel (at lining), i.e. parallel to the window or door (elevation) plane. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? FrameThickness
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _FrameThickness;
            }
            set { this.SetModelValue(this, ref _FrameThickness, value, v => FrameThickness = v, "FrameThickness"); }
        }

        private IfcShapeAspect _ShapeAspectStyle;

        /// <summary>
        /// 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcShapeAspect ShapeAspectStyle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ShapeAspectStyle;
            }
            set { this.SetModelValue(this, ref _ShapeAspectStyle, value, v => ShapeAspectStyle = v, "ShapeAspectStyle"); }
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
                    _OperationType = (IfcPermeableCoveringOperationEnum)Enum.Parse(typeof(IfcPermeableCoveringOperationEnum), value.EnumVal);
                    break;
                case 5:
                    _PanelPosition = (IfcWindowPanelPositionEnum)Enum.Parse(typeof(IfcWindowPanelPositionEnum), value.EnumVal);
                    break;
                case 6:
                    _FrameDepth = value.RealVal;
                    break;
                case 7:
                    _FrameThickness = value.RealVal;
                    break;
                case 8:
                    _ShapeAspectStyle = (IfcShapeAspect)value.EntityVal;
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
