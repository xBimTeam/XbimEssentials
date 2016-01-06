using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common;

// ReSharper disable once CheckNamespace
namespace Xbim.CobieExpress
{
    public partial class CobieAttribute
    {
        public void Set(string value)
        {
            if (value == null)
                Value = null;
            else
                Value = (StringValue)value;
        }

        public void Set(int value)
        {
            Value = (IntegerValue)value;
        }

        public void Set(float value)
        {
            Value = (FloatValue)value;
        }

        public void Set(double value)
        {
            Value = (FloatValue)value;
        }

        public void Set(bool value)
        {
            Value = (BooleanValue)value;
        }

        public void Set(DateTime value)
        {
            Value = (DateTimeValue)value;
        }

        public void Set(CobieAttribute value)
        {
            Value = value.Value;
        }

        public void Set(IExpressValueType value)
        {
            Set(value.Value);
        }

        public void Set(object value)
        {
            if (value == null)
                return;

            var attrVal = value as AttributeValue;
            if (attrVal != null)
            {
                Value = attrVal;
                return;
            }

            var expr = value as IExpressValueType;
            if (expr != null)
            {
                Set(expr);
                return;
            }

            var cAttr = value as CobieAttribute;
            if (cAttr != null)
            {
                Set(cAttr);
                return;
            }

            var typeName = value.GetType().Name.ToLowerInvariant();

            switch (typeName)
            {
                case "int":
                case "long":
                    Set((long)value);
                    return;
                case "double":
                    Set((double)value);
                    return;
                case "float":
                    Set((float)value);
                    return;
                case "string":
                    Set((string)value);
                    return;
                case "boolean":
                    Set((bool)value);
                    return;
                case "datetime":
                    Set((DateTime)value);
                    return;
            }
            throw new ArgumentException(typeName + " cannot be converted to AttributeValue.");
        }
    }
}
