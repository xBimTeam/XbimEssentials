#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceOfLinearExtrusion.cs
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

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   This surface is a simple swept surface or a generalised cylinder obtained by sweeping a curve in a given direction.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: This surface is a simple swept surface or a generalised cylinder obtained by sweeping a curve in a given direction. The parameterisation is as follows where the curve has a parameterisation l(u):
    ///   V = ExtrusionAxis
    ///   The parameterisation range for v is -¥ lt v lt ¥ and for u it is defined by the curve parameterisation.
    ///   NOTE: Corresponding STEP entity: surface_of_linear_extrusion. Please refer to ISO/IS 10303-42:1994, p.76 for the final definition of the formal standard. The following adaption has been made. The ExtrusionAxis and the Direction are defined as two separate attributes in correlation to the definition of the extruded_area_solid, and not as a single vector attribute. The vector is derived as ExtrusionAxis.
    ///   HISTORY: New entity in IFC Release 2x.
    ///   Informal propositions:
    ///   The surface shall not self-intersect
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcSurfaceOfLinearExtrusion : IfcSweptSurface
    {
        #region Fields

        private IfcDirection _extrudedDirection;
        private IfcLengthMeasure _depth;

        #endregion

        /// <summary>
        ///   The direction of the extrusion.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcDirection ExtrudedDirection
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _extrudedDirection;
            }
            set
            {
                this.SetModelValue(this, ref _extrudedDirection, value, v => ExtrudedDirection = v,
                                           "ExtrudedDirection");
            }
        }

        /// <summary>
        ///   The depth of the extrusion, it determines the parameterisation.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcLengthMeasure Depth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _depth;
            }
            set { this.SetModelValue(this, ref _depth, value, v => Depth = v, "Depth"); }
        }

        /// <summary>
        ///   Derived. The extrusion axis defined as vector.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public IfcVector ExtrusionAxis
        {
            get { return new IfcVector(_extrudedDirection, _depth); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _extrudedDirection = (IfcDirection) value.EntityVal;
                    break;
                case 3:
                    _depth = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            if (_depth <= 0)
                return "WR41 SurfaceOfLinearExtrusion : Depth must be > 0.\n";
            else
                return "";
        }
    }
}