#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindowStyle.cs
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
    ///   The window style defines a particular style of windows, which may be included into the spatial context of the building model through an (or multiple) instances of IfcWindow.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The window style defines a particular style of windows, which may be included into the spatial context of the building model through an (or multiple) instances of IfcWindow. A window style defines the overall parameter of the window style and refers to the particular parameter of the lining and one (or several) panels through the IfcWindowLiningProperties and the IfcWindowPanelProperties.
    ///   The window entity (IfcWindow) defines a particular occurrence of a window inserted in the spatial context of a project. The actual parameter of the window and/or its shape is defined at the IfcWindowStyle, to which the IfcWindow related by the inverse relationship IsDefinedBy pointing to IfcRelDefinesByType. The IfcWindowStyle also defines the particular attributes for the lining (IfcWindowLiningProperties) and panels (IfcWindowPanelProperties).
    ///   HISTORY New entity in IFC Release 2x.
    ///   IFC2x2 COMPATIBILITY NOTICE: The entity IfcWindowStyle is still subtyped from the IfcTypeProduct to provide upward compatibility. This is a recorded anomaly as all other types for building elements are now subtyped from IfcBuildingElementType and have the suffix "Type", not "Style". 
    ///   Geometry Use Definitions:
    ///   The IfcWindowStyle defines the baseline geometry, or the representation map, for all occurrences of the window style, given by the IfcWindow, pointing to this style. The representation of the window style may be given by the agreed set of minimal parameters, defined for the window lining and the window panel(s), or it my be given by a geometric representation used by the IfcRepresentationMap. The attribute ParameterTakesPrecedence decides, whether the set of parameters can be used to exactly represent the shape of the window style (TRUE), or whether the attached IfcRepresentationMap holds the exact representation (FALSE).
    ///   Interpretation of parameters
    ///   The IfcWindowStyleOperationTypeEnum defines the general layout of the window style. Depending on the enumerator, the appropriate instances of IfcWindowLiningProperties and IfcWindowPanelProperties are attached in the list of HasPropertySets. See geometry use definitions there.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcWindowStyle : IfcTypeProduct
    {
        #region Part 21 Step file Parse routines

        private IfcWindowStyleConstructionEnum _constructionType;
        private IfcWindowStyleOperationEnum _operationType;
        private bool _parameterTakesPrecedence;
        private bool _sizeable;

        /// <summary>
        ///   Type defining the basic construction and material type of the window.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcWindowStyleConstructionEnum ConstructionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _constructionType;
            }
            set { _constructionType = value; }
        }

        /// <summary>
        ///   Type defining the general layout and operation of the window style.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcWindowStyleOperationEnum OperationType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _operationType;
            }
            set { _operationType = value; }
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
            set { _parameterTakesPrecedence = value; }
        }

        /// <summary>
        ///   The Boolean indicates, whether the attached ShapeStyle can be sized (using scale factor of transformation), or not (FALSE). If not, the ShapeStyle should be inserted by the IfcWindow (using IfcMappedItem) with the scale factor = 1.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public bool Sizeable
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sizeable;
            }
            set { _sizeable = value; }
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
                    _constructionType =
                        (IfcWindowStyleConstructionEnum)
                        Enum.Parse(typeof (IfcWindowStyleConstructionEnum), value.EnumVal, true);
                    break;
                case 9:
                    _operationType =
                        (IfcWindowStyleOperationEnum)
                        Enum.Parse(typeof (IfcWindowStyleOperationEnum), value.EnumVal, true);
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