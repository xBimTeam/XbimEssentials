namespace Xbim.Ifc2x3.MeasureResource
{
    public static class UnitExtensions
    {

        /// <summary>
        /// Get the full name of the IfcUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string Name(this IfcUnit ifcUnit)
        {
            return Xbim.Ifc4.MeasureResource.UnitExtensions.Name(ifcUnit);
        }
        /// <summary>
        /// Get the symbol of the IfcUnit
        /// </summary>
        /// <returns>string holding symbol</returns>
        public static string Symbol(this IfcUnit ifcUnit)
        {
            return Xbim.Ifc4.MeasureResource.UnitExtensions.Symbol(ifcUnit);
        }

    }
}
