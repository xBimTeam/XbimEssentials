using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Xbim.Common;
using Xbim.Common.Federation;

namespace Xbim.IO.Memory
{
    /// <summary>
    /// This is a generic implementation of federated model. It doesn't assume anything about the underlying models
    /// and their implementation. It only operates on interfaces from xBIM Common. Exception will be thrown if you try to
    /// add multiple models with the same identifier. But it is your responsibility to keep identifiers unique.
    /// This model doesn't provide any Open() or Close() methods. It is your choice how you open the models and how you
    /// close and dispose them. 
    /// </summary>
    class FederatedModel: IFederatedModel
    {
        /// <summary>
        /// This is a generic implementation of federated model. It doesn't assume anything about the underlying models
        /// and their implementation. It only operates on interfaces from xBIM Common. Exception will be thrown if you try to
        /// add multiple models with the same identifier. But it is your responsibility to keep identifiers unique.
        /// This model doesn't provide any Open() or Close() methods. It is your choice how you open the models and how you
        /// close and dispose them. 
        /// </summary>
        public FederatedModel()
        {
            Instances = new ReferencedEntityCollection(this);
        }

        private readonly ReferencedModelCollection _modelCollection = new ReferencedModelCollection();
        
        /// <summary>
        /// All models referenced in this federation. It is up to you which kind of models you add to this.
        /// You might also want to open transactions on these models if you wish to make any changes. Federation
        /// model is generally ment for a read-only unified access to the underlying data. If you do any changes
        /// don't forget to save/persist them. It is also your own responsibility.
        /// </summary>
        public virtual IEnumerable<IReferencedModel> ReferencedModels { get { return _modelCollection; } }
        
        /// <summary>
        /// Adds new referenced model to the collection. You have to make sure that identifiers are unique.
        /// </summary>
        /// <param name="model"></param>
        public virtual void AddModelReference(IReferencedModel model)
        {
            _modelCollection.Add(model);
        }

        /// <summary>
        /// This is the most generic implementation of IReadOnlyEntityCollection. It only provides read-only functions
        /// for all models in the IFederatedModel.ReferencedModels enumeration. All access functions just use LINQ aggregation
        /// and selection to provide an unified access to the entities. Bare in mind that no changes to objects are allowed unless
        /// you open transaction explicitly in the model which holds them. This is easy to do as every entity has a property with IModel.
        /// But again, it is your responsibility to save any changes you make in any on the models. This only provides read access.
        /// </summary>
        public virtual IReadOnlyEntityCollection Instances { get; private set; }
    }

    /// <summary>
    /// This is the most generic implementation of IReadOnlyEntityCollection. It only provides read-only functions
    /// for all models in the IFederatedModel.ReferencedModels enumeration. All access functions just use LINQ aggregation
    /// and selection to provide an unified access to the entities. Bare in mind that no changes to objects are allowed unless
    /// you open transaction explicitly in the model which holds them. This is easy to do as every entity has a property with IModel.
    /// But again, it is your responsibility to save any changes you make in any on the models. This only provides read access.
    /// </summary>
    public class ReferencedEntityCollection : IReadOnlyEntityCollection
    {
        private readonly IFederatedModel _model;

        public ReferencedEntityCollection(IFederatedModel model)
        {
            _model = model;
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            return _model.ReferencedModels.SelectMany(m => m.Model.Instances).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        {
            return _model.ReferencedModels.Select(rm => rm.Model).SelectMany(model => model.Instances.Where(expr));
        }

        public T FirstOrDefault<T>() where T : IPersistEntity
        {
            return OfType<T>().FirstOrDefault();
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        {
            return Where(expr).FirstOrDefault();
        }

        public IEnumerable<T> OfType<T>() where T : IPersistEntity
        {
            return _model.ReferencedModels.Select(rm => rm.Model).SelectMany(model => model.Instances.OfType<T>());
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity
        {
            return _model.ReferencedModels.Select(rm => rm.Model).SelectMany(model => model.Instances.OfType<T>(activate));
        }

        public IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {
            return _model.ReferencedModels.Select(rm => rm.Model).SelectMany(model => model.Instances.OfType(stringType, activate));
        }

        public long Count
        {
            get { return _model.ReferencedModels.Select(rm => rm.Model).Sum(m => m.Instances.Count); }
        }

        public long CountOf<T>() where T : IPersistEntity
        {
            return _model.ReferencedModels.Select(rm => rm.Model).Sum(m => m.Instances.CountOf<T>());
        }
    }

    /// <summary>
    /// Helper class to make sure that referenced models have unique identifiers.
    /// </summary>
    internal class ReferencedModelCollection : KeyedCollection<string, IReferencedModel>
    {

        protected override string GetKeyForItem(IReferencedModel item)
        {
            return item.Identifier;
        }
    }
}
