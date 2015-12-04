using System;

namespace Xbim.IO
{
    [Flags]
    public enum IfcStorageType
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
        StpZip = 32
    }


}
