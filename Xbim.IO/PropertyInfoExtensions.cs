using System.Reflection;

namespace Xbim.IO
{
    public static class PropertyInfoExtensions
    {
        #region Extensions for .Net40 compatibility

        internal static object GetValue(this PropertyInfo propInfo, object obj)
        {
            return propInfo.GetValue(obj, null);
        }

        internal static void SetValue(this PropertyInfo propInfo, object obj, object value)
        {
            propInfo.SetValue(obj, value, null);
        }
        #endregion
    }
}
