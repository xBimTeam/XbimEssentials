using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc4.MeasureResource;
using Xbim.Common.Collections;

namespace Xbim.Tessellator
{

    public class XbimTessellator
    {
        private readonly IModel _model;
        private readonly XbimGeometryType _geometryType;
        public XbimTessellator(IModel model, XbimGeometryType geometryType)
        {
            _model = model;
            _geometryType = geometryType;
        }

        public IXbimShapeGeometryData Mesh(IXbimGeometryObject geomObject)
        {
            return new XbimShapeGeometry();

        }

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
            if (cfs != null) return Mesh(cfs);
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
            return Mesh(faceSets, faceBasedModel.EntityLabel, (float)faceBasedModel.Model.ModelFactors.Precision);
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
            return Mesh(shells, entityLabel, precision);
        }

        public XbimShapeGeometry Mesh(IIfcConnectedFaceSet connectedFaceSet)
        {
            var faces = new List<IList<IIfcFace>>();
            faces.Add(connectedFaceSet.CfsFaces.ToList());
            return Mesh(faces, connectedFaceSet.EntityLabel, (float)connectedFaceSet.Model.ModelFactors.Precision);
        }

        public XbimShapeGeometry Mesh(IIfcFacetedBrep fBRepModel)
        {
            return Mesh(fBRepModel.Outer);
        }

        public XbimShapeGeometry Mesh(IEnumerable<IList<IIfcFace>> facesList, int entityLabel, float precision)
        {
            if (_geometryType == XbimGeometryType.PolyhedronBinary)
                return MeshPolyhedronBinary(facesList, entityLabel, precision);
            if (_geometryType == XbimGeometryType.Polyhedron)
                return MeshPolyhedronText(facesList, entityLabel, precision);
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
            return Mesh(faces, tess.EntityLabel, (float)tess.Model.ModelFactors.Precision);
        }

        private XbimShapeGeometry MeshPolyhedronBinary(IIfcTriangulatedFaceSet triangulation)
        {

            XbimShapeGeometry shapeGeometry = new XbimShapeGeometry();
            shapeGeometry.Format = XbimGeometryType.PolyhedronBinary;

            using (var ms = new MemoryStream(0x4000))
            using (var binaryWriter = new BinaryWriter(ms))
            {

                // Prepare the header
                uint verticesCount = (uint)triangulation.Coordinates.CoordList.Count();
                uint triangleCount = (uint)triangulation.CoordIndex.Count();
                uint facesCount = 1;//at the moment just write one face for all triangles, may change to support styed faces in future
                shapeGeometry.BoundingBox = XbimRect3D.Empty;

                //Write out the header
                binaryWriter.Write((byte)1); //stream format version			
                // ReSharper disable once RedundantCast

                //now write out the faces
                if (triangulation.Normals.Any()) //we have normals so obey them
                {
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
                    binaryWriter.Write(verticesCount); //number of vertices
                    binaryWriter.Write(triangleCount); //number of triangles

                    XbimRect3D bb = XbimRect3D.Empty;
                    foreach (var coordList in triangulation.Coordinates.CoordList)
                    {
                        var pt = coordList.AsTriplet();
                        binaryWriter.Write((float)pt.A);
                        binaryWriter.Write((float)pt.B);
                        binaryWriter.Write((float)pt.C);
                        var rect = new XbimRect3D(pt.A, pt.B, pt.C, 0, 0, 0);
                        bb.Union(rect);
                    }

                    binaryWriter.Write(facesCount);

                    shapeGeometry.BoundingBox = bb;
                    Int32 numTrianglesInFace = triangulation.CoordIndex.Count();
                    binaryWriter.Write(-numTrianglesInFace); //not a planar face so make negative 
                    var packedNormals = new List<XbimPackedNormal>(triangulation.Normals.Count());
                    foreach (var normal in triangulation.Normals)
                    {
                        var tpl = normal.AsTriplet<IfcParameterValue>();
                        packedNormals.Add(new XbimPackedNormal(tpl.A, tpl.B, tpl.C));
                    }




                    int triangleIndex = 0;

                    foreach (var triangle in triangulation.CoordIndex)
                    {
                        var triangleTpl = triangle.AsTriplet();
                        var normalsIndexTpl = normalIndex[triangleIndex].AsTriplet();


                        WriteIndex(binaryWriter, (uint)triangleTpl.A - 1, (uint)verticesCount);
                        packedNormals[(int)normalsIndexTpl.A - 1].Write(binaryWriter);

                        WriteIndex(binaryWriter, (uint)triangleTpl.B - 1, (uint)verticesCount);
                        packedNormals[(int)normalsIndexTpl.B - 1].Write(binaryWriter);

                        WriteIndex(binaryWriter, (uint)triangleTpl.C - 1, (uint)verticesCount);
                        packedNormals[(int)normalsIndexTpl.C - 1].Write(binaryWriter);
                        triangleIndex++;
                    }

                }
                else //we need to calculate normals to get a better surface fit
                {
                    var triangulatedMesh = Triangulate(triangulation);
                    shapeGeometry.BoundingBox = triangulatedMesh.BoundingBox;
                    verticesCount = triangulatedMesh.VertexCount;
                    triangleCount = triangulatedMesh.TriangleCount;

                    binaryWriter.Write(verticesCount); //number of vertices
                    binaryWriter.Write(triangleCount); //number of triangles

                    foreach (var vert in triangulatedMesh.Vertices)
                    {
                        binaryWriter.Write((float)vert.X);
                        binaryWriter.Write((float)vert.Y);
                        binaryWriter.Write((float)vert.Z);
                    }
                    facesCount = (uint)triangulatedMesh.Faces.Count;
                    binaryWriter.Write((UInt32)facesCount);
                    foreach (var faceGroup in triangulatedMesh.Faces)
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
                            WriteIndex(binaryWriter, (uint)triangle[0].StartVertexIndex, verticesCount);
                            if (!planar) triangle[0].PackedNormal.Write(binaryWriter);

                            WriteIndex(binaryWriter, (uint)triangle[0].NextEdge.StartVertexIndex, verticesCount);
                            if (!planar) triangle[0].NextEdge.PackedNormal.Write(binaryWriter);

                            WriteIndex(binaryWriter, (uint)triangle[0].NextEdge.NextEdge.StartVertexIndex, verticesCount);
                            if (!planar) triangle[0].NextEdge.NextEdge.PackedNormal.Write(binaryWriter);
                        }
                    }
                }
                binaryWriter.Flush();
                ((IXbimShapeGeometryData)shapeGeometry).ShapeData = ms.ToArray();
            }
            return shapeGeometry;

        }


        private XbimShapeGeometry MeshPolyhedronText(IEnumerable<IList<IIfcFace>> facesList, int entityLabel, float precision)
        {
            var shapeGeometry = new XbimShapeGeometry();
            shapeGeometry.Format = XbimGeometryType.Polyhedron;
            using (var ms = new MemoryStream(0x4000))
            using (TextWriter textWriter = new StreamWriter(ms))
            {
                var faceLists = facesList.ToList();
                var triangulations = new List<XbimTriangulatedMesh>(faceLists.Count);
                foreach (var faceList in faceLists)
                    triangulations.Add(TriangulateFaces(faceList, entityLabel, precision));

                // Write out header
                uint verticesCount = 0;
                uint triangleCount = 0;
                uint facesCount = 0;
                var boundingBox = XbimRect3D.Empty;
                foreach (var triangulatedMesh in triangulations)
                {
                    verticesCount += triangulatedMesh.VertexCount;
                    triangleCount += triangulatedMesh.TriangleCount;
                    facesCount += (uint)triangulatedMesh.Faces.Count;
                    if (boundingBox.IsEmpty) boundingBox = triangulatedMesh.BoundingBox;
                    else boundingBox.Union(triangulatedMesh.BoundingBox);
                }

                textWriter.WriteLine("P {0} {1} {2} {3} {4}", 2, verticesCount, facesCount, triangleCount, 0);
                //write out vertices and normals  
                textWriter.Write("V");

                foreach (var p in triangulations.SelectMany(t => t.Vertices))
                    textWriter.Write(" {0},{1},{2}", p.X, p.Y, p.Z);

                textWriter.WriteLine();

                //now write out the faces
                uint verticesOffset = 0;
                foreach (var triangulatedMesh in triangulations)
                {
                    foreach (var faceGroup in triangulatedMesh.Faces)
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
                    verticesOffset += triangulatedMesh.VertexCount;
                }
                textWriter.Flush();
                shapeGeometry.BoundingBox = boundingBox;
                ((IXbimShapeGeometryData)shapeGeometry).ShapeData = ms.ToArray();
            }
            return shapeGeometry;
        }

        private XbimShapeGeometry MeshPolyhedronBinary(IEnumerable<IList<IIfcFace>> facesList, int entityLabel, float precision)
        {
            XbimShapeGeometry shapeGeometry = new XbimShapeGeometry();
            shapeGeometry.Format = XbimGeometryType.PolyhedronBinary;

            using (var ms = new MemoryStream(0x4000))
            using (var binaryWriter = new BinaryWriter(ms))
            {
                var faceLists = facesList.ToList();
                var triangulatedMeshes = new List<XbimTriangulatedMesh>(faceLists.Count);
                foreach (var faceList in faceLists)
                {
                    triangulatedMeshes.Add(TriangulateFaces(faceList, entityLabel, precision));
                }

                // Write out header
                uint verticesCount = 0;
                uint triangleCount = 0;
                uint facesCount = 0;
                var boundingBox = XbimRect3D.Empty;
                foreach (var triangulatedMesh in triangulatedMeshes)
                {
                    verticesCount += triangulatedMesh.VertexCount;
                    triangleCount += triangulatedMesh.TriangleCount;
                    facesCount += (uint)triangulatedMesh.Faces.Count;
                    if (boundingBox.IsEmpty)
                        boundingBox = triangulatedMesh.BoundingBox;
                    else
                        boundingBox.Union(triangulatedMesh.BoundingBox);
                }

                binaryWriter.Write((byte)1); //stream format version			
                                             // ReSharper disable once RedundantCast
                binaryWriter.Write((UInt32)verticesCount); //number of vertices
                binaryWriter.Write(triangleCount); //number of triangles

                foreach (var v in triangulatedMeshes.SelectMany(t => t.Vertices))
                {
                    binaryWriter.Write((float)v.X);
                    binaryWriter.Write((float)v.Y);
                    binaryWriter.Write((float)v.Z);
                }
                shapeGeometry.BoundingBox = boundingBox;
                //now write out the faces

                binaryWriter.Write(facesCount);
                uint verticesOffset = 0;
                int invalidNormal = ushort.MaxValue;
                foreach (var triangulatedMesh in triangulatedMeshes)
                {
                    foreach (var faceGroup in triangulatedMesh.Faces)
                    {
                        var numTrianglesInFace = faceGroup.Value.Count;
                        //we need to fix this
                        var planar = invalidNormal != faceGroup.Key; //we have a mesh of faces that all have the same normals at their vertices
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
                            if (!planar) triangle[0].NextEdge.PackedNormal.Write(binaryWriter);
                            WriteIndex(binaryWriter, (uint)triangle[0].NextEdge.NextEdge.StartVertexIndex + verticesOffset,
                                verticesCount);
                            if (!planar) triangle[0].NextEdge.NextEdge.PackedNormal.Write(binaryWriter);
                        }
                    }
                    verticesOffset += triangulatedMesh.VertexCount;
                }
                binaryWriter.Flush();
                ((IXbimShapeGeometryData)shapeGeometry).ShapeData = ms.ToArray();
            }
            return shapeGeometry;
        }

        private XbimTriangulatedMesh TriangulateFaces(IList<IIfcFace> ifcFaces, int entityLabel, float precision)
        {
            var faceId = 0;

            var faceCount = ifcFaces.Count;
            var triangulatedMesh = new XbimTriangulatedMesh(faceCount, precision);
            foreach (var ifcFace in ifcFaces)
            {

                //improves performance and reduces memory load
                var tess = new Tess();

                var contours = new List<ContourVertex[]>(/*Count?*/);
                foreach (var bound in ifcFace.Bounds) //build all the loops
                {
                    var polyLoop = bound.Bound as IIfcPolyLoop;

                    if (polyLoop == null) continue; //skip empty faces
                    var polygon = polyLoop.Polygon;

                    if (polygon.Count < 3) continue; //skip non-polygonal faces
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
                    if (contours.Count == 1 && contours[0].Length == 3) //its a triangle just grab it
                    {
                        triangulatedMesh.AddTriangle(contours[0][0].Data, contours[0][1].Data, contours[0][2].Data, faceId);
                        faceId++;
                    }
                    //else 
                    //if (contours.Count == 1 && contours[0].Length == 4) //its a quad just grab it
                    //{
                    //    foreach (var v in contours[0])
                    //    {
                    //        Console.WriteLine("{0:F4} ,{1:F4}, {2:F4}", v.Position.X, v.Position.Y, v.Position.Z);

                    //    }
                    //    Console.WriteLine("");
                    //    triangulatedMesh.AddTriangle(contours[0][0].Data, contours[0][1].Data, contours[0][3].Data, faceId);
                    //    triangulatedMesh.AddTriangle(contours[0][3].Data, contours[0][1].Data, contours[0][2].Data, faceId);
                    //    faceId++;
                    //}
                    else    //it is multi-sided and may have holes
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

            triangulatedMesh.UnifyFaceOrientation(entityLabel);
            return triangulatedMesh;
        }


        private XbimTriangulatedMesh Triangulate(IIfcTriangulatedFaceSet triangulation)
        {
            var faceId = 0;
            var entityLabel = triangulation.EntityLabel;
            var precision = (float)triangulation.Model.ModelFactors.Precision;
            var faceCount = triangulation.CoordIndex.Count();
            var triangulatedMesh = new XbimTriangulatedMesh(faceCount, precision);
            //add all the vertices in to the mesh
            var vertices = new List<int>(triangulation.Coordinates.CoordList.Count());
            foreach (var coord in triangulation.Coordinates.CoordList)
            {
                var tpl = coord.AsTriplet<IfcLengthMeasure>();
                var v = new Vec3(tpl.A, tpl.B, tpl.C);
                var idx = triangulatedMesh.AddVertex(v);
                vertices.Add(idx);
            }
            foreach (var triangleFace in triangulation.CoordIndex)
            {
                var tpl = triangleFace.AsTriplet<IfcPositiveInteger>();
                triangulatedMesh.AddTriangle(vertices[(int)tpl.A - 1], vertices[(int)tpl.B - 1], vertices[(int)tpl.C - 1], faceId);
            }
            triangulatedMesh.UnifyFaceOrientation(entityLabel);
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