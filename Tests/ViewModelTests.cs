using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc.ViewModels;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        

        [TestMethod]
        public void Can_CreateGroupViewModel()
        {
            const string file = "TestFiles\\SampleHouse4.ifc";
            using var model = IfcStore.Open(file);

            var componentTypeGroup = TreeViewBuilder.ComponentView(model);

            componentTypeGroup.Should().NotBeNull();


            componentTypeGroup.Should().NotContain(c => c.Name == "OpeningElement");
            componentTypeGroup.Should().Contain(c => c.Name == "Space");

            componentTypeGroup.Should().HaveCount(12);
        }

        [TestMethod]
        public void Can_CreateContainmentViewModel()
        {
            const string file = "TestFiles\\SampleHouse4.ifc";
            using var model = IfcStore.Open(file);

            var groupvm = TreeViewBuilder.ContainmentView(model);

            groupvm.Should().NotBeNull();
            groupvm.Should().HaveCount(1, "One project");
            var modelVm = groupvm.First();
            modelVm.Should().BeOfType<XbimModelViewModel>();
            modelVm.Children.Should().HaveCount(1, "one site");
            var site = modelVm.Children.First();
            site.Should().BeOfType<SpatialViewModel>();
            site.Children.Should().HaveCount(1, "one building");
            var building = site.Children.First();
            building.Should().BeOfType<SpatialViewModel>();
            building.Children.Should().HaveCount(2, "two storeys");
        }
    }
}
