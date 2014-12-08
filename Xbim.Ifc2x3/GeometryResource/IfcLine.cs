#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLine.cs
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
    ///   A line is an unbounded curve with constant tangent direction.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A line is an unbounded curve with constant tangent direction. A line is defined by a point and a direction. The positive direction of the line is in the direction of the Dir vector. The line is parameterized as follows: 
    ///   P = Pnt 
    ///   V = Dir 
    ///   l(u) = P + uV 
    ///   and the parametric range is -¥ lt u lt ¥
    ///   NOTE Corresponding STEP entity: line. Please refer to ISO/IS 10303-42:1994, p.37 for the final definition of the formal standard. The derived attribute Dim has been added at this level and was therefore demoted from the geometric_representation_item. 
    ///   HISTORY New class in IFC Release 1.0
    ///   Formal Propositions:
    ///   WR1   :   The dimensionality of the location (IfcCartesianPoint) shall be the same as of the direction (IfcVector).
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcLine : IfcCurve
    {
        #region Fields

        private IfcCartesianPoint _pnt;
        private IfcVector _dir;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The location of the line.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCartesianPoint Pnt
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pnt;
            }
            set { this.SetModelValue(this, ref _pnt, value, v => Pnt = v, "Pnt"); }
        }


        /// <summary>
        ///   The direction of the line, the magnitude and units of Dir affect the parameterization of the line.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcVector Dir
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dir;
            }
            set { this.SetModelValue(this, ref _dir, value, v => Dir = v, "Dir"); }
        }

        public override IfcDimensionCount Dim
        {
            get { return Pnt.Dim; }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _pnt = (IfcCartesianPoint) value.EntityVal;
                    break;
                case 1:
                    _dir = (IfcVector) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (Pnt.Dim != Dir.Dim)
                return
                    "WR1: Line: The dimensionality of the location (IfcCartesianPoint) shall be the same as of the direction (Vector).";
            return "";
        }

        #endregion
    }
}