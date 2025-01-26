using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.NetCore.Tests
{
   
    public class GithubTests
    {

        [Fact]
        public void GithubIssue_595_Fails_With_LinqSelectMany()
        {
            // In .net core only, a SelectMany<TSource, TResult> projection could produce a result containing null values when none were null
            // in the TSource set. Only the last projected Items would remain in the TResults collection. When calling ToArray/ToList.
            // This was because when using Ifc4 (cross-schema) interfaces, and projecting and casting (to the IIfcProperty interface) in
            // Netcore SelectMany<T1,T2> this invokes List<T1>.AddRange which in turn called
            // xbim's ProxyItemsSet.CopyTo(T[] targetArray, int offset) which was incorrectly implemented


            using var model = new MemoryModel(new EntityFactoryIfc4());

            InitialiseIfc4Model(model);

            // The issues below disappear when using IfcMaterial directly here
            var imaterial = model.Instances.OfType<IIfcMaterial>().First();


            // ACT

            
            List<IIfcProperty> flattenedProperties = imaterial.HasProperties.SelectMany(a => a.Properties).ToList();

            // Assert

            flattenedProperties.Should().HaveCount(2);

            flattenedProperties.Should().NotContainNulls();
        }

     
        private static void InitialiseIfc4Model(MemoryModel model)
        {
            using (var tx = model.BeginTransaction("memory"))
            {
                var material = model.Instances.New<IfcMaterial>(m => m.Name = "TestMaterial");

                model.Instances.New<IfcMaterialProperties>(ps =>
                {
                    ps.Name = "A";
                    ps.Material = material;
                    ps.Properties.Add(
                        model.Instances.New<IfcPropertySingleValue>(pv =>
                        {
                            pv.Name = "A1";
                            pv.NominalValue = new IfcMolecularWeightMeasure(1);
                        })
                    );
                });

                model.Instances.New<IfcMaterialProperties>(ps =>
                {
                    ps.Name = "B";
                    ps.Material = material;
                    ps.Properties.Add(
                        model.Instances.New<IfcPropertySingleValue>(pv =>
                        {
                            pv.Name = "B1";
                            pv.NominalValue = new IfcThermalConductivityMeasure(2);
                        })
                    );
                });

                tx.Commit();
            }
        }
    }

    public static class Enumerable2
    {
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (TSource item in source)
            {
                foreach (TResult item2 in selector(item))
                {
                    yield return item2;
                }
            }
        }
    }

}
