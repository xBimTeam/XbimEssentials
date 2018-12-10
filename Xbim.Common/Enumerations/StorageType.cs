using System;
using System.IO;

namespace Xbim.IO
{
   
    /// <summary>
    /// The data format a model file is stored in
    /// </summary>
    [Flags]
    public enum StorageType
    {
        /// <summary>
        /// Invalid Xbim storage type
        /// </summary>
        Invalid = 0,
        /// <summary>
        ///   IFC in XML format
        /// </summary>
        IfcXml = 1,
        /// <summary>
        ///   Native IFC format
        /// </summary>
        Ifc = 2,
        /// <summary>
        ///   compressed IFC format
        /// </summary>           
        IfcZip = 4,
        Xbim = 8,
        Stp = 16,
        StpZip = 32,
        Zip = 64
    }
    public static class IfcStorageTypeExtensions
    {
        public static StorageType StorageType(this String path)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return IO.StorageType.Invalid;
            ext = ext.ToLowerInvariant();
            if (ext == ".ifc") return IO.StorageType.Ifc;
            if (ext == ".ifcxml") return IO.StorageType.IfcXml;
            if (ext == ".ifczip") return IO.StorageType.IfcZip;
            if (ext == ".xbim") return IO.StorageType.Xbim;
            if (ext == ".stp") return IO.StorageType.Stp;
            if (ext == ".stpzip") return IO.StorageType.StpZip;
            if (ext.Contains("zip")) return IO.StorageType.Zip;
            return IO.StorageType.Invalid;
        }

        /// <summary>
        /// Returns true if ifc or stp file extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsStepTextFile(this String path)
        {
            const string stepTextFileTypes = ".ifc;.stp;";
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;
            return stepTextFileTypes.Contains($"{ext.ToLowerInvariant()};");
        }

        public static bool IsStepZipFile(this String path)
        {
            const string stepTextFileTypes = ".ifczip;.stpzip;.zip;";
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;
            return stepTextFileTypes.Contains($"{ext.ToLowerInvariant()};");
        }

        public static bool IsStepXmlFile(this String path)
        {           
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;
            return ext.ToLowerInvariant() == ".ifcxml";
        }
    }
}
