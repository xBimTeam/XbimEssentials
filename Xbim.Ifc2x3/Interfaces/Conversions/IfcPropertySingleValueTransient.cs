﻿using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcPropertySingleValueTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcPropertySingleValue
    {
        public IEnumerable<IIfcExternalReferenceRelationship> HasExternalReferences { get { yield break; } }
        public IfcIdentifier Name { get; internal set; }
        public IfcText? Description { get; internal set; }
        public IEnumerable<Ifc4.Interfaces.IIfcPropertySet> PartOfPset { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPropertyDependencyRelationship> PropertyForDependance { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPropertyDependencyRelationship> PropertyDependsOn { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcComplexProperty> PartOfComplex { get { yield break; } }
        public IEnumerable<IIfcResourceConstraintRelationship> HasConstraints { get{yield break;}}
        public IEnumerable<IIfcResourceApprovalRelationship> HasApprovals { get { yield break; } }
        public Ifc4.Interfaces.IIfcValue NominalValue { get; internal set; }
        public Ifc4.Interfaces.IIfcUnit Unit { get; internal set; }
    }
}
