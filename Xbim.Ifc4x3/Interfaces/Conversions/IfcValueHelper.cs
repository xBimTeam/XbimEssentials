using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Metadata;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4x3
{
    /// <summary>
    /// This helper is using reflection because IfcValue types are identical between IFC4 and IFC4x3
    /// </summary>
    public static class IfcValueHelper
    {

        private static ExpressMetaData _ifc4meta;
        private static ExpressMetaData _ifc4x3meta;

        private static ExpressMetaData Ifc4Meta => _ifc4meta ?? (_ifc4meta = ExpressMetaData.GetMetadata(typeof(Ifc4.Interfaces.IIfcValue).Module));
        private static ExpressMetaData Ifc4x3Meta => _ifc4x3meta ?? (_ifc4x3meta = ExpressMetaData.GetMetadata(typeof(Ifc4x3.MeasureResource.IfcValue).Module));

        public static Ifc4.Interfaces.IIfcValue ToIfc4(this Ifc4x3.MeasureResource.IfcValue value)
        {
            if (value == null)
                return null;
            var name = value.GetType().Name.ToUpperInvariant();
            if (!Ifc4Meta.TryGetExpressType(name, out ExpressType type))
                throw new NotSupportedException();
            return Activator.CreateInstance(type.Type, value.Value) as IIfcValue;
        }

        public static Ifc4x3.MeasureResource.IfcValue ToIfc3(this Ifc4.Interfaces.IIfcValue value)
        {
            if (value == null)
                return null;
            var name = value.GetType().Name.ToUpperInvariant();
            if (!Ifc4x3Meta.TryGetExpressType(name, out ExpressType type))
                throw new NotSupportedException();
            return Activator.CreateInstance(type.Type, value.Value) as Ifc4x3.MeasureResource.IfcValue;
        }
    }
}
