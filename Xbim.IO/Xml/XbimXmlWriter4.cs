using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Step21;
using Xbim.IO.Xml.BsConf;

namespace Xbim.IO.Xml
{
    public class XbimXmlWriter4
    {
        #region Fields

        private const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private const string Xlink = "http://www.w3.org/1999/xlink";
        private readonly string _ns = "http://www.buildingsmart-tech.org/ifcXML/MVD4/IFC4";
        private readonly string _nsLocation = "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1/IFC4_ADD1.xsd";
        private readonly string _expressUri = "http://www.buildingsmart-tech.org/ifc/IFC4/Add1/IFC4_ADD1.exp";
        private readonly string _configurationUri = "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1/IFC4_ADD1_config.xml";
        private readonly string _rootElementName = "ifcXML";
        private HashSet<long> _written;
        private readonly configuration _conf;
        #endregion

        #region Properties

        public string Name;
        public string TimeStamp;
        public string Author;
        public string Organization;
        public string PreprocessorVersion;
        public string OriginatingSystem;
        public string Authorization;
        public string Documentation;

        private IStepFileHeader _fileHeader; // removed the initialiser because it's assigned from the model on write.

        #endregion

        /// <summary>
        /// Constructor of the XML writer. This writer implements conventions for IFC4 XML and needs to
        /// get configuration with speficic cases like inversed inverses and other special cases.
        /// Default settings and configuration are for IFC4 Add1. If you pass configuration and settings
        /// make sure both are for the same schema definition.
        /// </summary>
        /// <param name="configuration">XML to Express configuration</param>
        /// <param name="settings">Settings for writer like namespaces and root element name</param>
        public XbimXmlWriter4(configuration configuration = null, XbimXmlSettings settings = null)
        {
            _conf = configuration ?? configuration.IFC4Add1;
            TimeStamp = DateTime.Now.ToString("s");
            PreprocessorVersion = string.Format("Xbim File Processor version {0}", Assembly.GetAssembly(GetType()).GetName().Version);
            OriginatingSystem = string.Format("Xbim version {0}", Assembly.GetExecutingAssembly().GetName().Version);
            if (settings == null)
            return;

            _ns = settings.Namespace;
            _nsLocation = settings.NamespaceLocation;
            _expressUri = settings.ExpressUri;
            _configurationUri = settings.Configuration;
            _rootElementName = settings.RootName;
        }

        private ExpressMetaData _metadata;

        /// <summary>
        /// This function writes model entities to the defined XML output. 
        /// </summary>
        /// <param name="model">Model to be used for serialization. If no entities are specified IModel.Instances will be used as a 
        /// source of entities to be serialized.</param>
        /// <param name="output">Output XML</param>
        /// <param name="entities">Optional entities enumerable. If you define this enumerable it will be
        /// used instead of all entities from IModel.Instances. This allows to define different way of entities retrieval
        /// like volatile instances from persisted DB model</param>
        public void Write(IModel  model, XmlWriter output, IEnumerable<IPersistEntity> entities = null)
        {
            _metadata = model.Metadata;
            
            try
            {
                _written = new HashSet<long>();

                output.WriteStartDocument();
                output.WriteStartElement(_rootElementName, _ns);
                //xmlns declarations
                output.WriteAttributeString("xmlns", "xsi", null, Xsi);
                output.WriteAttributeString("xmlns", "xlink", null, Xlink);
                output.WriteAttributeString("xmlns", "stp", null, _ns);
                output.WriteAttributeString("schemaLocation", Xsi, string.Format("{0} {1}", _ns, _nsLocation));

                //attributes
                output.WriteAttributeString("id", "uos_1");
                output.WriteAttributeString("express", _expressUri);
                output.WriteAttributeString("configuration", _configurationUri);

                _fileHeader = model.Header;
                WriteHeader(output);
                

                //use specified entities enumeration or just all instances in the model
                if(entities != null)
                    foreach (var entity in entities)
                        Write(entity, output, true);
                else
                    foreach (var entity in model.Instances)
                        Write(entity, output, true);

                output.WriteEndElement(); //uos
                output.WriteEndDocument();
            }
            //catch (Exception e)
            //{
            //    throw new Exception("Failed to write IfcXml file", e);
            //}
            finally
            {
                _written = null;
            }
        }

        private void WriteHeader(XmlWriter output)
        {
            output.WriteStartElement("header");
            output.WriteElementString("name", _fileHeader.FileName.Name);
            output.WriteElementString("time_stamp", _fileHeader.FileName.TimeStamp);

            if (_fileHeader.FileName.AuthorName.Count > 0)
                output.WriteElementString("author", string.Join(", ", _fileHeader.FileName.AuthorName));
            if (_fileHeader.FileName.Organization.Any())
                output.WriteElementString("organization", string.Join(", ", _fileHeader.FileName.Organization));
            output.WriteElementString("preprocessor_version", _fileHeader.FileName.PreprocessorVersion);
            output.WriteElementString("originating_system", _fileHeader.FileName.OriginatingSystem);
            output.WriteElementString("authorization", _fileHeader.FileName.AuthorizationName);
            if (_fileHeader.FileDescription.Description.Any())
                output.WriteElementString("documentation", string.Join(", ", _fileHeader.FileDescription.Description));
            output.WriteEndElement(); //end iso_10303_28_header
        }

        private void Write(IPersistEntity entity, XmlWriter output, bool onlyOnce, int pos = -1, string name = null)
        {
            var exists = _written.Contains(entity.EntityLabel);
            if (exists && onlyOnce) //we have already done it and nothing should be written out
                return;

            if(!exists)
                _written.Add(entity.EntityLabel);

            var expressType = _metadata.ExpressType(entity);
            var elementName = name ?? expressType.ExpressName;

            output.WriteStartElement(elementName);
            output.WriteAttributeString(exists ? "ref" : "id", string.Format("i{0}", entity.EntityLabel));

            if (pos > -1) //we are writing out a list element
                output.WriteAttributeString("pos", pos.ToString());

            //specify type if it is different from the name of the element
            if(name != null && string.CompareOrdinal(name, expressType.ExpressName) != 0)
                output.WriteAttributeString("type", Xsi, expressType.ExpressName);

            //only write properties if this is the first occurence
            if(!exists)
                WriteProperties(entity, output, expressType);
            //specify this is an empty element refering to another one
            else
                output.WriteAttributeString("nil", Xsi, "true");

            
            output.WriteEndElement();
        }

        private readonly Dictionary<Type, List<XmlMetaProperty>> _propertiesCache 
            = new Dictionary<Type, List<XmlMetaProperty>>(); 

        private void WriteProperties(IPersistEntity entity, XmlWriter output, ExpressType expressType)
        {
            List<XmlMetaProperty> properties;
            if (!_propertiesCache.TryGetValue(expressType.Type, out properties))
            {
                properties = XmlMetaProperty.GetProperties(expressType, _conf);
                //cache
                _propertiesCache.Add(expressType.Type, properties);
            }
            

            foreach (var property in properties) //only write out persistent attributes, ignore inverses
            {
                WriteProperty(property, entity, output, -1);
            }
        }

        private void WriteProperty(string propName, Type propType, object propVal, IPersistEntity entity, XmlWriter output,
            int pos, EntityAttributeAttribute attr, bool wrap = false)
        {
            //don't write anything if this is uninitialized optional item set
            var optSet = propVal as IOptionalItemSet;
            if (optSet != null && !optSet.Initialized)
                return;

            //null or a value type that maybe null, need to write out sets and lists if they are mandatroy but empty
            if (propVal == null)
            {
                if (!typeof(IExpressEnumerable).IsAssignableFrom(propType) ||
                    attr.State != EntityAttributeState.Mandatory) return;

                //write out empty mandatory set with proper enumeration type
                output.WriteStartElement(propName);
                output.WriteAttributeString("cType", attr.ListType);
                output.WriteEndElement();
                return;
            }

            if (typeof(IExpressSelectType).IsAssignableFrom(propType) && propType.IsInterface)
            // a select type get the type of the actual value
            {
                var realType = propVal.GetType();
                var exprType = _metadata.ExpressType(realType);
                var realName = exprType != null ? exprType.ExpressName : realType.Name;
                output.WriteStartElement(propName);
                WriteProperty(realName, realType, propVal, entity, output, -1, attr, true);
                output.WriteEndElement();
                return;
            }

            //make sure we don't mess around with nullables
            propType = XmlMetaProperty.GetNonNullableType(propType);
            if (typeof(IExpressValueType).IsAssignableFrom(propType))
            {
                var cpl = propVal as IExpressComplexType;
                var valString = cpl == null
                    ? propVal.ToString()
                    : string.Join(" ", cpl.Properties);

                output.WriteStartElement(propName + (wrap ? "-wrapper" : ""));
                if (pos > -1)
                    output.WriteAttributeString("pos", pos.ToString());
                output.WriteValue(valString);
                output.WriteEndElement();
                return;
            }

            if (typeof(IExpressEnumerable).IsAssignableFrom(propType))
            {
                var generic = propType.GetGenericArguments().FirstOrDefault();
                if (generic != null && generic.IsValueType && !XmlMetaProperty.IsStringCompatible(generic))
                {
                    output.WriteAttributeString(propName, string.Join(" ", ((IEnumerable)propVal).Cast<object>()));
                    return;
                }

                //special case for IfcRelDefinesByProperties
                //if (propName == "RelatedObjects" && entity.ExpressType.Name == "IfcRelDefinesByProperties")
                if (attr.MaxCardinality == 1) //list which shouldn't have more than 1 element in
                {
                    propVal = ((IEnumerable)propVal).Cast<object>().FirstOrDefault();
                    var relEntity = propVal as IPersistEntity;
                    if (relEntity == null) return;
                    Write(relEntity, output, false, -1, propName);
                    return;
                }

                output.WriteStartElement(propName);
                output.WriteAttributeString("stp", "cType", _ns, attr.ListType);
                var i = 0;
                var isSelect = typeof(IExpressSelectType).IsAssignableFrom(propType.GetGenericArguments().FirstOrDefault());
                foreach (var item in (IExpressEnumerable)propVal)
                {
                    WriteProperty(item.GetType().Name, item.GetType(), item, entity, output, i, attr, isSelect);
                    i++;
                }
                output.WriteEndElement();
                return;
            }
            if (typeof(IPersistEntity).IsAssignableFrom(propType))
            //all writable entities must support this interface and ExpressType have been handled so only entities left
            {
                var persistEntity = (IPersistEntity)propVal;
                Write(persistEntity, output, false, pos, propName);
                return;

            }
            if (typeof(IExpressComplexType).IsAssignableFrom(propType)) //it is a complex value tpye
            {
                var properties = ((IExpressComplexType)propVal).Properties;
                output.WriteAttributeString(propName, string.Join(" ", properties));
                return;
            }
            if (propType.IsValueType || typeof(string) == propType) //it might be an in-built value type double, string etc
            {
                var pInfoType = propVal.GetType();

                if (pos < 0)
                    output.WriteStartAttribute(propName);

                if (pInfoType.IsEnum) //convert enum
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement(propName);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    output.WriteValue(propVal.ToString().ToLower());
                }
                else if (pInfoType.UnderlyingSystemType == typeof(Boolean))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("boolean-wrapper");
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    output.WriteValue((bool)propVal ? "true" : "false");
                }
                else if (pInfoType.UnderlyingSystemType == typeof(Double))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("double-wrapper");
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    output.WriteValue(string.Format(new Part21Formatter(), "{0:R}", propVal));
                }
                else if (pInfoType.UnderlyingSystemType == typeof(Int16))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("integer-wrapper");
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    output.WriteValue(propVal.ToString());
                }
                else if (pInfoType.UnderlyingSystemType == typeof(Int32) ||
                         pInfoType.UnderlyingSystemType == typeof(Int64))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("long-wrapper");
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    output.WriteValue(propVal.ToString());
                }
                else if (pInfoType.UnderlyingSystemType == typeof(String)) //convert  string
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("string-wrapper");
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    output.WriteValue(string.Format(new Part21Formatter(), "{0}", propVal));
                }

                else
                    throw new NotSupportedException(string.Format("Invalid Value Type {0}", pInfoType.Name));

                if (pos > -1)
                    output.WriteEndElement();
                else
                    output.WriteEndAttribute();

                return;
            }
            
        }

        private void WriteProperty(XmlMetaProperty property, IPersistEntity entity, XmlWriter output,
                                   int pos, bool wrap = false)
        {
            var propVal = property.MetaProperty.PropertyInfo.GetValue(entity, null);
            var propType = property.MetaProperty.PropertyInfo.PropertyType;
            var propName = property.MetaProperty.PropertyInfo.Name;
            var attr = property.MetaProperty.EntityAttribute;

            //handling of XML attributes is baked before
            if (property.IsAttributeValue)
            {
                property.AttributeSetter(propVal, output);
                return;
            }

            WriteProperty(propName, propType, propVal, entity, output, pos, attr, wrap);
        }
    }
}