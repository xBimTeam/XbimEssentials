using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xbim.CobieExpress.IO.TableStore.TableMapping
{
    public class PropertyMapping
    {
        /// <summary>
        /// Name in the header of the column
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Header { get; set; }
        
        /// <summary>
        /// Column index [A-AZ]
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string ColumnIndex { get; set; }
        
        /// <summary>
        /// Colour of the column as hex value
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Colour { get; set; }

        /// <summary>
        /// If true but no value is found on any path, default value is used
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool Required { get; set; }

        /// <summary>
        /// If true but no value is found on any path, default value is used
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public MultiRowRepresentation MultiRow { get; set; }

        /// <summary>
        /// Default value to be used if this property is required but no value is available on any path
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Paths to the data to fill in the rows. Is the result contains IEnumerable of objects and the resulting
        /// string is londer than 255 characters the row will be multiplied. First path containing data will be used.
        /// Special variable 'parent' might be used to refere to the parent of this object
        /// </summary>
        [XmlArray("Paths", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("Path", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<string> Paths { get; set; }
    }

    public enum MultiRowRepresentation
    {
        None,
        IfNecessary,
        Always
    }
}
