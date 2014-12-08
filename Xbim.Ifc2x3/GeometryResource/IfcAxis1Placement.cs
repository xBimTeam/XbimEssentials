#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAxis1Placement.cs
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
    ///   The direction and location in three dimensional space of a single axis.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: The direction and location in three dimensional space of a single axis. An axis1_placement is defined in terms of a locating point (inherited from placement supertype) and an axis direction: this is either the direction of axis or defaults to (0.0,0.0,1.0). The actual direction for the axis placement is given by the derived attribute z (Z). 
    ///   NOTE: Corresponding STEP name: axis1_placement, please refer to ISO/IS 10303-42:1994, p. 28 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 1.5 
    ///   Illustration:
    ///   Definition of the IfcAxis1Placement within the three-dimensional coordinate system. 
    ///   EXPRESS specification:
    ///   Formal Propositions:
    ///   WR1   :   The Axis when given should only reference a three-dimensional IfcDirection.  
    ///   WR2   :   The Cartesian point defining the Location shall have the dimensionality of 3.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcAxis1Placement : IfcPlacement
    {
        #region Fields

        private IfcDirection _axis;

        #endregion

        /// <summary>
        ///   Optional. The direction of the local Z axis.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
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
        ///   Derived. The normalized direction of the local Z axis. It is either identical with the Axis value, if given, or it defaults to [0.,0.,1.]
        /// </summary>
        public IfcDirection Z
        {
            get
            {
                if (_axis != null)
                    return _axis.Normalise();
                else
                    return new IfcDirection(0, 0, 1);
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0) base.IfcParse(propIndex, value);
            else if (propIndex == 1)
                _axis = (IfcDirection) value.EntityVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        public override string WhereRule()
        {
            string err = "";
            if (_axis != null && _axis.Dim != 3)
                err +=
                    "WR1 Axis1Placement : The Axis when given should only reference a three-dimensional IfcDirection.\n";
            if (this.Location.Dim != 3)
                err +=
                    "WR2 Axis1Placement : The Cartesian point defining the Location shall have the dimensionality of 3\n";
            return err;
        }
    }
}