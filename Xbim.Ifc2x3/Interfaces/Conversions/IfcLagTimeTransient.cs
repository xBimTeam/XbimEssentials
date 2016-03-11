using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcLagTimeTransient : PersistEntityTransient, IIfcLagTime 
    {
        public IIfcTimeOrRatioSelect LagValue
        {
            get;
            internal set;
        }

        public IfcTaskDurationEnum DurationType
        {
            get;
            internal set;
        }

        public Ifc4.MeasureResource.IfcLabel? Name
        {
            get;
            internal set;
        }

        public IfcDataOriginEnum? DataOrigin
        {
            get;
            internal set;
        }

        public Ifc4.MeasureResource.IfcLabel? UserDefinedDataOrigin
        {
            get;
            internal set;
        }
    }
}
