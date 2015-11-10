using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc4.ExternalReferenceResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcClassificationReferenceTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcClassificationReference
    {
        private readonly IfcClassificationItem _item;
        public override IModel Model { get { return _item.Model; } }

        public IfcClassificationReferenceTransient(IfcClassificationItem item)
        {
            _item = item;
        }

        /// <summary>
        /// New in IFC4. Will be always null.
        /// </summary>
        public IfcURIReference? Location { get { return null; } }

        public IfcIdentifier? Identification { get
        {
            return _item.Notation != null
                ? (string)(_item.Notation.NotationValue)
                : null;
        } }

        public IfcLabel? Name
        {
            get { return (string)_item.Title; }
        }

        /// <summary>
        /// New in IFC4. Empty enumeration.
        /// </summary>
        public IEnumerable<IIfcExternalReferenceRelationship> ExternalReferenceForResources { get{ yield break; } }

        public IIfcClassificationReferenceSelect ReferencedSource
        {
            get
            {
                var relatingItem = _item.IsClassifiedItemIn.Select(i => i.RelatingItem).FirstOrDefault();
                if(relatingItem != null)
                    return new IfcClassificationReferenceTransient(relatingItem);
                return _item.ItemOf;
            }
        }

        /// <summary>
        /// New in IFC4. Will always return null
        /// </summary>
        public IfcText? Description { get { return null; }}

        /// <summary>
        /// New in IFC4. Will always return null
        /// </summary>
        public IfcIdentifier? Sort { get { return null; } }

        public IEnumerable<Ifc4.Interfaces.IIfcRelAssociatesClassification> ClassificationRefForObjects
        {
            get
            {
                var notations = Model.Instances.Where<IfcClassificationNotation>(n => n.NotationFacets.Contains(_item.Notation));
                return Model.Instances.Where<IfcRelAssociatesClassification>(r => notations.Any(n => (r.RelatingClassification as IfcClassificationNotation) == n));
            }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcClassificationReference> HasReferences
        {
            get
            {
                var relatedItems = _item.IsClassifyingItemIn.SelectMany(i => i.RelatedItems);
                return relatedItems.Select(item => new IfcClassificationReferenceTransient(item));
            }
        }
    }
}
