#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcExtrudedAreaSolid.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntityAttribute]
    public class IfcExtrudedAreaSolid : IfcSweptAreaSolid
    {
        #region Fields

        private IfcDirection _extrudedDirection;
        private IfcPositiveLengthMeasure _depth;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

 
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcDirection ExtrudedDirection
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _extrudedDirection;
            }
            set
            {
                this.SetModelValue(this, ref _extrudedDirection, value, v => ExtrudedDirection = v,
                                           "ExtrudedDirection");
            }
        }


        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Depth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _depth;
            }
            set { this.SetModelValue(this, ref _depth, value, v => Depth = v, "Depth"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _extrudedDirection = (IfcDirection) value.EntityVal;
                    break;
                case 3:
                    _depth = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_extrudedDirection != null &&
                XbimVector3D.DotProduct(_extrudedDirection.XbimVector3D(), new XbimVector3D(0, 0, 1)) == 0)
                baseErr +=
                    "WR31 ExtrudedAreaSolid : The ExtrudedDirection shall not be perpendicular to the local z-axis.\n";
            return baseErr;
        }
    }
}