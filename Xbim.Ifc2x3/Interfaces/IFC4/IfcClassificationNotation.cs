using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.ExternalReferenceResource;
using Xbim.Ifc4.MeasureResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    public partial class IfcClassificationNotation : IIfcClassificationReference
	{
		IIfcClassificationReferenceSelect IIfcClassificationReference.ReferencedSource 
		{ 
			get
			{
				return null;
			} 
			set
			{
                /* do nothing */
			}
		}
        IfcText? IIfcClassificationReference.Description
        {
            get
            {
                return null;
            }
            set
            {
                /* do nothing */
            }
        }
        IfcIdentifier? IIfcClassificationReference.Sort
        {
            get
            {
                return null;
            }
            set
            {
                /* do nothing */

            }
        }
		IEnumerable<IIfcRelAssociatesClassification> IIfcClassificationReference.ClassificationRefForObjects 
		{ 
			get
			{
                return Model.Instances.Where<IIfcRelAssociatesClassification>(e => Equals(e.RelatingClassification), "RelatingClassification", this);
			} 
		}
		IEnumerable<IIfcClassificationReference> IIfcClassificationReference.HasReferences 
		{ 
			get
			{
                return Model.Instances.Where<IIfcClassificationReference>(e => Equals(e.ReferencedSource), "ReferencedSource", this);
			} 
		}
        IfcURIReference? IIfcExternalReference.Location
        {
            get { return null; }
            set { /* do nothing */}
        }

        IfcIdentifier? IIfcExternalReference.Identification
        {
            get
            {
                var facets = NotationFacets.Select(f => (string)f.NotationValue).ToList();
                return !facets.Any() ? null : string.Join("", facets);
            }
            set
            {
                NotationFacets.Clear();
                if (!value.HasValue)
                    return;
                NotationFacets.Add(Model.Instances.New<IfcClassificationNotationFacet>(f => f.NotationValue = value.Value.ToString()));
            }
        }

        IfcLabel? IIfcExternalReference.Name
        {
            get { return null; }
            set { /* do nothing */ } 
        }

        IEnumerable<IIfcExternalReferenceRelationship> IIfcExternalReference.ExternalReferenceForResources
        {
            get { yield break; }
        }
	}
}