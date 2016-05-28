using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.ExternalReferenceResource;
using Xbim.Ifc4.MeasureResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    public partial class IfcClassificationItem : IIfcClassificationReference
	{
		IIfcClassificationReferenceSelect IIfcClassificationReference.ReferencedSource 
		{ 
			get
			{
                var relatingItem = IsClassifiedItemIn.Select(i => i.RelatingItem).FirstOrDefault();
                if (relatingItem != null)
                    return relatingItem;
                return ItemOf;
			} 
			set
			{
				
			}
		}
		IfcText? IIfcClassificationReference.Description  { get { return null; } set { } }

        IfcIdentifier? IIfcClassificationReference.Sort { get { return null; } set { } }

		IEnumerable<IIfcRelAssociatesClassification> IIfcClassificationReference.ClassificationRefForObjects 
		{ 
			get
			{
                var notations = Model.Instances.Where<IfcClassificationNotation>(n => n.NotationFacets.Contains(Notation));
                return Model.Instances.Where<Kernel.IfcRelAssociatesClassification>(r => notations.Any(n => n.Equals(r.RelatingClassification)));
			} 
		}
		IEnumerable<IIfcClassificationReference> IIfcClassificationReference.HasReferences 
		{ 
			get { return IsClassifyingItemIn.SelectMany(i => i.RelatedItems); } 
		}
        IfcURIReference? IIfcExternalReference.Location { get { return null; } set { } }

        IfcIdentifier? IIfcExternalReference.Identification
        {
            get
            {
                return Notation != null
                ? (string)(Notation.NotationValue)
                : null;
            }
            set {
                Notation = value.HasValue ?
                    Model.Instances.New<IfcClassificationNotationFacet>(f => f.NotationValue = value.Value.ToString()) : 
                    null;
            }
        }

        IfcLabel? IIfcExternalReference.Name
        {
            get
            {
                return (string)Title;
            }
            set
            {
                Title = value.HasValue
                    ? value.Value.ToString()
                    : null;
            }
        }

        IEnumerable<IIfcExternalReferenceRelationship> IIfcExternalReference.ExternalReferenceForResources
        {
            get { yield break; }
        }
	}
}