using System;
using System.IO;

namespace Xbim.IO
{
    public static class StringExtensions
    {
        public static IfcStorageType StorageType(this String path)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return IfcStorageType.Invalid;
            ext = ext.ToLowerInvariant();
            if (ext == ".ifc") return IfcStorageType.Ifc;
            if (ext == ".ifcxml") return IfcStorageType.IfcXml;
            if (ext == ".ifczip") return IfcStorageType.IfcZip;
            if (ext == ".xbim") return IfcStorageType.Xbim;
            if (ext == ".stp") return IfcStorageType.Stp;
            if (ext == ".stpzip") return IfcStorageType.StpZip;
            return IfcStorageType.Invalid;
        }
    }
}
