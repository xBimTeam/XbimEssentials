using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public class Extractor
    {
        #region Insert products with context
        private bool _siteGeometryExists;

        public Extractor(IModel source)
        {
            BuildInverseStyleLookup(source);

            var elements = source.Instances
                .OfType<IIfcElement>()
                .Where(i => !(i is IIfcOpeningElement))
                .ToList();

            // filter out all elements which will be brought in as a decomposition of other elements
            var decomposing = new List<IIfcProduct>();
            // this as a side effect fills in decomposing elements
            GetAggregations(new HashSet<int>(elements.Select(element => element.EntityLabel)), source, decomposing);
            var decomposingIds = new HashSet<int>(decomposing.Select(d => d.EntityLabel));
        }


        //TODO: Consider deprecating / refactoring in favour of IfcStore.InsertCopy?

        /// <summary>
        /// This is a higher level function which uses InsertCopy function alongside with the knowledge of IFC schema to copy over
        /// products with their types and other related information (classification, aggregation, documents, properties) and optionally
        /// geometry. It will also bring in spatial hierarchy relevant to selected products. However, resulting model is not guaranteed 
        /// to be compliant with any Model View Definition unless you explicitly check the compliance. Context of a single product tend to 
        /// consist from hundreds of objects which need to be identified and copied over so this operation might be potentially expensive.
        /// You should never call this function more than once between two models. It not only selects objects to be copied over but also
        /// excludes other objects from being coppied over so that it doesn't bring the entire model in a chain dependencies. This means
        /// that some objects are modified (like spatial relations) and won't get updated which would lead to an inconsistent copy.
        /// </summary>
        /// <param name="target">The target model</param>
        /// <param name="products">Products from other model to be inserted into this model</param>
        /// <param name="includeGeometry">If TRUE, geometry of the products will be copied over.</param>
        /// <param name="keepLabels">If TRUE, entity labels from original model will be used. Always set this to FALSE
        /// if you are going to insert products from multiple source models or if you are going to insert products to a non-empty model</param>
        /// <param name="progress">A progress delegate</param>
        public void InsertCopy(IModel target, IEnumerable<IIfcProduct> products, bool includeGeometry, bool keepLabels, 
            IProgress<double> progress = null)
        {
            var primaryElements = new List<IIfcProduct>();

            var roots = products.Cast<IPersistEntity>().ToList();
            //return if there is nothing to insert
            if (!roots.Any())
            {
                progress?.Report(1.0);
                return;
            }

            var source = roots.First().Model;
            if (source == target)
                //don't do anything if the source and target are the same
                return;

            var toInsert = GetEntitiesToInsert(source, roots, out primaryElements);
            var project = source.Instances.FirstOrDefault<IIfcProject>();
            if (project != null)
                toInsert.Add(project);

            double count = toInsert.Count;
            //create new cache is none is defined
            var cache = new XbimInstanceHandleMap(source, target);

            var includeSiteGeometry = false;
            if (!_siteGeometryExists)
            {
                lock (source)
                {
                    if (!_siteGeometryExists)
                    {
                        var hasSite = toInsert.OfType<IIfcRelAggregates>().Any(e => e.RelatingObject is IIfcSite);
                        if (hasSite)
                        {
                            includeSiteGeometry = true;
                            _siteGeometryExists = true;
                        }
                    }
                }
            }

            var filter = GetFilter(primaryElements, includeGeometry, includeSiteGeometry, target.Metadata);

            var counter = 0;
            foreach (var entity in toInsert)
            {
                target.InsertCopy(entity, cache, filter, true, keepLabels);
                counter++;
                if (counter % 20 == 0)
                    progress?.Report(counter / count);
            }

            progress?.Report(1.0);
        }

        private List<IPersistEntity> GetEntitiesToInsert(IModel source, List<IPersistEntity> roots, out List<IIfcProduct> primaryElements)
        {
            var primary = roots.OfType<IIfcProduct>().ToList();

            //add any aggregated elements. For example IfcRoof is typically aggregation of one or more slabs so we need to bring
            //them along to have all the information both for geometry and for properties and materials.
            //This has to happen before we add spatial hierarchy or it would bring in full hierarchy which is not an intention
            var decomposition = new List<IIfcProduct>();
            var primaryIds = new HashSet<int>(primary.Select(p => p.EntityLabel));
            var decompositionRels = GetAggregations(primaryIds, source, decomposition).ToList();
            decomposition.ForEach(d =>
            {
                if (primaryIds.Add(d.EntityLabel))
                    primary.Add(d);
            });

            roots.AddRange(decompositionRels);

            //we should add spatial hierarchy right here so it brings its attributes as well
            var spatialRels = source.Instances.Where<IIfcRelContainedInSpatialStructure>(
                r => r.RelatedElements.Any(e => primaryIds.Contains(e.EntityLabel))).ToList();
            var spatialRefs =
                source.Instances.Where<IIfcRelReferencedInSpatialStructure>(
                    r => r.RelatedElements.Any(e => primaryIds.Contains(e.EntityLabel))).ToList();
            var bottomSpatialHierarchy =
                spatialRels.Select(r => r.RelatingStructure).Union(spatialRefs.Select(r => r.RelatingStructure)).ToList();
            var spatialAggregations = GetUpstreamHierarchy(bottomSpatialHierarchy, source).ToList();

            //add all spatial elements from bottom and from upstream hierarchy
            primary.AddRange(bottomSpatialHierarchy);
            primary.AddRange(spatialAggregations.Select(r => r.RelatingObject).OfType<IIfcProduct>());
            roots.AddRange(spatialAggregations);
            roots.AddRange(spatialRels);
            roots.AddRange(spatialRefs);

            //we should add any feature elements used to subtract mass from a product
            var featureRels = GetFeatureRelations(primary).ToList();
            var openings = featureRels.Select(r => r.RelatedOpeningElement);
            primary.AddRange(openings);
            roots.AddRange(featureRels);

            //object types and properties for all primary products (elements and spatial elements)
            roots.AddRange(primary.SelectMany(p => p.IsDefinedBy));
            roots.AddRange(primary.SelectMany(p => p.IsTypedBy));



            //assignmnet to groups will bring in all system aggregarions if defined in the file
            roots.AddRange(primary.SelectMany(p => p.HasAssignments));

            //associations with classification, material and documents
            roots.AddRange(primary.SelectMany(p => p.HasAssociations));

            primaryElements = primary;
            return roots;
        }

        private static string[] ignoreNamespaces = new[] { "GeometricConstraintResource", "GeometricModelResource", "GeometryResource", "ProfileResource", "TopologyResource", "RepresentationResource" };

        private static HashSet<Type> GetIgnoredTypes(ExpressMetaData metaData)
        {
            var types = metaData.Types().Where(t => !t.Type.IsAbstract && ignoreNamespaces.Any(ns => t.Type.Namespace.EndsWith(ns))).Select(t => t.Type);
            return new HashSet<Type>(types);
        }

        private static Type GetImplementation<T>(ExpressMetaData metadata)
        {
            var implementations = metadata.TypesImplementing(typeof(T)).ToList();
            var ids = new HashSet<short>(implementations.Select(i => i.TypeId));
            var root = implementations.AsQueryable()
                .FirstOrDefault(i => !ids.Contains(i.SuperType.TypeId));
            return root.Type;
        }

        private static Dictionary<Type, Dictionary<ExpressMetaProperty, ProcessingType>> GetProcessingInstructions(ExpressMetaData metaData, bool includeGeometry, bool includeSiteGeometry)
        {
            var productType = GetImplementation<IIfcProduct>(metaData);
            var types = metaData.Types()
                .Where(t => !t.Type.IsAbstract && !ignoreNamespaces.Any(ns => t.Type.Namespace.EndsWith(ns)))
                .ToList();
            var result = new Dictionary<Type, Dictionary<ExpressMetaProperty, ProcessingType>>();
            foreach (var type in types)
            {
                var instructions = new Dictionary<ExpressMetaProperty, ProcessingType>(type.Properties.Count);
                foreach (var propertyKvp in type.Properties)
                {
                    var propIdx = propertyKvp.Key - 1;
                    var property = propertyKvp.Value;

                    // single instance
                    if (property.EnumerableType == null)
                    {
                        var propType = property.PropertyInfo.PropertyType;
                        if (propType.IsValueType || propType == typeof(string))
                        {
                            instructions.Add(property, ProcessingType.Pass);
                            continue;
                        }


                        if (propType.IsAssignableFrom(productType) ||
                            productType.IsAssignableFrom(propType))
                        {
                            instructions.Add(property, ProcessingType.Entity);
                            continue;
                        }

                        //if geometry is to be included don't filter anything else out
                        if (includeGeometry && (!(typeof(IIfcSite).IsAssignableFrom(type.Type)) || includeSiteGeometry))
                        {
                            instructions.Add(property, ProcessingType.Pass);
                            continue;
                        }

                        //leave out geometry and placement of products
                        if (typeof(IIfcProduct).IsAssignableFrom(type.Type) &&
                            (property.PropertyInfo.Name == "Representation" || property.PropertyInfo.Name == "ObjectPlacement")
                            )
                        {
                            instructions.Add(property, ProcessingType.Remove);
                            continue;
                        }

                        //leave out representation maps
                        if (typeof(IIfcTypeProduct).IsAssignableFrom(type.Type) && property.PropertyInfo.Name == "RepresentationMaps")
                        {
                            instructions.Add(property, ProcessingType.Remove);
                            continue;
                        }

                        //leave out eventual connection geometry
                        if (typeof(IIfcRelSpaceBoundary).IsAssignableFrom(type.Type) && property.PropertyInfo.Name == "ConnectionGeometry")
                        {
                            instructions.Add(property, ProcessingType.Remove);
                            continue;
                        }


                        instructions.Add(property, ProcessingType.Pass);
                        continue;

                    }

                    // item set
                    var genType = property.EnumerableType;
                    if (genType.IsValueType || genType == typeof(string))
                    {
                        instructions.Add(property, ProcessingType.Pass);
                        continue;
                    }

                    if (genType.IsAssignableFrom(productType) ||
                        productType.IsAssignableFrom(genType))
                    {
                        instructions.Add(property, ProcessingType.List);
                        continue;
                    }

                    instructions.Add(property, ProcessingType.Pass);
                    continue;
                }
                result.Add(type.Type, instructions);
            }
            return result;
        }

        private enum ProcessingType
        {
            Pass,
            Remove,
            Entity,
            List
        }

        private Dictionary<int, IIfcStyledItem> _inverseStyleLookup;

        private void BuildInverseStyleLookup(IModel source)
        {
            _inverseStyleLookup = new Dictionary<int, IIfcStyledItem>();
            foreach (var item in source.Instances.OfType<IIfcStyledItem>())
            {
                if (item.Item == null)
                    continue;

                if (_inverseStyleLookup.ContainsKey(item.Item.EntityLabel))
                    continue;

                _inverseStyleLookup.Add(item.Item.EntityLabel, item);
            }
        }

        private PropertyTranformDelegate GetFilter(List<IIfcProduct> primaryElements, bool includeGeometry, bool includeSiteGeometry, ExpressMetaData metadata)
        {
            var primaryIds = new HashSet<int>(primaryElements.Select(e => e.EntityLabel));
            var ignoredTypes = GetIgnoredTypes(metadata);
            var instructions = GetProcessingInstructions(metadata, includeGeometry, includeSiteGeometry);

            return new PropertyTranformDelegate((ExpressMetaProperty property, object parentObject) =>
            {
                //ignore inverses except for style
                if (property.IsInverse)
                {
                    if (property.Name[0] == 'S' && property.Name == "StyledByItem")
                    {
                        var label = (parentObject as IPersistEntity).EntityLabel;
                        if (_inverseStyleLookup.TryGetValue(label, out IIfcStyledItem style))
                            return style;
                        return null;
                    }
                    return null;
                }


                var value = property.PropertyInfo.GetValue(parentObject, null);
                // all our checking logic is for objects, not values
                if (property.PropertyInfo.PropertyType.IsValueType)
                    return value;

                var parentType = parentObject.GetType();
                if (ignoredTypes.Contains(parentType))
                    return value;

                if (!instructions.TryGetValue(parentType, out Dictionary<ExpressMetaProperty, ProcessingType> processing))
                    return value;

                var instruction = processing[property];
                switch (instruction)
                {
                    case ProcessingType.Pass:
                        return value;
                    case ProcessingType.Remove:
                        return null;
                    case ProcessingType.Entity:
                        {
                            // product but not listed
                            if (value is IIfcProduct product && !primaryIds.Contains(product.EntityLabel))
                                return null;
                            else
                                return value;
                        }
                    case ProcessingType.List:
                        {
                            //this can either be a list of IPersistEntity or select type. The very base type is IPersist
                            if (value is IEnumerable<IPersist> entities)
                            {
                                return entities
                                    .Where(entity => !(entity is IIfcProduct p) || primaryIds.Contains(p.EntityLabel))
                                    .ToList();
                            }
                            else
                                return value;
                        }
                    default:
                        return value;
                }
            });


        }

        private static IEnumerable<IIfcRelVoidsElement> GetFeatureRelations(IEnumerable<IIfcProduct> products)
        {
            var elementIds = new HashSet<int>(products.OfType<IIfcElement>().Select(e => e.EntityLabel));
            if (elementIds.Count == 0)
                return Enumerable.Empty<IIfcRelVoidsElement>();
            var model = products.First().Model;
            return model.Instances
                .Where<IIfcRelVoidsElement>(r => elementIds.Contains(r.RelatingBuildingElement.EntityLabel));

        }

        private IEnumerable<IIfcRelDecomposes> GetAggregations(HashSet<int> productIds, IModel source, List<IIfcProduct> decomposition)
        {
            decomposition.Clear();
            var result = new List<IIfcRelDecomposes>();
            while (productIds.Any())
            {
                var rels = source.Instances.Where<IIfcRelDecomposes>(r =>
                {
                    if (r is IIfcRelAggregates aggr)
                        return productIds.Contains(aggr.RelatingObject.EntityLabel);
                    if (r is IIfcRelNests nest)
                        return productIds.Contains(nest.RelatingObject.EntityLabel);
                    if (r is IIfcRelProjectsElement prj)
                        return productIds.Contains(prj.RelatingElement.EntityLabel);
                    if (r is IIfcRelVoidsElement voids)
                        return productIds.Contains(voids.RelatingBuildingElement.EntityLabel);
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

                result.AddRange(rels);
                decomposition.AddRange(relatedProducts);
                productIds = new HashSet<int>(relatedProducts.Select(p => p.EntityLabel));
            }
            return result;
        }

        private static IEnumerable<IIfcRelAggregates> GetUpstreamHierarchy(IEnumerable<IIfcSpatialElement> spatialStructureElements, IModel model)
        {
            var allRels = model.Instances.Where<IIfcRelAggregates>(r => r.RelatingObject is IIfcSpatialStructureElement || r.RelatedObjects.Any(o => o is IIfcSpatialStructureElement));
            var lookUp = new Dictionary<int, HashSet<IIfcRelAggregates>>();
            foreach (var rel in allRels)
            {
                foreach (var item in rel.RelatedObjects.OfType<IIfcSpatialStructureElement>())
                {
                    if (lookUp.TryGetValue(item.EntityLabel, out HashSet<IIfcRelAggregates> rels))
                    {
                        rels.Add(rel);
                        continue;
                    }
                    rels = new HashSet<IIfcRelAggregates>(new[] { rel });
                    lookUp.Add(item.EntityLabel, rels);
                }
            }

            while (spatialStructureElements.Any())
            {
                var rels = new HashSet<IIfcRelAggregates>();
                foreach (var id in spatialStructureElements.Select(s => s.EntityLabel))
                {
                    if (lookUp.TryGetValue(id, out HashSet<IIfcRelAggregates> rls))
                        foreach (var r in rls)
                            rels.Add(r);
                }

                spatialStructureElements = rels.Select(r => r.RelatingObject).OfType<IIfcSpatialStructureElement>();

                foreach (var rel in rels)
                    yield return rel;
            }
        }
        #endregion
    }
}
