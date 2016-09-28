using System;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcLagTimeTransient : PersistEntityTransient, IIfcLagTime 
    {
        // ReSharper disable InconsistentNaming
        private readonly IIfcTimeOrRatioSelect _lagValue;
        private readonly IfcTaskDurationEnum _durationType;
        // ReSharper restore InconsistentNaming

        public IfcLagTimeTransient(string isoDate)
        {
            _durationType = IfcTaskDurationEnum.NOTDEFINED;
            _lagValue = new Ifc4.DateTimeResource.IfcDuration(isoDate);
        }

        public IIfcTimeOrRatioSelect LagValue
        {
            get { return _lagValue; }
            set { throw new NotSupportedException();}
        }

        public IfcTaskDurationEnum DurationType
        {
            get { return _durationType; }
            set { throw new NotSupportedException();}
        }

        public IfcLabel? Name
        {
            get { return null; }
            set { throw new NotSupportedException();}
        }

        public IfcDataOriginEnum? DataOrigin
        {
            get { return null; }
            set { throw new NotSupportedException();}
        }

        public IfcLabel? UserDefinedDataOrigin
        {
            get { return null; }
            set { throw new NotSupportedException();}
        }
    }
}
