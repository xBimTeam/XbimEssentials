#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementarySurface.cs
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
    ///   An elementary surface (IfcElementarySurface) is a simple analytic surface with defined parametric representation.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: An elementary surface (IfcElementarySurface) is a simple analytic surface with defined parametric representation. 
    ///   NOTE Corresponding STEP entity: elementary_surface. Only the subtype plane is incorporated as IfcPlane. The derived attribute Dim has been added (see also note at IfcGeometricRepresentationItem). Please refer to ISO/IS 10303-42:1994, p. 69 for the final definition of the formal standard. 
    ///   HISTORY New class in IFC Release 1.5
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcElementarySurface : IfcSurface, IPlacement3D
    {
        #region Fields

        private IfcAxis2Placement3D _position;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The position and orientation of the surface. This attribute is used in the definition of the parameterization of the surface.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
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
            if (propIndex == 0)
                _position = (IfcAxis2Placement3D) value.EntityVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        /// <summary>
        ///   The space dimensionality of this class, derived from the dimensionality of the Position.
        /// </summary>
        public override IfcDimensionCount Dim
        {
            get { return Position.Dim; }
        }

        #region IPlacement3D Members

        IfcAxis2Placement3D IPlacement3D.Position
        {
            get { return this.Position; }
        }

        #endregion
    }
}