#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDoor.cs
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
    ///   Construction for closing an opening, intended primarily for access with hinged, pivoted or sliding operation.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Construction for closing an opening, intended primarily for access with hinged, pivoted or sliding operation. 
    ///   Definition from IAI: The door includes constructions with revolving and folding operations. A door consists of a lining and one or several panels, properties concerning the lining and panel are defined by the IfcDoorLiningProperties and the IfcDoorPanelProperties.
    ///   The door entity, IfcDoor, defines a particular occurrence of a door inserted in the spatial context of a project. A door can:
    ///   either be inserted as a filler in an opening, then the IfcDoor has an inverse attribute FillsVoids provided, 
    ///   or be a "free standing" door, then the IfcDoor has no inverse attribute FillsVoids provided. 
    ///   The actual parameter of the door and/or its shape are defined by the IfcDoor as the occurrence definition (or project instance), or by the IfcDoorStyle as the specific definition (or project type). Parameters are given:
    ///   at the IfcDoor for occurrence specific parameters. The IfcDoor specifies: 
    ///   the door width and height 
    ///   the door opening direction (by the y-axis of the ObjectPlacement) 
    ///   at the IfcDoorStyle, to which the IfcDoor is related by the inverse relationship IsDefinedBy pointing to IfcRelDefinesByType, for style parameters common to all occurrences of the same style. 
    ///   the operation type (single swing, double swing, revolving, etc.) 
    ///   the door hinge side (by using two different styles for right and left opening doors) 
    ///   the construction type 
    ///   the particular attributes for the lining by the IfcDoorLiningProperties 
    ///   the particular attributes for the panels by the IfcDoorPanelProperties 
    ///   The IfcDoor is normally inserted into an IfcOpeningElement (but does not need to - see above) using the IfcRelFillsElement relationship. It is also directly linked to the spatial structure of the project (and here normally to the IfcBuildingStorey) using the IfcRelContainedInSpatialStructure relationship.
    ///   HISTORY New entity in IFC Release 1.0.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcDoor are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcDoor are part of this IFC release:
    ///   Pset_DoorCommon: common property set for all door occurrences 
    ///   Pset_DoorWindowGlazingType: specific property set for the glazing properties of the door glazing, if available 
    ///   Pset_DoorWindowShadingType: specific property set for the shading properties of the door glazing, if available 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcDoor is given by the IfcProductDefinitionShape, allowing multiple geometric representations. The IfcDoor, in case of an occurrance object, gets its parameter and shape from the IfcDoorStyle. If an IfcRepresentationMap (a block definition) is defined for the IfcDoorStyle, then the IfcDoor inserts it through the IfcMappedItem.
    ///   Local Placement
    ///   The local placement for IfcDoor is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point to the local placement of the same element (if given), in which the IfcDoor is used as a filling (normally an IfcOpeningElement), as provided by the IfcRelFillsElement relationship. 
    ///   If the IfcDoor is not inserted into an IfcOpeningElement, then the PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Door opening operation by door style
    ///   The parameters that defines the shape of the IfcDoor, are given at the IfcDoorStyle and the property sets, which are included in the IfcDoorStyle. The IfcDoor only defines the local placement which determines the opening direction of the door. The overall size of the IfcDoor is determined by its OverallWidth and OverallHeight parameter. If omitted, it should be taken from the profile of the IfcOpeningElement, in which the IfcDoor is inserted.
    ///   The door panel (for swinging doors) opens always into the direction of the positive Y axis of the local placement. The determination of whether the door opens to the left or to the right is done at the level of the IfcDoorStyle. Here it is a left side opening door given by IfcDoorStyle.OperationType = SingleSwingLeft 
    ///   If the door should open to the other side, then the local placement has to be changed. It is still a left side opening door, given by IfcDoorStyle.OperationType = SingleSwingLeft 
    ///   If the door panel (for swinging doors) opens to the right, a separate door style needs to be used (here IfcDoorStyle.OperationType = SingleSwingRight) and it always opens into the direction of the positive Y axis of the local placement.  
    ///   If the door panel (for swinging doors) opens to the right, and into the opposite directions, the local placement of the door need to change. The door style is given by IfcDoorStyle.OperationType = SingleSwingRight. 
    ///   EXPRESS specification:
    ///   ENTITY IfcDoor  
    ///   SUBTYPE OF (  IfcBuildingElement);  
    ///   OverallHeight   :   OPTIONAL IfcPositiveLengthMeasure;  
    ///   OverallWidth   :   OPTIONAL IfcPositiveLengthMeasure;  
    ///  
    ///  
    ///   END_ENTITY;  
    ///  
    ///   Attribute definitions:
    ///   OverallHeight   :   Overall measure of the height, it reflects the Z Dimension of a bounding box, enclosing the body of the door opening. If omitted, the OverallHeight should be taken from the geometric representation of the IfcOpening in which the door is inserted. 
    ///   NOTE  The body of the door might be taller then the door opening (e.g. in cases where the door lining includes a casing). In these cases the OverallHeight shall still be given as the door opening height, and not as the total height of the door lining. 
    ///   OverallWidth   :   Overall measure of the width, it reflects the X Dimension of a bounding box, enclosing the body of the door opening. If omitted, the OverallWidth should be taken from the geometric representation of the IfcOpening in which the door is inserted. 
    ///   NOTE  The body of the door might be wider then the door opening (e.g. in cases where the door lining includes a casing). In these cases the OverallWidth shall still be given as the door opening width, and not as the total width of the door lining.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcDoor : IfcBuildingElement
    {
        #region Fields

        private IfcPositiveLengthMeasure? _overallHeight;
        private IfcPositiveLengthMeasure? _overallWidth;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Overall measure of the height, it reflects the Z Dimension of a bounding box, enclosing the body of the door opening. If omitted, the OverallHeight should be taken from the geometric representation of the IfcOpening in which the door is inserted.
        /// </summary>
        /// <remarks>
        ///   NOTE  The body of the door might be taller then the door opening (e.g. in cases where the door lining includes a casing). In these cases the OverallHeight shall still be given as the door opening height, and not as the total height of the door lining.
        /// </remarks>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? OverallHeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overallHeight;
            }
            set { this.SetModelValue(this, ref _overallHeight, value, v => OverallHeight = v, "OverallHeight"); }
        }


        /// <summary>
        ///   Optional. Overall measure of the width, it reflects the X Dimension of a bounding box, enclosing the body of the door opening. If omitted, the OverallWidth should be taken from the geometric representation of the IfcOpening in which the door is inserted.
        /// </summary>
        /// <remarks>
        ///   NOTE  The body of the door might be wider then the door opening (e.g. in cases where the door lining includes a casing). In these cases the OverallWidth shall still be given as the door opening width, and not as the total width of the door lining.
        /// </remarks>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? OverallWidth
        {
            get 
            { 
                ((IPersistIfcEntity)this).Activate(false);
                return _overallWidth; 
            }
            set { this.SetModelValue(this, ref _overallWidth, value, v => OverallWidth = v, "OverallWidth"); }
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