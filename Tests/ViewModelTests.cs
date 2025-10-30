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

            var level = building.Children.First();
            level.Should().BeOfType<SpatialViewModel>();
            level.Children.Should().HaveCount(9, "children in Level");

            level.Children.Take(3).Should().AllBeOfType<SpatialViewModel>();
            level.Children.Skip(3).Should().AllBeOfType<ContainedElementsViewModel>();

            var wallGroup = level.Children.Skip(7).First();
            wallGroup.Name.Should().Be("Wall");
            wallGroup.Children.Should().HaveCount(5);
            wallGroup.Children.Take(3).Should().AllSatisfy(e => e.Name.Should().Contain("Wall #"));
            wallGroup.Children.Skip(3).Should().AllSatisfy(e => e.Name.Should().Contain("WallStandardCase #"));

        }
    }
}
