using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Xbim.XbimExtensions;
using System.IO;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.Extensions;
using Xbim.XbimExtensions.Transactions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO;

namespace Xbim.IO.DynamicGrouping
{
    /// <summary>
    /// Loads an IfcGroup hierarcy from an XML
    /// </summary>
    public class GroupsFromXml
    {
        private XmlDocument _xmlDoc;
        private XbimModel _model;
        private TextWriter errLog;
        private int _numCreated = 0;
        public TextWriter ErrorLog { get { return errLog; } }
        public int NumCreatedGroups { get { return _numCreated; } }
        public List<IfcGroup> RootGroups { get; set; }

        public GroupsFromXml(XbimModel model)
        {
            errLog = new StringWriter();
            _model = model;
            RootGroups = new List<IfcGroup>();
        }

        public void CreateGroups(XmlDocument xmlDocument)
        {
            _xmlDoc = xmlDocument;
            using (XbimReadWriteTransaction trans = _model.BeginTransaction())
            {
                Process();
                trans.Commit();
            }
        }

        public void CreateGroups(string xmlFileName)
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(xmlFileName);
            Process();
        }

        public IfcGroup GetExistingRootGroup(XmlDocument xmlDocument)
        {
            _xmlDoc = xmlDocument;
            if (_xmlDoc == null)
            {
                errLog.WriteLine("No input XML data.");
                return null;
            }

            //get all groups nodes
            XmlNodeList groups = _xmlDoc.GetElementsByTagName("my:groups");
            if (groups == null || groups.Count == 0)
            {
                errLog.WriteLine("No groups in the XML document - not even root element.");
                return null;
            }

            foreach (XmlNode group in groups)
            {
                string className = (group as XmlElement).GetAttribute("my:classificationName");
                IfcGroup root = _model.Instances.Where<IfcGroup>(g => g.Name == className).FirstOrDefault();
                if (root != null)
                {
                    return root;
                }
            }
            return null;
        }

        private void Process()
        {
            if (_xmlDoc == null)
            {
                errLog.WriteLine("No input XML data.");
                return;
            }

            //get all groups nodes
            XmlNodeList groups = _xmlDoc.GetElementsByTagName("my:groups");
            if (groups == null || groups.Count == 0)
            {
                errLog.WriteLine("No groups in the XML document - not even root element.");
                return;
            }

            //start xbim transaction
            using (XbimReadWriteTransaction transaction = _model.BeginTransaction())
            {

                foreach (XmlNode group in groups)
                {
                    string className = (group as XmlElement).GetAttribute("my:classificationName");
                    IfcGroup root = CreateGroup(className, "", null);

                    //create group for all non-grouped elements
                    CreateGroup("No group", "", root);

                    XmlNodeList groupList = group.ChildNodes;
                    if (groupList.Count == 0)
                    {
                        errLog.WriteLine("No groups in the XML document");
                        continue;
                    }
                    ProcessGroups(groupList, root);
                }

                transaction.Commit();
            }
        }

        //recursive function for groups creation
        private void ProcessGroups(XmlNodeList groupNodeList, IfcGroup parentGroup)
        {
            foreach (XmlNode groupNode in groupNodeList)
            {
                //check node type
                if (groupNode.Name != "my:group")
                {
                    errLog.WriteLine("Unexpected node: " + groupNode.Name);
                    continue;
                }

                XmlElement groupElement = groupNode as XmlElement;
                string name = groupElement.GetAttribute("my:name");
                if (string.IsNullOrEmpty(name))
                {
                    errLog.WriteLine("Group with no name identified.");
                    continue;
                }

                string classification = groupElement.GetAttribute("my:classification");
                if (string.IsNullOrEmpty(classification))
                {
                    errLog.WriteLine("Group '"+name+"' does not have any classification code assigned.");
                }

                //create group
                IfcGroup group = CreateGroup(name, classification, parentGroup);

                //recursive call for the child nodes
                XmlNodeList children = groupNode.ChildNodes;
                if (children.Count != 0) 
                    ProcessGroups(children, group);
            }
        }

        //create group and add it into the group hierarchy (top -> down creation)
        private IfcGroup CreateGroup(string groupName, string classification, IfcGroup parentGroup)
        {
            IfcGroup group = _model.Instances.Where<IfcGroup>(g => g.Name == groupName).FirstOrDefault();
            if (group == null) group = _model.Instances.New<IfcGroup>(g => { g.Name = groupName; g.Description = classification; });

            if (parentGroup != null)
            {
                //check if it is not already child group.
                IfcGroup child = parentGroup.GetGroupedObjects<IfcGroup>().Where(g => g.Name == groupName).FirstOrDefault();
                if (child == null)
                    //ad if it is not
                    parentGroup.AddObjectToGroup(group);
            }

            //add to the root groups if this is root (there is no parent group)
            if (parentGroup == null)
                RootGroups.Add(group);

            _numCreated++;
            return group;
        }


    }
}
