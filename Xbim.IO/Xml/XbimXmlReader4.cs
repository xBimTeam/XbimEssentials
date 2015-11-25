using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ExpressMetaData _metadata;
        private readonly GetOrCreateEntity _getOrCreate;
        private readonly FinishEntity _finish;
        private static readonly Dictionary<string, StepParserType> Primitives;
        private Dictionary<string, int> _idMap;
        private int _lastId;
        private readonly string _xsi = "http://www.w3.org/2001/XMLSchema-instance";

        /// <summary>
        /// Constructor of the reader for IFC2x3 XML. XSD is different for different versions of IFC and there is a major difference
        /// between IFC2x3 and IFC4 to there are two different classes to deal with this.
        /// </summary>x
        /// <param name="getOrCreate">Delegate which will be used to getOrCreate new entities</param>
        /// <param name="finish">Delegate which will be called once the entity is finished (no changes will be made to it)
        /// This is useful for a DB when this is the point when it can be serialized to DB</param>
        /// <param name="metadata">Metadata model used to inspect Express types and their properties</param>
        public XbimXmlReader4(GetOrCreateEntity getOrCreate, FinishEntity finish, ExpressMetaData metadata)
        {
            if (getOrCreate == null) throw new ArgumentNullException("getOrCreate");
            if (finish == null) throw new ArgumentNullException("finish");
            if (metadata == null) throw new ArgumentNullException("metadata");
            _getOrCreate = getOrCreate;
            _finish = finish;
            _metadata = metadata;
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

        public StepFileHeader Read(XmlReader input)
        {
            _idMap = new Dictionary<string, int>();
            
            var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty);
            var rootElement = true;
            var headerElement = true;
            while (input.Read())
            {
                //skip everything except for element nodes
                if (input.NodeType != XmlNodeType.Element)
                    continue;

                if (rootElement)
                {
                    //do any root element processing here (check namespace, but it might be defined further down)
                    rootElement = false;
                    continue;
                }

                if (headerElement)
                {
                    //header is the first inner node if defined (it is optional)
                    var name = input.LocalName.ToLowerInvariant();
                    if (name == "header")
                    {
                        header = ReadHeader(input.ReadSubtree());
                        continue;
                    }
                    headerElement = false;
                }

                //process all root entities in the file (that has to be IPersistEntity)
                ReadEntity(input.IsEmptyElement ? input : input.ReadSubtree());
            }

            return header;

        }

        private ExpressType GetExpresType(XmlReader input)
        {
            var typeName = input.LocalName.ToUpperInvariant();
            ExpressType expType;
            if (!_metadata.TryGetExpressType(typeName, out expType))
            {
                //try to get type name from an attribute
                typeName = input.GetAttribute("type", _xsi);
                if (typeName == null || !_metadata.TryGetExpressType(typeName.ToUpperInvariant(), out expType))
                    return null;
            }
            return expType;
        }

        private IPersistEntity ReadEntity(XmlReader input)
        {
            var typeName = input.LocalName.ToUpperInvariant();
            var expType = GetExpresType(input);
            if (expType == null)
                throw new XbimParserException(typeName + "is not an IPersistEntity type");

            bool isRef;
            var id = GetId(input, out isRef);
            if(!id.HasValue)
                throw new XbimParserException("Wrong entity XML format");

            var entity = _getOrCreate(id.Value, expType.Type);
            if (isRef) return entity;

            //read all attributes
            while (input.MoveToNextAttribute())
            {
                var pInfo = GetMetaProperty(expType, input.LocalName);
                if(pInfo == null) continue;
                SetPropertyFromAttribute(pInfo, entity, input.Value);
            }

            if (input.IsEmptyElement)
            {
                _finish(entity);
                return entity;
            }

            //read all elements
            while (input.Read())
            {
                var pInfo = GetMetaProperty(expType, input.LocalName);
                if (pInfo == null) continue;
                SetPropertyFromElement(pInfo, entity, input.IsEmptyElement ? input : input.ReadSubtree());
            }

            //finalize
            _finish(entity);
            return entity;
        }

        private bool InitPropertyValue(Type type, string value, out IPropertyValue propertyValue)
        {
            var propVal = new PropertyValue();
            if (type == typeof (bool))
            {
                propVal.Init(string.CompareOrdinal(value, "true") == 0 ? ".T." : ".F.", StepParserType.Boolean);
                propertyValue = propVal;
                return true;
            }
            if (type == typeof (bool?))
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
            if (typeof (string) == type)
            {
                propVal.Init("'"  + value + "'", StepParserType.String);
                propertyValue = propVal;
                return true;
            }
            if (typeof (int) == type || typeof (long) == type)
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

            throw new XbimParserException("Unexpected type: " + type.Name);
        }

        private void SetPropertyFromElement(ExpressMetaProperty property, IPersistEntity entity, XmlReader input)
        {
        }

        private void SetPropertyFromAttribute(ExpressMetaProperty property, IPersistEntity entity, string value, Type valueType = null)
        {
            var pIndex = property.EntityAttribute.Order - 1;
            var type = valueType ?? property.PropertyInfo.PropertyType;
            type = GetNonNullableType(type);
            var propVal = new PropertyValue();
            
            if (type.IsValueType || type == typeof(string))
            {
                if (typeof(IExpressComplexType).IsAssignableFrom(type))
                {
                    var meta = _metadata.ExpressType(type);
                    var values = value.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                    var underType = meta.UnderlyingComplexType;
                    foreach (var v in values)
                    {
                        IPropertyValue pv;
                        if(InitPropertyValue(underType, v, out pv))
                            entity.Parse(pIndex, pv, null);
                    }
                    return;
                }
                if (type.IsEnum)
                {
                    propVal.Init(value, StepParserType.Enum);
                    entity.Parse(pIndex, propVal, null);
                    return;
                }

                //handle other value types
                if (typeof(IExpressValueType).IsAssignableFrom(type))
                {
                    var meta = _metadata.ExpressType(type);
                    type = meta.UnderlyingType;
                }
                IPropertyValue pVal;
                if (InitPropertyValue(type, value, out pVal))
                    entity.Parse(pIndex, pVal, null);
                return;
            }

            //lists of value types will be serialized as lists. If this is not an IEnumerable this is not the case
            if (!typeof(IEnumerable).IsAssignableFrom(type) || !type.IsGenericType)
                throw new XbimParserException("Unexpected enumerable type " + type.Name);

            var genType = type.GetGenericArguments()[0];
            if (genType.IsValueType || genType == typeof(string))
            {
                //handle enumerable of value type and string
                var values = value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var v in values)
                {
                    SetPropertyFromAttribute(property, entity, v, genType);
                }
                return;
            }

            //rectangular nested lists can also be serialized as attribute if defined in configuration
            if (typeof(IEnumerable).IsAssignableFrom(genType))
            {
                //handle rectangular nested lists (like IfcPointList3D)
                throw new NotImplementedException();
            }
        }

        private static Type GetNonNullableType(Type type)
        {
            if (!type.IsValueType)
                return type;
            if (!type.IsGenericType)
                return type;
            if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return type;
            return type.GetGenericArguments()[0];
        }

        private static ExpressMetaProperty GetMetaProperty(ExpressType expType, string name)
        {
            var prop = expType.Properties.FirstOrDefault(p => string.CompareOrdinal(p.Value.Name, name) == 0);
            return prop.Value ?? expType.Inverses.FirstOrDefault(p => string.CompareOrdinal(p.Name, name) == 0);
        }


        private int? GetId(XmlReader input, out bool isRefType)
        {
            isRefType = false;
            int? nextId = null;
            ExpressType expressType;
            var strId = input.GetAttribute("id");
            if (string.IsNullOrEmpty(strId))
            {
                strId = input.GetAttribute("ref");
                if (!string.IsNullOrEmpty(strId)) isRefType = true;
            }
            if (!string.IsNullOrEmpty(strId)) //must be a new instance or a reference to an existing one  
            {
                int lookup;
                if (!_idMap.TryGetValue(strId, out lookup))
                {
                    ++_lastId;
                    nextId = _lastId;
                    _idMap.Add(strId, nextId.Value);
                }
                else
                    nextId = lookup;
            }
            else if (
                IsExpressEntity(input.LocalName, out expressType) && 
                !typeof(IExpressValueType).IsAssignableFrom(expressType.Type)) //its a type with no identity, make one
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

        private StepFileHeader ReadHeader(XmlReader input)
        {
            var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty);
            while (input.Read())
            {
                if (input.NodeType != XmlNodeType.Element)
                    continue;
                switch (input.LocalName.ToLowerInvariant())
                {
                    case "name":
                        header.FileName.Name = input.ReadInnerXml();
                        break;
                    case "time_stamp":
                        header.FileName.TimeStamp = input.ReadInnerXml();
                        break;
                    case "author":
                        header.FileName.AuthorName.Add(input.ReadInnerXml());
                        break;
                    case "organization":
                        header.FileName.Organization.Add(input.ReadInnerXml());
                        break;
                    case "preprocessor_version":
                        header.FileName.PreprocessorVersion = input.ReadInnerXml();
                        break;
                    case "originating_system":
                        header.FileName.OriginatingSystem = input.ReadInnerXml();
                        break;
                    case "authorization":
                        header.FileName.AuthorizationName = input.ReadInnerXml();
                        break;
                    case "documentation":
                        header.FileDescription.Description.Add(input.ReadInnerXml());
                        break;
                }
            }
            return header;
        }

        public static XmlSchemaVersion ReadSchemaVersion(XmlReader input)
        {
            //read the root element
            while (input.Read())
            {
                //read namespace
                while (input.MoveToNextAttribute())
                {
                    if (input.Value == "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL")
                        return XmlSchemaVersion.Ifc2x3;

                    if (input.Value == "http://www.buildingsmart-tech.org/ifcXML/MVD4/IFC")
                        return XmlSchemaVersion.Ifc4Add1;

                    if (input.Value == "http://www.buildingsmart-tech.org/ifcXML/IFC4/final")
                        return XmlSchemaVersion.Ifc4;
                }
            }
            return XmlSchemaVersion.Unknown;
        }
    }

    public enum XmlSchemaVersion
    {
        // ReSharper disable once InconsistentNaming
        Ifc2x3,
        Ifc4Add1,
        Ifc4,
        Unknown
    }
}
