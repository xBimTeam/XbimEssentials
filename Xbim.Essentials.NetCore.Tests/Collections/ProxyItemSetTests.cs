using FluentAssertions;
using System;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Collections;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.NetCore.Tests.Collections
{
    public class ProxyItemSetTests
    {
        [Fact]
        public void CanCopyToGenericArray()
        {
            using var model = new MemoryModel(new EntityFactoryIfc4());

            ProxyItemSet<IfcMaterial, IIfcMaterial> set1 = BuildSet(model, 2);
            ProxyItemSet<IfcMaterial, IIfcMaterial> set2 = BuildSet(model, 3,2);

            IIfcMaterial[] array = new IIfcMaterial[5];
            set1.CopyTo(array, 0);
            set2.CopyTo(array, set1.Count());

            array.Should().NotContainNulls();

            int i = 0;
            do
            {
                array[i].Name.ToString().Should().EndWith($"{i}");
                i++;
            } while (i < array.Length);

        }

        private static ProxyItemSet<IfcMaterial, IIfcMaterial> BuildSet(MemoryModel model, int count, int start = 0)
        {
            TestItemSet<IfcMaterial> collection = null;
            using (var tx = model.BeginTransaction("memory"))
            {
                var parent = model.Instances.New<IfcWall>(m => m.Name = $"Parent");
                collection = new TestItemSet<IfcMaterial>(parent, count, 0);
                for (int i = 0; i < count; i++)
                {
                    var item = model.Instances.New<IfcMaterial>(m => m.Name = $"Item {start + i}");
                    collection.Add(item);
                }

                tx.Commit();
            }
           
            var set1 = new ProxyItemSet<IfcMaterial, IIfcMaterial>(collection);
            return set1;
        }
    }

    internal class TestItemSet<T> : Xbim.Common.Collections.ItemSet<T>
    {
        public TestItemSet(IPersistEntity parent, int capacity, int property) :  base(parent, capacity, property)
        { }
    }
}
