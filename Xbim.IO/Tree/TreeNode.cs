using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Extensions;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.IO.Tree
{
    public class TreeNode
    {
        public TreeNode(IfcRoot entity, bool isRoot, bool isLeaf)
        {
            IsRoot = isRoot;
            IsLeaf = isLeaf;
            if (!IsLeaf)
            {
                Children = new TreeNodes();
            }

            if (entity != null)
            {
                SetDefaults(entity);
            }
        }

        public TreeNodes Children;

        public bool IsRoot
        {
            get;
            internal set;
        }

        public bool IsLeaf
        {
            get;
            protected set;
        }

        public string Name
        {
            get;
            set;
        }

        public String IfcType
        {
            get;
            set;
        }

        public long EntityId
        {
            get;
            set;
        }

        public virtual bool HasChildren()
        {
            return ((!IsLeaf) && Children.Count > 0);
        }

        private void SetDefaults(IfcRoot entity)
        {
            Name = entity.CreateFriendlyName();
            EntityId = entity.EntityLabel;

            if (entity is IfcObjectDefinition)
            {
                IfcType = entity.GetType().Name;
            }
            else if (entity is IfcRelDecomposes)
            {
                IfcRelDecomposes rel = entity as IfcRelDecomposes;
                IfcType = rel.RelatingObject.GetType().Name;
            }

        }

    }

    /// <summary>
    /// Defines a Family of Elements. e.g IfcRoof
    /// </summary>
    public class FamilyNode : TreeNode
    {
        public FamilyNode()
            : base(null, true, false)
        {
            EntityId = 0;
        }
    }

    /// <summary>
    /// Defines a Container Structure of Elements. i.e Decomposition of Site - Building - Floor etc
    /// </summary>
    public class CompositionNode : TreeNode
    {
        public CompositionNode(IfcRoot entity)
            : base(entity, false, false)
        {

        }
    }

    /// <summary>
    /// Defines a element Instance. Always a leaf
    /// </summary>
    public class ElementNode : TreeNode
    {
        public ElementNode(IfcRoot entity)
            : base(entity, false, true)
        { }
    }
}
