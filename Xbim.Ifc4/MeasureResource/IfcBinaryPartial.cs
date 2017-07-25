using System;
using System.Text;

namespace Xbim.Ifc4.MeasureResource
{
    public partial struct IfcBinary
    {
        //public IfcBinary(byte[] val)
        //{
        //    _value = "0" + ByteArrayToString(val);
        //}
        //
        //
        //public static implicit operator IfcBinary(byte[] value)
        //{
        //    return new IfcBinary(value);
        //}
        //
        //public static implicit operator byte[] (IfcBinary obj)
        //{
        //    return obj.Bytes;
        //}
        //
        public byte[] Bytes
        {
            get
            {
                return _value;
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
