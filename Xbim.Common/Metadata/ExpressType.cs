using System;
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
        private readonly List<ExpressType> _subTypes = new List<ExpressType>();
        private List<Type> _nonAbstractSubTypes;
        private readonly List<ExpressMetaProperty> _expressEnumerableProperties = new List<ExpressMetaProperty>();
        private readonly string _expressName;
        private readonly short _typeId;
        private readonly List<PropertyInfo> _indexedProperties;
        private readonly List<int> _indexedValues;

        #endregion

        #region Properties

        public ExpressType SuperType { get; internal set; }

        public bool IndexedClass { get; private set; }

        public string ExpressName
        {
            get { return _expressName; }
        }

        public short TypeId
        {
            get { return _typeId; }
        }

        public IEnumerable<ExpressMetaProperty> Inverses
        {
            get { return _inverses; }
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
        public IEnumerable<Type> NonAbstractSubTypes
        {
            get
            {
                lock (this)
                {
                    //this needs to be set up after hierarchy is set up
                    if (_nonAbstractSubTypes != null) return _nonAbstractSubTypes;
                    _nonAbstractSubTypes = new List<Type>();
                    AddNonAbstractTypes(this, _nonAbstractSubTypes);
                }
                return _nonAbstractSubTypes;
            }
        }

        #endregion

        public ExpressType(Type type)
        {
            Type = type;
            var entNameAttr = Type.GetCustomAttributes(typeof(ExpressTypeAttribute), false).FirstOrDefault();
#if DEBUG
            if (entNameAttr == null)
                throw new Exception("Express Type is not defined for " + Type.Name);
#endif
            _typeId = (short)((ExpressTypeAttribute)entNameAttr).EntityTypeId;
            _expressName = ((ExpressTypeAttribute)entNameAttr).Name;

            IndexedClass = type.GetCustomAttributes(typeof(IndexedClass), true).Any();

            

            var properties =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var propInfo in properties)
            {
                var attributeIdx = -1;
                var attribute =
                    ((EntityAttributeAttribute[])propInfo.GetCustomAttributes(typeof(EntityAttributeAttribute), false)).FirstOrDefault();
                if(attribute == null)
                    continue;

                if (attribute.Order > 0)
                {
                    // SUPPORT: if the code breaks here there's a problem with the order attribut in a class property
                    _properties.Add(attribute.Order,
                                                new ExpressMetaProperty { PropertyInfo = propInfo, EntityAttribute = attribute });
                    attributeIdx = attribute.Order;
                }
                else
                    _inverses.Add(new ExpressMetaProperty { PropertyInfo = propInfo, EntityAttribute = attribute });
               
                var isIndexed =
                    propInfo.GetCustomAttributes(typeof(IndexedProperty), false).Any();
                if (!isIndexed) continue;

                //TODO: MC: Review with Steve. This is not true for IfcRelDefinesByProperties.RelatingPropertyDefinition in IFC4
                //Debug.Assert(typeof(IPersistEntity).IsAssignableFrom(propInfo.PropertyType)
                //    || typeof(IEnumerable<IPersistEntity>).IsAssignableFrom(propInfo.PropertyType)); //only handles to IPersistEntitiess or collecctions of IPersistEntities are indexable
                    
                if (_indexedProperties == null) _indexedProperties = new List<PropertyInfo>();
                if (_indexedValues == null) _indexedValues = new List<int>();
                _indexedProperties.Add(propInfo);
                _indexedValues.Add(attributeIdx);
                IndexedClass = true; //if it has keys it must be an indexed class
            }

            //cache enumerable properties
            foreach (var prop in _properties.Values.Where(prop => typeof(IExpressEnumerable).IsAssignableFrom(prop.PropertyInfo.PropertyType)))
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
        internal IEnumerable<int> GetIndexedValues(IPersistEntity ent)
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
                else if (o is IExpressEnumerable)
                {
                    foreach (var obj in (IExpressEnumerable)o)
                    {
                        var h =((IPersistEntity)obj).EntityLabel;
                        keys.Add(h); //normally there are only one or two keys so don't worry about performance of contains on a list
                    }                    
                }
                //TODO: MC: This won't be true for IfcRelDefinesByProperties.RelatingPropertyDefinition where 'o' might be only IPersist (IfcPropertySetDefinitionSet is a value type)

            }
            return keys;
        }

        internal IEnumerable<PropertyInfo> IndexedProperties
        {
            get { return _indexedProperties ?? Enumerable.Empty<PropertyInfo>(); }
        }

        internal IList<int> IndexedValues
        {
            get { return _indexedValues; }
        }

        private static void AddNonAbstractTypes(ExpressType expressType, ICollection<Type> nonAbstractTypes)
        {
            if (!expressType.Type.IsAbstract) //this is a concrete type so add it
                nonAbstractTypes.Add(expressType.Type);
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
    }

}
