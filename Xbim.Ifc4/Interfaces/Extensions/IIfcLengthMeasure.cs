using System;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4.Interfaces
{
    // Enables us to identify all classes of measure that are sub-classes of IfcLengthMeasure
    public partial interface IIfcLengthMeasure : IExpressRealType
    {

    }
}

namespace Xbim.Ifc4.MeasureResource
{
    public partial struct IfcLengthMeasure : IIfcLengthMeasure
    {

    }

    public partial struct IfcNonNegativeLengthMeasure : IIfcLengthMeasure
    {

    }

    public partial struct IfcPositiveLengthMeasure : IIfcLengthMeasure
    {

    }
}
