#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcParameterizedProfileDef.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProfileResource
{
    /// <summary>
    ///   The parameterized profile definition defines a 2D position coordinate system to which the parameters of the different profiles relate to.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The parameterized profile definition defines a 2D position coordinate system to which the parameters of the different profiles relate to. All profiles are defined centric to the origin of the position coordinate system, or more specific, the origin [0.,0.] shall be in the center of the bounding box gravity of the profile.
    ///   The Position attribute of the IfcParameterizedProfileDef is used to position the profile within the XY plane of the underlying coordinate system of the swept surface geometry, the swept area solid or the sectioned spine. It can be used to position the profile at any cardinal point that becomes the origin [0.,0.,0.] of the extruded or rotated surface or solid.
    ///   HISTORY  New entity in Release IFC2x Edition 2.
    ///   IFC2x Platform CHANGE  The IfcParameterizedProfileDef is introduced as an intermediate new abstract entity that unifies the definition and usage of the position coordinate system for all parameterized profiles. The Position attribute has been removed at all subtypes (like IfcRectangleProfileDef, IfcCircleProfileDef, etc.).
    ///   IFC2x Edition 3 CHANGE  All profile origins are now in the center of the bounding box.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcParameterizedProfileDef : IfcProfileDef
    {
        #region Fields

        private IfcAxis2Placement2D _position;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Position coordinate system of the parameterized profile definition.
        /// </summary>

        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement2D Position
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
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _position = (IfcAxis2Placement2D) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}