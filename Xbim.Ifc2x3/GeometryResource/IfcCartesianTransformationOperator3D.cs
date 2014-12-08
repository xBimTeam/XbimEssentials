#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperator3D.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A Cartesian transformation operator 3d defines a geometric transformation in three-dimensional space composed of translation, rotation, mirroring and uniform scaling.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A Cartesian transformation operator 3d defines a geometric transformation in three-dimensional space composed of translation, rotation, mirroring and uniform scaling. The list of normalised vectors u defines the columns of an orthogonal matrix T. These vectors are computed from the direction attributes axis1, axis2 and axis3 by the base axis function. If |T|= -1, the transformation includes mirroring. 
    ///   NOTE: Corresponding STEP entity : cartesian_transformation_operator_3d, please refer to ISO/IS 10303-42:1994, p. 33 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 2x.
    ///   Formal Propositions:
    ///   WR1   :   The coordinate space dimensionality of this entity shall be 3.  
    ///   WR2   :   The inherited Axis1 should have (if given) the dimensionality of 3.  
    ///   WR3   :   The inherited Axis2 should have (if given) the dimensionality of 3.  
    ///   WR4   :   The Axis3 should have (if given) the dimensionality of 3.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcCartesianTransformationOperator3D : IfcCartesianTransformationOperator
    {
        #region Fields

        private IfcDirection _axis3;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. The exact direction of U[3], the derived Z axis direction.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcDirection Axis3
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _axis3;
            }
            set { this.SetModelValue(this, ref _axis3, value, v => Axis3 = v, "Axis3"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _axis3 = (IfcDirection) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived. The list of mutually orthogonal, normalised vectors defining the transformation matrix T. 
        ///   They are derived from the explicit attributes Axis3, Axis1, and Axis2 in that order.
        /// </summary>
        public List<IfcDirection> U
        {
            get { throw new NotImplementedException(); }
        }
    }
}