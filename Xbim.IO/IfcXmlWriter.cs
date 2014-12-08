#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcXmlWriter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO.Parser;
using Xbim.XbimExtensions;


#endregion

namespace Xbim.IO
{
    public class IfcXmlWriter
    {
        #region Fields

        private const string _xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private const string _xlink = "http://www.w3.org/1999/xlink";
        private const string _namespace = "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL";
        private const string _ifcXSD = "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL/IFC2X3.xsd";
        private const string _iso10303urn = "urn:iso.org:standard:10303:part(28):version(2):xmlschema:common";
        private const string _exXSD = "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL/ex.xsd";
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

        private IIfcFileHeader _fileHeader; // removed the initialiser because it's assigned from the model on write.

        #endregion

        public IfcXmlWriter()
        {
            DateTime now = DateTime.Now;
            TimeStamp = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", now.Year, now.Month, now.Day,
                                      now.Hour, now.Minute, now.Second);
            PreprocessorVersion = string.Format("Xbim.Ifc File Processor version {0}",
                                                Assembly.GetAssembly(typeof(P21Parser)).GetName().Version);
            OriginatingSystem = string.Format("Xbim version {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }


        public void Write(XbimModel model, XmlWriter output)
        {
            try
            {
                _written = new HashSet<long>();

                output.WriteStartDocument();
                output.WriteStartElement("ex", "iso_10303_28", _iso10303urn);
                output.WriteAttributeString("version", "2.0");
                output.WriteAttributeString("xmlns", "xsi", null, _xsi);
                output.WriteAttributeString("xmlns", "xlink", null, _xlink);
                output.WriteAttributeString("xmlns", "ex", null, _iso10303urn);
                output.WriteAttributeString("xsi", "schemaLocation", null,
                                            string.Format("{0} {1}", _iso10303urn, _exXSD));

                _fileHeader = model.Header;
                WriteISOHeader(output);
                //Write out the uos
                output.WriteStartElement("uos", _namespace);
                output.WriteAttributeString("id", "uos_1");
                output.WriteAttributeString("description", "Xbim IfcXml Export");
                output.WriteAttributeString("configuration", "i_ifc2x3");
                output.WriteAttributeString("edo", "");
                output.WriteAttributeString("xmlns", "ex", null, _iso10303urn);
                output.WriteAttributeString("xmlns", "ifc", null, _namespace);
                output.WriteAttributeString("xsi", "schemaLocation", null, string.Format("{0} {1}", _namespace, _ifcXSD));

                foreach (var item in model.InstanceHandles)
                {
                    Write(model, item.EntityLabel, output);
                }

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
            output.WriteElementString("ex", "name", null, _fileHeader.FileName.Name);
            output.WriteElementString("ex", "time_stamp", null, _fileHeader.FileName.TimeStamp);

            if (_fileHeader.FileName.AuthorName.Count > 0)
                foreach (string name in _fileHeader.FileName.AuthorName)
                    output.WriteElementString("ex", "author", null, name);
            else
                output.WriteElementString("ex", "author", null, "");

            if (_fileHeader.FileName.Organization.Count > 0)
                foreach (string name in _fileHeader.FileName.Organization)
                    output.WriteElementString("ex", "organization", null, name);
            else
                output.WriteElementString("ex", "organization", null, "");

            output.WriteElementString("ex", "preprocessor_version", null, _fileHeader.FileName.PreprocessorVersion);
            output.WriteElementString("ex", "originating_system", null, _fileHeader.FileName.OriginatingSystem);
            output.WriteElementString("ex", "authorization", null, _fileHeader.FileName.AuthorizationName);

            if (_fileHeader.FileDescription.Description.Count > 0)
                foreach (string name in _fileHeader.FileDescription.Description)
                    output.WriteElementString("ex", "documentation", null, name);
            else
                output.WriteElementString("ex", "documentation", null, "");

            output.WriteEndElement(); //end iso_10303_28_header
        }

        private void Write(XbimModel model, int handle, XmlWriter output, int pos = -1)
        {

            if (_written.Contains(handle)) //we have already done it
                return;
            //int nextId = _written.Count + 1;
            _written.Add(handle);

            IPersistIfcEntity entity = model.GetInstanceVolatile(handle); //load either the cache or a volatile version of the entity
            IfcType ifcType = IfcMetaData.IfcType(entity);

            output.WriteStartElement(ifcType.Type.Name);
            
            output.WriteAttributeString("id", string.Format("i{0}", handle));
            if (pos > -1) //we are writing out a list element
                output.WriteAttributeString("pos", pos.ToString());
            
            IEnumerable<IfcMetaProperty> toWrite;
            if (WriteInverses)
            {
                List<IfcMetaProperty> l = new List<IfcMetaProperty>(ifcType.IfcProperties.Values);
                l.AddRange(ifcType.IfcInverses);
                toWrite = l;
            }
            else
            {
                toWrite = ifcType.IfcProperties.Values;
            }
            
            foreach (IfcMetaProperty ifcProperty in toWrite) //only write out persistent attributes, ignore inverses
            {
                if (ifcProperty.IfcAttribute.State != IfcAttributeState.DerivedOverride)
                {
                    Type propType = ifcProperty.PropertyInfo.PropertyType;
                    object propVal = ifcProperty.PropertyInfo.GetValue(entity, null);

                    WriteProperty(model, ifcProperty.PropertyInfo.Name, propType, propVal, entity, output, -1,
                                  ifcProperty.IfcAttribute);
                }
            }
            output.WriteEndElement();
        }

        private void WriteProperty(XbimModel model, string propName, Type propType, object propVal, object entity, XmlWriter output,
                                   int pos, IfcAttribute attr)
        {
            if (propVal == null)
            //null or a value type that maybe null, need to write out sets and lists if they are mandatroy but empty
            {
                if (typeof(ExpressEnumerable).IsAssignableFrom(propType) && attr.State == IfcAttributeState.Mandatory
                    && !typeof(IfcCartesianPoint).IsAssignableFrom(propType)
                    && !typeof(IfcDirection).IsAssignableFrom(propType))
                //special case as these two classes are optimised
                {
                    output.WriteStartElement(propName);
                    output.WriteAttributeString("ex", "cType", null, attr.ListType);
                    output.WriteEndElement();
                }
                return;
            }
            else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                     (typeof(ExpressType).IsAssignableFrom(propVal.GetType()))) //deal with undefined types (nullables)
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
                    if (typeof(ExpressComplexType).IsAssignableFrom(propVal.GetType()))
                    {
                        IEnumerable<object> complexProps = ((ExpressComplexType)propVal).Properties;
                        foreach (object complexProp in complexProps)
                        {
                            int wrapPos = 0;
                            WriteProperty(model, propName, complexProp.GetType(), complexProp, entity, output, wrapPos, attr);
                            wrapPos++;
                        }
                    }
                    else
                        output.WriteValue((propVal).ToString());

                    output.WriteEndElement();
                }

            }
            else if (typeof(ExpressType).IsAssignableFrom(propType))
            {
                Type realType = propVal.GetType();

                if (realType != propType)
                //we have a type but it is a select type use the actual value but write out explicitly
                {
                    output.WriteStartElement(propName);
                    //WriteProperty(model, realType.Name + "-wrapper", realType, propVal, entity, output, pos, attr);
                    WriteProperty(model, realType.Name, realType, propVal, entity, output, pos, attr);
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
                    else if (propVal.ToString() != null)
                        output.WriteElementString(propName, (propVal).ToString());
                }
            }
            else if (typeof(ExpressEnumerable).IsAssignableFrom(propType)
                     && !typeof(IfcCartesianPoint).IsAssignableFrom(propType)
                     && !typeof(IfcDirection).IsAssignableFrom(propType))
            //special case as these two classes are optimised
            {
                //try
                //{
                output.WriteStartElement(propName);
                output.WriteAttributeString("ex", "cType", null, ((ExpressEnumerable)propVal).ListType);
                int i = 0;
                foreach (object item in ((ExpressEnumerable)propVal))
                {
                    WriteProperty(model, item.GetType().Name, item.GetType(), item, entity, output, i, attr);
                    i++;
                }
                output.WriteEndElement();

                //}
                //catch (Exception e)
                //{

                //    throw;
                //}
            }
            else if (typeof(IPersistIfcEntity).IsAssignableFrom(propType))
            //all writable entities must support this interface and ExpressType have been handled so only entities left
            {
                IPersistIfcEntity persistVal = (IPersistIfcEntity)propVal;
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
                    Write(model, persistVal.EntityLabel, output, pos);
                }
                if (pos == -1) output.WriteEndElement();
            }
            else if (typeof(ExpressComplexType).IsAssignableFrom(propType)) //it is a complex value tpye
            {
                IEnumerable<object> properties = ((ExpressComplexType)propVal).Properties;
            }
            else if (propType.IsValueType) //it might be an in-built value type double, string etc
            {
                Type pInfoType = propVal.GetType();

                if (pInfoType.IsEnum) //convert enum
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
                else if (pInfoType.UnderlyingSystemType == typeof(Boolean))
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
                else if (pInfoType.UnderlyingSystemType == typeof(Double))
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
                else if (pInfoType.UnderlyingSystemType == typeof(Int16))
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
                else if (pInfoType.UnderlyingSystemType == typeof(Int32) ||
                         pInfoType.UnderlyingSystemType == typeof(Int64))
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
                else if (pInfoType.UnderlyingSystemType == typeof(String)) //convert  string
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

                else
                    throw new ArgumentException(string.Format("Invalid Value Type {0}", pInfoType.Name),
                                                "pInfoType");

                output.WriteEndElement();
            }
            else if (typeof(ExpressSelectType).IsAssignableFrom(propType))
            // a select type get the type of the actual value
            {
                Type realType = propVal.GetType();
                output.WriteStartElement(propName);
                if (typeof(ExpressType).IsAssignableFrom(realType))
                {
                    //WriteProperty(model, realType.Name + "-wrapper", realType, propVal, entity, output, pos, attr);
                    WriteProperty(model, realType.Name, realType, propVal, entity, output, pos, attr);
                }
                else
                {
                    WriteProperty(model, realType.Name, realType, propVal, entity, output, -2, attr);
                }
                output.WriteEndElement();
            }
            //else
            //    throw new Exception(string.Format("Entity of type {0} has illegal property {1} of type {2}", entity.GetType().ToString(), propType.Name, propType.Name));
        }
    }
}