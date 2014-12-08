#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindowLiningProperties.cs
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
    /// <summary>
    ///   The window lining is the frame which enables the window to be fixed in position. The window lining is used to hold the window panels or other casements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The window lining is the frame which enables the window to be fixed in position. The window lining is used to hold the window panels or other casements. The parameter of the window lining (IfcWindowLiningProperties) define the geometrically relevant parameter of the lining.
    ///   The IfcWindowLiningProperties are included in the list of properties (HasPropertySets) of the IfcWindowStyle. More information about the window lining can be included in the same list of the IfcWindowStyle using the IfcPropertySet for dynamic extensions. 
    ///   HISTORY New Entity in IFC Release 2.0. Has been renamed from IfcWindowLining in IFC Release 2x. 
    ///   Geometry Use Definitions
    ///   The IfcWindowLiningProperties does not hold an own geometric representation. However it defines parameter, which can be used to create the shape of the window style (which is inserted by the IfcWindow into the spatial context of the project).
    ///   Interpretation of parameter
    ///   The parameters at the IfcWindowLiningProperties define a standard window lining, including (if given) a mullion and a transom (for horizontal and vertical splits). The outer boundary of the lining is determined by the occurrence parameter assigned to the IfcWindow, which inserts the IfcWindowStyle.
    ///   The lining is applied to all faces of the opening reveal. The parameter are: 
    ///   LiningDepth 
    ///   LiningThickness 
    ///  
    ///   If the OperationType of the window style is 
    ///   DoublePanelVertical (shown) 
    ///   TriplePanelBottom 
    ///   TriplePanelTop 
    ///   TriplePanelLeft 
    ///   TriplePanelRight 
    ///   the following additional parameter apply: 
    ///   MullionThickness 
    ///   FirstMullionOffset - measured as offset to the Z axis (in XZ plane) 
    ///  
    ///   If the OperationType of the window style is 
    ///   DoublePanelHorizontal 
    ///   TriplePanelBottom 
    ///   TriplePanelTop 
    ///   TriplePanelLeft 
    ///   TriplePanelRight 
    ///   the following additional parameter apply 
    ///   TransomThickness 
    ///   FirstTransomOffset measured as offset to the X axis (in XZ plane) 
    ///  
    ///   If the OperationType of the window style is 
    ///   TriplePanelVertical 
    ///   the following additional parameter apply 
    ///   SecondMullionOffset 
    ///  
    ///   If the OperationType of the window style is 
    ///   TriplePanelHorizontal 
    ///   the following additional parameter apply 
    ///   SecondTransomOffset 
    ///  
    ///   Formal Propositions:
    ///   WR31   :   Either both parameter, LiningDepth and LiningThickness are given, or only the LiningThickness, then the LiningDepth is variable. It is not valid to only assert the LiningDepth. 
    ///   NOTE  A LiningDepth with NIL ($) value indicates a window style with a lining equal to the wall thickness. 
    ///   WR32   :   Either both parameter, FirstTransomOffset and SecondTransomOffset are given, or only the FirstTransomOffset, or none of both. It is not valid to only assert the SecondTransomOffset.  
    ///   WR33   :   Either both parameter, FirstMullionOffset and SecondMullionOffset are given, or only the FirstMullionOffset, or none of both. It is not valid to only assert the SecondMullionOffset.  
    ///   WR34   :   The IfcWindowLiningProperties shall only be used in the context of an IfcWindowStyle.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcWindowLiningProperties : IfcPropertySetDefinition
    {
        #region Part 21 Step file Parse routines

        private IfcPositiveLengthMeasure? _liningDepth;
        private IfcPositiveLengthMeasure? _liningThickness;
        private IfcPositiveLengthMeasure? _transomThickness;
        private IfcPositiveLengthMeasure? _mullionThickness;
        private IfcNormalisedRatioMeasure? _firstTransomOffset;
        private IfcNormalisedRatioMeasure? _secondTransomOffset;
        private IfcNormalisedRatioMeasure? _firstMullionOffset;
        private IfcNormalisedRatioMeasure? _secondMullionOffset;
        private IfcShapeAspect _shapeAspectStyle;

        /// <summary>
        ///   Optional. Depth of the window lining (dimension measured perpendicular to window elevation plane).
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? LiningDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _liningDepth;
            }
            set { this.SetModelValue(this, ref _liningDepth, value, v => LiningDepth = v, "LiningDepth"); }
        }

        /// <summary>
        ///   Optional. Thickness of the window lining (measured parallel to the window elevation plane).
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? LiningThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _liningThickness;
            }
            set
            {
                this.SetModelValue(this, ref _liningThickness, value, v => LiningThickness = v,
                                           "LiningThickness");
            }
        }

        /// <summary>
        ///   Optional. Thickness of the transom (horizontal separator of window panels within a window), measured parallel to the window elevation plane. The transom is part of the lining and the transom depth is assumed to be identical to the lining depth.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? TransomThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transomThickness;
            }
            set
            {
                this.SetModelValue(this, ref _transomThickness, value, v => TransomThickness = v,
                                           "TransomThickness");
            }
        }

        /// <summary>
        ///   Optional. Thickness of the mullion (vertical separator of window panels within a window), measured parallel to the window elevation plane. The mullion is part of the lining and the mullion depth is assumed to be identical to the lining depth.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MullionThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _mullionThickness;
            }
            set
            {
                this.SetModelValue(this, ref _mullionThickness, value, v => MullionThickness = v,
                                           "MullionThickness");
            }
        }

        /// <summary>
        ///   Optional. Offset of the transom centerline, measured along the z-axis of the window placement co-ordinate system. An offset value = 0.5 indicates that the transom is positioned in the middle of the window.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? FirstTransomOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _firstTransomOffset;
            }
            set
            {
                this.SetModelValue(this, ref _firstTransomOffset, value, v => FirstTransomOffset = v,
                                           "FirstTransomOffset");
            }
        }

        /// <summary>
        ///   Optional. Offset of the transom centerline for the second transom, measured along the x-axis of the window placement co-ordinate system. An offset value = 0.666 indicates that the second transom is positioned at two/third of the window.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? SecondTransomOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _secondTransomOffset;
            }
            set
            {
                this.SetModelValue(this, ref _secondTransomOffset, value, v => SecondTransomOffset = v,
                                           "SecondTransomOffset");
            }
        }

        /// <summary>
        ///   Optional. Offset of the mullion centerline, measured along the x-axis of the window placement co-ordinate system. An offset value = 0.5 indicates that the mullion is positioned in the middle of the window.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? FirstMullionOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _firstMullionOffset;
            }
            set
            {
                this.SetModelValue(this, ref _firstMullionOffset, value, v => FirstMullionOffset = v,
                                           "FirstMullionOffset");
            }
        }

        /// <summary>
        ///   Optional. Offset of the mullion centerline for the second mullion, measured along the x-axis of the window placement co-ordinate system. An offset value = 0.666 indicates that the second mullion is positioned at two/third of the window.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? SecondMullionOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _secondMullionOffset;
            }
            set
            {
                this.SetModelValue(this, ref _secondMullionOffset, value, v => SecondMullionOffset = v,
                                           "SecondMullionOffset");
            }
        }

        /// <summary>
        ///   Optional. Optional link to a shape aspect definition, which points to the part of the geometric representation of the window style, which is used to represent the lining.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Optional)]
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
                    _liningDepth = value.RealVal;
                    break;
                case 5:
                    _liningThickness = value.RealVal;
                    break;
                case 6:
                    _transomThickness = value.RealVal;
                    break;
                case 7:
                    _mullionThickness = value.RealVal;
                    break;
                case 8:
                    _firstTransomOffset = value.RealVal;
                    break;
                case 9:
                    _secondTransomOffset = value.RealVal;
                    break;
                case 10:
                    _firstMullionOffset = value.RealVal;
                    break;
                case 11:
                    _secondMullionOffset = value.RealVal;
                    break;
                case 12:
                    _shapeAspectStyle = (IfcShapeAspect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string err = "";
            if (_liningDepth.HasValue && !_liningThickness.HasValue)
                err +=
                    "WR31 WindowLiningProperties : Either both parameter, LiningDepth and LiningThickness are given, or only the LiningThickness, then the LiningDepth is variable. It is not valid to only assert the LiningDepth.\n";
            if (_secondTransomOffset.HasValue && !_firstTransomOffset.HasValue)
                err +=
                    "WR32 WindowLiningProperties : Either both parameter, FirstTransomOffset and SecondTransomOffset are given, or only the FirstTransomOffset, or none of both. It is not valid to only assert the SecondTransomOffset.\n";
            if (_secondMullionOffset.HasValue && !_firstMullionOffset.HasValue)
                err +=
                    "WR33 WindowLiningProperties : Either both parameter, FirstMullionOffset and SecondMullionOffset are given, or only the FirstMullionOffset, or none of both. It is not valid to only assert the SecondMullionOffset.\n";
            IfcTypeObject defines = DefinesType.FirstOrDefault();
            if (defines != null && !(defines is IfcWindowStyle))
                err +=
                    "WR34 WindowLiningProperties : The WindowLiningProperties shall only be used in the context of an WindowStyle.\n";
            return err;
        }
    }
}