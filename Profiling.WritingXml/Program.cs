using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xbim.Common;
using Xbim.Ifc4;
using Xbim.Ifc4.Kernel;
using Xbim.IO.Memory;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;

namespace Profiling.WritingXml
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new Stopwatch();
            const string path = "..\\..\\..\\Xbim.IO.Tests\\TestFiles\\SampleHouse4.ifc";
            using (var model = new MemoryModel(new EntityFactory()))
            {
                w.Start();
                model.LoadStep21(path);
                w.Stop();
                Console.WriteLine(@"{0}ms to open model from STEP file.", w.ElapsedMilliseconds);

                w.Restart();
                WriteXml(model, "profiling.xml");
                w.Stop();
                Console.WriteLine(@"{0}ms to write model as XML.", w.ElapsedMilliseconds);
            }
        }

        private static void WriteXml(IModel model, string path)
        {
            using (var xml = XmlWriter.Create(path, new XmlWriterSettings { Indent = false }))
            {
                var writer = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                var project = model.Instances.OfType<IfcProject>();
                var products = model.Instances.OfType<IfcObject>();
                var relations = model.Instances.OfType<IfcRelationship>();

                var all =
                    new IPersistEntity[] { }
                    //start from root
                        .Concat(project)
                    //add all products not referenced in the project tree
                        .Concat(products)
                    //add all relations which are not inversed
                        .Concat(relations)
                    //make sure all other objects will get written
                        .Concat(model.Instances);

                writer.Write(model, xml, all);
                xml.Close();
            }
        }
    }
}
