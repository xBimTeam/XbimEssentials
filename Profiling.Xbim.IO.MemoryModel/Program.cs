using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Common.Step21;
using Xbim.Ifc4;
using Xbim.Ifc2x3.TopologyResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.IO.Memory;
using Xbim.IO.Parser;
using Xbim.IO.Step21;
using ValueType = Xbim.IO.Parser.ValueType;
using Xbim.Ifc4.Interfaces;

namespace Profiling.Xbim.IO
{
    class Program
    {
        static void Main(string[] args)
        {
            var ignoreNamespaces = new[] { "GeometricConstraintResource", "GeometricModelResource", "GeometryResource", "ProfileResource", "TopologyResource", "RepresentationResource" };
            var ignoreTypes = typeof(EntityFactory)
                .Assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && ignoreNamespaces.Any(ns => t.Namespace.EndsWith(ns)))
                .Select(t => t.Name.ToUpperInvariant())
                .ToList();

            var fileName = @"c:\Users\Martin\Source\Samples\LakesideRestaurant.ifc";
            var sw = Stopwatch.StartNew();
            var count = 0L;
            var wallCount = 0;
            var relProps = 0;
            using (var file = File.OpenRead(fileName))
            using (var mm = MemoryModel.OpenReadStep21(file, null, null, ignoreTypes))
            {
                sw.Stop();

                count = mm.Instances.Count;
                wallCount = mm.Instances.OfType<IIfcWall>().Count();
                relProps = mm.Instances.OfType<IIfcRelDefinesByProperties>().Count();
                var mem = GC.GetTotalMemory(true);

                Console.WriteLine($"Opening memory model: {sw.ElapsedMilliseconds}ms, number of entities: {count}");
                Console.WriteLine($"Number of walls: {wallCount}, number of IfcRelDefinesByProperties: {relProps}");
                Console.WriteLine($"Memory: {mem/1000000}MB");
            }


            sw.Restart();
            using (var file = File.OpenRead(fileName))
            using (var mm = MemoryModel.OpenReadStep21(file, null, null))
            {
                sw.Stop();

                count = mm.Instances.Count;
                wallCount = mm.Instances.OfType<IIfcWall>().Count();
                relProps = mm.Instances.OfType<IIfcRelDefinesByProperties>().Count();
                var mem = GC.GetTotalMemory(true);

                Console.WriteLine($"Opening memory model: {sw.ElapsedMilliseconds}ms, number of entities: {count}");
                Console.WriteLine($"Number of walls: {wallCount}, number of IfcRelDefinesByProperties: {relProps}");
                Console.WriteLine($"Memory: {mem/1000000}MB");
            }

            sw.Restart();

            //using (var file = File.OpenRead(fileName))
            //{
            //    var ef = new EntityFactory();
            //    var m = new MemoryModel(ef);
            //    var s = new XbimP21Scanner(file, file.Length);
            //    var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, m);
            //    s.EntityCreate = (string name, long? label, bool inHeader, out int[] i) => {

            //        //allow all attributes to be parsed
            //        i = null;

            //        if (inHeader)
            //        {
            //            switch (name)
            //            {
            //                case "FILE_DESCRIPTION":
            //                    return header.FileDescription;
            //                case "FILE_NAME":
            //                    return header.FileName;
            //                case "FILE_SCHEMA":
            //                    return header.FileSchema;
            //                default:
            //                    return null;
            //            }
            //        }

            //        if (label == null)
            //            return ef.New(name);

            //        //var typeId = Metadata.ExpressTypeId(name);
            //        var ent = ef.New(m, name, (int)label, true);

            //        //make sure that new added entities will have higher labels to avoid any clashes
            //        return ent;
            //    };
            //    s.Scan();
            //}
            //sw.Stop();
            //Console.WriteLine($"Reading data in memory using pure scanner: {sw.ElapsedMilliseconds}ms");
            //sw.Restart();

            //var bc = new BlockingCollection<List<Tuple<int, string>>>();
            //Task.Run(() =>
            //{
            //    foreach (var c in bc.GetConsumingEnumerable())
            //    {
            //        foreach (var item in c)
            //        {
            //            var tok = item.Item1;
            //            var val = item.Item2;

            //            if (tok < 0)
            //            {
            //                throw new Exception();
            //            }
            //        }
            //    }
            //});
            //using (var file = File.OpenRead(fileName))
            //{
            //    var scanner = new Scanner(file);
            //    int tok;
            //    var limit = 50; //number of entities
            //    var buffer = new List<Tuple<int, string>>(limit * 20);
            //    var count = 0;
            //    var endEntity = (int)';';
            //    do
            //    {
            //        tok = scanner.yylex();
            //        if (tok < 63)
            //        {
            //            buffer.Add(new Tuple<int, string>(tok, null));
            //            if (tok == endEntity) //semicolon marks end of entity
            //            {
            //                count++;
            //            }
            //        }
            //        else
            //        {
            //            buffer.Add(new Tuple<int, string>(tok, scanner.yytext));
            //        }

            //        //pass to blocking queue when there the limit of entities is reached
            //        if (count == limit)
            //        {
            //            bc.Add(buffer);
            //            buffer = new List<Tuple<int, string>>(buffer.Count);
            //            count = 0;
            //        }
            //    }
            //    while (tok != (int)Tokens.EOF);
            //    if (buffer.Count > 0)
            //    {
            //        bc.Add(buffer);
            //    }
            //    bc.CompleteAdding();
            //}


            //sw.Stop();
            //Console.WriteLine($"Just scanning the file into blocking collection: {sw.ElapsedMilliseconds}ms");
        }
    }
}
