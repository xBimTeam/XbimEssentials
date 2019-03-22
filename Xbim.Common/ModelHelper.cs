using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xbim.IO;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.IO.Step21;
using Xbim.IO.Parser;
using Microsoft.Extensions.Logging;

namespace Xbim.Common
{
    public class ModelHelper
    {
        #region Delete
        /// <summary>
        /// This only keeps cache of metadata and types to speed up reflection search.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<ReferingType>> ReferingTypesCache =
            new ConcurrentDictionary<Type, List<ReferingType>>();

        /// <summary>
        /// This will remove all references to the entity from model .
        /// Be carefull as this might take a while to check for all occurances of the object. This operation
        /// doesn't guarantee that model is compliant with any kind of schema but it leaves it consistent. So if you
        /// serialize the model there won't be any references to the object which wouldn't be there.
        /// After this funcion it run it is safe to remove the entity from the model.
        /// </summary>
        /// <param name="model">Model from which the entity should be deleted</param>
        /// <param name="entity">Entity to be deleted</param>
        public static void RemoveReferences(IModel model, IPersistEntity entity)
        {
            var referingTypes = GetReferingTypes(model, entity);
            foreach (var referingType in referingTypes)
                ReplaceReferences(model, entity, referingType, null, model.Logger);
        }

        /// <summary>
        /// This will replace the entity with another entity and will optionally remove it from model dictionary.
        /// This will replace all references in the model.
        /// Be carefull as this might take a while to check for all occurances of the object. 
        /// </summary>
        /// <param name="entity">Entity to be replaces</param>
        /// <param name="replacement">Entity to replace first entity</param>
        public static void Replace<TEntity, TReplacement>(TEntity entity, TReplacement replacement)
            where TEntity : IPersistEntity
            where TReplacement : IPersistEntity
        {
            var model = entity.Model;
            var referingTypes = GetReferingTypes(model, entity);
            foreach (var referingType in referingTypes)
                ReplaceReferences(model, entity, referingType, replacement, model.Logger);
        }

        /// <summary>
        /// This helper function can be used to replace one type for other. For example IfcBuildingElementProxy for IfcAirFlowTerminal.
        /// It will create the replacement object and replace all references in the model so that original objects will remain there 
        /// and can be safely removed while maintaining referential integrity of the model.
        /// </summary>
        /// <typeparam name="TOriginal">Type of the original entities</typeparam>
        /// <typeparam name="TReplacement">Type to be instantiated to replace original entities</typeparam>
        /// <param name="model">Model of entities</param>
        /// <param name="entities">Entities to be replaced</param>
        /// <param name="action">Action to perform on newly created entities. This can be null if no action is required</param>
        /// <returns></returns>
        public static Dictionary<TOriginal, TReplacement> Replace<TOriginal, TReplacement>(IModel model,
            IEnumerable<TOriginal> entities, Action<TOriginal, TReplacement> action = null)
            where TReplacement : IInstantiableEntity
            where TOriginal : IPersistEntity
        {
            // sort into mono typed collections
            var toReplace = new Dictionary<Type, Dictionary<TOriginal, TReplacement>>();
            TReplacement dummy = default(TReplacement);
            foreach (var entity in entities)
            {
                var type = entity.GetType();
                if (toReplace.TryGetValue(type, out Dictionary<TOriginal, TReplacement> r))
                    r.Add(entity, dummy);
                else
                    toReplace.Add(type, new Dictionary<TOriginal, TReplacement> { { entity, dummy } });
            }

            var result = new Dictionary<TOriginal, TReplacement>();
            var replacementType = model.Metadata.ExpressType(typeof(TReplacement));
            foreach (var kvp in toReplace)
            {
                var type = model.Metadata.ExpressType(kvp.Key);
                var commonType = GetCommonAncestor(type, replacementType);
                var referingTypes = GetReferingTypes(model, type.Type);

                var replacements = kvp.Value;
                foreach (var entity in replacements.Keys.ToList())
                {
                    var replacement = InsertShallowCopy<TReplacement>(entity);
                    action?.Invoke(entity, replacement);
                    replacements[entity] = replacement;
                    result.Add(entity, replacement);
                }

                foreach (var referingType in referingTypes)
                    ReplaceReferences(model, replacements, referingType, model.Logger);
            }

            return result;
        }

        /// <summary>
        /// Batch delete is a lot more performant because it only traverses all the candidate refering entities once
        /// and uses hash search to identiy the match
        /// </summary>
        /// <param name="model">Model to be used</param>
        /// <param name="entities">Entities to be deleted</param>
        public static void RemoveReferences(IModel model, IEnumerable<IPersistEntity> entities)
        {
            var f = entities.FirstOrDefault();
            if (f == null)
                return;

            // make uniformly typed lists
            var toDelete = new Dictionary<Type, HashSet<IPersistEntity>>();
            foreach (var entity in entities)
            {
                var t = entities.GetType();
                if (toDelete.TryGetValue(t, out HashSet<IPersistEntity> e))
                {
                    e.Add(entity);
                    continue;
                }
                toDelete.Add(t, new HashSet<IPersistEntity> { entity });
            }
            foreach (var kvp in toDelete)
            {
                var t = kvp.Key;
                IPersistEntity replacement = null;
                var del = kvp.Value.ToDictionary(e => e, e => replacement);

                var referingTypes = GetReferingTypes(model, t);
                foreach (var referingType in referingTypes)
                    ReplaceReferences(model, del, referingType, model.Logger);
            }
        }

        /// <summary>
        /// If the input types are fifferent this returns their common super type if any
        /// </summary>
        /// <param name="a">Type A</param>
        /// <param name="b">Type B</param>
        /// <returns></returns>
        private static ExpressType GetCommonAncestor(ExpressType a, ExpressType b)
        {
            if (a == b)
                return a;

            // if required output type is different to input type we should find a common ancestor
            var found = false;
            while (a != null)
            {

                var outExpress = b;
                while (outExpress != null)
                {
                    if (outExpress == a)
                    {
                        a = outExpress;
                        found = true;
                        break;
                    }
                    outExpress = outExpress.SuperType;
                }
                if (found)
                    break;
                a = a.SuperType;
            }
            return a;
        }

        private static IEnumerable<ReferingType> GetReferingTypes(IModel model, IPersistEntity entity)
        {
            var entityType = entity.GetType();
            return GetReferingTypes(model, entityType);
        }

        /// <summary>
        /// Gets all types refering to the specified type. This information can be used to
        /// replace or remove the reference in a way where data integrity of the model is maintained.
        /// </summary>
        /// <param name="model">Model with metadata</param>
        /// <param name="entityType">Type of IPersistEntity</param>
        /// <returns></returns>
        private static IEnumerable<ReferingType> GetReferingTypes(IModel model, Type entityType)
        {
            if (ReferingTypesCache.TryGetValue(entityType, out List<ReferingType> referingTypes))
                return referingTypes;

            referingTypes = new List<ReferingType>();
            if (!ReferingTypesCache.TryAdd(entityType, referingTypes))
            {
                //it is there already (done in another thread)
                return ReferingTypesCache[entityType];
            }

            //find all potential references
            var types = model.Metadata.Types().Where(t => typeof(IInstantiableEntity).GetTypeInfo().IsAssignableFrom(t.Type));

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var type in types)
            {
                var singleReferences = type.Properties.Values.Where(p =>
                    p.EntityAttribute != null && p.EntityAttribute.Order > 0 &&
                    p.PropertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(entityType)).ToList();
                var listReferences =
                    type.Properties.Values.Where(p =>
                        p.EntityAttribute != null && p.EntityAttribute.Order > 0 &&
                        p.PropertyInfo.PropertyType.GetTypeInfo().IsGenericType &&
                        p.PropertyInfo.PropertyType.GenericTypeArgumentIsAssignableFrom(entityType)).ToList();
                if (!singleReferences.Any() && !listReferences.Any()) continue;

                referingTypes.Add(new ReferingType { Type = type, SingleReferences = singleReferences, ListReferences = listReferences });
            }
            return referingTypes;
        }

        /// <summary>
        /// Deletes references to specified entity from all entities in the model where entity is
        /// a references as an object or as a member of a collection.
        /// </summary>
        /// <param name="model">Model to be used</param>
        /// <param name="entity">Entity to be removed from references</param>
        /// <param name="referingType">Candidate type containing reference to the type of entity</param>
        /// <param name="replacement">New reference. If this is null it just removes references to entity</param>
        /// <param name="log">Log object</param>
        private static void ReplaceReferences(IModel model, IPersistEntity entity, ReferingType referingType, IPersistEntity replacement, ILogger log)
        {
            if (entity == null)
                return;

            //get all instances of this type and nullify and remove the entity
            var entitiesToCheck = model.Instances.OfType(referingType.Type.Type.Name, true);
            foreach (var toCheck in entitiesToCheck)
            {
                //check properties
                foreach (var pInfo in referingType.SingleReferences.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null && replacement == null)
                        continue;

                    //it is enough to compare references
                    if (!ReferenceEquals(pVal, entity)) continue;
                    try
                    {
                        pInfo.SetValue(toCheck, replacement);
                    }
                    catch (Exception)
                    {
                        log.LogWarning($"Incompatible replacement: {toCheck.GetType().Name}.{pInfo.Name} Expected type: {pInfo.PropertyType.Name} Actual type: {replacement.GetType().Name}");
                    }
                }

                foreach (var pInfo in referingType.ListReferences.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null) continue;

                    //it might be uninitialized optional item set
                    if (pVal is IOptionalItemSet optSet && !optSet.Initialized)
                        continue;

                    //or it is non-optional item set implementing IList
                    if (pVal is IList itemSet)
                    {
                        for (int i = 0; i < itemSet.Count; i++)
                        {
                            var item = itemSet[i];
                            if (!ReferenceEquals(item, entity))
                                continue;
                            itemSet.RemoveAt(i);
                            if (replacement != null)
                            {
                                try
                                {
                                    itemSet.Insert(i, replacement);
                                }
                                catch (Exception)
                                {
                                    log.LogWarning($"Incompatible replacement: {toCheck.GetType().Name}.{pInfo.Name} Expected type: {pInfo.PropertyType.GenericTypeArguments[0].Name} Actual type: {replacement.GetType().Name}");
                                }
                            }
                        }
                    }
                    else
                    {
                        var e = new NotSupportedException($"Property {toCheck.GetType().Name}.{pInfo.Name} doesn't implement IList interface which is necessary for replacement code to work");
                        log.LogError(e, "Failed to replace in list");
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Batch replacement of references is more efficient because it only performs linear search.
        /// Replacing one by one has exponential performance hit
        /// </summary>
        /// <param name="model">Model to operate on</param>
        /// <param name="replacements">Dictionary where keys are objects to be replaced and values are replacements</param>
        /// <param name="referingType">Cached reflection</param>
        /// <param name="log">Log</param>
        private static void ReplaceReferences(IModel model, IDictionary replacements, ReferingType referingType, ILogger log)
        {
            //get all instances of this type and nullify and remove the entity
            var entitiesToCheck = model.Instances.OfType(referingType.Type.Type.Name, true);
            foreach (var toCheck in entitiesToCheck)
            {
                //check single value properties
                foreach (var pInfo in referingType.SingleReferences.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck) as IPersistEntity;
                    if (!replacements.Contains(pVal))
                        continue;

                    var replacement = replacements[pVal];
                    try
                    {
                        pInfo.SetValue(toCheck, replacement);
                    }
                    catch (Exception)
                    {
                        log.LogWarning($"Incompatible replacement: {toCheck.GetType().Name}.{pInfo.Name} Expected type: {pInfo.PropertyType.Name} Actual type: {replacement.GetType().Name}");
                        // if it failed to replace, set to null to maintain referential integrity
                        pInfo.SetValue(toCheck, null);
                    }
                }

                // check list properties
                foreach (var pInfo in referingType.ListReferences.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null) continue;

                    //it might be uninitialized optional item set
                    if (pVal is IOptionalItemSet optSet && !optSet.Initialized)
                        continue;

                    //or it is non-optional item set implementing IList
                    if (pVal is IList itemSet)
                    {
                        for (int i = 0; i < itemSet.Count; i++)
                        {
                            if (!(itemSet[i] is IPersistEntity item))
                                continue;

                            if (!replacements.Contains(item))
                                continue;

                            var replacement = replacements[item];
                            itemSet.RemoveAt(i);
                            if (replacement != null)
                            {
                                try
                                {
                                    itemSet.Insert(i, replacement);
                                }
                                catch (Exception)
                                {
                                    log.LogWarning($"Incompatible replacement: {toCheck.GetType().Name}.{pInfo.Name} Expected type: {pInfo.PropertyType.GenericTypeArguments[0].Name} Actual type: {replacement.GetType().Name}");
                                }
                            }
                        }
                    }
                    else
                    {
                        var e = new NotSupportedException($"Property {toCheck.GetType().Name}.{pInfo.Name} doesn't implement IList interface which is necessary for replacement code to work");
                        log.LogError(e, "Failed to replace in list");
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// Helper structure to hold information for reference removal. If multiple objects of the same type are to
        /// be removed this will cache the information about where to have a look for the references.
        /// </summary>
        private struct ReferingType
        {
            public ExpressType Type;
            public List<ExpressMetaProperty> SingleReferences;
            public List<ExpressMetaProperty> ListReferences;
        }
        #endregion

        public static void Expand<IParentEntity, IUniqueEntity>(IModel model, Func<IParentEntity, ICollection<IUniqueEntity>> accessor) where IParentEntity : IPersistEntity where IUniqueEntity : IPersistEntity
        {
            //get duplicates in one go to avoid exponential search
            var candidates = new Dictionary<IUniqueEntity, List<IParentEntity>>();
            foreach (var entity in model.Instances.OfType<IParentEntity>())
            {
                foreach (var val in accessor(entity))
                {
                    List<IParentEntity> assets;
                    if (!candidates.TryGetValue(val, out assets))
                    {
                        assets = new List<IParentEntity>();
                        candidates.Add(val, assets);
                    }
                    assets.Add(entity);
                }
            }

            var multi = candidates.Where(a => a.Value.Count > 1);
            var map = new XbimInstanceHandleMap(model, model);

            foreach (var kvp in multi)
            {
                var value = kvp.Key;
                var entities = kvp.Value;

                //skip the first
                for (int i = 1; i < entities.Count; i++)
                {
                    //clear map to create complete copy every time
                    map.Clear();
                    var copy = model.InsertCopy(value, map, null, false, false);

                    //remove original and add fresh copy
                    var entity = entities[i];
                    var collection = accessor(entity);
                    collection.Remove(value);
                    collection.Add(copy);
                }
            }
        }

        #region Insert
        /// <summary>
        /// Inserts deep copy of an object into this model. The entity must originate from the same schema (the same EntityFactory). 
        /// This operation happens within a transaction which you have to handle yourself unless you set the parameter "noTransaction" to true.
        /// Insert will happen outside of transactional behaviour in that case. Resulting model is not guaranteed to be valid according to any
        /// model view definition. However, it is granted to be consistent. You can optionaly bring in all inverse relationships. Be carefull as it
        /// might easily bring in almost full model.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the copied entity</typeparam>
        /// <param name="model">Model to be used as a target</param>
        /// <param name="toCopy">Entity to be copied</param>
        /// <param name="mappings">Mappings of previous inserts</param>
        /// <param name="includeInverses">Option if to bring in all inverse entities (enumerations in original entity)</param>
        /// <param name="keepLabels">Option if to keep entity labels the same</param>
        /// <param name="propTransform">Optional delegate which you can use to filter the content which will get coppied over.</param>
        /// <param name="getLabeledEntity">Functor to be used to create entity with specified label</param>
        /// <returns>Copy from this model</returns>
        public static T InsertCopy<T>(IModel model, T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
           bool keepLabels, Func<Type, int, IPersistEntity> getLabeledEntity) where T : IPersistEntity
        {
            try
            {
                var toCopyLabel = toCopy.EntityLabel;
                XbimInstanceHandle copyHandle;
                var toCopyHandle = new XbimInstanceHandle(toCopy);
                //try to get the value if it was created before
                if (mappings.TryGetValue(toCopyHandle, out copyHandle))
                {
                    return (T)copyHandle.GetEntity();
                }

                var expressType = model.Metadata.ExpressType(toCopy);
                var copy = keepLabels ? getLabeledEntity(toCopy.GetType(), toCopyLabel) : model.Instances.New(toCopy.GetType());

                copyHandle = new XbimInstanceHandle(copy);
                //key is the label in original model
                mappings.Add(toCopyHandle, copyHandle);

                var props = expressType.Properties.Values.Where(p => !p.EntityAttribute.IsDerived);
                if (includeInverses)
                    props = props.Union(expressType.Inverses);

                foreach (var prop in props)
                {
                    var value = propTransform != null
                        ? propTransform(prop, toCopy)
                        : prop.PropertyInfo.GetValue(toCopy, null);
                    if (value == null) continue;

                    var isInverse = (prop.EntityAttribute.Order == -1); //don't try and set the values for inverses
                    var theType = value.GetType();
                    //if it is an express type or a value type, set the value
                    if (theType.GetTypeInfo().IsValueType || typeof(ExpressType).GetTypeInfo().IsAssignableFrom(theType) ||
                        theType == typeof(string))
                    {
                        prop.PropertyInfo.SetValue(copy, value, null);
                    }
                    else if (!isInverse && typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(theType))
                    {
                        prop.PropertyInfo.SetValue(copy,
                            InsertCopy(model, (IPersistEntity)value, mappings, propTransform, includeInverses, keepLabels, getLabeledEntity), null);
                    }
                    else if (!isInverse && typeof(IList).GetTypeInfo().IsAssignableFrom(theType))
                    {
                        var itemType = theType.GetItemTypeFromGenericType();

                        var copyColl = prop.PropertyInfo.GetValue(copy, null) as IList;
                        if (copyColl == null)
                            throw new Exception(string.Format("Unexpected collection type ({0}) found", itemType.Name));

                        foreach (var item in (IList)value)
                        {
                            var actualItemType = item.GetType();
                            if (actualItemType.GetTypeInfo().IsValueType || typeof(ExpressType).GetTypeInfo().IsAssignableFrom(actualItemType))
                                copyColl.Add(item);
                            else if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(actualItemType))
                            {
                                var cpy = InsertCopy(model, (IPersistEntity)item, mappings, propTransform, includeInverses,
                                    keepLabels, getLabeledEntity);
                                copyColl.Add(cpy);
                            }
                            else if (typeof(IList).GetTypeInfo().IsAssignableFrom(actualItemType)) //list of lists
                            {
                                var listColl = (IList)item;
                                var getAt = copyColl.GetType().GetTypeInfo().GetMethod("GetAt");
                                if (getAt == null) throw new Exception(string.Format("GetAt Method not found on ({0}) found", copyColl.GetType().Name));
                                var copyListColl = getAt.Invoke(copyColl, new object[] { copyColl.Count }) as IList;
                                if (copyListColl == null)
                                    throw new XbimException("Collection can't be used as IList");
                                foreach (var listItem in listColl)
                                {
                                    var actualListItemType = listItem.GetType();
                                    if (actualListItemType.GetTypeInfo().IsValueType ||
                                        typeof(ExpressType).GetTypeInfo().IsAssignableFrom(actualListItemType))
                                        copyListColl.Add(listItem);
                                    else if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(actualListItemType))
                                    {
                                        var cpy = InsertCopy(model, (IPersistEntity)listItem, mappings, propTransform,
                                            includeInverses,
                                            keepLabels, getLabeledEntity);
                                        copyListColl.Add(cpy);
                                    }
                                    else
                                        throw new Exception(string.Format("Unexpected collection item type ({0}) found",
                                            itemType.Name));
                                }
                            }
                            else
                                throw new Exception(string.Format("Unexpected collection item type ({0}) found",
                                    itemType.Name));
                        }
                    }
                    else if (isInverse && value is IEnumerable<IPersistEntity>) //just an enumeration of IPersistEntity
                    {
                        foreach (var ent in (IEnumerable<IPersistEntity>)value)
                        {
                            InsertCopy(model, ent, mappings, propTransform, includeInverses, keepLabels, getLabeledEntity);
                        }
                    }
                    else if (isInverse && value is IPersistEntity) //it is an inverse and has a single value
                    {
                        InsertCopy(model, (IPersistEntity)value, mappings, propTransform, includeInverses, keepLabels, getLabeledEntity);
                    }
                    else
                        throw new Exception(string.Format("Unexpected item type ({0})  found", theType.Name));
                }
                return (T)copy;
            }
            catch (Exception e)
            {
                throw new XbimException(string.Format("General failure in InsertCopy ({0})", e.Message), e);
            }
        }

        /// <summary>
        /// Inserts shallow copy of an object into the same model. The entity must originate from the same schema (the same EntityFactory). 
        /// 
        /// 
        /// </summary>
        /// <typeparam name="TOut">Prefered output type. Should have shared ancestor at some level with the input type</typeparam>
        /// <param name="toCopy">Entity to be copied</param>
        /// <returns>Copy from this model</returns>
        public static TOut InsertShallowCopy<TOut>(IPersistEntity toCopy) where TOut : IPersistEntity, IInstantiableEntity
        {
            var model = toCopy.Model;
            try
            {
                var expressType = GetCommonAncestor(model.Metadata.ExpressType(toCopy), model.Metadata.ExpressType(typeof(TOut)));
                var copy = model.Instances.New<TOut>();

                var props = expressType.Properties.Values.Where(p => !p.EntityAttribute.IsDerived);
                foreach (var prop in props)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    if (value == null) continue;

                    var isInverse = (prop.EntityAttribute.Order == -1); //don't try and set the values for inverses
                    var theType = value.GetType();
                    //if it is an express type or a value type, set the value
                    if (theType.IsValueType || typeof(ExpressType).IsAssignableFrom(theType) ||
                        theType == typeof(string))
                    {
                        prop.PropertyInfo.SetValue(copy, value, null);
                    }
                    else if (!isInverse && typeof(IPersistEntity).IsAssignableFrom(theType))
                    {
                        prop.PropertyInfo.SetValue(copy, value, null);
                    }
                    else if (!isInverse && typeof(IList).IsAssignableFrom(theType))
                    {
                        var itemType = theType.GetItemTypeFromGenericType();

                        var copyColl = prop.PropertyInfo.GetValue(copy, null) as IList;
                        if (copyColl == null)
                            throw new Exception(string.Format("Unexpected collection type ({0}) found", itemType.Name));

                        foreach (var item in (IList)value)
                        {
                            var actualItemType = item.GetType();
                            if (actualItemType.IsValueType || typeof(ExpressType).IsAssignableFrom(actualItemType))
                                copyColl.Add(item);
                            else if (typeof(IPersistEntity).IsAssignableFrom(actualItemType))
                            {
                                copyColl.Add(item);
                            }
                            else if (typeof(IList).IsAssignableFrom(actualItemType)) //list of lists
                            {
                                var listColl = (IList)item;
                                var getAt = copyColl.GetType().GetMethod("GetAt");
                                if (getAt == null) throw new Exception(string.Format("GetAt Method not found on ({0}) found", copyColl.GetType().Name));
                                var copyListColl = getAt.Invoke(copyColl, new object[] { copyColl.Count }) as IList;
                                if (copyListColl == null)
                                    throw new XbimException("Collection can't be used as IList");
                                foreach (var listItem in listColl)
                                {
                                    var actualListItemType = listItem.GetType();
                                    if (actualListItemType.IsValueType ||
                                        typeof(ExpressType).IsAssignableFrom(actualListItemType))
                                        copyListColl.Add(listItem);
                                    else if (typeof(IPersistEntity).IsAssignableFrom(actualListItemType))
                                    {
                                        copyListColl.Add(listItem);
                                    }
                                    else
                                        throw new Exception(string.Format("Unexpected collection item type ({0}) found",
                                            itemType.Name));
                                }
                            }
                            else
                                throw new Exception(string.Format("Unexpected collection item type ({0}) found",
                                    itemType.Name));
                        }
                    }
                    else
                        throw new Exception(string.Format("Unexpected item type ({0})  found", theType.Name));
                }
                return copy;
            }
            catch (Exception e)
            {
                throw new XbimException(string.Format("General failure in InsertCopy ({0})", e.Message), e);
            }
        }

        #endregion

        #region Partial file
        /// <summary>
        /// Writes data island of direct references into the writer
        /// </summary>
        /// <param name="model">Model to be used</param>
        /// <param name="root">Entity from the model to be used as the root of data island</param>
        /// <param name="writer">Writer to be used</param>
        /// <param name="written">List of entities written. In case multiple calls to this function are attempted using the same
        /// result stream this will make sure that the same entity gets serialized only once.</param>
        public static void WritePartialFile(IModel model, IPersistEntity root, TextWriter writer, HashSet<int> written)
        {
            WriteEntityRecursive(root, model.Metadata, writer, written);
        }

        private static void WriteEntityRecursive(IPersistEntity entity, ExpressMetaData metadata, TextWriter writer, HashSet<int> written)
        {
            if (written.Contains(entity.EntityLabel))
                return;

            Part21Writer.WriteEntity(entity, writer, metadata);
            written.Add(entity.EntityLabel);

            var references = entity as IContainsEntityReferences;
            if (references == null)
                return;

            foreach (var item in references.References)
            {
                WriteEntityRecursive(item, metadata, writer, written);
            }
        }
        #endregion

        #region Schema Version
        public static List<string> GetStepFileSchemaVersion(Stream stream)
        {
            var scanner = new Scanner(stream);
            int tok = scanner.yylex();
            int dataToken = (int)Tokens.DATA;
            int eof = (int)Tokens.EOF;
            int typeToken = (int)Tokens.TYPE;
            int stringToken = (int)Tokens.STRING;

            //looking for: FILE_SCHEMA(('IFC2X3'));
            var schemas = new List<string>();

            while (tok != dataToken && tok != eof)
            {
                if (tok != typeToken)
                {
                    tok = scanner.yylex();
                    continue;
                }

                if (!string.Equals(scanner.yylval.strVal, "FILE_SCHEMA", StringComparison.OrdinalIgnoreCase))
                {
                    tok = scanner.yylex();
                    continue;
                }

                tok = scanner.yylex();
                //go until closing bracket
                while (tok != ')')
                {
                    if (tok != stringToken)
                    {
                        tok = scanner.yylex();
                        continue;
                    }

                    schemas.Add(scanner.yylval.strVal.Trim('\''));
                    tok = scanner.yylex();
                }
                break;
            }
            return schemas;
        }
        #endregion
    }
}
