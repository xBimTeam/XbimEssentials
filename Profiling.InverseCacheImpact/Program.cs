using System;
using System.Diagnostics;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;

namespace Profiling.InverseCacheImpact
{
    internal class Program
    {
        private static void Main()
        {
            //6574ms to find 4 objects with speficic type without cache.              ---> baseline
            //6586ms to find 4 objects with speficic type with cache.                 ---> creating cache didn't cost almost any CPU
            //4ms to find 4 objects with speficic type with cache, repeated query.    ---> repeated request is BAZING fast

            using (var model = IfcStore.Open(@"c:\CODE\SampleData\LakesideRestaurant\LakesideRestaurant.ifc"))
            {
                var w = Stopwatch.StartNew();
                var objects1 =
                    model.Instances.Where<IfcObject>(
                        o =>
                            o.IsDefinedBy.OfType<IfcRelDefinesByType>()
                                .Any(r => r.RelatingType.Name == "Basic Wall:nbl_Int_PlstrbrdGyp-GalvStlStud-PlstrbrdGyp")).ToList();
                w.Stop();
                Console.WriteLine(@"{0}ms to find {1} objects with speficic type without cache.", w.ElapsedMilliseconds, objects1.Count);
                using (model.BeginCaching())
                {
                    w.Restart();
                    var objects2 =
                        model.Instances.Where<IfcObject>(
                            o =>
                                o.IsDefinedBy.OfType<IfcRelDefinesByType>()
                                    .Any(r => r.RelatingType.Name == "Basic Wall:nbl_Int_PlstrbrdGyp-GalvStlStud-PlstrbrdGyp")).ToList();
                    w.Stop();
                    Console.WriteLine(@"{0}ms to find {1} objects with speficic type with cache.", w.ElapsedMilliseconds, objects2.Count);
                    w.Restart();
                    var objects3 =
                        model.Instances.Where<IfcObject>(
                            o =>
                                o.IsDefinedBy.OfType<IfcRelDefinesByType>()
                                    .Any(r => r.RelatingType.Name == "Basic Wall:nbl_Int_PlstrbrdGyp-GalvStlStud-PlstrbrdGyp")).ToList();
                    w.Stop();
                    Console.WriteLine(@"{0}ms to find {1} objects with speficic type with cache, repeated query.", w.ElapsedMilliseconds, objects3.Count);
                }
            }
        }
    }
}
