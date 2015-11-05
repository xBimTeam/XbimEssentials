using System;
using System.Collections.Generic;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    public abstract class IfcPhysicalSimpleQuantityTransient : Xbim.Ifc4.Interfaces.IIfcPhysicalSimpleQuantity
    {
        protected IfcPhysicalSimpleQuantityTransient()
        {
        }

        public void Parse(int propIndex, IPropertyValue value, int[] nested)
        {
            throw new NotSupportedException("Transient object");
        }

        public string WhereRule()
        {
            return "";
        }

        public int EntityLabel { get { return -1; }}
        public IModel Model { get { return null; } }
        public ActivationStatus ActivationStatus { get{return ActivationStatus.ActivatedRead;} }
        public void Activate(bool write)
        {
        }
        public void Activate(Action activation)
        {
        }

        public ExpressType ExpressType { get { return null; } }

        public IModel ModelOf { get { return Model; } }
        public IfcLabel Name { get; internal set; }
        public IfcText? Description { get; internal set; }
        public IEnumerable<IIfcExternalReferenceRelationship> HasExternalReferences { get { yield break; } }
        public IEnumerable<Ifc4.Interfaces.IIfcPhysicalComplexQuantity> PartOfComplex { get { yield break; } }
        public Ifc4.Interfaces.IIfcNamedUnit Unit { get; internal set; }
    }
}
