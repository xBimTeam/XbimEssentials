using System;
using System.Text;

namespace Xbim.Ifc2x3.UtilityResource
{
    public partial struct IfcGloballyUniqueId
    {
        /// <summary>
        ///   Constructs a GloballyUniqueId from a System.Guid
        /// </summary>
        public static IfcGloballyUniqueId FromGuid(Guid gid)
        {
            return new IfcGloballyUniqueId(){_value = ConvertToBase64(gid)};
        }

        public static Guid ConvertFromBase64(string base64StrId)
        {
            if (base64StrId.Length != 22)
                throw new ArgumentOutOfRangeException("base64StrId", "The Guid must be 22 characters long");
            var lastBase64Num = Convert.ToByte(base64StrId[0]);
            if (lastBase64Num - 48 > 3)
                throw new ArgumentOutOfRangeException("base64StrId",
                                                      string.Format(
                                                          "Illegal Guid {0} found, it is greater than 128 bits",
                                                          base64StrId));
            var num = new uint[6];
            var digits = 2;
            for (int i = 0, pos = 0; i < 6; i++)
            {
                num[i] = From64String(base64StrId.Substring(pos, digits));
                pos += digits;
                digits = 4;
            }

            var guidBytes = new byte[16];
            var data1 = (num[0] * 16777216 + num[1]); // 16-13. bytes
            var data2 = (ushort)(num[2] / 256); // 12-11. bytes
            var data3 = (ushort)((num[2] % 256) * 256 + num[3] / 65536); // 10-09. bytes

            guidBytes[8] = (byte)((num[3] / 256) % 256); //    08. byte
            guidBytes[9] = (byte)(num[3] % 256); //    07. byte
            guidBytes[10] = (byte)(num[4] / 65536); //    06. byte
            guidBytes[11] = (byte)((num[4] / 256) % 256); //    05. byte
            guidBytes[12] = (byte)(num[4] % 256); //    04. byte
            guidBytes[13] = (byte)(num[5] / 65536); //    03. byte
            guidBytes[14] = (byte)((num[5] / 256) % 256); //    02. byte
            guidBytes[15] = (byte)(num[5] % 256); //    01. byte

            var d1Bytes = BitConverter.GetBytes(data1);
            var d2Bytes = BitConverter.GetBytes(data2);
            var d3Bytes = BitConverter.GetBytes(data3);

            for (var i = 0; i < d1Bytes.Length; i++)
                guidBytes[i] = d1Bytes[i];
            guidBytes[4] = d2Bytes[0];
            guidBytes[5] = d2Bytes[1];
            guidBytes[6] = d3Bytes[0];
            guidBytes[7] = d3Bytes[1];


            return new Guid(guidBytes);
        }
        /// <summary>
        ///   Returns the 22 character length base 64 ifc compliant string
        /// </summary>
        public static string ConvertToBase64(Guid guid)
        {
            var winBytes = guid.ToByteArray();

            var data1 = BitConverter.ToUInt32(winBytes, 0);
            var data2 = BitConverter.ToUInt16(winBytes, 4);
            var data3 = BitConverter.ToUInt16(winBytes, 6);


            // Creation of six 32 Bit integers from the components of the GUID structure
            var num = new uint[6];
            num[0] = (data1 / 16777216); //    16. byte  (pGuid->Data1 / 16777216) is the same as (pGuid->Data1 >> 24)
            num[1] = (data1 % 16777216);
            // 15-13. bytes (pGuid->Data1 % 16777216) is the same as (pGuid->Data1 & 0xFFFFFF)
            num[2] = (uint)(data2 * 256 + data3 / 256); // 12-10. bytes
            num[3] = (uint)((data3 % 256) * 65536 + winBytes[8] * 256 + winBytes[9]); // 09-07. bytes
            num[4] = (uint)(winBytes[10] * 65536 + winBytes[11] * 256 + winBytes[12]); // 06-04. bytes
            num[5] = (uint)(winBytes[13] * 65536 + winBytes[14] * 256 + winBytes[15]); // 03-01. bytes

            //convert nums to base 64 characters
            var digits = 2;
            var chars = new StringBuilder();
            for (var i = 0; i < 6; i++)
            {
                chars.Append(To64String(num[i], digits));
                digits = 4;
            }
            return chars.ToString();
        }

        public static string AsPart21(Guid guid)
        {
            return string.Format(@"'{0}'", ConvertToBase64(guid));
        }

        /// <summary>
        ///   Helper function to convert from Guid to base 64 string
        /// </summary>
        private static string To64String(uint num, int nDigits)
        {
            //StringBuilder result= new StringBuilder();
            var result = new char[nDigits];
            var act = num;
            for (var iDigit = 0; iDigit < nDigits; iDigit++)
            {
                result[nDigits - iDigit - 1] = CConversionTable[(int)(act % 64)];
                act /= 64;
            }
            return new string(result);
        }

        /// <summary>
        ///   Helper function to convert from base 64 string to Guid
        /// </summary>
        private static uint From64String(string str)
        {
            uint result = 0;
            foreach (var c in str)
            {
                var chrVal = (uint)CConversionTable.IndexOf(c);
                result = (result * 64) + chrVal;
            }
            return result;
        }

        public static implicit operator Guid(IfcGloballyUniqueId gid)
        {
            return ConvertFromBase64(gid._value);
        }

        private const string CConversionTable =
            //          1         2         3         4         5         6   
            //0123456789012345678901234567890123456789012345678901234567890123
           "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_$";
    }
}
