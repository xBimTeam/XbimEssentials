using System;
using System.Collections.Generic;
using System.IO;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Step21;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.Ifc
{

    // IMPORTANT: if we ever rename this provider we need to update the DefaultModelProviderFactory, since
    // this is the default provider when the assembly is loaded, and it's discovered by Name through reflection

    /// <summary>
    /// The <see cref="HeuristicModelProvider"/> encapsulates the underlying <see cref="IModel"/> implementations we use 
    /// to provide different persistance performance characteristics, depending on the use-case and the consumer's inputs.
    /// </summary>
    /// <remarks>This store permits a <see cref="MemoryModel"/> to be used where it's appropriate, while switching to an
    /// <see cref="EsentModel"/> when persistance is required, or a source model is above a size threshold.
    /// The store also permits a <see cref="MemoryModel"/> to persisted.
    /// </remarks>
    public class HeuristicModelProvider : BaseModelProvider
    {
        /// <summary>
        /// The default largest size in MB for an IFC file to be loaded into memory, above this size the store will choose to use 
        /// the database storage media to minimise the memory footprint. This size can be set in the config file or in the open 
        /// statement of this store 
        /// </summary>
        public static double DefaultIfcDatabaseSizeThreshHoldMb = 100; //default size set to 100MB

        /// <summary>
        /// Describes the capabilities of the provider
        /// </summary>
        public override StoreCapabilities Capabilities => new StoreCapabilities(isTransient: false, supportsTransactions: true);

        /// <summary>
        /// Closes a Model store, releasing any resources
        /// </summary>
        /// <param name="model"></param>
        public override void Close(IModel model)
        {
            if (model is EsentModel esentSub)
                esentSub.Close();

            // memory models don't need closing

        }

        /// <summary>
        /// Creates a new Persistent model store
        /// </summary>
        /// <param name="ifcVersion"></param>
        /// <param name="storagePath"></param>
        /// <returns></returns>
        public override IModel Create(XbimSchemaVersion ifcVersion, string dbPath)
        {
            var factory = GetFactory(ifcVersion);
            var model = EsentModel.CreateModel(factory, dbPath);
            return model;

        }

        /// <summary>
        /// Creates a new model store, with the consumer choosing the implementation
        /// </summary>
        /// <param name="ifcVersion"></param>
        /// <param name="storageType"></param>
        /// <returns></returns>
        public override IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            var factory = GetFactory(ifcVersion);
            if (storageType == XbimStoreType.EsentDatabase)
            {
                return EsentModel.CreateTemporaryModel(factory);
            }

            return new MemoryModel(factory);
        }

        /// <summary>
        /// Gets the location of a model, where it is a persisted model store.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override string GetLocation(IModel model)
        {
            if (model == null)
                return null;
            if (model is EsentModel esentModel)
            {
                return esentModel.DatabaseName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the IFC Schema version for a model
        /// </summary>
        /// <param name="modelPath">Path to a model in any supported format (IFC, IfcXml, IfcZip, or XBIM)</param>
        /// <returns></returns>
        public override XbimSchemaVersion GetXbimSchemaVersion(string modelPath)
        {
            var storageType = modelPath.StorageType();
            if (storageType == StorageType.Invalid)
            {
                return XbimSchemaVersion.Unsupported;
            }
            if (storageType != StorageType.Xbim)
            {
                return MemoryModel.GetSchemaVersion(modelPath);
            }

            // Have to use Esent for internal format
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

        /// <summary>
        /// Opens a model from the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="dataType"></param>
        /// <param name="schema"></param>
        /// <param name="modelType"></param>
        /// <param name="accessMode"></param>
        /// <param name="progDelegate"></param>
        /// <param name="codePageOverride"></param>
        /// <returns></returns>
        public override IModel Open(Stream stream, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, 
            XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
        {
            //any Esent model needs to run from the file so we need to create a temporal one
            var xbimFilePath = Path.GetTempFileName();
            xbimFilePath = Path.ChangeExtension(xbimFilePath, ".xbim");
 
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
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadXml(stream, stream.Length, progDelegate);
                        return model;
                    }
                    throw new ArgumentOutOfRangeException("HeuristicModelProvider only supports EsentModel and MemoryModel");

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
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadStep21(stream, stream.Length, progDelegate);
                        return model;
                    }
                    throw new ArgumentOutOfRangeException("HeuristicModelProvider only supports EsentModel and MemoryModel");

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
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadZip(stream, progDelegate);
                        return model;
                    }
                    throw new ArgumentOutOfRangeException("HeuristicModelProvider only supports EsentModel and MemoryModel");

                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        /// <summary>
        /// Opens a model from the provided path, inferring model implementation based on format and model size
        /// </summary>
        /// <param name="path">Path to the model file, in any support IFC or XBIM format</param>
        /// <param name="schemaVersion"></param>
        /// <param name="ifcDatabaseSizeThreshHold"></param>
        /// <param name="progDelegate"></param>
        /// <param name="accessMode"></param>
        /// <param name="codePageOverride"></param>
        /// <returns></returns>
        public override IModel Open(string path, XbimSchemaVersion schemaVersion, double? ifcDatabaseSizeThreshHold = null, 
            ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
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
                double ifcMaxLength = (ifcDatabaseSizeThreshHold ?? DefaultIfcDatabaseSizeThreshHoldMb) * 1024 * 1024;
                // we need to make an Esent database, if ifcMaxLength<0 we use in memory
                if (ifcMaxLength >= 0 && fInfo.Length > ifcMaxLength) 
                {
                    var tmpFileName = Path.GetTempFileName();
                    var model = CreateEsentModel(schemaVersion, codePageOverride);
                    // We delete the XBIM on close as the consumer is not controlling the generation of the XBIM file
                    if (model.CreateFrom(path, tmpFileName, progDelegate, keepOpen: true, deleteOnClose: true))
                        return model; 
                    
                    throw new FileLoadException(path + " file was not a valid IFC format");
                }
                else //we can use a memory model
                {
                    var model = CreateMemoryModel(schemaVersion);
                    if (storageType.HasFlag(StorageType.IfcZip) || storageType.HasFlag(StorageType.Zip) || storageType.HasFlag(StorageType.StpZip))
                    {
                        model.LoadZip(path, progDelegate);
                    }
                    else if (storageType.HasFlag(StorageType.Ifc) || storageType.HasFlag(StorageType.Stp))
                        model.LoadStep21(path, progDelegate);
                    else if (storageType.HasFlag(StorageType.IfcXml))
                        model.LoadXml(path, progDelegate);

                    // if we are looking at a memory model loaded from a file it might be safe to fix the file name in the 
                    // header with the actual file loaded
                    FileInfo f = new FileInfo(path);
                    model.Header.FileName.Name = f.FullName;
                    return model;
                }
            }
        }

        /// <summary>
        /// Persists the model to a permanent store
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="progDelegate"></param>
        public override void Persist(IModel model, string fileName, ReportProgressDelegate progDelegate = null)
        {
            if (model is EsentModel esentModel)
            {
                var fullSourcePath = Path.GetFullPath(esentModel.DatabaseName);
                var fullTargetPath = Path.GetFullPath(fileName);
                if (string.Compare(fullSourcePath, fullTargetPath, StringComparison.OrdinalIgnoreCase) == 0)
                    return; // do nothing - don't save on top of self
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

        private MemoryModel CreateMemoryModel(XbimSchemaVersion schema)
        {
            var factory = GetFactory(schema);
            return new MemoryModel(factory);
        }

        
    }
}
