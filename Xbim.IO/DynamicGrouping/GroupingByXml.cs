using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using Xbim.Ifc2x3.Kernel;
using System.IO;
using Xbim.Ifc2x3.Extensions;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions.Transactions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO;

namespace Xbim.IO.DynamicGrouping
{
    /// <summary>
    /// This class provides methods to perform the grouping of elements in the model.
    /// The resulting groups are populated in the model using IfcGroup and IFCRELASSIGNSTOGROUP;
    /// </summary>
    public class GroupingByXml
    {
        private XmlDocument _xmlDoc;
        private XbimModel _model;
        private IfcGroup _rootGroup;


        public GroupingByXml(XbimModel model)
        {
            _model = model;
        }

        /// <summary>
        /// Performs the grouping of elements of the model according to the rules defined in the XML pointed by <paramref name="XMLfileName"/>
        /// </summary>
        /// <param name="XMLfileName">Name of the xml file defining the grouping rules.</param>
        /// <returns></returns>
        public bool GroupElements(string XMLfileName, IfcGroup rootGroup = null)
        {
            if (string.IsNullOrEmpty(XMLfileName)) throw new ArgumentNullException("File name cannot be null or empty.");
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(XMLfileName);
           

            using (XbimReadWriteTransaction trans = _model.BeginTransaction())
            {
                if (rootGroup != null)
                {
                    if (((IPersistIfcEntity)rootGroup).ModelOf != _model) throw new Exception("Model of the group is different than model to be used.");
                    _rootGroup = rootGroup;
                }
                else
                {
                    _rootGroup = _model.Instances.New<IfcGroup>(g => g.Name = "Root group");
                }

                bool result = PerformGrouping();
                trans.Commit();
                return result;
            }
        }

        public bool GroupElements(XmlDocument document, IfcGroup rootGroup)
        {
            if (document == null) throw new ArgumentNullException("XML document must be specified");
            _xmlDoc = document;

            if (rootGroup != null)
            {
                if (((IPersistIfcEntity)rootGroup).ModelOf != _model) throw new Exception("Model of the group is different than model to be used.");
                _rootGroup = rootGroup;
            }
            else
            {
                _rootGroup = _model.Instances.New<IfcGroup>(g => g.Name = "Root group");
            }


            using (XbimReadWriteTransaction trans = _model.BeginTransaction("Elements to groups"))
            {
                bool result = PerformGrouping();
                trans.Commit();
                return result;
            }
        }

        private Type elemType;
        private string eName;
        string elTypeName;

        private string grpName;
        private TextWriter errLog;

        public TextWriter ErrorLog { get { return errLog; } }

        public void Load(XmlDocument document)
        {
            if (document == null) throw new ArgumentNullException("XML document must be specified");
            _xmlDoc = document;
            _rootGroup = _model.Instances.New<IfcGroup>(g => g.Name = "Root group");
        }

        /// <summary>
        /// Performs grouping based on the XML data provided.
        /// </summary>
        /// <param name="errLog">Stream for err output</param>
        /// <returns>True if grouping was successful. False otherwise.</returns>
        public bool PerformGrouping()
        {
            errLog = new StringWriter();

            if (_xmlDoc == null)
            {
                errLog.WriteLine("No input XML data.");
                return false;
            }

            //get all group nodes
            XmlNodeList groups = _xmlDoc.GetElementsByTagName("group");
            if (groups == null || groups.Count == 0)
            {
                errLog.WriteLine("No group rules in the XML document");
                return false;
            }

            //clear groups from elements
            ClearGroups(_rootGroup);


            //create group for all non-grouped elements
            IfcGroup noGroup = GetGroup("No group");
            if (noGroup == null)
            {
                noGroup = _model.Instances.New<IfcGroup>(gr => { gr.Name = "No group"; gr.Description = ""; });
                _rootGroup.AddObjectToGroup(noGroup);
            }
            List<IfcElement> allElements = _model.Instances.OfType<IfcElement>().ToList();
            List<IfcTypeObject> allTypes = _model.Instances.OfType<IfcTypeObject>().ToList();

            foreach (XmlNode group in groups)
            {
                grpName = GetName(group);
                if (String.IsNullOrEmpty(grpName))
                {
                    errLog.WriteLine("Group without name detected. All information in this group specification will be skipped.");
                    continue;
                }

                //search for the existing group with the same name (convert to upper case for the comparison) and create new group if none exists.
                IfcGroup iGroup = GetGroup(grpName);
                if (iGroup == null)
                {
                    iGroup = _model.Instances.New<IfcGroup>(gr => gr.Name = grpName);
                    _rootGroup.AddObjectToGroup(iGroup);
                }


                //process all ifc objects specified for the group
                XmlNodeList elements = ((XmlElement)group).GetElementsByTagName("element");
                foreach (XmlNode element in group)
                {
                    eName = GetName(element);
                    if (String.IsNullOrEmpty(eName))
                    {
                        errLog.WriteLine("Element without name detected in group '" + grpName + "'. This element will NOT be included in the");
                        continue;
                    }

                    //get elemType for reflection of attributes
                    IfcType ifcType;
                    if (!IfcMetaData.TryGetIfcType(eName, out ifcType))
                        continue;
                    else
                        elemType = ifcType.Type;
                    XbimQueryBuilder qBuilder = new XbimQueryBuilder(elemType);

                    //process all element attributes
                    XmlNodeList attributeGroups = ((XmlElement)element).GetElementsByTagName("attributes");
                    //there could be different grouping rules for attribute rules inside (all, none, oneOf, any)
                    foreach (XmlNode attrGroup in attributeGroups)
                    {
                        CreateAttributeCondition(qBuilder, attrGroup);
                    }

                    //process element properties
                    XmlNodeList propertyGroups = ((XmlElement)element).GetElementsByTagName("properties");
                    //there could be different grouping rules for attribute rules inside (all, none, oneOf, any)
                    foreach (XmlNode propGroup in propertyGroups)
                    {
                        CreatePropertyCondition(qBuilder, propGroup);
                    }


                    //element type processing
                    IEnumerable<IPersistIfcEntity> types = null;
                    XmlNodeList elTypeNodeList = (element as XmlElement).GetElementsByTagName("elementType");
                    foreach (XmlNode elTypeNode in elTypeNodeList) //there should be just one 'elTypeNode'
                    {
                        XbimQueryBuilder typeQueryBuilder = null;
                        elTypeName = elTypeNode.InnerText;
                        if (string.IsNullOrEmpty(elTypeName))
                        {
                            errLog.WriteLine("Name of the element type is not specified for element of type '" + eName + "'. Element type conditions will not be applied. ");
                            continue;
                        }

                        IfcType ifcTypeLookup;
                        if (!IfcMetaData.TryGetIfcType(eName, out ifcTypeLookup))
                            continue;
                        else
                            elemType = ifcTypeLookup.Type;
                        if (!typeof(IfcTypeObject).IsAssignableFrom(elemType))
                        {
                            errLog.WriteLine("'" + elTypeName + "' is not type object.");
                            continue;
                        }

                        typeQueryBuilder = new XbimQueryBuilder(elTypeName);

                        //type attributes
                        XmlNodeList typeAttrGroups = ((XmlElement)element).GetElementsByTagName("typeAttributes");
                        foreach (XmlNode typeAttrGrpNode in typeAttrGroups)
                        {
                            CreateAttributeCondition(typeQueryBuilder, typeAttrGrpNode);
                        }

                        //process element type properties
                        XmlNodeList typePropGroups = ((XmlElement)element).GetElementsByTagName("typeProperties");
                        //there could be different grouping rules for attribute rules inside (all, none, oneOf, any)
                        foreach (XmlNode propGroup in typePropGroups)
                        {
                            CreatePropertyCondition(typeQueryBuilder, propGroup);
                        }


                        types = _model.Instances.Where<IPersistIfcEntity>(typeQueryBuilder.BuildQuery());
                        break;
                    }

                    //get elements and element type
                    IEnumerable<IPersistIfcEntity> instances = allElements.Where(qBuilder.BuildQuery().Compile()).ToList();

                    //check elements against element type (if defined)
                    if (types != null)
                    {
                        instances = FilterElementsByType(instances, types);
                    }

                    //add result to the group
                    foreach (var inst in instances)
                    {
                        IfcElement el = inst as IfcElement;
                        if (el != null && allElements.Remove(el))
                        {
                            IfcTypeObject type = el.GetDefiningType();
                            if (allTypes.Remove(type)) //ensure that it is not added twice
                                iGroup.AddObjectToGroup(type);

                            //get all elements of this type
                            IEnumerable<IfcRelDefinesByType> rels = _model.Instances.Where<IfcRelDefinesByType>(r => r.RelatingType == type);
                            foreach (var rel in rels)
                            {
                                IEnumerable<IfcElement> elemOfType = rel.RelatedObjects.OfType<IfcElement>();
                                foreach (var item in elemOfType)
                                {
                                    allElements.Remove(item);
                                }
                            }
                            //IfcObjectDefinition objDef = inst as IfcObjectDefinition;
                            //if (objDef != null) iGroup.AddObjectToGroup(objDef);
                        }
                    }
                }
            }

            //fill the no-group group with elements which are not in any group
            foreach (IfcTypeObject element in allTypes) noGroup.AddObjectToGroup(element);
            return true;
        }

        private IEnumerable<IPersistIfcEntity> FilterElementsByType(IEnumerable<IPersistIfcEntity> elements, IEnumerable<IPersistIfcEntity> types)
        {
            //create lists from input arguments because enumeration crashes otherwise
            List<IPersistIfcEntity> elemList = elements.ToList();
            List<IPersistIfcEntity> typeList = types.ToList();

            foreach (var element in elemList)
            {
                IfcObject obj = element as IfcObject;
                if (obj != null)
                {
                    IfcTypeObject defType = obj.GetDefiningType();
                    if (defType != null)
                    {
                        if (typeList.Contains(defType)) yield return element;
                    }
                    else
                    {
                        yield return element;
                    }
                    
                }

            }
        }

        private void CreateAttributeCondition(XbimQueryBuilder qBuilder, XmlNode attributeGroupNode)
        {
            GroupRule grpRule = GetGroupRule(attributeGroupNode);
            //apply logical operators according to the "select"

            XmlNodeList attrs = ((XmlElement)attributeGroupNode).GetElementsByTagName("attribute");
            foreach (XmlNode attr in attrs)
            {
                //attribute name
                XmlNode nameNode = ((XmlElement)attr).GetElementsByTagName("name").Item(0);
                if (nameNode == null)
                {
                    errLog.WriteLine("Attribute with unspecified name in element '" + eName + "', group '" + grpName + "'");
                    continue;
                }
                string attName = nameNode.InnerText;
                PropertyInfo propInfo = elemType.GetProperty(attName);
                if (propInfo == null)
                {
                    errLog.WriteLine("Attribute with name '" + attName + "' doesn't exist in the element '" + eName + "' in group '" + grpName + "'");
                    continue;
                }

                //attribut value
                XmlNode valueNode = ((XmlElement)attr).GetElementsByTagName("value").Item(0);
                if (nameNode == null)
                {
                    errLog.WriteLine("Attribute with unspecified value in element '" + eName + "', group '" + grpName + "'");
                    continue;
                }
                string attValue = valueNode.InnerText;
                ValueRule attValRule = GetValueRule(valueNode);

                //create part of the expression
                qBuilder.AddAttributeCondition(attName, attValue, attValRule, grpRule);
            }
        }

        private void CreatePropertyCondition(XbimQueryBuilder qBuilder, XmlNode propGroupNode)
        {
            GroupRule pGrpRule = GetGroupRule(propGroupNode);
            //apply logical operators according to the "select"

            XmlNodeList pSets = ((XmlElement)propGroupNode).GetElementsByTagName("propertySet");
            foreach (XmlNode pSet in pSets)
            {
                string pSetName = GetName(pSet);
                NameRule pSetNameRule = GetNameRule(pSet);

                XmlNode propNode = ((XmlElement)pSet).GetElementsByTagName("property").Item(0);
                string propName = GetName(propNode);
                NameRule propNameRule = GetNameRule(propNode);
                if (string.IsNullOrEmpty(propName))
                {
                    errLog.WriteLine("Name of the property must be specified. Property set: '" + pSetName + "', element: '" + eName + "'.");
                    continue;
                }

                XmlNode valNode = (propNode as XmlElement).GetElementsByTagName("value").Item(0);
                string value = valNode.InnerText;
                ValueRule valRule = GetValueRule(valNode);

                if (string.IsNullOrEmpty(value))
                {
                    errLog.WriteLine("Value of the property '" + propName + "' must be specified. Property set: '" + pSetName + "', element: '" + eName + ".");
                    continue;
                }

                qBuilder.AddPropertyCondition(pSetName, pSetNameRule, propName, propNameRule, value, valRule, pGrpRule);
            }
        }

        private GroupRule GetGroupRule(XmlNode node)
        {
            XmlAttribute attr = node.Attributes["select"];
            string select = "all"; //default value
            if (attr != null && !String.IsNullOrEmpty(attr.Value))
            {
                select = attr.Value;
            }

            GroupRule result = GroupRule.ALL;
            if (select.ToLower() == "oneof") result = GroupRule.ONE_OF;
            if (select.ToLower() == "none") result = GroupRule.NONE;
            if (select.ToLower() == "any") result = GroupRule.ANY;

            return result;
        }

        private ValueRule GetValueRule(XmlNode node)
        {
            XmlAttribute attr = node.Attributes["type"];
            string type = "is"; //default value
            if (attr != null && !String.IsNullOrEmpty(attr.Value))
            {
                type = attr.Value;
            }

            ValueRule result = ValueRule.IS;
            if (type.ToLower() == "isnot") result = ValueRule.IS_NOT;
            if (type.ToLower() == "contains") result = ValueRule.CONTAINS;
            if (type.ToLower() == "notcontains") result = ValueRule.NOT_CONTAINS;
            if (type.ToLower() == "greater") result = ValueRule.GREATER_THAN;
            if (type.ToLower() == "less") result = ValueRule.LESS_THAN;

            return result;
        }

        private NameRule GetNameRule(XmlNode node)
        {
            XmlAttribute attr = node.Attributes["type"];
            string type = "is"; //default value
            if (attr != null && !String.IsNullOrEmpty(attr.Value))
            {
                type = attr.Value;
            }
            NameRule result = NameRule.IS;
            if (type.ToLower() == "isnot") result = NameRule.IS_NOT;
            if (type.ToLower() == "contains") result = NameRule.CONTAINS;
            if (type.ToLower() == "notcontains") result = NameRule.NOT_CONTAINS;

            return result;
        }


        private string GetName(XmlNode node)
        {
            XmlAttribute attr = node.Attributes["name"];
            if (attr != null)
            {
                string name = attr.Value;
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                return name.ToUpper();
            }
            return null;
        }


        private IfcGroup GetGroup(string name)
        {
            return GetGroup(name, _rootGroup);
        }

        //recursive function to find group with specified name in the scope of the root
        private IfcGroup GetGroup(string name, IfcGroup root)
        {
            if (root.Name == name) return root;
            else
            {
                IEnumerable<IfcGroup> children = root.GetGroupedObjects<IfcGroup>();
                foreach (IfcGroup group in children)
                {
                    IfcGroup result = GetGroup(name, group);
                    if (result != null)
                        return result;
                }
                return null;
            }
        }

        private void ClearGroups(IfcGroup root)
        {
            List<IfcElement> elements = root.GetGroupedObjects<IfcElement>().ToList();
            int count = elements.Count;
            
            //must use for cycle instead of foreach because enumeration would collapse
            for (int i = 0; i < count; i++)
            {
                root.RemoveObjectFromGroup(elements[i]);
            }

            //recursive call for children
            IEnumerable<IfcGroup> children = root.GetGroupedObjects<IfcGroup>();
            foreach (IfcGroup group in children)
            {
                ClearGroups(group);
            }
        }

    }
}
