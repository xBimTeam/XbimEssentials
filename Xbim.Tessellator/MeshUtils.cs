/*
** SGI FREE SOFTWARE LICENSE B (Version 2.0, Sept. 18, 2008) 
** Copyright (C) 2011 Silicon Graphics, Inc.
** All Rights Reserved.
**
** Permission is hereby granted, free of charge, to any person obtaining a copy
** of this software and associated documentation files (the "Software"), to deal
** in the Software without restriction, including without limitation the rights
** to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
** of the Software, and to permit persons to whom the Software is furnished to do so,
** subject to the following conditions:
** 
** The above copyright notice including the dates of first publication and either this
** permission notice or a reference to http://oss.sgi.com/projects/FreeB/ shall be
** included in all copies or substantial portions of the Software. 
**
** THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
** INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
** PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL SILICON GRAPHICS, INC.
** BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
** TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
** OR OTHER DEALINGS IN THE SOFTWARE.
** 
** Except as contained in this notice, the name of Silicon Graphics, Inc. shall not
** be used in advertising or otherwise to promote the sale, use or other dealings in
** this Software without prior written authorization from Silicon Graphics, Inc.
*/
/*
** Original Author: Eric Veach, July 1994.
** libtess2: Mikko Mononen, http://code.google.com/p/libtess2/.
** LibTessDotNet: Remi Gillig, https://github.com/speps/LibTessDotNet
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xbim.Tessellator
{
    public struct Vec3EqualityComparer : IEqualityComparer<Vec3>
    {
        private readonly float _precision2;
        private readonly float _gridDim;
        public Vec3EqualityComparer(float precision)
        {
            _precision2 = precision * precision;
            _gridDim = precision*10;//coursen  up
        }
        public bool Equals(Vec3 a, Vec3 b)
        {
            Vec3 c;
            Vec3.Sub(ref a,ref b, out c);
            bool same = c.Length2 <= _precision2;
            
            return same;
        }

        public int GetHashCode(Vec3 point)
        {
            
			//This hashcode snaps points to a grid of 10 * tolerance to ensure similar points fall into the same hash cell
            double xs = point.X - point.X % _gridDim;
            double ys = point.Y - point.Y % _gridDim;
            double zs = point.Z - point.Z % _gridDim;
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ xs.GetHashCode();
                hash = hash * 16777619 ^ ys.GetHashCode();
                hash = hash * 16777619 ^ zs.GetHashCode();
                return hash;
            }
        }
    }

    public struct Vec3Comparer : IComparer<Vec3>
    {
        public int Compare(Vec3 a, Vec3 b)
        {
            if (a.X > b.X ) return 1;
            if (a.X == b.X  && a.Y > b.Y) return 1;
            if (a.X == b.X && a.Y == b.Y && a.Z > b.Z) return 1;
            if (a.X == b.X && a.Y == b.Y && a.Z == b.Z) return 0;
            return -1;
        }
    }

    public struct Vec3 
    {
        public readonly static Vec3 Zero = new Vec3();

        public double X, Y, Z;


        public static bool Colinear( Vec3 a,  Vec3 b,  Vec3 c)
        {
            Vec3 ab;
            Vec3 bc;
            Sub(ref a, ref b, out ab);
            Sub(ref b, ref c, out bc);
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return ab.X * bc.Y == bc.X * ab.Y &&
                   ab.Y * bc.Z == bc.Y * ab.Z &&
                   ab.Z * bc.X == bc.Z * ab.X;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(double x, double y, double z)
        {
            X = (float)x;
            Y = (float)y;
            Z = (float)z;
        }

        public double this[int index]
        {
            get
            {
                if (index == 0) return X;
                if (index == 1) return Y;
                if (index == 2) return Z;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else if (index == 2) Z = value;
                else throw new IndexOutOfRangeException();
            }
        }

        public static void Sub(ref Vec3 lhs, ref Vec3 rhs, out Vec3 result)
        {
            result.X = lhs.X - rhs.X;
            result.Y = lhs.Y - rhs.Y;
            result.Z = lhs.Z - rhs.Z;
        }

        public static void AddTo(ref Vec3 lhs, ref Vec3 rhs)
        {
            lhs.X += rhs.X;
            lhs.Y += rhs.Y;
            lhs.Z += rhs.Z;
        }

        public bool IsValid
        {
            get { return Length2 > 0; }
        }
        public static double Angle(ref Vec3 v1, ref Vec3 v2)
        {
            double cosinus;
            Dot(ref v1, ref v2, out cosinus);
            if (cosinus > -0.70710678118655 && cosinus < 0.70710678118655)
                return Math.Acos(cosinus);
            Vec3 v3;
            Cross(ref v1, ref v2, out v3);
            var sinus = v3.Length;
            if (cosinus < 0.0) return Math.PI - Math.Asin(sinus);
            return Math.Asin(sinus);
        }
        public static void Neg(ref Vec3 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
        }

        public float Length
        {
            get
            {
                return (float) Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public double Length2
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }
        public static void Dot(ref Vec3 u, ref Vec3 v, out double dot)
        {
            dot = u.X * v.X + u.Y * v.Y + u.Z * v.Z;
        }

        public static void Cross(ref Vec3 u, ref Vec3 v, out Vec3 cross)
        {
            cross.X = u.Y * v.Z - u.Z * v.Y;
            cross.Y = u.Z * v.X - u.X * v.Z;
            cross.Z = u.X * v.Y - u.Y * v.X;
        }

        public static bool Normalize(ref Vec3 v)
        {
            double len = v.X * v.X + v.Y * v.Y + v.Z * v.Z;
            if(len <= 0.0f) return false;
            len = 1.0f / (float)Math.Sqrt(len);
            v.X *= len;
            v.Y *= len;
            v.Z *= len;
            return true;
        }
        public static int LongAxis(ref Vec3 v)
        {
            int i = 0;
            if (Math.Abs(v.Y) > Math.Abs(v.X)) i = 1;
            if (Math.Abs(v.Z) > Math.Abs(i == 0 ? v.X : v.Y)) i = 2;
            return i;
        }
    }

    internal static class MeshUtils
    {
        public const int Undef = ~0;

        public class Vertex
        {
// ReSharper disable InconsistentNaming
            internal Vertex _prev, _next;
            internal Edge _anEdge;
            internal Vec3 _coords;
            internal double _s, _t;
            internal PqHandle _pqHandle;
            internal int _n;
            internal int _data;
// ReSharper restore InconsistentNaming
        }

        public class Face
        {
// ReSharper disable InconsistentNaming
            internal Face _prev, _next;

            internal Edge _anEdge;

            internal Face _trail;
            internal int _n;
            internal bool _marked, _inside;
            // ReSharper restore InconsistentNaming
            internal int VertsCount
            {
                get
                {
                    int n = 0;
                    var eCur = _anEdge;
                    do {
                        n++;
                        eCur = eCur._Lnext;
                    } while (eCur != _anEdge);
                    return n;
                }
            }
        }

        public struct EdgePair
        {
// ReSharper disable InconsistentNaming
            internal Edge _e, _eSym;
// ReSharper restore InconsistentNaming

            public static EdgePair Create()
            {
                var pair = new EdgePair();
                pair._e = new Edge();
                pair._e._pair = pair;
                pair._eSym = new Edge();
                pair._eSym._pair = pair;
                return pair;
            }
        }

        public class Edge
        {
// ReSharper disable InconsistentNaming
            internal EdgePair _pair;
            internal Edge _next, _Sym, _Onext, _Lnext;
            internal Vertex _Org;
            internal Face _Lface;
            internal Tess.ActiveRegion _activeRegion;
            internal int _winding;
// ReSharper restore InconsistentNaming

            internal Face Rface { get { return _Sym._Lface; } set { _Sym._Lface = value; } }
            internal Vertex Dst { get { return _Sym._Org; }  set { _Sym._Org = value; } }

            internal Edge Oprev { get { return _Sym._Lnext; } set { _Sym._Lnext = value; } }
            internal Edge Lprev { get { return _Onext._Sym; } set { _Onext._Sym = value; } }
            internal Edge Dprev { get { return _Lnext._Sym; } set { _Lnext._Sym = value; } }
            internal Edge Rprev { get { return _Sym._Onext; } set { _Sym._Onext = value; } }
            internal Edge Dnext { get { return Rprev._Sym; } set { Rprev._Sym = value; } }
            internal Edge Rnext { get { return Oprev._Sym; } set { Oprev._Sym = value; } }

            internal Edge() { }

            internal static void EnsureFirst(ref Edge e)
            {
                if (e == e._pair._eSym)
                {
                    e = e._Sym;
                }
            }
        }

        /// <summary>
        /// MakeEdge creates a new pair of half-edges which form their own loop.
        /// No vertex or face structures are allocated, but these must be assigned
        /// before the current edge operation is completed.
        /// </summary>
        public static Edge MakeEdge(Edge eNext)
        {
            Debug.Assert(eNext != null);

            var pair = EdgePair.Create();
            var e = pair._e;
            var eSym = pair._eSym;

            // Make sure eNext points to the first edge of the edge pair
            Edge.EnsureFirst(ref eNext);

            // Insert in circular doubly-linked list before eNext.
            // Note that the prev pointer is stored in Sym->next.
            var ePrev = eNext._Sym._next;
            eSym._next = ePrev;
            ePrev._Sym._next = e;
            e._next = eNext;
            eNext._Sym._next = eSym;

            e._Sym = eSym;
            e._Onext = e;
            e._Lnext = eSym;
            e._Org = null;
            e._Lface = null;
            e._winding = 0;
            e._activeRegion = null;

            eSym._Sym = e;
            eSym._Onext = eSym;
            eSym._Lnext = e;
            eSym._Org = null;
            eSym._Lface = null;
            eSym._winding = 0;
            eSym._activeRegion = null;

            return e;
        }

        /// <summary>
        /// Splice( a, b ) is best described by the Guibas/Stolfi paper or the
        /// CS348a notes (see Mesh.cs). Basically it modifies the mesh so that
        /// a->Onext and b->Onext are exchanged. This can have various effects
        /// depending on whether a and b belong to different face or vertex rings.
        /// For more explanation see Mesh.Splice().
        /// </summary>
        public static void Splice(Edge a, Edge b)
        {
            var aOnext = a._Onext;
            var bOnext = b._Onext;

            aOnext._Sym._Lnext = b;
            bOnext._Sym._Lnext = a;
            a._Onext = bOnext;
            b._Onext = aOnext;
        }

        /// <summary>
        /// MakeVertex( newVertex, eOrig, vNext ) attaches a new vertex and makes it the
        /// origin of all edges in the vertex loop to which eOrig belongs. "vNext" gives
        /// a place to insert the new vertex in the global vertex list. We insert
        /// the new vertex *before* vNext so that algorithms which walk the vertex
        /// list will not see the newly created vertices.
        /// </summary>
        public static void MakeVertex(Vertex vNew, Edge eOrig, Vertex vNext)
        {
            Debug.Assert(vNew != null);

            // insert in circular doubly-linked list before vNext
            var vPrev = vNext._prev;
            vNew._prev = vPrev;
            vPrev._next = vNew;
            vNew._next = vNext;
            vNext._prev = vNew;

            vNew._anEdge = eOrig;
            // leave coords, s, t undefined

            // fix other edges on this vertex loop
            var e = eOrig;
            do {
                e._Org = vNew;
                e = e._Onext;
            } while (e != eOrig);
        }

        /// <summary>
        /// MakeFace( newFace, eOrig, fNext ) attaches a new face and makes it the left
        /// face of all edges in the face loop to which eOrig belongs. "fNext" gives
        /// a place to insert the new face in the global face list. We insert
        /// the new face *before* fNext so that algorithms which walk the face
        /// list will not see the newly created faces.
        /// </summary>
        public static void MakeFace(Face fNew, Edge eOrig, Face fNext)
        {
            Debug.Assert(fNew != null);

            // insert in circular doubly-linked list before fNext
            var fPrev = fNext._prev;
            fNew._prev = fPrev;
            fPrev._next = fNew;
            fNew._next = fNext;
            fNext._prev = fNew;

            fNew._anEdge = eOrig;
            fNew._trail = null;
            fNew._marked = false;

            // The new face is marked "inside" if the old one was. This is a
            // convenience for the common case where a face has been split in two.
            fNew._inside = fNext._inside;

            // fix other edges on this face loop
            var e = eOrig;
            do {
                e._Lface = fNew;
                e = e._Lnext;
            } while (e != eOrig);
        }

        /// <summary>
        /// KillEdge( eDel ) destroys an edge (the half-edges eDel and eDel->Sym),
        /// and removes from the global edge list.
        /// </summary>
        public static void KillEdge(Edge eDel)
        {
            // Half-edges are allocated in pairs, see EdgePair above
            Edge.EnsureFirst(ref eDel);

            // delete from circular doubly-linked list
            var eNext = eDel._next;
            var ePrev = eDel._Sym._next;
            eNext._Sym._next = ePrev;
            ePrev._Sym._next = eNext;
        }

        /// <summary>
        /// KillVertex( vDel ) destroys a vertex and removes it from the global
        /// vertex list. It updates the vertex loop to point to a given new vertex.
        /// </summary>
        public static void KillVertex(Vertex vDel, Vertex newOrg)
        {
            var eStart = vDel._anEdge;

            // change the origin of all affected edges
            var e = eStart;
            do {
                e._Org = newOrg;
                e = e._Onext;
            } while (e != eStart);

            // delete from circular doubly-linked list
            var vPrev = vDel._prev;
            var vNext = vDel._next;
            vNext._prev = vPrev;
            vPrev._next = vNext;
        }

        /// <summary>
        /// KillFace( fDel ) destroys a face and removes it from the global face
        /// list. It updates the face loop to point to a given new face.
        /// </summary>
        public static void KillFace(Face fDel, Face newLFace)
        {
            var eStart = fDel._anEdge;

            // change the left face of all affected edges
            var e = eStart;
            do {
                e._Lface = newLFace;
                e = e._Lnext;
            } while (e != eStart);

            // delete from circular doubly-linked list
            var fPrev = fDel._prev;
            var fNext = fDel._next;
            fNext._prev = fPrev;
            fPrev._next = fNext;
        }
    }
}
