using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO.Parser;
using Xbim.XbimExtensions;

namespace Xbim.IO
{
    public class XbimParserState
    {

        public XbimParserState(IPersistIfcEntity entity)
        {
            _currentInstance = new Part21Entity(entity);
            _processStack.Push(_currentInstance);
        }

        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private int _listNestLevel = -1;
        private Part21Entity _currentInstance;
        private readonly IndexPropertyValue _propertyValue = new IndexPropertyValue();

        public void BeginList()
        {
            Part21Entity p21 = _processStack.Peek();
            if (p21.CurrentParamIndex == -1)
                p21.CurrentParamIndex++; //first time in take the first argument

            _listNestLevel++;
        }

        public void EndList()
        {
            _listNestLevel--;
            Part21Entity p21 = _processStack.Peek();
            p21.CurrentParamIndex++;
            //Console.WriteLine("EndList");
        }

        public void EndEntity()
        {
            _processStack.Pop();
            //Debug.Assert(_processStack.Count == 0);
        }

        internal void BeginNestedType(string typeName)
        {
            IfcType ifcType = IfcMetaData.IfcType(typeName);
            _currentInstance = new Part21Entity((IPersistIfc)Activator.CreateInstance(ifcType.Type));
            _processStack.Push(_currentInstance);
        }

        internal void EndNestedType()
        {
            _propertyValue.Init(_processStack.Pop().Entity);
            _currentInstance = _processStack.Peek();
            if (_currentInstance.Entity != null)
                _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        private void SetEntityParameter()
        {
            if (_currentInstance.Entity != null)
            {
                //CurrentInstance.SetPropertyValue(PropertyValue);
                try
                {
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
                }
                catch (Exception e)
                {
                    XbimModel.Logger.ErrorFormat("Parser error, the Attribute {0} of {1} is incorrectly specified and has been ignored. {2}",
                       _currentInstance.CurrentParamIndex,
                        _currentInstance.Entity.GetType().Name,
                        e.Message);
                }
               
            }
            if (_listNestLevel == 0)
                _currentInstance.CurrentParamIndex++;
        }

        internal void SetIntegerValue(long value)
        {
            _propertyValue.Init(value, IfcParserType.Integer);
            SetEntityParameter();
        }

        internal void SetHexValue(double value)
        {
            _propertyValue.Init(value, IfcParserType.HexaDecimal);
            SetEntityParameter();
        }

        internal void SetFloatValue(double value)
        {
            _propertyValue.Init(value, IfcParserType.Real);
            SetEntityParameter();
        }

        internal void SetStringValue(string value)
        {

            _propertyValue.Init(value, IfcParserType.String);
            SetEntityParameter();
        }

        internal void SetEnumValue(string value)
        {
            _propertyValue.Init(value, IfcParserType.Enum);
            SetEntityParameter();
        }

        internal void SetBooleanValue(bool value)
        {
            _propertyValue.Init(value, IfcParserType.Boolean);
            SetEntityParameter();
        }

        internal void SetNonDefinedValue()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal void SetOverrideValue()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal void SetObjectValue(IPersistIfc value)
        {
            _propertyValue.Init(value);
            //CurrentInstance.SetPropertyValue(PropertyValue);
            _currentInstance.Entity.IfcParse(_currentInstance.CurrentParamIndex, _propertyValue);
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal void SkipProperty()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal IfcMetaProperty CurrentProperty
        {
            get
            {
                return IfcMetaData.IfcType(_currentInstance.Entity).IfcProperties[_currentInstance.CurrentParamIndex+1];
            }
        }
        internal short CurrentPropertyId
        {
            get
            {
                return (short) _currentInstance.CurrentParamIndex;
            }
        }

        /// <summary>
        /// Returns true if the parser is working through a list of items
        /// </summary>
        public bool InList 
        {
            get
            {
                return _listNestLevel > 0;
            }
        }
    }
}
