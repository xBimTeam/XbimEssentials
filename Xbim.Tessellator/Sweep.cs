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
using System.Diagnostics;

namespace Xbim.Tessellator
{
    public partial class Tess
    {
        internal class ActiveRegion
        {
            internal MeshUtils.Edge EUp;
            internal Dict<ActiveRegion>.Node NodeUp;
            internal int WindingNumber;
            internal bool Inside, Sentinel, Dirty, FixUpperEdge;
        }

        private ActiveRegion RegionBelow(ActiveRegion reg)
        {
            return reg.NodeUp._prev._key;
        }

        private ActiveRegion RegionAbove(ActiveRegion reg)
        {
            return reg.NodeUp._next._key;
        }

        /// <summary>
        /// Both edges must be directed from right to left (this is the canonical
        /// direction for the upper edge of each region).
        /// 
        /// The strategy is to evaluate a "t" value for each edge at the
        /// current sweep line position, given by tess->event. The calculations
        /// are designed to be very stable, but of course they are not perfect.
        /// 
        /// Special case: if both edge destinations are at the sweep event,
        /// we sort the edges by slope (they would otherwise compare equally).
        /// </summary>
        private bool EdgeLeq(ActiveRegion reg1, ActiveRegion reg2)
        {
            var e1 = reg1.EUp;
            var e2 = reg2.EUp;

            if (e1.Dst == _event)
            {
                if (e2.Dst == _event)
                {
                    // Two edges right of the sweep line which meet at the sweep event.
                    // Sort them by slope.
                    if (Geom.VertLeq(e1._Org, e2._Org))
                    {
                        return Geom.EdgeSign(e2.Dst, e1._Org, e2._Org) <= 0.0;
                    }
                    return Geom.EdgeSign(e1.Dst, e2._Org, e1._Org) >= 0.0;
                }
                return Geom.EdgeSign(e2.Dst, _event, e2._Org) <= 0.0;
            }
            if (e2.Dst == _event)
            {
                return Geom.EdgeSign(e1.Dst, _event, e1._Org) >= 0.0;
            }

            // General case - compute signed distance *from* e1, e2 to event
            var t1 = Geom.EdgeEval(e1.Dst, _event, e1._Org);
            var t2 = Geom.EdgeEval(e2.Dst, _event, e2._Org);
            return (t1 >= t2);
        }

        private void DeleteRegion(ActiveRegion reg)
        {
            if (reg.FixUpperEdge)
            {
                // It was created with zero winding number, so it better be
                // deleted with zero winding number (ie. it better not get merged
                // with a real edge).
                Debug.Assert(reg.EUp._winding == 0);
            }
            reg.EUp._activeRegion = null;
            _dict.Remove(reg.NodeUp);
        }

        /// <summary>
        /// Replace an upper edge which needs fixing (see ConnectRightVertex).
        /// </summary>
        private void FixUpperEdge(ActiveRegion reg, MeshUtils.Edge newEdge)
        {
            Debug.Assert(reg.FixUpperEdge);
            _mesh.Delete(reg.EUp);
            reg.FixUpperEdge = false;
            reg.EUp = newEdge;
            newEdge._activeRegion = reg;
        }

        private ActiveRegion TopLeftRegion(ActiveRegion reg)
        {
            var org = reg.EUp._Org;

            // Find the region above the uppermost edge with the same origin
            do {
                reg = RegionAbove(reg);
            } while (reg.EUp._Org == org);

            // If the edge above was a temporary edge introduced by ConnectRightVertex,
            // now is the time to fix it.
            if (reg.FixUpperEdge)
            {
                var e = _mesh.Connect(RegionBelow(reg).EUp._Sym, reg.EUp._Lnext);
                FixUpperEdge(reg, e);
                reg = RegionAbove(reg);
            }

            return reg;
        }

        private ActiveRegion TopRightRegion(ActiveRegion reg)
        {
            var dst = reg.EUp.Dst;

            // Find the region above the uppermost edge with the same destination
            do {
                reg = RegionAbove(reg);
            } while (reg.EUp.Dst == dst);

            return reg;
        }

        /// <summary>
        /// Add a new active region to the sweep line, *somewhere* below "regAbove"
        /// (according to where the new edge belongs in the sweep-line dictionary).
        /// The upper edge of the new region will be "eNewUp".
        /// Winding number and "inside" flag are not updated.
        /// </summary>
        private ActiveRegion AddRegionBelow(ActiveRegion regAbove, MeshUtils.Edge eNewUp)
        {
            var regNew = new ActiveRegion();

            regNew.EUp = eNewUp;
            regNew.NodeUp = _dict.InsertBefore(regAbove.NodeUp, regNew);
            regNew.FixUpperEdge = false;
            regNew.Sentinel = false;
            regNew.Dirty = false;

            eNewUp._activeRegion = regNew;

            return regNew;
        }

        private void ComputeWinding(ActiveRegion reg)
        {
            reg.WindingNumber = RegionAbove(reg).WindingNumber + reg.EUp._winding;
            reg.Inside = Geom.IsWindingInside(_windingRule, reg.WindingNumber);
        }

        /// <summary>
        /// Delete a region from the sweep line. This happens when the upper
        /// and lower chains of a region meet (at a vertex on the sweep line).
        /// The "inside" flag is copied to the appropriate mesh face (we could
        /// not do this before -- since the structure of the mesh is always
        /// changing, this face may not have even existed until now).
        /// </summary>
        private void FinishRegion(ActiveRegion reg)
        {
            var e = reg.EUp;
            var f = e._Lface;

            f._inside = reg.Inside;
            f._anEdge = e;
            DeleteRegion(reg);
        }

        /// <summary>
        /// We are given a vertex with one or more left-going edges.  All affected
        /// edges should be in the edge dictionary.  Starting at regFirst->eUp,
        /// we walk down deleting all regions where both edges have the same
        /// origin vOrg.  At the same time we copy the "inside" flag from the
        /// active region to the face, since at this point each face will belong
        /// to at most one region (this was not necessarily true until this point
        /// in the sweep).  The walk stops at the region above regLast; if regLast
        /// is null we walk as far as possible.  At the same time we relink the
        /// mesh if necessary, so that the ordering of edges around vOrg is the
        /// same as in the dictionary.
        /// </summary>
        private MeshUtils.Edge FinishLeftRegions(ActiveRegion regFirst, ActiveRegion regLast)
        {
            var regPrev = regFirst;
            var ePrev = regFirst.EUp;

            while (regPrev != regLast)
            {
                regPrev.FixUpperEdge = false;	// placement was OK
                var reg = RegionBelow(regPrev);
                var e = reg.EUp;
                if (e._Org != ePrev._Org)
                {
                    if (!reg.FixUpperEdge)
                    {
                        // Remove the last left-going edge.  Even though there are no further
                        // edges in the dictionary with this origin, there may be further
                        // such edges in the mesh (if we are adding left edges to a vertex
                        // that has already been processed).  Thus it is important to call
                        // FinishRegion rather than just DeleteRegion.
                        FinishRegion(regPrev);
                        break;
                    }
                    // If the edge below was a temporary edge introduced by
                    // ConnectRightVertex, now is the time to fix it.
                    e = _mesh.Connect(ePrev.Lprev, e._Sym);
                    FixUpperEdge(reg, e);
                }

                // Relink edges so that ePrev.Onext == e
                if (ePrev._Onext != e)
                {
                    _mesh.Splice(e.Oprev, e);
                    _mesh.Splice(ePrev, e);
                }
                FinishRegion(regPrev); // may change reg.eUp
                ePrev = reg.EUp;
                regPrev = reg;
            }

            return ePrev;
        }

        /// <summary>
        /// Purpose: insert right-going edges into the edge dictionary, and update
        /// winding numbers and mesh connectivity appropriately.  All right-going
        /// edges share a common origin vOrg.  Edges are inserted CCW starting at
        /// eFirst; the last edge inserted is eLast.Oprev.  If vOrg has any
        /// left-going edges already processed, then eTopLeft must be the edge
        /// such that an imaginary upward vertical segment from vOrg would be
        /// contained between eTopLeft.Oprev and eTopLeft; otherwise eTopLeft
        /// should be null.
        /// </summary>
        private void AddRightEdges(ActiveRegion regUp, MeshUtils.Edge eFirst, MeshUtils.Edge eLast, MeshUtils.Edge eTopLeft, bool cleanUp)
        {
            bool firstTime = true;

            var e = eFirst; do
            {
                Debug.Assert(Geom.VertLeq(e._Org, e.Dst));
                AddRegionBelow(regUp, e._Sym);
                e = e._Onext;
            } while (e != eLast);

            // Walk *all* right-going edges from e.Org, in the dictionary order,
            // updating the winding numbers of each region, and re-linking the mesh
            // edges to match the dictionary ordering (if necessary).
            if (eTopLeft == null)
            {
                eTopLeft = RegionBelow(regUp).EUp.Rprev;
            }

            ActiveRegion regPrev = regUp, reg;
            var ePrev = eTopLeft;
            while (true)
            {
                reg = RegionBelow(regPrev);
                e = reg.EUp._Sym;
                if (e._Org != ePrev._Org) break;

                if (e._Onext != ePrev)
                {
                    // Unlink e from its current position, and relink below ePrev
                    _mesh.Splice(e.Oprev, e);
                    _mesh.Splice(ePrev.Oprev, e);
                }
                // Compute the winding number and "inside" flag for the new regions
                reg.WindingNumber = regPrev.WindingNumber - e._winding;
                reg.Inside = Geom.IsWindingInside(_windingRule, reg.WindingNumber);

                // Check for two outgoing edges with same slope -- process these
                // before any intersection tests (see example in tessComputeInterior).
                regPrev.Dirty = true;
                if (!firstTime && CheckForRightSplice(regPrev))
                {
                    Geom.AddWinding(e, ePrev);
                    DeleteRegion(regPrev);
                    _mesh.Delete(ePrev);
                }
                firstTime = false;
                regPrev = reg;
                ePrev = e;
            }
            regPrev.Dirty = true;
            Debug.Assert(regPrev.WindingNumber - e._winding == reg.WindingNumber);

            if (cleanUp)
            {
                // Check for intersections between newly adjacent edges.
                WalkDirtyRegions(regPrev);
            }
        }

        /// <summary>
        /// Two vertices with idential coordinates are combined into one.
        /// e1.Org is kept, while e2.Org is discarded.
        /// </summary>
        private void SpliceMergeVertices(MeshUtils.Edge e1, MeshUtils.Edge e2)
        {
            _mesh.Splice(e1, e2);
        }

        /// <summary>
        /// Find some weights which describe how the intersection vertex is
        /// a linear combination of "org" and "dest".  Each of the two edges
        /// which generated "isect" is allocated 50% of the weight; each edge
        /// splits the weight between its org and dst according to the
        /// relative distance to "isect".
        /// </summary>
        private void VertexWeights(MeshUtils.Vertex isect, MeshUtils.Vertex org, MeshUtils.Vertex dst, out double w0, out double w1)
        {
            var t1 = Geom.VertL1Dist(org, isect);
            var t2 = Geom.VertL1Dist(dst, isect);

            w0 = 0.5 * t2 / (t1 + t2);
            w1 = 0.5 * t1 / (t1 + t2);

            isect._coords.X += w0 * org._coords.X + w1 * dst._coords.X;
            isect._coords.Y += w0 * org._coords.Y + w1 * dst._coords.Y;
            isect._coords.Z += w0 * org._coords.Z + w1 * dst._coords.Z;
        }

        /// <summary>
        /// We've computed a new intersection point, now we need a "data" pointer
        /// from the user so that we can refer to this new vertex in the
        /// rendering callbacks.
        /// </summary>
        private void GetIntersectData(MeshUtils.Vertex isect, MeshUtils.Vertex orgUp, MeshUtils.Vertex dstUp, MeshUtils.Vertex orgLo, MeshUtils.Vertex dstLo)
        {
            isect._coords = Vec3.Zero;
            double w0, w1, w2, w3;
            VertexWeights(isect, orgUp, dstUp, out w0, out w1);
            VertexWeights(isect, orgLo, dstLo, out w2, out w3);

            if (_combineCallback != null)
            {
                isect._data = _combineCallback(
                    isect._coords,
                    new[] { orgUp._data, dstUp._data, orgLo._data, dstLo._data },
                    new[] { w0, w1, w2, w3 }
                );
            }
        }

        /// <summary>
        /// Check the upper and lower edge of "regUp", to make sure that the
        /// eUp->Org is above eLo, or eLo->Org is below eUp (depending on which
        /// origin is leftmost).
        /// 
        /// The main purpose is to splice right-going edges with the same
        /// dest vertex and nearly identical slopes (ie. we can't distinguish
        /// the slopes numerically).  However the splicing can also help us
        /// to recover from numerical errors.  For example, suppose at one
        /// point we checked eUp and eLo, and decided that eUp->Org is barely
        /// above eLo.  Then later, we split eLo into two edges (eg. from
        /// a splice operation like this one).  This can change the result of
        /// our test so that now eUp->Org is incident to eLo, or barely below it.
        /// We must correct this condition to maintain the dictionary invariants.
        /// 
        /// One possibility is to check these edges for intersection again
        /// (ie. CheckForIntersect).  This is what we do if possible.  However
        /// CheckForIntersect requires that tess->event lies between eUp and eLo,
        /// so that it has something to fall back on when the intersection
        /// calculation gives us an unusable answer.  So, for those cases where
        /// we can't check for intersection, this routine fixes the problem
        /// by just splicing the offending vertex into the other edge.
        /// This is a guaranteed solution, no matter how degenerate things get.
        /// Basically this is a combinatorial solution to a numerical problem.
        /// </summary>
        private bool CheckForRightSplice(ActiveRegion regUp)
        {
            var regLo = RegionBelow(regUp);
            var eUp = regUp.EUp;
            var eLo = regLo.EUp;

            if (Geom.VertLeq(eUp._Org, eLo._Org))
            {
                if (Geom.EdgeSign(eLo.Dst, eUp._Org, eLo._Org) > 0.0)
                {
                    return false;
                }

                // eUp.Org appears to be below eLo
                if (!Geom.VertEq(eUp._Org, eLo._Org))
                {
                    // Splice eUp._Org into eLo
                    _mesh.SplitEdge(eLo._Sym);
                    _mesh.Splice(eUp, eLo.Oprev);
                    regUp.Dirty = regLo.Dirty = true;
                }
                else if (eUp._Org != eLo._Org)
                {
                    // merge the two vertices, discarding eUp.Org
                    _pq.Remove(eUp._Org._pqHandle);
                    SpliceMergeVertices(eLo.Oprev, eUp);
                }
            }
            else
            {
                if (Geom.EdgeSign(eUp.Dst, eLo._Org, eUp._Org) < 0.0)
                {
                    return false;
                }

                // eLo.Org appears to be above eUp, so splice eLo.Org into eUp
                RegionAbove(regUp).Dirty = regUp.Dirty = true;
                _mesh.SplitEdge(eUp._Sym);
                _mesh.Splice(eLo.Oprev, eUp);
            }
            return true;
        }
        
        /// <summary>
        /// Check the upper and lower edge of "regUp", to make sure that the
        /// eUp->Dst is above eLo, or eLo->Dst is below eUp (depending on which
        /// destination is rightmost).
        /// 
        /// Theoretically, this should always be true.  However, splitting an edge
        /// into two pieces can change the results of previous tests.  For example,
        /// suppose at one point we checked eUp and eLo, and decided that eUp->Dst
        /// is barely above eLo.  Then later, we split eLo into two edges (eg. from
        /// a splice operation like this one).  This can change the result of
        /// the test so that now eUp->Dst is incident to eLo, or barely below it.
        /// We must correct this condition to maintain the dictionary invariants
        /// (otherwise new edges might get inserted in the wrong place in the
        /// dictionary, and bad stuff will happen).
        /// 
        /// We fix the problem by just splicing the offending vertex into the
        /// other edge.
        /// </summary>
        private bool CheckForLeftSplice(ActiveRegion regUp)
        {
            var regLo = RegionBelow(regUp);
            var eUp = regUp.EUp;
            var eLo = regLo.EUp;

            Debug.Assert(!Geom.VertEq(eUp.Dst, eLo.Dst));

            if (Geom.VertLeq(eUp.Dst, eLo.Dst))
            {
                if (Geom.EdgeSign(eUp.Dst, eLo.Dst, eUp._Org) < 0.0)
                {
                    return false;
                }

                // eLo.Dst is above eUp, so splice eLo.Dst into eUp
                RegionAbove(regUp).Dirty = regUp.Dirty = true;
                var e = _mesh.SplitEdge(eUp);
                _mesh.Splice(eLo._Sym, e);
                e._Lface._inside = regUp.Inside;
            }
            else
            {
                if (Geom.EdgeSign(eLo.Dst, eUp.Dst, eLo._Org) > 0.0)
                {
                    return false;
                }

                // eUp.Dst is below eLo, so splice eUp.Dst into eLo
                regUp.Dirty = regLo.Dirty = true;
                var e = _mesh.SplitEdge(eLo);
                _mesh.Splice(eUp._Lnext, eLo._Sym);
                e.Rface._inside = regUp.Inside;
            }
            return true;
        }

        /// <summary>
        /// Check the upper and lower edges of the given region to see if
        /// they intersect.  If so, create the intersection and add it
        /// to the data structures.
        /// 
        /// Returns TRUE if adding the new intersection resulted in a recursive
        /// call to AddRightEdges(); in this case all "dirty" regions have been
        /// checked for intersections, and possibly regUp has been deleted.
        /// </summary>
        private bool CheckForIntersect(ActiveRegion regUp)
        {
            var regLo = RegionBelow(regUp);
            var eUp = regUp.EUp;
            var eLo = regLo.EUp;
            var orgUp = eUp._Org;
            var orgLo = eLo._Org;
            var dstUp = eUp.Dst;
            var dstLo = eLo.Dst;

            Debug.Assert(!Geom.VertEq(dstLo, dstUp));
            Debug.Assert(Geom.EdgeSign(dstUp, _event, orgUp) <= 0.0);
            Debug.Assert(Geom.EdgeSign(dstLo, _event, orgLo) >= 0.0);
            Debug.Assert(orgUp != _event && orgLo != _event);
            Debug.Assert(!regUp.FixUpperEdge && !regLo.FixUpperEdge);

            if( orgUp == orgLo )
            {
                // right endpoints are the same
                return false;
            }

            var tMinUp = Math.Min(orgUp._t, dstUp._t);
            var tMaxLo = Math.Max(orgLo._t, dstLo._t);
            if( tMinUp > tMaxLo )
            {
                // t ranges do not overlap
                return false;
            }

            if (Geom.VertLeq(orgUp, orgLo))
            {
                if (Geom.EdgeSign( dstLo, orgUp, orgLo ) > 0.0)
                {
                    return false;
                }
            }
            else
            {
                if (Geom.EdgeSign( dstUp, orgLo, orgUp ) < 0.0)
                {
                    return false;
                }
            }

            // At this point the edges intersect, at least marginally

            var isect = new MeshUtils.Vertex();
            Geom.EdgeIntersect(dstUp, orgUp, dstLo, orgLo, isect);
            // The following properties are guaranteed:
            Debug.Assert(Math.Min(orgUp._t, dstUp._t) <= isect._t);
            Debug.Assert(isect._t <= Math.Max(orgLo._t, dstLo._t));
            Debug.Assert(Math.Min(dstLo._s, dstUp._s) <= isect._s);
            Debug.Assert(isect._s <= Math.Max(orgLo._s, orgUp._s));

            if (Geom.VertLeq(isect, _event))
            {
                // The intersection point lies slightly to the left of the sweep line,
                // so move it until it''s slightly to the right of the sweep line.
                // (If we had perfect numerical precision, this would never happen
                // in the first place). The easiest and safest thing to do is
                // replace the intersection by tess._event.
                isect._s = _event._s;
                isect._t = _event._t;
            }
            // Similarly, if the computed intersection lies to the right of the
            // rightmost origin (which should rarely happen), it can cause
            // unbelievable inefficiency on sufficiently degenerate inputs.
            // (If you have the test program, try running test54.d with the
            // "X zoom" option turned on).
            var orgMin = Geom.VertLeq(orgUp, orgLo) ? orgUp : orgLo;
            if (Geom.VertLeq(orgMin, isect))
            {
                isect._s = orgMin._s;
                isect._t = orgMin._t;
            }

            if (Geom.VertEq(isect, orgUp) || Geom.VertEq(isect, orgLo))
            {
                // Easy case -- intersection at one of the right endpoints
                CheckForRightSplice(regUp);
                return false;
            }

            if (   (! Geom.VertEq(dstUp, _event)
                && Geom.EdgeSign(dstUp, _event, isect) >= 0.0)
                || (! Geom.VertEq(dstLo, _event)
                && Geom.EdgeSign(dstLo, _event, isect) <= 0.0 ))
            {
                // Very unusual -- the new upper or lower edge would pass on the
                // wrong side of the sweep event, or through it. This can happen
                // due to very small numerical errors in the intersection calculation.
                if (dstLo == _event)
                {
                    // Splice dstLo into eUp, and process the new region(s)
                    _mesh.SplitEdge(eUp._Sym);
                    _mesh.Splice(eLo._Sym, eUp);
                    regUp = TopLeftRegion(regUp);
                    eUp = RegionBelow(regUp).EUp;
                    FinishLeftRegions(RegionBelow(regUp), regLo);
                    AddRightEdges(regUp, eUp.Oprev, eUp, eUp, true);
                    return true;
                }
                if( dstUp == _event ) {
                    /* Splice dstUp into eLo, and process the new region(s) */
                    _mesh.SplitEdge(eLo._Sym);
                    _mesh.Splice(eUp._Lnext, eLo.Oprev);
                    regLo = regUp;
                    regUp = TopRightRegion(regUp);
                    var e = RegionBelow(regUp).EUp.Rprev;
                    regLo.EUp = eLo.Oprev;
                    eLo = FinishLeftRegions(regLo, null);
                    AddRightEdges(regUp, eLo._Onext, eUp.Rprev, e, true);
                    return true;
                }
                // Special case: called from ConnectRightVertex. If either
                // edge passes on the wrong side of tess._event, split it
                // (and wait for ConnectRightVertex to splice it appropriately).
                if (Geom.EdgeSign( dstUp, _event, isect ) >= 0.0)
                {
                    RegionAbove(regUp).Dirty = regUp.Dirty = true;
                    _mesh.SplitEdge(eUp._Sym);
                    eUp._Org._s = _event._s;
                    eUp._Org._t = _event._t;
                }
                if (Geom.EdgeSign(dstLo, _event, isect) <= 0.0)
                {
                    regUp.Dirty = regLo.Dirty = true;
                    _mesh.SplitEdge(eLo._Sym);
                    eLo._Org._s = _event._s;
                    eLo._Org._t = _event._t;
                }
                // leave the rest for ConnectRightVertex
                return false;
            }

            // General case -- split both edges, splice into new vertex.
            // When we do the splice operation, the order of the arguments is
            // arbitrary as far as correctness goes. However, when the operation
            // creates a new face, the work done is proportional to the size of
            // the new face.  We expect the faces in the processed part of
            // the mesh (ie. eUp._Lface) to be smaller than the faces in the
            // unprocessed original contours (which will be eLo._Oprev._Lface).
            _mesh.SplitEdge(eUp._Sym);
            _mesh.SplitEdge(eLo._Sym);
            _mesh.Splice(eLo.Oprev, eUp);
            eUp._Org._s = isect._s;
            eUp._Org._t = isect._t;
            eUp._Org._pqHandle = _pq.Insert(eUp._Org);
            if (eUp._Org._pqHandle.Handle == PqHandle.Invalid)
            {
                throw new InvalidOperationException("PQHandle should not be invalid");
            }
            GetIntersectData(eUp._Org, orgUp, dstUp, orgLo, dstLo);
            RegionAbove(regUp).Dirty = regUp.Dirty = regLo.Dirty = true;
            return false;
        }

        /// <summary>
        /// When the upper or lower edge of any region changes, the region is
        /// marked "dirty".  This routine walks through all the dirty regions
        /// and makes sure that the dictionary invariants are satisfied
        /// (see the comments at the beginning of this file).  Of course
        /// new dirty regions can be created as we make changes to restore
        /// the invariants.
        /// </summary>
        private void WalkDirtyRegions(ActiveRegion regUp)
        {
            var regLo = RegionBelow(regUp);
            MeshUtils.Edge eUp, eLo;

            while (true)
            {
                // Find the lowest dirty region (we walk from the bottom up).
                while (regLo.Dirty)
                {
                    regUp = regLo;
                    regLo = RegionBelow(regLo);
                }
                if (!regUp.Dirty)
                {
                    regLo = regUp;
                    regUp = RegionAbove( regUp );
                    if(regUp == null || !regUp.Dirty)
                    {
                        // We've walked all the dirty regions
                        return;
                    }
                }
                regUp.Dirty = false;
                eUp = regUp.EUp;
                eLo = regLo.EUp;

                if (eUp.Dst != eLo.Dst)
                {
                    // Check that the edge ordering is obeyed at the Dst vertices.
                    if (CheckForLeftSplice(regUp))
                    {

                        // If the upper or lower edge was marked fixUpperEdge, then
                        // we no longer need it (since these edges are needed only for
                        // vertices which otherwise have no right-going edges).
                        if (regLo.FixUpperEdge)
                        {
                            DeleteRegion(regLo);
                            _mesh.Delete(eLo);
                            regLo = RegionBelow(regUp);
                            eLo = regLo.EUp;
                        }
                        else if( regUp.FixUpperEdge )
                        {
                            DeleteRegion(regUp);
                            _mesh.Delete(eUp);
                            regUp = RegionAbove(regLo);
                            eUp = regUp.EUp;
                        }
                    }
                }
                if (eUp._Org != eLo._Org)
                {
                    if(    eUp.Dst != eLo.Dst
                        && ! regUp.FixUpperEdge && ! regLo.FixUpperEdge
                        && (eUp.Dst == _event || eLo.Dst == _event) )
                    {
                        // When all else fails in CheckForIntersect(), it uses tess._event
                        // as the intersection location. To make this possible, it requires
                        // that tess._event lie between the upper and lower edges, and also
                        // that neither of these is marked fixUpperEdge (since in the worst
                        // case it might splice one of these edges into tess.event, and
                        // violate the invariant that fixable edges are the only right-going
                        // edge from their associated vertex).
                        if (CheckForIntersect(regUp))
                        {
                            // WalkDirtyRegions() was called recursively; we're done
                            return;
                        }
                    }
                    else
                    {
                        // Even though we can't use CheckForIntersect(), the Org vertices
                        // may violate the dictionary edge ordering. Check and correct this.
                        CheckForRightSplice(regUp);
                    }
                }
                if (eUp._Org == eLo._Org && eUp.Dst == eLo.Dst)
                {
                    // A degenerate loop consisting of only two edges -- delete it.
                    Geom.AddWinding(eLo, eUp);
                    DeleteRegion(regUp);
                    _mesh.Delete(eUp);
                    regUp = RegionAbove(regLo);
                }
            }
        }

        /// <summary>
        /// Purpose: connect a "right" vertex vEvent (one where all edges go left)
        /// to the unprocessed portion of the mesh.  Since there are no right-going
        /// edges, two regions (one above vEvent and one below) are being merged
        /// into one.  "regUp" is the upper of these two regions.
        /// 
        /// There are two reasons for doing this (adding a right-going edge):
        ///  - if the two regions being merged are "inside", we must add an edge
        ///    to keep them separated (the combined region would not be monotone).
        ///  - in any case, we must leave some record of vEvent in the dictionary,
        ///    so that we can merge vEvent with features that we have not seen yet.
        ///    For example, maybe there is a vertical edge which passes just to
        ///    the right of vEvent; we would like to splice vEvent into this edge.
        /// 
        /// However, we don't want to connect vEvent to just any vertex.  We don''t
        /// want the new edge to cross any other edges; otherwise we will create
        /// intersection vertices even when the input data had no self-intersections.
        /// (This is a bad thing; if the user's input data has no intersections,
        /// we don't want to generate any false intersections ourselves.)
        /// 
        /// Our eventual goal is to connect vEvent to the leftmost unprocessed
        /// vertex of the combined region (the union of regUp and regLo).
        /// But because of unseen vertices with all right-going edges, and also
        /// new vertices which may be created by edge intersections, we don''t
        /// know where that leftmost unprocessed vertex is.  In the meantime, we
        /// connect vEvent to the closest vertex of either chain, and mark the region
        /// as "fixUpperEdge".  This flag says to delete and reconnect this edge
        /// to the next processed vertex on the boundary of the combined region.
        /// Quite possibly the vertex we connected to will turn out to be the
        /// closest one, in which case we won''t need to make any changes.
        /// </summary>
        private void ConnectRightVertex(ActiveRegion regUp, MeshUtils.Edge eBottomLeft)
        {
            var eTopLeft = eBottomLeft._Onext;
            var regLo = RegionBelow(regUp);
            var eUp = regUp.EUp;
            var eLo = regLo.EUp;
            bool degenerate = false;

            if (eUp.Dst != eLo.Dst)
            {
                CheckForIntersect(regUp);
            }

            // Possible new degeneracies: upper or lower edge of regUp may pass
            // through vEvent, or may coincide with new intersection vertex
            if (Geom.VertEq(eUp._Org, _event))
            {
                _mesh.Splice(eTopLeft.Oprev, eUp);
                regUp = TopLeftRegion(regUp);
                eTopLeft = RegionBelow(regUp).EUp;
                FinishLeftRegions(RegionBelow(regUp), regLo);
                degenerate = true;
            }
            if (Geom.VertEq(eLo._Org, _event))
            {
                _mesh.Splice(eBottomLeft, eLo.Oprev);
                eBottomLeft = FinishLeftRegions(regLo, null);
                degenerate = true;
            }
            if (degenerate)
            {
                AddRightEdges(regUp, eBottomLeft._Onext, eTopLeft, eTopLeft, true);
                return;
            }

            // Non-degenerate situation -- need to add a temporary, fixable edge.
            // Connect to the closer of eLo.Org, eUp.Org.
            MeshUtils.Edge eNew;
            if (Geom.VertLeq(eLo._Org, eUp._Org))
            {
                eNew = eLo.Oprev;
            }
            else
            {
                eNew = eUp;
            }
            eNew = _mesh.Connect(eBottomLeft.Lprev, eNew);

            // Prevent cleanup, otherwise eNew might disappear before we've even
            // had a chance to mark it as a temporary edge.
            AddRightEdges(regUp, eNew, eNew._Onext, eNew._Onext, false);
            eNew._Sym._activeRegion.FixUpperEdge = true;
            WalkDirtyRegions(regUp);
        }

        /// <summary>
        /// The event vertex lies exacty on an already-processed edge or vertex.
        /// Adding the new vertex involves splicing it into the already-processed
        /// part of the mesh.
        /// </summary>
        private void ConnectLeftDegenerate(ActiveRegion regUp, MeshUtils.Vertex vEvent)
        {
            var e = regUp.EUp;
            if (Geom.VertEq(e._Org, vEvent))
            {
                // e.Org is an unprocessed vertex - just combine them, and wait
                // for e.Org to be pulled from the queue
                // C# : in the C version, there is a flag but it was never implemented
                // the vertices are before beginning the tesselation
                throw new InvalidOperationException("Vertices should have been merged before");
            }

            if (!Geom.VertEq(e.Dst, vEvent))
            {
                // General case -- splice vEvent into edge e which passes through it
                _mesh.SplitEdge(e._Sym);
                if (regUp.FixUpperEdge)
                {
                    // This edge was fixable -- delete unused portion of original edge
                    _mesh.Delete(e._Onext);
                    regUp.FixUpperEdge = false;
                }
                _mesh.Splice(vEvent._anEdge, e);
                SweepEvent(vEvent);	// recurse
                return;
            }

            // See above
            throw new InvalidOperationException("Vertices should have been merged before");
        }

        /// <summary>
        /// Purpose: connect a "left" vertex (one where both edges go right)
        /// to the processed portion of the mesh.  Let R be the active region
        /// containing vEvent, and let U and L be the upper and lower edge
        /// chains of R.  There are two possibilities:
        /// 
        /// - the normal case: split R into two regions, by connecting vEvent to
        ///   the rightmost vertex of U or L lying to the left of the sweep line
        /// 
        /// - the degenerate case: if vEvent is close enough to U or L, we
        ///   merge vEvent into that edge chain.  The subcases are:
        ///     - merging with the rightmost vertex of U or L
        ///     - merging with the active edge of U or L
        ///     - merging with an already-processed portion of U or L
        /// </summary>
        private void ConnectLeftVertex(MeshUtils.Vertex vEvent)
        {
            var tmp = new ActiveRegion();

            // Get a pointer to the active region containing vEvent
            tmp.EUp = vEvent._anEdge._Sym;
            var regUp = _dict.Find(tmp).Key;
            var regLo = RegionBelow(regUp);
            if (regLo == null)
            {
                // This may happen if the input polygon is coplanar.
                return;
            }
            var eUp = regUp.EUp;
            var eLo = regLo.EUp;

            // Try merging with U or L first
// ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Geom.EdgeSign(eUp.Dst, vEvent, eUp._Org) == 0.0)
            {
                ConnectLeftDegenerate(regUp, vEvent);
                return;
            }

            // Connect vEvent to rightmost processed vertex of either chain.
            // e._Dst is the vertex that we will connect to vEvent.
            var reg = Geom.VertLeq(eLo.Dst, eUp.Dst) ? regUp : regLo;

            if (regUp.Inside || reg.FixUpperEdge)
            {
                MeshUtils.Edge eNew;
                if (reg == regUp)
                {
                    eNew = _mesh.Connect(vEvent._anEdge._Sym, eUp._Lnext);
                }
                else
                {
                    eNew = _mesh.Connect(eLo.Dnext, vEvent._anEdge)._Sym;
                }
                if (reg.FixUpperEdge)
                {
                    FixUpperEdge(reg, eNew);
                }
                else
                {
                    ComputeWinding(AddRegionBelow(regUp, eNew));
                }
                SweepEvent(vEvent);
            }
            else
            {
                // The new vertex is in a region which does not belong to the polygon.
                // We don't need to connect this vertex to the rest of the mesh.
                AddRightEdges(regUp, vEvent._anEdge, vEvent._anEdge, null, true);
            }
        }

        /// <summary>
        /// Does everything necessary when the sweep line crosses a vertex.
        /// Updates the mesh and the edge dictionary.
        /// </summary>
        private void SweepEvent(MeshUtils.Vertex vEvent)
        {
            _event = vEvent;

            // Check if this vertex is the right endpoint of an edge that is
            // already in the dictionary. In this case we don't need to waste
            // time searching for the location to insert new edges.
            var e = vEvent._anEdge;
            while (e._activeRegion == null)
            {
                e = e._Onext;
                if (e == vEvent._anEdge)
                {
                    // All edges go right -- not incident to any processed edges
                    ConnectLeftVertex(vEvent);
                    return;
                }
            }

            // Processing consists of two phases: first we "finish" all the
            // active regions where both the upper and lower edges terminate
            // at vEvent (ie. vEvent is closing off these regions).
            // We mark these faces "inside" or "outside" the polygon according
            // to their winding number, and delete the edges from the dictionary.
            // This takes care of all the left-going edges from vEvent.
            var regUp = TopLeftRegion(e._activeRegion);
            var reg = RegionBelow(regUp);
            var eTopLeft = reg.EUp;
            var eBottomLeft = FinishLeftRegions(reg, null);

            // Next we process all the right-going edges from vEvent. This
            // involves adding the edges to the dictionary, and creating the
            // associated "active regions" which record information about the
            // regions between adjacent dictionary edges.
            if (eBottomLeft._Onext == eTopLeft)
            {
                // No right-going edges -- add a temporary "fixable" edge
                ConnectRightVertex(regUp, eBottomLeft);
            }
            else
            {
                AddRightEdges(regUp, eBottomLeft._Onext, eTopLeft, eTopLeft, true);
            }
        }

        /// <summary>
        /// Make the sentinel coordinates big enough that they will never be
        /// merged with real input features.
        /// 
        /// We add two sentinel edges above and below all other edges,
        /// to avoid special cases at the top and bottom.
        /// </summary>
        private void AddSentinel(double smin, double smax, double t)
        {
            var e = _mesh.MakeEdge();
            e._Org._s = smax;
            e._Org._t = t;
            e.Dst._s = smin;
            e.Dst._t = t;
            _event = e.Dst; // initialize it

            var reg = new ActiveRegion();
            reg.EUp = e;
            reg.WindingNumber = 0;
            reg.Inside = false;
            reg.FixUpperEdge = false;
            reg.Sentinel = true;
            reg.Dirty = false;
            reg.NodeUp = _dict.Insert(reg);
        }

        /// <summary>
        /// We maintain an ordering of edge intersections with the sweep line.
        /// This order is maintained in a dynamic dictionary.
        /// </summary>
        private void InitEdgeDict()
        {
            _dict = new Dict<ActiveRegion>(EdgeLeq);

            var w = _bmaxX - _bminX;
            var h = _bmaxY - _bminY;

            var smin = _bminX - w;
            var smax = _bmaxX + w;
            var tmin = _bminY - h;
            var tmax = _bmaxY + h;

            AddSentinel(smin, smax, tmin);
            AddSentinel(smin, smax, tmax);
        }

        private void DoneEdgeDict()
        {
            int fixedEdges = 0;

            ActiveRegion reg;
            while ((reg = _dict.Min().Key) != null)
            {
                // At the end of all processing, the dictionary should contain
                // only the two sentinel edges, plus at most one "fixable" edge
                // created by ConnectRightVertex().
                if (!reg.Sentinel)
                {
                    Debug.Assert(reg.FixUpperEdge);
                    Debug.Assert(++fixedEdges == 1);
                }
                Debug.Assert(reg.WindingNumber == 0);
                DeleteRegion(reg);
            }

            _dict = null;
        }

        /// <summary>
        /// Remove zero-length edges, and contours with fewer than 3 vertices.
        /// </summary>
        private void RemoveDegenerateEdges()
        {
            MeshUtils.Edge eHead = _mesh._eHead, e, eNext, eLnext;

            for (e = eHead._next; e != eHead; e = eNext)
            {
                eNext = e._next;
                eLnext = e._Lnext;

                if (Geom.VertEq(e._Org, e.Dst) && e._Lnext._Lnext != e)
                {
                    // Zero-length edge, contour has at least 3 edges

                    SpliceMergeVertices(eLnext, e);	// deletes e.Org
                    _mesh.Delete(e); // e is a self-loop
                    e = eLnext;
                    eLnext = e._Lnext;
                }
                if (eLnext._Lnext == e)
                {
                    // Degenerate contour (one or two edges)

                    if (eLnext != e)
                    {
                        if (eLnext == eNext || eLnext == eNext._Sym)
                        {
                            eNext = eNext._next;
                        }
                        _mesh.Delete(eLnext);
                    }
                    if (e == eNext || e == eNext._Sym)
                    {
                        eNext = eNext._next;
                    }
                    _mesh.Delete(e);
                }
            }
        }

        /// <summary>
        /// Insert all vertices into the priority queue which determines the
        /// order in which vertices cross the sweep line.
        /// </summary>
        private void InitPriorityQ()
        {
            MeshUtils.Vertex vHead = _mesh._vHead, v;
            int vertexCount = 0;

            for (v = vHead._next; v != vHead; v = v._next)
            {
                vertexCount++;
            }
            // Make sure there is enough space for sentinels.
            vertexCount += 8;
    
            _pq = new PriorityQueue<MeshUtils.Vertex>(vertexCount, Geom.VertLeq);

            vHead = _mesh._vHead;
            for( v = vHead._next; v != vHead; v = v._next ) {
                v._pqHandle = _pq.Insert(v);
                if (v._pqHandle.Handle == PqHandle.Invalid)
                {
                    throw new InvalidOperationException("PQHandle should not be invalid");
                }
            }
            _pq.Init();
        }

        private void DonePriorityQ()
        {
            _pq = null;
        }

        /// <summary>
        /// Delete any degenerate faces with only two edges.  WalkDirtyRegions()
        /// will catch almost all of these, but it won't catch degenerate faces
        /// produced by splice operations on already-processed edges.
        /// The two places this can happen are in FinishLeftRegions(), when
        /// we splice in a "temporary" edge produced by ConnectRightVertex(),
        /// and in CheckForLeftSplice(), where we splice already-processed
        /// edges to ensure that our dictionary invariants are not violated
        /// by numerical errors.
        /// 
        /// In both these cases it is *very* dangerous to delete the offending
        /// edge at the time, since one of the routines further up the stack
        /// will sometimes be keeping a pointer to that edge.
        /// </summary>
        private void RemoveDegenerateFaces()
        {
            MeshUtils.Face f, fNext;
            MeshUtils.Edge e;

// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            for (f = _mesh._fHead._next; f != _mesh._fHead; f = fNext)
            {
                fNext = f._next;
                e = f._anEdge;
                Debug.Assert(e._Lnext != e);

                if (e._Lnext._Lnext == e)
                {
                    // A face with only two edges
                    Geom.AddWinding(e._Onext, e);
                    _mesh.Delete(e);
                }
            }
        }

        /// <summary>
        /// ComputeInterior computes the planar arrangement specified
        /// by the given contours, and further subdivides this arrangement
        /// into regions.  Each region is marked "inside" if it belongs
        /// to the polygon, according to the rule given by windingRule.
        /// Each interior region is guaranteed to be monotone.
        /// </summary>
        protected void ComputeInterior()
        {
            // Each vertex defines an event for our sweep line. Start by inserting
            // all the vertices in a priority queue. Events are processed in
            // lexicographic order, ie.
            // 
            // e1 < e2  iff  e1.x < e2.x || (e1.x == e2.x && e1.y < e2.y)
            RemoveDegenerateEdges();
            InitPriorityQ();
            RemoveDegenerateFaces();
            InitEdgeDict();

            MeshUtils.Vertex v, vNext;
            while ((v = _pq.ExtractMin()) != null)
            {
                 while (true)
                 {
                    vNext = _pq.Minimum();
                    if (vNext == null || !Geom.VertEq(vNext, v))
                    {
                        break;
                    }

                    // Merge together all vertices at exactly the same location.
                    // This is more efficient than processing them one at a time,
                    // simplifies the code (see ConnectLeftDegenerate), and is also
                    // important for correct handling of certain degenerate cases.
                    // For example, suppose there are two identical edges A and B
                    // that belong to different contours (so without this code they would
                    // be processed by separate sweep events). Suppose another edge C
                    // crosses A and B from above. When A is processed, we split it
                    // at its intersection point with C. However this also splits C,
                    // so when we insert B we may compute a slightly different
                    // intersection point. This might leave two edges with a small
                    // gap between them. This kind of error is especially obvious
                    // when using boundary extraction (BoundaryOnly).
                    vNext = _pq.ExtractMin();
                    SpliceMergeVertices(v._anEdge, vNext._anEdge);
                }
                SweepEvent(v);
            }

            DoneEdgeDict();
            DonePriorityQ();

            RemoveDegenerateFaces();
            _mesh.Check();
        }
    }
}
