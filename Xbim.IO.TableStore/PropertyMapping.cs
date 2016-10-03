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
        /// Cached index to be used so that letter doesn't have to be converted to the index all the time
        /// </summary>
        [XmlIgnore]
        internal int ColumnIndex { get; set; }

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
        /// If TRUE the column of this property will be hidden in the initial state
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool Hidden { get; set; }

        /// <summary>
        /// List of paths to search for a value. First path containing data will be used.
        /// Special variable 'parent' might be used to refer to the parent of this object.
        /// Special attribute .[table] might be used to refer to the table where parent object is stored.
        /// Special attribute .[type] might be used to refer to the Express type of the object.
        /// Special variable '()' might be used to refer to object higher in the context of search path within the parent object.
        /// </summary>
        [XmlAttribute("Paths", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        // ReSharper disable once InconsistentNaming
        public string _Paths
        {
            get { return _paths; }
            set
            {
                _paths = value;
                //empty cache
                _pathsEnumerationCache = null;
            }
        }

        /// <summary>
        /// If TRUE the column of this property will be considered to be a key for deserialization
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool IsKey { get; set; }

        /// <summary>
        /// If TRUE the column will be used to recognize multi-row records
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool IsMultiRowIdentity { get; set; }

        /// <summary>
        /// Allowed Data Type of the column
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public AllowedType DataType { get; set; }

        private List<string> _pathsEnumerationCache;
        private string _paths;

        /// <summary>
        /// Preprocessed list of paths where value might be found.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<string> Paths {
            get
            {
                return _pathsEnumerationCache ?? (_pathsEnumerationCache =  string.IsNullOrWhiteSpace(_Paths) ? 
                    new List<string>() : 
                    _Paths.Split(',').Select(p => p.Trim(' ')).ToList());
            }
        }
    }

    public enum MultiRow
    {
        None,
        IfNecessary,
        Always
    }

    /// <summary>
    /// Allowed Types to allow validation of fields to correct format
    /// </summary>
    public enum AllowedType
    {
        Text,
        AlphaNumeric,
        Email,
        Numeric,
        ISODate,
        ISODateTime,
        AnyType
    }
}
