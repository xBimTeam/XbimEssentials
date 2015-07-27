namespace Xbim.Common.Geometry
{
    /// <summary>
    /// Discriminates representations on the application of boolean operations
    /// </summary>
    public enum XbimGeometryRepresentationType
    {
        /// <summary>
        /// boolean operations with voids and extensions are included in the resulting representation
        /// </summary>
        OpeningsAndAdditionsIncluded = 1,
        /// <summary>
        /// boolean operations with voids and extensions are excluded in the resulting representation
        /// </summary>
        OpeningsAndAdditionsExcluded = 2,
        /// <summary>
        /// representation of voids and extensions only 
        /// </summary>
        OpeningsAndAdditionsOnly = 4
    }
}
