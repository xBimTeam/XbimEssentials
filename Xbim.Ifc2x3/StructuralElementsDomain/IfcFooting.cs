#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFooting.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    /// <summary>
    ///   The geometric representation of IfcFooting is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are: 
    ///   Local Placement
    ///   The local placement for IfcFooting is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Standard Geometric Representation
    ///   Provided that it is possible the standard geometric representation of IfcFooting is defined using the swept solid representation. The RepresentationType attribute of IfcShapeRepresentation should have the value 'SweptSolid'. The following constraints apply to the standard representation: 
    ///   Solid: IfcExtrudedAreaSolid shall be supported 
    ///   Profile: All applicable profile types shall be supported 
    ///   Extrusion: All extrusion directions shall be supported. 
    ///   If it is impossible to define the geometry using the swept solid representation the representations defined in its supertype IfcBuildingElement may be used.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcFooting : IfcBuildingElement
    {
        #region Fields

        private IfcFootingTypeEnum _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Predefined shape types for a stair that are specified in an Enum.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcFootingTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _predefinedType = (IfcFootingTypeEnum) Enum.Parse(typeof (IfcFootingTypeEnum), value.EnumVal, true);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (PredefinedType == IfcFootingTypeEnum.USERDEFINED && ObjectType == null)
                baseErr +=
                    "WR1 Footing: The attribute ObjectType shall be given, if the predefined type is set to USERDEFINED.\n";
            return baseErr;
        }

        #endregion
    }
}