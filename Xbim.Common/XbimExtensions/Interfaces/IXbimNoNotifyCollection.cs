using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Common.XbimExtensions.Interfaces
{
    /// <summary>
    /// used to access an Xbim collection and prevent notify messages between sent when changes occure
    /// </summary>
    internal interface IXbimNoNotifyCollection
    {
        void Add(object o);
    }
}
