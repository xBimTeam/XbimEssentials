#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRole.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Xbim.Ifc2x3.ActorResource
{
    /// <summary>
    ///   Roles which may be played by an actor.
    /// </summary>
    public enum IfcRole
    {
        Supplier,
        Manufacturer,
        Contractor,
        Subcontractor,
        Architect,
        StructuralEngineer,
        CostEngineer,
        Client,
        BuildingOwner,
        BuildingOperator,
        MechanicalEngineer,
        ElectricalEngineer,
        ProjectManager,
        FacilitiesManager,
        CivilEngineer,

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
            MessageId = "ComissioningEngineer")] ComissioningEngineer,
        Engineer,
        Owner,
        Consultant,
        ConstructionManager,
        FieldConstructionManager,
        Reseller,
        UserDefined
    }
}