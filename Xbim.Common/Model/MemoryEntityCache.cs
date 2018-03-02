using System.Linq;
using Xbim.Common;

namespace Xbim.Common.Model
{
    internal class MemoryEntityCache : IEntityCache
    {
        StepModel _model;

        public MemoryEntityCache(StepModel model)
        {
            _model = model;
        }

        public int Size => _model.Instances.Count();

        public bool IsActive => true;

        public void Clear() { }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.EntityCacheReference = null;
                _model = null;
            }
        }

        public void Start() { }

        public void Stop() { }
    }
}
