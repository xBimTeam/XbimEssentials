using System;
using System.Diagnostics;
using System.IO;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.IO;
using Xbim.IO.Memory;

namespace Profiling.Parsing
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string name = "SampleHouse.ifc";
            const string name = "Lakeside.ifc";
            var w = new Stopwatch();
            
            using (var model = new XbimModel())
            {
                w.Start();
                model.CreateFrom(name, null, null, true, true);
                //model.CreateFrom(name);
                w.Stop();
                Console.WriteLine("{0}ms to create Esent model", w.ElapsedMilliseconds);
                model.Close();
            }
            
            //using (var model = new XbimModel())
            //{
            //    w.Restart();
            //    model.Open(Path.ChangeExtension(name, ".xbim"));
            //    w.Stop();
            
            //    Console.WriteLine("{0}ms to open Esent model", w.ElapsedMilliseconds);
            //    model.Close();
            //}


            w.Restart();
            var model2 = new MemoryModel<EntityFactory>();
            model2.Open(name);
            w.Stop();
            Console.WriteLine("{0}ms to load memory model", w.ElapsedMilliseconds);

            //Console.ReadLine();
        }
    }
}
