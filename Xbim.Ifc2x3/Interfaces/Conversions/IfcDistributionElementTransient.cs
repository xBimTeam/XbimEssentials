using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ProductExtension;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcDistributionElementTransient : Xbim.Ifc4.Interfaces.IIfcDistributionElement
    {
        Ifc4.Interfaces.IIfcElement _element;
        internal IfcDistributionElementTransient(IfcElement element)
        {
            _element = element;

        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelConnectsPortToElement> HasPorts
        {
            get { if(_element is IfcDistributionElement) return ((IfcDistributionElement)_element).HasPorts; else return Enumerable.Empty<Ifc4.Interfaces.IIfcRelConnectsPortToElement>();
            }
        }

        public Ifc4.MeasureResource.IfcIdentifier? Tag
        {
            get { return _element.Tag; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelFillsElement> FillsVoids
        {
            get { return _element.FillsVoids; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelConnectsElements> ConnectedTo
        {
            get { return _element.ConnectedTo; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelInterferesElements> IsInterferedByElements
        {
            get { return _element.IsInterferedByElements; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelInterferesElements> InterferesElements
        {
            get { return _element.InterferesElements; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelProjectsElement> HasProjections
        {
            get { return _element.HasProjections; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelReferencedInSpatialStructure> ReferencedInStructures
        {
            get { return _element.ReferencedInStructures; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelVoidsElement> HasOpenings
        {
            get { return _element.HasOpenings; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelConnectsWithRealizingElements> IsConnectionRealization
        {
            get { return _element.IsConnectionRealization; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelSpaceBoundary> ProvidesBoundaries
        {
            get { return _element.ProvidesBoundaries; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelConnectsElements> ConnectedFrom
        {
            get { return _element.ConnectedFrom; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelContainedInSpatialStructure> ContainedInStructure
        {
            get { return _element.ContainedInStructure; }
        }

        public Ifc4.Interfaces.IIfcObjectPlacement ObjectPlacement
        {
            get { return _element.ObjectPlacement; }
        }

        public Ifc4.Interfaces.IIfcProductRepresentation Representation
        {
            get { return _element.Representation; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelAssignsToProduct> ReferencedBy
        {
            get { return _element.ReferencedBy; }
        }

        public Ifc4.MeasureResource.IfcLabel? ObjectType
        {
            get { return _element.ObjectType; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelDefinesByObject> IsDeclaredBy
        {
            get { return _element.IsDeclaredBy; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelDefinesByObject> Declares
        {
            get { return _element.Declares; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelDefinesByType> IsTypedBy
        {
            get { return _element.IsTypedBy; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelDefinesByProperties> IsDefinedBy
        {
            get { return _element.IsDefinedBy; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelAssigns> HasAssignments
        {
            get { return _element.HasAssignments; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelNests> Nests
        {
            get { return _element.Nests; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelNests> IsNestedBy
        {
            get { return _element.IsNestedBy; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelDeclares> HasContext
        {
            get { return _element.HasContext; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelAggregates> IsDecomposedBy
        {
            get { return _element.IsDecomposedBy; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelAggregates> Decomposes
        {
            get { return _element.Decomposes; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcRelAssociates> HasAssociations
        {
            get { return _element.HasAssociations; }
        }

        public Ifc4.UtilityResource.IfcGloballyUniqueId GlobalId
        {
            get { return _element.GlobalId; }
        }

        public Ifc4.Interfaces.IIfcOwnerHistory OwnerHistory
        {
            get { return _element.OwnerHistory; }
        }

        public Ifc4.MeasureResource.IfcLabel? Name
        {
            get { return _element.Name; }
        }

        public Ifc4.MeasureResource.IfcText? Description
        {
            get { return _element.Description; }
        }

        public int EntityLabel
        {
            get { return _element.EntityLabel; }
        }

        public Common.IModel Model
        {
            get { return _element.Model; }
        }

        public Common.ActivationStatus ActivationStatus
        {
            get { return _element.ActivationStatus; }
        }

        public void Activate(bool write)
        {
            ((Xbim.Common.IPersistEntity)_element).Activate(write);
        }

        public void Activate(Action activation)
        {
            ((Xbim.Common.IPersistEntity)_element).Activate(activation);
        }

        public Common.Metadata.ExpressType ExpressType
        {
            get { return ((Xbim.Common.IPersistEntity)_element).ExpressType; }
        }

        public Common.IModel ModelOf
        {
            get { return _element.ModelOf; }
        }

        public void Parse(int propIndex, Common.IPropertyValue value, int[] nested)
        {
            _element.Parse(propIndex, value, nested);
        }

        public string WhereRule()
        {
            return _element.WhereRule();
        }
    }
}
