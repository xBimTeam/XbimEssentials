using System;

// ReSharper disable once CheckNamespace
namespace Xbim.IO
{
    [Obsolete("XbimModel is deprecated in Xbim.IO namespace. It got moved to Xbim.Ifc2x3.IO because it is IFC specific.")]
    public class XbimModel: Ifc2x3.IO.XbimModel
    {
    }
}
