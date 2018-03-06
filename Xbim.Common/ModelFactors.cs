using System;
using System.Collections.Generic;

namespace Xbim.Common
{
    public class XbimModelFactors : IModelFactors
    {
        private HashSet<string> _workArounds = new HashSet<string>();
        /// <summary>
        /// returns true if a model specific work around needs to be applied
        /// </summary>
        /// <param name="workAroundName"></param>
        /// <returns></returns>
        public bool ApplyWorkAround(string workAroundName)
        {
            return _workArounds.Contains(workAroundName);
        }
        public void AddWorkAround(string workAroundName)
        {
            _workArounds.Add(workAroundName);
        }
        /// <summary>
        /// Indicates level of detail for IfcProfileDefinitions, if 0 no fillet radii are applied, no leg slopes area applied, if 1 all details are applied
        /// </summary>
        public int ProfileDefLevelOfDetail { get; set; }

        /// <summary>
        /// If this number is greater than 0, any faceted meshes will be simplified if the number of faces exceeds the threshold
        /// </summary>
        public int SimplifyFaceCountThreshHold { get; set; }

        /// <summary>
        /// If the SimplifyFaceCountThreshHold is greater than 0, this is the minimum length of any edge in a face in millimetres, default is 10mm
        /// </summary>
        public double ShortestEdgeLength { get; set; }
        /// <summary>
        /// Precision used for Boolean solid geometry operations, default 0.001mm
        /// </summary>
        public double PrecisionBoolean { get; set; }
        /// <summary>
        /// The maximum Precision used for Boolean solid geometry operations, default 10mm
        /// </summary>
        public double PrecisionBooleanMax { get; set; }
        /// <summary>
        /// The defection on a curve when triangulating the model
        /// </summary>
        public double DeflectionTolerance { get; set; }
        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        public double AngleToRadiansConversionFactor { get; private set; }
        /// <summary>
        /// Conversion to metres
        /// </summary>
        public double LengthToMetresConversionFactor { get; private set; }
        /// <summary>
        /// Used to display a vertex this is the diameter that will be used to auto-generate a geometric representation of a topological vertex
        /// </summary>
        public double VertexPointDiameter { get; private set; }
        /// <summary>
        /// The maximum number of faces to sew and check the result is a valid BREP, face sets with more than this number of faces will be processed as read from the model
        /// </summary>
        public int MaxBRepSewFaceCount { get; set; }
        /// <summary>
        /// The  normal tolerance under which two given points are still assumed to be identical
        /// </summary>
        public double Precision { get; set; }
        /// <summary>
        /// Returns the value for one metre in the units of the model
        /// </summary>
        /// /// <summary>
        /// The  maximum tolerance under which two given points are still assumed to be identical
        /// </summary>
        public double PrecisionMax { get; set; }
        /// <summary>
        /// The number of decimal places to round a number to in order to truncate distances, not to be confused with precision, this is mostly for hashing and reporting, 
        /// precision determines if two points are the same. NB this must be less that the precision for booleans
        /// </summary>
        public int Rounding { get; private set; }

        public double OneMetre { get; private set; }

        /// <summary>
        /// Returns the value for one millimetre in the units of the model
        /// </summary>
        public double OneMilliMetre { get; private set; }

        private int _significantOrder;
        public int GetGeometryFloatHash(float number)
        {
            return Math.Round(number, _significantOrder).GetHashCode();
        }

        public int GetGeometryDoubleHash(double number)
        {
            return Math.Round(number, _significantOrder).GetHashCode();
        }

        public XbimModelFactors(double angToRads, double lenToMeter, double precision)
        {
            Initialise(angToRads, lenToMeter, precision);
        }

        /// <summary>
        /// The min angle used when meshing shapes, works with DeflectionTolerance to set the resolution for linearizing edges, default = 0.5
        /// </summary>
        public double DeflectionAngle { get; set; }

        public double OneFoot { get; private set; }

        public double OneInch { get; private set; }

        public double OneKilometer { get; private set; }

        public double OneMeter { get; private set; }

        public double OneMile { get; private set; }

        public double OneMilliMeter { get; private set; }


        public void Initialise(double angToRads, double lenToMeter, double defaultPrecision)
        {
            ProfileDefLevelOfDetail = 0;
            SimplifyFaceCountThreshHold = 1000;

            //  WorldCoordinateSystem = wcs;
            AngleToRadiansConversionFactor = angToRads;
            LengthToMetresConversionFactor = lenToMeter;

            OneMeter = OneMetre = 1 / lenToMeter;
            OneMilliMeter = OneMilliMetre = OneMeter / 1000.0;
            OneKilometer = OneMeter * 1000.0;
            OneFoot = OneMeter / 3.2808;
            OneInch = OneMeter / 39.37;
            OneMile = OneMeter * 1609.344;


            DeflectionTolerance = OneMilliMetre * 5; //5mm chord deflection
            DeflectionAngle = 0.5;
            VertexPointDiameter = OneMilliMetre * 10; //1 cm           
            Precision = defaultPrecision;
            PrecisionMax = Math.Max(OneMilliMetre / 10, Precision * 100);
            MaxBRepSewFaceCount = 0;
            PrecisionBoolean = Math.Max(Precision, OneMilliMetre / 10); //might need to make it courser than point precision if precision is very fine
            PrecisionBooleanMax = Math.Max(OneMilliMetre * 100, Precision * 100);
            Rounding = Math.Abs((int)Math.Log10(Precision * 100)); //default round all points to 100 times  precision, this is used in the hash functions

            var exp = Math.Floor(Math.Log10(Math.Abs(OneMilliMetre / 10d))); //get exponent of first significant digit
            _significantOrder = exp > 0 ? 0 : (int)Math.Abs(exp);
            ShortestEdgeLength = 10 * OneMilliMetre;
        }
    }
}
