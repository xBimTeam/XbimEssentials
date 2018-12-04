using System;
using System.Text;

namespace Xbim.IO.Step21
{
    public static class StepGuidHelper
    {
        public static string ToPart21(this Guid guid)
        {
            return string.Format(@"{0}", ConvertToBase64(guid));
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

        private const string CConversionTable =
            //          1         2         3         4         5         6   
            //0123456789012345678901234567890123456789012345678901234567890123
           "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_$";
    }
}
