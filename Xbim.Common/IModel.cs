using System;
using System.Collections.Generic;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Common.Metadata;

namespace Xbim.Common
{
	public interface IModel
	{
        /// <summary>
        /// Returns or sets a user defined id for the model
        /// </summary>
        int UserDefinedId { get; set; }

        /// <summary>
        /// Returns a geometry store, null if geometry storage is not supported
        /// </summary>
        IGeometryStore GeometryStore { get; }
		IStepFileHeader Header { get; }

		bool IsTransactional { get; }

        IList<XbimInstanceHandle> InstanceHandles { get; }

	    IEntityCollection Instances { get; }

	    bool Activate(IPersistEntity owningEntity, bool write);

		void Delete (IPersistEntity entity);
		
		ITransaction BeginTransaction(string name);
		
		/// <summary>
        /// It is a good practise to implement this property with WeakReference back field so it gets disposed 
		/// when transaction goes out of the scope. It would stay allive otherwise which is not desired unless you 
		/// want to keep it for undo-redo sessions. But even it that case it should be referenced from elsewhere.
        /// </summary>
		ITransaction CurrentTransaction { get; }

		ExpressMetaData Metadata { get; }

		IModelFactors ModelFactors { get; }

        ///<summary>
        /// Implementation of IModel variant of InsertCopy() function
        /// </summary>
        /// <typeparam name="T">Type of the object to be inserted. This must be a type supported by this model</typeparam>
        /// <param name="toCopy">Object to copy</param>
        /// <param name="mappings">Mappings make sure object is only inserted once. You should use one instance of mappings for all InsertCopy() calls between two models</param>
        /// <param name="propTransform">Delegate which can be used to transform properties. You can use this to filter out certain properties or referenced objects</param>
        /// <param name="includeInverses">If TRUE interse relations are also copied over. This may potentially bring over almost entire model if not controlled by propTransform delegate</param>
        /// <param name="keepLabels">If TRUE entity labels of inserted objects will be the same as the labels of original objects. This should be FALSE if you are inserting objects to existing model
        /// or if you are inserting objects from multiple source models into a single target model where entity labels may potentially clash.</param>
        /// <returns>New created object in this model which is a deep copy of original object</returns>
        /// <returns></returns>
	    T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform,
	        bool includeInverses, bool keepLabels) where T : IPersistEntity;

		/// <summary>
        /// Performs a set of actions on a collection of entities inside a single read only transaction
        /// This improves database  performance for retrieving and accessing complex and deep objects
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity;

		/// <summary>
        /// This event is fired every time new entity is created.
        /// </summary>
        event NewEntityHandler EntityNew;
        
		/// <summary>
        /// This event is fired every time any entity is modified. If your model is not
        /// transactional it might not be called at all as the central point for all
        /// modifications is a transaction.
        /// </summary>
        event ModifiedEntityHandler EntityModified;
        
		/// <summary>
        /// This event is fired every time when entity gets deleted from model.
        /// </summary>
        event DeletedEntityHandler EntityDeleted;

	}

	public delegate void NewEntityHandler(IPersistEntity entity);
    public delegate void ModifiedEntityHandler(IPersistEntity entity);
    public delegate void DeletedEntityHandler(IPersistEntity entity);

    public delegate object PropertyTranformDelegate(ExpressMetaProperty property, object parentObject);


	public interface IModelFactors
    {
        /// <summary>
        /// Indicates level of detail for IfcProfileDefinitions, if 0 no fillet radii are applied, no leg slopes area applied, if 1 all details are applied
        /// </summary>
        int ProfileDefLevelOfDetail { get; set; }

        /// <summary>
        /// If this number is greater than 0, any faceted meshes will be simplified if the number of faces exceeds the threshhold
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
        /// The maximum number of faces to sew and check the result is a valid BREP, face sets with more than this number of faces will be processed as read from the model
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
        /// The number of decimal places to round a number to in order to truncate distances, not to be confused with precision, this is mostly for hashing and reporting, precision determins if two points are the same. NB this must be less that the precision for booleans
        /// </summary>
        int Rounding { get; }

        double OneMetre { get; }

        /// <summary>
        /// Returns the value for one millimetre in the units of the model
        /// </summary>
        double OneMilliMetre { get; }

        /// <summary>
        /// The min angle used when meshing shapes, works with DeflectionTolerance to set the resolution for linearising edges, default = 0.5
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
    }

    public class XbimModelFactors : IModelFactors
    {

        /// <summary>
        /// Indicates level of detail for IfcProfileDefinitions, if 0 no fillet radii are applied, no leg slopes area applied, if 1 all details are applied
        /// </summary>
        public int ProfileDefLevelOfDetail { get; set; }

        /// <summary>
        /// If this number is greater than 0, any faceted meshes will be simplified if the number of faces exceeds the threshhold
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
        /// The number of decimal places to round a number to in order to truncate distances, not to be confused with precision, this is mostly for hashing and reporting, precision determins if two points are the same. NB this must be less that the precision for booleans
        /// </summary>
        public int Rounding { get; private set; }
        
        public double OneMetre { get; private set; }
        
        /// <summary>
        /// Returns the value for one millimetre in the units of the model
        /// </summary>
        public double OneMilliMetre { get; private set; }

    //    public readonly XbimMatrix3D? WorldCoordinateSystem;
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
           Initialise(angToRads,lenToMeter,precision);
        }

        /// <summary>
        /// The min angle used when meshing shapes, works with DeflectionTolerance to set the resolution for linearising edges, default = 0.5
        /// </summary>
        public double DeflectionAngle { get; set; }

        public double OneFoot { get; private set;  }

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
            Precision = defaultPrecision ;
            PrecisionMax = OneMilliMetre / 10;
            MaxBRepSewFaceCount = 0;
            PrecisionBoolean = Math.Max(Precision, OneMilliMetre / 10); //might need to make it courser than point precision if precision is very fine
            PrecisionBooleanMax = OneMilliMetre * 100;
            Rounding = Math.Abs((int)Math.Log10(Precision * 100)); //default round all points to 100 times  precision, this is used in the hash functions

            var exp = Math.Floor(Math.Log10(Math.Abs(OneMilliMetre / 10d))); //get exponent of first significant digit
            _significantOrder = exp > 0 ? 0 : (int)Math.Abs(exp);
            ShortestEdgeLength = 10 * OneMilliMetre;
        }
    }
}
