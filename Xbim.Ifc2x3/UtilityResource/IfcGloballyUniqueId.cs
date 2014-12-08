#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGloballyUniqueId.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.UtilityResource
{
    [Serializable]
    public struct IfcGloballyUniqueId : IPersistIfc, ExpressType
    {
        private const string cConversionTable =
            //          1         2         3         4         5         6   
            //0123456789012345678901234567890123456789012345678901234567890123
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_$";

        private readonly string _guid;

        #region ExpressType Members

        public string ToPart21
        {
            get { return string.Format(@"'{0}'", _guid); }
        }

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof(string); }
        }

        public object Value
        {
            get { return _guid; }
        }

        #endregion

        public bool IsUndefined
        {
            get { return string.IsNullOrEmpty(_guid); }
        }

        public static IfcGloballyUniqueId NewGuid()
        {
            return new IfcGloballyUniqueId(ConvertToBase64(Guid.NewGuid()));
        }

        #region Operators

        public static implicit operator string(IfcGloballyUniqueId obj)
        {
            return obj == null ? null : (obj._guid);
        }

        public static implicit operator IfcGloballyUniqueId(string str)
        {
            return new IfcGloballyUniqueId(str);
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcGloballyUniqueId) obj)._guid == _guid;
        }

        public static bool operator ==(IfcGloballyUniqueId obj1, IfcGloballyUniqueId obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcGloballyUniqueId obj1, IfcGloballyUniqueId obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        #endregion

        /// <summary>
        ///   Constructs a GloballyUniqueId from a 22 character ifc string
        /// </summary>
        public IfcGloballyUniqueId(string base64strId)
        {
            _guid = base64strId;
        }

        public static Guid ConvertFromBase64(string base64strId)
        {
            if (base64strId.Length != 22)
                throw new ArgumentOutOfRangeException("GloballyUniqueId", "The Guid must be 22 characters long");
            byte lastBase64Num = Convert.ToByte(base64strId[0]);
            if (lastBase64Num - 48 > 3)
                throw new ArgumentOutOfRangeException("GloballyUniqueId",
                                                      string.Format(
                                                          "Illegal Guid {0} found, it is greater than 128 bits",
                                                          base64strId));
            UInt32[] num = new UInt32[6];
            int digits = 2;
            for (int i = 0, pos = 0; i < 6; i++)
            {
                num[i] = From64String(base64strId.Substring(pos, digits));
                pos += digits;
                digits = 4;
            }

            byte[] guidBytes = new byte[16];
            UInt32 data1 = (num[0]*16777216 + num[1]); // 16-13. bytes
            UInt16 data2 = (UInt16) (num[2]/256); // 12-11. bytes
            UInt16 data3 = (UInt16) ((num[2]%256)*256 + num[3]/65536); // 10-09. bytes

            guidBytes[8] = (byte) ((num[3]/256)%256); //    08. byte
            guidBytes[9] = (byte) (num[3]%256); //    07. byte
            guidBytes[10] = (byte) (num[4]/65536); //    06. byte
            guidBytes[11] = (byte) ((num[4]/256)%256); //    05. byte
            guidBytes[12] = (byte) (num[4]%256); //    04. byte
            guidBytes[13] = (byte) (num[5]/65536); //    03. byte
            guidBytes[14] = (byte) ((num[5]/256)%256); //    02. byte
            guidBytes[15] = (byte) (num[5]%256); //    01. byte

            byte[] d1bytes = BitConverter.GetBytes(data1);
            byte[] d2bytes = BitConverter.GetBytes(data2);
            byte[] d3bytes = BitConverter.GetBytes(data3);

            for (int i = 0; i < d1bytes.Length; i++)
                guidBytes[i] = d1bytes[i];
            guidBytes[4] = d2bytes[0];
            guidBytes[5] = d2bytes[1];
            guidBytes[6] = d3bytes[0];
            guidBytes[7] = d3bytes[1];


            return new Guid(guidBytes);
        }

        /// <summary>
        ///   Constructs a GloballyUniqueId from a System.Guid
        /// </summary>
        public IfcGloballyUniqueId(Guid gid)
        {
            _guid = ConvertToBase64(gid);
        }

        /// <summary>
        ///   Returns the 22 character length base 64 ifc compliant string
        /// </summary>
        public static string ConvertToBase64(Guid guid)
        {
            byte[] winBytes = guid.ToByteArray();

            UInt32 data1 = BitConverter.ToUInt32(winBytes, 0);
            UInt16 data2 = BitConverter.ToUInt16(winBytes, 4);
            UInt16 data3 = BitConverter.ToUInt16(winBytes, 6);


            // Creation of six 32 Bit integers from the components of the GUID structure
            UInt32[] num = new UInt32[6];
            num[0] = (data1/16777216); //    16. byte  (pGuid->Data1 / 16777216) is the same as (pGuid->Data1 >> 24)
            num[1] = (data1%16777216);
            // 15-13. bytes (pGuid->Data1 % 16777216) is the same as (pGuid->Data1 & 0xFFFFFF)
            num[2] = (UInt32) (data2*256 + data3/256); // 12-10. bytes
            num[3] = (UInt32) ((data3%256)*65536 + winBytes[8]*256 + winBytes[9]); // 09-07. bytes
            num[4] = (UInt32) (winBytes[10]*65536 + winBytes[11]*256 + winBytes[12]); // 06-04. bytes
            num[5] = (UInt32) (winBytes[13]*65536 + winBytes[14]*256 + winBytes[15]); // 03-01. bytes

            //convert nums to base 64 characters
            int digits = 2;
            StringBuilder chars = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                chars.Append(To64String(num[i], digits));
                digits = 4;
            }
            return chars.ToString();
        }

        public override string ToString()
        {
            return _guid;
        }

        public static string AsPart21(Guid guid)
        {
            return string.Format(@"'{0}'", ConvertToBase64(guid));
        }

        /// <summary>
        ///   Helper function to convert from Guid to base 64 string
        /// </summary>
        private static string To64String(UInt32 num, int nDigits)
        {
            //StringBuilder result= new StringBuilder();
            char[] result = new char[nDigits];
            UInt32 act = num;
            for (int iDigit = 0; iDigit < nDigits; iDigit++)
            {
                result[nDigits - iDigit - 1] = cConversionTable[(int) (act%64)];
                act /= 64;
            }
            return new string(result);
        }

        /// <summary>
        ///   Helper function to convert from base 64 string to Guid
        /// </summary>
        private static UInt32 From64String(string str)
        {
            UInt32 result = 0;
            foreach (char c in str)
            {
                UInt32 chrVal = (UInt32) cConversionTable.IndexOf(c);
                result = (result*64) + chrVal;
            }
            return result;
        }

        public static implicit operator Guid(IfcGloballyUniqueId gid)
        {
            return ConvertFromBase64(gid._guid);
        }

        #region IPersistIfc Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
        }

        public string WhereRule()
        {
            try
            {
                Guid guid = ConvertFromBase64(_guid);
                string guidStr = ConvertToBase64(guid);
                if (_guid != guidStr)
                    return
                        "InvalidGuidString GloballyUniqueID : The 64 bit encoding of the GloballyUniqueID does not comply with the Ifc standard";
            }
            catch (Exception e)
            {
                return string.Format("InvalidGuidString GloballyUniqueID : {0}", e.Message);
            }
            return "";
        }

        #endregion

        public static bool TryConvertFromBase64(string base64StrId, out Guid guid)
        {
            guid = Guid.Empty;
            if (string.IsNullOrWhiteSpace(base64StrId) || base64StrId.Length != 22 ) return false;
            byte lastBase64Num = Convert.ToByte(base64StrId[0]);
            if ( lastBase64Num - 48 > 3) return false;
            guid = ConvertFromBase64(base64StrId);
            return true;

        }
    }
}