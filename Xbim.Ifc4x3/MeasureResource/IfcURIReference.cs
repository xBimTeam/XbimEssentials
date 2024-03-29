// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Common;
using Xbim.Common.Exceptions;

namespace Xbim.Ifc4x3.MeasureResource
{
	[ExpressType("IfcURIReference", 1001)]
	[DefinedType(typeof(string))]
    // ReSharper disable once PartialTypeWithSinglePart
	public partial struct IfcURIReference : IfcSimpleValue, IExpressValueType, IExpressStringType, System.IEquatable<string>
	{ 
		private string _value;
        
		public object Value
        {
            get { return _value; }
        }

 
		string IExpressStringType.Value { get { return _value; } }

		public override string ToString()
        {
			return _value ?? "";
        }
        public IfcURIReference(string val)
        {
            _value = val;
        }


        public static implicit operator IfcURIReference(string value)
        {
            return new IfcURIReference(value);
        }

        public static implicit operator string(IfcURIReference obj)
        {
            return obj._value;

        }


        public override bool Equals(object obj)
        {
			if (obj == null && Value == null)
                return true;

            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return ((IfcURIReference) obj)._value == _value;
        }

		public bool Equals(string other)
	    {
	        return this == other;
	    }

        public static bool operator ==(IfcURIReference obj1, IfcURIReference obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcURIReference obj1, IfcURIReference obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return Value != null ? _value.GetHashCode() : base.GetHashCode();
        }

		#region IPersist implementation
		void IPersist.Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			if (propIndex != 0)
				throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
            _value = value.StringVal;
            
		}
		#endregion

		#region IExpressValueType implementation
        System.Type IExpressValueType.UnderlyingSystemType { 
			get 
			{
				return typeof(string);
			}
		}
		#endregion


	}
}
