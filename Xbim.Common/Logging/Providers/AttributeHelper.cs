#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    AttributeHelper.cs
// (See accompanying copyright.rtf)

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Xbim.Common.Logging.Providers
{
	/// <summary>
	/// A helper class used to retrieve attributes from Assemblies and Types
	/// </summary>
	public class AttributeHelper
	{
		/// <summary>
		/// Retrieve a list of attributes of type T from the member provided.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mInfo"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAttributes<T>(ICustomAttributeProvider mInfo, bool inherit) where T : System.Attribute
		{
		    if (mInfo == null) return null;
		    var attributes = mInfo.GetCustomAttributes(typeof(T), inherit).OfType<T>().ToList();
		    return attributes.Any() ? attributes : Enumerable.Empty<T>();
		}

		/// <summary>
		/// Retrieve attribute T from the member provided.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mInfo"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(ICustomAttributeProvider mInfo, bool inherit) where T : System.Attribute
		{
			return GetAttributes<T>(mInfo, inherit).FirstOrDefault();			
		}
		
		/// <summary>
		/// Retrieve DescriptionAttributeValue from the member provided
		/// </summary>
		/// <param name="mInfo"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static string GetDescriptionAttributeValue(MemberInfo mInfo, bool inherit)
		{			
			var attribute = GetAttribute<DescriptionAttribute>(mInfo, inherit);
		    return attribute != null ? attribute.Description : string.Empty;	
		}
	}
}
