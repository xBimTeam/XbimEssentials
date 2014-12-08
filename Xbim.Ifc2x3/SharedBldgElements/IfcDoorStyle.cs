#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDoorStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   The door style, IfcDoorStyle, defines a particular style of doors, which may be included into the spatial context of the building model through an (or multiple) instances of IfcDoor.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The door style, IfcDoorStyle, defines a particular style of doors, which may be included into the spatial context of the building model through an (or multiple) instances of IfcDoor. A door style defines the overall parameter of the door style and refers to the particular parameter of the lining and one (or several) panels through the IfcDoorLiningProperties and the IfcDoorPanelProperties.
    ///   The door entity, IfcDoor, defines a particular occurrence of a door inserted in the spatial context of a project. The actual parameter of the door and/or its shape is defined at the IfcDoorStyle, to which the IfcDoor is related by the inverse relationship IsDefinedBy pointing to IfcRelDefinedByType. The IfcDoorStyle also defines the particular attributes for the lining, IfcDoorLiningProperties, and panels, IfcDoorPanelProperties.
    ///   HISTORY  New entity in IFC Release 2x.
    ///   IFC2x Edition 2 COMPATIBILITY NOTICE  The entity IfcDoorStyle is still subtyped from the IfcTypeProduct to provide upward compatibility. This is a recorded anomaly as all other types for building elements are now subtyped from IfcBuildingElementType and have the suffix "Type", not "Style". 
    ///   Geometry Use Definitions:
    ///   The IfcDoorStyle defines the baseline geometry, or the representation map, for all occurrences of the door style, given by the IfcDoor, pointing to this style. The representation of the door style may be given by the agreed set of minimal parameters, defined for the door lining and the door panel(s), or it may be given by a geometric representation used by the IfcRepresentationMap. The attribute ParameterTakesPrecedence decides, whether the set of parameters can be used to exactly represent the shape of the door style (TRUE), or whether the attached IfcRepresentationMap holds the exact representation (FALSE).
    ///   Interpretation of parameter
    ///   The IfcDoorStyleOperationTypeEnum defines the general layout of the door style. Depending on the enumerator, the appropriate instances of IfcDoorLiningProperties and IfcDoorPanelProperties are attached in the list of HasPropertySets. The IfcDoorStyleOperationTypeEnum mainly determines the hinge side (left hung, or right hung), the operation (swinging, sliding, folding, etc.) and the number of panels.
    ///   See geometry use definitions at IfcDoorStyleOperationTypeEnum for the correct usage of opening symbols for different operation types. 
    ///   EXPRESS specification
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcDoorStyle : IfcTypeProduct
    {
        #region Fields

        private IfcDoorStyleOperationEnum _operationType = IfcDoorStyleOperationEnum.NOTDEFINED;
        private IfcDoorStyleConstructionEnum _constructionType = IfcDoorStyleConstructionEnum.NOTDEFINED;
        private bool _parameterTakesPrecedence;
        private bool _sizeable;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Type defining the general layout and operation of the door style.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcDoorStyleOperationEnum OperationType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _operationType;
            }
            set { this.SetModelValue(this, ref _operationType, value, v => OperationType = v, "OperationType"); }
        }

        /// <summary>
        ///   Type defining the basic construction and material type of the door.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcDoorStyleConstructionEnum ConstructionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _constructionType;
            }
            set
            {
                this.SetModelValue(this, ref _constructionType, value, v => ConstructionType = v,
                                           "ConstructionType");
            }
        }

        /// <summary>
        ///   The Boolean value reflects, whether the parameter given in the attached lining and panel properties exactly define the geometry (TRUE), or whether the attached style shape take precedence (FALSE). In the last case the parameter have only informative value.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public bool ParameterTakesPrecedence
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _parameterTakesPrecedence;
            }
            set
            {
                this.SetModelValue(this, ref _parameterTakesPrecedence, value, v => ParameterTakesPrecedence = v,
                                           "ParameterTakesPrecedence");
            }
        }

        /// <summary>
        ///   The Boolean indicates, whether the attached IfcMappedRepresentation (if given) can be sized (using scale factor of transformation), or not (FALSE). If not, the IfcMappedRepresentation should be IfcShapeRepresentation of the IfcDoor (using IfcMappedItem as the Item) with the scale factor = 1.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public bool Sizeable
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sizeable;
            }
            set { this.SetModelValue(this, ref _sizeable, value, v => Sizeable = v, "Sizeable"); }
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
                    _operationType =
                        (IfcDoorStyleOperationEnum) Enum.Parse(typeof (IfcDoorStyleOperationEnum), value.EnumVal, true);
                    break;
                case 9:
                    _constructionType =
                        (IfcDoorStyleConstructionEnum)
                        Enum.Parse(typeof (IfcDoorStyleConstructionEnum), value.EnumVal, true);
                    break;
                case 10:
                    _parameterTakesPrecedence = value.BooleanVal;
                    break;
                case 11:
                    _sizeable = value.BooleanVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}