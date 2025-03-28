using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xbim.IO.Step21;
using Xunit;

namespace Xbim.Essentials.Tests
{
    public class Part21FormatterTests
    {
        public static string fmt = "R";
        //public static string fmt = "G";
        //public static string fmt = "G17";

        [Fact]
        public void FormatDoubleScientificNotation()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = 0.0000053;

            string result = formatter.Format(fmt, arg, null);
            string expected = "5.3E-06";
            result.Should().Be(expected);
        }

        [Fact]
        public void FormatDoubleScientificNotationRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = 0.0000053;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            roundTripDbl.Should().Be(arg);
        }

        [Fact]
        public void FormatDouble()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678.5323;

            string result = formatter.Format(fmt, arg, null);
            string expected = "-12345678.5323";
            result.Should().Be(expected);
        }

        [Fact]
        public void FormatDoubleRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678.5323;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            roundTripDbl.Should().Be(arg);
        }

        [Fact]
        public void FormatDoubleLargeDecimal()
        {
            Part21Formatter formatter = new Part21Formatter();
            // Number from G specifier example
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?view=netframework-4.8#the-general-g-format-specifier
            double arg = 0.84551240822557006;

            string result = formatter.Format(fmt, arg, null);
            string expected = "0.84551240822557";   // Last digits of double are truncated
            result.Should().Be(expected);
        }

        [Fact]
        public void FormatDoubleLargeDecimalRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            // Number from G specifier example
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?view=netframework-4.8#the-general-g-format-specifier
            double arg = 0.84551240822557006;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            
            roundTripDbl.Should().BeApproximately(arg, 0.000000000000001);
        }

        [Fact]
        public void FormatDoubleWithoutDecimal()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678;

            string result = formatter.Format(fmt, arg, null);
            string expected = "-12345678.";
            result.Should().Be(expected);
        }

        [Fact]
        public void FormatDoubleWithoutDecimalRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -12345678;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            roundTripDbl.Should().Be(arg);
        }

        [Fact]
        public void FormatDoubleWithoutDecimalLarge()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -1234567812345678;

            string result = formatter.Format(fmt, arg, null);
            string expected = "-1234567812345678.";
            result.Should().Be(expected);
        }

        [Fact]
        public void FormatDoubleWithoutDecimalLargeRoundTrip()
        {
            Part21Formatter formatter = new Part21Formatter();
            double arg = -1234567812345678;

            string result = formatter.Format(fmt, arg, null);
            var roundTripDbl = double.Parse(result, new CultureInfo("en-US", false));
            roundTripDbl.Should().Be(arg);
        }

        [InlineData(@"hôtel", @"'h\X\F4tel'")]
        [InlineData(@"Zôë", @"'Z\X\F4\X\EB'")]
        [InlineData("Multi\r\nLine", @"'Multi\X\0D\X\0ALine'")]
        [InlineData(@"コンクリート体積", @"'\X2\30B330F330AF30EA30FC30C84F537A4D\X0\'")]
        [InlineData(@"Emôji👍😄⬆️", @"'Em\X\F4ji\X4\0001F44D0001F604\X0\\X2\2B06FE0F\X0\'")]
        [Theory]
        public void FormatStringAsCorrectEncoding(string input, string expected)
        {
            Part21Formatter formatter = new Part21Formatter();

            string result = formatter.Format(fmt, input, null);

            result.Should().Be(expected);
        }
    }
}