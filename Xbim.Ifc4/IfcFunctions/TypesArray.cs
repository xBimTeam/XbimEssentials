using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;

namespace Xbim.Ifc4.IfcFunctions
{
    internal class TypesArray
    {
        private readonly IPersist _instance;
        private readonly string[] _args;

        public TypesArray(IPersist instance)
        {
            _instance = instance;
        }

        public TypesArray(string[] args)
        {
            _args = args;
        }

        public static TypesArray operator *(TypesArray c1, TypesArray c2)
        {
            // this should be the intersection
            var v1 = c1.ToList();
            var v2 = c2.ToList();

            var t = v1.Intersect(v2).ToArray();
            return  new TypesArray(t);
        }

        private IEnumerable<string> ToList()
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
                    yield return  string.Format("{0}.{1}", schema, tp.Name).ToUpperInvariant();
                    tp = tp.BaseType;
                }
            }
        }

        public bool Contains(string content)
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

        public override int GetHashCode()
        {
            if (_instance != null)
                return _instance.GetType().GetHashCode();
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var ta = obj as TypesArray;
            if (ta == null)
                return false;
            if (ta._instance != null && _instance != null)
                return _instance.GetType().Equals(ta._instance.GetType());

            return base.Equals(obj);
        }

        public static bool operator ==(TypesArray c1, TypesArray c2)
        {
            if (ReferenceEquals(c1, c2))
                return true;
            if (ReferenceEquals(c1, null))
                return false;
            if (ReferenceEquals(c2, null))
                    return false;
            return c1.Equals(c2);
        }

        public static bool operator !=(TypesArray c1, TypesArray c2)
        {
            return !(c1 == c2);
        }
    }
}
