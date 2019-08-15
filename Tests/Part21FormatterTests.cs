using System;
using System.Collections.Generic;
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
        [TestMethod]
        public void FormatDoubleScientificNotation()
        {
            Part21Formatter formatter = new Part21Formatter();
            string fmt_1 = "G";
            double arg = 0.0000053;

            string result_1 = formatter.Format(fmt_1, arg, null);
            string expected = "5.3E-06";
            Assert.AreEqual(expected, result_1, "Wrong conversion!");
        }

        [TestMethod]
        public void FormatDouble()
        {
            Part21Formatter formatter = new Part21Formatter();
            string fmt_1 = "G";
            double arg = -12345678.5321;

            string result_1 = formatter.Format(fmt_1, arg, null);
            string expected = "-12345678.5321";
            Assert.AreEqual(expected, result_1, "Wrong conversion!");
        }
    }
}
