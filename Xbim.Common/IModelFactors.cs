namespace Xbim.Common
{
    public interface IModelFactors
    {
        /// <summary>
        /// Indicates level of detail for IfcProfileDefinitions, if 0 no fillet radii are applied, no leg slopes area applied, if 1 all details are applied
        /// </summary>
        int ProfileDefLevelOfDetail { get; set; }

        /// <summary>
        /// If this number is greater than 0, any faceted meshes will be simplified if the number of faces exceeds the threshold
        /// </summary>
        int SimplifyFaceCountThreshHold { get; set; }

        /// <summary>
        /// If the SimplifyFaceCountThreshHold is greater than 0, this is the minimum length of any edge in a face in millimetres, default is 10mm
        /// </summary>
        double ShortestEdgeLength { get; set; }

        /// <summary>
        /// Precision used for Boolean solid geometry operations, default 0.001mm
        /// </summary>
        double PrecisionBoolean { get; set; }

        /// <summary>
        /// The maximum Precision used for Boolean solid geometry operations, default 10mm
        /// </summary>
        double PrecisionBooleanMax { get; set; }

        /// <summary>
        /// The defection on a curve when triangulating the model
        /// </summary>
        double DeflectionTolerance { get; set; }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        double AngleToRadiansConversionFactor { get; }

        /// <summary>
        /// Conversion to metres
        /// </summary>
        double LengthToMetresConversionFactor { get; }

        /// <summary>
        /// Used to display a vertex this is the diameter that will be used to auto-generate a geometric representation of a topological vertex
        /// </summary>
        double VertexPointDiameter { get; }

        /// <summary>
        /// The maximum number of faces to sew and check the result is a valid BREP, face sets with more than this number of faces will be processed 
        /// as read from the model
        /// </summary>
        int MaxBRepSewFaceCount { get; set; }

        /// <summary>
        /// The  normal tolerance under which two given points are still assumed to be identical
        /// </summary>
        double Precision { get; set; }

        /// <summary>
        /// Returns the value for one metre in the units of the model
        /// </summary>
        /// /// <summary>
        /// The  maximum tolerance under which two given points are still assumed to be identical
        /// </summary>
        double PrecisionMax { get; set; }

        /// <summary>
        /// The number of decimal places to round a number to in order to truncate distances, not to be confused with precision, this is 
        /// mostly for hashing and reporting, precision determines if two points are the same. NB this must be less that the precision for Booleans
        /// </summary>
        int Rounding { get; }

        double OneMetre { get; }

        /// <summary>
        /// Returns the value for one millimetre in the units of the model
        /// </summary>
        double OneMilliMetre { get; }

        /// <summary>
        /// The min angle used when meshing shapes, works with DeflectionTolerance to set the resolution for linearizing edges, default = 0.5
        /// </summary>
        double DeflectionAngle { get; set; }

        double OneFoot { get; }
        double OneInch { get; }
        double OneKilometer { get; }
        double OneMeter { get; }
        double OneMile { get; }
        double OneMilliMeter { get; }

        int GetGeometryFloatHash(float number);
        int GetGeometryDoubleHash(double number);

        void Initialise(double angleToRadiansConversionFactor, double lengthToMetresConversionFactor, double defaultPrecision);

        bool ApplyWorkAround(string name);
    }
}
