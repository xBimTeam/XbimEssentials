using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Model;
using Xbim.Common.Step21;
using Xbim.IO.Parser;
using Xbim.IO.Step21;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;


namespace Xbim.IO.Memory
{
    public class MemoryModel : StepModel
    {
        private static ZipArchiveEntry GetZipEntry(Stream fileStream)
        {
            using (var zipStream = new ZipArchive(fileStream))
            {
                return zipStream.Entries.FirstOrDefault(z => z.Name.IsStepTextFile()); //ignores xbim and zip files
            }
        }

        public static XbimSchemaVersion GetSchemaVersion(string fileName)
        {
            // need to get the header for each step file storage type
            //if it is a zip, xml or ifc text
            if (fileName.IsStepTextFile())
            {
                using (var fileStream = File.OpenRead(fileName))
                {
                    return  GetStepFileXbimSchemaVersion(fileStream);
                }
            }
            else if (fileName.IsStepZipFile())
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(fileName))
                    {
                        var entry = archive.Entries.FirstOrDefault(z => z.Name.IsStepTextFile());
                        if (entry == null) throw new FileLoadException($"File does not contain a valid model: {fileName}");
                        using (var reader = entry.Open())
                        {
                            if (entry.Name.IsStepTextFile())
                                return GetStepFileXbimSchemaVersion(reader);
                            if (entry.Name.IsStepXmlFile())
                            {
                                XmlSchemaVersion schema;
                                using (var xml = XmlReader.Create(reader))
                                {
                                    schema = XbimXmlReader4.ReadSchemaVersion(xml);
                                }

                                switch (schema)
                                {
                                    case XmlSchemaVersion.Ifc2x3:
                                        return XbimSchemaVersion.Ifc2X3;
                                    case XmlSchemaVersion.Ifc4Add1:
                                    case XmlSchemaVersion.Ifc4:
                                        return XbimSchemaVersion.Ifc4;
                                    case XmlSchemaVersion.Unknown:
                                        return XbimSchemaVersion.Unsupported;
                                }
                            }
                            else
                                throw new FileLoadException($"File does not contain a valid model: {fileName}");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new FileLoadException($"File is an invalid zip format: {fileName}");
                }
            }

            else if (fileName.IsStepXmlFile())
            {
                using (var fileStream = File.OpenRead(fileName))
                {
                    XmlSchemaVersion schema;
                    using (var reader = XmlReader.Create(fileStream))
                    {
                        schema = XbimXmlReader4.ReadSchemaVersion(reader);
                    }

                    switch (schema)
                    {
                        case XmlSchemaVersion.Ifc2x3:
                            return XbimSchemaVersion.Ifc2X3;
                        case XmlSchemaVersion.Ifc4Add1:
                        case XmlSchemaVersion.Ifc4:
                            return XbimSchemaVersion.Ifc4;
                        case XmlSchemaVersion.Unknown:
                            return XbimSchemaVersion.Unsupported;
                    }

                }
            }

            throw new FileLoadException($"File is an invalid model format: {fileName}");
        }

        public static XbimSchemaVersion GetStepFileXbimSchemaVersion(IEnumerable<string> schemas)
        {
            foreach (var schema in schemas)
            {
                if (string.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4;
                if (schema.StartsWith("Ifc2x", StringComparison.OrdinalIgnoreCase)) //return this as 2x3
                    return XbimSchemaVersion.Ifc2X3;
                if (schema.StartsWith("Cobie2X4", StringComparison.OrdinalIgnoreCase)) //return this as Cobie
                    return XbimSchemaVersion.Cobie2X4;
            }
            return XbimSchemaVersion.Unsupported;
        }

        public static XbimSchemaVersion GetStepFileXbimSchemaVersion(Stream stream)
        {
            return GetStepFileXbimSchemaVersion(GetStepFileSchemaVersion(stream));
        }

        public MemoryModel(IEntityFactory entityFactory, int labelFrom): base(entityFactory, labelFrom) {  }

        public MemoryModel(IEntityFactory entityFactory) : this(entityFactory, 0) { }

        public MemoryModel(IEntityFactory entityFactory, ILogger logger = null, int labelFrom = 0) : base (entityFactory, logger, labelFrom) {  }

        private MemoryModel(EntityFactoryResolverDelegate resolver, ILogger logger = null, int labelFrom = 0) : base(resolver, logger, labelFrom) { }

        public virtual void LoadXml(string path, ReportProgressDelegate progDelegate = null)
        {
            using (var file = File.OpenRead(path))
            {
                LoadXml(file, file.Length, progDelegate);

            }
        }

        public virtual void LoadXml(Stream stream, long streamSize, ReportProgressDelegate progDelegate = null)
        {
            _read.Clear();
            var schema = EntityFactory.SchemasIds.First();
            if (string.Equals(schema, "IFC2X3", StringComparison.OrdinalIgnoreCase))
            {
                var reader3 = new XbimXmlReader3(GetOrCreateXMLEntity, entity => { }, Metadata);
                if (progDelegate != null) reader3.ProgressStatus += progDelegate;
                Header = reader3.Read(stream, this);
                if (progDelegate != null) reader3.ProgressStatus -= progDelegate;
            }
            else
            {
                var xmlReader = new XbimXmlReader4(GetOrCreateXMLEntity, entity => { }, Metadata);
                if (progDelegate != null) xmlReader.ProgressStatus += progDelegate;
                Header = xmlReader.Read(stream, this);
                if (progDelegate != null) xmlReader.ProgressStatus -= progDelegate;
            }

            if (Header.FileSchema.Schemas == null)
                Header.FileSchema.Schemas = new List<string>();
            if (!Header.FileSchema.Schemas.Any())
                Header.FileSchema.Schemas.Add(schema);

            //purge
            _read.Clear();
        }

        private readonly Dictionary<int, IPersistEntity> _read = new Dictionary<int, IPersistEntity>();
        private IPersistEntity GetOrCreateXMLEntity(int label, Type type)
        {
            if (_read.TryGetValue(label, out IPersistEntity exist))
                return exist;

            var ent = EntityFactory.New(this, type, label, true);
            AddEntityInternal(ent);
            _read.Add(label, ent);
            return ent;
        }

        public virtual void LoadZip(string file, ReportProgressDelegate progDelegate = null)
        {
            using (var stream = File.OpenRead(file))
            {
                LoadZip(stream, progDelegate);
            }
        }

        /// <summary>
        /// Loads the content of the model from ZIP archive. If the actual model file inside the archive is XML
        /// it is supposed to have an extension containing 'XML' like '.ifcxml', '.stpxml' or similar.
        /// </summary>
        /// <param name="stream">Input stream of the ZIP archive</param>
        /// <param name="progDelegate"></param>
        public virtual void LoadZip(Stream stream, ReportProgressDelegate progDelegate = null)
        {
            using (var zipStream = new ZipArchive(stream))
            {
                var zipContent = zipStream.Entries.FirstOrDefault(z => z.Name.IsStepTextFile()); //ignores xbim and zip files
                if (zipContent.Name.IsStepTextFile())
                {
                    using (var reader = zipContent.Open())
                    {
                        LoadStep21(reader, zipContent.Length, progDelegate);
                    }
                }
                else if (zipContent.Name.IsStepXmlFile())
                {
                    using (var reader = zipContent.Open())
                    {
                        LoadXml(reader, zipContent.Length, progDelegate);
                    }
                }
            }
        }

        public static MemoryModel OpenRead(string fileName, ReportProgressDelegate progressDel)
        {
            return OpenRead(fileName, null, progressDel);
        }
        public static MemoryModel OpenRead(string fileName)
        {
            return OpenRead(fileName, null, null);
        }
        public static MemoryModel OpenRead(string fileName, ILogger logger, ReportProgressDelegate progressDel = null)
        {
            //step21 text file can resolve version in parser
            if (fileName.IsStepTextFile())
            {
                using (var file = File.OpenRead(fileName))
                {
                    return OpenReadStep21(file, logger, progressDel);
                }
            }


            var version = GetSchemaVersion(fileName); //an exception is thrown if this fails

            switch (version)
            {

                case XbimSchemaVersion.Ifc4:
                    var mm4 = new MemoryModel(new Ifc4.EntityFactory(), logger);

                    if (fileName.IsStepZipFile())
                        mm4.LoadZip(fileName, progressDel);
                    else if (fileName.IsStepXmlFile())
                        mm4.LoadXml(fileName, progressDel);
                    else
                        throw new FileLoadException($"Unsupported file type extension: {Path.GetExtension(fileName)}");

                    return mm4;

                case XbimSchemaVersion.Ifc2X3:
                    var mm2x3 = new MemoryModel(new Xbim.Ifc2x3.EntityFactory(), logger);
                    if (fileName.IsStepZipFile())
                        mm2x3.LoadZip(fileName, progressDel);
                    else if (fileName.IsStepXmlFile())
                        mm2x3.LoadXml(fileName, progressDel);
                    else
                        throw new FileLoadException($"Unsupported file type extension: {Path.GetExtension(fileName)}");

                    return mm2x3;

                case XbimSchemaVersion.Cobie2X4:
                case XbimSchemaVersion.Unsupported:
                default:
                    throw new FileLoadException($"Unsupported file format: {fileName}");
            }
        }

        /// <summary>
        /// Reads schema version fron the file on the fly inside the parser so it doesn't need to
        /// access the file twice.
        /// </summary>
        /// <param name="file">Input step21 text file</param>
        /// <param name="logger">Logger</param>
        /// <param name="progressDel">Progress delegate</param>
        /// <returns>New memory model</returns>
        public static MemoryModel OpenReadStep21(string file, ILogger logger = null, ReportProgressDelegate progressDel = null)
        {
            using (var stream = File.OpenRead(file))
            {
                return OpenReadStep21(stream, logger, progressDel);
            }
        }

        /// <summary>
        /// Reads schema version fron the stream on the fly inside the parser so it doesn't need to
        /// access the file twice.
        /// </summary>
        /// <param name="stream">Input stream for step21 text file</param>
        /// <param name="logger">Logger</param>
        /// <param name="progressDel">Progress delegate</param>
        /// <returns>New memory model</returns>
        public static MemoryModel OpenReadStep21(Stream stream, ILogger logger = null, ReportProgressDelegate progressDel = null, IEnumerable<string> ignoreTypes = null)
        {
            var model = new MemoryModel((IEnumerable<string> schemas) => {
                var schema = GetStepFileXbimSchemaVersion(schemas);
                switch (schema)
                {
                    case XbimSchemaVersion.Ifc4:
                        return new Ifc4.EntityFactory();
                    case XbimSchemaVersion.Ifc2X3:
                        return new Ifc2x3.EntityFactory();
                    case XbimSchemaVersion.Cobie2X4:
                    case XbimSchemaVersion.Unsupported:
                    default:
                        return null;
                }
            }, logger);
            model.LoadStep21(stream, stream.Length, progressDel, ignoreTypes);
            return model;
        }

        public virtual void SaveAsXml(Stream stream, XmlWriterSettings xmlSettings, XbimXmlSettings xbimSettings = null, configuration configuration = null, ReportProgressDelegate progress = null)
        {
            using (var xmlWriter = XmlWriter.Create(stream, xmlSettings))
            {
                var schema = EntityFactory.SchemasIds.FirstOrDefault();
                switch (schema)
                {
                    case "IFC2X3":
                        var writer3 = new IfcXmlWriter3();
                        writer3.Write(this, xmlWriter, GetXmlOrderedEntities(schema));
                        break;
                    case "IFC4":
                        var writer4 = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                        writer4.Write(this, xmlWriter, GetXmlOrderedEntities(schema));
                        break;
                    default:
                        var writer = new XbimXmlWriter4(xbimSettings);
                        writer.Write(this, xmlWriter);
                        break;
                }

            }
        }

        private IEnumerable<IPersistEntity> GetXmlOrderedEntities(string schema)
        {
            if (schema != null && schema.ToUpper().Contains("COBIE"))
            {
                return Instances.OfType("Facility", true)
                    .Concat(Instances);
            }

            if (schema == null || !schema.StartsWith("IFC"))
                return Instances;

            var project = Instances.OfType("IfcProject", true);
            var products = Instances.OfType("IfcObject", true);
            var relations = Instances.OfType("IfcRelationship", true);

            //create nice deep XML structure if possible
            var all =
                new IPersistEntity[] { }
                //start from root
                    .Concat(project)
                    //add all products not referenced in the project tree
                    .Concat(products)
                    //add all relations which are not inversed
                    .Concat(relations)
                    //make sure all other objects will get written
                    .Concat(Instances);
            return all;
        }

        public virtual void SaveAsXMLZip(Stream stream, XmlWriterSettings xmlSettings, XbimXmlSettings xbimSettings = null, configuration configuration = null, ReportProgressDelegate progress = null)
        {
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var schema = EntityFactory.SchemasIds.FirstOrDefault();
                var ext = schema != null && schema.StartsWith("IFC") ? ".ifcxml" : ".xml";
                var newEntry = zipStream.CreateEntry($"data{ext}");
                using (var writer = newEntry.Open())
                {
                    SaveAsXml(writer, xmlSettings, xbimSettings, configuration, progress);
                }

            }
        }

        public virtual void SaveAsStep21Zip(Stream stream, ReportProgressDelegate progress = null)
        {
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var schema = EntityFactory.SchemasIds.FirstOrDefault();
                var ext = schema != null && schema.StartsWith("IFC") ? ".ifc" : ".stp";
                var newEntry = zipStream.CreateEntry($"data{ext}");
                using (var writer = newEntry.Open())
                {
                    SaveAsStep21(writer, progress);
                }

            }
        }
    }
}
