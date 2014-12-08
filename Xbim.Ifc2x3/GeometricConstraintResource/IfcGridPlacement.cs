#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGridPlacement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    [IfcPersistedEntityAttribute]
    public class IfcGridPlacement : IfcObjectPlacement
    {
        #region Fields

        private IfcVirtualGridIntersection _placementLocation;
        private IfcVirtualGridIntersection _placementRefDirection;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   A constraint on one or both ends of the path for an ExtrudedSolid
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcVirtualGridIntersection PlacementLocation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _placementLocation;
            }
            protected set
            {
                this.SetModelValue(this, ref _placementLocation, value, v => PlacementLocation = v,
                                           "PlacementLocation ");
            }
        }


        /// <summary>
        ///   Reference to a second grid axis intersection, which defines the orientation of the grid placement
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcVirtualGridIntersection PlacementRefDirection
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _placementRefDirection;
            }
            protected set
            {
                this.SetModelValue(this, ref _placementRefDirection, value, v => PlacementRefDirection = v,
                                           "PlacementRefDirection ");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _placementLocation = (IfcVirtualGridIntersection) value.EntityVal;
                    break;
                case 1:
                    _placementRefDirection = (IfcVirtualGridIntersection) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        internal override void CopyValues(IfcObjectPlacement value)
        {
            IfcGridPlacement gp = value as IfcGridPlacement;
            PlacementLocation = gp.PlacementLocation;
            PlacementRefDirection = gp.PlacementRefDirection;
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}
