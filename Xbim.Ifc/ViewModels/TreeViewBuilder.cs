using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class TreeViewBuilder
    {
        public static List<IXbimViewModel> ContainmentView(IModel model)
        {
            var svList = new List<IXbimViewModel>();

            var project = model.Instances.FirstOrDefault<IIfcProject>();
            if (project == null) return svList;

            svList.Add(new XbimModelViewModel(project, null));
            foreach (var child in svList)
            {
                LazyLoadAll(child);
            }
            return svList;
        }

        public static List<IXbimViewModel> ComponentView(IModel model)
        {
            var spaces = typeof(IIfcSpace).Name;
            var types = model.Instances.OfType<IIfcProduct>()
                .Select(itm => itm.GetType())
                .Distinct()
                .Where (itm => itm.IsSubclassOf(typeof(IIfcElement)) || itm.Name == spaces)
                .OrderBy(itm => itm.Name)
                .ToArray();

            var eleList = types.Select(type => new TypeViewModel(type, model)).Cast<IXbimViewModel>().ToList();

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
