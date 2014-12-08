#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDoorStyleOperationEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic ways to describe how doors operate.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This enumeration defines the basic ways to describe how doors operate. 
    ///   HISTORY  New Enumeration in Release IFC2x .
    ///   Illustration
    ///   Enumerator Description Figure 
    ///   SINGLE_SWING_LEFT
    ///   Door with one panel that opens (swings) to the left. The hinges are on the left side as viewed in the direction of the positive y-axis.
    ///   Note: Direction of swing (whether in or out) is determined at the IfcDoor.
    ///   
    ///   SINGLE_SWING_RIGHT
    ///   Door with one panel that opens (swings) to the right. The hinges are on the right side as viewed in the direction of the positive y-axis.
    ///   Note: Direction of swing (whether in or out) is determined at the IfcDoor.
    ///   
    ///   DOUBLE_DOOR_
    ///   SINGLE_SWING
    ///   Door with two panels, one opens (swings) to the left the other opens (swings) to the right.
    ///   Note: Direction of swing (whether in or out) is determined at the IfcDoor. 
    ///   
    ///   DOUBLE_SWING_LEFT
    ///   Door with one panel that swings in both directions and to the left in the main trafic direction. Also called double acting door.
    ///   Note: Direction of main swing (whether in or out) is determined at the IfcDoor. 
    ///   
    ///   DOUBLE_SWING_RIGHT
    ///   Door with one panel that swings in both directions and to the right in the main trafic direction. Also called double acting door.
    ///   Note: Direction of main swing (whether in or out) is determined at the IfcDoor.
    ///   
    ///   DOUBLE_DOOR_
    ///   DOUBLE_SWING
    ///   Door with two panels, one swings in both directions and to the right in the main trafic direction the other swings also in both directions and to the left in the main trafic direction.
    ///   Note: Direction of main swing (whether in or out) is determined at the IfcDoor.
    ///   
    ///   DOUBLE_DOOR_
    ///   SINGLE_SWING_
    ///   OPPOSITE_LEFT
    ///   Door with two panels that both open to the left, one panel swings in one direction and the other panel swings in the opposite direction.
    ///   Note: Direction of main swing (whether in or out) is determined at the IfcDoor.
    ///   
    ///   DOUBLE_DOOR_
    ///   SINGLE_SWING_
    ///   OPPOSITE_RIGHT Door with two panels that both open to the right, one panel swings in one direction and the other panel swings in the opposite direction.
    ///   Note: Direction of main swing (whether in or out) is determined at the IfcDoor.
    ///   
    ///   SLIDING_TO_LEFT
    ///   Door with one panel that is sliding to the left.   
    ///   SLIDING_TO_RIGHT
    ///   Door with one panel that is sliding to the right.   
    ///   DOUBLE_DOOR_SLIDING
    ///   Door with two panels, one is sliding to the left the other is sliding to the right.   
    ///   FOLDING_TO_LEFT
    ///   Door with one panel that is folding to the left.   
    ///   FOLDING_TO_RIGHT Door with one panel that is folding to the right.   
    ///   DOUBLE_DOOR_FOLDING
    ///   Door with two panels, one is folding to the left the other is folding to the right.   
    ///   REVOLVING
    ///   An entrance door consisting of four leaves set in a form of a cross and revolving around a central vertical axis (the four panels are described by a single IfcDoor panel property).   
    ///   ROLLINGUP
    ///   Door that opens by rolling up.
    ///   Note: Whether it rolls up to the inside or outside is determined at the IfcDoor.
    ///   
    ///   USERDEFINED User defined operation type   
    ///   NOTDEFINED A door with a not defined operation type is considered as a door with a lining, but no panels. It is thereby always open.  
    ///  
    ///   NOTE
    ///   Figures are shown in the ground view. 
    ///   Figures (symbolic representation) depend on the national building code. 
    ///   These figures are only shown as illustrations, the actual representation in the ground view might differ. 
    ///   Open to the outside is declared as open into the direction of the positive y-axis, determined by the ObjectPlacement at IfcDoor 
    ///   The location of the panel relative to the wall thickness is defined by the ObjectPlacement at IfcDoor, and the IfcDoorLiningProperties.LiningOffset parameter. 
    ///   EXPRESS specification:
    /// </remarks>
    public enum IfcDoorStyleOperationEnum
    {
        SINGLE_SWING_LEFT,
        SINGLE_SWING_RIGHT,
        DOUBLE_DOOR_SINGLE_SWING,
        DOUBLE_DOOR_SINGLE_SWING_OPPOSITE_LEFT,
        DOUBLE_DOOR_SINGLE_SWING_OPPOSITE_RIGHT,
        DOUBLE_SWING_LEFT,
        DOUBLE_SWING_RIGHT,
        DOUBLE_DOOR_DOUBLE_SWING,
        SLIDING_TO_LEFT,
        SLIDING_TO_RIGHT,
        DOUBLE_DOOR_SLIDING,
        FOLDING_TO_LEFT,
        FOLDING_TO_RIGHT,
        DOUBLE_DOOR_FOLDING,
        REVOLVING,
        ROLLINGUP,
        USERDEFINED,
        NOTDEFINED
    }
}