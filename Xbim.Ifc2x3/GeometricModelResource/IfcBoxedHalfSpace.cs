#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoxedHalfSpace.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   This entity is a subtype of the half space solid which is trimmed by a surrounding rectangular box.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: This entity is a subtype of the half space solid which is trimmed by a surrounding rectangular box. The box has its edges parallel to the coordinate axes of the geometric coordinate system. 
    ///   NOTE: The purpose of the box is to facilitate CSG computations by producing a solid of finite size.
    ///   Definition from IAI: The IfcBoxedHalfSpace (from ISO 10303-42:1994 boxed_half_space) is used (as its supertype IfcHalfSpaceSolid) only within Boolean operations. It divides the domain into exactly two subsets, where the domain in question is that of the attribute Enclosure. 
    ///   NOTE: Corresponding STEP entity : boxed_half_space, please refer to ISO/IS 10303-42:1994, p. 185 for the final definition of the formal standard. The IFC class IfcBoundingBox is used for the definition of the enclosure, providing the same definition as box_domain. 
    ///   HISTORY: New entity in IFC Release 1.5.1, improved documentation available in IFC Release 2x.
    ///   Illustration:
    ///   Purpose 
    ///   The IfcBoundingBox (relating to ISO 10303-42:1994 box_domain) that provides the enclosure is given for the convenience of the receiving application to enable the use of size box comparison for efficiency (e.g., to check first whether size boxes intersect, if not no cAlculations has to be done to check whether the solids of the entities intersect).
    ///   Parameter 
    ///   The Enclosure therefore helps to prevent dealing with infinite-size related issues. The enclosure box is positioned within the positioning coordinate system of the unbounded surface, given by the attribute BaseSurface (see IfcElementarySurface.Position). The AgreementFlag defines whether the box is defined into the direction of the positive z axis (FALSE) or in the direction of the negative z axis (TRUE).
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBoxedHalfSpace : IfcHalfSpaceSolid
    {
        #region Fields

        private IfcBoundingBox _enclosure;

        #endregion

        /// <summary>
        ///   The box which bounds the half space for computational purposes only.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcBoundingBox Enclosure
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _enclosure;
            }
            set { this.SetModelValue(this, ref _enclosure, value, v => Enclosure = v, "Enclosure"); }
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
                    _enclosure = (IfcBoundingBox) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}