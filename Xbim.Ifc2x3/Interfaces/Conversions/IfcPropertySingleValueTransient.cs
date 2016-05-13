using System;
using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcPropertySingleValueTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcPropertySingleValue
    {
        private readonly IfcIdentifier _name;
        private readonly Ifc4.Interfaces.IIfcValue _nominalValue;

        public IfcPropertySingleValueTransient(string name, Ifc4.Interfaces.IIfcValue value)

        {
            _name = name;
            _nominalValue = value;
        }

        public IEnumerable<IIfcExternalReferenceRelationship> HasExternalReferences { get { yield break; } }

        public IfcIdentifier Name
        {
            get { return _name; }
            set { throw new NotSupportedException(); }
        }

        public IfcText? Description
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcPropertySet> PartOfPset { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPropertyDependencyRelationship> PropertyForDependance { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPropertyDependencyRelationship> PropertyDependsOn { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcComplexProperty> PartOfComplex { get { yield break; } }
        public IEnumerable<IIfcResourceConstraintRelationship> HasConstraints { get{yield break;}}
        public IEnumerable<IIfcResourceApprovalRelationship> HasApprovals { get { yield break; } }

        public Ifc4.Interfaces.IIfcValue NominalValue
        {
            get { return _nominalValue; }
            set { throw new NotSupportedException(); }
        }

        public Ifc4.Interfaces.IIfcUnit Unit
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
