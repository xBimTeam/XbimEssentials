using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
    /// <summary>
    /// Interface used to allow fluent building instances on a model
    /// </summary>
    public interface IModelInstanceBuilder
    {
        /// <summary>
        /// The Instances in the model
        /// </summary>
        IEntityCollection Instances { get; }

        /// <summary>
        /// The entire model object
        /// </summary>
        IModel Model { get; }

        /// <summary>
        /// An <see cref="IEntityFactory"/> built for this model, enabling easy cross-schema creation of entities.
        /// </summary>
        EntityCreator Factory { get; }
    }
}
