using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO
{
    /// <summary>
    /// A class for holding mappings between instances in one model and instances in another model
    /// </summary>
    public class XbimInstanceHandleMap : Dictionary<XbimInstanceHandle, XbimInstanceHandle>
    {
        readonly IModel fromModel;
        readonly IModel toModel;

        public XbimInstanceHandleMap(IModel from, IModel to)
        {
            fromModel = from;
            toModel = to;
        }

    }
}
