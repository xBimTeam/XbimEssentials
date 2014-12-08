using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;

namespace Xbim.IO.ViewModels
{
    public class TreeViewBuilder
    {
        public static List<IXbimViewModel> ContainmentView(XbimModel model)
        {
            List<IXbimViewModel> svList = new List<IXbimViewModel>();

            IfcProject project = model.IfcProject as IfcProject;
            if (project != null)
            {
                //svList.Add(new SpatialViewModel(project));
                svList.Add(new XbimModelViewModel(project, null));
                foreach (var child in svList)
                {
                    LazyLoadAll(child);
                }

            }
            return svList;
        }

        public static List<IXbimViewModel> ComponentView(XbimModel model)
        {
            List<IXbimViewModel> eleList = new List<IXbimViewModel>();
            var spaces = typeof(IfcSpace).Name;
            var types = model.IfcProducts.Select(itm => itm.GetType())
                                          .Distinct()
                                          .Where (itm => itm.IsSubclassOf(typeof(IfcElement)) || itm.Name == spaces)
                                          .OrderBy(itm => itm.Name)
                                          .ToArray();

            foreach (Type type in types)
            {
                eleList.Add(new TypeViewModel(type, model));
            }

            foreach (var child in eleList)
            {
                LazyLoadAll(child);
            }

            return eleList;
        }

        private static void LazyLoadAll(IXbimViewModel parent)
        {

            foreach (var child in parent.Children)
            {
                LazyLoadAll(child);
            }

        }
    }
}
