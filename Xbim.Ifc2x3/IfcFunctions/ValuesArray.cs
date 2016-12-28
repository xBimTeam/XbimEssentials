using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common;

namespace Xbim.Ifc2x3.IfcFunctions
{
    internal class ValuesArray<T>
    {
        private List<T> _list;
        private IPersist instance;

        public ValuesArray(IPersist instance)
        {
            this.instance = instance;
        }

        private IEnumerable<T> List
        {
            get
            {
                return _list;
            }
        }

        public static ValuesArray<T> operator *(ValuesArray<T> c1, ValuesArray<T> c2)
        {
            return null;
        }

        public bool Contains(T content)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }        
    }
}
