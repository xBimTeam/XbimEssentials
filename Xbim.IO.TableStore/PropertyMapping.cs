using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Xbim.IO.TableStore
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
        public string Column { get; set; }
        
        /// <summary>
        /// Status of the column
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public DataStatus Status { get; set; }

        /// <summary>
        /// If true but no value is found on any path, default value is used
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public MultiRow MultiRow { get; set; }

        /// <summary>
        /// Default value to be used if this property is required but no value is available on any path
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// List of paths to search for a value. First path containing data will be used.
        /// Special variable 'parent' might be used to refere to the parent of this object
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Paths { get; set; }

        /// <summary>
        /// Preprocessed list of paths where value might be found.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<string> PathsEnumeration {
            get
            {
                return string.IsNullOrWhiteSpace(Paths) ? 
                    Enumerable.Empty<string>() : 
                    Paths.Split(',').Select(p => p.Trim(' '));
            }
        }
    }

    public enum MultiRow
    {
        None,
        IfNecessary,
        Always
    }
}
