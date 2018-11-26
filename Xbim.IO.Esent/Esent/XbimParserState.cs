using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Parser;

namespace Xbim.IO.Esent
{
    public class XbimParserState
    {

        private readonly ILogger _logger;
        private readonly ExpressMetaData _metadata;
        private readonly List<int> _nestedIndex = new List<int>();
        public int[] NestedIndex { get { return _listNestLevel > 0 ? _nestedIndex.ToArray() : null; } }

        public XbimParserState(IPersistEntity entity, ILogger logger = null)
        {
            _logger = logger ?? XbimLogging.CreateLogger<EsentModel>();
            _currentInstance = new Part21Entity(entity);
            _processStack.Push(_currentInstance);
            _metadata = entity.Model.Metadata;
        }

        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private int _listNestLevel = -1;
        private Part21Entity _currentInstance;
        private readonly IndexPropertyValue _propertyValue = new IndexPropertyValue();

        public void BeginList()
        {
            var p21 = _processStack.Peek();
            if (p21.CurrentParamIndex == -1)
                p21.CurrentParamIndex++; //first time in take the first argument

            _listNestLevel++;

            if (_listNestLevel < 2) return;

            if (_listNestLevel - 1 > _nestedIndex.Count)
                _nestedIndex.Add(0);
            else
                _nestedIndex[_listNestLevel - 2]++;
        }

        public void EndList()
        {
            _listNestLevel--;
            
            if (_listNestLevel == 0)
                _currentInstance.CurrentParamIndex++;

            //we are finished with the list
            if(_listNestLevel <= 0) _nestedIndex.Clear();
        }

        public void EndEntity()
        {
            _processStack.Pop();
            //Debug.Assert(_processStack.Count == 0);
        }

        internal void BeginNestedType(string typeName)
        {
            var type = _metadata.ExpressType(typeName);
            _currentInstance = new Part21Entity((IPersist)Activator.CreateInstance(type.Type));
            _processStack.Push(_currentInstance);
        }

        internal void EndNestedType()
        {
            _propertyValue.Init(_processStack.Pop().Entity);
            _currentInstance = _processStack.Peek();
            if (_currentInstance.Entity != null)
                _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        private void SetEntityParameter()
        {
            if (_currentInstance.Entity != null)
            {
                //CurrentInstance.SetPropertyValue(PropertyValue);
                try
                {
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
                }
                catch (Exception e)
                {
                    if (_logger != null)
                        _logger.LogError("Parser error, the Attribute {0} of {1} is incorrectly specified and has been ignored. {2}",
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
            _propertyValue.Init(value, StepParserType.Integer);
            SetEntityParameter();
        }

        internal void SetHexValue(byte[] value)
        {
            _propertyValue.Init(value, StepParserType.HexaDecimal);
            SetEntityParameter();
        }

        internal void SetFloatValue(double value)
        {
            _propertyValue.Init(value, StepParserType.Real);
            SetEntityParameter();
        }

        internal void SetStringValue(string value)
        {

            _propertyValue.Init(value, StepParserType.String);
            SetEntityParameter();
        }

        internal void SetEnumValue(string value)
        {
            _propertyValue.Init(value, StepParserType.Enum);
            SetEntityParameter();
        }

        internal void SetBooleanValue(bool value)
        {
            _propertyValue.Init(value, StepParserType.Boolean);
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

        internal void SetObjectValue(IPersist value)
        {
            _propertyValue.Init(value);
            //CurrentInstance.SetPropertyValue(PropertyValue);
            _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal void SkipProperty()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal ExpressMetaProperty CurrentProperty
        {
            get
            {
                return _metadata.ExpressType(_currentInstance.Entity).Properties[_currentInstance.CurrentParamIndex+1];
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
