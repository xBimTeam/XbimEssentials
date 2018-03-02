using System.Linq;
using Xbim.Common;

namespace Xbim.IO.Memory
{
    internal class MemoryEntityCache : IEntityCache
    {
        MemoryModel _model;

        public MemoryEntityCache(MemoryModel model)
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
