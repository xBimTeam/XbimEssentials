#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperator2DnonUniform.cs
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
    [IfcPersistedEntityAttribute]
    public class IfcCartesianTransformationOperator2DnonUniform : IfcCartesianTransformationOperator2D
    {
        /// <summary>
        ///   Optional.   The scaling value specified for the transformation along the axis 2. This is normally the y scale factor.
        /// </summary>
        public double Scale2
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        ///   Derived. The derived scale S(2) of the transformation along the axis 2 (normally the y axis), equal to Scale2 if that exists, or equal to the derived Scl1 (normally the x axis scale factor) otherwise.
        /// </summary>
        public double Scl2
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}