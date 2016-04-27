using System.Text;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.TimeSeriesResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class TimeSeriesExtensions
    {

        public static string GetAsString(this IfcTimeSeries ifcTimeSeries)
        {
            StringBuilder timeSeries = new StringBuilder();
            string start = ifcTimeSeries.StartTime.AsString();
            if (!string.IsNullOrEmpty(start))
            {
                timeSeries.Append("Start:");
                timeSeries.Append(start);
                timeSeries.Append(", ");
            }
            string end = ifcTimeSeries.EndTime.AsString();
            if (!string.IsNullOrEmpty(end))
            {
                timeSeries.Append("End:");
                timeSeries.Append(end);
                timeSeries.Append(", ");
            }

            if (ifcTimeSeries is IfcRegularTimeSeries)
            {
                IfcRegularTimeSeries ifcRegularTimeSeries = (ifcTimeSeries as IfcRegularTimeSeries);
                timeSeries.Append("TimeStep:");
                timeSeries.Append(string.Format("{0,0:N2}", ifcRegularTimeSeries.TimeStep.Value));
               
                //Values field is private?

            }
            if (ifcTimeSeries is IfcIrregularTimeSeries)
            {
                //Values field is private?
            }

            return timeSeries.ToString();
        }

    }
}
