#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnnotation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   An annotation is a graphical representation within the geometric (and spatial) context of a project, that adds a note or meaning to the objects which constitutes the project model.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An annotation is a graphical representation within the geometric (and spatial) context of a project, that adds a note or meaning to the objects which constitutes the project model. Annotations include additional line drawings, text, dimensioning, hatching and other forms of graphical notes.
    ///   If available, the annotation should be related to the spatial context of the project, by containing the annotation within the appropriate level of the building structure (site, building, storey, or space). This is handled by the IfcRelContainedInSpatialStructure relationship.
    ///   HISTORY: New entity in Release IFC2x Edition 2. 
    ///   Geometry Use Definitions
    ///   The geometric representation of any IfcAnnotation is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for any IfcAnnotation is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Styled Representations
    ///   The standard representation of IfcAnnotation is defined using the styled item representation by IfcStyledRepresentation that holds the style information assigned to the geometric representation items together with the geometric representation items.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcAnnotation : IfcProduct
    {
        /// <summary>
        ///   Inverse. Relationship to a spatial structure element, to which the associate is primarily associated.
        /// </summary>
        public IfcRelContainedInSpatialStructure ContainedInStructure
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}