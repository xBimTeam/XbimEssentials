#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceOfRevolution.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A surface of revolution (IfcSurfaceOfRevolution) is the surface obtained by rotating a curve one complete revolution about an axis.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A surface of revolution (IfcSurfaceOfRevolution) is the surface obtained by rotating a curve one complete revolution about an axis. The data shall be interpreted as below. 
    ///   The parameterisation is as follows where the curve has a parameterisation l(u):
    ///   C = AxisPosition.Location
    ///   V = AxisPosition.Z
    ///   In order to produce a single-value surface the a complete revolution, the curve shall be such that when expressed in a cylindrical coordinate system the curve shall be such that when expressed in a cylindrical coordinate system (r,φ ,z) centred at C with an axis V no two distinct parametric points on the curve shall have the same values for (r, z). 
    ///   For a surface of revolution the parametric range is 0 lt u lt 360 degree. The parameterisation range for v is defined by referenced curve.
    ///   NOTE: Corresponding STEP entity: surface_of_revolution. Please refer to ISO/IS 10303-42:1994, p.76 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 2x.
    ///   Informal propositions:
    ///   The surface shall not self-intersect 
    ///   The swept curve shall not be coincident with the axis line for any finite part of its legth.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcSurfaceOfRevolution : IfcSweptSurface
    {
        private IfcAxis1Placement _axisPosition;

        /// <summary>
        ///   A point on the axis of revolution and the direction of the axis of revolution.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcAxis1Placement AxisPosition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _axisPosition;
            }
            set { this.SetModelValue(this, ref _axisPosition, value, v => AxisPosition = v, "AxisPosition"); }
        }

        /// <summary>
        ///   Derived. The line coinciding with the axis of revolution.
        /// </summary>
        /// <remarks>
        ///   IfcLine :=  IfcRepresentationItem() || IfcGeometricRepresentationItem () || IfcCurve() || IfcLine(AxisPosition.Location, 
        ///   IfcRepresentationItem() || IfcGeometricRepresentationItem () || IfcVector(AxisPosition.Z,1.0));
        /// </remarks>
        public IfcLine AxisLine
        {
            get { throw new NotImplementedException(); }
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
                    _axisPosition = (IfcAxis1Placement) value.EntityVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}