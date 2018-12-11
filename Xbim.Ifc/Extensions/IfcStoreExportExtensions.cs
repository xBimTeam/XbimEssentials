using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.IO;
using Xbim.IO.Step21;
using Xbim.IO.Xml;

namespace Xbim.Ifc
{
    public static class IfcStoreExportExtensions
    {
        /// <summary>
        /// Saves a model as a STEP IFC file
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stream"></param>
        /// <param name="progDelegate"></param>
        public static void SaveAsIfc(this IModel model, Stream stream, ReportProgressDelegate progDelegate = null)
        {

            using (TextWriter tw = new StreamWriter(stream))
            {
                Part21Writer.Write(model, tw, model.Metadata, null, progDelegate);
                tw.Flush();
            }
        }

        /// <summary>
        /// Saves a model as IfcXML file
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stream"></param>
        /// <param name="progDelegate"></param>
        public static void SaveAsIfcXml(this IModel model, Stream stream, ReportProgressDelegate progDelegate = null)
        {
            var settings = new XmlWriterSettings { Indent = true };
            var schema = model.SchemaVersion;
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                if (schema == XbimSchemaVersion.Ifc2X3)
                {
                    var writer = new IfcXmlWriter3();
                    writer.Write(model, xmlWriter, model.Instances);

                }
                else if (schema == XbimSchemaVersion.Ifc4 || schema == XbimSchemaVersion.Ifc4x1)
                {
                    var writer = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                    var project = model.Instances.OfType<Ifc4.Kernel.IfcProject>();
                    var products = model.Instances.OfType<Ifc4.Kernel.IfcObject>();
                    var relations = model.Instances.OfType<Ifc4.Kernel.IfcRelationship>();

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

                    writer.Write(model, xmlWriter, all);
                }
                xmlWriter.Close();
            }
        }

        /// <summary>
        /// Saves a model as a Zipped IFC file
        /// </summary>
        /// <param name="model">The model to export</param>
        /// <param name="stream">The stream will be closed and flushed on return</param>
        /// <param name="zipEntryName">The name of the file zipped inside the file</param>
        /// <param name="storageType">Specify IfcZip and then either IfcXml or Ifc</param>
        /// <param name="progDelegate"></param>
        public static void SaveAsIfcZip(this IModel model, Stream stream, string zipEntryName, StorageType storageType, 
            ReportProgressDelegate progDelegate = null)
        {
            Debug.Assert(storageType.HasFlag(StorageType.IfcZip));
            var fileBody = Path.ChangeExtension(zipEntryName,
                storageType.HasFlag(StorageType.IfcXml) ? "ifcXml" : "ifc"
                );

            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                var newEntry = zipStream.CreateEntry(fileBody);
                using (var writer = newEntry.Open())
                {

                    if (storageType.HasFlag(StorageType.IfcXml))
                        model.SaveAsIfcXml(writer, progDelegate);
                    else //assume it is Ifc
                        model.SaveAsIfc(writer, progDelegate);
                }

            }
        }
    }
}
