using System.Collections.Generic;

namespace Xbim.Common.XbimExtensions
{
    public struct XbimTriplet<T>
    {
        public T A;
        public T B;
        public T C;
        public XbimTriplet(IEnumerable<T> coll)
        {
            var enumer = coll.GetEnumerator();
            A = enumer.MoveNext() ? enumer.Current : default(T);
            B = enumer.MoveNext() ? enumer.Current : default(T);
            C = enumer.MoveNext() ? enumer.Current : default(T);
        }
    }
}
