using System.Collections.Generic;

namespace Xbim.Common.Federation
{
    public interface IFederatedModel
    {
        IEnumerable<IReferencedModel> ReferencedModels { get; }
        void AddModelReference(IReferencedModel model);
        IReadOnlyEntityCollection Instances { get; }
    }
}
