#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSweptSurface.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcSweptSurface : IfcSurface, IPlacement3D
    {
        private IfcProfileDef _sweptCurve;
        private IfcAxis2Placement3D _position;

        /// <summary>
        ///   The curve to be swept in defining the surface. The curve is defined as a profile within the position coordinate system.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcProfileDef SweptCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sweptCurve;
            }
            set { this.SetModelValue(this, ref _sweptCurve, value, v => SweptCurve = v, "SweptCurve"); }
        }

        /// <summary>
        ///   Position coordinate system for the placement of the profile within the xy plane of the axis placement.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement3D Position
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _position;
            }
            set { this.SetModelValue(this, ref _position, value, v => Position = v, "Position"); }
        }

        /// <summary>
        ///   Derived.  The space dimensionality of this class, derived from the dimensionality of the Position.
        /// </summary>
        public override IfcDimensionCount Dim
        {
            get { return _position.Dim; }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _sweptCurve = (IfcProfileDef) value.EntityVal;
                    break;
                case 1:
                    _position = (IfcAxis2Placement3D) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #region IPlacement3D Members

        IfcAxis2Placement3D IPlacement3D.Position
        {
            get { return this.Position; }
        }

        #endregion

        public override string WhereRule()
        {
            string err = "";
            if (_sweptCurve is IfcDerivedProfileDef)
                err += "WR 1 SweptSurface : Swept curves of type  DerivedProfileDef are not permitted\n";
            if (_sweptCurve.ProfileType != IfcProfileTypeEnum.CURVE)
                err += "WR 2 SweptSurface : The profileType of swept curves must be of type Curve\n";
            return err;
        }
    }
}