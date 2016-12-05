using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Common
{
    public class FlagSetter
    {
        public static void SetActivationFlag(IPersistEntity entity, bool value)
        {
            var p = entity as PersistEntity;
            if (p == null)
                return;
            p._activated = true;
        }

    }
}
