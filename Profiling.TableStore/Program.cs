using System;
using System.Diagnostics;
using Xbim.CobieExpress.IO;
using Xbim.IO.TableStore;

namespace Profiling.TableStore
{
    internal class Program
    {
        private static void Main()
        {
            Test2();
        }

        private static void Test1()
        {
            var model = CobieModel.OpenStep21Zip("..\\..\\..\\Xbim.IO.Tests\\TestFiles\\LakesideRestaurant.cobieZip");
            //var mapping = GetSimpleMapping();
            var mapping = ModelMapping.Load(Xbim.CobieExpress.IO.Properties.Resources.COBieUK2012);
            mapping.Init(model.Metadata);

            var w = new Stopwatch();
            w.Start();
            var storage = new Xbim.IO.TableStore.TableStore(model, mapping);
            storage.Store("Result.xlsx");
            w.Stop();
            Console.WriteLine(@"{0}ms to store the tables.", w.ElapsedMilliseconds);
        }

        private static void Test2()
        {
            string report;
            var w = new Stopwatch();
            w.Start();
            var model = CobieModel.ImportFromTable("Result.xlsx", out report);
            w.Stop();
            var count = model.Instances.Count;
            Console.WriteLine(@"{0}ms to load {1} objects from the tables.", w.ElapsedMilliseconds, count);

        }
    }
}
