#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDoorLiningProperties.cs
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
    ///   The door lining is the frame which enables the door leaf to be fixed in position. The door lining is used to hang the door leaf.
    /// </summary>
    /// <remarks>
    ///   Definition of IAI: The door lining is the frame which enables the door leaf to be fixed in position. The door lining is used to hang the door leaf. The parameters of the door lining (IfcDoorLiningProperties) define the geometrically relevant parameter of the lining.
    ///   The IfcDoorLiningProperties are included in the list of properties (HasPropertySets) of the IfcDoorStyle. More information about the door lining can be included in the same list of the IfcDoorStyle using the IfcPropertySet for dynamic extensions. 
    ///   HISTORY: New entity in IFC Release 2.0
    ///   Geometry Use Definitions
    ///   The IfcDoorLiningProperties does not hold its own geometric representation. However it defines parameters which can be used to create the shape of the door style (which is inserted by the IfcDoor into the spatial context of the project).
    ///   Interpretation of parameter
    ///   The parameters of the IfcDoorLiningProperties define a standard door lining, including (if given) a threshold and a transom. The outer boundary of the lining is determined by the occurrence parameter assigned to the IfcDoor, which inserts the IfcDoorStyle.
    ///   The lining is applied to the left, right and upper side of the opening reveal. The parameters are: 
    ///   LiningDepth, if omited, equal to wall thickness - this only takes effect if a value for LiningThickness is given. If both parameters are not given, then there is no lining. 
    ///   LiningThickness 
    ///  
    ///   The lining can only cover part of the opening reveal. 
    ///   LiningOffset : given if lining edge has an offset to the x axis of the local placement. 
    ///   NOTE  In addition to the LiningOffset, the local placement of the IfcDoor can already have an offset to the wall edge and thereby shift the lining along the y axis. The actual position of the lining is calculated from the origin of the local placement along the positive y axis with the distance given by LiningOffset. 
    ///   The lining may include a casing, which covers part of the wall faces around the opening. The casing covers the left, right and upper side of the lining on both sides of the wall. The parameters are: 
    ///   CasingDepth 
    ///   CasingThickness 
    ///  
    ///   The lining may include a threshold, which covers the bottom side of the opening. The parameters are: 
    ///   ThresholdDepth, if omited, equal to wall thickness - this only takes effect if a value for ThresholdThickness is given. If both parameters are not given, then there is no threshold. 
    ///   ThresholdThickness 
    ///   ThresholdOffset (not shown in figure): given, if the threshold edge has an offset to the x axis of the local placement. 
    ///  
    ///   The lining may have a transom which separates the door panel from a window panel. The transom, if given, is defined by: 
    ///   TransomOffset : a parallel edge to the x axis of the local placement 
    ///   TransomThickness 
    ///   The depth of the transom is identical to the depth of the lining and not given as separate parameter.
    ///  
    ///   NOTE: special agreement on LiningDepth 
    ///   It describes the length of the lining along the reveal of the door opening. It can be given by an absolute value, if the door lining has a specific depth depending on the door style. However often it is equal to the wall thickness. If the same door style is used (like the same type of single swing door), but inserted into different walls with different thicknesses, it would be necessary to create a special door style for each wall thickness. Therefore several CAD systems allow to set the value to "automatically aligned" to wall thickness. This should be exchanged by leaving the optional attribute LiningDepth unassigned.
    ///   NOTE: the same agreement applies to ThresholdDepth
    ///   Formal Propositions:
    ///   WR31   :   Either both parameter, LiningDepth and LiningThickness are given, or only the LiningThickness, then the LiningDepth is variable. It is not valid to only assert the LiningDepth. 
    ///   NOTE  A LiningDepth with NIL ($) value indicates a door style with a lining equal to the wall thickness. 
    ///  
    ///   WR32   :   Either both parameter, ThresholdDepth and ThresholdThickness are given, or only the ThresholdThickness, then the ThresholdDepth is variable. It is not valid to only assert the ThresholdDepth. 
    ///   NOTE  A ThresholdDepth with NIL ($) value indicates a door style with a lining equal to the wall thickness. 
    ///  
    ///   WR33   :   Either both parameter, TransomDepth and TransomThickness are given, or none of them. 
    ///  
    ///   WR34   :   Either both parameter, the CasingDepth and the CasingThickness, are given, or none of them. 
    ///  
    ///   WR35   :   The IfcDoorLiningProperties shall only be used in the context of an IfcDoorStyle.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcDoorLiningProperties : IfcPropertySetDefinition
    {
        #region Part 21 Step file Parse routines

        private IfcPositiveLengthMeasure? _liningDepth;
        private IfcPositiveLengthMeasure? _liningThickness;
        private IfcPositiveLengthMeasure? _thresholdDepth;
        private IfcPositiveLengthMeasure? _thresholdThickness;
        private IfcPositiveLengthMeasure? _transomThickness;
        private IfcLengthMeasure? _transomOffset;
        private IfcLengthMeasure? _liningOffset;
        private IfcLengthMeasure? _thresholdOffset;
        private IfcPositiveLengthMeasure? _casingThickness;
        private IfcPositiveLengthMeasure? _casingDepth;
        private IfcShapeAspect _shapeAspectStyle;

        /// <summary>
        ///   Optional. Depth of the door lining, measured perpendicular to the plane of the door lining. If omited (and with a given value to lining thickness) it indicates an adjustable depth (i.e. a depth that adjusts to the thickness of the wall into which the occurrence of this door style is inserted).
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
        ///   Optional. Thickness (width in plane parallel to door leaf) of the door lining.
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
        ///   Optional. Depth (dimension in plane perpendicular to door leaf) of the door threshold. Only given if the door lining includes a threshold. If omited (and with a given value to threshold thickness) it indicates an adjustable depth (i.e. a depth that adjusts to the thickness of the wall into which the occurrence of this door style is inserted).
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? ThresholdDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _thresholdDepth;
            }
            set { this.SetModelValue(this, ref _thresholdDepth, value, v => ThresholdDepth = v, "ThresholdDepth"); }
        }

        /// <summary>
        ///   Optional. Thickness (width in plane parallel to door leaf) of the door threshold. Only given if the door lining includes a threshold and the parameter is known.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? ThresholdThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _thresholdThickness;
            }
            set
            {
                this.SetModelValue(this, ref _thresholdThickness, value, v => ThresholdThickness = v,
                                           "ThresholdThickness");
            }
        }

        /// <summary>
        ///   Optional. Offset of the transom (if given) which divides the door leaf from a glazing (or window) above. The offset is given from the bottom of the door opening.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
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
        ///   Optional. Offset of the transom (if given) which divides the door leaf from a glazing (or window) above. The offset is given from the bottom of the door opening.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcLengthMeasure? TransomOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transomOffset;
            }
            set { this.SetModelValue(this, ref _transomOffset, value, v => TransomOffset = v, "TransomOffset"); }
        }

        /// <summary>
        ///   Optional. Offset (dimension in plane perpendicular to door leaf) of the door lining. The offset is given as distance to the x axis of the local placement.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcLengthMeasure? LiningOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _liningOffset;
            }
            set { this.SetModelValue(this, ref _liningOffset, value, v => LiningOffset = v, "LiningOffset"); }
        }

        /// <summary>
        ///   Optional. Offset (dimension in plane perpendicular to door leaf) of the door threshold. The offset is given as distance to the x axis of the local placement. Only given if the door lining includes a threshold and the parameter is known.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcLengthMeasure? ThresholdOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _thresholdOffset;
            }
            set
            {
                this.SetModelValue(this, ref _thresholdOffset, value, v => ThresholdOffset = v,
                                           "ThresholdOffset");
            }
        }

        /// <summary>
        ///   Optional. Thickness of the casing (dimension in plane of the door leaf). If given it is applied equally to all four sides of the adjacent wall.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? CasingThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _casingThickness;
            }
            set
            {
                this.SetModelValue(this, ref _casingThickness, value, v => CasingThickness = v,
                                           "CasingThickness");
            }
        }

        /// <summary>
        ///   Optional. Depth of the casing (dimension in plane perpendicular to door leaf). If given it is applied equally to all four sides of the adjacent wall.
        /// </summary>
        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? CasingDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _casingDepth;
            }
            set { this.SetModelValue(this, ref _casingDepth, value, v => CasingDepth = v, "CasingDepth"); }
        }

        /// <summary>
        ///   Optional. Pointer to the shape aspect, if given. The shape aspect reflects the part of the door shape, which represents the door lining.
        /// </summary>
        [IfcAttribute(15, IfcAttributeState.Optional)]
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
                    _thresholdDepth = value.RealVal;
                    break;
                case 7:
                    _thresholdThickness = value.RealVal;
                    break;
                case 8:
                    _transomThickness = value.RealVal;
                    break;
                case 9:
                    _transomOffset = value.RealVal;
                    break;
                case 10:
                    _liningOffset = value.RealVal;
                    break;
                case 11:
                    _thresholdOffset = value.RealVal;
                    break;
                case 12:
                    _casingThickness = value.RealVal;
                    break;
                case 13:
                    _casingDepth = value.RealVal;
                    break;
                case 14:
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
            string err = "";
            if (_liningDepth.HasValue && !_liningThickness.HasValue)
                err +=
                    "WR31 DoorLiningProperties: Either both parameter, LiningDepth and LiningThickness are given, or only the LiningThickness, then the LiningDepth is variable. It is not valid to only assert the LiningDepth.\n";
            if (_thresholdDepth.HasValue && !_thresholdThickness.HasValue)
                err +=
                    "WR32 DoorLiningProperties: Either both parameter, ThresholdDepth and ThresholdThickness are given, or only the ThresholdThickness, then the ThresholdDepth is variable. It is not valid to only assert the ThresholdDepth\n";
            if (_transomOffset.HasValue ^ _transomThickness.HasValue)
                err +=
                    "WR33 DoorLiningProperties: Either both parameter, TransomOffset and TransomThickness are given, or neither of them.\n";
            if (_casingDepth.HasValue ^ _casingThickness.HasValue)
                err +=
                    "WR34 DoorLiningProperties: Either both parameter, the CasingDepth and the CasingThickness, are given, or neither of them.\n";
            IfcTypeObject defType = DefinesType.FirstOrDefault();
            if (defType != null && !(defType is IfcDoorStyle))
                err +=
                    "WR35 DoorLiningProperties: The IfcDoorLiningProperties shall only be used in the context of an IfcDoorStyle.\n";
            return err;
        }

        #endregion
    }
}