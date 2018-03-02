using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xbim.Common.Metadata
{
    public class ExpressType
    {
        #region Fields

        private readonly SortedList<int, ExpressMetaProperty> _properties = new SortedList<int, ExpressMetaProperty>();
        private readonly List<ExpressMetaProperty> _inverses = new List<ExpressMetaProperty>();
        private readonly List<ExpressMetaProperty> _derives = new List<ExpressMetaProperty>();
        private readonly List<ExpressType> _subTypes = new List<ExpressType>();
        private List<ExpressType> _nonAbstractSubTypes;
        private readonly List<ExpressMetaProperty> _expressEnumerableProperties = new List<ExpressMetaProperty>();
        private readonly string _expressName;
        private readonly short _typeId;
        private readonly List<PropertyInfo> _indexedProperties;
        private readonly List<int> _indexedValues;
        private readonly string _expressNameUpper;
        private readonly Type _underlyingType;
        private readonly Type _underlyingComplexType;

        #endregion

        #region Properties

        public ExpressType SuperType { get; internal set; }

        public bool IndexedClass { get; private set; }

        public string ExpressName
        {
            get { return _expressName; }
        }

        public string ExpressNameUpper
        {
            get { return _expressNameUpper; }
        }

        public short TypeId
        {
            get { return _typeId; }
        }

        public IEnumerable<ExpressMetaProperty> Inverses
        {
            get { return _inverses; }
        }

        public IEnumerable<ExpressMetaProperty> Derives
        {
            get { return _derives; }
        }

        public SortedList<int, ExpressMetaProperty> Properties
        {
            get { return _properties; }
        }

        public Type Type { get; private set; }

        public IEnumerable<ExpressMetaProperty> ExpressEnumerableProperties
        {
            get
            {
                return _expressEnumerableProperties;
            }
        }

        /// <summary>
        /// Don't ask for this before types hierarchy is finished or it will cache incomplete result.
        /// </summary>
        public IEnumerable<ExpressType> NonAbstractSubTypes
        {
            get
            {
                lock (this)
                {
                    //this needs to be set up after hierarchy is set up
                    if (_nonAbstractSubTypes != null) return _nonAbstractSubTypes;
                    _nonAbstractSubTypes = new List<ExpressType>();
                    AddNonAbstractTypes(this, _nonAbstractSubTypes);
                }
                return _nonAbstractSubTypes;
            }
        }

        #endregion

        public ExpressType(Type type)
        {
            Type = type;
            var typeInfo = Type.GetTypeInfo();
            var entNameAttr = typeInfo.GetCustomAttributes(typeof(ExpressTypeAttribute), false).FirstOrDefault();
#if DEBUG
            if (entNameAttr == null)
                throw new Exception("Express Type is not defined for " + Type.Name);
#endif
            _typeId = (short)((ExpressTypeAttribute)entNameAttr).EntityTypeId;
            _expressName = ((ExpressTypeAttribute)entNameAttr).Name;
            _expressNameUpper = _expressName.ToUpperInvariant();

            //it is not an indexed class by default. If it has any indexed properties it is an indexed class but that is detected later on.
            IndexedClass = false;

            var dta = typeInfo.GetCustomAttributes(typeof(DefinedTypeAttribute), false).FirstOrDefault() as DefinedTypeAttribute;
            if (dta != null)
            {
                _underlyingType = dta.UnderlyingType;
                if (UnderlyingType.GetTypeInfo().IsGenericType && typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(UnderlyingType))
                {
                    var cplType = UnderlyingType.GetTypeInfo().GetGenericArguments()[0];
                    if (cplType != null)
                        _underlyingComplexType = cplType;
                }
            }

            var properties =
                typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var propInfo in properties)
            {
                var attribute =
                    ((EntityAttributeAttribute[])propInfo.GetCustomAttributes(typeof(EntityAttributeAttribute), false)).FirstOrDefault();
                if(attribute == null)
                    continue;

                var metaProperty = new ExpressMetaProperty {PropertyInfo = propInfo, EntityAttribute = attribute};
                if (attribute.Order > 0)
                {
                    _properties.Add(attribute.Order, metaProperty);
                }
                else if (attribute.State == EntityAttributeState.Derived)
                {
                    _derives.Add(metaProperty);
                }
                else
                {
                    var invAttr = propInfo.GetCustomAttributes(typeof (InverseProperty), false).First() as InverseProperty;
                    metaProperty.InverseAttributeProperty = invAttr;
                    _inverses.Add(metaProperty);
                }

                //set up enumerable type
                if (propInfo.PropertyType.GetTypeInfo().IsGenericType && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(propInfo.PropertyType))
                {
                    var eType = propInfo.PropertyType;
                    while (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(eType))
                    {
                        var genArgs = eType.GetTypeInfo().GetGenericArguments();
                        if (genArgs.Any())
                            eType = genArgs[0];
                        else
                            break;
                    }
                    metaProperty.EnumerableType = eType;
                }

                var isIndexed =
                    propInfo.GetCustomAttributes(typeof(IndexedProperty), false).Any();
                metaProperty.IsIndexed = isIndexed;
                if (!isIndexed) continue;

                //TODO: MC: Review with Steve. This is not true for IfcRelDefinesByProperties.RelatingPropertyDefinition in IFC4
                //Debug.Assert(typeof(IPersistEntity).IsAssignableFrom(propInfo.PropertyType)
                //    || typeof(IEnumerable<IPersistEntity>).IsAssignableFrom(propInfo.PropertyType)); //only handles to IPersistEntitiess or collecctions of IPersistEntities are indexable
                    
                if (_indexedProperties == null) _indexedProperties = new List<PropertyInfo>();
                if (_indexedValues == null) _indexedValues = new List<int>();
                _indexedProperties.Add(propInfo);
                _indexedValues.Add(attribute.Order);
                IndexedClass = true; //if it has keys it must be an indexed class
            }

            //cache enumerable properties
            foreach (var prop in _properties.Values.Where(prop => typeof(IExpressEnumerable).GetTypeInfo().IsAssignableFrom(prop.PropertyInfo.PropertyType)))
            {
                _expressEnumerableProperties.Add(prop);
            }
        }




        public override string ToString()
        {
            return Type.Name;
        }




        /// <summary>
        /// If the type has indexed attributes, this returns a set of unique values for the specified IPersistEntity
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public IEnumerable<int> GetIndexedValues(IPersistEntity ent)
        {
            if (IndexedProperties == null)
                return Enumerable.Empty<int>();
            var keys = new HashSet<int>();
            foreach (var prop in IndexedProperties)
            {
                var o = prop.GetValue(ent, null);
                if(o == null) 
                    continue;
                var optSet = o as IOptionalItemSet;
                if (optSet != null && !optSet.Initialized)
                    continue;

                var entity = o as IPersistEntity;
                if (entity != null)
                {
                    var h = entity.EntityLabel;
                    keys.Add(h); //normally there are only one or two keys so don't worry about performance of contains on a list
                }
                else if (o is IExpressEnumerable && o!=null) //null checking has been added for models that contain missing entities
                {
                    foreach (var obj in (IExpressEnumerable)o) 
                    {
                        if (obj != null)
                        {
                            var h = ((IPersistEntity)obj).EntityLabel;
                            keys.Add(h); //normally there are only one or two keys so don't worry about performance of contains on a list
                        }
                    }                    
                }
            }
            return keys;
        }

        public IEnumerable<PropertyInfo> IndexedProperties
        {
            get { return _indexedProperties ?? Enumerable.Empty<PropertyInfo>(); }
        }

        public IList<int> IndexedValues
        {
            get { return _indexedValues; }
        }

        private static void AddNonAbstractTypes(ExpressType expressType, ICollection<ExpressType> nonAbstractTypes)
        {
            if (!expressType.Type.GetTypeInfo().IsAbstract) //this is a concrete type so add it
                nonAbstractTypes.Add(expressType);
            foreach (var subType in expressType._subTypes)
                AddNonAbstractTypes(subType, nonAbstractTypes);
        }


        /// <summary>
        /// returns true if the attribute is indexed
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        public bool IsIndexedAttribute(int attributeIndex)
        {
            return IndexedValues != null && IndexedValues.Contains(attributeIndex);
        }

        /// <summary>
        /// Returns true if the type has an indexed attribute
        /// </summary>
        /// <returns></returns>
        public bool HasIndexedAttribute
        {
            get
            {
                return IndexedValues != null && _indexedValues.Count > 0;
            }
        }

        public string Name 
        {
            get
            {
                return Type.Name; 
            }
        }

        public List<ExpressType> SubTypes
        {
            get { return _subTypes; }
        }

        /// <summary>
        /// deep enumeration of all subtypes down in the inheritance hierarchy
        /// </summary>
        public IEnumerable<ExpressType> AllSubTypes
        {
            get
            {
                if(_subTypes == null)
                    yield break;
                foreach (var type in _subTypes)
                {
                    yield return type;
                    foreach (var subType in type.AllSubTypes)
                    {
                        yield return subType;
                    }
                }
            }
        }

        public Type UnderlyingType
        {
            get { return _underlyingType; }
        }

        public Type UnderlyingComplexType
        {
            get { return _underlyingComplexType; }
        }
    }

}
