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

namespace Xbim.Common.Helpers
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
			IEnumerable<T> result = null;

			if (mInfo != null)
			{
				T[] attributes = (T[])mInfo.GetCustomAttributes(typeof(T), inherit);

				if (attributes != null && attributes.Count() > 0)
				{
					result = new List<T>(attributes);
				}
			}

			return result;
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
			IEnumerable<T> attributes = GetAttributes<T>(mInfo, inherit);
			T attribute = null;

			if (attributes != null)
			{
				attribute = attributes.First();
			}

			return attribute;
		}
		
		/// <summary>
		/// Retrieve DescriptionAttributeValue from the member provided
		/// </summary>
		/// <param name="mInfo"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static string GetDescriptionAttributeValue(MemberInfo mInfo, bool inherit)
		{
			string attributeValue = string.Empty;

			DescriptionAttribute attribute = GetAttribute<DescriptionAttribute>(mInfo, inherit);

			if (attribute != null)
			{
				attributeValue = attribute.Description;
			}

			return attributeValue;
		}
	}
}
