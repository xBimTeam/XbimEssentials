#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAxis2Placement3D.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

using Xbim.Common.Geometry;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   The location and orientation in three dimensional space of three mutually perpendicular axes.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: The location and orientation in three dimensional space of three mutually perpendicular axes. An axis2_placement_3D is defined in terms of a point (inherited from placement supertype) and two (ideally orthogonal) axes. It can be used to locate and originate an object in three dimensional space and to define a placement coordinate system. The entity includes a point which forms the origin of the placement coordinate system. Two direction vectors are required to complete the definition of the placement coordinate system. The axis is the placement Z axis direction and the ref_direction is an approximation to the placement X axis direction. 
    ///   Definition from IAI: If the attribute values for Axis and RefDirection are not given, the placement defaults to P[1] (x-axis) as [1.,0.,0.], P[2] (y-axis) as [0.,1.,0.] and P[3] (z-axis) as [0.,0.,1.].  
    ///   NOTE  Corresponding STEP name: axis2_placement_3d, please refer to ISO/IS 10303-42:1994 for the final definition of the formal standard. The WR5 is added to ensure that either both attributes Axis and RefDirection are given, or both are omitted.
    ///   HISTORY  New entity in IFC Release 1.5.
    ///   Illustration
    ///   Definition of the IfcAxis2Placement3D
    ///   Formal Propositions:
    ///   WR1   :   The dimensionality of the placement location shall be 3.  
    ///   WR2   :   The Axis when given should only reference a three-dimensional IfcDirection.  
    ///   WR3   :   The RefDirection when given should only reference a three-dimensional IfcDirection.  
    ///   WR4   :   The Axis and RefDirection shall not be parallel or anti-parallel.  
    ///   WR5   :   Either both (Axis and RefDirection) are not given and therefore defaulted, or both shall be given. This is a further constraint in IFC Release 1.5.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcAxis2Placement3D : IfcPlacement, IfcAxis2Placement
    {
        #region Fields

        private IfcDirection _axis;
        private IfcDirection _refDirection;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. The exact direction of the local Z Axis.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcDirection Axis
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _axis;
            }
            set { this.SetModelValue(this, ref _axis, value, v => Axis = v, "Axis"); }
        }

        /// <summary>
        ///   Optional. The direction used to determine the direction of the local X Axis. If necessary an adjustment is made to maintain orthogonality to the Axis direction. If Axis and/or RefDirection is omitted, these directions are taken from the geometric coordinate system.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcDirection RefDirection
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _refDirection;
            }
            set { this.SetModelValue(this, ref _refDirection, value, v => RefDirection = v, "RefDirection"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _axis = (IfcDirection) value.EntityVal;
                    break;
                case 2:
                    _refDirection = (IfcDirection) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived. The normalized directions of the placement X Axis (P[0]) and the placement Y Axis (P[1]) and the placement Z Axis (P[2]).
        /// </summary>
       
        public List<XbimVector3D> P
        {
            get
            {
                List<XbimVector3D> p = new List<XbimVector3D>(3);
                if (RefDirection == null && Axis == null)
                {
                    p.Add(new XbimVector3D(1, 0, 0));
                    p.Add(new XbimVector3D(0, 1, 0));
                    p.Add(new XbimVector3D(0, 0, 1));
                }
                else if (RefDirection != null && Axis != null)
                {
                    XbimVector3D za = _axis.XbimVector3D();
                    za.Normalize();
                    XbimVector3D xa = _refDirection.XbimVector3D();
                    xa.Normalize();
                    XbimVector3D ya = XbimVector3D.CrossProduct(za, xa);
                    ya.Normalize();
                    p.Add(xa);
                    p.Add(ya);
                    p.Add(za);
                }
                else
                    throw new ArgumentException("RefDirection and Axis must be noth either null or both defined");
                return p;
            }
        }


        public override string ToString()
        {
            if (RefDirection != null && Axis != null)
                return string.Format("L={0}, D={1}, A={2}", Location, RefDirection, Axis);
            else
                return base.ToString();
        }

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string err = "";
            if (Dim != 3)
                err += "WR1 Axis2Placement3D: The dimensionality of the placement location shall be 3.";
            if (Axis != null && Axis.Dim != 3)
                err +=
                    "WR2 Axis2Placement3D: The Axis when given should only reference a three-dimensional IfcDirection.";
            if (RefDirection != null && RefDirection.Dim != 3)
                err +=
                    "WR3 Axis2Placement3D: The RefDirection when given should only reference a three-dimensional IfcDirection.";
            if (Axis != null && RefDirection != null && IfcDirection.CrossProduct(Axis, RefDirection).Magnitude <= 0)
                err += "WR4 Axis2Placement3D: The Axis and RefDirection shall not be parallel or anti-parallel. ";
            if (RefDirection != null ^ Axis != null)
                err +=
                    "WR5 Axis2Placement3D: Either both (Axis and RefDirection) are not given and therefore defaulted, or both shall be given.";

            return err;
        }

        #endregion

        public new int Dim
        {
            get { return base.Dim; }
        }
    }
}