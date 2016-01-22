using System.Xml.Serialization;

namespace Xbim.CobieExpress.IO.TableStore.TableMapping
{
    public class StatusRepresentation
    {
        /// <summary>
        /// Representation colour for data with the defined status
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Colour { get; set; }

        /// <summary>
        /// Weight of the font
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// If TRUE thin solid black border will be created.
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public bool Border { get; set; }

        /// <summary>
        /// Status for which this representation is defined
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public DataStatus Status { get; set; }
    }

    public enum FontWeight
    {
        Normal,
        Bold,
        Italics,
        BoldItalics
    }
}
