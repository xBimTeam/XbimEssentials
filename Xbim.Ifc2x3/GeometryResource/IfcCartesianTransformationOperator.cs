#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperator.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A Cartesian transformation operator defines a geometric transformation composed of translation, rotation, mirroring and uniform scaling.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A Cartesian transformation operator defines a geometric transformation composed of translation, rotation, mirroring and uniform scaling. 
    ///   The list of normalised vectors u defines the columns of an orthogonal matrix T. 
    ///   These vectors are computed, by the base axis function, from the direction attributes axis1, axis2 and, in Cartesian transformation operator 3d, axis3. If |T|= -1, 
    ///   the transformation includes mirroring. The local origin point A, the scale value S and the matrix T together define a transformation. 
    ///   The transformation for a point with position vector P is defined by 
    ///   P -&gt; A + STP 
    ///   The transformation for a direction d is defined by 
    ///   d -&gt; Td 
    ///   The transformation for a vector with orientation d and magnitude k is defined by
    ///   d -&gt; Td, and 
    ///   k -&gt; Sk 
    ///   For those entities whose attributes include an axis2 placement, the transformation is applied, after the derivation, 
    ///   to the derived attributes p defining the placement coordinate directions. 
    ///   For a transformed surface, the direction of the surface normal at any point is obtained by transforming the normal, at the corresponding point, 
    ///   to the original surface. For geometric entities with attributes (such as the radius of a circle) which have the dimensionality of length, 
    ///   the values will be multiplied by S. 
    ///   For curves on surface the p curve.reference to curve will be unaffected by any transformation. 
    ///   The Cartesian transformation operator shall only be applied to geometry defined in a consistent system of units with the same units on each axis. 
    ///   With all optional attributes omitted, the transformation defaults to the identity transformation. 
    ///   The Cartesian transformation operator shall only be instantiated as one of its subtypes. 
    ///   NOTE: Corresponding STEP entity : cartesian_transformation_operator, please refer to ISO/IS 10303-42:1994, p. 32 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcCartesianTransformationOperator : IfcGeometricRepresentationItem
    {
        private IfcDirection _axis1;
        private IfcDirection _axis2;
        private IfcCartesianPoint _localOrigin;
        private double? _scale;

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional.   The direction used to determine U[1], the derived X axis direction.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcDirection Axis1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _axis1;
            }
            set { this.SetModelValue(this, ref _axis1, value, v => Axis1 = v, "Axis1"); }
        }

        /// <summary>
        ///   Optional. The direction used to determine U[2], the derived Y axis direction.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcDirection Axis2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _axis2;
            }
            set { this.SetModelValue(this, ref _axis2, value, v => Axis2 = v, "Axis2"); }
        }

        /// <summary>
        ///   The required translation, specified as a cartesian point. The actual translation included in the transformation is from the geometric origin to the local origin.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcCartesianPoint LocalOrigin
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _localOrigin;
            }
            set { this.SetModelValue(this, ref _localOrigin, value, v => LocalOrigin = v, "LocalOrigin"); }
        }


        /// <summary>
        ///   Optional. The scaling value specified for the transformation.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public double? Scale
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _scale;
            }
            set { this.SetModelValue(this, ref _scale, value, v => Scale = v, "Scale"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _axis1 = (IfcDirection) value.EntityVal;
                    break;
                case 1:
                    _axis2 = (IfcDirection) value.EntityVal;
                    break;
                case 2:
                    _localOrigin = (IfcCartesianPoint) value.EntityVal;
                    break;
                case 3:
                    _scale = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived. The derived scale S of the transformation, equal to scale if that exists, or 1.0 otherwise.
        /// </summary>
        public double Scl
        {
            get { return Scale.HasValue ? Scale.Value : 1.0; }
        }

        /// <summary>
        ///   Derived.   The space dimensionality of this class, determined by the space dimensionality of the local origin.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return LocalOrigin.Dim; }
        }


        public override string WhereRule()
        {
            if (Scl <= 0)
                return "WR1 CartesianTransformationOperator : The derived scaling Scl shall be greater than zero\n";
            else
                return "";
        }
    }
}