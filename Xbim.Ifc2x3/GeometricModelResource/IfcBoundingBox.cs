#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundingBox.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A box domain is an orthogonal box parallel to the axes of the geometric coordinate system which may be used to limit the domain of a half space solid.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A box domain is an orthogonal box parallel to the axes of the geometric coordinate system which may be used to limit the domain of a half space solid. A box domain is specified by the coordinates of the bottom left corner, and the lengths of the sides measured in the directions of the coordinate axes.
    ///   Definition from IAI: Every semantic object having a physical extent might have a minimum default representation of a bounding box. The bounding box is therefore also used as minimal geometric representation for any geometrically represented object. Therefore the IfcBoundingBox is subtyped from IfcGeometricRepresentationItem. 
    ///   NOTE: Corresponding STEP entity : box_domain, please refer to ISO/IS 10303-42:1994, p. 186 for the final definition of the formal standard. In IFC the bounding box can also be used outside of the context of an IfcBoxedHalfSpace.
    ///   HISTORY: New entity in IFC Release 1.0 .
    ///   Illustration: 
    ///   The IfcBoundingBox is defined with an own location which can be used to place the IfcBoundingBox relative to the geometric coordinate system. The IfcBoundingBox is defined by the lower left corner (Corner) and the upper right corner (XDim, YDim, ZDim measured within the parent co-ordinate system). 
    ///   EXPRESS specification:
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBoundingBox : IfcGeometricRepresentationItem
    {
        #region Fields

        private IfcCartesianPoint _corner;
        private IfcPositiveLengthMeasure _xDim;
        private IfcPositiveLengthMeasure _yDim;
        private IfcPositiveLengthMeasure _zDim;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Location of the bottom left corner (having the minimum values).
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCartesianPoint Corner
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _corner;
            }
            set { this.SetModelValue(this, ref _corner, value, v => Corner = v, "Corner"); }
        }

        /// <summary>
        ///   Length attribute (measured along the edge parallel to the X Axis)
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure XDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _xDim;
            }
            set { this.SetModelValue(this, ref _xDim, value, v => XDim = v, "XDim"); }
        }

        /// <summary>
        ///   Width attribute (measured along the edge parallel to the Y Axis)
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure YDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _yDim;
            }
            set { this.SetModelValue(this, ref _yDim, value, v => YDim = v, "YDim"); }
        }

        /// <summary>
        ///   Height attribute (measured along the edge parallel to the Z Axis).
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure ZDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _zDim;
            }
            set { this.SetModelValue(this, ref _zDim, value, v => ZDim = v, "ZDim"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _corner = (IfcCartesianPoint) value.EntityVal;
                    break;
                case 1:
                    _xDim = value.RealVal;
                    break;
                case 2:
                    _yDim = value.RealVal;
                    break;
                case 3:
                    _zDim = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived.   The space dimensionality of this class, it is always 3.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return new IfcDimensionCount(3); }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}