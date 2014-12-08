#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindow.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   Construction for closing a vertical or near vertical opening in a wall or pitched roof that will admit light and may admit fresh air.
    /// </summary>
    /// <remarks>
    ///   Definition form ISO 6707-1:1989: Construction for closing a vertical or near vertical opening in a wall or pitched roof that will admit light and may admit fresh air.
    ///   Definition from IAI: A window consists of a lining and one or several panels. Properties concerning the lining and panel(s) are defined by the IfcWindowLiningProperties and the IfcWindowPanelProperties.
    ///   The window entity (IfcWindow) defines a particular occurrence of a window inserted in the spatial context of a project. The actual parameter of the window and/or its shape is defined at the IfcWindowStyle, to which the IfcWindow is related by the inverse relationship IsDefinedBy pointing to IfcRelDefinesByType. The IfcWindowStyle also defines the particular attributes for the lining (IfcWindowLiningProperties) and panels (IfcWindowPanelProperties). Therefore:
    ///   the IfcWindow is the occurrence definition (or project instance) 
    ///   the IfcWindowStyle is the specific definition (or project type) 
    ///   The IfcWindow is normally inserted into an IfcOpeningElement (but does not need to) using the IfcRelFillsElement relationship. It is also directly linked to the spatial structure of the project (and here normally to the IfcBuildingStorey) using the IfcRelContainedInSpatialStructure relationship.
    ///   HISTORY New entity in IFC Release 1.0.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcWindow are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcWindow are part of this IFC release:
    ///   Pset_WindowCommon: common property set for all window occurrences 
    ///   Pset_DoorWindowGlazingType: specific property set for the glazing properties of the window glazing, if available 
    ///   Pset_DoorWindowShadingType: specific property set for the shading properties of the window glazing, if available 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcWindow is given by the IfcProductDefinitionShape, allowing multiple geometric representation. The IfcWindow, in case of an occurrence object, gets its parameter and shape from the IfcWindowStyle. If an IfcRepresentationMap (a block definition) is defined for the IfcWindowStyle, then the IfcWindow inserts it through the IfcMappedItem (refered to by IfcShapeRepresentation.Items).
    ///   Local Placement
    ///   The local placement for IfcWindow is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations.
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point to the local placement of the same element (if given), in which the IfcWindow is used as a filling (normally an IfcOpeningElement), as provided by the IfcRelFillsElement relationship. 
    ///   If the IfcWindow is not inserted into an IfcOpeningElement, then the PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Standard Geometric Representation using parameters given by the style
    ///   The parameters, which define the shape of the IfcWindow, are given at the IfcWindowStyle and the property sets, which are included in the IfcWindowStyle. The IfcWindow only defines the local placement. The overall size of the IfcWindow is determined by its OverallWidth and OverallHeight parameter, if omitted, it should be taken from the profile of the IfcOpening, in which the IfcWindow is inserted.
    ///   EXAMPLE Inserting the IfcWindowStyle.OperationType = DoublePanelHorizontal
    ///   The insertion of the window style into the IfcOpeningElement by creating an instance of IfcWindow. The parameter: 
    ///   OverallHeigth 
    ///   OverallWidth 
    ///   show the extend of the window in the positive Z and X axis of the local placement of the window. The lining and the transom are created by the given parameter (the flag ParameterTakesPrecedence = TRUE).
    ///   The representation type of the inserted window is
    ///   IfcShapeRepresentation.RepresentationType = 'MappedRepresentation' 
    ///  
    ///   The final window (DoublePanelHorizontal) with 
    ///   first panel
    ///   PanelPosition = TOP
    ///   OperationType = BOTTOMHUNG 
    ///   second panel
    ///   PanelPosition = BOTTOM
    ///   OperationType = TILTANDTURNLEFTHAND
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcWindow : IfcBuildingElement
    {
        #region Part 21 Step file Parse routines

        private IfcPositiveLengthMeasure? _overallHeight;
        private IfcPositiveLengthMeasure? _overallWidth;

        /// <summary>
        ///   Optional.   Overall measure of the height, it reflects the Z Dimension of a bounding box, enclosing the body of the window opening. If omitted, the OverallHeight should be taken from the geometric representation of the IfcOpening in which the window is inserted.
        /// </summary>
        /// <remarks>
        ///   NOTE  The body of the window might be taller then the window opening (e.g. in cases where the window lining includes a casing). In these cases the OverallHeight shall still be given as the window opening height, and not as the total height of the window lining.
        /// </remarks>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? OverallHeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overallHeight;
            }
            set { _overallHeight = value; }
        }

        /// <summary>
        ///   Optional. Overall measure of the width, it reflects the X Dimension of a bounding box, enclosing the body of the window opening. If omitted, the OverallWidth should be taken from the geometric representation of the IfcOpening in which the window is inserted.
        /// </summary>
        /// <remarks>
        ///   NOTE  The body of the window might be wider then the window opening (e.g. in cases where the window lining includes a casing). In these cases the OverallWidth shall still be given as the window opening width, and not as the total width of the window lining.
        /// </remarks>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? OverallWidth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overallWidth;
            }
            set { _overallWidth = value; }
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
                    _overallHeight = value.RealVal;
                    break;
                case 9:
                    _overallWidth = value.RealVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}