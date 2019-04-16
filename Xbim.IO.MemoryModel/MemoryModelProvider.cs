using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Step21;

namespace Xbim.IO.Memory
{
    /// <summary>
    /// A simple model provider implementation using the in-memory <see cref="MemoryModel"/>
    /// </summary>
    public class MemoryModelProvider : BaseModelProvider
    {
        public override StoreCapabilities Capabilities =>  new StoreCapabilities(isTransient: true, supportsTransactions: true);

        public override void Close(IModel model)
        {
            // Nothing to do for In Memory
        }

        public override IModel Create(XbimSchemaVersion ifcVersion, string dbPath)
        {
            throw new NotImplementedException("The MemoryModelProvider does not support creation of XBIM models");
        }

        public override IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            var factory = GetFactory(ifcVersion);
            return new MemoryModel(factory);
        }

        public override string GetLocation(IModel model)
        {
            return string.Empty;
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
                return MemoryModel.GetSchemaVersion(modelPath);
            }

            throw new NotImplementedException("The MemoryModelProvider does not support reading of XBIM models");
        }

        public override IModel Open(Stream stream, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, 
            XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
        {
            if(modelType != XbimModelType.MemoryModel)
            {
                throw new ArgumentOutOfRangeException(nameof(modelType), "MemoryModelProvider only supports MemoryModel");
            }
            switch (dataType)
            {
                case StorageType.Xbim:
                    throw new NotSupportedException("MemoryModelProvider cannot support opening XBIM Streams");

                case StorageType.IfcXml:
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadXml(stream, stream.Length, progDelegate);
                        return model;
                    }

                case StorageType.Stp:
                case StorageType.Ifc:
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadStep21(stream, stream.Length, progDelegate);
                        return model;
                    }

                case StorageType.IfcZip:
                case StorageType.StpZip:
                case StorageType.Zip:
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadZip(stream, progDelegate);
                        return model;
                    }

                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        public override IModel Open(string path, XbimSchemaVersion schemaVersion, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
        {
            var storageType = path.StorageType();

            if (storageType == StorageType.Xbim) //open the XbimFile
            {
                throw new NotSupportedException("The MemoryModelProvider does not support loading of XBIM files.");
            }
            else //it will be an IFC file if we are at this point
            {
                {
                    var model = CreateMemoryModel(schemaVersion);
                    if (storageType.HasFlag(StorageType.IfcZip) || storageType.HasFlag(StorageType.Zip) || storageType.HasFlag(StorageType.StpZip))
                    {
                        model.LoadZip(path, progDelegate);
                    }
                    else if (storageType.HasFlag(StorageType.Ifc) || storageType.HasFlag(StorageType.Stp))
                    {
                        model.LoadStep21(path, progDelegate);
                    }
                    else if (storageType.HasFlag(StorageType.IfcXml))
                    {
                        model.LoadXml(path, progDelegate);
                    }

                    FileInfo f = new FileInfo(path);
                    model.Header.FileName.Name = f.FullName;
                    return model;
                }
            }
        }

        public override void Persist(IModel model, string fileName, ReportProgressDelegate progDelegate = null)
        {
            throw new NotImplementedException("MemoryModelProvider is a transient store and does not support persistance");
        }

        private MemoryModel CreateMemoryModel(XbimSchemaVersion schema)
        {
            var factory = GetFactory(schema);
            return new MemoryModel(factory);
        }
    }
}
