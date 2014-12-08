#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBSplineCurve.cs
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
    ///   A B-spline curve is a piecewise parametric polynominal or rational curve described in terms of control points and basis functions.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A B-spline curve is a piecewise parametric polynominal or rational curve described in terms of control points and basis functions. The B-spline curve has been selected as the most stable format to represent all types of polynominal or rational parametric curves. With appropriate attribute values it is capable of representing single span or spline curves of explicit polynomial, rational, Bezier or B-spline type.
    ///   Interpretation of the data is as follows:
    ///   All weights shall be positive and the curve is given by
    ///   k+1 = number of control points 
    ///   Pi = control points 
    ///   wi = weights 
    ///   d = degree 
    ///   The knot array is an array of (k+d+2) real numbers [u-d ... uk+1], such that for all indices j in [-d,k], uj = uj+1. This array is obtained from the knot data list by repeating each multiple knot according to the multiplicity. N di, the ith normalised B-spline basis function of degree d, is defined on the subset [ui-d, ... , ui+1] of this array.
    ///   Let L denote the number of distinct values amongst the d+k+2 knots in the knot array; L will be referred to as the 'upper index on knots'. Let mj denote the multiplicity (i.e. number of repetitions) of the jth distinct knot. Then
    ///  
    ///   All knot multiplicities except the first and the last shall be in the range 1 ... degree; the first and last may have a maximum value of degree + 1. In evaluating the basis functions, a knot u of e.g. multiplicity 3 is interpreted as a string u, u, u, in the knot array. The B-spline curve has 3 special subtypes (IAI note: only 1, Bezier curve, included in this IFC release) where the knots and knot multiplicities are derived to provide simple default capabilities.
    ///   Logical flag is provided to indicate whether the curve self intersects or not. 
    ///   Illustration from ISO 10303-42:
    ///  
    ///   NOTE: Corresponding STEP entity: b_spline_curve. Please refer to ISO/IS 10303-42:1994, p. 45 for the final definition of the formal standard. 
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    ///   Formal Propositions:
    ///   WR41   :   All control points shall have the same dimensionality.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcBSplineCurve : IfcBoundedCurve
    {
        public IfcBSplineCurve()
        {
            _controlPointsList = new CartesianPointList(this);
        }

        #region Fields

        private int _degree;
        private CartesianPointList _controlPointsList;
        private IfcBSplineCurveForm _curveForm = IfcBSplineCurveForm.UNSPECIFIED;
        private IfcLogical _closedCurve;
        private IfcLogical _selfIntersect;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The algebraic degree of the basis functions.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public int Degree
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _degree;
            }
            set { this.SetModelValue(this, ref _degree, value, v => Degree = v, "Degree"); }
        }


        /// <summary>
        ///   The list of control points for the curve.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 2)]
        public CartesianPointList ControlPointsList
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _controlPointsList;
            }
            set
            {
                this.SetModelValue(this, ref _controlPointsList, value, v => ControlPointsList = v,
                                           "ControlPointsList");
            }
        }


        /// <summary>
        ///   Used to identify particular types of curve; it is for information only.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcBSplineCurveForm CurveForm
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _curveForm;
            }
            set { this.SetModelValue(this, ref _curveForm, value, v => CurveForm = v, "CurveForm"); }
        }


        /// <summary>
        ///   Indication of whether the curve is closed; it is for information only.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcLogical ClosedCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _closedCurve;
            }
            set { this.SetModelValue(this, ref _closedCurve, value, v => ClosedCurve = v, "ClosedCurve"); }
        }


        /// <summary>
        ///   Indication whether the curve self-intersects or not; it is for information only.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcLogical SelfIntersect
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _selfIntersect;
            }
            set { this.SetModelValue(this, ref _selfIntersect, value, v => SelfIntersect = v, "SelfIntersect"); }
        }


        public override IfcDimensionCount Dim
        {
            get
            {
                IfcCartesianPoint first = ControlPointsList.FirstOrDefault();
                return first != null ? first.Dim : (IfcDimensionCount) 0;
            }
        }

        /// <summary>
        ///   The array of control points used to define the geometry of the curve. This is derived from the list of control points.
        /// </summary>
        public IfcCartesianPoint[] ControlPoints
        {
            get
            {
                if (_controlPointsList.Count > 256)
                    return null;
                else
                    return _controlPointsList.ToArray();
            }
        }

        /// <summary>
        ///   The upper index on the array of control points; the lower index is 0. This value is derived from the control points list.
        /// </summary>
        public int UpperIndexOnControlPoints
        {
            get { return _controlPointsList.Count - 1; }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _degree = (int) value.IntegerVal;
                    break;
                case 1:
                    _controlPointsList.Add((IfcCartesianPoint)value.EntityVal);
                    break;
                case 2:
                    _curveForm = (IfcBSplineCurveForm) Enum.Parse(typeof (IfcBSplineCurveForm), value.EnumVal, true);
                    break;
                case 3:
                    _closedCurve = value.BooleanVal;
                    break;
                case 4:
                    _selfIntersect = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            IfcDimensionCount dim = Dim;
            foreach (IfcCartesianPoint item in _controlPointsList)
            {
                if (item.Dim != dim)
                    return "WR41 BSplineCurve : All control points shall have the same dimensionality.";
            }
            return "";
        }
    }
}