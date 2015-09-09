#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCompositeCurve.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A composite curve (IfcCompositeCurve) is a collection of curves joined end-to-end.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A composite curve (IfcCompositeCurve) is a collection of curves joined end-to-end. The individual segments of the curve are themselves defined as composite curve segments. The parameterization of the composite curve is an accumulation of the parametric ranges of the referenced bounded curves. The first segment is parameterized from 0 to l1, and, for i ³ 2, the ith segment is parameterized from 
    ///   where lk is the parametric length (i.e., difference between maximum and minimum parameter values) of the curve underlying the kth segment. 
    ///   NOTE Corresponding STEP entity: composite_curve, please refer to ISO/IS 10303-42:1994, p. 56 for the final definition of the formal standard. The WR2 is added to ensure consistent Dim at all segments. 
    ///   HISTORY New class in IFC Release 1.0 
    ///   Informal Propositions:
    ///   The SameSense attribute of each segment correctly specifies the senses of the component curves. When traversed in the direction indicated by SameSense, the segments shall join end-to-end. 
    ///   Formal Propositions:
    ///   WR41   :   No transition code should be Discontinuous, except for the last code of an open curve.  
    ///   WR42   :   Ensures, that all segments used in the curve have the same dimensionality.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcCompositeCurve : IfcBoundedCurve
    {
        public IfcCompositeCurve()
        {
            _segments = new CompositeCurveSegmentList(this);
        }

        #region Fields

        private CompositeCurveSegmentList _segments;
        private IfcLogical? _selfIntersect;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1), IndexedProperty]
        public CompositeCurveSegmentList Segments
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _segments;
            }
            set { this.SetModelValue(this, ref _segments, value, v => Segments = v, "Segments"); }
        }

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcLogical? SelfIntersect
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _selfIntersect;
            }
            set { this.SetModelValue(this, ref _selfIntersect, value, v => SelfIntersect = v, "SelfIntersect "); }
        }

        public int NSegments
        {
            get { return Segments.Count; }
        }

        public bool ClosedCurve
        {
            get
            {
                IfcCompositeCurveSegment last = Segments.LastOrDefault();
                return last != null ? last.Transition != IfcTransitionCode.DISCONTINUOUS : false;
            }
        }

        public override IfcDimensionCount Dim
        {
            get
            {
                IfcCompositeCurveSegment first = Segments.FirstOrDefault();
                return first != null ? first.Dim : (IfcDimensionCount) 0;
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _segments.Add((IfcCompositeCurveSegment) value.EntityVal);
                    break;
                case 1:
                    _selfIntersect = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            int discon = Segments.Where(c => c.Transition == IfcTransitionCode.DISCONTINUOUS).Count();
            bool closed = ClosedCurve;
            string err = "";
            if ((!closed && discon != 1) || (closed && discon != 0))
                err +=
                    "WR41: CompositeCurve : No transition code should be Discontinuous, except for the last code of an open curve.\n";

            bool first = true;
            int dim = 0;
            foreach (IfcCompositeCurveSegment seg in Segments)
            {
                if (first)
                {
                    dim = seg.Dim;
                    first = false;
                }
                else if (dim != seg.Dim)
                {
                    err +=
                        "WR42 CompositeCurve : Ensures, that all segments used in the curve have the same dimensionality.\n";
                    break;
                }
            }
            return err;
        }

        #endregion
    }
}
