// This class was kindly contributed by Lars-Erik
// see the discussions on https://xbim.codeplex.com/discussions/444743
// Many thanks

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.IO.Parser
{
    public class XbimP21Decoder
    {
        private const string CodeTableToken = @"\PA\";
        private const string UpperAsciiToken = @"\S\";
        private const string Hex8Token = @"\X\";
        private const string Hex16Token = @"\X2\";
        private const string Hex16EndToken = @"\X0\";
        private const byte UpperAsciiLBound = 0x80;
        private int i;
        private string p21;
        private StringBuilder builder;
        private bool eof;

        public string Unescape(IfcText value)
        {
            Initialize(value);
            while(!eof)
            {
                if (At(CodeTableToken))
                    MovePast(CodeTableToken);

                if (At(UpperAsciiToken))
                    ParseUpperAscii();
                else if (At(Hex8Token))
                    ParseHex8();
                else if (At(Hex16Token))
                    ParseHex16();
                else
                    CopyCharacter();
            }
            return builder.ToString();
        }

        private void ParseUpperAscii()
        {
            MovePast(UpperAsciiToken);
            if (eof) throw new EofException(UpperAsciiToken);
            var upperAscii = CurrentChar() + UpperAsciiLBound;
            builder.Append((char) upperAscii);
            MoveNext();
        }

        private void ParseHex8()
        {
            MovePast(Hex8Token);
            if (eof || !HasLength(2)) throw new EofException(Hex8Token);
            var hex = p21.Substring(i, 2);
            try
            {
                var byteValue = Convert.ToByte(hex, 16);
                builder.Append((char) byteValue);
                MovePast(hex);
            }
            catch
            {
                throw new InvalidCharacterException(String.Format("Invalid hexadecimal representation '{0}'", hex));
            }
        }

        private void ParseHex16()
        {
            MovePast(Hex16Token);
            if (eof || !HasLength(4)) throw new EofException(Hex16Token);
            var hex = p21.Substring(i, 4);
            try
            {
                var byteValue = Convert.ToByte(hex, 16);
                builder.Append((char)byteValue);
                MovePast(hex);
            }
            catch
            {
                throw new InvalidCharacterException(String.Format("Invalid hexadecimal representation '{0}'", hex));
            }
            if (!At(Hex16EndToken))
                throw new InvalidCharacterException(String.Format("Invalid end of hexadecimal representation"));
            MovePast(Hex16EndToken);
        }

        private void CopyCharacter()
        {
            builder.Append(CurrentChar());
            MoveNext();
        }

        private char CurrentChar()
        {
            return p21[i];
        }

        private void Initialize(IfcText value)
        {
            builder = new StringBuilder();
            p21 = value.ToPart21.Trim('\'');
            eof = p21.Length == 0;
            i = 0;
        }

        private bool At(string token)
        {
            return HasLength(token) &&
                   p21.Substring(i, token.Length).Equals(token);
        }

        private bool HasLength(string token)
        {
            return HasLength(token.Length);
        }

        private bool HasLength(int length)
        {
            return i + length <= p21.Length;
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
            i += length;
            eof = i >= p21.Length;
        }
    }

    public class EofException : Exception
    {
        public EofException(string token)
            : base(String.Format("Unexpected eof after '{0}'", token))
        {
        }
    }

    public class InvalidCharacterException : Exception
    {
        public InvalidCharacterException(string message)
            : base(message)
        {
        }
    }
}

