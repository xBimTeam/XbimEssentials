using System;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc4.Functions
{
    // this class name is determined in order to allow a similar syntax to the IFC express function definition
    internal class New
    {
        internal static Direction IfcDirection(double x, double y, double z)
        {
            return new Direction(x, y, z);
        }

        internal static Direction IfcDirection(double x, double y)
        {
            return new Direction(x, y);
        }

        public static Vector IfcVector(Direction direction, double mod)
        {
            return new Vector(direction, mod);
        }

        public static Vector IfcVector(IIfcDirection direction, double mod)
        {
            return new Vector(new Direction(direction), mod);
        }

        DimensionalExponents DimensionalExponents(int len, int mass, int time, int elec, int temp, int substance, int lum)
        {
            return new DimensionalExponents(len, mass, time, elec, temp, substance, lum);
        }

        public static VectorOrDirection VectorOrDirection(Direction direction)
        {
           return  new VectorOrDirection(direction);
        }
    }
}