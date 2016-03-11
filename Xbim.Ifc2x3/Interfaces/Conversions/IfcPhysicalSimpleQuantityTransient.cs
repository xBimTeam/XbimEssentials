using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal abstract class IfcPhysicalSimpleQuantityTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcPhysicalSimpleQuantity
    {
        public IfcLabel Name { get; internal set; }
        public IfcText? Description { get; internal set; }
        public IEnumerable<IIfcExternalReferenceRelationship> HasExternalReferences { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPhysicalComplexQuantity> PartOfComplex { get { yield break; } }
        public Ifc4.Interfaces.IIfcNamedUnit Unit { get; internal set; }
    }
}
