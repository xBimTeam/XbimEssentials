using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO.Tree
{
    public class TreeComponentStrategy : TreeQueryStrategy
    {
        public TreeComponentStrategy(IModel model)
        {
            _model = model;
        }

        public override TreeNodes GetTreeStructure()
        {
            return GetComponentStructure();
        }

        /// <summary>
        /// Groups all elements by their Family Type
        /// </summary>
        /// <returns></returns>
        private TreeNodes GetComponentStructure()
        {
            TreeNodes tree = new TreeNodes();

            var familyTypes = from t in GetFamilyElements() orderby t.Name select t;

            foreach (Type type in familyTypes)
            {
                FamilyNode family = new FamilyNode();
                family.Name = type.Name;

                LoadFamilyElements(type, family);

                tree.Add(family);
            }

            return tree;
        }
    }
}
