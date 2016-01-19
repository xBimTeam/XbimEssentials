using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xbim.CobieExpress.IO.TableStore.TableMapping
{
    /// <summary>
    /// This class describes how to represent class as a table
    /// </summary>
    public class ClassMapping
    {
        /// <summary>
        /// Name of the applicable class for property mappings 
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string ClassName { get; set; }

        /// <summary>
        /// If TRUE this class mapping doesn't require any parent context
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool IsRoot { get; set; }

        /// <summary>
        /// Name of the target table
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string TableName { get; set; }

        /// <summary>
        /// Property mappings
        /// </summary>
        [XmlArray("PropertyMappings", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("PropertyMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<PropertyMapping> PropertyMappings { get; set; }
    }
}
