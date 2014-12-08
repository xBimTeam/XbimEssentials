#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRevolvedAreaSolid.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntityAttribute]
    public class IfcRevolvedAreaSolid : IfcSweptAreaSolid
    {
        /// <summary>
        ///   Axis about which revolution will take place.
        /// </summary>
        private IfcAxis1Placement _Axis;

        [IfcAttribute(3, IfcAttributeState.Mandatory)] //, IsIntroduced = IfcSchemaVersion.IFC2x3)]
            public IfcAxis1Placement Axis
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _Axis;
            }
            set { this.SetModelValue(this, ref _Axis, value, v => Axis = v, "Axis"); }
        }

        /// <summary>
        ///   Angle through which the sweep will be made. This angle is measured from the plane of the sweep.
        /// </summary>
        private IfcPlaneAngleMeasure _Angle;

        [IfcAttribute(4, IfcAttributeState.Mandatory)] // , IsIntroduced = IfcSchemaVersion.IFC2x3)]
            public IfcPlaneAngleMeasure Angle
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _Angle;
            }
            set { this.SetModelValue(this, ref _Angle, value, v => Angle = v, "Angle"); }
        }

        /// <summary>
        ///   The line of the axis of revolution.
        /// </summary>
        public IfcLine AxisLine
        {
            get
            {
                // TODO: Cla: Implement derived Axisline properly... should I use a modelmanager?

                // AxisLine	 : 	IfcLine :=  IfcRepresentationItem() || 
                //      IfcGeometricRepresentationItem () || 
                //      IfcCurve() || 
                //      IfcLine(
                //          Axis.Location, 
                //          IfcRepresentationItem() || IfcGeometricRepresentationItem () || IfcVector(Axis.Z,1.0)
                //      );

                if (_Axis != null)
                {
                    IfcLine line = new IfcLine();
                    line.Pnt = _Axis.Location;
                    line.Dir = new IfcVector(_Axis.Z, 1.0);
                    return line;
                }
                else //no valid match
                    return null; // todo: Cla: is this a right default?
            }
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
                    _Axis = (IfcAxis1Placement) value.EntityVal;
                    break;
                case 3:
                    _Angle = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_Axis.Location.Coordinates[2] != 0)
                baseErr +=
                    "WR31 IfcRevolvedAreaSolid : The start of the axis shall lie in the XY plane of the position coordinate system.\n";
            if (_Axis.Z.DirectionRatios[2] != 0)
                baseErr +=
                    "WR32 IfcRevolvedAreaSolid : The direction of the axis shall be parallel to the XY plane of the position coordinate system.\n";

            return baseErr;
        }
    }
}