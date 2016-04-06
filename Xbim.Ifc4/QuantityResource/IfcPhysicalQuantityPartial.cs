using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc4.Interfaces
{
    /// <summary>
    /// Readonly interface for IfcPhysicalQuantity
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial interface IIfcPhysicalQuantity : IPropertyOrQuantity
    {
    }
}

namespace Xbim.Ifc4.QuantityResource
{
    public abstract partial class IfcPhysicalQuantity
    {
        IfcIdentifier IPropertyOrQuantity.Name
        {
            get { return Name.Value as string; }
        }

    }
}
