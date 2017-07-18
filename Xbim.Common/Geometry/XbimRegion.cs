namespace Xbim.Common.Geometry
{
    /// <summary>
    /// Used in the clustering analysis of elements the model.
    /// </summary>
    public class XbimRegion
    {
        public string Name;
        public XbimVector3D Size;
        public XbimPoint3D Centre;
        public int Population = -1;

        public XbimRegion(string name, XbimRect3D bounds, int population, XbimMatrix3D worldCoordinateSystem)
        {
            Name = name;
            Size = new XbimVector3D(bounds.SizeX,bounds.SizeY,bounds.SizeZ);
            Centre = bounds.Centroid();
            Population = population;
            WorldCoordinateSystem = worldCoordinateSystem;
        }

        public XbimRegion()
        {
        }

        public XbimRect3D ToXbimRect3D()
        {
            return new XbimRect3D(
                Centre - Size * 0.5,
                Size);
        }

        internal double Diagonal()
        {
            return Size.Length;
        }

        public XbimMatrix3D WorldCoordinateSystem { get; set; }
    }
}
