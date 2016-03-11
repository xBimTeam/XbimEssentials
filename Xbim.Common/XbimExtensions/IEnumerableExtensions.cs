using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Common.XbimExtensions
{
    public static class IEnumerableExtensions
    {
        public static XbimTriplet<T> AsTriplet<T>(this IEnumerable<T> coll)
        {
            return new XbimTriplet<T>(coll);
        }
    }
}
