using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Xbim.CobieExpress.IO.TableStore.TableMapping
{
    /// <summary>
    /// This class describes how to map selected data from the model to table representation
    /// </summary>
    [XmlRoot("ModelMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
    public class ModelMapping
    {
        /// <summary>
        /// Name of this mapping
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Name { get; set; }
        
        /// <summary>
        /// Mappings for classes in the model
        /// </summary>
        [XmlArray("ClassMappings", Namespace = "http://www.openbim.org/mapping/table/1.0"), 
        XmlArrayItem("ClassMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<ClassMapping> ClassMappings { get; set; }

        #region Serialization
        public ModelMapping()
        {
            Namespaces.Add("map", "http://www.openbim.org/mapping/table/1.0");
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces();

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ModelMapping));

        public static ModelMapping Load(string data)
        {
            return Serializer.Deserialize(new StringReader(data)) as ModelMapping;
        }

        public static ModelMapping Load(Stream input)
        {
            return Serializer.Deserialize(input) as ModelMapping;
        }

        public void Save(Stream output)
        {
            Serializer.Serialize(output, this);
        }

        /// <summary>
        /// Returns this object serialized as XML string
        /// </summary>
        /// <returns></returns>
        public string AsXMLString()
        {
            var writer = new StringWriter();
            Serializer.Serialize(writer, this);
            return writer.ToString();
        }
        #endregion
    }
}
