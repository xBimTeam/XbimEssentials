using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Step21.Parser;

namespace Xbim.IO.Xml
{
    public class XbimXmlReader4
    {
        public event ReportProgressDelegate ProgressStatus;
        private readonly ExpressMetaData _metadata;
        private readonly GetOrCreateEntity _getOrCreate;
        private readonly FinishEntity _finish;
        private static readonly Dictionary<string, StepParserType> Primitives;
        private Dictionary<string, int> _idMap;
        private int _lastId;
        private readonly string _xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private readonly ILogger Logger;


        /// <summary>
        /// This can be used for analytical and file debugging purposes where 
        /// relation between XML entity in the file and loaded entity needs
        /// to be kept.
        /// </summary>
        public Dictionary<string, int> IdMap { get { return _idMap; } }


        /// <summary>
        /// Constructor of the reader for IFC2x3 XML. XSD is different for different versions of IFC and there is a major difference
        /// between IFC2x3 and IFC4 to there are two different classes to deal with this.
        /// </summary>x
        /// <param name="getOrCreate">Delegate which will be used to getOrCreate new entities</param>
        /// <param name="finish">Delegate which will be called once the entity is finished (no changes will be made to it)
        /// This is useful for a DB when this is the point when it can be serialized to DB</param>
        /// <param name="metadata">Metadata model used to inspect Express types and their properties</param>
        /// <param name="logger">A Logger</param>
        public XbimXmlReader4(GetOrCreateEntity getOrCreate, FinishEntity finish, ExpressMetaData metadata, ILogger logger)
        {
            if (getOrCreate == null)
                throw new ArgumentNullException("getOrCreate");

            if (finish == null)
                throw new ArgumentNullException("finish");

            if (metadata == null)
                throw new ArgumentNullException("metadata");

            _getOrCreate = getOrCreate;
            _finish = finish;
            _metadata = metadata;
            Logger = logger ?? XbimLogging.CreateLogger<XbimXmlReader4>(); ;
        }

        private XbimXmlReader4()
        {
            Logger = XbimLogging.CreateLogger<XbimXmlReader4>(); ;
        }

        static XbimXmlReader4()
        {
            Primitives = new Dictionary<string, StepParserType>
            {
                {"double-wrapper", StepParserType.Real},
                {"long-wrapper", StepParserType.Integer},
                {"string-wrapper", StepParserType.String},
                {"integer-wrapper", StepParserType.Integer},
                {"boolean-wrapper", StepParserType.Boolean},
                {"logical-wrapper", StepParserType.Boolean},
                {"decimal-wrapper", StepParserType.Real},
                {"hexBinary-wrapper", StepParserType.HexaDecimal},
                {"base64Binary-wrapper", StepParserType.Entity},
                {typeof (double).Name, StepParserType.Real},
                {typeof (long).Name, StepParserType.Integer},
                {typeof (string).Name, StepParserType.String},
                {typeof (int).Name, StepParserType.Integer},
                {typeof (bool).Name, StepParserType.Boolean},
                {"Enum", StepParserType.Enum}
            };

        }

        public StepFileHeader Read(Stream xmlStream, IModel model, bool onlyHeader = false)
        {
            //   using (var xmlInStream = new StreamReader(inputStream, Encoding.GetEncoding("ISO-8859-9"))) //this is a work around to ensure Latin character sets are read

            using (var input = XmlReader.Create(xmlStream))
            {
                _streamSize = xmlStream.Length;
                _idMap = new Dictionary<string, int>();

                var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty,model);
                var rootElement = true;
                var headerElement = true;
                while (input.Read())
                {
                    //skip everything except for element nodes
                    if (input.NodeType != XmlNodeType.Element)
                        continue;

                    if (rootElement)
                    {   
                        ReadSchemaInHeader(input, header);
                        rootElement = false;
                        continue;
                    }

                    if (headerElement)
                    {
                        headerElement = false;
                        //header is the first inner node if defined (it is optional)
                        var name = input.LocalName.ToLowerInvariant();
                        if ((name == "header" || name == "iso_10303_28_header") && !input.IsEmptyElement)
                        {
                            header = ReadHeader(input, header);
                            continue;
                        }
                    }

                    //if this is IFC2x3 file and we only need the header we need to make sure we read schema information from "uos" element
                    if (input.LocalName == "uos")
                    {
                        ReadSchemaInHeader(input, header);
                    }

                    if (onlyHeader) return header;

                    //process all root entities in the file (that has to be IPersistEntity)
                    ReadEntity(input);
                    if (_streamSize != -1 && ProgressStatus != null)
                    {
                        double pos = xmlStream.Position;
                        var newPercentage = Convert.ToInt32(pos / _streamSize * 100.0);
                        if (newPercentage > _percentageParsed)
                        {
                            ProgressStatus(_percentageParsed, "Parsing");
                            _percentageParsed = newPercentage;
                        }
                    }
                }
                if (ProgressStatus != null) ProgressStatus(100, "Parsing");
                return header;
            }

        }

        private void ReadSchemaInHeader(XmlReader input, IStepFileHeader header)
        {
            if (!input.IsEmptyElement && input.HasAttributes)
            {
                while (input.MoveToNextAttribute())
                {
                    if (input.Value == "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL")
                    {
                        header.FileSchema.Schemas.Add("IFC2X3");
                        break;
                    }
                    if (input.Value == "http://www.buildingsmart-tech.org/ifcXML/MVD4/IFC4" || input.Value == "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1")
                    {
                        header.FileSchema.Schemas.Add("IFC4");
                        header.FileSchema.Schemas.Add("IFC4Add1");
                        break;
                    }
                    if (input.Value == "http://www.buildingsmart-tech.org/ifcXML/IFC4/final")
                    {
                        header.FileSchema.Schemas.Add("IFC4");
                        break;
                    }
                    if (input.Value == "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add2")
                    {
                        header.FileSchema.Schemas.Add("IFC4");
                        break;
                    }
                }
                input.MoveToElement();
            }
        }

        private ExpressType GetExpresType(XmlReader input)
        {
            //type defined by xml attribute has always a priority over the name of the element
            var typeName = (input.GetAttribute("type", _xsi) ?? input.LocalName).ToUpper();
            if (_metadata.TryGetExpressType(typeName, out ExpressType expType))
                return expType;

            //try to replace WRAPPER keyword
            typeName = input.LocalName.ToUpper();
            if (!typeName.Contains("-")) return null;

            typeName = typeName.Replace("-WRAPPER", "");
            _metadata.TryGetExpressType(typeName, out expType);
            return expType;
        }

        private IPersistEntity ReadEntity(XmlReader input, Type suggestedType = null)
        {
            var expType = GetExpresType(input);
            if (expType == null && suggestedType != null && !suggestedType.IsAbstract)
            {
                expType = _metadata.ExpressType(suggestedType);
            }
            if (expType == null)
            {
                var typeName = input.GetAttribute("type") ?? input.LocalName;
                throw new XbimParserException(typeName + "is not an IPersistEntity type");
            }

            var id = GetId(input, expType, out bool isRef);
            if (!id.HasValue)
                throw new XbimParserException("Wrong entity XML format");

            var entity = _getOrCreate(id.Value, expType.Type);
            if (isRef)
            {
                if (!input.IsEmptyElement)
                {
                    // Consume anything until the end of the entity.
                    // this is usualy empty element but some people put data in there as well to indicate some of the
                    // content for humans reading the XML (as much as it is silly)
                    var depth = input.Depth;
                    var hasContent = false;
                    while (input.Read())
                    {
                        if (input.NodeType == XmlNodeType.EndElement && input.Depth == depth)
                            break;
                        hasContent = true;
                    }
                    if (hasContent)
                    {
                        // Log a warning as it is a wrong practise to put content in ref elements
                        Logger.LogWarning("Reference to element {0}, ref='{1}' is not empty. This is a wrong practise.", entity.ExpressType.Name, id);
                    }
                }
                return entity;
            }

            //read all attributes
            while (input.MoveToNextAttribute())
            {
                var pInfo = GetMetaProperty(expType, input.LocalName);
                if (pInfo == null) continue;
                SetPropertyFromString(pInfo, entity, input.Value, null);
            }
            input.MoveToElement();

            if (input.IsEmptyElement)
            {
                _finish(entity);
                return entity;
            }

            //read all element properties
            var pDepth = input.Depth;
            while (input.Read())
            {
                if (input.NodeType == XmlNodeType.EndElement && input.Depth == pDepth) break;
                if (input.NodeType != XmlNodeType.Element) continue;
                var pInfo = GetMetaProperty(expType, input.LocalName);
                if (pInfo == null) continue;
                SetPropertyFromElement(pInfo, entity, input, null);
                if (input.NodeType == XmlNodeType.EndElement && input.Depth == pDepth) break;
            }

            //finalize
            _finish(entity);
            return entity;
        }

        private bool InitPropertyValue(Type type, string value, out IPropertyValue propertyValue)
        {
            var propVal = new PropertyValue();
            if (type == typeof(bool))
            {
                propVal.Init(string.CompareOrdinal(value, "true") == 0 ? ".T." : ".F.", StepParserType.Boolean);
                propertyValue = propVal;
                return true;
            }
            if (type == typeof(bool?))
            {
                if (string.CompareOrdinal(value, "unknown") == 0)
                {
                    propertyValue = null;
                    return false;
                }
                propVal.Init(string.CompareOrdinal(value, "true") == 0 ? ".T." : ".F.", StepParserType.Boolean);
                propertyValue = propVal;
                return true;
            }
            if (typeof(string) == type)
            {
                propVal.Init("'" + value + "'", StepParserType.String);
                propertyValue = propVal;
                return true;
            }
            if (typeof(int) == type || typeof(long) == type)
            {
                propVal.Init(value, StepParserType.Integer);
                propertyValue = propVal;
                return true;
            }
            if (typeof(float) == type || typeof(double) == type)
            {
                propVal.Init(value, StepParserType.Real);
                propertyValue = propVal;
                return true;
            }
            if (typeof(byte[]) == type)
            {
                propVal.Init("\"0" + value + "\"", StepParserType.HexaDecimal);
                propertyValue = propVal;
                return true;
            }

            throw new XbimParserException("Unexpected type: " + type.Name);
        }

        private void Parse(IPersistEntity entity, int propIndex, IPropertyValue value, int[] nested)
        {
            try
            {
                entity.Parse(propIndex, value, nested);
            }
            catch (InvalidCastException e)
            {
                var prop = entity.ExpressType.Properties[propIndex + 1];
                var type = value.Type;
                var vType = "";
                switch (type)
                {
                    case StepParserType.Boolean:
                        vType = "Boolean";
                        break;
                    case StepParserType.Enum:
                        vType = "Enumeration value " + value.EnumVal;
                        break;
                    case StepParserType.Entity:
                        vType = value.EntityVal.GetType().Name;
                        var vEntity = value.EntityVal as IPersistEntity;
                        if (vEntity != null)
                        {
                            var info = GetErrIdentityInfo(vEntity);
                            if (!string.IsNullOrWhiteSpace(info))
                                vType = vType + " (" + info + ")";
                        }

                        break;
                    case StepParserType.HexaDecimal:
                        vType = "HexaDecimal";
                        break;
                    case StepParserType.Integer:
                        vType = "Integer";
                        break;
                    case StepParserType.Real:
                        vType = "Real";
                        break;
                    case StepParserType.String:
                        vType = "String";
                        break;
                    case StepParserType.Undefined:
                    default:
                        vType = "Undefined";
                        break;
                }


                var identity = GetErrIdentityInfo(entity);
                var msg = String.Format("{0} is not assignable to {1}.{2} ({3})",
                        vType,
                        entity.ExpressType.ExpressName,
                        prop.Name, identity);
                Logger.LogError(new EventId(1000, "Failed Assignment"), e, msg);
            }
        }

        private string GetErrIdentityInfo(IPersistEntity entity)
        {
            var xmlRef = _idMap.FirstOrDefault(kv => kv.Value == entity.EntityLabel).Key;
            var et = entity.ExpressType;
            var guidProp = et.Properties.FirstOrDefault(p => p.Value.Name == "GlobalId").Value;
            var nameProp = et.Properties.FirstOrDefault(p => p.Value.Name == "Name").Value;

            var identityBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(xmlRef))
                identityBuilder.AppendFormat("id/ref={0} ", xmlRef);
            if (guidProp != null)
            {
                var guidObj = guidProp.PropertyInfo.GetValue(entity);
                if (guidObj != null && !string.IsNullOrWhiteSpace(guidObj.ToString()))
                    identityBuilder.AppendFormat("guid='{0}' ", guidObj.ToString());
            }
            if (nameProp != null)
            {
                var nameObj = nameProp.PropertyInfo.GetValue(entity);
                if (nameObj != null && !string.IsNullOrWhiteSpace(nameObj.ToString())) 
                    identityBuilder.AppendFormat("name='{0}' ", nameObj.ToString());
            }
            return identityBuilder.ToString().Trim();
        }

        private void SetPropertyFromElement(ExpressMetaProperty property, IPersistEntity entity, XmlReader input, int[] pos, ExpressType valueType = null)
        {
            var name = input.LocalName;
            var type = valueType != null ? valueType.Type : GetNonNullableType(property.PropertyInfo.PropertyType);
            var pIndex = property.EntityAttribute.Order - 1;
            var expType = valueType ?? GetExpresType(input);
            try
            {
                //select type
            if (typeof (IExpressSelectType).GetTypeInfo().IsAssignableFrom(type) && type.GetTypeInfo().IsInterface)
                {
                    //move to inner element which represents the data
                    var sDepth = input.Depth;
                    while (input.Read())
                    {
                        if (input.NodeType == XmlNodeType.EndElement && input.Depth == sDepth) return;
                        if (input.NodeType != XmlNodeType.Element) continue;
                        expType = GetExpresType(input);
                        if (expType == null)
                            throw new XbimParserException("Unexpected select data type " + name);

                    if (typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(expType.Type))
                        {
                            SetPropertyFromElement(property, entity, input, pos, expType);
                            return;
                        }

                    if (typeof (IPersistEntity).GetTypeInfo().IsAssignableFrom(expType.Type))
                        {
                            SetPropertyFromElement(property, entity, input, pos, expType);
                            return;
                        }

                        //this should either be a defined type or entity
                        throw new XbimParserException("Unexpected select data type " + expType.Name);
                    }
                    return;
                }

                //defined type
            if (typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(type))
                {
                    var cType = expType.UnderlyingComplexType;

                    //if it is just a value we can use 'SetPropertyFromAttribute'
                if (type == property.PropertyInfo.PropertyType && !(cType != null && typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(cType)))
                    {
                        var strValue = input.ReadElementContentAsString();
                        SetPropertyFromString(property, entity, strValue, pos);
                        return;
                    }

                    if (cType != null)
                    {
                        var cInnerValueType = typeof(List<>);
                        cInnerValueType = cInnerValueType.MakeGenericType(cType);
                        if (!(Activator.CreateInstance(cInnerValueType) is IList cInnerList))
                            throw new XbimParserException("Initialization of " + cInnerValueType.Name + " failed.");

                        if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(cType))
                        {
                            if (!input.IsEmptyElement)
                            {
                                var cDepth = input.Depth;
                                while (input.Read())
                                {
                                    if (input.NodeType == XmlNodeType.EndElement && input.Depth == cDepth) break;
                                    if (input.NodeType != XmlNodeType.Element) continue;
                                    var innerEntity = ReadEntity(input);
                                    cInnerList.Add(innerEntity);
                                    if (input.NodeType == XmlNodeType.EndElement && input.Depth == cDepth) break;
                                }

                                var cValueVal = Activator.CreateInstance(expType.Type, cInnerList);
                                var cpValue = new PropertyValue();
                                cpValue.Init(cValueVal);
                                Parse(entity, pIndex, cpValue, null);
                            }
                        }
                        else
                        {
                            var cValuesString = input.ReadElementContentAsString();
                            SetPropertyFromString(property, entity, cValuesString, pos);
                        }
                        return;
                    }

                    //normal defined type has string based constructor which will set the right value
                    var sValue = input.ReadElementContentAsString();
                    if (property.EnumerableType == expType.Type)
                    {
                        InitPropertyValue(expType.UnderlyingType, sValue, out IPropertyValue pValue);
                        Parse(entity, pIndex, pValue, pos);
                        return;
                    }
                    else
                    {
                        var pValue = new PropertyValue();
                        var pValueVal = Activator.CreateInstance(expType.Type, sValue);
                        pValue.Init(pValueVal);
                        Parse(entity, pIndex, pValue, pos);
                        return;
                    }
                }

            if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(type) || 
                    (typeof(IEnumerable).IsAssignableFrom(type) && property.EntityAttribute.MaxCardinality != null && property.EntityAttribute.MaxCardinality[0] == 1) ||
                    (typeof(IEnumerable).IsAssignableFrom(type) && input.GetAttribute("type", _xsi) != null)
                    )
                {
                    // invers property like IsTypedBy might not have anything to define the type
                    // because only single type is applicable
                    if (typeof(IEnumerable).IsAssignableFrom(type) &&
                        property.EntityAttribute.MaxCardinality != null &&
                        property.EntityAttribute.MaxCardinality[0] == 1)
                    {
                        type = type.GetGenericArguments()[0];
                    }

                    var value = ReadEntity(input, type);
                    var pVal = new PropertyValue();

                    // swap value and entity if this is inverse relation
                    if (property.IsInverse)
                    {
                        pVal.Init(entity);
                        var remotePropName = property.InverseAttributeProperty.RemoteProperty;
                        var remoteProperty = value.ExpressType.Properties.FirstOrDefault(p => p.Value.Name == remotePropName).Value;
                        if (remoteProperty == null)
                            throw new XbimParserException("Non existing counterpart to " + property.Name);
                        Parse(value, remoteProperty.EntityAttribute.Order - 1, pVal, null);
                    }
                    else
                    {
                        pVal.Init(value);
                        Parse(entity, pIndex, pVal, null);
                    }
                    return;
                }

                //enumeration or inverse enumeration
            if (typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(type))
                {
                    //do nothing with empty list. If it is mandatory it is initialized anyway
                    if (input.IsEmptyElement)
                        return;

                    var enumDepth = input.Depth;
                    while (input.Read())
                    {
                        if (input.NodeType == XmlNodeType.EndElement && input.Depth == enumDepth) break;
                        if (input.NodeType != XmlNodeType.Element) continue;

                        //position is optional
                        var posAttr = input.GetAttribute("pos");
                        pos = null;
                        if (!string.IsNullOrWhiteSpace(posAttr))
                        {
                            var idx = posAttr.Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                                .Select(i => Convert.ToInt32(i)).ToList();
                            //remove the last one as it is not used in Parse function
                            if (idx.Count > 0)
                                idx.RemoveAt(idx.Count - 1);
                            //only set if it has any values in
                            if (idx.Count > 0)
                                pos = idx.ToArray();
                        }

                        //it might be a primitive
                        name = input.LocalName;
                        if (Primitives.ContainsKey(input.LocalName))
                        {
                            var iVal = input.ReadElementContentAsString();
                            var pVal = new PropertyValue();
                            pVal.Init(iVal, Primitives[name]);
                            Parse(entity, pIndex, pVal, pos);
                            if (input.NodeType == XmlNodeType.EndElement && input.Depth == enumDepth) break;
                            continue;
                        }

                        var eType = GetExpresType(input);
                        if (eType == null)
                            throw new XbimParserException("Unexpected type " + name);
                        SetPropertyFromElement(property, entity, input, pos, eType);
                        if (input.NodeType == XmlNodeType.EndElement && input.Depth == enumDepth) break;
                    }
                    return;
                }
            }
            catch (XbimParserException)
            {
                // rethrow
                throw;
            }
            catch (Exception e)
            {
                // wrap into parser exception
                throw new XbimParserException(e.Message, e);
            }

            // we should never get here
            throw new XbimParserException("Unexpected type: " + type.Name);
        }

        private readonly char[] _separator = { ' ' };
        private long _streamSize;
        private int _percentageParsed;

        private void SetPropertyFromString(ExpressMetaProperty property, IPersistEntity entity, string value, int[] pos, Type valueType = null)
        {
            var pIndex = property.EntityAttribute.Order - 1;
            var type = valueType ?? property.PropertyInfo.PropertyType;
            type = GetNonNullableType(type);
            var propVal = new PropertyValue();
            try
            {
                if (type.GetTypeInfo().IsValueType || type == typeof(string))
                {
                if (typeof(IExpressComplexType).GetTypeInfo().IsAssignableFrom(type))
                    {
                        var meta = _metadata.ExpressType(type);
                        var values = value.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                        var underType = meta.UnderlyingComplexType;
                        foreach (var v in values)
                        {
                        if (InitPropertyValue(underType, v, out IPropertyValue pv))
                                Parse(entity, pIndex, pv, pos);
                        }
                        return;
                    }
                if (type.GetTypeInfo().IsEnum)
                    {
                        propVal.Init(value, StepParserType.Enum);
                        Parse(entity, pIndex, propVal, pos);
                        return;
                    }

                    //handle other value types
                if (typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(type))
                    {
                        var meta = _metadata.ExpressType(type);
                        type = meta.UnderlyingType;
                    }
                if (InitPropertyValue(type, value, out IPropertyValue pVal))
                        Parse(entity, pIndex, pVal, pos);
                    return;
                }

                //lists of value types will be serialized as lists. If this is not an IEnumerable this is not the case
            if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type) || !type.GetTypeInfo().IsGenericType)
                    throw new XbimParserException("Unexpected enumerable type " + type.Name);

            var genType = type.GetTypeInfo().GetGenericArguments()[0];
            if (genType.GetTypeInfo().IsValueType || genType == typeof(string))
                {
                    //handle enumerable of value type and string
                    var values = value.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var v in values)
                    {
                        SetPropertyFromString(property, entity, v, pos, genType);
                    }
                    return;
                }

                //rectangular nested lists can also be serialized as attribute if defined in configuration
            if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(genType))
                {
                    //handle rectangular nested lists (like IfcPointList3D)
                    var min = property.EntityAttribute.MaxCardinality != null ? property.EntityAttribute.MaxCardinality.LastOrDefault() : 0;
                    var max = property.EntityAttribute.MinCardinality != null ? property.EntityAttribute.MinCardinality.LastOrDefault() : -1;
                    if (min != max)
                        throw new XbimParserException(property.Name + " is not rectangular so it can't be serialized as a simple text string");

                    var values = value.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
                    var valType = GetNonGenericType(genType);
                if (typeof (IExpressValueType).GetTypeInfo().IsAssignableFrom(valType))
                    {
                        var expValType = _metadata.ExpressType(valType);
                        if (expValType == null)
                            throw new XbimParserException("Unexpected data type " + valType.Name);
                        valType = expValType.UnderlyingType;
                    }

                    for (var i = 0; i < values.Length; i++)
                    {
                    InitPropertyValue(valType, values[i], out IPropertyValue pValue);
                        var idx = i / min;
                        Parse(entity, pIndex, pValue, new[] { idx });
                    }
                }
            }
            catch (XbimParserException)
            {
                // rethrow
                throw;
            }
            catch (Exception e)
            {
                // wrap
                throw new XbimParserException(e.Message, e);
            }

        }

        private static Type GetNonGenericType(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType) return type;
            while (type.GetTypeInfo().IsGenericType && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type))
            {
                var genArgs = type.GetTypeInfo().GetGenericArguments();
                if (!genArgs.Any()) break;
                type = genArgs[0];
            }
            return type;
        }

        private static Type GetNonNullableType(Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
                return type;
            if (!type.GetTypeInfo().IsGenericType)
                return type;
            if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return type;
            return type.GetTypeInfo().GetGenericArguments()[0];
        }

        private static ExpressMetaProperty GetMetaProperty(ExpressType expType, string name)
        {
            var prop = expType.Properties.FirstOrDefault(p => string.CompareOrdinal(p.Value.Name, name) == 0);
            return prop.Value ?? expType.Inverses.FirstOrDefault(p => string.CompareOrdinal(p.Name, name) == 0);
        }


        private int? GetId(XmlReader input, ExpressType expressType, out bool isRefType)
        {
            isRefType = false;
            int? nextId = null;
            var strId = input.GetAttribute("id");
            if (string.IsNullOrEmpty(strId))
            {
                strId = input.GetAttribute("ref");
                if (!string.IsNullOrEmpty(strId)) isRefType = true;
            }
            if (!string.IsNullOrEmpty(strId)) //must be a new instance or a reference to an existing one  
            {
                if (!_idMap.TryGetValue(strId, out int lookup))
                {
                    ++_lastId;
                    nextId = _lastId;
                    _idMap.Add(strId, nextId.Value);
                }
                else
                    nextId = lookup;
            }
            else if (!typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(expressType.Type)) //its a type with no identity, make one
            {
                ++_lastId;
                nextId = _lastId;
            }

            return nextId;
        }

        private bool IsExpressEntity(string elementName, out ExpressType expressType)
        {
            return _metadata.TryGetExpressType(elementName.ToUpperInvariant(), out expressType);
        }

        private StepFileHeader ReadHeader(XmlReader input, StepFileHeader header)
        {
            var depth = input.Depth;
            while (input.Read())
            {
                if (input.NodeType == XmlNodeType.EndElement && input.Depth == depth) break;
                if (input.NodeType != XmlNodeType.Element) continue;

                switch (input.LocalName.ToLowerInvariant())
                {
                    case "name":
                        header.FileName.Name = GetTextFromElement(input);
                        break;
                    case "time_stamp":
                        header.FileName.TimeStamp = GetTextFromElement(input);
                        break;
                    case "author":
                        header.FileName.AuthorName.Add(GetTextFromElement(input));
                        break;
                    case "organization":
                        header.FileName.Organization.Add(GetTextFromElement(input));
                        break;
                    case "preprocessor_version":
                        header.FileName.PreprocessorVersion = GetTextFromElement(input);
                        break;
                    case "originating_system":
                        header.FileName.OriginatingSystem = GetTextFromElement(input);
                        break;
                    case "authorization":
                        header.FileName.AuthorizationName = GetTextFromElement(input);
                        break;
                    case "documentation":
                        header.FileDescription.Description.Add(GetTextFromElement(input));
                        break;
                }
            }
            return header;
        }

        private string GetTextFromElement(XmlReader input)
        {
            if (input.NodeType != XmlNodeType.Element || input.IsEmptyElement)
                return null;

            if (!input.Read())
                return null;

            if (input.NodeType != XmlNodeType.Text)
                throw new FormatException("Unexpected node type");

            return input.Value;
        }

        public static IStepFileHeader ReadHeader(Stream input)
        {
            var xReader = new XbimXmlReader4();
            var fakeModel = new Memory.MemoryModel(new Ifc4.EntityFactoryIfc4());
            return xReader.Read(input, fakeModel, true); //using a dummy model to get the assembly correct
        }

        public static XmlSchemaVersion ReadSchemaVersion(XmlReader input)
        {
            var dist = 0;
            //read the root element
            while (input.Read())
            {
                //don't dig deeper than 100 elements
                if (dist > 100) return XmlSchemaVersion.Unknown;

                //skip any whitespaces or anything
                if (input.NodeType != XmlNodeType.Element) continue;
                dist++;

                //read namespace info
                while (input.MoveToNextAttribute())
                {
                    if (string.Equals(input.Value, "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(input.Value, "http://www.iai-international.org/ifcXML2/RC2/IFC2X3", StringComparison.OrdinalIgnoreCase))
                        return XmlSchemaVersion.Ifc2x3;

                    if (string.Equals(input.Value, "http://www.buildingsmart-tech.org/ifcXML/MVD4/IFC", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(input.Value, "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1", StringComparison.OrdinalIgnoreCase))
                        return XmlSchemaVersion.Ifc4Add1;

                    if (string.Equals(input.Value, "http://www.buildingsmart-tech.org/ifcXML/IFC4/final", StringComparison.OrdinalIgnoreCase))
                        return XmlSchemaVersion.Ifc4;

                    if (string.Equals(input.Value, "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add2", StringComparison.OrdinalIgnoreCase))
                        return XmlSchemaVersion.Ifc4Add2;
                }
                input.MoveToElement();
            }
            return XmlSchemaVersion.Unknown;
        }
    }

    public enum XmlSchemaVersion
    {
        // ReSharper disable once InconsistentNaming
        Ifc2x3,
        Ifc4Add1,
        Ifc4Add2,
        Ifc4,
        Unknown
    }
}
