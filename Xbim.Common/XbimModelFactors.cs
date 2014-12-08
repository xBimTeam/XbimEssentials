using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Geometry;

namespace Xbim.Common
{
    public class XbimModelFactors
    {

        /// <summary>
        /// Indicates level of detail for IfcProfileDefinitions, if 0 no fillet radii are applied, no leg slopes area applied, if 1 all details are applied
        /// </summary>
        public int ProfileDefLevelOfDetail = 0;
        /// <summary>
        /// If this number is greater than 0, any faceted meshes will be simplified if the number of faces exceeds the threshhold
        /// </summary>
        public int SimplifyFaceCountThreshHold = 1000;

        /// <summary>
        /// If the SimplifyFaceCountThreshHold is greater than 0, this is the minimum length of any edge in a face in millimetres, default is 10mm
        /// </summary>
        public double ShortestEdgeLength;
        /// <summary>
        /// Precision used for Boolean solid geometry operations, default 0.001mm
        /// </summary>
        public double PrecisionBoolean;
        /// <summary>
        /// The maximum Precision used for Boolean solid geometry operations, default 10mm
        /// </summary>
        public double PrecisionBooleanMax;
        /// <summary>
        /// The defection on a curve when triangulating the model
        /// </summary>
        public double DeflectionTolerance;    
        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        readonly public double AngleToRadiansConversionFactor;
        /// <summary>
        /// Conversion to metres
        /// </summary>
        readonly public double LengthToMetresConversionFactor;
        /// <summary>
        /// Used to display a vertex this is the diameter that will be used to auto-generate a geometric representation of a topological vertex
        /// </summary>
        readonly public double VertexPointDiameter;
        /// <summary>
        /// The maximum number of faces to sew and check the result is a valid BREP, face sets with more than this number of faces will be processed as read from the model
        /// </summary>
        public int MaxBRepSewFaceCount;
        /// <summary>
        /// The  normal tolerance under which two given points are still assumed to be identical
        /// </summary>
        public double Precision;
        /// <summary>
        /// Returns the value for one metre in the units of the model
        /// </summary>
        /// /// <summary>
        /// The  maximum tolerance under which two given points are still assumed to be identical
        /// </summary>
        public double PrecisionMax;
        /// <summary>
        /// The number of decimal places to round a number to in order to truncate distances, not to be confused with precision, this is mostly for hashing and reporting, precision determins if two points are the same. NB this must be less that the precision for booleans
        /// </summary>
        readonly public int Rounding;
        readonly public double OneMetre;
        /// <summary>
        /// Returns the value for one millimetre in the units of the model
        /// </summary>
        readonly public double OneMilliMetre;

        public readonly XbimMatrix3D? WorldCoordinateSystem;
        private int _significantOrder;
        public int GetGeometryFloatHash(float number)
        {
            return Math.Round(number, _significantOrder).GetHashCode();
        }

        public int GetGeometryDoubleHash(double number)
        {
            return Math.Round(number, _significantOrder).GetHashCode();
        }

        public XbimModelFactors(double angToRads, double lenToMeter, double? precision = null, XbimMatrix3D? wcs= null)
        {
            WorldCoordinateSystem = wcs;
            AngleToRadiansConversionFactor = angToRads;
            LengthToMetresConversionFactor = lenToMeter;
           
            OneMeter = OneMetre = 1/lenToMeter;
            OneMilliMeter = OneMilliMetre = OneMeter / 1000.0;
            OneKilometer = OneMeter * 1000.0;
            OneFoot = OneMeter / 3.2808;
            OneInch = OneMeter / 39.37;
            OneMile = OneMeter * 1609.344;


            DeflectionTolerance = OneMilliMetre*5; //5mm chord deflection
            DeflectionAngle = 0.5; 
            VertexPointDiameter = OneMilliMetre * 10; //1 cm
            //if (precision.HasValue)
            //    Precision = Math.Min(precision.Value,OneMilliMetre / 1000);
            //else
            //    Precision = Math.Max(1e-5, OneMilliMetre / 1000);
            if (precision.HasValue)
                Precision = precision.Value;
            else
                Precision = 1e-5;
            PrecisionMax = OneMilliMetre / 10;
            MaxBRepSewFaceCount = 0;
            PrecisionBoolean =  Math.Max(Precision,OneMilliMetre/10); //might need to make it courser than point precision if precision is very fine
            PrecisionBooleanMax = OneMilliMetre *100;
            Rounding = Math.Abs((int)Math.Log10(Precision*100)); //default round all points to 100 times  precision, this is used in the hash functions

            var exp = Math.Floor(Math.Log10(Math.Abs(OneMilliMetre / 10d))); //get exponent of first significant digit
            _significantOrder = exp > 0 ? 0 : (int)Math.Abs(exp);
            ShortestEdgeLength = 10 * OneMilliMetre;
        }

        /// <summary>
        /// The min angle used when meshing shapes, works with DeflectionTolerance to set the resolution for linearising edges, default = 0.5
        /// </summary>
        public double DeflectionAngle { get; set; }

        public double OneFoot
        {
            get; private set; 
        }
        public double OneInch
        {
            get;
            private set;
        }
        public double OneKilometer
        {
            get;
            private set;
        }
        public double OneMeter
        {
            get;
            private set;
        }
        public double OneMile
        {
            get;
            private set;
        }
        public double OneMilliMeter
        {
            get;
            private set;
        }
    }
}
