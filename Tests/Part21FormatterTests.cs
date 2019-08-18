using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO.Step21;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Part21FormatterTests
    {
        public static string fmt = "R";
        //public static string fmt = "G";
        //public static string fmt = "G17";

        [TestMethod]
        public void FormatDoubleScientificNotation()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = 0.0000053;

            string result = formatter.Format(fmt, arg, null);
            string expected = "5.3E-06";
            Assert.AreEqual(expected, result, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleScientificNotationRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = 0.0000053;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            Assert.AreEqual(arg, roundTripDbl, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDouble()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678.5323;

            string result = formatter.Format(fmt, arg, null);
            string expected = "-12345678.5323";
            Assert.AreEqual(expected, result, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678.5323;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            Assert.AreEqual(arg, roundTripDbl, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleLargeDecimal()
        {
            Part21Formatter formatter = new Part21Formatter();
            // Number from G specifier example
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?view=netframework-4.8#the-general-g-format-specifier
            double arg = 0.84551240822557006;

            string result = formatter.Format(fmt, arg, null);
            string expected = "0.84551240822557006";
            Assert.AreEqual(expected, result, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleLargeDecimalRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            // Number from G specifier example
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?view=netframework-4.8#the-general-g-format-specifier
            double arg = 0.84551240822557006;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            Assert.AreEqual(arg, roundTripDbl, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleWithoutDecimal()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678;

            string result = formatter.Format(fmt, arg, null);
            string expected = "-12345678.";
            Assert.AreEqual(expected, result, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleWithoutDecimalRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            Assert.AreEqual(arg, roundTripDbl, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleWithoutDecimalLarge()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -1234567812345678;

            string result = formatter.Format(fmt, arg, null);
            string expected = "-1234567812345678.";
            Assert.AreEqual(expected, result, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDoubleWithoutDecimalLargeRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -1234567812345678;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            Assert.AreEqual(arg, roundTripDbl, "Wrong conversion!");
        }
    }
}