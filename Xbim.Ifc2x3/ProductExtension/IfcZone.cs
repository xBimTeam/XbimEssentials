#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcZone.cs
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
    ///   A zone (IfcZone) is an aggregation of spaces, partial spaces or other zones.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A zone (IfcZone) is an aggregation of spaces, partial spaces or other zones. Zone structures may not be hierarchical (in contrary to the spatial structure of a project - see IfcSpatialStructureElement), i.e. one individual IfcSpace may be associated with zero, one, or several IfcZone's. IfcSpace's are aggregated into an IfcZone by using the objectified relationship IfcRelAssignsToGroup as specified at the supertype IfcGroup.
    ///   NOTE  Certain use cases may restrict the freedom of non hierarchical relationships. In some building service use cases the zone denotes a view based delimited volume for the purpose of analysis and calculation. This type of zone cannot overlap with respect to that analysis, but may overlap otherwise.
    ///   NOTE  One of the purposes of a zone is to define a fire compartmentation. In this case it defines the geometric information about the fire compartment (through the contained spaces) and information, whether this compartment is ventilated or sprinkler protected. In addition the fire risk code and the harard type can be added, the coding is normally defined within a national fire regulation. All that information is available within the relevant property sets. 
    ///   RECOMMENDATION  In case of a zone denoting a (fire) compartment, the following types should be used, if applicable, as values of the ObjectType attribute:
    ///   FireCompartment - a zone of spaces, collected to represent a single fire compartment. 
    ///   ElevatorShaft - a collection of spaces within an elevator, potentially going through many storeys. 
    ///   RisingDuct 
    ///   RunningDuct 
    ///   Additional classifications of the IfcZone, as provided by a national classification system, can be assigned by using the IfcRelAssociatesClassification relationship.
    ///   Formal Propositions:
    ///   WR1   :   A Zone is grouped by the objectified relationship IfcRelAssignsToGroup. Only objects of type IfcSpace or IfcZone are allowed as RelatedObjects.  
    ///   HISTORY New entity in IFC Release 1.0 
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcZone are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcZone are part of this IFC release:
    ///   Pset_ZoneCommon: common property set for all types of zone 
    ///   Pset_SpaceFireSafetyRequirements: common property set for all types of zones to capture the fire safety requirements 
    ///   Pset_SpaceLightingRequirements: common property set for all types of zones to capture the lighting requirements 
    ///   Pset_SpaceOccupancyRequirements: common property set for all types of zones to capture the occupancy requirements 
    ///   Pset_SpaceThermalRequirements: common property set for all types of zones to capture the thermal requirements
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcZone : IfcGroup
    {
    }
}