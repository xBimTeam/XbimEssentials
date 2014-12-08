#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    XbimListProxy.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.XbimExtensions.SelectTypes;

#endregion

namespace Xbim.XbimExtensions
{
    public class XbimListProxy<T> : List<T>, ExpressEnumerable
    {
        private readonly string _xmlType;

        internal XbimListProxy(string XmlType)
        {
            _xmlType = XmlType;
        }

        #region ExpressEnumerable Members

        public string ListType
        {
            get { return _xmlType; }
        }

        public void Add(object o)
        {
            base.Add((T) o);
        }

        #endregion
    }
}