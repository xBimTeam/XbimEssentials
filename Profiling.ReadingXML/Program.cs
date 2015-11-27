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
            const string path = "profiling.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                w.Start();
                model.OpenXml(path);
                w.Stop();
                Console.WriteLine(@"{0}ms to open model from XML file.", w.ElapsedMilliseconds);
            }
        }
    }
}
