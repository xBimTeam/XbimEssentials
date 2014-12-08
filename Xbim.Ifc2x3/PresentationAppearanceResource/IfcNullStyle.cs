#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcNullStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Globalization;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    public enum IfcNullEnum
    {
        NULL
    }

    [Serializable]
    public struct IfcNullStyle : IfcPresentationStyleSelect, IFormattable, IPersistIfc, ExpressType
    {
        public static implicit operator string(IfcNullStyle obj)
        {
            return "NULL";
        }

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof (IfcNullEnum); }
        }

        public object Value
        {
            get { return IfcNullEnum.NULL; }
        }

        public static implicit operator IfcNullStyle(string str)
        {
            if (str == "NULL")
                return new IfcNullStyle();
            else
                throw new Exception(string.Format(@"Invalid NullStyle value, expected 'NULL' found '{0}'", str));
        }

        public string ToPart21
        {
            get { return ".NULL."; }
        }

        #region IPersistIfc Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (!(propIndex == 0 && ((value.Type==IfcParserType.Enum && value.EnumVal=="NULL")  || value.StringVal == "NULL")))
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        public string WhereRule()
        {
            return "";
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format) || format == "P21") return ".NULL.";
            else
                throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Invalid format string: '{0}'.",
                                                        format));
        }

        #endregion
    }
}