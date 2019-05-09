using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Step21;

namespace Xbim.IO
{
    /// <summary>
    /// Interface defining a provider of <see cref="IModel"/>s that can be used to Create, Open and persist model data
    /// </summary>
    /// <remarks>The implementors can use this to provide the concrete implementation of the <see cref="IModel"/> without
    /// a direct reference to the actual type.</remarks>
    public interface IModelProvider
    {
        /// <summary>
        /// Closes a model and releases any resources
        /// </summary>
        /// <param name="model"></param>
        void Close(IModel model);

        /// <summary>
        /// Creates a new empty persistent model with the provided path
        /// </summary>
        /// <param name="ifcVersion"></param>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        IModel Create(XbimSchemaVersion ifcVersion, string dbPath);

        /// <summary>
        /// Creates a new empty model, backed by the requested type
        /// </summary>
        /// <param name="ifcVersion"></param>
        /// <param name="storageType"></param>
        /// <returns></returns>
        IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType);

        /// <summary>
        /// Gets the <see cref="XbimSchemaVersion"/> of the provided model file
        /// </summary>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        XbimSchemaVersion GetXbimSchemaVersion(string modelPath);

        /// <summary>
        /// Opens a model from the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="dataType"></param>
        /// <param name="schemaVersion"></param>
        /// <param name="modelType"></param>
        /// <param name="accessMode"></param>
        /// <param name="progDelegate"></param>
        /// <param name="codePageOverride"></param>
        /// <returns></returns>
        IModel Open(Stream stream, StorageType dataType, XbimSchemaVersion schemaVersion, XbimModelType modelType, 
            XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1);

        /// <summary>
        /// Opens a model from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="schemaVersion"></param>
        /// <param name="ifcDatabaseSizeThreshHold"></param>
        /// <param name="progDelegate"></param>
        /// <param name="accessMode"></param>
        /// <param name="codePageOverride"></param>
        /// <returns></returns>
        /// 
        IModel Open(string path, XbimSchemaVersion schemaVersion, double? ifcDatabaseSizeThreshHold = null, 
            ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1);

        /// <summary>
        /// Describes the capabilities of the underlying model store(s)
        /// </summary>
        StoreCapabilities Capabilities { get; }

        /// <summary>
        /// Saves the current model to a persistent store
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        /// <param name="progDelegate"></param>
        void Persist(IModel model,string fileName, ReportProgressDelegate progDelegate = null);

        /// <summary>
        /// Gets the location any persisted backing store for the Model
        /// </summary>
        /// <remarks>For EsentModel this would be the location of the xbim file</remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetLocation(IModel model);

        /// <summary>
        /// Gets and sets a delegate to allow consumers to determine the <see cref="IEntityFactory"/> to build 
        /// for a given <see cref="XbimSchemaVersion"/>
        /// </summary>
        Func<XbimSchemaVersion, IEntityFactory> EntityFactoryResolver { get; set; }
    }
}