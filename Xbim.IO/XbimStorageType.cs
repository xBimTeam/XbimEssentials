using System;

namespace Xbim.IO
{
    [Flags]
    public enum XbimStorageType
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
        Step21 = 2,

        /// <summary>
        ///   compressed IFC format
        /// </summary>
        Step21Zip = 4,

        Ifc = 8,
           
        Xbim = 16,
        IfcZip = 32    
    }


}
