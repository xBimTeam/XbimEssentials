using System;
using System.Collections.Generic;
using System.IO;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Step21;

namespace Xbim.IO.Esent
{
    public class EsentModelProvider : BaseModelProvider
    {
        public override StoreCapabilities Capabilities => new StoreCapabilities(isTransient: false, supportsTransactions: true);

        public override void Close(IModel model)
        {
            if (model is EsentModel esentSub)
                esentSub.Close();
        }

        public override IModel Create(XbimSchemaVersion ifcVersion, string dbPath)
        {
            var factory = GetFactory(ifcVersion);
            return EsentModel.CreateModel(factory, dbPath);
        }

        public override IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            var factory = GetFactory(ifcVersion);
            if (storageType == XbimStoreType.EsentDatabase)
            {
                return EsentModel.CreateTemporaryModel(factory);
            }
            throw new NotSupportedException($"{storageType} is not a supported Storage Type");
        }

        public override string GetLocation(IModel model)
        {
            if (model == null)
                return null;
            if (model is EsentModel esentModel)
            {
                return esentModel.DatabaseName;
            }
            throw new NotSupportedException($"{model.GetType().Name} is not a supported Model Type");
        }

        public override XbimSchemaVersion GetXbimSchemaVersion(string modelPath)
        {
            var storageType = modelPath.StorageType();
            if (storageType == StorageType.Invalid)
            {
                return XbimSchemaVersion.Unsupported;
            }
            if (storageType != StorageType.Xbim)
            {
                return Memory.MemoryModel.GetSchemaVersion(modelPath);
            }

            var stepHeader = EsentModel.GetStepFileHeader(modelPath);
            IList<string> schemas = stepHeader.FileSchema.Schemas;
            var schemaIdentifier = string.Join(", ", schemas);
            foreach (var schema in schemas)
            {
                if (string.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4;
                if (string.Compare(schema, "Ifc4x1", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4x1;
                if (string.Compare(schema, "Ifc2x3", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc2X3;
                if (schema.StartsWith("Ifc2x", StringComparison.OrdinalIgnoreCase)) //return this as 2x3
                    return XbimSchemaVersion.Ifc2X3;

            }
            return XbimSchemaVersion.Unsupported;
        }

        public override IModel Open(Stream stream, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
        {
            //any Esent model needs to run from the file so we need to create a temporal one
            var xbimFilePath = DatabaseFileName ?? Path.GetTempFileName();
            xbimFilePath =  Path.ChangeExtension(xbimFilePath, ".xbim");

            switch (dataType)
            {
                case StorageType.Xbim:
                    //xBIM file has to be opened from the file so we need to create temporary file if it is not a local file stream
                    var localFile = false;
                    var fileStream = stream as FileStream;
                    if (fileStream != null)
                    {
                        var name = fileStream.Name;
                        //if it is an existing local file, just use it
                        if (File.Exists(name))
                        {
                            xbimFilePath = name;
                            //close the stream from argument to have an exclusive access to the file
                            stream.Close();
                            localFile = true;
                        }
                    }
                    if (!localFile)
                    {
                        using (var tempFile = File.Create(xbimFilePath))
                        {
                            stream.CopyTo(tempFile);
                            tempFile.Close();
                        }
                    }
                    // Scope to avoid name clashes
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        model.Open(xbimFilePath, accessMode, progDelegate);
                        return model;
                    }

                case StorageType.IfcXml:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(stream, stream.Length, dataType, xbimFilePath, progDelegate, keepOpen: true, cacheEntities: true))
                            return model;
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    
                    throw new ArgumentOutOfRangeException("EsentModelProvider only supports EsentModel");

                case StorageType.Stp:
                case StorageType.Ifc:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(stream, stream.Length, dataType, xbimFilePath, progDelegate, keepOpen: true, cacheEntities: true))
                            return model;
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    throw new ArgumentOutOfRangeException("EsentModelProvider only supports EsentModel");

                case StorageType.IfcZip:
                case StorageType.StpZip:
                case StorageType.Zip:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(stream, stream.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return model;
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    throw new ArgumentOutOfRangeException("EsentModelProvider only supports EsentModel");

                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        public override IModel Open(string path, XbimSchemaVersion schemaVersion, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
        {
            var storageType = path.StorageType();

            if (storageType == StorageType.Xbim) //open the XbimFile
            {
                var model = CreateEsentModel(schemaVersion, codePageOverride);
                model.Open(path, accessMode, progDelegate);
                return model;
            }
            else //it will be an IFC file if we are at this point
            {
                var fInfo = new FileInfo(path);
                
                var tmpFileName = DatabaseFileName ?? Path.GetTempFileName();
                var model = CreateEsentModel(schemaVersion, codePageOverride);
                // We delete the XBIM on close as the consumer is not controlling the generation of the XBIM file
                if (model.CreateFrom(path, tmpFileName, progDelegate, keepOpen: true, deleteOnClose: DatabaseFileName == null))
                    return model;

                throw new FileLoadException(path + " file was not a valid IFC format");
                
            }
        }

        public override void Persist(IModel model, string fileName, ReportProgressDelegate progDelegate = null)
        {
            if (model is EsentModel esentModel)
            {
                var fullSourcePath = Path.GetFullPath(esentModel.DatabaseName);
                var fullTargetPath = Path.GetFullPath(fileName);
                if (string.Compare(fullSourcePath, fullTargetPath, StringComparison.OrdinalIgnoreCase) == 0)
                    return; // do nothing - don't save on top of self
            }
            else
            {
                throw new ArgumentOutOfRangeException("EsentModelProvider only supports EsentModel");
            }

            // Create a new Esent model for this Model => Model copy
            var factory = GetFactory(model.SchemaVersion);
            using (var esentDb = new EsentModel(factory))
            {
                esentDb.CreateFrom(model, fileName, progDelegate);
                esentDb.Close();
            }
        }

        private EsentModel CreateEsentModel(XbimSchemaVersion schema, int codePageOverride)
        {
            var factory = GetFactory(schema);
            var model = new EsentModel(factory)
            {
                CodePageOverride = codePageOverride
            };
            return model;
        }

        public string DatabaseFileName { get; set; }
    }
}
