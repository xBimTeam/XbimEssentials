using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xbim.Common;

namespace Xbim.Ifc2x3.IfcFunctions
{
    internal class ValuesArray<T> where T : class 
    {
        private readonly IPersist _instance;
        private readonly T[] _args;

        public ValuesArray(IPersist instance)
        {
            _instance = instance;
        }

        public ValuesArray(T[] args)
        {
            _args = args;
        }

        public static ValuesArray<T> operator *(ValuesArray<T> c1, ValuesArray<T> c2)
        {
            // this should be the intersection
            var v1 = c1.ToList();
            var v2 = c2.ToList();

            var t = v1.Intersect(v2).ToArray();
            return  new ValuesArray<T>(t);
        }

        private IEnumerable<T> ToList()
        {
            if (_args != null)
            {
                foreach (var arg in _args)
                {
                    yield return arg;
                }
            }
            else if (_instance != null)
            {
                var tp = _instance.GetType();
                var schema = "";
                var asArr = tp.FullName.Split(new[] { "."}, StringSplitOptions.None);
                schema = asArr[1];               
                while (tp != null)
                {
                    yield return string.Format("{0}.{1}", schema, tp.Name).ToUpperInvariant() as T;
                    tp = tp.GetTypeInfo().BaseType;
                }
            }
        }

        public bool Contains(T content)
        {
            if (_instance != null)
            {
                // todo: can we speed this up?
            }
            return ToList().Contains(content);
        }

        public int Count()
        {
            return ToList().Count();
        }        
    }
}
