#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcHalfSpaceSolid.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A half space solid is defined by the half space which is the regular subset of the domain which lies on one side of an unbounded surface.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A half space solid is defined by the half space which is the regular subset of the domain which lies on one side of an unbounded surface. The side of the surface which is in the half space is determined by the surface normal and the agreement flag. If the agreement flag is TRUE, then the subset is the one the normal points away from. If the agreement flag is FALSE, then the subset is the one the normal points into. For a valid half space solid the surface shall divide the domain into exactly two subsets. Also, within the domain the surface shall be manifold and all surface normals shall point into the same subset. 
    ///   NOTE A half space is not a subtype of solid model (IfcSolidModel), half space solids are only useful as operands in Boolean expressions.
    ///   NOTE Corresponding STEP entity: half_space_solid. Please refer to ISO/IS 10303-42:1994, p. 185 for the final definition of the formal standard. The derived attribute Dim has been added at this level and was therefore demoted from the geometric_representation_item. 
    ///   HISTORY New class in IFC Release 1.5 
    ///   Informal propositions:
    ///   The base surface shall divide the domain into exactly two subsets. If the half space solid is of subtype boxed half space (IfcBoxedHalfSpace), the domain in question is that of the attribute enclosure. In all other cases the domain is all of space and the base surface shall be unbounded. 
    ///   The base surface shall be an unbounded surface (subtype of IfcElementarySurface). 
    ///   Illustration:
    ///   Definition of the IfcHalfSpaceSolid within a given coordinate system. The base surface is given by an unbounded
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcHalfSpaceSolid : IfcGeometricRepresentationItem, IfcBooleanOperand
    {
        #region Fields

        private IfcSurface _baseSurface;
        private IfcBoolean _agreementFlag;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Surface defining side of half space.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcSurface BaseSurface
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _baseSurface;
            }
            set { this.SetModelValue(this, ref _baseSurface, value, v => BaseSurface = v, "BaseSurface"); }
        }

        /// <summary>
        ///   The agreement flag is TRUE if the normal to the BaseSurface points away from the material of the IfcHalfSpaceSolid. Otherwise it is FALSE.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcBoolean AgreementFlag
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _agreementFlag;
            }
            set { this.SetModelValue(this, ref _agreementFlag, value, v => AgreementFlag = v, "AgreementFlag"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _baseSurface = (IfcSurface) value.EntityVal;
                    break;
                case 1:
                    _agreementFlag = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   The space dimensionality of this class, it is always 3
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return new IfcDimensionCount(3); }
        }

        public override string WhereRule()
        {
            return "";
        }

        int IfcBooleanOperand.Dim
        {
            get { return Dim; }
        }
    }
}