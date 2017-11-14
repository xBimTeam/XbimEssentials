using System;
using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal abstract class IfcPhysicalSimpleQuantityTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcPhysicalSimpleQuantity
    {
        public Ifc4.Interfaces.IIfcNamedUnit _unit;
        public IfcLabel Name
        {
            get { return new IfcLabel(); }
            set { throw new NotSupportedException(); }
        }

        public IfcText? Description
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        public IEnumerable<IIfcExternalReferenceRelationship> HasExternalReferences { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPhysicalComplexQuantity> PartOfComplex { get { yield break; } }

        public Ifc4.Interfaces.IIfcNamedUnit Unit
        {
            get { return _unit; }
            set { throw new NotSupportedException(); }
        }
    }
}
