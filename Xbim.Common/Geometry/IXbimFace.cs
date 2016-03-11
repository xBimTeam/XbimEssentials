﻿using System;

namespace Xbim.Common.Geometry
{
    public interface IXbimFace : IXbimGeometryObject, IEquatable<IXbimFace>
    {
        IXbimWire OuterBound { get; }
        IXbimWireSet InnerBounds { get; }
        double Area { get; }
        double Perimeter { get; }
        /// <summary>
        /// The topological normal of the face, nb.  this may differ from the normal of the bound
        /// </summary>
        XbimVector3D Normal { get; }
        bool IsPlanar { get; }
        void SaveAsBrep(string fileName);
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }
    }
}
