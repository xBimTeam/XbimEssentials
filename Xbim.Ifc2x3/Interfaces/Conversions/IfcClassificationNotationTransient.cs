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
    internal class IfcClassificationNotationTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcClassificationReference
    {
        private readonly IfcClassificationNotation _notation;
        public override IModel Model { get { return _notation.Model; } }

        public IfcClassificationNotationTransient(IfcClassificationNotation notation)
        {
            _notation = notation;
        }

        /// <summary>
        /// New in IFC4. Will be always null.
        /// </summary>
        public IfcURIReference? Location { get { return null; } }

        public IfcIdentifier? Identification { get
        {
            var facets = _notation.NotationFacets.Select(f => (string) f.NotationValue);
            return string.Join("", facets);
        } }

        public IfcLabel? Name
        {
            get
            {
                var facets = _notation.NotationFacets.Select(f => (string)f.NotationValue);
                return string.Join("", facets);
            }
        }

        /// <summary>
        /// New in IFC4. Empty enumeration.
        /// </summary>
        public IEnumerable<IIfcExternalReferenceRelationship> ExternalReferenceForResources { get{ yield break; } }

        public IIfcClassificationReferenceSelect ReferencedSource
        {
            get { return null; }
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
                return Model.Instances.Where<IfcRelAssociatesClassification>(r => (r.RelatingClassification as IfcClassificationNotation) == _notation);
            }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcClassificationReference> HasReferences
        {
            get
            {
                yield break;
            }
        }
    }
}
