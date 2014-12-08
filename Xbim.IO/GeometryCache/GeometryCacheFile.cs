#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.IO
// Filename:    GeometryCacheFile.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;

#endregion

namespace Xbim.IO.GeometryCache
{
    /// <summary>
    ///   Structure of the file is:
    ///   === HEADER SECTION ===
    ///   File signature in ascii ("xbimgeomcache " + 4 byte ascii version e.g. xbimgeomcache 0001, (len = 18)
    ///   long int lenght of the file
    ///   long int address of the _positions section
    ///   === GEOMETRY SECTION ===
    ///   [binary representation of triangulated meshes]
    ///   === POSITIONS SECTION ===
    ///   int positions count
    ///   [long position key + long position value] x [positions count]
    ///   === END OF STREAM ===
    /// </summary>
    public class GeometryCacheFile : IDisposable
    {
        public const string GeomCacheExtension = "xbimgc";

        private Dictionary<long, long> _positions = new Dictionary<long, long>();

        internal const string FILESIGNATURE = "xbimgeomcache 0001";
        internal const long offset_FileLenght = 18;
        internal const long offset_PositionsSectionAddress = 26;

        // private Stream _geomStream;
        private BinaryReader _geomReader;
        private bool _isInitialised = false;

        public void Open(string FileName)
        {
            if (!File.Exists(FileName))
                throw new Exception("GeometryCacheFile - File not found: " + FileName);
            Stream _geomStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            Initialise(_geomStream);
        }

        public void Open(Stream stream)
        {
            // _geomStream = stream;
            Initialise(stream);
        }

        private void Initialise(Stream _geomStream)
        {
            if (!_geomStream.CanSeek)
                throw new Exception("GeometryCacheFile requires a stream which can seek.");

            _geomReader = new BinaryReader(_geomStream);

            // is signature ok?
            // 
            char[] signature_ch_arr = _geomReader.ReadChars(18);
            string read = new string(signature_ch_arr);
            if (read != FILESIGNATURE)
                throw new Exception("GeometryCacheFile wrong signature found.");

            // is file complete?
            _geomStream.Seek(offset_FileLenght, SeekOrigin.Begin);
            long expectedLenght = _geomReader.ReadInt64();
            long streamlenght = _geomStream.Length;
            if (expectedLenght != streamlenght)
                throw new Exception("GeometryCacheFile wrong lenght of cache file found.");

            // get the position dictionary:
            _geomStream.Seek(offset_PositionsSectionAddress, SeekOrigin.Begin);
            long dictionaryPosition = _geomReader.ReadInt64();
            _geomStream.Seek(dictionaryPosition, SeekOrigin.Begin);
            int dictionaryLenght = _geomReader.ReadInt32();

            _positions = new Dictionary<long, long>(dictionaryLenght);
            for (int count = 0; count < dictionaryLenght; count++)
            {
                long key = _geomReader.ReadInt64();
                long value = _geomReader.ReadInt64();
                _positions.Add(key, value);
            }

            _isInitialised = true;
        }

        public int GetMeshCount()
        {
            if (!_isInitialised)
                return 0;
            return _positions.Count();
        }

        public bool HasGeometryFor(long EntityLabel)
        {
            if (!_isInitialised)
                return false;
            return _positions.ContainsKey(EntityLabel);
        }

        public SpaceTriangulatedModel GetTriangulatedModel(long EntityLabel)
        {
            EntityLabel = Math.Abs(EntityLabel);
            if (!HasGeometryFor(EntityLabel))
                return null;

            SpaceTriangulatedModel ret = new SpaceTriangulatedModel();
            ret.EntityLabel = EntityLabel;
            long startingPosition = _positions[EntityLabel];
            _geomReader.BaseStream.Seek(startingPosition, SeekOrigin.Begin);

            ret.Matrix = ReadMatrix(_geomReader);
            ret.Mesh = ReadTriangulatedModelAtCurrentPosition(0);
            return ret;
        }

        private static Matrix3D ReadMatrix(BinaryReader br)
        {
            // if starts with 0 then it's an identity matrix
            byte bType = br.ReadByte();
            if (bType == 0)
                return Matrix3D.Identity;

            Matrix3D m = new Matrix3D();
            m.M11 = br.ReadSingle();
            m.M12 = br.ReadSingle();
            m.M13 = br.ReadSingle();
            m.M14 = br.ReadSingle();
            m.M21 = br.ReadSingle();
            m.M22 = br.ReadSingle();
            m.M23 = br.ReadSingle();
            m.M24 = br.ReadSingle();
            m.M31 = br.ReadSingle();
            m.M32 = br.ReadSingle();
            m.M33 = br.ReadSingle();
            m.M34 = br.ReadSingle();
            m.OffsetX = br.ReadSingle();
            m.OffsetY = br.ReadSingle();
            m.OffsetZ = br.ReadSingle();
            m.M44 = br.ReadSingle();
            return m;
        }

        private MeshGeometry ReadTriangulatedModelAtCurrentPosition(int indent)
        {
            MeshGeometry mesh = new MeshGeometry();

            int iPositionsCount = _geomReader.ReadInt32();

            for (int iPos = 0; iPos < iPositionsCount; iPos++)
            {
                mesh.Positions.Add(
                    new Point3D(
                        _geomReader.ReadSingle(),
                        _geomReader.ReadSingle(),
                        _geomReader.ReadSingle())
                    );
            }

            int iIndexCount = _geomReader.ReadInt32();
            for (int iIdx = 0; iIdx < iIndexCount; iIdx++)
            {
                mesh.TriangleIndices.Add(
                    _geomReader.ReadInt32()
                    );
            }

            int iNormalsCount = _geomReader.ReadInt32();
            for (int iNrm = 0; iNrm < iNormalsCount; iNrm++)
            {
                mesh.Normals.Add(
                    new Vector3D(
                        _geomReader.ReadSingle(),
                        _geomReader.ReadSingle(),
                        _geomReader.ReadSingle())
                    );
            }

            int iChildrenCount = _geomReader.ReadInt32();

            Debug.Write(new string(' ', indent*2));
            Debug.WriteLine("Pos/Nrm: " + iPositionsCount + " Idx: " + iIndexCount + " Chld: " + iChildrenCount);

            for (int iChld = 0; iChld < iChildrenCount; iChld++)
            {
                mesh.AddChild(ReadTriangulatedModelAtCurrentPosition(indent + 1));
            }
            return mesh;
        }


        public void Close()
        {
            if (_isInitialised)
            {
                _geomReader.BaseStream.Close();
                _geomReader.Close();
            }
            _geomReader = null;
            _isInitialised = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
            _geomReader = null;
        }

        #endregion

        public void AddPosition(long EntityLabel, long Position)
        {
            _positions.Add(EntityLabel, Position);
        }

        public void WritePositions(BinaryWriter _geomWriter)
        {
            _geomWriter.Write(_positions.Count);
            foreach (var pos in _positions)
            {
                _geomWriter.Write(pos.Key);
                _geomWriter.Write(pos.Value);
            }
        }

        public long[] EntityLabels
        {
            get { return _positions.Keys.ToArray(); }
        }

        public void Init(BinaryWriter GeomWriter)
        {
            GeomWriter.Write(FILESIGNATURE.ToCharArray());

            if (GeomWriter.BaseStream.Position != offset_FileLenght)
                throw new Exception("Error in address settings");
            GeomWriter.Write(0L); // placeholder for file lenght (useful for quick integrity check)

            if (GeomWriter.BaseStream.Position != offset_PositionsSectionAddress)
                throw new Exception("Error in address settings");
            GeomWriter.Write(0L); // placeholder for _positions address 
        }

        public void FillHeader(long indexStart, long FileLenght, BinaryWriter _geomWriter)
        {
            // now complete information in the header.
            _geomWriter.BaseStream.Seek(offset_PositionsSectionAddress, SeekOrigin.Begin);
            _geomWriter.Write(indexStart);

            _geomWriter.BaseStream.Seek(offset_FileLenght, SeekOrigin.Begin);
            _geomWriter.Write(FileLenght);
        }
    }
}