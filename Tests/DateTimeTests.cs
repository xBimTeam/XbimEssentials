using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.IO.Memory;
using Xbim.Ifc2x3;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc2x3.MeasureResource;
namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class DateTimeTests
    {
        [TestMethod]
        public void IfcDateAndTimeToIfcDateTimeTest()
        {
            using (var iModel = new MemoryModel(new Xbim.Ifc2x3.EntityFactoryIfc2x3()))
            {
                using (var txn = iModel.BeginTransaction("Insert date time"))
                { 
                    var ifc2x3DateAndTime = iModel.Instances.New<IfcDateAndTime>();

                    var res = ifc2x3DateAndTime.ToISODateTimeString();
                    Assert.IsTrue(res == "0001-01-01T12:00:00.000");
                    ifc2x3DateAndTime.DateComponent = iModel.Instances.New<IfcCalendarDate>();
                    ifc2x3DateAndTime.DateComponent.YearComponent = 2015;
                    ifc2x3DateAndTime.DateComponent.MonthComponent = 11;
                    ifc2x3DateAndTime.DateComponent.DayComponent = 5;
                    ifc2x3DateAndTime.TimeComponent = iModel.Instances.New<IfcLocalTime>();
                    ifc2x3DateAndTime.TimeComponent.HourComponent = 12;
                    ifc2x3DateAndTime.TimeComponent.MinuteComponent = 11;
                    ifc2x3DateAndTime.TimeComponent.SecondComponent = 32.123456789;
                    res = ifc2x3DateAndTime.ToISODateTimeString();
                    Assert.IsTrue(res == "2015-11-05T12:11:32.123");
                    res = ifc2x3DateAndTime.DateComponent.ToISODateTimeString();
                    Assert.IsTrue(res == "2015-11-05T12:00:00");
                    res = ifc2x3DateAndTime.TimeComponent.ToISODateTimeString();
                    Assert.IsTrue(res == "0001-01-01T12:11:32.123");
                }
            }
        }

        [TestMethod]
        public void IfcTimeMeasureToIfcDurationTest()
        {
            const double numSeconds = 3000501.32;
            var timeMeasure = new Xbim.Ifc2x3.MeasureResource.IfcTimeMeasure(numSeconds);
            var isoDuration = timeMeasure.ToISODateTimeString();
            Assert.IsTrue(isoDuration == "P34DT17H28M21.320S");
            //convert back
            var ifc4Duration = new Xbim.Ifc4.DateTimeResource.IfcDuration(isoDuration);
            var timeSpan = ifc4Duration.ToTimeSpan();
            Assert.IsTrue(timeSpan.TotalSeconds == numSeconds);
           
            timeMeasure = new Xbim.Ifc2x3.MeasureResource.IfcTimeMeasure(Math.Truncate(numSeconds));
            isoDuration = timeMeasure.ToISODateTimeString();
            Assert.IsTrue(isoDuration == "P34DT17H28M21S");
        }

    }
}
