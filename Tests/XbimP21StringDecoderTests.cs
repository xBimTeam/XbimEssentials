using FluentAssertions;
using Xbim.IO.Step21;
using Xunit;

namespace Xbim.Essentials.Tests
{
    public class XbimP21StringDecoderTests
    {
        [InlineData("", "")]
        [InlineData("''", "'")]
        [InlineData("'Acme'", "'Acme'")]
        [InlineData("O''Leary", "O'Leary")]
        [InlineData("O'Leary", "O'Leary")]
        [InlineData(@"http://foo.bar.com/Abc?c=d", "http://foo.bar.com/Abc?c=d")]
        [InlineData(@"\S\Drger", @"Ärger")]     // D = 0x44. 0x44 + 0x80(top bit) = 0xC4 => Ä (in ISO-8859-1)
        [InlineData(@"h\S\ttel", @"hôtel")]     // t = 0x74. + 0x80 => 0xF4 = ô (in ISO-8859-1)
        [InlineData(@"\PA\h\S\ttel", @"hôtel")]     // t = 0x74. + 0x80 => 0xF4 = ô (in ISO-8859-1) - Explicitly state 8859-1 codepage
        [InlineData(@"\PE\h\S\ttel", @"hєtel")] // t = 0x74. + 0x80 => 0xF4 = є (in ISO-8859-5) P*E* => 5th 8859 codepage (Cyrillic)
        [InlineData(@"\X2\30B330F330AF30EA30FC30C84F537A4D\X0\", "コンクリート体積")]
        [InlineData(@"file://ATTRIBUTE\X2\30B330F330AF30EA30FC30C84F537A4D8868\X0\.XLSX", @"file://ATTRIBUTEコンクリート体積表.XLSX")]
        [InlineData(@"file://ATTRIBUTE/\X2\30B330F330AF30EA30FC30C84F537A4D8868\X0\.XLSX", @"file://ATTRIBUTE/コンクリート体積表.XLSX")]
        [InlineData(@"file://ATTRIBUTE\\\X2\30B330F330AF30EA30FC30C84F537A4D8868\X0\.XLSX", @"file://ATTRIBUTE\コンクリート体積表.XLSX")]
        [InlineData(@"file://ATTRIBUTE\\X2\30B330F330AF30EA30FC30C84F537A4D8868\X0\.XLSX", 
                         @"file://ATTRIBUTE\X2\30B330F330AF30EA30FC30C84F537A4D8868\X0\.XLSX")] // Invalid encoding. Only \\ un-escaped
        [InlineData(@"F:\\Dropbox\\00 - ndBIM_Coordena\S\C\S\'\S\C\S\#o\\01 - Parcerias Comerciais\\YellowBox\\2015-09-24\\IFC\\ARQ.ifc", 
                         @"F:\Dropbox\00 - ndBIM_CoordenaÃ§Ã£o\01 - Parcerias Comerciais\YellowBox\2015-09-24\IFC\ARQ.ifc")]
        [InlineData(@"Em\X\F4ji\X4\0001F44D0001F604\X0\\X2\2B06FE0F\X0\", @"Emôji👍😄⬆️")]
        
        [Theory]
        public void CanDecodeString(string p21input, string expectedOutput)
        {
            var decoder = new XbimP21StringDecoder();

            var result = decoder.Unescape(p21input);

            result.Should().Be(expectedOutput);

        }
    }
}


