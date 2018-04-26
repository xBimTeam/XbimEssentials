using System;
using System.Text;

namespace Xbim.IO.Step21
{
    public class XbimP21StringDecoder
    {
        // Documentation on this can be found at 
        // http://www.buildingsmart-tech.org/implementation/get-started/string-encoding/string-encoding-decoding-summary
        // and in the stringconverter.java class thereby posted.
        //
        // available patterns are:
        //  ''                  -> '
        //  \\                  -> \
        //  \S\<1 char>         -> add 0x80 to the char
        //  \P[A-I]\            -> Use codepage ISO-8859-x where x is 1 for A, 2 for B, ... 9 for I. (eg '\PB\' sets ISO-8859-2)
        //  \X\<2 chars>        -> 1 byte hexadecimal unicode
        //  \X2\<4 chars>\X0\   -> 2 byte Unicode sequence (can be repeated until termination with \X0\)
        //  \X4\<8 chars>\X0\   -> 4 byte Unicode sequence (can be repeated until termination with \X0\)
        
        // A list of available code pages for .NET is available at
        // http://msdn.microsoft.com/en-us/library/system.text.encodinginfo.codepage.aspx
        // Unit tests have been performed with data from http://www.i18nguy.com/unicode/supplementary-test.html#utf8
        //
        private const string SingleApostrophToken = @"''";
        private const string SingleBackslashToken = @"\\";
        private const string CodeTableToken = @"\P";
        private const string UpperAsciiToken = @"\S\";
        private const string Hex8Token = @"\X\";
        private const string Hex16Token = @"\X2\";
        private const string Hex32Token = @"\X4\";
        private const string LongHexEndToken = @"\X0\";
        private const byte UpperAsciiShift = 0x80;
        private int iCurChar;
        private string p21;
        private StringBuilder builder;
        private bool eof;
        Encoding OneByteDecoder;


        public string Unescape(string value, int codePageOverride = -1)
        {
           Initialize(value, codePageOverride);
            while (!eof)
            {
                if (At(SingleApostrophToken))
                    ReplaceApostrophes();
                else if (At(SingleBackslashToken))
                    ReplaceBackSlashes();
                else if (At(CodeTableToken))
                    ParseCodeTable();
                else if (At(UpperAsciiToken))
                    ParseUpperAscii();
                else if (At(Hex8Token))
                    ParseHex8();
                else if (At(Hex16Token))
                    ParseTerminatedHex(4);
                else if (At(Hex32Token))
                    ParseTerminatedHex(8);
                else
                    CopyCharacter();
            }
            return builder.ToString();
        }

        private void ParseCodeTable()
        {
            var CodePageIds = "ABCDEFGHI";
            MovePast(CodeTableToken);
            if (eof || !HasLength(2)) throw new XbimP21EofException();
            var CodePageChar = CurrentChar();
            var iAddress = CodePageIds.IndexOf(CodePageChar);
            if (iAddress == -1)
                throw new XbimP21InvalidCharacterException(String.Format("Invalid codepage character '{0}'", CodePageChar));
            MoveNext();
            if (CurrentChar() != '\\')
                throw new XbimP21InvalidCharacterException(String.Format("Invalid codepage termination '{0}'", CurrentChar()));
            iAddress++;
            Move(1); // past the last backslash
            OneByteDecoder = Encoding.GetEncoding("iso-8859-" + iAddress.ToString());
        }

        private void ReplaceBackSlashes()
        {
            MovePast(SingleBackslashToken);
            builder.Append('\\');
        }

        private void ReplaceApostrophes()
        {
            MovePast(SingleApostrophToken);
            builder.Append("'");
        }

        private void ParseUpperAscii()
        {
            MovePast(UpperAsciiToken);
            if (eof) throw new XbimP21EofException();
            var val = (byte)(CurrentChar() + UpperAsciiShift);
            var upperAscii = new byte[] { val };
            builder.Append(OneByteDecoder.GetChars(upperAscii));
            MoveNext();
        }

        private void ParseHex8()
        {
            MovePast(Hex8Token);
            if (eof || !HasLength(2)) throw new XbimP21EofException();
            var byteval = GetHexLength(2);
            builder.Append(OneByteDecoder.GetChars(byteval));
        }

        private byte[] GetHexLength(int StringLenght)
        {
            StringLenght /= 2;
            var ret = new byte[StringLenght];
            for (var i = 0; i < StringLenght; i++)
            {
                var hex = p21.Substring(iCurChar, 2);
                try
                {
                    ret[i] = Convert.ToByte(hex, 16);
                    Move(2);
                }
                catch (Exception)
                {
                    throw new XbimP21InvalidCharacterException(String.Format("Invalid hexadecimal representation '{0}'", hex));
                }
            }
            return ret;
        }

        private void ParseTerminatedHex(int stringLenght)
        {
            Move(4); // move past token
            
            // prepare decoder
            var EncodingName = "unicodeFFFE";
            if (stringLenght == 8)
                EncodingName = "utf-32BE";
            var enc = Encoding.GetEncoding(EncodingName);

            // multiple (including none) sequences of stringLenght characters could follow until the termination
            while (!At(LongHexEndToken))
            {
                if (eof || !HasLength(stringLenght + LongHexEndToken.Length))  
                    throw new XbimP21EofException();
                var byteval = GetHexLength(stringLenght);
                builder.Append(enc.GetChars(byteval, 0, stringLenght / 2));
            }
            MovePast(LongHexEndToken);
        }

        private void CopyCharacter()
        {
            builder.Append(CurrentChar());
            MoveNext();
        }

        private char CurrentChar()
        {
            return p21[iCurChar];
        }

        private void Initialize(string value, int codePageOverride = -1)
        {
            if (codePageOverride == -1)
               OneByteDecoder = Encoding.GetEncoding("iso-8859-1");
            else
               OneByteDecoder = Encoding.GetEncoding(codePageOverride);
            builder = new StringBuilder();
            p21 = value;
            eof = (p21.Length == 0);
            iCurChar = 0;
        }

        private bool At(string token)
        {
            return HasLength(token) &&
                   p21.Substring(iCurChar, token.Length).Equals(token);
        }

        private bool HasLength(string token)
        {
            return HasLength(token.Length);
        }

        private bool HasLength(int length)
        {
            return iCurChar + length <= p21.Length;
        }

        private void MoveNext()
        {
            Move(1);
        }

        private void MovePast(string token)
        {
            Move(token.Length);
        }

        private void Move(int length)
        {
            if (eof) return;
            iCurChar += length;
            eof = (iCurChar >= p21.Length);
        }
    }

    public class XbimP21EofException : Exception
    {
        public XbimP21EofException()
            : base(String.Format("Unexpected end of buffer."))
        {
        }
    }

    public class XbimP21InvalidCharacterException : Exception
    {
        public XbimP21InvalidCharacterException(string message)
            : base(message)
        {
        }
    }
}