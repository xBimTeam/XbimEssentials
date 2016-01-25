using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;

namespace Xbim.CobieExpress.IO.TableStore.TableMapping
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

        public IEnumerable<IPersistEntity> GetInstances(IPersistEntity parent)
        {
            var propName = ParentPath;
            var expType = parent.ExpressType;
            var metaProperty =
                expType.Properties.Values.FirstOrDefault(p => p.Name == propName) ??
                expType.Inverses.FirstOrDefault(p => p.Name == propName) ??
                expType.Derives.FirstOrDefault(p => p.Name == propName);
            if(metaProperty == null)
                throw new XbimException("It wasn't possible to find desired property in the object");

            var propInfo = metaProperty.PropertyInfo;
            var resultObject = propInfo.GetValue(parent, null);

            //it might be a single object
            var resultEntity = resultObject as IPersistEntity;
            if(resultEntity != null)
                return new[]{resultEntity};

            return resultObject as IEnumerable<IPersistEntity>;
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
