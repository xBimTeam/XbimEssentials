#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperator2D.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;


#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A Cartesian transformation operator 2d defines a geometric transformation in two-dimensional space composed of translation, rotation, mirroring and uniform scaling.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: 
    ///   A Cartesian transformation operator 2d defines a geometric transformation in two-dimensional space composed of translation, rotation, mirroring and uniform scaling. 
    ///   The list of normalised vectors u defines the columns of an orthogonal matrix T. 
    ///   These vectors are computed from the direction attributes axis1 and axis2 by the base axis function. If |T|= -1, the transformation includes mirroring. 
    ///   NOTE: Corresponding STEP entity : cartesian_transformation_operator_2d, please refer to ISO/IS 10303-42:1994, p. 36 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcCartesianTransformationOperator2D : IfcCartesianTransformationOperator
    {
        #region Part 21 Step file Parse routines

  
        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Dim != 2)
                baseErr +=
                    "WR1 CartesianTransformationOperator2D : The coordinate space dimensionality of this entity shall be 2\n";
            if (Axis1 != null && Axis1.Dim != 2)
                baseErr +=
                    "WR2 CartesianTransformationOperator2D : The inherited Axis1 should have (if given) the dimensionality of 2\n";
            if (Axis2 != null && Axis2.Dim != 2)
                baseErr +=
                    "WR2 CartesianTransformationOperator2D : The inherited Axis2 should have (if given) the dimensionality of 2\n";
            return baseErr;
        }
        #endregion
    }
}