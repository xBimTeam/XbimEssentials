#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundaryNodeCondition.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    /// <summary>
    ///   The entity IfcBoundaryNodeCondition describes boundary conditions that can be applied to structural point connections, 
    ///   either directly for the connection (e.g. the joint) or for the relation between a structural member and the connection. 
    ///   The following conventions to the values of the LinearStiffness[X,Y,Z] and RotationalStiffness[X,Y,Z] apply:
    ///   value (-1.) represents an infinitive large value – or a fixed connectivity with infinitive stiffness 
    ///   value zero (0.), represents no stiffness or a free connectivity 
    ///   any other value represents a finitive stiffness or spring connectivity in that direction or rotation 
    ///   value NIL ($) represents an unknown connectivity condition 
    ///   NOTE: Instances of the entity IfcBoundaryNodeCondition are used e.g., to define the boundary condition for instances of 
    ///   IfcStructuralPointConnection or IfcRelConnectsStructuralMember pointing to a structural node connection.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcBoundaryNodeCondition : IfcBoundaryCondition
    {
        #region Fields

        private IfcLinearStiffnessMeasure? _linearStiffnessX;
        private IfcLinearStiffnessMeasure? _linearStiffnessY;
        private IfcLinearStiffnessMeasure? _linearStiffnessZ;
        private IfcRotationalStiffnessMeasure? _rotationalStiffnessX;
        private IfcRotationalStiffnessMeasure? _rotationalStiffnessY;
        private IfcRotationalStiffnessMeasure? _rotationalStiffnessZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Linear stiffness value in x-direction of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLinearStiffnessMeasure? LinearStiffnessX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearStiffnessX;
            }
            set
            {
                this.SetModelValue(this, ref _linearStiffnessX, value, v => LinearStiffnessX = v,
                                           "LinearStiffnessX");
            }
        }

        /// <summary>
        ///   Linear stiffness value in y-direction of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLinearStiffnessMeasure? LinearStiffnessY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearStiffnessY;
            }
            set
            {
                this.SetModelValue(this, ref _linearStiffnessY, value, v => LinearStiffnessY = v,
                                           "LinearStiffnessY");
            }
        }

        /// <summary>
        ///   Linear stiffness value in z-direction of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLinearStiffnessMeasure? LinearStiffnessZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearStiffnessZ;
            }
            set
            {
                this.SetModelValue(this, ref _linearStiffnessZ, value, v => LinearStiffnessZ = v,
                                           "LinearStiffnessZ");
            }
        }

        /// <summary>
        ///   Rotational stiffness value about the x-axis of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcRotationalStiffnessMeasure? RotationalStiffnessX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rotationalStiffnessX;
            }
            set
            {
                this.SetModelValue(this, ref _rotationalStiffnessX, value, v => RotationalStiffnessX = v,
                                           "RotationalStiffnessX");
            }
        }

        /// <summary>
        ///   Rotational stiffness value about the y-axis of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcRotationalStiffnessMeasure? RotationalStiffnessY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rotationalStiffnessY;
            }
            set
            {
                this.SetModelValue(this, ref _rotationalStiffnessY, value, v => RotationalStiffnessY = v,
                                           "RotationalStiffnessY");
            }
        }

        /// <summary>
        ///   Rotational stiffness value about the z-axis of the coordinate system defined by the instance which uses this resource object.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcRotationalStiffnessMeasure? RotationalStiffnessZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rotationalStiffnessZ;
            }
            set
            {
                this.SetModelValue(this, ref _rotationalStiffnessZ, value, v => RotationalStiffnessZ = v,
                                           "RotationalStiffnessZ");
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _linearStiffnessX = value.RealVal;
                    break;
                case 2:
                    _linearStiffnessY = value.RealVal;
                    break;
                case 3:
                    _linearStiffnessZ = value.RealVal;
                    break;
                case 4:
                    _rotationalStiffnessX = value.RealVal;
                    break;
                case 5:
                    _rotationalStiffnessY = value.RealVal;
                    break;
                case 6:
                    _rotationalStiffnessZ = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}