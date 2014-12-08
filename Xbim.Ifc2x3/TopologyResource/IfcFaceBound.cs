#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFaceBound.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A face bound is a loop which is intended to be used for bounding a face.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A face bound is a loop which is intended to be used for bounding a face. 
    ///   NOTE Corresponding STEP entity: face_bound. Please refer to ISO/IS 10303-42:1994, p. 139 for the final definition of the formal standard. 
    ///   HISTORY New class in IFC Release 1.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcFaceBound : IfcTopologicalRepresentationItem, IBoundary
    {
        #region Fields

        private IfcLoop _bound;
        private IfcBoolean _orientation = true;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The loop which will be used as a face boundary.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcLoop Bound
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _bound;
            }
            set { this.SetModelValue(this, ref _bound, value, v => Bound = v, "Bound"); }
        }

        /// <summary>
        ///   This indicated whether (TRUE) or not (FALSE) the loop has the same sense when used to bound the face as when first defined. If sense is FALSE the senses of all its component oriented edges are implicitly reversed when used in the face.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcBoolean Orientation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _orientation;
            }
            set { this.SetModelValue(this, ref _orientation, value, v => Orientation = v, "Orientation"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _bound = (IfcLoop) value.EntityVal;
                    break;
                case 1:
                    _orientation = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }

        #region IBoundary Members

        public IEnumerable<IVertex3D> ClockWiseVertices
        {
            get
            {
                IfcPolyLoop polyLoop = Bound as IfcPolyLoop;
                if (polyLoop != null)
                {
                    if (Orientation)
                        return polyLoop.Polygon.Cast<IVertex3D>();
                    else
                        return polyLoop.Polygon.Cast<IVertex3D>().Reverse();
                }
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IBoundary Members

        public bool IsOuterBound
        {
            get { return (this is IfcFaceOuterBound); }
        }

        #endregion

        internal IVector3D Normal()
        {
            IfcPolyLoop polyLoop = Bound as IfcPolyLoop;
            if (polyLoop != null)
            {
                int numPts = polyLoop.Polygon.Count;
                for (int i = 1; i < polyLoop.Polygon.Count; i++)
                {
                    XbimPoint3D c = polyLoop.Polygon[i].XbimPoint3D();
                    XbimPoint3D p = polyLoop.Polygon[i - 1].XbimPoint3D();
                    XbimPoint3D n = polyLoop.Polygon[(i + 1 == numPts) ? 0 : i + 1].XbimPoint3D();
                    XbimVector3D left = c - p;
                    XbimVector3D right = n - c;
                    XbimVector3D cp = XbimVector3D.CrossProduct(left, right);
                    cp.Normalize();
                    if (!double.IsNaN(cp.X)) //happens if the three points are in a straigh line
                    {
                        if (!Orientation)
                            cp.Negate();
                        return new IfcDirection(cp);
                    }
                    else if (i == polyLoop.Polygon.Count - 1)
                    {
                        // if its the last round of for look then just return the last cp
                        return new IfcDirection(cp);
                    }
                }
                //srl removed the exception to stop invalid faces from causing a crash, an invalid normal is returned which can be checked for
                return new IfcDirection(0, 0, 0); //return an invalid normal as the face is either a line or a point
                //throw new Exception("IfcFaceBound:Normal has an Invalid face");
            }
            else
                //srl removed the exception to stop invalid faces from causing a crash, an invalid normal is returned which can be checked for
                return new IfcDirection(0, 0, 0); //return an invalid normal as the face is either a line or a point
               // throw new Exception("IfcFaceBound:Normal has an undefined bound");
        }
    }
}
