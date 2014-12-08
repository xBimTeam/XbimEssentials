using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO.Parser
{
    public class IndexPropertyValue : IPropertyValue
    {
        private bool _bool;
        private string _string;
        private long _long;
        private double _double;
        private IPersistIfc _object;
        private IfcParserType _parserType;

        public IfcParserType Type
        {
            get { return _parserType; }
        }

        #region IPropertyValue Members

        public bool BooleanVal
        {
            get
            {
                if (_parserType == IfcParserType.Boolean) return _bool;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Boolean"));
            }
        }

        public string EnumVal
        {
            get
            {
                if (_parserType == IfcParserType.Enum) return _string;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Enum"));
            }
        }

        public object EntityVal
        {
            get
            {
                if (_parserType == IfcParserType.Entity) return _object;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Entity"));
            }
        }

        public long HexadecimalVal
        {
            get
            {
                if (_parserType == IfcParserType.HexaDecimal) return _long;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "HexaDecimal"));
            }
        }

        public long IntegerVal
        {
            get
            {
                if (_parserType == IfcParserType.Integer) return _long;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Integer"));
            }
        }

        public double NumberVal
        {
            get
            {
                if (_parserType == IfcParserType.Integer) return Convert.ToDouble(_long);
                if (_parserType == IfcParserType.Real || _parserType == IfcParserType.HexaDecimal)
                    return _double;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Number"));
            }
        }

        public double RealVal
        {
            get
            {
                if (_parserType == IfcParserType.Real) return _double;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Real"));
            }
        }

        public string StringVal
        {
            get
            {
                if (_parserType == IfcParserType.String) return _string;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "String"));
            }
        }

        #endregion

        internal void Init(IPersistIfc iPersistIfc)
        {
            _object = iPersistIfc;
            _parserType = IfcParserType.Entity;
        }

        internal void Init(long value, IfcParserType ifcParserType)
        {
            _long = value;
            _parserType = ifcParserType;
        }

        internal void Init(string value, IfcParserType ifcParserType)
        {
            _string = value;
            _parserType = ifcParserType;
        }

        internal void Init(double value, IfcParserType ifcParserType)
        {
            _double = value;
            _parserType = ifcParserType;
        }

        internal void Init(bool value, IfcParserType ifcParserType)
        {
            _bool = value;
            _parserType = ifcParserType;
        }
    }

}
