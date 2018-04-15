using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;

namespace Xbim.Common.Tests
{
    public class IModelFake : IModel
    {
        public int UserDefinedId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object Tag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IGeometryStore GeometryStore => throw new NotImplementedException();

        public IStepFileHeader Header => throw new NotImplementedException();

        public bool IsTransactional => throw new NotImplementedException();

        public IList<XbimInstanceHandle> InstanceHandles => throw new NotImplementedException();

        public IEntityCollection Instances => throw new NotImplementedException();

        public ITransaction CurrentTransaction => throw new NotImplementedException();

        public ExpressMetaData Metadata => throw new NotImplementedException();

        public IModelFactors ModelFactors => throw new NotImplementedException();

        public IInverseCache InverseCache => throw new NotImplementedException();

        public XbimSchemaVersion SchemaVersion => throw new NotImplementedException();

        public ILogger Logger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEntityCache EntityCache => throw new NotImplementedException();
#pragma warning disable CS0067
        public event NewEntityHandler EntityNew;
        public event ModifiedEntityHandler EntityModified;
        public event DeletedEntityHandler EntityDeleted;
#pragma warning restore CS0067
        public bool Activate(IPersistEntity owningEntity)
        {
            throw new NotImplementedException();
        }

        public IInverseCache BeginCaching()
        {
            throw new NotImplementedException();
        }

        public IEntityCache BeginEntityCaching()
        {
            throw new NotImplementedException();
        }

        public IInverseCache BeginInverseCaching()
        {
            throw new NotImplementedException();
        }

        public ITransaction BeginTransaction(string name)
        {
            throw new NotImplementedException();
        }

        public void Delete(IPersistEntity entity)
        {
            throw new NotImplementedException();
        }

        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            throw new NotImplementedException();
        }

        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses, bool keepLabels) where T : IPersistEntity
        {
            throw new NotImplementedException();
        }

        public void StopCaching()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~IModelFake() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
