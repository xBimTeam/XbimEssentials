using System;
using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ConnectedFaceSetExtensions
    {
        public static void Bounds(this IfcConnectedFaceSet fSet,
            out double Xmin, out double Ymin, out double Zmin, out double Xmax, out double Ymax, out double Zmax)
        {
            double xmin = 0; double ymin = 0; double zmin = 0; double xmax = 0; double ymax = 0; double zmax = 0;
            var first = true;
            var model = fSet.Model;
            model.ForEach(fSet.CfsFaces, face=>
            {
                var outer = face.Bounds.FirstOrDefault<IfcFaceOuterBound>(fb => true) ??
                                     face.Bounds.FirstOrDefault();
                if (outer == null) return;
                var loop = outer.Bound as IfcPolyLoop;
                if (loop != null)
                {
                    foreach (var pt in loop.Polygon)
                    {
                        if (first)
                        {
                            xmin = pt.X;
                            ymin = pt.Y;
                            zmin = pt.Z;
                            xmax = pt.X;
                            ymax = pt.Y;
                            zmax = pt.Z;
                            first = false;
                        }
                        else
                        {
                            xmin = Math.Min(xmin, pt.X);
                            ymin = Math.Min(ymin, pt.Y);
                            zmin = Math.Min(zmin, pt.Z);
                            xmax = Math.Max(xmax, pt.X);
                            ymax = Math.Max(ymax, pt.Y);
                            zmax = Math.Max(zmax, pt.Z);
                        }
                    }
                }
                
            });
            Xmin = xmin; Ymin = ymin; Zmin = zmin; Xmax = xmax; Ymax = ymax; Zmax = zmax;
        }
    }
}
