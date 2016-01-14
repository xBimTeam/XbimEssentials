using System.Collections.Generic;

namespace Xbim.Common
{
    /// <summary>
    /// A class for holding mappings between instances in one model and instances in another model
    /// </summary>
    public class XbimInstanceHandleMap : Dictionary<XbimInstanceHandle, XbimInstanceHandle>
    {
        public IModel FromModel { get; private set; }
        public IModel ToModel { get; private set; }

        public XbimInstanceHandleMap(IModel from, IModel to)
        {
            FromModel = @from;
            ToModel = to;
        }
    }
}
