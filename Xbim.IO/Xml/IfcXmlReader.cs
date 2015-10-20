#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcXmlReader.cs
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
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Esent;
using Xbim.IO.Step21.Parser;

#endregion

namespace Xbim.IO.Xml
{
     
    // ReSharper disable once InconsistentNaming
    public delegate void WriteXMLEntityEventHandler( IPersistEntity entity, int count);

    public class IfcXmlReader
    {
        private static readonly Dictionary<string, StepParserType> Primitives;
        private readonly int _transactionBatchSize = 100;
        private Dictionary<string, int> _idMap;
        private int _lastId;
        static IfcXmlReader()
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

        private abstract class XmlNode 
        {
            public readonly XmlNode Parent;
            public int? Position;

            protected XmlNode()
            {

            }

            protected XmlNode(XmlNode parent)
            {
                Parent = parent;
            }

           
        }

        private class XmlEntity : XmlNode
        {
            public readonly IPersistEntity Entity;

            public XmlEntity(XmlNode parent, IPersistEntity ent)
                : base(parent)
            {
                Entity = ent;
            }
        }

        private class XmlExpressType : XmlNode
        {
            public string Value { get; set; }

            public readonly Type Type;

            public XmlExpressType(XmlNode parent, Type type)
                : base(parent)
            {
                Type = type;
            }
        }

        private class XmlBasicType : XmlNode
        {
            public string Value { get; set; }

            public readonly StepParserType Type;

            public XmlBasicType(XmlNode parent, StepParserType type)
                : base(parent)
            {
                Type = type;
            }
        }

        private class XmlProperty : XmlNode
        {
            public readonly PropertyInfo Property;
            public readonly int PropertyIndex;

            public XmlProperty(XmlNode parent, PropertyInfo prop, int propIndex)
                : base(parent)
            {
                Property = prop;
                PropertyIndex = propIndex;
            }

            public void SetValue(string val, StepParserType parserType)
            {
                if (parserType == StepParserType.Boolean && String.Compare(val, "unknown", StringComparison.OrdinalIgnoreCase) == 0) //do nothing with IfcLogicals that are undefined
                    return;
                var propVal = new PropertyValue();
                propVal.Init(val, parserType);
                ((XmlEntity)Parent).Entity.Parse(PropertyIndex - 1, propVal, null);
            }

            public void SetValue(object o)
            {
                var propVal = new PropertyValue();
                propVal.Init(o);
                ((XmlEntity)Parent).Entity.Parse(PropertyIndex - 1, propVal, null);
            }
        }

        public enum CollectionType
        {
            List,
            ListUnique,
            Set
        }

        private class XmlUosCollection : XmlCollectionProperty
        {
            internal override void SetCollection(IModel model, XmlReader reader)
            {

            }
        }



        private class XmlCollectionProperty : XmlNode
        {
            internal XmlCollectionProperty()
            {

            }

            public XmlCollectionProperty(XmlNode parent, PropertyInfo prop, int propIndex)
                : base(parent)
            {
                Property = prop;
                PropertyIndex = propIndex;
            }

            public readonly List<XmlNode> Entities = new List<XmlNode>();
            public readonly PropertyInfo Property;
            public CollectionType CType = CollectionType.Set;
            public readonly int PropertyIndex;
            public static int CompareNodes(XmlNode a, XmlNode b)
            {
                if (a.Position > b.Position)
                    return 1;
                else if (a.Position < b.Position)
                    return -1;
                else
                    return 0;
            }

            internal virtual void SetCollection(IModel model, XmlReader reader)
            {
                switch (CType)
                {
                    case CollectionType.List:
                    case CollectionType.ListUnique:
                        Entities.Sort(CompareNodes);
                        break;
                    case CollectionType.Set:
                        break;
                    default:
                        throw new Exception("Unknown list type, " + CType);
                }
                
            }
        }

        private XmlNode _currentNode;
        private int _entitiesParsed;
        private string _expressNamespace;
        private string _cTypeAttribute;
        private string _posAttribute;

        private void StartElement(PersistedEntityInstanceCache cache, XmlReader input, XbimEntityCursor entityTable, XbimLazyDBTransaction transaction)
        {
            var elementName = input.LocalName;
            bool isRefType;
            var id = GetId(input, out isRefType);
           
            ExpressType expressType;
            
            StepParserType parserType;
            ExpressMetaProperty prop;
            int propIndex;


            if (id.HasValue && IsIfcEntity(elementName, out expressType)) //we have an element which is an Ifc Entity
            {
                var ent = !cache.Contains(id.Value) ? 
                    cache.CreateNew(expressType.Type, id.Value) : 
                    cache.GetInstance(id.Value, false, true);

                var xmlEnt = new XmlEntity(_currentNode, ent);
               
                //if we have a completely empty element that is not a ref we need to make sure it is written to the database as EndElement will not be called
                if (input.IsEmptyElement && !isRefType)
                {
                    _entitiesParsed++;
                    //now write the entity to the database
                    entityTable.AddEntity(xmlEnt.Entity);
                    if (_entitiesParsed % _transactionBatchSize == (_transactionBatchSize - 1))
                    {
                        transaction.Commit();
                        transaction.Begin();
                    }
                    

                }


                var pos = input.GetAttribute(_posAttribute);
                if (string.IsNullOrEmpty(pos)) pos = input.GetAttribute("pos"); //try without namespace
                if (!string.IsNullOrEmpty(pos))
                    xmlEnt.Position = Convert.ToInt32(pos);
                if (!input.IsEmptyElement)
                {
                    // add the entity to its parent if its parent is a list
                    //if (!(_currentNode is XmlUosCollection) && _currentNode is XmlCollectionProperty && !(_currentNode.Parent is XmlUosCollection))
                    //    ((XmlCollectionProperty)_currentNode).Entities.Add(xmlEnt);

                    _currentNode = xmlEnt;
                }
                else if (_currentNode is XmlProperty)
                {
                    // if it is a ref then it will be empty element and wont have an end tag
                    // so nither SetValue nor EndElement will be called, so set the value of ref here e.g. #3
                    ((XmlProperty)(_currentNode)).SetValue(ent);
                }
                else if (!(_currentNode is XmlUosCollection) && _currentNode is XmlCollectionProperty && !(_currentNode.Parent is XmlUosCollection))
                {
                    ((XmlCollectionProperty)_currentNode).Entities.Add(xmlEnt);
                }
            }
            else if (input.IsEmptyElement)
            {
                if (IsIfcProperty(elementName, out propIndex, out prop))
                {
                    var node = new XmlProperty(_currentNode, prop.PropertyInfo, propIndex);
                    var propVal = new PropertyValue();
                    var t = node.Property.PropertyType;

                    if (typeof (IExpressEnumerable).IsAssignableFrom(t)) return;
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                        t = Nullable.GetUnderlyingType(t);
                    IExpressValueType et = null;
                    if (t != null && typeof(IExpressValueType).IsAssignableFrom(t))
                        et = (IExpressValueType)(Activator.CreateInstance(t));

                    var pt = StepParserType.Undefined;
                    if (et != null)
                        pt = Primitives[et.UnderlyingSystemType.Name];
                    else
                    {
                        if (t != null && t.IsEnum)
                        {
                            pt = StepParserType.Enum;
                        }
                        else
                        {
                            if (t != null) pt = Primitives[t.Name];
                        }
                    }

                    switch (pt.ToString().ToLower())
                    {
                        case "string":
                            propVal.Init("'" + input.Value + "'", pt);
                            break;
                        case "boolean":
                            propVal.Init(Convert.ToBoolean(input.Value) ? ".T." : ".F", pt);
                            break;
                        default:
                            propVal.Init(input.Value, pt);
                            break;
                    }
                    ((XmlEntity)node.Parent).Entity.Parse(node.PropertyIndex - 1, propVal, null);
                }
                else if (IsIfcType(elementName, out expressType))
                {
                    var param = new object[1];
                    param[0] = ""; // empty element
                    var ent = (IPersist)Activator.CreateInstance(expressType.Type, param);

                    ((XmlProperty)_currentNode).SetValue(ent);
                }
            }
            else if (!id.HasValue && IsIfcProperty(elementName, out propIndex, out prop)) //we have an element which is a property
            {

                var cType = input.GetAttribute(_cTypeAttribute);
                if (string.IsNullOrEmpty(cType)) cType = input.GetAttribute("cType"); //in case namespace omitted
                if (IsCollection(prop)) //the property is a collection
                {
                    var xmlColl = new XmlCollectionProperty(_currentNode, prop.PropertyInfo, propIndex);
                    switch (cType)
                    {
                        case "list":
                            xmlColl.CType = CollectionType.List;
                            break;
                        case "list-unique":
                            xmlColl.CType = CollectionType.ListUnique;
                            break;
                        case "set":
                            xmlColl.CType = CollectionType.Set;
                            break;
                        default:
                            xmlColl.CType = CollectionType.List;
                            break;
                    }

                    _currentNode = xmlColl;
                }
                else //it is a simple value property;
                {


                    // its parent can be a collection, if yes then this property needs to be added to parent
                    XmlNode n = new XmlProperty(_currentNode, prop.PropertyInfo, propIndex);
                    var collectionProperty = _currentNode as XmlCollectionProperty;
                    if (collectionProperty != null && !(collectionProperty.Parent is XmlUosCollection))
                        collectionProperty.Entities.Add(n);

                    if (!input.IsEmptyElement) _currentNode = n;
                }
            }
            else if (!id.HasValue && IsIfcType(elementName, out expressType)) // we have an Ifc ExpressType
            {


                // its parent can be a collection, if yes then this property needs to be added to parent
                XmlNode n = new XmlExpressType(_currentNode, expressType.Type);
                var collectionProperty = _currentNode as XmlCollectionProperty;
                if (collectionProperty != null && !(collectionProperty.Parent is XmlUosCollection))
                    collectionProperty.Entities.Add(n);

                if (!input.IsEmptyElement) _currentNode = n;
            }
            else if (!id.HasValue && IsPrimitiveType(elementName, out parserType)) // we have an basic type i.e. double, bool etc
            {
                // its parent can be a collection, if yes then this property needs to be added to parent
                XmlNode n = new XmlBasicType(_currentNode, parserType);
                var collectionProperty = _currentNode as XmlCollectionProperty;
                if (collectionProperty != null && !(collectionProperty.Parent is XmlUosCollection))
                    collectionProperty.Entities.Add(n);

                if (!input.IsEmptyElement) _currentNode = n;
            }
            else
                throw new Exception("Illegal XML element tag");
        }

        private bool IsIfcProperty(string elementName, out int index, out ExpressMetaProperty prop)
        {
            ExpressType expressType;
            var xmlEntity = _currentNode as XmlEntity;
            if (xmlEntity != null && !_metadata.TryGetExpressType(elementName.ToUpper(), out expressType))
            {
                var t = _metadata.ExpressType(xmlEntity.Entity);

                foreach (var p in t.Properties.Where(p => p.Value.PropertyInfo.Name == elementName))
                {
                    prop = p.Value;
                    index = p.Key;
                    return true;
                }
            }
            prop = null;
            index = -1;
            return false;
        }

        private bool IsCollection(ExpressMetaProperty prop)
        {
            return typeof(IExpressEnumerable).IsAssignableFrom(prop.PropertyInfo.PropertyType);
        }

        private bool IsPrimitiveType(string elementName, out StepParserType basicType)
        {
            return Primitives.TryGetValue(elementName, out basicType); //we have a primitive type

        }

        private bool IsIfcType(string elementName, out ExpressType expressType)
        {
            var ok = _metadata.TryGetExpressType(elementName.ToUpper(), out expressType);
            if (!ok)
            {

                if (elementName.Contains("-wrapper") && elementName.StartsWith(_expressNamespace) == false) // we have an inline type definition
                {
                    var inputName = elementName.Substring(0, elementName.LastIndexOf("-", StringComparison.Ordinal));
                    ok = _metadata.TryGetExpressType(inputName.ToUpper(), out expressType);
                }
            }
            return ok && typeof(IExpressValueType).IsAssignableFrom(expressType.Type);
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
                // if we have id or refid then remove letters and get the number part
                //Match match = Regex.Match(strId, @"\d+");

                //if (!match.Success)
                //    throw new Exception(String.Format("Illegal entity id: {0}", strId));
                //return Convert.ToInt32(match.Value);
                
            }
            else if (IsIfcEntity(input.LocalName, out expressType) && !typeof(IExpressValueType).IsAssignableFrom(expressType.Type)) //its a type with no identity, make one
            {
                ++_lastId;
                nextId = _lastId;
            }
            
            return nextId;
        }

        private bool IsIfcEntity(string elementName, out ExpressType expressType)
        {
            return _metadata.TryGetExpressType(elementName.ToUpper(), out expressType);
        }

        private void EndElement(XmlReader input, XmlNodeType prevInputType, string prevInputName, out IPersistEntity writeEntity)
        {
            try
            {
                // before end element, we need to deal with SetCollection
                var collectionProperty = _currentNode as XmlCollectionProperty;
                if (collectionProperty != null)
                {
                    // SetCollection will handle SetValue for Collection
                    var cType = collectionProperty.CType;
                    switch (cType)
                    {
                        case CollectionType.List:
                        case CollectionType.ListUnique:
                            collectionProperty.Entities.Sort(XmlCollectionProperty.CompareNodes);
                            break;
                        case CollectionType.Set:
                            break;
                        default:
                            throw new Exception("Unknown list type, " + cType);
                    }

                    foreach (var item in collectionProperty.Entities)
                    {
                        var entity = item as XmlEntity;
                        if (entity == null) continue;
                        var node = entity;
                        var collectionOwner = entity.Parent.Parent as XmlEntity;
                        var collection = entity.Parent as XmlCollectionProperty; //the collection to add to;
                        if (collectionOwner == null) continue;
                        IPersist ifcCollectionOwner = collectionOwner.Entity;
                        var pv = new PropertyValue();
                        pv.Init(node.Entity);
                        if (collection != null) ifcCollectionOwner.Parse(collection.PropertyIndex - 1, pv, null);
                    }
                }
                else if (_currentNode.Parent is XmlProperty)
                {
                    var propNode = (XmlProperty)_currentNode.Parent;
                    var entity = _currentNode as XmlEntity;
                    if (entity != null)
                    {
                        var entityNode = entity;
                        propNode.SetValue(entityNode.Entity);
                    }
                    else if (_currentNode is XmlExpressType)
                    {
                        //create ExpressType, call ifcparse with propindex and object
                        //((XmlProperty)_currentNode.Parent).SetValue((XmlExpressType)_currentNode);

                        var expressNode = (XmlExpressType)_currentNode;
                        if (expressNode.Type != propNode.Property.PropertyType)
                        {
                            //propNode.SetValue(expressNode);
                            ExpressType expressType;
                            if (IsIfcType(input.LocalName, out expressType))
                            //we have an IPersistIfc
                            {
                                var param = new object[1];
                                param[0] = expressNode.Value;
                                var ent = (IPersist)Activator.CreateInstance(expressType.Type, param);

                                propNode.SetValue(ent);
                            }
                        }
                        else
                        {
                            propNode.SetValue(expressNode.Value, Primitives[expressNode.Type.Name]);
                        }
                    }
                    else if (_currentNode is XmlBasicType)
                    {
                        //set PropertyValue to write type boolean, integer, call ifcparse with string

                        var basicNode = (XmlBasicType)_currentNode;
                        propNode.SetValue(basicNode.Value, basicNode.Type);
                    }
                }


                else if (prevInputType == XmlNodeType.Element && prevInputName == input.LocalName &&
                         _currentNode is XmlProperty && _currentNode.Parent is XmlEntity)
                {
                    // WE SHOULDNT EXECUTE THE FOLLOWING CODE IF THIS PROPERTY ALREADY CALLED SETVALUE
                    var node = (XmlProperty)_currentNode;
                    var propVal = new PropertyValue();
                    var t = node.Property.PropertyType;
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                        t = Nullable.GetUnderlyingType(t);
                    IExpressValueType et = null;
                    if (t != null && typeof(IExpressValueType).IsAssignableFrom(t))
                        et = (IExpressValueType)(Activator.CreateInstance(t));

                    var pt = StepParserType.Undefined;
                    if (et != null)
                        pt = Primitives[et.UnderlyingSystemType.Name];
                    else
                    {
                        if (t != null && t.IsEnum)
                        {
                            pt = StepParserType.Enum;
                        }
                        else if (t != null && Primitives.ContainsKey(t.Name))
                            pt = Primitives[t.Name];
                    }

                    if (pt != StepParserType.Undefined)
                    {
                        switch (pt.ToString().ToLower())
                        {
                            case "string":
                                propVal.Init("'" + input.Value + "'", pt);
                                break;
                            case "boolean":
                                propVal.Init(Convert.ToBoolean(input.Value) ? ".T." : ".F", pt);
                                break;
                            default:
                                propVal.Init(input.Value, pt);
                                break;
                        }

                        ((XmlEntity)node.Parent).Entity.Parse(node.PropertyIndex - 1, propVal, null);
                    }
                }

                else if (_currentNode.Parent is XmlCollectionProperty && !(_currentNode.Parent is XmlUosCollection))
                {
                    var entity = _currentNode as XmlEntity;
                    if (entity != null)
                    {
                        ((XmlCollectionProperty)entity.Parent).Entities.Add(entity);
                    }
                    else if (_currentNode is XmlExpressType)
                    {
                        var expressNode = (XmlExpressType)_currentNode;
                        //actualEntityType is the actual type of the value to create
                        var actualEntityType = expressNode.Type;
                        //Determine if the Express Type is a Nullable, if so get the type of the Nullable
                        if (actualEntityType.IsGenericType && actualEntityType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            actualEntityType = Nullable.GetUnderlyingType(actualEntityType);

                        //need to resolve what the Parser type is
                        //if the generic type of the collection is different from the actualEntityType then we need to create an entity and call Ifc Parse
                        //otherwise we need to call Ifcparse with a string value and the type of the underlying type
                        var collection = (XmlCollectionProperty) _currentNode.Parent; //the collection to add to;
                        var collectionValueType = collection.Property.PropertyType;
                        var collectionGenericType = GetItemTypeFromGenericType(collectionValueType);
                        var genericTypeIsSameAsValueType = (collectionGenericType == actualEntityType);
                        var pv = new PropertyValue();

                        if (genericTypeIsSameAsValueType) //call IfcParse with string value and parser type
                        {
                            var actualEntityValue = (IExpressValueType)(Activator.CreateInstance(actualEntityType));
                            //resolve the underlying type
                            var parserType = Primitives[actualEntityValue.UnderlyingSystemType.Name];
                            if (parserType == StepParserType.String)
                                pv.Init("'" + expressNode.Value + "'", parserType);
                            else
                                pv.Init(expressNode.Value, parserType);
                        }
                        else //call IfcParse with an entity
                        {
                            var param = new object[1];
                            param[0] = expressNode.Value;
                            var actualEntityValue = (IExpressValueType)(Activator.CreateInstance(expressNode.Type, param));
                            pv.Init(actualEntityValue);
                        }

                        var collectionOwner = _currentNode.Parent.Parent as XmlEntity; //go to owner of collection
                        if (collectionOwner != null)
                        {
                            IPersist ifcCollectionOwner = collectionOwner.Entity;
                            ifcCollectionOwner.Parse(collection.PropertyIndex - 1, pv, null);
                        }
                    }
                    else if (_currentNode is XmlBasicType)
                    {
                        var basicNode = (XmlBasicType)_currentNode;
                        var collectionOwner = _currentNode.Parent.Parent as XmlEntity;
                        var collection = (XmlCollectionProperty) _currentNode.Parent; //the collection to add to;
                        if (collectionOwner != null)
                        {
                            IPersist ifcCollectionOwner = collectionOwner.Entity;
                            var pv = new PropertyValue();
                            pv.Init(basicNode.Value, basicNode.Type);
                            ifcCollectionOwner.Parse(collection.PropertyIndex - 1, pv, null);
                        }
                    }
                }


                writeEntity = null;
                if (_currentNode.Parent == null) return;
                var currentNode = _currentNode as XmlEntity;
                if (currentNode != null)
                    writeEntity = currentNode.Entity;
                _currentNode = _currentNode.Parent;
            }
            catch (Exception e)
            {
                throw new Exception("Error reading IfcXML data at node " + input.LocalName, e);
            }
        }

        public Type GetItemTypeFromGenericType(Type genericType)
        {
            if (genericType.IsGenericType || genericType.IsInterface)
            {
                var genericTypes = genericType.GetGenericArguments();
                return genericTypes.GetUpperBound(0) >= 0 ? genericTypes[genericTypes.GetUpperBound(0)] : null;
            }
            if (genericType.BaseType != null)
                return GetItemTypeFromGenericType(genericType.BaseType);
            return null;
        }

        private void SetValue(XmlReader input, XmlNodeType prevInputType)
        {
            try
            {
                // we are here because this node is of type Text or WhiteSpace
                if (prevInputType == XmlNodeType.Element) // previous node should be of Type Element before we go next
                {
                    var currentNode = _currentNode as XmlExpressType;
                    if (currentNode != null)
                    {
                        var node = currentNode;
                        node.Value = input.Value;
                    }
                    else if (_currentNode is XmlBasicType)
                    {
                        var node = (XmlBasicType)_currentNode;
                        node.Value = input.Value;

                    }
                    else if (_currentNode is XmlProperty)
                    {
                        var node = (XmlProperty)_currentNode;
                        var propVal = new PropertyValue();
                        var t = node.Property.PropertyType;
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                            t = Nullable.GetUnderlyingType(t);
                       

                        // if the propertytype is abstract, we cant possibly set any text value on it
                        // effectively this ignores white spaces, e.g. NominalValue of IfcPropertySingleValue
                        if (!t.IsAbstract)
                        {
                            StepParserType parserType;
                            if (typeof(IExpressValueType).IsAssignableFrom(t) && !(typeof(IExpressComplexType).IsAssignableFrom(t) ))
                            {
                                var et = (IExpressValueType)(Activator.CreateInstance(t));
                                parserType = et.UnderlyingSystemType == typeof(bool?) ? 
                                    StepParserType.Boolean : 
                                    Primitives[et.UnderlyingSystemType.Name];
                            }
                            else if (t.IsEnum)
                            {
                                parserType = StepParserType.Enum;
                            }
                            else 
                            {
                                if (!Primitives.TryGetValue(t.Name, out parserType))
                                    parserType = StepParserType.Undefined;
                            }

                            if (parserType == StepParserType.String)
                            {
                                propVal.Init("'" + input.Value + "'", parserType);
                                ((XmlEntity)node.Parent).Entity.Parse(node.PropertyIndex - 1, propVal, null);
                            }
                            else if (parserType != StepParserType.Undefined && !string.IsNullOrWhiteSpace(input.Value))
                            {
                                if (parserType == StepParserType.Boolean)
                                {
                                    if(String.Compare(input.Value, "unknown", StringComparison.OrdinalIgnoreCase) != 0) //do nothing with IfcLogicals that are undefined
                                        propVal.Init(Convert.ToBoolean(input.Value) ? ".T." : ".F.", parserType);
                                }
                                else
                                    propVal.Init(input.Value, parserType);

                                ((XmlEntity)node.Parent).Entity.Parse(node.PropertyIndex - 1, propVal, null);
                            }

                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error reading IfcXML data at node " + input.LocalName, e);
            }
        }

        private ExpressMetaData _metadata;

        internal StepFileHeader Read(PersistedEntityInstanceCache entityInstanceCache, XbimEntityCursor entityTable,  XmlReader input)
        {

            _metadata = entityInstanceCache.Model.Metadata;

            // Read until end of file
            _idMap = new Dictionary<string, int>();
            _lastId = 0;
            _entitiesParsed = 0;
            var foundHeader = false;
            var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty);
            var headerId="";
            while (_currentNode == null && input.Read()) //read through to UOS
            {
                switch (input.NodeType)
                {
                    case XmlNodeType.Element:
                        if (String.Compare(input.LocalName, "uos", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _currentNode = new XmlUosCollection();
                           
                        }
                        else if (String.Compare(input.LocalName, "iso_10303_28", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            foundHeader = true;
                           
                            if (!string.IsNullOrWhiteSpace(input.Prefix))
                            {
                                _expressNamespace = input.Prefix;
                                _cTypeAttribute = _expressNamespace + ":cType";
                                _posAttribute = _expressNamespace + ":pos";
                                _expressNamespace += ":";
                            }
                            else
                            {
                                _cTypeAttribute = "cType";
                                _posAttribute = "pos";
                            } //correct the values if the namespace is defined correctly
                            while (input.MoveToNextAttribute())
                            {
                                if (input.Value == "urn:oid:1.0.10303.28.2.1.1" ||
                                    input.Value =="urn:iso.org:standard:10303:part(28):version(2):xmlschema:common")
                                {
                                    _expressNamespace = input.LocalName;
                                    _cTypeAttribute = _expressNamespace + ":cType";
                                    _posAttribute = _expressNamespace + ":pos";
                                    _expressNamespace += ":";
                                    break;
                                }  
                            }
                        }
                        else
                        {
                            headerId = input.LocalName.ToLower();
                        }
                        break;
                    case XmlNodeType.Text:
                        switch (headerId)
                        {
                            case "name":
                                header.FileName.Name = input.Value;
                                break;
                            case "time_stamp":
                                header.FileName.TimeStamp= input.Value;
                                break;
                            case "author":
                                header.FileName.AuthorName.Add(input.Value);
                                break;
                            case "organization":
                                header.FileName.Organization.Add(input.Value);
                                break;
                            case "preprocessor_version":
                                header.FileName.PreprocessorVersion = input.Value;
                                break;
                            case "originating_system":
                                header.FileName.OriginatingSystem = input.Value;
                                break;
                            case "authorization":
                                header.FileName.AuthorizationName = input.Value;
                                break;
                            case "documentation":
                                header.FileDescription.Description.Add(input.Value);
                                break;
                        }
                        break;
                }
               
            }
            if (!foundHeader)
                throw new Exception("Invalid XML format, iso_10303_28 tag not found");
           
            var prevInputType = XmlNodeType.None;
            var prevInputName = "";

            // set counter for start of every element that is not empty, and reduce it on every end of that element


           
            try
            {
                using (var transaction = entityTable.BeginLazyTransaction())
                {
                    while (input.Read())
                    {
                        switch (input.NodeType)
                        {
                            case XmlNodeType.Element:
                                StartElement(entityInstanceCache, input, entityTable, transaction);
                                break;
                            case XmlNodeType.EndElement:
                                IPersistEntity toWrite;
                                //if toWrite has a value we have completed an Ifc Entity
                                EndElement(input, prevInputType, prevInputName, out toWrite);
                                if (toWrite != null)
                                {
                                    _entitiesParsed++;
                                    //now write the entity to the database
                                    entityTable.AddEntity(toWrite as IInstantiableEntity);
                                    if (_entitiesParsed % _transactionBatchSize == (_transactionBatchSize - 1))
                                    {
                                        transaction.Commit();
                                        transaction.Begin();
                                    }
                                }
                                break;
                            case XmlNodeType.Whitespace:
                                SetValue(input, prevInputType);
                                break;
                            case XmlNodeType.Text:
                                SetValue(input, prevInputType);
                                break;
                        }
                        prevInputType = input.NodeType;
                        prevInputName = input.LocalName;
                    }
                    transaction.Commit();
                }
            }
            catch(Exception e)
            {
                throw new Exception(String.Format("Error reading XML, Line={0}, Position={1}, Tag='{2}'", ((IXmlLineInfo) input).LineNumber, ((IXmlLineInfo) input).LinePosition, input.LocalName), e);
            }
            return header;
        }
    }
}