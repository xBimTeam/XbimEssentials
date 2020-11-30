using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc4.MeasureResource;
using Microsoft.Extensions.Logging;

namespace Xbim.Tessellator
{
    public class XbimTessellator
    {
        private readonly IModel _model;
        private readonly XbimGeometryType _geometryType;
        private readonly ILogger _logger;        

        public XbimTessellator(IModel model, XbimGeometryType geometryType, ILogger logger = null)
        {
            _model = model;
            _geometryType = geometryType;
            _logger = logger;
        }

        /// <summary>
        /// Issue reporting mask. Default reports only open bodies.
        /// </summary>
        public XbimTriangulationStatus ReportMask { get; set; } = XbimTriangulationStatus.IsOpenBody;

        /// <summary>
        /// Returns true if the object can be meshed by the tesselator, if it cannot create an IXbimGeometryObject
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool CanMesh(object shape)
        {
            return
                shape is IIfcFaceBasedSurfaceModel ||
                shape is IIfcShellBasedSurfaceModel ||
                shape is IIfcConnectedFaceSet ||
                shape is IIfcTessellatedFaceSet ||
                shape is IIfcFacetedBrep;
        }

        public XbimShapeGeometry Mesh(IIfcRepresentationItem shape)
        {
            var fbm = shape as IIfcFaceBasedSurfaceModel;
            if (fbm != null) return Mesh(fbm);
            var sbm = shape as IIfcShellBasedSurfaceModel;
            if (sbm != null) return Mesh(sbm);
            var cfs = shape as IIfcConnectedFaceSet;
            if (cfs != null) return Mesh(cfs, false);
            var fbr = shape as IIfcFacetedBrep;
            if (fbr != null) return Mesh(fbr);
            var tfs = shape as IIfcTriangulatedFaceSet;
            if (tfs != null) return Mesh(tfs);
            var pfs = shape as IIfcPolygonalFaceSet;
            if (pfs != null) return Mesh(pfs);
            throw new ArgumentException("Unsupported representation type for tessellation, " + shape.GetType().Name);
        }

        public XbimShapeGeometry Mesh(IIfcFaceBasedSurfaceModel faceBasedModel)
        {
            var faceSets = new List<IList<IIfcFace>>();
            foreach (var faceSet in faceBasedModel.FbsmFaces)
                faceSets.Add(faceSet.CfsFaces.ToList());
            return Mesh(faceSets, faceBasedModel.EntityLabel, (float)faceBasedModel.Model.ModelFactors.Precision, false);
        }

        public XbimShapeGeometry Mesh(IIfcShellBasedSurfaceModel shellBasedModel)
        {
            return Mesh(shellBasedModel.SbsmBoundary, shellBasedModel.EntityLabel, (float)shellBasedModel.Model.ModelFactors.Precision);
        }

        public XbimShapeGeometry Mesh(IEnumerable<IIfcShell> shellSet, int entityLabel, float precision)
        {
            var shells = new List<IList<IIfcFace>>();
            foreach (var shell in shellSet)
            {
                var closedShell = shell as IIfcClosedShell;
                var openShell = shell as IIfcOpenShell;
                if (closedShell != null) shells.Add(closedShell.CfsFaces.ToList());
                else if (openShell != null) shells.Add(openShell.CfsFaces.ToList());
            }
            return Mesh(shells, entityLabel, precision, false);
        }

        public XbimShapeGeometry Mesh(IIfcConnectedFaceSet connectedFaceSet, bool isIntentiallyClosed)
        {
            var faces = new List<IList<IIfcFace>>();
            faces.Add(connectedFaceSet.CfsFaces.ToList());
            return Mesh(faces, connectedFaceSet.EntityLabel, (float)connectedFaceSet.Model.ModelFactors.Precision, isIntentiallyClosed);
        }

        public XbimShapeGeometry Mesh(IIfcFacetedBrep fBRepModel)
        {
            return Mesh(fBRepModel.Outer, true);
        }

        public XbimShapeGeometry Mesh(IEnumerable<IList<IIfcFace>> facesList, int entityLabel, float precision, bool isIntentiallyClosed)
        {
            if (_geometryType == XbimGeometryType.PolyhedronBinary)
                return MeshPolyhedronBinary(facesList, entityLabel, precision, isIntentiallyClosed);
            if (_geometryType == XbimGeometryType.Polyhedron)
                return MeshPolyhedronText(facesList, entityLabel, precision, isIntentiallyClosed);
            throw new Exception("Illegal Geometry type, " + _geometryType);
        }

        public XbimShapeGeometry Mesh(IIfcTriangulatedFaceSet triangulation)
        {
            if (_geometryType == XbimGeometryType.PolyhedronBinary)
                return MeshPolyhedronBinary(triangulation);
            if (_geometryType == XbimGeometryType.Polyhedron)
                return MeshPolyhedronText(triangulation);
            throw new Exception("Illegal Geometry type, " + _geometryType);
        }

        public XbimShapeGeometry Mesh(IIfcPolygonalFaceSet triangulation)
        {
            if (_geometryType == XbimGeometryType.PolyhedronBinary)
                return MeshPolyhedronBinary(triangulation);
            if (_geometryType == XbimGeometryType.Polyhedron)
                return MeshPolyhedronText(triangulation);
            throw new Exception("Illegal Geometry type, " + _geometryType);
        }

        private XbimShapeGeometry MeshPolyhedronText(IIfcTriangulatedFaceSet triangulation)
        {
            throw new NotImplementedException();
        }

        private XbimShapeGeometry MeshPolyhedronText(IIfcPolygonalFaceSet triangulation)
        {
            throw new NotImplementedException();
        }

        private XbimShapeGeometry MeshPolyhedronBinary(IIfcPolygonalFaceSet tess)
        {
            var faces = new List<IList<IIfcFace>>();
            faces.Add(new XbimPolygonalFaceSet(tess));
            return Mesh(faces, tess.EntityLabel, (float)tess.Model.ModelFactors.Precision, tess.Closed ?? false);
        }

        private XbimShapeGeometry MeshPolyhedronBinary(IIfcTriangulatedFaceSet triangulation)
        {
            var mesh = Triangulate(triangulation);
            return ToBinaryShapeGeometry(mesh);
        }

        private XbimShapeGeometry ToBinaryShapeGeometry(params XbimTriangulatedMesh[] meshes)
        {
            XbimShapeGeometry shapeGeometry = new XbimShapeGeometry();
            shapeGeometry.Format = XbimGeometryType.PolyhedronBinary;

            using (var ms = new MemoryStream(0x4000))
            using (var binaryWriter = new BinaryWriter(ms))
            {
                // Write out header
                uint verticesCount = 0;
                uint triangleCount = 0;
                uint facesCount = 0;
                var boundingBox = XbimRect3D.Empty;
                bool isAllVolumeDefined = true;

                foreach (var mesh in meshes)
                {
                    verticesCount += mesh.VertexCount;
                    triangleCount += mesh.TriangleCount;
                    facesCount += (uint)mesh.Faces.Count;
                    shapeGeometry.Volume += mesh.Volume;
                    isAllVolumeDefined &= mesh.Volume.HasValue;

                    if (boundingBox.IsEmpty)
                        boundingBox = mesh.BoundingBox;
                    else
                        boundingBox.Union(mesh.BoundingBox);
                }

                if (!isAllVolumeDefined)
                    // Reset, if there are non-closed bodies within the collection
                    shapeGeometry.Volume = null;

                binaryWriter.Write((byte)1); //stream format version			
                                             // ReSharper disable once RedundantCast
                binaryWriter.Write((UInt32)verticesCount); //number of vertices
                binaryWriter.Write(triangleCount); //number of triangles

                // use minimum bbox as a local origin
                var origin = boundingBox.Min;
                var isLarge = IsLarge(origin.X) || IsLarge(origin.Y) || IsLarge(origin.Z);

                var vertices = isLarge ?
                    meshes.SelectMany(t => t.Vertices).Select(v => new Vec3(v.X - origin.X, v.Y - origin.Y, v.Z - origin.Z)) :
                    meshes.SelectMany(t => t.Vertices);

                foreach (var v in vertices)
                {
                    binaryWriter.Write((float)v.X);
                    binaryWriter.Write((float)v.Y);
                    binaryWriter.Write((float)v.Z);
                }

                if (isLarge)
                {
                    var bb = boundingBox;
                    shapeGeometry.BoundingBox = new XbimRect3D(bb.X - origin.X, bb.Y - origin.Y, bb.Z - origin.Z, bb.SizeX, bb.SizeY, bb.SizeZ);
                    shapeGeometry.LocalShapeDisplacement = new XbimVector3D(origin.X, origin.Y, origin.Z);
                }
                else
                {
                    shapeGeometry.BoundingBox = boundingBox;
                }

                // Write faces
                binaryWriter.Write(facesCount);
                uint verticesOffset = 0;
                foreach (var mesh in meshes)
                {
                    foreach (var faceGroup in mesh.Faces)
                    {
                        var numTrianglesInFace = faceGroup.Value.Count;
                        //we need to fix this
                        var planar = (ushort.MaxValue != faceGroup.Key); //we have a mesh of faces that all have the same normals at their vertices
                        if (!planar) numTrianglesInFace *= -1; //set flag to say multiple normals

                        // ReSharper disable once RedundantCast
                        binaryWriter.Write((Int32)numTrianglesInFace);

                        bool first = true;
                        foreach (var triangle in faceGroup.Value)
                        {
                            if (planar && first)
                            {
                                triangle[0].PackedNormal.Write(binaryWriter);
                                first = false;
                            }
                            WriteIndex(binaryWriter, (uint)triangle[0].StartVertexIndex + verticesOffset, verticesCount);
                            if (!planar)
                                triangle[0].PackedNormal.Write(binaryWriter);
                            WriteIndex(binaryWriter, (uint)triangle[0].NextEdge.StartVertexIndex + verticesOffset, verticesCount);
                            if (!planar) 
                                triangle[0].NextEdge.PackedNormal.Write(binaryWriter);
                            WriteIndex(binaryWriter, (uint)triangle[0].NextEdge.NextEdge.StartVertexIndex + verticesOffset, verticesCount);
                            if (!planar) 
                                triangle[0].NextEdge.NextEdge.PackedNormal.Write(binaryWriter);
                        }
                    }
                    verticesOffset += mesh.VertexCount;
                }
                binaryWriter.Flush();
                ((IXbimShapeGeometryData)shapeGeometry).ShapeData = ms.ToArray();
            }
            return shapeGeometry;
        }

        private XbimShapeGeometry ToTextShapeGeometry(params XbimTriangulatedMesh[] meshes)
        {
            var shapeGeometry = new XbimShapeGeometry();
            shapeGeometry.Format = XbimGeometryType.Polyhedron;

            using (var ms = new MemoryStream(0x4000))
            using (TextWriter textWriter = new StreamWriter(ms))
            {
                // Write out header
                uint verticesCount = 0;
                uint triangleCount = 0;
                uint facesCount = 0;
                var boundingBox = XbimRect3D.Empty;
                bool isAllVolumeDefined = true;

                foreach (var mesh in meshes)
                {
                    verticesCount += mesh.VertexCount;
                    triangleCount += mesh.TriangleCount;
                    facesCount += (uint)mesh.Faces.Count;
                    shapeGeometry.Volume += mesh.Volume;
                    isAllVolumeDefined &= mesh.Volume.HasValue;

                    if (boundingBox.IsEmpty)
                        boundingBox = mesh.BoundingBox;
                    else
                        boundingBox.Union(mesh.BoundingBox);
                }

                if (!isAllVolumeDefined)
                    // Reset, if there are non-closed bodies within the collection
                    shapeGeometry.Volume = null;

                textWriter.WriteLine("P {0} {1} {2} {3} {4}", 2, verticesCount, facesCount, triangleCount, 0);
                //write out vertices and normals  
                textWriter.Write("V");

                foreach (var p in meshes.SelectMany(t => t.Vertices))
                    textWriter.Write(" {0},{1},{2}", p.X, p.Y, p.Z);

                textWriter.WriteLine();

                //now write out the faces
                uint verticesOffset = 0;
                foreach (var mesh in meshes)
                {
                    foreach (var faceGroup in mesh.Faces)
                    {
                        textWriter.Write("T");
                        int currentNormal = -1;
                        foreach (var triangle in faceGroup.Value)
                        {
                            var pn1 = triangle[0].PackedNormal.ToUnit16();
                            var pn2 = triangle[0].NextEdge.PackedNormal.ToUnit16();
                            var pn3 = triangle[0].NextEdge.NextEdge.PackedNormal.ToUnit16();
                            if (pn1 != currentNormal)
                            {
                                textWriter.Write(" {0}/{1},", triangle[0].StartVertexIndex + verticesOffset, pn1);
                                currentNormal = pn1;
                            }
                            else
                                textWriter.Write(" {0},", triangle[0].StartVertexIndex + verticesOffset);

                            if (pn1 != pn2)
                            {
                                textWriter.Write("{0}/{1},", triangle[0].NextEdge.StartVertexIndex + verticesOffset, pn2);
                                currentNormal = pn2;
                            }
                            else
                                textWriter.Write("{0},", triangle[0].NextEdge.StartVertexIndex + verticesOffset);
                            if (pn2 != pn3)
                            {
                                textWriter.Write("{0}/{1}", triangle[0].NextEdge.NextEdge.StartVertexIndex + verticesOffset, pn3);
                                currentNormal = pn3;
                            }
                            else
                                textWriter.Write("{0}", triangle[0].NextEdge.NextEdge.StartVertexIndex + verticesOffset);
                        }
                        textWriter.WriteLine();
                    }
                    verticesOffset += mesh.VertexCount;
                }
                textWriter.Flush();
                shapeGeometry.BoundingBox = boundingBox;
                ((IXbimShapeGeometryData)shapeGeometry).ShapeData = ms.ToArray();
            }
            return shapeGeometry;

        }

        private bool IsLarge(double coordinate)
        {
            return coordinate > _model.ModelFactors.OneMilliMeter * 999999;
        }

        private XbimShapeGeometry MeshPolyhedronText(IEnumerable<IList<IIfcFace>> facesList, int entityLabel, float precision, bool isIntentionallyClosed)
        {
            var meshes = new XbimTriangulatedMesh[facesList.Count()];
            int index = 0;
            foreach (var faceList in facesList)
            {
                meshes[index] = TriangulateFaces(faceList, entityLabel, precision, isIntentionallyClosed);
                index++;
            }

            return ToTextShapeGeometry(meshes);
        }

        private XbimShapeGeometry MeshPolyhedronBinary(IEnumerable<IList<IIfcFace>> facesList, int entityLabel, float precision, bool isIntentionallyClosed)
        {
            var meshes = new XbimTriangulatedMesh[facesList.Count()];
            int index = 0;
            foreach (var faceList in facesList)
            {
                meshes[index] = TriangulateFaces(faceList, entityLabel, precision, isIntentionallyClosed);
                index++;
            }
            return ToBinaryShapeGeometry(meshes);
        }

        private XbimTriangulatedMesh TriangulateFaces(IList<IIfcFace> ifcFaces, int entityLabel, float precision, bool isIntentiallyClosed)
        {
            var faceId = 0;

            var faceCount = ifcFaces.Count;
            var triangulatedMesh = new XbimTriangulatedMesh(faceCount, precision);
            foreach (var ifcFace in ifcFaces)
            {
                // improves performance and reduces memory load
                var tess = new Tess();

                var contours = new List<ContourVertex[]>(/*Count?*/);
                foreach (var bound in ifcFace.Bounds) // build all the loops
                {
                    var polyLoop = bound.Bound as IIfcPolyLoop;

                    if (polyLoop == null) continue; // skip empty faces
                    var polygon = polyLoop.Polygon;

                    if (polygon.Count < 3) continue; // skip non-polygonal faces
                    var is3D = (polygon[0].Dim == 3);

                    var contour = new ContourVertex[polygon.Count];
                    if (bound.Orientation)
                    {
                        for (var j = 0; j < polygon.Count; j++)
                        {
                            var v = new Vec3(polygon[j].X, polygon[j].Y, is3D ? polygon[j].Z : 0);
                            triangulatedMesh.AddVertex(v, ref contour[j]);
                        }
                    }
                    else
                    {
                        var i = 0;
                        for (var j = polygon.Count - 1; j >= 0; j--)
                        {
                            var v = new Vec3(polygon[j].X, polygon[j].Y, is3D ? polygon[j].Z : 0);
                            triangulatedMesh.AddVertex(v, ref contour[i]);
                            i++;
                        }
                    }
                    contours.Add(contour);
                }

                if (contours.Any())
                {
                    if (contours.Count == 1 && contours[0].Length == 3) // its a triangle just grab it
                    {
                        triangulatedMesh.AddTriangle(contours[0][0].Data, contours[0][1].Data, contours[0][2].Data, faceId);
                        faceId++;
                    }
                    else  // it is multi-sided and may have holes
                    {
                        tess.AddContours(contours);

                        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
                        var faceIndices = new List<int>(tess.ElementCount * 3);
                        var elements = tess.Elements;
                        var contourVerts = tess.Vertices;
                        for (var j = 0; j < tess.ElementCount * 3; j++)
                        {
                            var idx = contourVerts[elements[j]].Data;
                            if (idx < 0) //WE HAVE INSERTED A POINT
                            {
                                //add it to the mesh
                                triangulatedMesh.AddVertex(contourVerts[elements[j]].Position, ref contourVerts[elements[j]]);
                            }
                            faceIndices.Add(contourVerts[elements[j]].Data);
                        }

                        if (faceIndices.Count > 0)
                        {
                            for (var j = 0; j < tess.ElementCount; j++)
                            {
                                var p1 = faceIndices[j * 3];
                                var p2 = faceIndices[j * 3 + 1];
                                var p3 = faceIndices[j * 3 + 2];
                                triangulatedMesh.AddTriangle(p1, p2, p3, faceId);
                            }
                            faceId++;
                        }
                    }
                }
            }

            var status = triangulatedMesh.UnifyMeshOrientation(isIntentiallyClosed, true);
            ReportStatus(entityLabel, triangulatedMesh.Validate(status));
            
            return triangulatedMesh;
        }

        private void ReportStatus(int entityLabel, XbimTriangulationStatus status)
        {
            if (XbimTriangulationStatus.NoIssues != (status & ReportMask))
            {
                List<string> issueText = new List<string>();

                if (status.HasFlag(XbimTriangulationStatus.IsOpenBody))
                    issueText.Add("has open body");

                if (status.HasFlag(XbimTriangulationStatus.WasInvertedBody))
                    issueText.Add("has been inverted to reflect a positive volume");

                if (status.HasFlag(XbimTriangulationStatus.HasFaultyOrUnconnectedTriangles))
                    issueText.Add("contains unconnected or faulty triangles not displayed");

                _logger.LogWarning("Shape validation result of #{0}: {1}", entityLabel, string.Join(", ", issueText));
            }
        }

        private XbimTriangulatedMesh Triangulate(IIfcTriangulatedFaceSet triangulation)
        {
            // Use a single face only
            const int faceId = 0;

            var precision = (float)triangulation.Model.ModelFactors.Precision;
            var faceCount = triangulation.CoordIndex.Count();
            var triangulatedMesh = new XbimTriangulatedMesh(faceCount, precision);
            // Add all the vertices in to the mesh
            var vertices = new List<int>(triangulation.Coordinates.CoordList.Count());

            // If normals are provided
            List<XbimTriplet<IfcParameterValue>> nTriplets = triangulation.Normals.Select(n => n.AsTriplet<IfcParameterValue>()).ToList();
            List<IEnumerable<IfcPositiveInteger>> normalIndex = new List<IEnumerable<IfcPositiveInteger>>();
            bool hasPnIndex = triangulation.PnIndex.Any();
            if (hasPnIndex)
            {
                if (triangulation.PnIndex is List<IItemSet<IfcPositiveInteger>>) //the list of triplets has not been flattened
                {
                    foreach (var item in triangulation.PnIndex as List<IItemSet<IfcPositiveInteger>>)
                    {
                        normalIndex.Add(item);
                    }
                }
                else
                {
                    for (int i = 0; i < triangulation.PnIndex.Count; i += 3)
                    {
                        var item = new List<IfcPositiveInteger>() { triangulation.PnIndex[i], triangulation.PnIndex[i + 1], triangulation.PnIndex[i + 2] };
                        normalIndex.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in triangulation.CoordIndex)
                {
                    normalIndex.Add(item);
                }
            }

            // Add coordinates
            foreach (var coord in triangulation.Coordinates.CoordList)
            {
                var tpl = coord.AsTriplet<IfcLengthMeasure>();
                var v = new Vec3(tpl.A, tpl.B, tpl.C);
                var idx = triangulatedMesh.AddVertex(v);
                vertices.Add(idx);
            }

            // Create triangles
            int tIndex = 0;
            foreach (var triangleFace in triangulation.CoordIndex)
            {
                var tpl = triangleFace.AsTriplet<IfcPositiveInteger>();
                var edges = triangulatedMesh.AddTriangle(vertices[(int)tpl.A - 1], vertices[(int)tpl.B - 1], vertices[(int)tpl.C - 1], faceId);

                // If there are normals given
                if (nTriplets.Count > 0)
                {
                    var normalsIndexTpl = normalIndex[tIndex].AsTriplet();
                    // Use given normals
                    edges[0].Normal = new Vec3(nTriplets[(int)normalsIndexTpl.A - 1]);
                    edges[1].Normal = new Vec3(nTriplets[(int)normalsIndexTpl.B - 1]);
                    edges[2].Normal = new Vec3(nTriplets[(int)normalsIndexTpl.C - 1]);
                }

                tIndex++;
            }

            // If not marked as closed, don't go for orientation alignment
            XbimTriangulationStatus? status = null;
            if (triangulation.Closed ?? false)
                // It makes no sense to align a mesh which isn't meant to have an orientation
                status = triangulatedMesh.UnifyMeshOrientation(true, nTriplets.Count == 0);
            else
                // So only balance normals (and if not provided already, compute normals)
                triangulatedMesh.BalanceNormals(nTriplets.Count == 0);

            ReportStatus(triangulation.EntityLabel, triangulatedMesh.Validate(status));

            return triangulatedMesh;
        }

        private void WriteIndex(BinaryWriter bw, UInt32 index, UInt32 maxInt)
        {
            if (maxInt <= 0xFF)
                bw.Write((byte)index);
            else if (maxInt <= 0xFFFF)
                bw.Write((UInt16)index);
            else
                bw.Write(index);
        }
    }
}