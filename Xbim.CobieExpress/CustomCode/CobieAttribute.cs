using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
