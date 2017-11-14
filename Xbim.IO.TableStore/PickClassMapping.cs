using System.Xml.Serialization;

namespace Xbim.IO.TableStore
{
    public class PickClassMapping
    {
        /// <summary>
        /// Name of the class used as a pick value
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Class { get; set; }

        /// <summary>
        /// Column for pick values of this type
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Column { get; set; }

        /// <summary>
        /// Column header for pick values of this type
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Header { get; set; }

        /// <summary>
        /// Template will be used to assemble resulting pick-value string
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string ValueTemplate { get; set; }
    }
}
