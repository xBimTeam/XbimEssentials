using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Logging;
using Xbim.Common.Metadata;

namespace Xbim.IO.Translation
{
    public class EntityTranslator
    {
        private readonly ModelTranslator _modelTranslator;
        protected readonly ILogger Logger = LoggerFactory.GetLogger();
        public IModel Source { get { return _modelTranslator.Source; } }
        public IModel Target { get { return _modelTranslator.Target; } }


        public EntityTranslator(ModelTranslator modelTranslator, Type canTranslate)
        {
            _modelTranslator = modelTranslator;
            CanTranslateType = canTranslate;
        }

        public virtual ExpressType TargetType
        {
            get { return Target.Metadata.ExpressType(CanTranslateType.Name.ToUpper()); }
        }

        public virtual ExpressType SourceType
        {
            get { return Source.Metadata.ExpressType(CanTranslateType); }
        }

        protected IEnumerable<ExpressMetaProperty> SourceExplicitProperties
        {
            get { return SourceType.Properties.Values.Where(p => !p.EntityAttribute.IsDerivedOverride && p.EntityAttribute.Order > 0); }
        }

        protected IEnumerable<ExpressMetaProperty> TargetExplicitProperties
        {
            get { return TargetType.Properties.Values.Where(p => !p.EntityAttribute.IsDerivedOverride && p.EntityAttribute.Order > 0); }
        }

        public Type CanTranslateType { get; private set; }

        public virtual IPersistEntity Translate(IPersistEntity original)
        {
            var result = CreateEntity(original);
            if (result == null)
            {
                Logger.ErrorFormat("It wasn't possible to create translation for {0}", SourceType.Name);
                return null;
            }

            //translate properties
            TranslateProperties(original, result);

            return result;
        }

        //results of this function needs to be cached if it is not based on the value
        protected virtual ExpressMetaProperty GetTargetExpressProperty(ExpressMetaProperty sourceProperty, object value)
        {
            var sourcePropName = sourceProperty.PropertyInfo.Name;
            //try name match
            var result = TargetExplicitProperties.FirstOrDefault(p => p.PropertyInfo.Name == sourcePropName);
            if (result != null)
                return result;

            //try property type match. If there is just one candidate it might be the right one.
            var candidates = TargetExplicitProperties.Where(p => p.PropertyInfo.PropertyType.Name == sourceProperty.PropertyInfo.PropertyType.Name).ToList();
            if (candidates.Count == 1)
                return candidates.First();

            //try to get assignable type
            var targetPropertyTypeCandidate =
                Target.Metadata.Module.GetType(sourceProperty.PropertyInfo.PropertyType.Name);
            if (targetPropertyTypeCandidate != null)
            {
                candidates = TargetExplicitProperties.Where(p => p.PropertyInfo.PropertyType.IsAssignableFrom(targetPropertyTypeCandidate)).ToList();
                if (candidates.Count == 1)
                    return candidates.First();    
            }

            //try value type match. If there is just one candidate it might be the right one.
            candidates = TargetExplicitProperties.Where(p => p.PropertyInfo.PropertyType.Name == value.GetType().Name).ToList();
            if (candidates.Count == 1)
                return candidates.First();

            //try to get assignable type
            targetPropertyTypeCandidate =
                Target.Metadata.Module.GetType(value.GetType().Name);
            if (targetPropertyTypeCandidate != null)
            {
                candidates = TargetExplicitProperties.Where(p => p.PropertyInfo.PropertyType.IsAssignableFrom(targetPropertyTypeCandidate)).ToList();
                if (candidates.Count == 1)
                    return candidates.First();
            }

            return null;
        }

        protected virtual object GetSourcePropertyValue(ExpressMetaProperty sourceProperty, IPersistEntity original)
        {
            return sourceProperty.PropertyInfo.GetValue(original, null);
        }

        protected virtual object GetTargetValueTypeValue(IPersistEntity result, object value, ExpressMetaProperty resultProperty)
        {
            var systemValue = GetNonNullableSystemValue(value);
            var typeToSet = GetTargetValueType(value, resultProperty);

            if (typeToSet == null)
            {
                Logger.ErrorFormat("It was not possible to find type for property {0} of {1} (value {2})", resultProperty.PropertyInfo.Name, result.GetType().Name, value);
                return null;
            }

            try
            {
                return  Activator.CreateInstance(typeToSet, systemValue);
            }
            catch (Exception)
            {
                Logger.ErrorFormat("It was not possible to create instance of {0} (value {1})", typeToSet.Name, result.GetType().Name, value);
            }

            return null;
        }

        protected Type GetTargetValueType(object sourceValue,
            ExpressMetaProperty resultProperty)
        {
            if (resultProperty.PropertyInfo.PropertyType.IsValueType)
            {
                if (!resultProperty.PropertyInfo.PropertyType.IsGenericType)
                    return resultProperty.PropertyInfo.PropertyType;

                if (resultProperty.PropertyInfo.PropertyType.IsGenericType)
                    return resultProperty.PropertyInfo.PropertyType.GetGenericArguments()[0];
            }

            if (typeof (IList).IsAssignableFrom(resultProperty.PropertyInfo.PropertyType))
            {
                return resultProperty.PropertyInfo.PropertyType.GetGenericArguments()[0];
            }

            var sourceType = sourceValue.GetType();
            if (!sourceType.IsValueType)
                throw new Exception("This function suppose that either value type or target property type are ValueType");

            if (!sourceType.IsGenericType)
            {
                return Target.Metadata.Module.GetType(sourceType.Name);
            }

            if (sourceType.IsGenericType)
            {
                sourceType = sourceType.GetGenericArguments()[0];
                return Target.Metadata.Module.GetType(sourceType.Name);
            }

            return null;
        }

        protected object GetNonNullableSystemValue(object value)
        {
            while (true)
            {
                var type = value.GetType();
                if (type.IsValueType && type.IsGenericType)
                {
                    value = type.GetProperty("Value").GetValue(value, null);
                    continue;
                }

                var expressType = value as IExpressValueType;
                if (expressType != null)
                    return expressType.Value;

                return value;
            }
        }

        protected virtual void TranslateProperties(IPersistEntity original, IPersistEntity result)
        {
            foreach (var prop in SourceExplicitProperties)
            {
                var value = GetSourcePropertyValue(prop, original);
                if (value == null) continue;

                var resultExpressProperty = GetTargetExpressProperty(prop, value);
                if (resultExpressProperty == null)
                {
                    Logger.ErrorFormat("It wasn't possible to find a match for property {0} of {1}/{2}", prop.PropertyInfo.Name, original.GetType().Name, result.GetType().Name);
                    continue;
                }

                var sourcePropertyType = value.GetType();
                //if it is an express type or a value type, set the value
                if (sourcePropertyType.IsValueType)
                {
                    var valueToSet = GetTargetValueTypeValue(result, value ,resultExpressProperty);
                    if(valueToSet != null)
                        resultExpressProperty.PropertyInfo.SetValue(result, valueToSet, null);
                }
                else if (typeof(IPersistEntity).IsAssignableFrom(sourcePropertyType))
                {
                    resultExpressProperty.PropertyInfo.SetValue(result, _modelTranslator.TranslateEntity((IPersistEntity)value), null);
                }
                else if (typeof(IList).IsAssignableFrom(sourcePropertyType))
                {
                    var copyColl = resultExpressProperty.PropertyInfo.GetValue(result, null) as IList;
                    if (copyColl == null)
                        throw new Exception(string.Format("Unexpected collection type ({0}) found.", resultExpressProperty.PropertyInfo.PropertyType.Name));

                    foreach (var item in (IList)value)
                    {
                        var actualItemType = item.GetType();
                        if (actualItemType.IsValueType)
                        {
                            var valueToSet = GetTargetValueTypeValue(result, value, resultExpressProperty);
                            copyColl.Add(valueToSet);
                        }
                        else if (typeof(IPersistEntity).IsAssignableFrom(actualItemType))
                        {
                            var cpy = _modelTranslator.TranslateEntity((IPersistEntity)item);
                            copyColl.Add(cpy);
                        }
                        else
                            throw new Exception(string.Format("Unexpected collection item type ({0}) found", resultExpressProperty.PropertyInfo.PropertyType.Name));
                    }
                }
                else
                    throw new Exception(string.Format("Unexpected item type ({0})  found", sourcePropertyType.Name));
            }
        }

        protected virtual IPersistEntity CreateEntity(IPersistEntity original)
        {
            return Target.Instances.New(TargetType.Type);
        }

    }
}
