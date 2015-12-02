using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc4;
using Xbim.IO.Memory;

namespace Profiling.ReadingXML
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new Stopwatch();
            const string path = @"c:\CODE\XbimGit\XbimEssentials\TestResults\SampleHouse4.xml"; //"profiling.xml";
            using (var model = new MemoryModel(new EntityFactory()))
            {
                w.Start();
                model.LoadXml(path);
                w.Stop();
                Console.WriteLine(@"{0}ms to open model from XML file.", w.ElapsedMilliseconds);
            }
        }
    }
}
