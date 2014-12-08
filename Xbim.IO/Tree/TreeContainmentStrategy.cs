using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO.Tree
{
    public class TreeContainmentStrategy : TreeQueryStrategy
    {
        public TreeContainmentStrategy(IModel model)
        {
            _model = model;
        }

        public override TreeNodes GetTreeStructure()
        {
            return GetContainmentStructure();
        }

        /// <summary>
        /// Creates a Hierarchical structure reflecting the containment structure of the model.
        /// </summary>
        /// <returns></returns>
        private TreeNodes GetContainmentStructure()
        {
            Initialise();

            TreeNodes root = new TreeNodes();
            AddRelComposes();
            AddRelContained();

            root.Add(LocateProjectNode());

            // Clear?
            return root;
        }
    }
}
