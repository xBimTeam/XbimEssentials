using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc.Extensions
{
    public static class StringExtensions
    {

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// if this string is not empty or white space it is returned, 
        /// if it is empty or white space the then string is returned if it is not null or white space
        /// if both are null or white space the empty string is returned 
        /// </summary>
        /// <param name="thisOne"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        public static string IfEmptyThen(this IfcLabel thisOne, IfcLabel then)
        {
            if (!string.IsNullOrWhiteSpace(thisOne)) return thisOne;
            if (!string.IsNullOrWhiteSpace(then)) return then;
            return string.Empty;
        }
        public static string IfEmptyThen(this IfcLabel thisOne, IfcLabel? then)
        {
            if (!string.IsNullOrWhiteSpace(thisOne)) return thisOne;
            if (!string.IsNullOrWhiteSpace(then)) return then;
            return string.Empty;
        }
        public static string IfEmptyThen(this IfcLabel? thisOne, IfcLabel? then)
        {
            if (!string.IsNullOrWhiteSpace(thisOne)) return thisOne;
            if (!string.IsNullOrWhiteSpace(then)) return then;
            return string.Empty;
        }
        public static string IfEmptyThen(this string thisOne, string then)
        {
            if (!string.IsNullOrWhiteSpace(thisOne)) return thisOne;
            return !string.IsNullOrWhiteSpace(then) ? then : string.Empty;
        }

        /// <summary>
        /// Compares two strings ignoring culture and case
        /// </summary>
        /// <param name="str"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool IsSame(this string str, string compare)
        {
            return string.Compare(str, compare, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static bool IsSame(this IfcLabel label, string compare)
        {
            return string.Compare(label, compare, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static bool IsSame(this IfcLabel label, IfcLabel compare)
        {
            return string.Compare(label, compare, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static bool IsSame(this IfcLabel? label, string compare)
        {
            return label.HasValue? string.Compare(label.Value, compare, StringComparison.OrdinalIgnoreCase) == 0:compare==null;
        }
        public static bool IsSame(this IfcLabel? label, IfcLabel? compare)
        {
            if (label == null && compare == null) return true;
            if (label == null || compare == null) return false;
            return string.Compare(label.Value, compare.Value, StringComparison.OrdinalIgnoreCase) == 0 ;
        }

        public static bool IsSame(this IfcIdentifier label, string compare)
        {
            return string.Compare(label, compare, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static bool IsSame(this IfcIdentifier label, IfcIdentifier compare)
        {
            return string.Compare(label, compare, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static bool IsSame(this IfcIdentifier? label, string compare)
        {
            return label.HasValue ? string.Compare(label.Value, compare, StringComparison.OrdinalIgnoreCase) == 0 : compare == null;
        }
        public static bool IsSame(this IfcIdentifier? label, IfcIdentifier? compare)
        {
            if (label == null && compare == null) return true;
            if (label == null || compare == null) return false;
            return string.Compare(label.Value, compare.Value, StringComparison.OrdinalIgnoreCase) == 0;
        }
        /// <summary>
        /// True if the string is empty or any white space
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsEmpty(this IfcLabel label)
        {
            return string.IsNullOrWhiteSpace(label);
        }
        /// <summary>
        /// True if the string is empty or any white space
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsEmpty(this IfcLabel? label)
        {
            return string.IsNullOrWhiteSpace(label);
        }
        public static bool IsEmpty(this string label)
        {
            return string.IsNullOrWhiteSpace(label);
        }
        /// <summary>
        /// True if the string is empty or any white space
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsEmpty(this IfcText label)
        {
            return string.IsNullOrWhiteSpace(label);
        }
        /// <summary>
        /// True if the string is empty or any white space
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsEmpty(this IfcText? label)
        {
            return string.IsNullOrWhiteSpace(label);
        }
    }
}
