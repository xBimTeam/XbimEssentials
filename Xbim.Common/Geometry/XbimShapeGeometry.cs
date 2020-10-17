using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Xbim.Common.Geometry
{
    public delegate int ReadIndex(byte[] array, int offset);
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
        const int VersionPos = 0;
        const int VertexCountPos = VersionPos + sizeof(byte);
        const int TriangleCountPos = VertexCountPos + sizeof(int);
        const int VertexPos = TriangleCountPos + sizeof(int);
        public byte Version => _shapeData?.Length > 0 ? _shapeData[VersionPos] : (byte)0;
        public int VertexCount => _shapeData?.Length > 0 ? BitConverter.ToInt32(_shapeData, VertexCountPos) : 0;
        public int TriangleCount => _shapeData?.Length > 0 ? BitConverter.ToInt32(_shapeData, TriangleCountPos) : 0;
        public int FaceCount
        {
            get
            {
                var faceCountPos = VertexPos + (VertexCount * 3 * sizeof(float));
                return _shapeData?.Length > 0 ? BitConverter.ToInt32(_shapeData, faceCountPos) : 0;
            }
        }
        public int Length => _shapeData?.Length > 0 ? _shapeData.Length : 0;
        public byte[] ToByteArray() => _shapeData;
        public IEnumerable<XbimPoint3D> Vertices
        {
            get
            {
                const int offsetY = sizeof(float);
                const int offsetZ = 2 * sizeof(float);
                for (int i = 0; i < VertexCount; i++)
                {
                    var p = VertexPos + (i * 3 * sizeof(float));
                    yield return new XbimPoint3D(BitConverter.ToSingle(_shapeData, p), BitConverter.ToSingle(_shapeData, p + offsetY), BitConverter.ToSingle(_shapeData, p + offsetZ));
                }
            }
        }
        /// <summary>
        /// Returns the vector at the specified position
        /// </summary>
        /// <param name="vectorIndex"></param>
        /// <returns></returns>
        public XbimPoint3D this[int vectorIndex]
        {
            get
            {
                const int offsetY = sizeof(float);
                const int offsetZ = 2 * sizeof(float);

                var p = VertexPos + (vectorIndex * 3 * sizeof(float));
                return new XbimPoint3D(BitConverter.ToSingle(_shapeData, p), BitConverter.ToSingle(_shapeData, p + offsetY), BitConverter.ToSingle(_shapeData, p + offsetZ));

            }
        }
        public IEnumerable<WexBimMeshFace> Faces
        {
            get
            {
                var faceOffset = VertexPos + (VertexCount * 3 * sizeof(float)) + sizeof(int);//start of vertices * space taken by vertices + the number of faces
                ReadIndex readIndex;
                int sizeofIndex;
                if (VertexCount <= 0xFF)
                {
                    readIndex = (array, offset) => array[offset];
                    sizeofIndex = sizeof(byte);
                }
                else if (VertexCount <= 0xFFFF)
                {
                    readIndex = (array, offset) => BitConverter.ToUInt16(array, offset);
                    sizeofIndex = sizeof(ushort);
                }
                else
                {
                    readIndex = (array, offset) => BitConverter.ToInt32(array, offset);
                    sizeofIndex = sizeof(int);
                }
                for (int i = 0; i < FaceCount; i++)
                {
                    var face = new WexBimMeshFace(readIndex, sizeofIndex, _shapeData, faceOffset);
                    faceOffset += face.ByteSize;
                    yield return face;
                }
            }
        }
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


        /// <summary>
        /// If the shape coordinates are large the actual serialised geometry should be
        /// reduced to local origin to avoid problems with floating point precission
        /// of float coordinates. This displacement should be presented in 
        /// LocalShapeDisplacement and should be added to placement of the shape in the product.
        /// </summary>
        IVector3D IXbimShapeGeometryData.LocalShapeDisplacement => LocalShapeDisplacement;
        public XbimVector3D? LocalShapeDisplacement { get; set; }


    }

    public class WexBimMeshFace
    {
        private byte[] _array;
        private int _offsetStart;
        private ReadIndex _readIndex;
        private int _sizeofIndex;

        internal WexBimMeshFace(ReadIndex readIndex, int sizeofIndex, byte[] array, int faceOffset)
        {
            _readIndex = readIndex;
            _array = array;
            _offsetStart = faceOffset;
            _sizeofIndex = sizeofIndex;
        }
        public int ByteSize
        {
            get
            {
                if (IsPlanar)
                    return sizeof(int) + 2 + (TriangleCount * 3 * _sizeofIndex); // trianglecount+ normal in 2 bytes + triangulation with no normals
                else
                    return sizeof(int) + (TriangleCount * 3 * (_sizeofIndex + 2)); //trianglecount+ normal  + triangulation with normals in 2 bytes
            }
        }
        public int TriangleCount => Math.Abs(BitConverter.ToInt32(_array, _offsetStart));
        public bool IsPlanar => BitConverter.ToInt32(_array, _offsetStart) > 0;
        public IEnumerable<int> Indices
        {
            get
            {

                if (IsPlanar)
                {
                    var indexOffset = _offsetStart + sizeof(int) + 2; //offset + trianglecount + packed normal of plane in the 2 bytes
                    var indexSpan = 3 * _sizeofIndex;
                    for (int i = 0; i < TriangleCount; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            yield return _readIndex(_array, indexOffset + (j * _sizeofIndex)); //skip the normal in the 2 bytes
                        }
                        indexOffset += indexSpan;
                    }
                }
                else
                {
                    var indexOffset = _offsetStart + sizeof(int); //offset + trianglecount
                    var indexSpan = _sizeofIndex + 2; //index  + normal in 2 bytes
                    var triangleSpan = 3 * indexSpan;

                    for (int i = 0; i < TriangleCount; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            yield return _readIndex(_array, indexOffset + (j * indexSpan));
                        }
                        indexOffset += triangleSpan;
                    }
                }
            }
        }

        /// <summary>
        /// returns the normal for a specific point at a specific index on the face
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public XbimVector3D NormalAt(int index)
        {
            var indexOffset = _offsetStart + sizeof(int); //offset + trianglecount
            if (IsPlanar) //no matter what you send in for the index you will get the same value because it is planar
            {
                var u = _array[indexOffset];
                var v = _array[indexOffset + 1];
                var pn = new XbimPackedNormal(u, v);
                return pn.Normal;
            }
            else
            {
                var indexSpan = _sizeofIndex + 2;
                int normalOffset = indexOffset + (index * indexSpan) + _sizeofIndex;
                var u = _array[normalOffset];
                var v = _array[normalOffset + 1];
                var pn = new XbimPackedNormal(u, v);
                return pn.Normal;
            }
        }
        public IEnumerable<XbimVector3D> Normals
        {
            get
            {
                var indexOffset = _offsetStart + sizeof(int); //offset + trianglecount
                if (IsPlanar)
                {
                    var u = _array[indexOffset];
                    var v = _array[indexOffset + 1];
                    var pn = new XbimPackedNormal(u, v);
                    yield return pn.Normal;
                }
                else
                {
                    var indexSpan = _sizeofIndex + 2;
                    var triangleSpan = 3 * indexSpan;
                    var normalOffset = indexOffset + _sizeofIndex;
                    for (int i = 0; i < TriangleCount; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var u = _array[normalOffset + (j * indexSpan)];
                            var v = _array[normalOffset + (j * indexSpan) + 1];
                            var pn = new XbimPackedNormal(u,v);                           
                            yield return pn.Normal;
                        }
                        normalOffset += triangleSpan;
                    }
                }
            }
        }
    }
   
}
