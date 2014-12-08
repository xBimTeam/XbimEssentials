#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCompositeCurveSegment.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class CompositeCurveSegmentList : XbimList<IfcCompositeCurveSegment>
    {
        internal CompositeCurveSegmentList(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }


    [IfcPersistedEntityAttribute]
    public class IfcCompositeCurveSegment : IfcGeometricRepresentationItem
    {
        #region Fields

        private IfcTransitionCode _transition;
        private IfcBoolean _sameSense = true;
        private IfcCurve _parentCurve;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The state of transition (i.e., geometric continuity from the last point of this segment to the first point of the next segment) in a composite curve.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcTransitionCode Transition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transition;
            }
            set { this.SetModelValue(this, ref _transition, value, v => Transition = v, "Transition"); }
        }

        /// <summary>
        ///   An indicator of whether or not the sense of the segment agrees with, or opposes, that of the parent curve. If SameSense is false, the point with highest parameter value is taken as the first point of the segment.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcBoolean SameSense
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sameSense;
            }
            set { this.SetModelValue(this, ref _sameSense, value, v => SameSense = v, "SameSense"); }
        }

        /// <summary>
        ///   The bounded curve which defines the geometry of the segment.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcCurve ParentCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _parentCurve;
            }
            set { this.SetModelValue(this, ref _parentCurve, value, v => ParentCurve = v, "ParentCurve"); }
        }


        /// <summary>
        ///   Derived. The space dimensionality of this class, defined by the dimensionality of the first ParentCurve.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return ParentCurve.Dim; }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _transition = (IfcTransitionCode) Enum.Parse(typeof (IfcTransitionCode), value.EnumVal, true);
                    break;
                case 1:
                    _sameSense = value.BooleanVal;
                    break;
                case 2:
                    _parentCurve = (IfcCurve) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Inverse.   The set of composite curves which use this composite curve segment as a segment. This set shall not be empty.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public IEnumerable<IfcCompositeCurve> UsingCurves
        {
            get { return ModelOf.Instances.Where<IfcCompositeCurve>(c => c.Segments.Contains(this)); }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (!(ParentCurve is IfcBoundedCurve))
                return "WR1 CompositeCurveSegment : The parent curve shall be a bounded curve.";
            else return "";
        }

        #endregion
    }
}