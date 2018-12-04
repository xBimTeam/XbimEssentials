using System;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.IO.Step21;

namespace Xbim.IO.Parser
{
    public class IndexPropertyValue : IPropertyValue
    {
        private bool _bool;
        private string _string;
        private long _long;
        private double _double;
        private byte[] _bytes;
        private IPersist _object;
        private StepParserType _parserType;

        public StepParserType Type
        {
            get { return _parserType; }
        }

        #region IPropertyValue Members

        public bool BooleanVal
        {
            get
            {
                if (_parserType == StepParserType.Boolean) return _bool;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Boolean"));
            }
        }

        public string EnumVal
        {
            get
            {
                if (_parserType == StepParserType.Enum) return _string;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Enum"));
            }
        }

        public object EntityVal
        {
            get
            {
                if (_parserType == StepParserType.Entity) return _object;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Entity"));
            }
        }

        public byte[] HexadecimalVal
        {
            get
            {
                if (_parserType != StepParserType.HexaDecimal)
                    throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "HexaDecimal"));

                return _bytes;

            }
        }

        public long IntegerVal
        {
            get
            {
                if (_parserType == StepParserType.Integer) return _long;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Integer"));
            }
        }

        public double NumberVal
        {
            get
            {
                if (_parserType == StepParserType.Integer) return Convert.ToDouble(_long);
                if (_parserType == StepParserType.Real || _parserType == StepParserType.HexaDecimal)
                    return _double;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Number"));
            }
        }

        public double RealVal
        {
            get
            {
                if (_parserType == StepParserType.Real) return _double;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "Real"));
            }
        }

        public string StringVal
        {
            get
            {
                if (_parserType == StepParserType.String) return _string;
                throw new Exception(string.Format("Wrong parameter type, found {0}, expected {1}",
                                                  _parserType.ToString(), "String"));
            }
        }

        #endregion

        internal void Init(IPersist iPersistIfc)
        {
            _object = iPersistIfc;
            _parserType = StepParserType.Entity;
        }

        internal void Init(long value, StepParserType stepParserType)
        {
            _long = value;
            _parserType = stepParserType;
        }

        internal void Init(string value, StepParserType stepParserType)
        {
            _string = value;
            _parserType = stepParserType;
        }

        internal void Init(byte[] value, StepParserType stepParserType)
        {
            _bytes = value;
            _parserType = stepParserType;
        }

        internal void Init(double value, StepParserType stepParserType)
        {
            _double = value;
            _parserType = stepParserType;
        }

        internal void Init(bool value, StepParserType stepParserType)
        {
            _bool = value;
            _parserType = stepParserType;
        }
    }

}
