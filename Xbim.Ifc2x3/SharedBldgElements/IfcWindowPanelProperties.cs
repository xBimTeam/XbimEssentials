#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindowPanelProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   A description of the window panel. A window panel is a casement, i.e. a component, fixed or opening, consisting essentially of a frame and the infilling.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A description of the window panel. A window panel is a casement, i.e. a component, fixed or opening, consisting essentially of a frame and the infilling. The infilling of a window panel is normally glazing. The way of operation is defined in the operation type. 
    ///   The IfcWindowPanelProperties are included in the list of properties (HasPropertySets) of the IfcWindowStyle. More information about the window panel can be included in the same list of the IfcWindowStyle using the IfcPropertySet for dynamic extensions.
    ///   HISTORY New entity in IFC Release 2.0, it had been renamed from IfcWindowPanel in IFC Release 2x.
    ///   Geometry Use Definitions
    ///   The IfcWindowPanelProperties does not hold an own geometric representation. However it defines parameter, which can be used to create the shape of the IfcWindowStyle (which is inserted by the IfcWindow into the spatial context of the project).
    ///   Interpretation of parameter
    ///   The parameters at the IfcWindowPanelProperties define a standard window panel. The outer boundary of the panel is determined by the occurrence parameter assigned to the IfcWindow, which inserts the IfcWindowStyle. It has to take the lining parameter into account as well. The position of the window panel within the overall window is determined by the PanelPosition attribute.
    ///   The panel is applied to the position within the lining, as defined by the panel position attribute. The following parameter apply to that panel: 
    ///   FrameDepth 
    ///   FrameThickness
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcWindowPanelProperties : IfcPropertySetDefinition
    {
        #region Part 21 Step file Parse routines

        private IfcWindowPanelOperationEnum _operationType;
        private IfcWindowPanelPositionEnum _panelPosition;
        private IfcPositiveLengthMeasure? _frameDepth;
        private IfcPositiveLengthMeasure? _frameThickness;
        private IfcShapeAspect _shapeAspectStyle;

        /// <summary>
        ///   Types of window panel operations. Also used to assign standard symbolic presentations according to national building standards.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcWindowPanelOperationEnum OperationType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _operationType;
            }
            set { this.SetModelValue(this, ref _operationType, value, v => OperationType = v, "OperationType"); }
        }

        /// <summary>
        ///   Position of this panel within the overall window style.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcWindowPanelPositionEnum PanelPosition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _panelPosition;
            }
            set { this.SetModelValue(this, ref _panelPosition, value, v => PanelPosition = v, "PanelPosition"); }
        }

        /// <summary>
        ///   Optional. Depth of panel frame, measured from front face to back face horizontally (i.e. perpendicular to the window (elevation) plane.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? FrameDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _frameDepth;
            }
            set { this.SetModelValue(this, ref _frameDepth, value, v => FrameDepth = v, "FrameDepth"); }
        }

        /// <summary>
        ///   Optional. Width of panel frame, measured from inside of panel (at glazing) to outside of panel (at lining), i.e. parallel to the window (elevation) plane.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? FrameThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _frameThickness;
            }
            set { this.SetModelValue(this, ref _frameThickness, value, v => FrameThickness = v, "FrameThickness"); }
        }

        /// <summary>
        ///   Optional. Optional link to a shape aspect definition, which points to the part of the geometric representation of the window style, which is used to represent the panel.
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
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _operationType =
                        (IfcWindowPanelOperationEnum)
                        Enum.Parse(typeof (IfcWindowPanelOperationEnum), value.EnumVal, true);
                    break;
                case 5:
                    _panelPosition =
                        (IfcWindowPanelPositionEnum)
                        Enum.Parse(typeof (IfcWindowPanelPositionEnum), value.EnumVal, true);
                    break;
                case 6:
                    _frameDepth = value.RealVal;
                    break;
                case 7:
                    _frameThickness = value.RealVal;
                    break;
                case 8:
                    _shapeAspectStyle = (IfcShapeAspect) value.EntityVal;
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