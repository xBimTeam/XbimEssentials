using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;

namespace Xbim.IO
{
    public class ModelHelper
    {
        #region Delete
        /// <summary>
        /// This only keeps cache of metadata and types to speed up reflection search.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<DeleteCandidateType>> DeteteCandidatesCache =
            new ConcurrentDictionary<Type, List<DeleteCandidateType>>();

        /// <summary>
        /// This will delete the entity from model dictionary and also from any references in the model.
        /// Be carefull as this might take a while to check for all occurances of the object. Also make sure 
        /// you don't use this object anymore yourself because it won't get disposed until than. This operation
        /// doesn't guarantee that model is compliant with any kind of schema but it leaves it consistent. So if you
        /// serialize the model there won't be any references to the object which wouldn't be there.
        /// </summary>
        /// <param name="model">Model from which the entity should be deleted</param>
        /// <param name="entity">Entity to be deleted</param>
        /// <param name="instanceRemoval">Delegate to be used to remove entity from instance collection. 
        /// This should be reversable action within a transaction.</param>
        public static void Delete(IModel model, IPersistEntity entity, Func<IPersistEntity, bool> instanceRemoval)
        {
            var entityType = entity.GetType();
            List<DeleteCandidateType> candidateTypes;
            if (!DeteteCandidatesCache.TryGetValue(entityType, out candidateTypes))
            {
                candidateTypes = new List<DeleteCandidateType>();
                DeteteCandidatesCache.TryAdd(entityType, candidateTypes);

                //find all potential references and delete from there
                var types = model.Metadata.Types().Where(t => typeof(IInstantiableEntity).IsAssignableFrom(t.Type));
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var type in types)
                {
                    var toNullify = type.Properties.Values.Where(p =>
                        p.EntityAttribute != null && p.EntityAttribute.Order > 0 &&
                        p.PropertyInfo.PropertyType.IsAssignableFrom(entityType)).ToList();
                    var toRemove =
                        type.Properties.Values.Where(p =>
                            p.EntityAttribute != null && p.EntityAttribute.Order > 0 &&
                            p.PropertyInfo.PropertyType.IsGenericType &&
                            p.PropertyInfo.PropertyType.GenericTypeArgumentIsAssignableFrom(entityType)).ToList();
                    if (!toNullify.Any() && !toRemove.Any()) continue;

                    candidateTypes.Add(new DeleteCandidateType { Type = type, ToNullify = toNullify, ToRemove = toRemove });
                }
            }

            foreach (var candidateType in candidateTypes)
                DeleteReferences(model, entity, candidateType);

            //Remove from entity collection. This must be at the end for case it is being used in the meantime.
            instanceRemoval(entity);
        }

        /// <summary>
        /// Deletes references to specified entity from all entities in the model where entity is
        /// a references as an object or as a member of a collection.
        /// </summary>
        /// <param name="model">Model to be used</param>
        /// <param name="entity">Entity to be removed from references</param>
        /// <param name="candidateType">Candidate type containing reference to the type of entity</param>
        private static void DeleteReferences(IModel model, IPersistEntity entity, DeleteCandidateType candidateType)
        {
            //get all instances of this type and nullify and remove the entity
            var entitiesToCheck = model.Instances.OfType(candidateType.Type.Type.Name, true);
            foreach (var toCheck in entitiesToCheck)
            {
                //check properties
                foreach (var pInfo in candidateType.ToNullify.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null) continue;
                    //it is enough to compare references
                    if (!ReferenceEquals(pVal, entity)) continue;
                    pInfo.SetValue(toCheck, null);
                }

                foreach (var pInfo in candidateType.ToRemove.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null) continue;

                    //it might be uninitialized optional item set
                    var optSet = pVal as IOptionalItemSet;
                    if (optSet != null && !optSet.Initialized) continue;

                    //or it is non-optional item set implementind IList
                    var itemSet = pVal as IList;
                    if (itemSet != null)
                    {
                        if (itemSet.Contains(entity))
                            itemSet.Remove(entity);
                        continue;
                    }

                    //fall back operating on common list functions using reflection (this is slow)
                    var contMethod = pInfo.PropertyType.GetMethod("Contains");
                    if (contMethod == null) continue;
                    var contains = (bool)contMethod.Invoke(pVal, new object[] { entity });
                    if (!contains) continue;
                    var removeMethod = pInfo.PropertyType.GetMethod("Remove");
                    if (removeMethod == null) continue;
                    removeMethod.Invoke(pVal, new object[] { entity });
                }
            }
       
        }

        /// <summary>
        /// Helper structure to hold information for reference removal. If multiple objects of the same type are to
        /// be removed this will cache the information about where to have a look for the references.
        /// </summary>
        private struct DeleteCandidateType
        {
            public ExpressType Type;
            public List<ExpressMetaProperty> ToNullify;
            public List<ExpressMetaProperty> ToRemove;
        }
 #endregion

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
           bool keepLabels, Func<Type, int,IPersistEntity> getLabeledEntity) where T : IPersistEntity
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
                    if (theType.IsValueType || typeof(ExpressType).IsAssignableFrom(theType) ||
                        theType == typeof(string))
                    {
                        prop.PropertyInfo.SetValue(copy, value, null);
                    }
                    else if (!isInverse && typeof(IPersistEntity).IsAssignableFrom(theType))
                    {
                        prop.PropertyInfo.SetValue(copy,
                            InsertCopy(model, (IPersistEntity)value, mappings, propTransform, includeInverses, keepLabels, getLabeledEntity), null);
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
                                var cpy = InsertCopy(model, (IPersistEntity)item, mappings, propTransform, includeInverses,
                                    keepLabels, getLabeledEntity);
                                copyColl.Add(cpy);
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
        #endregion
    }
}
