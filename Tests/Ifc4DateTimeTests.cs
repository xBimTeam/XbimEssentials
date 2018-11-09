using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4.DateTimeResource;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Ifc4DateTimeTests
    {
        [TestMethod]
        public void DateTimeTest()
        {
            var dt = new DateTime(2016, 3, 31, 10, 54, 2);

            IfcDate date = dt;
            Assert.AreEqual(date.ToString(), "2016-03-31");
            Assert.AreEqual((DateTime)date, new DateTime(2016, 3, 31));
            date = "2016-03-31";
            Assert.AreEqual((DateTime)date, new DateTime(2016, 3, 31));


            IfcDateTime dateTime = dt;
            Assert.AreEqual(dateTime.ToString(), "2016-03-31T10:54:02.0000000");
            dateTime = "2016-03-31T10:54:02";
            Assert.AreEqual((DateTime)dateTime, dt);

            IfcTimeStamp stamp = dt;
            Assert.AreEqual((TimeSpan)stamp, dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            IfcTime time = dt;
            Assert.AreEqual(time.ToString(), "10:54:02.0000000");
            time = "10:54:02.0000000";
            var sTime = DateTime.Today.AddHours(10).AddMinutes(54).AddSeconds(2);
            Assert.AreEqual((DateTime)time, sTime);

            var span = new TimeSpan(9, 5, 32, 45, 12);
            IfcDuration duration = span;
            Assert.AreEqual(duration.ToString(), "P9DT5H32M45.012S");
            Assert.AreEqual(span, (TimeSpan)duration);

        }

        [TestMethod]
        public void DurationTest()
        {
            var durSpan = new TimeSpan(25, 15, 56, 12);
            var duration = new IfcDuration("P25DT15H56M12.000S");
            TimeSpan span = duration;
            IfcDuration spanDur = span;

            Assert.AreEqual(durSpan, span);
            Assert.AreEqual(duration, spanDur);

            //test negative
            durSpan = new TimeSpan(-25, -15, -56, -12);
            duration = new IfcDuration("-P25DT15H56M12.000S");
            span = duration;
            spanDur = span;

            Assert.AreEqual(durSpan, span);
            Assert.AreEqual(duration, spanDur);
        }
    }
}
