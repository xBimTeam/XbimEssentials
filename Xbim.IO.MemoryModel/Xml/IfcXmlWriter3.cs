#region Directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Step21;

#endregion

namespace Xbim.IO.Xml
{
    public class IfcXmlWriter3
    {
        #region Fields

        private const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private const string Xlink = "http://www.w3.org/1999/xlink";
        private const string Namespace = "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL";
        private const string IfcXsd = "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL/IFC2X3.xsd";
        private const string Iso10303Urn = "urn:iso.org:standard:10303:part(28):version(2):xmlschema:common";
        private const string ExXsd = "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL/ex.xsd";
        private HashSet<long> _written;

        public bool WriteInverses;

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

        public IfcXmlWriter3()
        {
            var xBimVersion = GetType().Assembly.GetName().Version;
            var now = DateTime.Now;
            TimeStamp = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", now.Year, now.Month, now.Day,
                                      now.Hour, now.Minute, now.Second);
            PreprocessorVersion = $"Xbim.Ifc File Processor version {xBimVersion}";
            OriginatingSystem = $"Xbim version {xBimVersion}";
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
                output.WriteStartElement("ex", "iso_10303_28", Iso10303Urn);
                output.WriteAttributeString("version", "2.0");
                output.WriteAttributeString("xmlns", "xsi", null, Xsi);
                output.WriteAttributeString("xmlns", "xlink", null, Xlink);
                output.WriteAttributeString("xmlns", "ex", null, Iso10303Urn);
                output.WriteAttributeString("xsi", "schemaLocation", null,
                                            string.Format("{0} {1}", Iso10303Urn, ExXsd));

                _fileHeader = model.Header;
                WriteISOHeader(output);
                //Write out the uos
                output.WriteStartElement("uos", Namespace);
                output.WriteAttributeString("id", "uos_1");
                output.WriteAttributeString("description", "Xbim IfcXml Export");
                output.WriteAttributeString("configuration", "i_ifc2x3");
                output.WriteAttributeString("edo", "");
                output.WriteAttributeString("xmlns", "ex", null, Iso10303Urn);
                output.WriteAttributeString("xmlns", "ifc", null, Namespace);
                output.WriteAttributeString("xsi", "schemaLocation", null, string.Format("{0} {1}", Namespace, IfcXsd));

                //use specified entities enumeration or just all instances in the model
                if(entities != null)
                    foreach (var entity in entities)
                        Write(entity, output);
                else
                    foreach (var entity in model.Instances)
                        Write(entity, output);

                output.WriteEndElement(); //uos
                output.WriteEndElement(); //iso_10303_28
                output.WriteEndDocument();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to write IfcXml file", e);
            }
            finally
            {
                _written = null;
            }
        }

        private void WriteISOHeader(XmlWriter output)
        {
            output.WriteStartElement("ex", "iso_10303_28_header", null);
            WriteInHeader(output,"ex", "name", null, _fileHeader.FileName.Name);
            WriteInHeader(output,"ex", "time_stamp", null, _fileHeader.FileName.TimeStamp);

            if (_fileHeader.FileName.AuthorName.Count > 0)
                foreach (var name in _fileHeader.FileName.AuthorName)
                    WriteInHeader(output,"ex", "author", null, name);
            else
                WriteInHeader(output,"ex", "author", null, "");

            if (_fileHeader.FileName.Organization.Count > 0)
                foreach (var name in _fileHeader.FileName.Organization)
                    WriteInHeader(output,"ex", "organization", null, name);
            else
                WriteInHeader(output,"ex", "organization", null, "");

            WriteInHeader(output,"ex", "preprocessor_version", null, _fileHeader.FileName.PreprocessorVersion);
            WriteInHeader(output,"ex", "originating_system", null, _fileHeader.FileName.OriginatingSystem);
            WriteInHeader(output,"ex", "authorization", null, _fileHeader.FileName.AuthorizationName);

            if (_fileHeader.FileDescription.Description.Count > 0)
                foreach (var name in _fileHeader.FileDescription.Description)
                    WriteInHeader(output,"ex", "documentation", null, name);
            else
                WriteInHeader(output,"ex", "documentation", null, "");

            output.WriteEndElement(); //end iso_10303_28_header
        }

        private void WriteInHeader(XmlWriter output, string prefix, string localName, string ns, string value)
        {
            try
            {
                output.WriteElementString(prefix, localName, ns, value);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to write the property called '{localName}' in the header section.", e);
            }
        }

        private void Write(IPersistEntity entity, XmlWriter output, int pos = -1)
        {

            if (_written.Contains(entity.EntityLabel)) //we have already done it
                return;
            _written.Add(entity.EntityLabel);

            var expressType = _metadata.ExpressType(entity);

            output.WriteStartElement(expressType.Type.Name);

            output.WriteAttributeString("id", string.Format("i{0}", entity.EntityLabel));
            if (pos > -1) //we are writing out a list element
                output.WriteAttributeString("pos", pos.ToString());
            
            IEnumerable<ExpressMetaProperty> toWrite;
            if (WriteInverses)
            {
                var l = new List<ExpressMetaProperty>(expressType.Properties.Values);
                l.AddRange(expressType.Inverses);
                toWrite = l;
            }
            else
            {
                toWrite = expressType.Properties.Values;
            }
            
            foreach (var ifcProperty in toWrite) //only write out persistent attributes, ignore inverses
            {
                if (ifcProperty.EntityAttribute.State != EntityAttributeState.DerivedOverride)
                {
                    var propType = ifcProperty.PropertyInfo.PropertyType;
                    var propVal = ifcProperty.PropertyInfo.GetValue(entity, null);

                    WriteProperty(ifcProperty.PropertyInfo.Name, propType, propVal, entity, output, -1,
                                  ifcProperty.EntityAttribute);
                }
            }
            output.WriteEndElement();
        }

        private void WriteProperty(string propName, Type propType, object propVal, object entity, XmlWriter output,
                                   int pos, EntityAttributeAttribute attr)
        {
            var optSet = propVal as IOptionalItemSet;
            if (optSet != null && !optSet.Initialized)
            {
                //don't write anything if this is uninitialized item set
                return;
            }
            if (propVal == null)
            //null or a value type that maybe null, need to write out sets and lists if they are mandatroy but empty
            {
                if (typeof(IExpressEnumerable).GetTypeInfo().IsAssignableFrom(propType) && attr.State == EntityAttributeState.Mandatory)
                //special case as these two classes are optimised
                {
                    output.WriteStartElement(propName);
                    output.WriteAttributeString("ex", "cType", null, attr.ListType);
                    output.WriteEndElement();
                }
                return;
            }
            if (propType.GetTypeInfo().IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                (propVal is IExpressValueType)) //deal with undefined types (nullables)
            {
                if (string.IsNullOrEmpty((propVal).ToString()))
                {
                    output.WriteElementString(propName, (propVal).ToString());
                }
                else
                {
                    output.WriteStartElement(propName);
                    if (pos > -1)
                        output.WriteAttributeString("pos", pos.ToString());
                    var val = propVal as IExpressComplexType;
                    if (val != null)
                    {
                        var complexProps = val.Properties;
                        var wrapPos = 0;
                        foreach (var complexProp in complexProps)
                        {
                            WriteProperty(propName, complexProp.GetType(), complexProp, entity, output, wrapPos, attr);
                            wrapPos++;
                        }
                    }
                    else
                        output.WriteValue((propVal).ToString());

                    output.WriteEndElement();
                }

            }
            else if (typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(propType))
            {
                var realType = propVal.GetType();

                if (realType != propType)
                    //we have a type but it is a select type use the actual value but write out explicitly
                {
                    output.WriteStartElement(propName);
                    WriteProperty(realType.Name, realType, propVal, entity, output, pos, attr);
                    output.WriteEndElement();
                }
                else
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement(propName);
                        output.WriteAttributeString("pos", pos.ToString());
                        output.WriteValue((propVal).ToString());
                        output.WriteEndElement();
                    }
                    // if its nullable 
                    //if (((ExpressType)propVal).ToPart21 != "$")
                    else 
                        output.WriteElementString(propName, (propVal).ToString());
                }
            }
            else if (typeof(IExpressEnumerable).GetTypeInfo().IsAssignableFrom(propType))
            {
                output.WriteStartElement(propName);
                output.WriteAttributeString("ex", "cType", null, attr.ListType);
                var i = 0;
                foreach (var item in ((IExpressEnumerable)propVal))
                {
                    WriteProperty(item.GetType().Name, item.GetType(), item, entity, output, i, attr);
                    i++;
                }
                output.WriteEndElement();
            }
            else if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(propType))
                //all writable entities must support this interface and ExpressType have been handled so only entities left
            {
                var persistVal = (IPersistEntity)propVal;
                if (pos == -1) output.WriteStartElement(propName);
                if (_written.Contains(persistVal.EntityLabel)) //we have already written it so use an xlink
                {
                    output.WriteStartElement(propVal.GetType().Name);
                    output.WriteAttributeString("ref", string.Format("i{0}", persistVal.EntityLabel));
                    output.WriteAttributeString("xsi", "nil", null, "true");
                    if (pos > -1) //we are writing out a list element
                        output.WriteAttributeString("pos", pos.ToString());
                    output.WriteEndElement();
                }
                else
                {
                    Write(persistVal, output, pos);
                }
                if (pos == -1) output.WriteEndElement();
            }
            else if (typeof(IExpressComplexType).GetTypeInfo().IsAssignableFrom(propType)) //it is a complex value tpye
            {
                var properties = ((IExpressComplexType)propVal).Properties;
            }
            else if (propType.GetTypeInfo().IsValueType || propType == typeof(string) || propType == typeof(byte[])) //it might be an in-built value type double, string etc
            {
                var pInfoType = propVal.GetType();

                if (pInfoType.GetTypeInfo().IsEnum) //convert enum
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement(propName);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);
                    output.WriteValue(propVal.ToString().ToLower());
                }
                else if (pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(Boolean))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("ex", "boolean-wrapper", null);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);
                    output.WriteValue((bool)propVal ? "true" : "false");
                }
                else if (pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(Double))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("ex", "double-wrapper", null);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);
                    output.WriteValue(string.Format(new Part21Formatter(), "{0:R}", propVal));
                }
                else if (pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(Int16))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("ex", "integer-wrapper", null);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);
                    output.WriteValue(propVal.ToString());
                }
                else if (pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(Int32) ||
                         pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(Int64))
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("ex", "long-wrapper", null);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);
                    output.WriteValue(propVal.ToString());
                }
                else if (pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(String)) //convert  string
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("ex", "string-wrapper", null);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);
                    output.WriteValue(string.Format(new Part21Formatter(), "{0}", propVal));
                }
                else if (pInfoType.GetTypeInfo().UnderlyingSystemType == typeof(byte[])) //convert  byte array
                {
                    if (pos > -1)
                    {
                        output.WriteStartElement("ex", "hexBinary-wrapper", null);
                        output.WriteAttributeString("pos", pos.ToString());
                    }
                    else
                        output.WriteStartElement(propName);

                    var ba = (byte[])propVal;
                    var hex = new System.Text.StringBuilder(ba.Length * 2);
                    foreach (byte b in ba)
                        hex.AppendFormat("{0:X2}", b);

                    output.WriteValue(hex.ToString());
                }

                else
                    throw new ArgumentException(string.Format("Invalid Value Type {0}", pInfoType.Name), "pInfoType");

                output.WriteEndElement();
            }
            else if (typeof(IExpressSelectType).GetTypeInfo().IsAssignableFrom(propType))
                // a select type get the type of the actual value
            {
                if (propVal != null)
                {
                    var realType = propVal.GetType();
                    output.WriteStartElement(propName);
                    if (typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(realType))
                    {
                        //WriteProperty(model, realType.Name + "-wrapper", realType, propVal, entity, output, pos, attr);
                        WriteProperty(realType.Name, realType, propVal, entity, output, pos, attr);
                    }
                    else
                    {
                        WriteProperty(realType.Name, realType, propVal, entity, output, -2, attr);
                    }
                }
                output.WriteEndElement();
            }
            //else
            //    throw new Exception(string.Format("Entity of type {0} has illegal property {1} of type {2}", entity.GetType().ToString(), propType.Name, propType.Name));
        }
    }
}