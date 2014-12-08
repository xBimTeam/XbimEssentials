using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;


namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [Serializable]
    [IfcEntity(IfcEntityType.SimpleValue)]
    public struct IfcSpecularRoughness : IPersistIfc, IfcSpecularHighlightSelect
    {

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.RealVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        private double _theValue;

        public object Value
        {
            get { return _theValue; }
        }

        public override string ToString()
        {
            return IfcReal.AsPart21(_theValue);
        }

        public IfcSpecularRoughness(string value)
        {
            _theValue = IfcReal.ToDouble(value);
        }

        public IfcSpecularRoughness(double value)
        {
            _theValue = value;
        }

        public string WhereRule()
        {
            return "";
        }

        public static explicit operator StepP21Token(IfcSpecularRoughness? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcSpecularRoughness)value).ToString());
            else
                return new StepP21Token("$");
        }

        public static implicit operator IfcSpecularRoughness(double? value)
        {
            if (value.HasValue)
                return new IfcSpecularRoughness((double)value);
            else
                return new IfcSpecularRoughness();
        }

        public static implicit operator IfcSpecularRoughness(double value)
        {
            return new IfcSpecularRoughness(value);
        }

        public static implicit operator double(IfcSpecularRoughness obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcSpecularRoughness? obj)
        {
            if (obj.HasValue)
                return ((IfcSpecularRoughness)obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcSpecularRoughness)obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcSpecularRoughness obj1, IfcSpecularRoughness obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcSpecularRoughness obj1, IfcSpecularRoughness obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public string ToPart21
        {
            get { return IfcReal.AsPart21(_theValue); }
        }

        public Type UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }
    }
}
