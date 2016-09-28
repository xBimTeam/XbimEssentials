using System.Collections.Generic;

namespace Xbim.Common.Federation
{
    public interface IFederatedModel
    {
        IModel ReferencingModel { get; }
        IEnumerable<IReferencedModel> ReferencedModels { get; }
        void AddModelReference(IReferencedModel model);
        IReadOnlyEntityCollection FederatedInstances { get; }
        IList<XbimInstanceHandle> FederatedInstanceHandles { get; }
    }
}
