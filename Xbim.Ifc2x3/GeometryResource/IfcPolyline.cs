#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPolyline.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;

using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class IfcPolyline : IfcBoundedCurve
    {
        public IfcPolyline()
        {
            _points = new CartesianPointList(this);
        }

        #region Part 21 Step file Parse routines

        private CartesianPointList _points;


        /// <summary>
        ///   The points defining the polyline. Use only for Ifc compatibility
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, 2)]
        public CartesianPointList Points
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _points;
            }
            set { this.SetModelValue(this, ref _points, value, v => Points = v, "Points"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
            {
                _points.Add((IfcCartesianPoint) value.EntityVal);
            }
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        /// <summary>
        ///   The dimensionality of all points must be the same and is equal to the forst point
        /// </summary>
        
        public override IfcDimensionCount Dim
        {
            get { return _points.Count > 0 ? _points[0].Dim : (IfcDimensionCount) 0; }
        }


        
        public bool IsClosed
        {
            get
            {
                if (_points.Count < 2) return false;
                IfcCartesianPoint start = _points.First();
                IfcCartesianPoint end = _points.Last();
                return start == end;
            }
        }


        public override string WhereRule()
        {
            bool first = true;
            int dim = 0;
            foreach (IfcCartesianPoint pt in _points)
            {
                if (first)
                {
                    dim = pt.Dim;
                    first = false;
                }
                else if (pt.Dim != dim)
                    return "WR41 Polyline : The space dimensionality of all Points shall be the same. ";
            }
            return "";
        }
    }
}