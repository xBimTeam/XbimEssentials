using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;
using Xbim.Benchmarking;

class Program
{
    static void Main(string[] args)
#if DEBUG
    {
        // Just used to test the benchmark code. 
        // Benchmarking should never be run in Debug

        WriteWarning("Running in DEBUG is only intended for testing the infrastructure. This does not benchmark anything!");
        var test = new TessellatorBenchmark();
        test.ifcFile= @"IFC4TessellationComplex.ifc";
        test.Setup();
        var result = test.TesselateWithoutNormalOptimisation();
        test.CleanUp();
    }
#else
    {
        if(System.Diagnostics.Debugger.IsAttached)
        {
            WriteWarning("Do not run under a debugger, even in RELEASE!  Run 'RunBenchmark.bat' to start. See https://benchmarkdotnet.org/articles/guides/good-practices.html ");
        }
        // The typical execution path: Runs the Benchmarks using the Release runner etc.
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args,
                DefaultConfig.Instance
            .AddDiagnoser(MemoryDiagnoser.Default)
            //.AddDiagnoser(ThreadingDiagnoser.Default)
            //.AddDiagnoser(new EtwProfiler())
            );
    }
#endif

    static void WriteWarning(string error)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Error.WriteLine(error);
        Console.ForegroundColor = original;
    }
}