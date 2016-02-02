using System.Xml.Serialization;

namespace Xbim.IO.TableStore
{
    public class EnumNameAlias
    {
        /// <summary>
        /// Name of the enumeration member
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string EnumMember { get; set; }

        /// <summary>
        /// Alias of the enumeration member
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Alias { get; set; }
    }
}
