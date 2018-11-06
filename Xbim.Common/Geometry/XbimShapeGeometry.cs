using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Xbim.Common.Geometry
{
    public struct XbimShapeGeometryHandle
    {
        /// <summary>
        /// The 3D model context that contains this shape geometry
        /// </summary>
        readonly short _contextHandle;
        /// <summary>
        /// The unique label of this shape geometry
        /// </summary>
        readonly int _shapeLabel;
        /// <summary>
        /// The number of references to this shape geoemetry
        /// </summary>
        readonly  int _referenceCount;

        public XbimShapeGeometryHandle(short contextHandle, int shapeLabel, int referenceCount)
        {
            _contextHandle = contextHandle;
            _shapeLabel = shapeLabel;
            _referenceCount = referenceCount;
        }

        /// <summary>
        /// The 3D model context that contains this shape geometry
        /// </summary>
        public short Context
        {
            get { return _contextHandle; }
        }
        /// <summary>
        /// The unique label of this shape geometry
        /// </summary>
        public int ShapeLabel
        {
            get
            {
                return _shapeLabel;
            }
        }
        /// <summary>
        /// The number of references to this shape
        /// </summary>
        public int ReferenceCount
        {
            get
            {
                return _referenceCount;
            }
        }
    }
    /// <summary>
    /// A basic shape geoemetry, note this is independent of placement and not specific to any product
    /// </summary>
    public class XbimShapeGeometry : IXbimShapeGeometryData
    {
        
        /// <summary>
        /// The unique label of this shape geometry
        /// </summary>
        int _shapeLabel;
        /// <summary>
        /// The label of the IFC object that defines this shape
        /// </summary>
        int _ifcShapeLabel;
        /// <summary>
        ///  Hash of the shape Geometry, based on the IFC representation, this is not unique
        /// </summary>
        int _geometryHash;
        /// <summary>
        /// The number of references to this shape
        /// </summary>
        int _referenceCount;

        /// <summary>
        /// The format in which the shape data is represented, i.e. triangular mesh, polygon, opencascade
        /// </summary>
        XbimGeometryType _format;
        /// <summary>
        /// The bounding box of this instance, requires tranformation to place in world coordinates
        /// </summary>
        XbimRect3D _boundingBox;
        /// <summary>
        /// The geometry data defining the shape
        /// </summary>
        byte[] _shapeData;



        /// <summary>
        /// The unique label of this shape geometry
        /// </summary>
        public int ShapeLabel
        {
            get
            {
                return _shapeLabel;
            }
            set
            {
                _shapeLabel = value;
            }
        }
        /// <summary>
        /// The label of the IFC object that defines this shape
        /// </summary>
        public int IfcShapeLabel
        {
            get
            {
                return _ifcShapeLabel;
            }
            set
            {
                _ifcShapeLabel = value;
            }
        }
        /// <summary>
        ///  Hash of the shape Geometry, based on the IFC representation, this is not unique
        /// </summary>
        public int GeometryHash
        {
            get
            {
                return _geometryHash;
            }
            set
            {
                _geometryHash = value;
            }
        }
        
        /// <summary>
        /// The cost in bytes of this shape
        /// </summary>
        public int Cost
        {
            get
            {
                if(_referenceCount==0)
                    return _shapeData.Length;
                return _referenceCount * _shapeData.Length;
            }
        }
        /// <summary>
        /// The number of references to this shape
        /// </summary>
        public int ReferenceCount
        {
            get
            {
                return _referenceCount;
            }
            set
            {
                _referenceCount = value;
            }
        }

        /// <summary>
        /// The level of detail or development that the shape is suited for
        /// </summary>
        public XbimLOD LOD { get; set; }

        byte IXbimShapeGeometryData.LOD
        {
            get
            {
                return (byte)LOD;
            }
            set
            {
                LOD = (XbimLOD)value;
            }
        }
        /// <summary>
        /// The format in which the shape data is represented, i.e. triangular mesh, polygon, opencascade
        /// </summary>
        public XbimGeometryType Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }
        /// <summary>
        /// The format in which the shape data is represented, i.e. triangular mesh, polygon, opencascade as a byte
        /// </summary>
        byte IXbimShapeGeometryData.Format
        {
            get
            {
                return (byte)_format;
            }
            set
            {
                _format = (XbimGeometryType)value;
            }
        }
        /// <summary>
        /// The bounding box of this instance, requires tranformation to place in world coordinates
        /// </summary>
        public XbimRect3D BoundingBox
        {
            get
            {
                return _boundingBox;
            }
            set
            {
                _boundingBox = value;
            }
        }
        /// <summary>
        /// The bounding box of this instance, requires tranformation to place in world coordinates
        /// </summary>
        byte[] IXbimShapeGeometryData.BoundingBox
        {
            get
            {
                return _boundingBox.ToFloatArray();
            }
            set
            {
                _boundingBox = XbimRect3D.FromArray(value);
            }
        }
        /// <summary>
        /// The geometry data defining the shape
        /// </summary>
        public string ShapeData
        {
            get
            {
                //Though you don't use it in "production" code, it should be preferable to use Base64 encoding to avoid invalid UTF-8 sequence. I used this property before to see IXbimShapeGeometryData.ShapeData property at my expense ;)
                //see: https://blogs.msdn.microsoft.com/shawnfa/2005/11/10/dont-roundtrip-ciphertext-via-a-string-encoding/
                //or : https://stackoverflow.com/questions/45410219/what-is-the-reason-that-encoding-utf8-getstring-and-encoding-utf8-getbytes-are-n
                //return System.Convert.ToBase64String(_shapeData);
                return Encoding.UTF8.GetString(_shapeData.ToArray());
            }
            set
            {
                //Though you don't use it in "production" code, it should be preferable to use Base64 encoding to avoid invalid UTF-8 sequence. I used this property before to see IXbimShapeGeometryData.ShapeData property at my expense ;)
                //see: https://blogs.msdn.microsoft.com/shawnfa/2005/11/10/dont-roundtrip-ciphertext-via-a-string-encoding/
                //or : https://stackoverflow.com/questions/45410219/what-is-the-reason-that-encoding-utf8-getstring-and-encoding-utf8-getbytes-are-n
                //_shapeData = System.Convert.FromBase64String(value);
                _shapeData = Encoding.UTF8.GetBytes(value);
            }
        }
        /// <summary>
        /// The geometry data defining the shape, this is a compressed representation of the data
        /// </summary>
        byte[] IXbimShapeGeometryData.ShapeDataCompressed
        {
            get
            {
                //var bytes = Encoding.UTF8.GetBytes(_shapeData);

                using (var msi = new MemoryStream(_shapeData))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        msi.CopyTo(gs);
                    }

                    return mso.ToArray();
                }
            }
            set
            {
                using (var msi = new MemoryStream(value))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }
                    //_shapeData = Encoding.UTF8.GetString(mso.ToArray());
                    _shapeData = mso.ToArray();
                }

            }
        }

        byte[] IXbimShapeGeometryData.ShapeData
        {
            get
            {
                return _shapeData;
            }
            set
            {
                _shapeData = value;
            }
        }
        /// <summary>
        /// Returns true if the geometry is valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _shapeLabel > 0;
            }
        }

        public override string ToString()
        {

            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", _shapeLabel, _ifcShapeLabel, _geometryHash, _shapeLabel, _referenceCount, LOD, _format, _boundingBox.ToString(), _shapeData);
        }

       
     

    }
}
