using System;
using System.Diagnostics;
using System.IO;
using Xbim.Ifc;
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
            var modelName = "Lakeside.ifc";
            //var modelName = @"c:\CODE\SampleData\UniversityOfAuckland\20160125WestRiverSide Hospital - IFC4-Autodesk_Hospital_Metric_Electrical.ifc";
            //var modelName = @"c:\CODE\SampleData\UniversityOfAuckland\20160125WestRiverSide Hospital - IFC4-Autodesk_Hospital_Metric_Plumbing.ifc";
            if (args.Length > 0)
            {
                if(File.Exists(args[0]))
                    modelName = args[0];
            }
            var w = new Stopwatch();
            w.Start();
            using (var model =  IfcStore.Open(modelName,null,-1))
            {               
                model.Close();
                w.Stop();
                Console.WriteLine(@"{0:F2} ms to create in memory model", w.ElapsedMilliseconds);
            }
            w.Restart();;
            using (var model = IfcStore.Open(modelName, null, 0))
            {          
                model.Close();
                w.Stop();
                Console.WriteLine(@"{0:F2} ms to create database model", w.ElapsedMilliseconds);
            }
            Console.WriteLine(@"Press any key to exit");
            Console.ReadKey();
        }
    }
}
