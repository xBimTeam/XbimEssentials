using System;
using System.IO;

namespace Xbim.IO
{
    public static class StringExtensions
    {
        public static XbimStorageType IfcStorageType(this String path)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return XbimStorageType.Invalid;
            ext = ext.ToLowerInvariant();
            if (ext == ".ifc") return XbimStorageType.Ifc;
            if (ext == ".ifcxml") return XbimStorageType.IfcXml;
            if (ext == ".ifczip") return XbimStorageType.IfcZip;
            if (ext == ".xbim") return XbimStorageType.Xbim;
            return XbimStorageType.Invalid;
        }
    }
}
