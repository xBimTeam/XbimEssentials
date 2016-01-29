using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    /// <summary>
    /// This class describes how to represent class as a table
    /// </summary>
    public class ClassMapping
    {

        public ClassMapping()
        {
            PropertyMappings = new List<PropertyMapping>();
        }

        private ModelMapping _modelMapping;

        private ExpressMetaData Metadata
        {
            get
            {
                return _modelMapping != null ?
                    _modelMapping.MetaData :
                    null;
            }
        }

        internal void Init(ModelMapping mapping)
        {
            _modelMapping = mapping;
        }

        /// <summary>
        /// Name of the applicable class for property mappings 
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Class { get; set; }

        /// <summary>
        /// If TRUE this class mapping doesn't require any parent context
        /// </summary>
        [XmlIgnore]
        public bool IsRoot { get { return string.IsNullOrWhiteSpace(ParentClass); }}

        /// <summary>
        /// Name of the target table
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string TableName { get; set; }

        /// <summary>
        /// Name of the target table
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public int TableOrder { get; set; }

        /// <summary>
        /// Status of the target table
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public DataStatus TableStatus { get; set; }

        /// <summary>
        /// Name of the parent class used for serialization as a key or part of the key 
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string ParentClass { get; set; }

        /// <summary>
        /// Path in parent class used get children instances
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string ParentPath { get; set; }

        /// <summary>
        /// Type to be used for deserialization in case that the 'Class' is an abstract type and there is no [type] hint
        /// in a property mappings
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string FallBackConcreteType { get; set; }

        /// <summary>
        /// If class mapping is partial it won't be used to create new objects when the table is loaded
        /// into object model. It will only be used to add additional information or references instead
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool IsPartial { get; set; }

        /// <summary>
        /// Property mappings
        /// </summary>
        [XmlArray("PropertyMappings", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("PropertyMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<PropertyMapping> PropertyMappings { get; set; }
        
        /// <summary>
        /// Children mappings where this mapping acts as a parent.
        /// Only call this after ModelMappings are initialized.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<ClassMapping> ChildrenMappings
        {
            get
            {
                if(_modelMapping == null || _modelMapping.ClassMappings == null)
                    yield break;

                var type = Type;
                foreach (var mapping in _modelMapping.ClassMappings)
                {
                    if (mapping.IsRoot) continue;

                    var mType = mapping.ParentType;
                    if (mType == type)
                    {
                        yield return mapping;
                        continue;
                    }
                    if (mType.SubTypes != null && mType.SubTypes.Contains(type))
                    {
                        yield return mapping;
                    }
                }
            }
        }

        /// <summary>
        /// This will return all instances which are usable for this class mapping from parent
        /// defined in this class mapping. It will use the path to get all instances. This might
        /// be hierarchical search which will be represented in a context. Every segment of the
        /// path might also be explicitly typed which will be used to filter instances on every level
        /// of search. Types can be defined using backslash '\' operator in the path.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public EntityContext GetContext(IPersistEntity parent)
        {
            var parts = ParentPath.Split('.');
            var root = new EntityContext(parent);
            var type = parent.ExpressType;
            foreach (var name in parts)
            {
                var propName = name;
                var typeName = type.Name;
                var ofType = propName.Contains("\\");
                string ofTypeName = null;
                if (ofType)
                {
                    var match = new Regex("(?<property>.+)\\\\(?<type>.+)").Match(propName);
                    propName = match.Groups["property"].Value;
                    ofTypeName = match.Groups["type"].Value;    
                }
                
                var metaProperty =
                    type.Properties.Values.FirstOrDefault(p => p.Name == propName) ??
                    type.Inverses.FirstOrDefault(p => p.Name == propName) ??
                    type.Derives.FirstOrDefault(p => p.Name == propName);
                if (metaProperty == null)
                    throw new XbimException(string.Format("Property {0} is not defined in type {1}", propName, typeName));

                var info = metaProperty.PropertyInfo;
                var leaves = root.Leaves.ToList();
                root.LeavesDepth++;
                foreach (var context in leaves)
                {
                    var value = info.GetValue(context.Entity, null);
                    if (value == null) continue;

                    //it might be a single object
                    var entity = value as IPersistEntity;
                    if (entity == null)
                    {
                        var entVal = value as IExpressValueType;
                        if (entVal != null && entVal.Value is IPersistEntity)
                            entity = (IPersistEntity)entVal.Value;
                    }
                    if (entity != null)
                    {
                        if (ofType)
                        {
                            type = _modelMapping.MetaData.ExpressType(ofTypeName.ToUpper());
                            var instType = entity.ExpressType;
                            //only add the instance for further processing if it is of desired type
                            if(type == instType || type.AllSubTypes.Contains(instType))
                                context.Add(entity);
                        }
                        else
                        {
                            context.Add(entity);
                            type = entity.ExpressType;    
                        }
                        continue;
                    }

                    var entityEnum = value as IEnumerable<IPersistEntity>;
                    if (entityEnum == null)
                    {
                        var entVal = value as IExpressValueType;
                        if (entVal != null && entVal.Value is IEnumerable<IPersistEntity>)
                            entityEnum = (IEnumerable<IPersistEntity>)entVal.Value;
                    }
                    if (entityEnum != null)
                    {
                        if (ofType)
                        {
                            type = _modelMapping.MetaData.ExpressType(ofTypeName.ToUpper());
                            if(type == null)
                                throw new XbimException(string.Format("{0} is not defined IPersistEntity type", ofTypeName));

                            var mInfo = entityEnum.GetType().GetMethod("OfType");
                            if (mInfo != null)
                            {
                                mInfo = mInfo.MakeGenericMethod(type.Type);
                                var subEnum = mInfo.Invoke(entityEnum, null) as IEnumerable<IPersistEntity>;
                                if (subEnum != null)
                                    entityEnum = subEnum;
                                else
                                    throw new XbimException("OfType method didn't work");
                            }
                        }
                        else
                        {
                            var enumType = entityEnum.GetType().GetGenericArguments()[0];
                            type = _modelMapping.MetaData.ExpressType(enumType);
                        }
                        context.Add(entityEnum);
                    }
                }
            }
            return root;
        }

        //cache for the type once retrieved
        private ExpressType _type;
        /// <summary>
        /// ExpressType representing for this class mapping
        /// </summary>
        [XmlIgnore]
        public ExpressType Type
        {
            get {
                return _type ?? (_type = Metadata != null ?
                Metadata.ExpressType(Class.ToUpper()) :
                null)
                ; 
            }
        }

        //cache for the type once retrieved
        private ExpressType _parentType;
        /// <summary>
        /// ExpressType of parent class for this class mapping
        /// </summary>
        [XmlIgnore]
        public ExpressType ParentType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ParentClass)) return null;
                return _parentType ?? (_parentType = Metadata != null ?
                Metadata.ExpressType(ParentClass.ToUpper()) :
                null)
                ;
            }
        }
    }
}
