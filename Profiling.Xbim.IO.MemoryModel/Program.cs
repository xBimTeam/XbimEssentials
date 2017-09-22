using System;
using System.Diagnostics;
using System.IO;
using Xbim.IO.Memory;

namespace Profiling.Xbim.IO
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var sw = new Stopwatch();
            sw.Start();
            using (var file = File.OpenRead(fileName))
            using (var mm = MemoryModel.OpenReadStep21(file))
            {
               
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
           // Console.ReadLine();
        }
    }
}
