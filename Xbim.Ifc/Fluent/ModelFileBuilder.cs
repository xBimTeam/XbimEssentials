using System;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
#nullable enable
    /// <summary>
    /// Builds a Model, ensuring new entities have OwnerHistory and defaults applied if specified
    /// </summary>
    public class ModelFileBuilder : IModelFileBuilder, IDisposable
    {
        // <inheritDocs>
        public IModel Model { get; }
        // <inheritDocs>
        public EntityCreator Factory { get; }
        // <inheritDocs>
        public ITransaction Transaction { get; private set; }
        // <inheritDocs>
        public XbimEditorCredentials? Editor { get; }
        // <inheritDocs>
        public IIfcOwnerHistory? OwnerHistory { get; set; }

        private bool disposedValue;

        /// <summary>
        /// Constructs a new <see cref="ModelFileBuilder"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="model"></param>
        public ModelFileBuilder(FluentModelBuilder parent, IModel model)
        {
            Model = model;
            Factory = new EntityCreator(model);
            Editor = parent.Editor;
            Transaction = Model.BeginTransaction("Fluent Builder");

            model.EntityNew += EntityAdded;
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <remarks>For use after a model has been Saved, enabling re-use of prior instances</remarks>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException">Raised if a transaction already exists</exception>
        public void NewTransaction(string name = "")
        {
            if(Model.CurrentTransaction != null)
            {
                throw new InvalidOperationException("A transaction already exists");
            }
            Transaction = Model.BeginTransaction(name ?? "Fluent Builder");
        }

        private void EntityAdded(IPersistEntity entity)
        {
            if (entity is IIfcRoot root)
            {
                root.WithDefaults(t => t with { GlobalId = Guid.NewGuid(), OwnerHistory = OwnerHistory });
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Model.EntityNew -= EntityAdded;
                    Transaction.Dispose();
                    Model.Dispose();
                }
                disposedValue = true;
            }
        }
        // <inheritDocs>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
