#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPlane.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A plane is an unbounded surface with a constant normal.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A plane is an unbounded surface with a constant normal. A plane is defined by a point on the plane and the normal direction to the plane. The data is to be interpreted as follows: 
    ///   C = SELF\IfcElementarySurface.Position.Location
    ///   x = SELF\IfcElementarySurface.Position.P[1]
    ///   y = SELF\IfcElementarySurface.Position.P[2]
    ///   z = SELF\IfcElementarySurface.Position.P[3] =&gt; normal to planeand the surface is parameterized as: 
    ///   where the parametric range is -∞ lt u,v lt ∞ .
    ///   In the above parameterization the length unit for the unit vectors x and y is derived from the context of the plane. 
    ///   NOTE Corresponding STEP entity: plane. Please refer to ISO/IS 10303-42:1994, p.69 for the final definition of the formal standard. 
    ///   HISTORY New class in IFC Release 1.5
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPlane : IfcElementarySurface
    {
        public override string WhereRule()
        {
            return "";
        }
    }
}