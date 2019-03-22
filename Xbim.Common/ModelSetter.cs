using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Common
{
    public static class ModelSetter
    {
        public static void SetToNull(IPersistEntity entity)
        {
            var p = entity as PersistEntity;
            if (p is null)
                return;
            p.Model = null;
        }

        public static void Set(IPersistEntity entity, IModel model)
        {
            var p = entity as PersistEntity;
            if (p is null)
                return;
            p.Model = model;
        }
    }
}
