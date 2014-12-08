using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.IO.Tree
{
    public abstract class TreeQueryStrategy
    {
        protected IModel _model;
        protected Dictionary<IfcObjectDefinition, CompositionNode> _nodeMap;

        public Dictionary<IfcObjectDefinition, CompositionNode> NodeMap
        {
            get
            {
                return _nodeMap;
            }
        }

        public IModel Model
        {
            get
            {
                return _model;
            }
        }

        public virtual TreeNodes GetTreeStructure()
        {
            return new TreeNodes();
        }

        protected void Initialise()
        {
            _nodeMap = new Dictionary<IfcObjectDefinition, CompositionNode>
                    (_model.Instances.OfType<IfcRelDecomposes>().Count());
        }

        protected CompositionNode LocateProjectNode()
        {
            CompositionNode root = null;

            IfcProject project = _model.Instances.OfType<IfcProject>().FirstOrDefault();
            if (project != null)
            {
                CompositionNode projectNode;
                if (NodeMap.TryGetValue(project, out projectNode))
                {
                    projectNode.IsRoot = true;
                    root = projectNode;
                }
            }
            //if (root == null)
            //{
            //    Trace.TraceWarning("No IfcProject located in Model");
            //}
            return root;
        }

        protected Type[] GetFamilyElements()
        {
            var spaces = typeof(Xbim.Ifc2x3.ProductExtension.IfcSpace).FullName;
            return _model.Instances.OfType<IfcProduct>().Where(itm => itm.GetType().IsSubclassOf(typeof(IfcElement)) || itm.GetType().FullName == spaces)
                                           .Select(itm => itm.GetType())
                                           .Distinct()
                                           .ToArray();
        }

        protected void LoadFamilyElements(Type familyType, FamilyNode family)
        {
            var products = from prod in _model.Instances.Where<IfcProduct>(p => p.GetType().IsAssignableFrom(familyType))
                           //orderby prod.Name
                           select prod;

            foreach (IfcProduct product in products)
            {
                ElementNode element = new ElementNode(product);
                family.Children.Add(element);
            }
        }

        protected void AddRelComposes()
        {
            foreach (IfcRelDecomposes rel in _model.Instances.OfType<IfcRelDecomposes>())
            {
                if (rel.RelatingObject != null)
                {
                    // Get the subject of the decomposition from the relationship
                    var parentRel = rel.RelatingObject;
                    if (parentRel == null)
                    { continue; }
                    //var parent = parentRel.RelatingObject;

                    CompositionNode treeItem;
                    if (!NodeMap.TryGetValue(parentRel, out treeItem))
                    {
                        treeItem = new CompositionNode(parentRel);
                        NodeMap.Add(parentRel, treeItem);
                    }
                    AddRelatedObjects(rel, treeItem);
                }
            }
        }

        protected void AddRelatedObjects(IfcRelDecomposes rel, CompositionNode treeItem)
        {
            foreach (IfcObjectDefinition child in rel.RelatedObjects)
            {
                if (child.EntityLabel == treeItem.EntityId) { continue; }//prevent any infinite looping
                CompositionNode childItem;
                if (!NodeMap.TryGetValue(child, out childItem)) //already written
                {
                    childItem = new CompositionNode(child);
                    NodeMap.Add(child, childItem);

                }
                treeItem.Children.Add(childItem);

            }
        }

        protected void AddRelContained()
        {

            foreach (IfcRelContainedInSpatialStructure scRel in
                _model.Instances.OfType<IfcRelContainedInSpatialStructure>())
            {
                if (scRel.RelatingStructure != null)
                {
                    CompositionNode treeItem;
                    if (!NodeMap.TryGetValue(scRel.RelatingStructure, out treeItem)) //already written
                    {
                        treeItem = new CompositionNode(scRel.RelatingStructure);
                        NodeMap.Add(scRel.RelatingStructure, treeItem);

                    }
                    AddRelatedElements(scRel, treeItem);
                }
            }
        }

        protected void AddRelatedElements(IfcRelContainedInSpatialStructure scRel, CompositionNode treeItem)
        {
            var applicableTypes = scRel.RelatedElements
                .Where(t => !t.GetType().IsSubclassOf(typeof(IfcFeatureElementSubtraction)));
            foreach (IfcObjectDefinition child in applicableTypes)
            {
                CompositionNode childItem;
                if (!NodeMap.TryGetValue(child, out childItem)) //already written
                {
                    childItem = new CompositionNode(child);
                    NodeMap.Add(child, childItem);

                }
                //Node family = GetFamily(treeItem, child);
                // TODO: Add child to family, not treeItem
                treeItem.Children.Add(childItem);
            }
        }
    }
}
