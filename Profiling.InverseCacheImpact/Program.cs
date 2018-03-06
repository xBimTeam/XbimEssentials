using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Profiling.InverseCacheImpact
{
    internal class Program
    {
        private static void Main()
        {
            //6574ms to find 4 objects with speficic type without cache.              ---> baseline
            //6586ms to find 4 objects with speficic type with cache.                 ---> creating cache didn't cost almost any CPU
            //4ms to find 4 objects with speficic type with cache, repeated query.    ---> repeated request is BAZING fast

            //using (var model = IfcStore.Open(@"c:\Users\Martin\Source\Samples\2011-09-14-Clinic-IFC\Clinic_MEP_20110906.ifc", null, -1))
            var w = Stopwatch.StartNew();
            using (var model = MemoryModel.OpenRead(@"..\..\..\Profiling.Parsing\Lakeside.ifc"))
            {
                w.Stop();
                Console.WriteLine("{0}ms to open the file", w.ElapsedMilliseconds);
                w.Restart();
                var objects1 =
                    model.Instances.Where<IfcObject>(
                        o =>
                            o.IsDefinedBy.OfType<IfcRelDefinesByType>()
                                .Any(r => r.RelatingType.Name == "Basic Wall:nbl_Int_PlstrbrdGyp-GalvStlStud-PlstrbrdGyp")).ToList();
                w.Stop();
                Console.WriteLine(@"{0}ms to find {1} objects with speficic type without cache.", w.ElapsedMilliseconds, objects1.Count);
                using (var cache = model.BeginInverseCaching())
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

                    w.Restart();
                    var pSets = model.Instances.
                        OfType<IfcObject>().
                        SelectMany(o => o.
                            IsDefinedBy.
                            OfType<IfcRelDefinesByProperties>().
                            Select(r => r.RelatingPropertyDefinition)).
                        ToList();
                    w.Stop();
                    Console.WriteLine(@"{0}ms to get all psets related to all objects.", w.ElapsedMilliseconds);
                }
            }
        }
    }
}
