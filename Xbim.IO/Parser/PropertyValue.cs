using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;
using System.Globalization;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.IO.Parser
{
    public struct PropertyValue : IPropertyValue
    {
        private string _strVal;
        private IfcParserType _ifcParserType;


        private object _entityVal;

        // Regex SpecialCharRegEx has been replaced with XbimP21StringDecoder
        static PropertyValue()
        {
        }

        private static string ConvertFromHex(Match m)
        {
            // Convert the number expressed in base-16 to an integer.
            int value = Convert.ToInt32(m.Groups[1].Value, 16);
            // Get the character corresponding to the integral value.
            return Char.ConvertFromUtf32(value);
        }

        internal void Init(string value, IfcParserType type)
        {
            _strVal = value;
            _ifcParserType = type;
        }

        internal void Init(object value)
        {
            _entityVal = value;
            _ifcParserType = IfcParserType.Entity;
        }

        public IfcParserType Type
        {
            get { return _ifcParserType; }
        }

        public bool BooleanVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.Boolean) return _strVal == ".T.";
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "Boolean"));
            }
        }

        public string EnumVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.Enum) return _strVal;
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "Enum"));
            }
        }

        public object EntityVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.Entity) return _entityVal;
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "Entity"));
            }
        }

        public long HexadecimalVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.HexaDecimal) return Convert.ToInt64(_strVal, 16);
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "HexaDecimal"));
            }
        }

        public long IntegerVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.Integer) return Convert.ToInt64(_strVal);
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "Integer"));
            }
        }

        /// <summary>
        ///   Returns a double if the type parsed is any kind of number
        /// </summary>
        public double NumberVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.Integer
                    || _ifcParserType == IfcParserType.Real) return IfcReal.ToDouble(_strVal);
                else if (_ifcParserType == IfcParserType.HexaDecimal)
                    return Convert.ToDouble(Convert.ToInt64(_strVal, 16));
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "Number"));
            }
        }

        public double RealVal
        {
            get
            {
                if (_ifcParserType == IfcParserType.Real || _ifcParserType == IfcParserType.Integer)
                {

                    return IfcReal.ToDouble(_strVal);
                }
                else if (_ifcParserType == IfcParserType.Entity && _entityVal is ExpressType && typeof(double).IsAssignableFrom(((ExpressType)_entityVal).UnderlyingSystemType))
                    return (double)(((ExpressType)_entityVal).Value);
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "Real"));
            }
        }

        public string StringVal
        {
            get
            {
                string ret = _strVal.Substring(1, _strVal.Length - 2); //remove the quotes
                if (ret.Contains('\\') || ret.Contains("'")) 
                {
                    XbimP21StringDecoder d = new XbimP21StringDecoder();
                    ret = d.Unescape(ret);
                }

                if (_ifcParserType == IfcParserType.String)
                    return ret;
                else
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                      _ifcParserType.ToString(), "String"));
            }
        }
    }

}
