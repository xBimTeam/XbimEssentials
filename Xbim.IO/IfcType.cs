using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3;

namespace Xbim.IO
{
    public class IfcType
    {
        public Type Type;
        public short TypeId;
        public SortedList<int, IfcMetaProperty> IfcProperties = new SortedList<int, IfcMetaProperty>();
        public List<IfcMetaProperty> IfcInverses = new List<IfcMetaProperty>();
        public IfcType IfcSuperType;
        public List<IfcType> IfcSubTypes = new List<IfcType>();
        private List<Type> _nonAbstractSubTypes;
        private List<PropertyInfo> _indexedProperties;
        private List<IfcMetaProperty> _expressEnumerableProperties;
        private List<int> _indexedValues;
        internal bool IndexedClass = false;
        public List<IfcMetaProperty> ExpressEnumerableProperties
        {
            get
            {
                if (_expressEnumerableProperties == null)
                {
                    _expressEnumerableProperties = new List<IfcMetaProperty>();
                    foreach (IfcMetaProperty prop in IfcProperties.Values)
                    {
                        if (typeof(ExpressEnumerable).IsAssignableFrom(prop.PropertyInfo.PropertyType))
                            _expressEnumerableProperties.Add(prop);
                    }
                }
                return _expressEnumerableProperties;
            }
        }

        public override string ToString()
        {
            return Type.Name;
        }

        public IfcEntityNameEnum IfcTypeEnum
        {
            get
            {
                return (IfcEntityNameEnum) Enum.Parse(typeof(IfcEntityNameEnum), Type.Name, true);
            }
        }

        public IList<Type> NonAbstractSubTypes
        {
            get
            {
                lock (this)
                {
                    if (_nonAbstractSubTypes == null)
                    {
                        _nonAbstractSubTypes = new List<Type>();
                        AddNonAbstractTypes(this, _nonAbstractSubTypes);
                    }
                    return _nonAbstractSubTypes;
                }
               
            }
        }
        /// <summary>
        /// If the type has indexed attributes, this returns a set of unique values for the specified IPersistIfcEntity
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        internal IEnumerable<int> GetIndexedValues(IPersistIfcEntity ent)
        {
            if (IndexedProperties == null)
                return Enumerable.Empty<int>();
            HashSet<int> keys = new HashSet<int>();
            foreach (var prop in IndexedProperties)
            {
                object o = prop.GetValue(ent, null);
                if (null!=o && typeof(IPersistIfcEntity).IsAssignableFrom(o.GetType()))
                {
                    int h = ((IPersistIfcEntity)o).EntityLabel;
                    keys.Add(h); //normally there are only one or two keys so don't worry about performance of contains on a list
                }
                else if (null != o && typeof(ExpressEnumerable).IsAssignableFrom(o.GetType()))
                {
                    foreach (var obj in (ExpressEnumerable)o)
                    {
                        int h =((IPersistIfcEntity)obj).EntityLabel;
                        keys.Add(h); //normally there are only one or two keys so don't worry about performance of contains on a list
                    }                    
                }
            }
            return keys;
        }

        internal List<PropertyInfo> IndexedProperties
        {
            get
            {
                return _indexedProperties;
            }

        }

        internal List<int> IndexedValues
        {
            get
            {
                return _indexedValues;
            }
        }

        private void AddNonAbstractTypes(IfcType ifcType, List<Type> nonAbstractTypes)
        {
            if (!ifcType.Type.IsAbstract) //this is a concrete type so add it
                nonAbstractTypes.Add(ifcType.Type);
            foreach (IfcType subType in ifcType.IfcSubTypes)
                AddNonAbstractTypes(subType, nonAbstractTypes);
        }


        /// <summary>
        /// returns true if the attribute is indexed
        /// </summary>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        public bool IsIndexedIfcAttribute(int attributeIndex)
        {
            return _indexedValues != null && _indexedValues.Contains(attributeIndex);
        }

        internal void AddIndexedAttribute(PropertyInfo pInfo, int attributeIdx)
        {
            if (_indexedProperties == null) _indexedProperties = new List<PropertyInfo>();
            if (_indexedValues == null) _indexedValues = new List<int>();
            _indexedProperties.Add(pInfo);
            _indexedValues.Add(attributeIdx);
            IndexedClass = true; //if it has keys it must be an indexed class
        }
        /// <summary>
        /// Returns true if the type has an indexed attribute
        /// </summary>
        /// <returns></returns>
        public bool HasIndexedAttribute
        {
            get
            {
                return _indexedValues != null && _indexedValues.Count > 0;
            }
        }

        public string Name 
        {
            get
            {
                return Type.Name; 
            }
        }
    }

}
