using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.IO.Tree
{
    public class TreeBuilder
    {
        public static TreeNodes BuildTreeStructure(TreeQueryStrategy strategy)
        {
            return strategy.GetTreeStructure();
        }
    }
}
