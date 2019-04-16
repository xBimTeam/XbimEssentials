using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xbim.Common;
using Xbim.Common.Step21;

namespace Xbim.IO.Step21.Parser
{
    public struct PropertyValue : IPropertyValue
    {
        private string _strVal;
        private StepParserType _stepParserType;


        private object _entityVal;

        // Regex SpecialCharRegEx has been replaced with XbimP21StringDecoder
        static PropertyValue()
        {
        }

        private static string ConvertFromHex(Match m)
        {
            // Convert the number expressed in base-16 to an integer.
            var value = Convert.ToInt32(m.Groups[1].Value, 16);
            // Get the character corresponding to the integral value.
            return Char.ConvertFromUtf32(value);
        }

        public void Init(string value, StepParserType type)
        {
            _strVal = value;
            _stepParserType = type;
        }

        public void Init(object value)
        {
            _entityVal = value;
            _stepParserType = StepParserType.Entity;
        }

        public StepParserType Type
        {
            get { return _stepParserType; }
        }

        public bool BooleanVal
        {
            get
            {
                if (_stepParserType == StepParserType.Boolean) return _strVal == ".T.";
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _stepParserType.ToString(), "Boolean"));
            }
        }

        public string EnumVal
        {
            get
            {
                if (_stepParserType == StepParserType.Enum) return _strVal;
                if (_stepParserType == StepParserType.String)
                    return _strVal.Trim('.', '\'');
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _stepParserType.ToString(), "Enum"));
            }
        }

        public object EntityVal
        {
            get
            {
                if (_stepParserType == StepParserType.Entity) return _entityVal;
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _stepParserType.ToString(), "Entity"));
            }
        }

        public byte[] HexadecimalVal
        {
            get
            {
                if (_stepParserType != StepParserType.HexaDecimal)
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _stepParserType.ToString(), "HexaDecimal"));

                if (string.IsNullOrWhiteSpace(_strVal))
                    return new byte[0];
                var hex = _strVal[0] == '"' ? _strVal.Substring(1, _strVal.Length-2) : _strVal; //trim leading and trailing quotes if present
                if (hex.Length == 0)
                    return new byte[0];
                if (hex == "0")
                    return new byte[0];

                hex = hex.Length % 2 == 0 ? hex :  hex.Substring(1); //trim leading offset number if present
                int numChars = hex.Length;
                var bytes = new byte[numChars / 2];
                for (int i = 0; i < numChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                return bytes;
            }
        }

        public long IntegerVal
        {
            get
            {
                if (_stepParserType == StepParserType.Integer) return Convert.ToInt64(_strVal);

                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                    _stepParserType.ToString(), "Integer"));
            }
        }

        /// <summary>
        ///   Returns a double if the type parsed is any kind of number
        /// </summary>
        public double NumberVal
        {
            get
            {
                if (_stepParserType == StepParserType.Integer
                    || _stepParserType == StepParserType.Real) return _strVal.ToDouble();
                else if (_stepParserType == StepParserType.HexaDecimal)
                    return Convert.ToDouble(Convert.ToInt64(_strVal, 16));
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _stepParserType, "Number"));
            }
        }

        public double RealVal
        {
            get
            {
                if (_stepParserType == StepParserType.Real || _stepParserType == StepParserType.Integer)
                {

                    return _strVal.ToDouble();
                }
                if (_stepParserType == StepParserType.Entity && _entityVal is IExpressValueType && typeof(double).GetTypeInfo().IsAssignableFrom(((IExpressValueType)_entityVal).UnderlyingSystemType))
                    return (double)(((IExpressValueType)_entityVal).Value);
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                    _stepParserType, "Real"));
            }
        }

        public string StringVal
        {
            get
            {
                var ret = _strVal[0] == '\'' ? _strVal.Substring(1, _strVal.Length - 2) : _strVal; //remove the quotes if present
                if (ret.Contains('\\') || ret.Contains("'")) 
                {
                    var d = new XbimP21StringDecoder();
                    ret = d.Unescape(ret);
                }

                if (_stepParserType == StepParserType.String)
                    return ret;
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _stepParserType.ToString(), "String"));
            }
        }
    }

}
