using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IfcStoreCopyItemsExtensions
    {

        /// <summary>
        /// This is a higher level function which uses InsertCopy function alongside with the knowledge of IFC schema to copy over
        /// products with their types and other related information (classification, aggregation, documents, properties) and optionally
        /// geometry. It will also bring in spatial hierarchy relevant to selected products. However, resulting model is not guaranteed 
        /// to be compliant with any Model View Definition unless you explicitly check the compliance. Context of a single product tend to 
        /// consist from hundreds of objects which need to be identified and copied over so this operation might be potentially expensive.
        /// You should never call this function more than once between two models. It not only selects objects to be copied over but also
        /// excludes other objects from being copied over so that it doesn't bring the entire model in a chain dependencies. This means
        /// that some objects are modified (like spatial relations) and won't get updated which would lead to an inconsistent copy.
        /// </summary>
        /// <param name="model">The target model</param>
        /// <param name="products">Products from other model to be inserted into this model</param>
        /// <param name="includeGeometry">If TRUE, geometry of the products will be copied over.</param>
        /// <param name="keepLabels">If TRUE, entity labels from original model will be used. Always set this to FALSE
        /// if you are going to insert products from multiple source models or if you are going to insert products to a non-empty model</param>
        /// <param name="mappings">Mappings to avoid multiple insertion of objects. Keep a single instance for insertion between two models.
        /// If you also use InsertCopy() function for some other insertions, use the same instance of mappings.</param>
        public static void InsertCopy(this IModel model, IEnumerable<IIfcProduct> products, bool includeGeometry, bool keepLabels, 
            XbimInstanceHandleMap mappings)
        {
            var context = new CopyContext
            {
                IncludeGeometry = includeGeometry
            };

            var roots = products.Cast<IPersistEntity>().ToList();
            //return if there is nothing to insert
            if (!roots.Any())
                return;

            var source = roots.First().Model;
            if (source == model)
                //don't do anything if the source and target are the same
                return;

            var toInsert = GetEntitiesToInsert(context, source, roots);
            //create new cache is none is defined
            var cache = mappings ?? new XbimInstanceHandleMap(source, model);

            foreach (var entity in toInsert)
                model.InsertCopy(entity, cache, 
                    (property, obj) => Filter(context, property, obj), 
                    true, keepLabels);
        }

        private static IEnumerable<IPersistEntity> GetEntitiesToInsert(CopyContext context, IModel model, List<IPersistEntity> roots)
        {
            context.PrimaryElements = roots.OfType<IIfcProduct>().ToList();

            //add any aggregated elements. For example IfcRoof is typically aggregation of one or more slabs so we need to bring
            //them along to have all the information both for geometry and for properties and materials.
            //This has to happen before we add spatial hierarchy or it would bring in full hierarchy which is not an intention
            var decompositionRels = GetAggregations(context, context.PrimaryElements.ToList(), model).ToList();
            context.PrimaryElements.AddRange(context.Decomposition);
            roots.AddRange(decompositionRels);

            //we should add spatial hierarchy right here so it brings its attributes as well
            var spatialRels = model.Instances.Where<IIfcRelContainedInSpatialStructure>(
                r => context.PrimaryElements.Any(e => r.RelatedElements.Contains(e))).ToList();
            var spatialRefs =
                model.Instances.Where<IIfcRelReferencedInSpatialStructure>(
                    r => context.PrimaryElements.Any(e => r.RelatedElements.Contains(e))).ToList();
            var bottomSpatialHierarchy =
                spatialRels.Select(r => r.RelatingStructure).Union(spatialRefs.Select(r => r.RelatingStructure)).ToList();
            var spatialAggregations = GetUpstreamHierarchy(bottomSpatialHierarchy, model).ToList();

            //add all spatial elements from bottom and from upstream hierarchy
            context.PrimaryElements.AddRange(bottomSpatialHierarchy);
            context.PrimaryElements.AddRange(spatialAggregations.Select(r => r.RelatingObject).OfType<IIfcProduct>());
            roots.AddRange(spatialAggregations);
            roots.AddRange(spatialRels);
            roots.AddRange(spatialRefs);

            //we should add any feature elements used to subtract mass from a product
            var featureRels = GetFeatureRelations(context.PrimaryElements).ToList();
            var openings = featureRels.Select(r => r.RelatedOpeningElement);
            context.PrimaryElements.AddRange(openings);
            roots.AddRange(featureRels);

            //object types and properties for all primary products (elements and spatial elements)
            roots.AddRange(context.PrimaryElements.SelectMany(p => p.IsDefinedBy));
            roots.AddRange(context.PrimaryElements.SelectMany(p => p.IsTypedBy));



            //assignmnet to groups will bring in all system aggregarions if defined in the file
            roots.AddRange(context.PrimaryElements.SelectMany(p => p.HasAssignments));

            //associations with classification, material and documents
            roots.AddRange(context.PrimaryElements.SelectMany(p => p.HasAssociations));

            return roots;
        }

        private static object Filter(CopyContext context, ExpressMetaProperty property, object parentObject)
        {
            //ignore inverses except for style
            if (property.IsInverse)
                return property.Name == "StyledByItem" ? property.PropertyInfo.GetValue(parentObject, null) : null;

            if (context.PrimaryElements != null && context.PrimaryElements.Any())
            {
                if (typeof(IIfcProduct).IsAssignableFrom(property.PropertyInfo.PropertyType))
                {
                    var element = property.PropertyInfo.GetValue(parentObject, null) as IIfcProduct;
                    if (element != null && context.PrimaryElements.Contains(element))
                        return element;
                    return null;
                }
                if (property.EnumerableType != null && !property.EnumerableType.IsValueType && property.EnumerableType != typeof(string))
                {
                    //this can either be a list of IPersistEntity or select type. The very base type is IPersist
                    var entities = property.PropertyInfo.GetValue(parentObject, null) as IEnumerable<IPersist>;
                    if (entities != null)
                    {
                        var persistEntities = entities as IList<IPersist> ?? entities.ToList();
                        var elementsToRemove = persistEntities.OfType<IIfcProduct>().Where(e => !context.PrimaryElements.Contains(e)).ToList();
                        //if there are no IfcElements return what is in there with no care
                        if (elementsToRemove.Any())
                            //return original values excluding elements not included in the primary set
                            return persistEntities.Except(elementsToRemove).ToList();
                    }
                }
            }

            //if geometry is to be included don't filter it out
            if (context.IncludeGeometry)
                return property.PropertyInfo.GetValue(parentObject, null);

            //leave out geometry and placement of products
            if (parentObject is IIfcProduct &&
                (property.PropertyInfo.Name == "Representation" || property.PropertyInfo.Name == "ObjectPlacement")
                )
                return null;

            //leave out representation maps
            if (parentObject is IIfcTypeProduct && property.PropertyInfo.Name == "RepresentationMaps")
                return null;

            //leave out eventual connection geometry
            if (parentObject is IIfcRelSpaceBoundary && property.PropertyInfo.Name == "ConnectionGeometry")
                return null;

            //return the value for anything else
            return property.PropertyInfo.GetValue(parentObject, null);
        }

        private static IEnumerable<IIfcRelVoidsElement> GetFeatureRelations(IEnumerable<IIfcProduct> products)
        {
            var elements = products.OfType<IIfcElement>().ToList();
            if (!elements.Any()) yield break;
            var model = elements.First().Model;
            var rels = model.Instances.Where<IIfcRelVoidsElement>(r => elements.Any(e => Equals(e, r.RelatingBuildingElement)));
            foreach (var rel in rels)
                yield return rel;
        }

        private static IEnumerable<IIfcRelDecomposes> GetAggregations(CopyContext context, List<IIfcProduct> products, IModel model)
        {
            context.Decomposition.Clear();
            while (true)
            {
                if (!products.Any())
                    yield break;

                var products1 = products;
                var rels = model.Instances.Where<IIfcRelDecomposes>(r =>
                {
                    if (r is IIfcRelAggregates aggr)
                        return products1.Any(p => Equals(aggr.RelatingObject, p));
                    if (r is IIfcRelNests nest)
                        return products1.Any(p => Equals(nest.RelatingObject, p));
                    if (r is IIfcRelProjectsElement prj)
                        return products1.Any(p => Equals(prj.RelatingElement, p));
                    if (r is IIfcRelVoidsElement voids)
                        return products1.Any(p => Equals(voids.RelatingBuildingElement, p));
                    return false;

                }).ToList();
                var relatedProducts = rels.SelectMany(r =>
                {
                    if (r is IIfcRelAggregates aggr)
                        return aggr.RelatedObjects.OfType<IIfcProduct>();
                    if (r is IIfcRelNests nest)
                        return nest.RelatedObjects.OfType<IIfcProduct>();
                    if (r is IIfcRelProjectsElement prj)
                        return new IIfcProduct[] { prj.RelatedFeatureElement };
                    if (r is IIfcRelVoidsElement voids)
                        return new IIfcProduct[] { voids.RelatedOpeningElement };
                    return null;
                }).Where(p => p != null).ToList();

                foreach (var rel in rels)
                    yield return rel;

                products = relatedProducts;
                context.Decomposition.AddRange(products);
            }
        }

        private static IEnumerable<IIfcRelAggregates> GetUpstreamHierarchy(IEnumerable<IIfcSpatialElement> spatialStructureElements, IModel model)
        {
            while (true)
            {
                var elements = spatialStructureElements.ToList();
                if (!elements.Any())
                    yield break;

                var rels = model.Instances.Where<IIfcRelAggregates>(r => elements.Any(s => r.RelatedObjects.Contains(s))).ToList();
                var decomposing = rels.Select(r => r.RelatingObject).OfType<IIfcSpatialStructureElement>();

                foreach (var rel in rels)
                    yield return rel;

                spatialStructureElements = decomposing;
            }
        }

        private class CopyContext
        {
            public List<IIfcProduct> PrimaryElements { get; set; }
            public List<IIfcProduct> Decomposition { get; private set; } = new List<IIfcProduct>();
            public bool IncludeGeometry { get; set; }
        }
    }
}
