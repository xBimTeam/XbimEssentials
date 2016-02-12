using System.Xml.Serialization;

namespace Xbim.IO.TableStore
{
    public class ClassScope
    {
        /// <summary>
        /// Name of the class
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Class { get; set; }

        /// <summary>
        /// Scope of the class
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public ClassScopeEnum Scope { get; set; }

    }

    public enum ClassScopeEnum
    {
        Local,
        Model
    }
}
