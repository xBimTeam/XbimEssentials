using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    /// <summary>
    /// This class describes how to map selected data from the model to table representation
    /// </summary>
    [XmlRoot("ModelMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
    public class ModelMapping
    {
        [XmlIgnore]
        public ExpressMetaData MetaData { get; private set; }

        /// <summary>
        /// Name of this mapping
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string Name { get; set; }

        /// <summary>
        /// String to be used to separate lists of values if enumeration of values is to be stored in a single cell
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string ListSeparator { get; set; }

        /// <summary>
        /// Name of the table where pick values should be stored
        /// </summary>
        [XmlAttribute(Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public string PickTableName { get; set; }

        /// <summary>
        /// Mappings for classes in the model
        /// </summary>
        [XmlArray("StatusRepresentations", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("StatusRepresentation", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<StatusRepresentation> StatusRepresentations { get; set; }

        /// <summary>
        /// Mappings for classes in the model
        /// </summary>
        [XmlArray("ClassMappings", Namespace = "http://www.openbim.org/mapping/table/1.0"), 
        XmlArrayItem("ClassMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<ClassMapping> ClassMappings { get; set; }

        /// <summary>
        /// Mappings for pick classes in the model (used as the enumerations)
        /// </summary>
        [XmlArray("PickClassMappings", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("PickClassMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<PickClassMapping> PickClassMappings { get; set; }

        /// <summary>
        /// Mappings for enumeration members. This allows localization of enumerations.
        /// </summary>
        [XmlArray("EnumerationMappings", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("EnumerationMapping", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<EnumMapping> EnumerationMappings { get; set; }

        /// <summary>
        /// Scopes of classes. If no scope is defined it defaults to 'Local'. This is only important
        /// for import of tabular data where this helpes to decide wheather to create new object
        /// as a local to its parent or if the objec should be looked up in the model
        /// </summary>
        [XmlArray("Scopes", Namespace = "http://www.openbim.org/mapping/table/1.0"),
        XmlArrayItem("Scope", Namespace = "http://www.openbim.org/mapping/table/1.0")]
        public List<ClassScope> Scopes { get; set; }

        #region Serialization
        public ModelMapping()
        {
            //init all lists as all are supposed to contain some data
            StatusRepresentations = new List<StatusRepresentation>();
            ClassMappings = new List<ClassMapping>();
            PickClassMappings = new List<PickClassMapping>();
            EnumerationMappings = new List<EnumMapping>();
            ListSeparator = ",";
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

        public void Init(ExpressMetaData metadata)
        {
            MetaData = metadata;
            if (ClassMappings == null) return;
            foreach (var classMapping in ClassMappings)
            {
                classMapping.Init(this);
            }
        }
    }
}
