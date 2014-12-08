#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralMember.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   The abstract entity IfcStructuralMember is the superclass of all structural elements representing the structural behaviour of building elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The abstract entity IfcStructuralMember is the superclass of all structural elements representing the structural behaviour of building elements. A further differentiation is made for structural curve members and structural face members (see IfcStructuralCurveMember and IfcStructuralFaceMember). Structural members can have 
    ///   a material definition, using IfcStructuralMember o-- IfcRelAssociatesMaterial --o IfcMaterial 
    ///   a profile definition, using IfcStructuralMember o-- IRelAssociatesProfileProperties --o IfcProfileProperties 
    ///   a parent analysis model, using IfcStructuralMember o-- IfcRelAssignsToGroup --o IfcStructuralAnalysisModel 
    ///   NOTE: Currently only the subtypes IfcStructuralCurveMember and IfcStructuralFaceMember are defined. However, for dynamic calculations, an extension for 'point members' will be necessary and might be added in future releases.
    ///   HISTORY: New entity in Release IFC2x Edition 2. 
    ///   Use Definition
    ///   Assignment of Material 
    ///   The relationship to a specific material definition is provided by the entity class IfcRelAssociatesMaterial. The instance of IfcStructuralMember is used to represent collectively its subclasses IfcStructuralCurveMember and IfcStructuralFaceMember.
    ///   Depending on the material, there are material specific subtypes of IfcMechanicalMaterialProperties, such as IfcMechanicalSteelMaterialProperties, or IfcMechanicalConcreteMaterialProperties.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcStructuralMember : IfcStructuralItem
    {
        /// <summary>
        ///   Inverse link to the relationship object, that connects a physical element to this structural member 
        ///   (the element of which this structural member is the analytical idealisation).
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsStructuralElement> ReferencesElement
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsStructuralElement>(
                        r => r.RelatedStructuralMember == this);
            }
        }

        /// <summary>
        ///   Inverse relationship to all structural connections (i.e. to supports or connecting elements) which are defined for this structural member.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsStructuralMember> ConnectedBy
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsStructuralMember>(
                        r => r.RelatingStructuralMember == this);
            }
        }
    }
}