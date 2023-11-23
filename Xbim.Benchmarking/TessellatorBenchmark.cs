using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Tessellator;

namespace Xbim.Benchmarking
{
    //[EtwProfiler]
    [MemoryDiagnoser]
    //[ThreadingDiagnoser]
    public class TessellatorBenchmark
    {
        private IModel? model;

        // Determines the number of iterations to invoke to create enough measureable work. Ideally need > 100us
        const int Iterations = 100;

        /// <summary>
        /// The model files we'll test with. Each benchmark will be run for each IFC file.
        /// </summary>
        [Params( "IFC4TessellationComplex.ifc",  "BasinTessellation.ifc")]
        public string ifcFile = "";

        [Benchmark(Baseline = true, OperationsPerInvoke = Iterations)]
        public long Baseline()
        {
            return DoTessellation();
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public long WithoutNormalCorrection()
        {
            return DoTessellation(false);
        }

        [Benchmark(OperationsPerInvoke = Iterations)]
        public long WithoutAnyCorrections()
        {
            return DoTessellation(false, false);
        }


        [IterationSetup]
        public void Setup()
        {
            model = IfcStore.Open(Path.Combine(@"TestFiles", ifcFile));
        }

        [IterationCleanup]
        public void CleanUp()
        {
            model?.Dispose();
        }

        private long DoTessellation(bool optimiseNormals = true, bool unifyFaces = true)
        {
           
            var representations = model!.Instances.OfType<IIfcRepresentationItem>();
            long cost = 0;
            var tessellator = new XbimTessellator(model, XbimGeometryType.PolyhedronBinary,  balanceNormals: optimiseNormals, unifyFaces: unifyFaces);
            for(int i = 0; i < Iterations; i++)
            {
                foreach(var rep in representations)
                {
                    if (tessellator.CanMesh(rep))
                    {
                        var geom = tessellator.Mesh(rep);
                        cost+= geom.Cost;
                    }
                }
            }
            return cost;
            
        }
    }
}
