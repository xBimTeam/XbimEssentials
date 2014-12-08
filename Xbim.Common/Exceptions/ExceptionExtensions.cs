#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ExceptionExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Text;

#endregion

namespace Xbim.Common.Exceptions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        ///   Returns a list of indented strings for each error in the exception
        /// </summary>
        public static string ErrorStack(this Exception e, string baseError)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(baseError);
            string indent = "\t";
            Exception ex = e;
            while (ex != null)
            {
                sb.AppendLine(indent + ex.Message);
                ex = ex.InnerException;
                indent += "\t";
            }
            return sb.ToString();
        }
    }
}