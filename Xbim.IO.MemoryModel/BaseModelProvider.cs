using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Step21;

namespace Xbim.IO
{
    public abstract class BaseModelProvider : IModelProvider
    {
        public abstract StoreCapabilities Capabilities { get; }
        public Func<XbimSchemaVersion, IEntityFactory> EntityFactoryResolver { get; set; }

        public abstract void Close(IModel model);
        public abstract IModel Create(XbimSchemaVersion ifcVersion, string path);
        public abstract IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType);
        public abstract string GetLocation(IModel model);
        public abstract XbimSchemaVersion GetXbimSchemaVersion(string modelPath);
        public abstract IModel Open(Stream data, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1);
        public abstract IModel Open(string path, XbimSchemaVersion schema, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1);
        public abstract void Persist(IModel model, string fileName, ReportProgressDelegate progDelegate = null);

        protected IEntityFactory GetFactory(XbimSchemaVersion type)
        {
            if (EntityFactoryResolver != null)
            {
                var entityFactory = EntityFactoryResolver(type);
                if (entityFactory != null)
                {
                    return entityFactory;
                }
            }

            switch (type)
            {
                case XbimSchemaVersion.Ifc4:
                    return new Ifc4.EntityFactoryIfc4();
                case XbimSchemaVersion.Ifc4x1:
                    return new Ifc4.EntityFactoryIfc4x1();
                case XbimSchemaVersion.Ifc2X3:
                    return new Ifc2x3.EntityFactoryIfc2x3();
                case XbimSchemaVersion.Cobie2X4:
                case XbimSchemaVersion.Unsupported:
                default:
                    throw new NotSupportedException("Schema '" + type + "' is not supported");
            }
        }
    }
}
