#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcIdentifier.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcEntity(IfcEntityType.SimpleValue)]
    [Serializable]
    public struct IfcIdentifier : IPersistIfc, IfcSimpleValue
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.StringVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        private string _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof (string); }
        }

        public object Value
        {
            get { return _theValue; }
        }

        public override string ToString()
        {
            return _theValue;
        }

        public string ToPart21
        {
            get { return _theValue != null ? string.Format(@"'{0}'", IfcText.Escape(_theValue)) : "$"; }
        }

        /// <summary>
        ///   Ensures label does not exceed 255 chars
        /// </summary>
        public IfcIdentifier(string identity)
        {
            _theValue = identity;
        }

        public static implicit operator string(IfcIdentifier obj)
        {
            return (obj._theValue);
        }

        public static implicit operator IfcIdentifier(string str)
        {
            return new IfcIdentifier(str);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcIdentifier) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcIdentifier r1, IfcIdentifier r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcIdentifier r1, IfcIdentifier r2)
        {
            return !Equals(r1, r2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcIdentifier value)
        {
            if (value._theValue != null)
                return new StepP21Token(string.Format(@"'{0}'", value._theValue));
            else
                return new StepP21Token("$");
        }

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (_theValue != null && _theValue.Length > 255)
                return "Identifier : Max length for an Identifier is 255 characters";
            else
                return "";
        }

        #endregion
    }
}