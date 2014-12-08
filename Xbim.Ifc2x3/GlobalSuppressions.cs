#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    GlobalSuppressions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Diagnostics.CodeAnalysis;

#endregion

[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Xbim.Ifc2x3.PresentationAppearanceResource")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Xbim.Ifc2x3.PresentationOrganizationResource")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Xbim.Ifc2x3.StructuralAnalysisDomain")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Xbim.Ifc2x3.TimeSeriesResource")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Xbim.Ifc2x3.Model")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ComissioningEngineer"
        , Scope = "member", Target = "Xbim.Ifc2x3.ActorResource.RoleEnum.#ComissioningEngineer")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Comissioning",
        Scope = "member", Target = "Xbim.Ifc2x3.ActorResource.Role.#ComissioningEngineer")]