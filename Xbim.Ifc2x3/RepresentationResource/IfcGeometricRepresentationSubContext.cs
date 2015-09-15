#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricRepresentationSubContext.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcGeometricRepresentationSubContext defines the context that applies to several shape representations of a product being a sub context, sharing the WorldCoordinateSystem, CoordinateSpaceDimension, Precision and TrueNorth attributes with the parent IfcGeometricRepresentationContext.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcGeometricRepresentationSubContext defines the context that applies to several shape representations of a product being a sub context, sharing the WorldCoordinateSystem, CoordinateSpaceDimension, Precision and TrueNorth attributes with the parent IfcGeometricRepresentationContext.
    ///   The IfcGeometricRepresentationSubContext is used to define semantically distinguished representation types for different information content, dependent on the representation view and the target scale. It can be used to control the level of detail of the shape representation that is most applicable to this geometric representation context. addition the sub context is used to control the later appearance of the IfcShapeRepresentation within a plot view. 
    ///   NOTE  If the IfcShapeRepresentation using this sub context has IfcStyledItem's assigned to the Items, the presentation style information (e.g. IfcCurveStyle, IfcTextStyle) associated with the IfcStyledItem is given in target plot dimensions. E.g. a line thickness (IfcCurveStyle.CurveWidth) is given by a thickness measure relating to the thickness for a plot within the (range of) target scale.
    ///   Each IfcProduct can then have several instances of subtypes of IfcRepresentation, each being assigned to a different geometric representation context (IfcGeometricRepresentationContext or IfcGeometricRepresentationSubContext). The application can then choose the most appropriate representation for showing the geometric shape of the product, depending on the target view and scale.
    ///   NOTE The provision of a model view (IfcGeometricRepresentationContext.ContextType = 'Model') is mandatory. Instances of IfcGeometricRepresentationSubContext relate to it as its ParentContext.
    ///   EXAMPLE  Instances of IfcGeometricRepresentationSubContext can be used to handle the multi-view blocks or macros, which are used in CAD programs to store several scale and/or view dependent geometric representations of the same object. 
    ///   HISTORY New entity in Release IFC 2x Edition 2.
    ///   Formal Propositions:
    ///   WR31   :   The parent context shall not be another geometric representation sub context.  
    ///   WR32   :   The attribute UserDefinedTargetView shall be given, if the attribute TargetView is set to USERDEFINED.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcGeometricRepresentationSubContext : IfcGeometricRepresentationContext
    {
        #region Fields

        private IfcGeometricRepresentationContext _parentContext;
        private IfcPositiveRatioMeasure? _targetScale;
        private IfcGeometricProjectionEnum _targetView = IfcGeometricProjectionEnum.NOTDEFINED;
        private IfcLabel? _userDefinedTargetView;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Override, returns parentContext
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.DerivedOverride)]
        public override IfcDimensionCount CoordinateSpaceDimension
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _parentContext.CoordinateSpaceDimension;
            }
            set
            {
                throw new ArgumentException(
                    "Cannot change CoordinateSpaceDimension for a subcontext, change parent context");
            }
        }

        /// <summary>
        ///   Override, returns parentContext
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.DerivedOverride)]
        public override IfcReal? Precision
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                if (_parentContext.Precision.HasValue)
                    return _parentContext.Precision;
                else
                    return 0.00001; //1.E-5
            }
            set { throw new ArgumentException("Cannot change Precision for a subcontext, change parent context"); }
        }

        /// <summary>
        ///   Override, returns parentContext
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.DerivedOverride)]
        public override IfcAxis2Placement WorldCoordinateSystem
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _parentContext.WorldCoordinateSystem;
            }
            set
            {
                throw new ArgumentException(
                    "Cannot change WorldCoordinateSystem for a subcontext, change parent context");
            }
        }


        /// <summary>
        ///   Override, returns parentContext
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.DerivedOverride)]
        public override IfcDirection TrueNorth
        {
            get
            {
                if (ParentContext.TrueNorth != null)
                    return ParentContext.TrueNorth;
                else
                {
                    IfcAxis2Placement2D ax2 = ParentContext.WorldCoordinateSystem as IfcAxis2Placement2D;
                    IfcAxis2Placement3D ax3 = ParentContext.WorldCoordinateSystem as IfcAxis2Placement3D;
                    if (ax2 != null)
                        return new IfcDirection(ax2.P[1]);
                    else if (ax3 != null)
                        return new IfcDirection(ax3.P[1]);
                    else return null;
                }
            }
            set { throw new ArgumentException("Cannot change TrueNorth for a subcontext, change parent context"); }
        }


        /// <summary>
        ///   SpatialStructuralElementParent context from which the sub context derives its world coordinate system, precision, space coordinate dimension and true north.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcGeometricRepresentationContext ParentContext
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _parentContext;
            }
            set { this.SetModelValue(this, ref _parentContext, value, v => ParentContext = v, "ParentContext"); }
        }


        /// <summary>
        ///   Optional. The target plot scale of the representation to which this representation context applies.
        /// </summary>
        /// <remarks>
        ///   Scale indicates the target plot scale for the representation sub context, all annotation styles are given in plot dimensions according to this target plot scale.
        ///   If multiple instances of IfcGeometricRepresentationSubContext are given having the same TargetView value, the target plot scale applies up to the next smaller scale, or up to unlimited small scale. 
        ///   Note: Scale 1:100 (given as 0.01 within TargetScale) is bigger then 1:200 (given as 0.005 within TargetScale).
        /// </remarks>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? TargetScale
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _targetScale;
            }
            set { this.SetModelValue(this, ref _targetScale, value, v => TargetScale = v, "TargetScale"); }
        }


        /// <summary>
        ///   Target view of the representation to which this representation context applies.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcGeometricProjectionEnum TargetView
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _targetView;
            }
            set
            {
                this.SetModelValue(this, ref _targetView, value, v => TargetView = v, "TargetView");
                if (value != IfcGeometricProjectionEnum.USERDEFINED) UserDefinedTargetView = null;
            }
        }


        /// <summary>
        ///   Optional. User defined target view, this attribute value shall be given, if the TargetView attribute is set to USERDEFINED.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcLabel? UserDefinedTargetView
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _userDefinedTargetView;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedTargetView, value, v => UserDefinedTargetView = v,
                                           "UserDefinedTargetView");
                if (value != null) TargetView = IfcGeometricProjectionEnum.USERDEFINED;
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
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    _parentContext = (IfcGeometricRepresentationContext)value.EntityVal;
                    break;
                case 7:
                    _targetScale = value.RealVal;
                    break;
                case 8:
                    _targetView =
                        (IfcGeometricProjectionEnum)Enum.Parse(typeof(IfcGeometricProjectionEnum), value.EnumVal, true);
                    break;
                case 9:
                    _userDefinedTargetView = value.StringVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (ParentContext is IfcGeometricRepresentationSubContext)
                baseErr +=
                    "WR31 GeometricRepresentationSubContext: The parent context shall not be another geometric representation sub context\n";
            if (TargetView == IfcGeometricProjectionEnum.USERDEFINED && !UserDefinedTargetView.HasValue)
                baseErr +=
                    "WR32 GeometricRepresentationSubContext: User defined target view, this attribute value shall be given, if the TargetView attribute is set to USERDEFINED\n";
            return baseErr;
        }

        #endregion
    }
}