using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Logging;

namespace Xbim.Ifc4.MeasureResource
{
    public partial struct IfcBinary
    {
        public IfcBinary(byte[] val)
        {
            _value = "0" + ByteArrayToString(val);
        }


        public static implicit operator IfcBinary(byte[] value)
        {
            return new IfcBinary(value);
        }

        public static implicit operator byte[] (IfcBinary obj)
        {
            return obj.Bytes;
        }

        public byte[] Bytes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_value) || _value.Equals("0"))
                    return new byte[0];
                var n = _value[0]; //addition to 4. Anything else than 0 means that it can't be converted to bytes
                if (n != '0')
                {
                    var log = LoggerFactory.GetLogger(GetType());
                    log.Warn("Binary data encoded in the string is not byte aligned so it can't be converted into byte array.");
                    return new byte[0];
                }

                var data = _value.Substring(1);
                return StringToByteArray(data);
            }
        }

        //conversion code from http://stackoverflow.com/a/311179/6345585
        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }

        private static byte[] StringToByteArray(string hex)
        {
            int numChars = hex.Length;
            byte[] bytes = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
