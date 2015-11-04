using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.IO.Memory;
using Xbim.Ifc2x3;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class DateTimeTests
    {
        [TestMethod]
        public void IfcDateAndTimeToIfcDateTimeTest()
        {
            using (var iModel = new MemoryModel<EntityFactory>())
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
    }
}
