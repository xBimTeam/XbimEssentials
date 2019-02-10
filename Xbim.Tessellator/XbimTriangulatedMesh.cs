using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Geometry;
using Xbim.Tessellator;

namespace Xbim.Tessellator
{


    public class XbimTriangulatedMesh
    {
        public struct  XbimTriangle
        {
            readonly XbimContourVertexCollection _vertices;
            readonly XbimTriangleEdge[] _edges;

            internal XbimTriangle(XbimTriangleEdge[] edges, XbimContourVertexCollection vertices)
            {
                _vertices = vertices;
                _edges = edges;
            }

            public bool IsEmpty
            {
                get { return _edges == null; }
            }

            public XbimVector3D Normal
            {
                get
                {
                    var p1 = _vertices[_edges[0].StartVertexIndex].Position;
                    var p2 = _vertices[_edges[0].NextEdge.StartVertexIndex].Position;
                    var p3 = _vertices[_edges[0].NextEdge.NextEdge.StartVertexIndex].Position;
                    var a = new XbimPoint3D(p1.X, p1.Y, p1.Z);
                    var b = new XbimPoint3D(p2.X, p2.Y, p2.Z);
                    var c = new XbimPoint3D(p3.X, p3.Y, p3.Z);
                    var cv = XbimVector3D.CrossProduct(b - a, c - a);
                    cv=cv.Normalized();
                    return cv;
                } 
            }
            public XbimPackedNormal PackedNormal
            {
                get
                {
                    return new XbimPackedNormal(Normal);
                }
            }
        }

        private readonly Dictionary<long, XbimTriangleEdge[]> _lookupList;
        private readonly List<XbimTriangleEdge[]> _faultyTriangles = new List<XbimTriangleEdge[]>();
        private Dictionary<int, List<XbimTriangleEdge[]>> _faces;
        private readonly XbimContourVertexCollection _vertices;
        
        double _minX = double.PositiveInfinity;
        double _minY = double.PositiveInfinity;
        double _minZ = double.PositiveInfinity;
        double _maxX = double.NegativeInfinity;
        double _maxY = double.NegativeInfinity;
        double _maxZ = double.NegativeInfinity;
       
        public XbimTriangulatedMesh(int faceCount, float precision)
        {
            var edgeCount = (int)(faceCount * 1.5);
            _lookupList = new Dictionary<long, XbimTriangleEdge[]>(edgeCount);
            _faces = new Dictionary<int, List<XbimTriangleEdge[]>>(faceCount);
            _vertices = new XbimContourVertexCollection(precision);
        }

        public uint TriangleCount
        {
            get
            {
                uint triangleCount = 0;
                foreach (var face in _faces.Values)
                    triangleCount += (uint)face.Count;
                return triangleCount;
            }
        }
        public IEnumerable<XbimTriangle> Triangles
        {
            get 
            {
                return from edgeListList in _faces.Values 
                       from edges in edgeListList 
                       select new XbimTriangle(edges, _vertices);
            }
        }
        public List<XbimTriangleEdge[]> FaultyTriangles
        {
            get { return _faultyTriangles; }
        }

        /// <summary>
        /// Returns the normal of the triangle that contains the specified edge
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public XbimVector3D TriangleNormal(XbimTriangleEdge edge)
        {
            var p1 = _vertices[edge.StartVertexIndex].Position;
            var p2 = _vertices[edge.NextEdge.StartVertexIndex].Position;
            var p3 = _vertices[edge.NextEdge.NextEdge.StartVertexIndex].Position;    
            var a = new XbimPoint3D(p1.X,p1.Y,p1.Z); 
            var b = new XbimPoint3D(p2.X,p2.Y,p2.Z); 
            var c = new XbimPoint3D(p3.X,p3.Y,p3.Z);
            var cv = XbimVector3D.CrossProduct(b - a, c - a );
            cv=cv.Normalized();
            return cv;
        }

        public Dictionary<int, List<XbimTriangleEdge[]>> Faces
        {
            get
            {
                return _faces;
            }
        }

       
        private bool AddEdge(XbimTriangleEdge edge)
        {
           
            var key = edge.Key;
            if (!_lookupList.ContainsKey(key))
            {
                var arr = new XbimTriangleEdge[2];
                arr[0] = edge;
                _lookupList[key] = arr;
            }
            else
            {
                var edges = _lookupList[key];
                if (edges[1] != null)
                    return false; //we already have a pair
                edges[1] = edge;
                edges[0].AdjacentEdge = edge;
                edge.AdjacentEdge = edges[0];
            }
           
            return true;
        }

        delegate bool IsMaxDelegate(ContourVertex p);
        /// <summary>
        /// Orientates edges to orientate in a uniform direction
        /// </summary>
        /// <returns></returns>
        public void UnifyFaceOrientation(int entityLabel)
        {
            XbimTriangleEdge[] extremeTriangle = FindExtremeTriangle();
            if (extremeTriangle == null) return;
            if (!IsFacingOutward(extremeTriangle[0]))
                extremeTriangle[0].Reverse();
            var triangles = new List<XbimTriangleEdge[]>
            {
                extremeTriangle
            };
            extremeTriangle[0].Freeze();

            do
            {
                triangles = UnifyConnectedTriangles(triangles);
            } while (triangles.Any());

            //doing the extreme edge first should do all connected

            foreach (var xbimEdges in _faces.Values.SelectMany(el => el).Where(e => !e[0].Frozen)) //check any rogue elements
            {
                if (!IsFacingOutward(xbimEdges[0]))
                    xbimEdges[0].Reverse();
                triangles = new List<XbimTriangleEdge[]> { new[] { xbimEdges[0], xbimEdges[0].NextEdge, xbimEdges[0].NextEdge.NextEdge } };
                xbimEdges[0].Freeze();
                do
                {
                    triangles = UnifyConnectedTriangles(triangles);
                } while (triangles.Any());

            }
            BalanceNormals();
        }

        private XbimTriangleEdge[] FindExtremeTriangle()
        {
            //find the biggest
            var sizeX = _maxX - _minX;
            var sizeY = _maxY - _minY;
            var sizeZ = _maxZ - _minZ;

            IsMaxDelegate isMax;
            if (sizeX >= sizeY && sizeX >= sizeZ) isMax = p => Math.Abs(p.Position.X - _maxX) < 1e-9;
            else if (sizeY >= sizeX && sizeY >= sizeZ) isMax = p => Math.Abs(p.Position.Y - _maxY) < 1e-9;
            else isMax = p => Math.Abs(p.Position.Z - _maxZ) < 1e-9;
           
            foreach (var face in _faces.Values)
            {
                //find the extreme triangle
                foreach (var t in face)
                {
                    foreach (var edge in t)
                    {
                        if (isMax(_vertices[edge.StartVertexIndex])
                            && !Vec3.Colinear(_vertices[edge.StartVertexIndex].Position, _vertices[edge.NextEdge.StartVertexIndex].Position, _vertices[edge.NextEdge.NextEdge.StartVertexIndex].Position))
                        {
                            return t;
                        }
                    }
                }                
            }

            return null;
        }

        //public void SmoothNormals()
        //{
            
        //    foreach(var triangle in Triangles)
        //    {
        //        // ...
        //        // p1, p2 and p3 are the points in the face (f)
        //        var p1 = triangle.

        //        // calculate facet normal of the triangle using cross product;
        //        // both components are "normalized" against a common point chosen as the base
        //        float3 n = (p2 - p1).Cross(p3 - p1);    // p1 is the 'base' here

        //        // get the angle between the two other points for each point;
        //        // the starting point will be the 'base' and the two adjacent points will be normalized against it
        //        a1 = (p2 - p1).Angle(p3 - p1);    // p1 is the 'base' here
        //        a2 = (p3 - p2).Angle(p1 - p2);    // p2 is the 'base' here
        //        a3 = (p1 - p3).Angle(p2 - p3);    // p3 is the 'base' here

        //        // normalize the initial facet normals if you want to ignore surface area
        //        if (!area_weighting)
        //        {
        //            normalize(n);
        //        }

        //        // store the weighted normal in an structured array
        //        v1.wnormals.push_back(n * a1);
        //        v2.wnormals.push_back(n * a2);
        //        v3.wnormals.push_back(n * a3);
        //    }
        //    for (int v = 0; v < vertcount; v++)
        //    {
        //        float3 N;

        //        // run through the normals in each vertex's array and interpolate them
        //        // vertex(v) here fetches the data of the vertex at index 'v'
        //        for (int n = 0; n < vertex(v).wnormals.size(); v++)
        //        {
        //            N += vertex(v).wnormals.at(n);
        //        }

        //        // normalize the final normal
        //        normalize(N);
        //    }
        //}

        public void BalanceNormals(double minAngle = Math.PI / 5)
        {
            //set up the base normals
            foreach (var faceGroup in Faces)
            {
                foreach (var triangle in faceGroup.Value)
                {
                    ComputeTriangleNormal(triangle);
                }
            }


            var edgesAtVertex = _faces.Values.SelectMany(el => el).SelectMany(e => e).Where(e => e != null).GroupBy(k => k.StartVertexIndex);
            foreach (var edges in edgesAtVertex)
            {               
                //create a set of faces to divide the point into a set of connected faces               
                var faceSet = new List<List<XbimTriangleEdge>>();//the first face set at this point

                //find an unconnected edge if one exists
                var unconnectedEdges = edges.Where(e => e.AdjacentEdge == null);
                var freeEdges = unconnectedEdges as IList<XbimTriangleEdge> ?? unconnectedEdges.ToList();

                if (!freeEdges.Any())
                //they are all connected to each other so find the first sharp edge or the any one if none sharp, this stops a face being split
                {
                    XbimTriangleEdge nextConnectedEdge = edges.First();
                    freeEdges = new List<XbimTriangleEdge>(1) { edges.First() }; //take the first if we don't find a sharp edge
                    //now look for any connected edges 
                    var visited = new HashSet<long>();
                    do
                    {
                        visited.Add(nextConnectedEdge.EdgeId);
                        nextConnectedEdge = nextConnectedEdge.NextEdge.NextEdge.AdjacentEdge;
                        if (nextConnectedEdge != null)
                        {
                            if (visited.Contains(nextConnectedEdge.EdgeId)) //we have been here before
                            {
                                break; //we are looping or at the start  
                            }
                            //if the edge is sharp and the triangle is not colinear, start here
                            var nextAngle = nextConnectedEdge.Angle;
                            if (nextAngle > minAngle && nextConnectedEdge.Normal.IsValid)
                            {
                                freeEdges = new List<XbimTriangleEdge>(1) { nextConnectedEdge };
                                break;
                            }

                        }
                    } while (nextConnectedEdge != null);
                }

                foreach (var edge in freeEdges)
                {
                    var face = new List<XbimTriangleEdge> { edge };
                    faceSet.Add(face);
                    XbimTriangleEdge nextConnectedEdge = edge;
                    //now look for any connected edges 
                    var visited = new HashSet<long>();
                    do
                    {
                        visited.Add(nextConnectedEdge.EdgeId);
                        var nextConnectedEdgeCandidate = nextConnectedEdge.NextEdge.NextEdge.AdjacentEdge;
                        while (nextConnectedEdgeCandidate != null && !visited.Contains(nextConnectedEdgeCandidate.EdgeId) && !nextConnectedEdgeCandidate.Normal.IsValid) //skip colinear triangles
                        {
                            //set the colinear triangle to have the same normals as the current edge
                            nextConnectedEdgeCandidate.Normal = nextConnectedEdge.Normal;
                            nextConnectedEdgeCandidate.NextEdge.Normal = nextConnectedEdge.Normal;
                            nextConnectedEdgeCandidate.NextEdge.NextEdge.Normal = nextConnectedEdge.Normal;
                            visited.Add(nextConnectedEdgeCandidate.EdgeId);
                            nextConnectedEdgeCandidate = nextConnectedEdgeCandidate.NextEdge.NextEdge.AdjacentEdge;
                        }

                        nextConnectedEdge = nextConnectedEdgeCandidate;
                        if (nextConnectedEdge != null)
                        {
                            if (visited.Contains(nextConnectedEdge.EdgeId))
                                break; //we are looping or at the start                       
                            //if the edge is sharp start a new face
                            var angle = nextConnectedEdge.Angle;                      
                            if ( angle > minAngle && nextConnectedEdge.Normal.IsValid)
                            {
                                face = new List<XbimTriangleEdge>();
                                faceSet.Add(face);
                            }
                            face.Add(nextConnectedEdge);
                        }
                    } while (nextConnectedEdge != null);
                    //move on to next face

                }        

                //we have our smoothing groups
                foreach (var vertexEdges in faceSet.Where(f => f.Count > 1))
                {
                    var vertexNormal = Vec3.Zero;
                    foreach (var edge in vertexEdges)
                    {
                        if (edge.Normal.IsValid)
                        {
                            Vec3.AddTo(ref vertexNormal, ref edge.Normal);
                        }                      
                    }

                    Vec3.Normalize(ref vertexNormal);
                    foreach (var edge in vertexEdges)
                         edge.Normal = vertexNormal;                   
                }


            }
           
            //now regroup faces
            _faces = _faces.Values.SelectMany(v => v).GroupBy(t=>ComputeTrianglePackedNormalInt(t)).ToDictionary(k=>k.Key,v=>v.ToList());
           
        }




        private List<XbimTriangleEdge[]> UnifyConnectedTriangles(List<XbimTriangleEdge[]> triangles)
        {
            var nextCandidates = new List<XbimTriangleEdge[]>();
            foreach (var triangle in triangles)
            {
                foreach (var edge in triangle)
                {
                    var adjacentEdge = edge.AdjacentEdge;
                    
                    if (adjacentEdge != null) //if we just have one it is a boundary
                    {
                        var adjacentTriangle = new[] {adjacentEdge, adjacentEdge.NextEdge, adjacentEdge.NextEdge.NextEdge};
                        if (adjacentEdge.EdgeId == edge.EdgeId) //they both face the same way
                        {
                            if (!adjacentEdge.Frozen)
                            {
                                adjacentEdge.Reverse(); //will reverse the entire triangle
                            }
                            else //we cannot align the edges correctly so break the connection
                            {
                                edge.AdjacentEdge = null;
                                adjacentEdge.AdjacentEdge = null; 
                                //Xbim3DModelContext.Logger.WarnFormat("Invalid triangle orientation has been ignored in entity #{0}", entityLabel);
                            }
                          
                        }
                        
                        if (!adjacentEdge.Frozen)
                        {
                            adjacentEdge.Freeze();
                            nextCandidates.Add(adjacentTriangle);
                        } 
                    }
                   
                }
            }
            return nextCandidates;
        }

        /// <summary>
        /// Adds the triangle using the three ints as inidices into the vertext collection
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="faceId"></param>
        public void AddTriangle(int p1, int p2, int p3, int faceId)
        {
            var e1 = new XbimTriangleEdge(p1);
            var e2 = new XbimTriangleEdge(p2);
            var e3 = new XbimTriangleEdge(p3);
            e1.NextEdge = e2;
            e2.NextEdge = e3;
            e3.NextEdge = e1;
     
            var edgeList = new[] { e1, e2, e3 };
            bool faulty = !AddEdge(e1);
            if (!faulty && !AddEdge(e2))
            {
                RemoveEdge(e1);
                faulty = true;
            }
            if (!faulty && !AddEdge(e3))
            {
                RemoveEdge(e1);
                RemoveEdge(e2);
                faulty = true;
            }
            if (faulty) 
                FaultyTriangles.Add(edgeList);
            List<XbimTriangleEdge[]> triangleList;
            if (!_faces.TryGetValue(faceId, out triangleList))
            {
                triangleList = new List<XbimTriangleEdge[]>();
                _faces.Add(faceId, triangleList);
            }
            triangleList.Add(edgeList);
        }

        /// <summary>
        /// Computes the packed normal for the triangle, if all the normals at each vertex are the same it is returned, if any are different XbimPackedNormal.Invalid is returned. Assumes the normals have been calculated and balanced
        /// </summary>
        /// <param name="edges"></param>
        private int ComputeTrianglePackedNormalInt(XbimTriangleEdge[] edges)
        {
            var pn = edges[0].PackedNormal;
            var pn0 = pn.ToUnit16();
            var pn1 = edges[1].PackedNormal.ToUnit16();
            var pn2 = edges[2].PackedNormal.ToUnit16();
            if (pn0 == pn1 && pn1 == pn2) return pn.ToUnit16();
            return ushort.MaxValue;
        }

        /// <summary>
        /// Calculates the normal for a connected triangle edge, assumes the edge is part of a complete triangle and there are 3 triangle edges
        /// </summary>
        public bool ComputeTriangleNormal(XbimTriangleEdge[] edges)
        {
            var p1 = _vertices[edges[0].StartVertexIndex].Position;
            var p2 = _vertices[edges[0].NextEdge.StartVertexIndex].Position;
            var p3 = _vertices[edges[0].NextEdge.NextEdge.StartVertexIndex].Position;

            var ax = p1.X; var bx = p2.X; var cx = p3.X;
            var ay = p1.Y; var by = p2.Y; var cy = p3.Y;
            var az = p1.Z; var bz = p2.Z; var cz = p3.Z;
            // calculate normal of a triangle
            var v = new Vec3(
                            (by - ay) * (cz - az) - (bz - az) * (cy - ay),
                            (bz - az) * (cx - ax) - (bx - ax) * (cz - az),
                            (bx - ax) * (cy - ay) - (by - ay) * (cx - ax)
                );            
            if (Vec3.Normalize(ref v))
            {
                edges[0].Normal = v;
                edges[1].Normal = v;
                edges[2].Normal = v;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an edge from the edge list
        /// </summary>
        /// <param name="edge"></param>
        private void RemoveEdge(XbimTriangleEdge edge)
        {
            var edges = _lookupList[edge.Key];
            if (edges[0] == edge) //if it is the first one 
            {
                if (edges[1] == null) //and there is no second one
                    _lookupList.Remove(edge.Key); //remove the entire key
                else
                    edges[0] = edges[1]; //keep the second one
            }
            if (edges[1] == edge) //if it is the second one just remove it and leave the first
                edges[1] = null;
        }

        /// <summary>
        /// Insets a vertex and returns the position in the list, removes duplicates
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int AddVertex(Vec3 v)
        {
            if (!_vertices.Contains(v))
            {
                var pos = _vertices.Count;
                _vertices.Add(v);
                _minX = Math.Min(_minX, v.X);
                _minY = Math.Min(_minY, v.Y);
                _minZ = Math.Min(_minZ, v.Z);
                _maxX = Math.Max(_maxX, v.X);
                _maxY = Math.Max(_maxY, v.Y);
                _maxZ = Math.Max(_maxZ, v.Z);
                return pos;
            }
            else
                return _vertices[v].Data;
        }

        /// <summary>
        /// Attempt to optimise the vertices in the triangulation.
        /// If the vertex is already present, it will be reuesed.
        /// </summary>
        /// <param name="v">The vertex to search</param>
        /// <param name="contourVertex">The reference vertex to populate</param>
        public void AddVertex(Vec3 v, ref ContourVertex contourVertex)
        {
            if (_vertices.Contains(v)) 
                contourVertex = _vertices[v];
            else
            {
                _vertices.Add(v, ref contourVertex);
                _minX = Math.Min(_minX, v.X);
                _minY = Math.Min(_minY, v.Y);
                _minZ = Math.Min(_minZ, v.Z);
                _maxX = Math.Max(_maxX, v.X);
                _maxY = Math.Max(_maxY, v.Y);
                _maxZ = Math.Max(_maxZ, v.Z);
            }
        }

        public uint VertexCount
        {
            get { return (uint)_vertices.Count; }
        }

        public IEnumerable<Vec3> Vertices
        {
            get { return _vertices.Select(c => c.Position); }
        }


        public XbimRect3D BoundingBox
        {
            get { return new XbimRect3D(_minX, _minY, _minZ, _maxX - _minX, _maxY - _minY, _maxZ - _minZ); }
        }

        public XbimPoint3D Centroid
        {
            get { return BoundingBox.Centroid(); }
        }

        public XbimVector3D PointingOutwardFrom(XbimPoint3D point3D)
        {
            var v = point3D - Centroid;
            v = v.Normalized();
            return v;
        }

        /// <summary>
        /// Returns true if the triangle that contains the edge is facing away from the centroid of the mesh
        /// </summary>
        /// <returns></returns>
        public bool IsFacingOutward(XbimTriangleEdge edge)
        {
            //find the centroid of the triangle
            var p1 = _vertices[edge.StartVertexIndex].Position;
            var p2 = _vertices[edge.NextEdge.StartVertexIndex].Position;
            var p3 = _vertices[edge.NextEdge.NextEdge.StartVertexIndex].Position;
            var centroid = new XbimPoint3D((p1.X + p2.X + p3.X) / 3, (p1.Y + p2.Y + p3.Y) / 3, (p1.Z + p2.Z + p3.Z) / 3);
            var normal = TriangleNormal(edge);            
            var vecOut = PointingOutwardFrom(centroid);
            var dot = vecOut.DotProduct(normal);
            return dot > 0;
        }

    }

}

/// <summary>
/// Edge class for triangular meshes only
/// </summary>
public class XbimTriangleEdge
{
    public int StartVertexIndex;
    public XbimTriangleEdge NextEdge;
    public XbimTriangleEdge AdjacentEdge;
    public Vec3 Normal;
    private bool _frozen;
    public int EndVertexIndex { get { return NextEdge.StartVertexIndex; } }
    public XbimTriangleEdge(int p1)
    {
        StartVertexIndex = p1;
    }

    public bool Frozen
    {
        get { return _frozen; }
        
    }

    /// <summary>
    /// Returns the angle of this edge, 0 if the edge has no adjacent edge or the the normals are invalid, returns -1 if invalid
    /// </summary>
    public double Angle
    {
        get
        {
            
            if (AdjacentEdge!=null && Normal.IsValid && AdjacentEdge.NextEdge.Normal.IsValid)
                return Vec3.Angle(ref Normal, ref AdjacentEdge.NextEdge.Normal);
            return 0;
        }    
        
    }


    public void Freeze()
    {
        _frozen = true;
        NextEdge._frozen=true;
        NextEdge.NextEdge._frozen = true;
    }


    public void Reverse()
    {
        if (!_frozen)
        { 
            var p1 = StartVertexIndex;
            var p2 = NextEdge.StartVertexIndex;
            var p3 = NextEdge.NextEdge.StartVertexIndex;
            StartVertexIndex = p2;
            NextEdge.StartVertexIndex = p3;
            NextEdge.NextEdge.StartVertexIndex = p1;
            var prevEdge = NextEdge.NextEdge;
            prevEdge.NextEdge = NextEdge;
            NextEdge.NextEdge = this;
            NextEdge = prevEdge;
        }
       
    }

    /// <summary>
    /// The ID of the edge, unique for all edges between vertices
    /// </summary>
    public long EdgeId
    {
        get
        {
            long a = StartVertexIndex;
            a <<= 32;
            return (a | (uint)EndVertexIndex);
        }
    }

    public bool IsEmpty { get { return EdgeId == 0; } }

    /// <summary>
    /// The key for the edge, this is the same for both directions of an  edge
    /// </summary>
    public long Key
    {
        get
        {
            long left = Math.Max(StartVertexIndex, EndVertexIndex);
            left <<= 32;
            long right = Math.Min(StartVertexIndex, EndVertexIndex);
            return (left | right);
        }
    }

    public XbimPackedNormal PackedNormal
    {
        get
        {
            return new XbimPackedNormal(Normal.X,Normal.Y,Normal.Z);
        }
    }
}
