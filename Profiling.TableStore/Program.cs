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
    }
}
