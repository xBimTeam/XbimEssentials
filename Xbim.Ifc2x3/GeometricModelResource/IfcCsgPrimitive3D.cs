#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCsgPrimitive3D.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    public abstract class IfcCsgPrimitive3D : IfcGeometricRepresentationItem, IfcBooleanOperand, IfcCsgSelect
    {
        #region Fields

        private IfcAxis2Placement3D _position;

        #endregion

        /// <summary>
        ///   Surface defining side of half space.
        /// </summary>
        [Ifc(1, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement3D Position
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _position;
            }
            set { this.SetModelValue(this, ref _position, value, v => Position = v, "Position"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _position = (IfcAxis2Placement3D) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }

        #region BooleanOperand Members

        public IfcDimensionCount Dim
        {
            get { return Dim; }
        }

        #endregion

        int IfcBooleanOperand.Dim
        {
            get { return Dim; }
        }
    }
}