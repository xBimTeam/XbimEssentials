using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4x3.GeometryResource;

namespace Xbim.Ifc4x3.MeasureResource
{
    // Ensure all LengthMeasures can be used via IIfcLengthMeasure
    public partial struct IfcLengthMeasure : IIfcLengthMeasure
    {

    }

    public partial struct IfcNonNegativeLengthMeasure : IIfcLengthMeasure, IfcCurveMeasureSelect
    {

    }

    public partial struct IfcPositiveLengthMeasure : IIfcLengthMeasure, IfcCurveMeasureSelect
    {

    }
}
