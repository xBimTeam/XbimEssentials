using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xbim.IO.TableStore
{
    public class EnumMapping
    {
        /// <summary>
        /// Name of the enumeration type 
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Enumeration { get; set; }

        /// <summary>
        /// Name of the enumeration type 
        /// </summary>
        [XmlArray(Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("Alias", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<EnumNameAlias> Aliases { get; set; }

    }
}
