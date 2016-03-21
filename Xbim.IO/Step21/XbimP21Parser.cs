#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    P21toModelParser.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Logging;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Parser;
using Xbim.IO.Step21.Parser;

#endregion

namespace Xbim.IO.Step21
{
    public class XbimP21Parser : P21Parser
    {
        private readonly ILogger _logger = LoggerFactory.GetLogger();
        public event ReportProgressDelegate ProgressStatus;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private int _listNestLevel = -1;
        private Part21Entity _currentInstance;
        public event CreateEntityEventHandler EntityCreate;
        private readonly Dictionary<long, IPersist> _entities;
        private PropertyValue _propertyValue;
        private readonly List<DeferredReference> _deferredReferences;
        private readonly double _streamSize = -1;
        private int _errorCount;
        public static int MaxErrorCount = 100;
        private int _percentageParsed;
        private bool _deferListItems;
        public bool Cancel = false;
        private readonly List<int> _nestedIndex = new List<int>();
        public int[] NestedIndex { get { return _listNestLevel > 0 ? _nestedIndex.ToArray() : null; } }

        private readonly ExpressMetaData _metadata;


        public XbimP21Parser(Stream strm, ExpressMetaData metadata, long streamSize)
            : base(strm)
        {
            _metadata = metadata;
            var entityApproxCount = 5000;
            if (streamSize>0)
            {
                _streamSize = streamSize;
                entityApproxCount = Convert.ToInt32(_streamSize/50); //average 50 bytes per entity.
            }

            _entities = new Dictionary<long, IPersist>(entityApproxCount);
            _deferredReferences = new List<DeferredReference>(entityApproxCount/2); //assume 50% deferred
            _errorCount = 0;
        }

        public int ErrorCount
        {
            get { return _errorCount; }
        }

        internal override void SetErrorMessage()
        {
        }

        protected override void CharacterError()
        {
            _logger.WarnFormat("Error parsing Ifc File, illegal character found");
        }

        protected override void BeginParse()
        {
        }

        protected override void EndParse()
        {
            foreach (var defRef in _deferredReferences)
            {
                if (!TrySetObjectValue(defRef.HostEntity, defRef.ParameterIndex, defRef.ReferenceId, defRef.NestedIndex))
                    _logger.WarnFormat("Entity #{0,-5} is referenced but could not be instantiated",
                                                      defRef.ReferenceId);
            }
        }

        protected override void BeginHeader()
        {
        }

        protected override void EndHeader()
        {
        }

        protected override void BeginScope()
        {
        }

        protected override void EndScope()
        {
        }

        protected override void EndSec()
        {
        }

        protected override void BeginList()
        {
            var p21 = _processStack.Peek();
            if (p21.CurrentParamIndex == -1)
                p21.CurrentParamIndex++; //first time in take the first argument

            _listNestLevel++;
            //  Console.WriteLine("BeginList");
            if (_listNestLevel < 2) return;

            if (_listNestLevel -1 > _nestedIndex.Count)
                _nestedIndex.Add(0);
            else
                _nestedIndex[_listNestLevel - 2]++;
        }

        protected override void EndList()
        {
            _listNestLevel--;
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
            //Console.WriteLine("EndList");

            //we are finished with the list
            if (_listNestLevel <= 0) _nestedIndex.Clear();
        }

        protected override void BeginComplex()
        {
        }

        protected override void EndComplex()
        {
        }

        protected override void NewEntity(string entityLabel)
        {
            _currentInstance = new Part21Entity(entityLabel);
            // Console.WriteLine(CurrentSemanticValue.strVal);
            _processStack.Push(_currentInstance);
            if (_streamSize == -1 || ProgressStatus == null) return;

            var sc = (Scanner) Scanner;
            double pos = sc.Buffer.Pos;
            var newPercentage = Convert.ToInt32(pos/_streamSize*100.0);
            if (newPercentage <= _percentageParsed) return;
            _percentageParsed = newPercentage;
            ProgressStatus(_percentageParsed, "Parsing");
            if (Cancel) YYAccept();
        }

        protected override void SetType(string entityTypeName)
        {
            if (InHeader)
            {
                int[] reqParams;
                _currentInstance = new Part21Entity(EntityCreate(entityTypeName, null, InHeader, out reqParams))
                {
                    RequiredParameters = reqParams
                };
                if(_currentInstance != null) _processStack.Push(_currentInstance);
            }
            else
            {
                var p21 = _processStack.Peek();
                int[] reqProps;
                p21.Entity = EntityCreate(entityTypeName, p21.EntityLabel, InHeader, out reqProps);
                p21.RequiredParameters = reqProps;
            }
            if (Cancel) YYAccept();
        }

        protected override void EndEntity()
        {
            var p21 = _processStack.Pop();
            //Debug.Assert(_processStack.Count == 0);
            _currentInstance = null;
            if (p21.Entity != null)
                _entities.Add(p21.EntityLabel, p21.Entity);


            // Console.WriteLine("EndEntity - " + CurrentSemanticValue.strVal);
        }

        protected override void EndHeaderEntity()
        {
            _processStack.Pop();

            _currentInstance = null;
            // Console.WriteLine("EndHeaderEntity - " + CurrentSemanticValue.strVal);
        }

        protected override void SetIntegerValue(string value)
        {
            _propertyValue.Init(value, StepParserType.Integer);
            SetEntityParameter(value);
        }

        protected override void SetHexValue(string value)
        {
            _propertyValue.Init(value, StepParserType.HexaDecimal);
            SetEntityParameter(value);
        }

        protected override void SetFloatValue(string value)
        {
            _propertyValue.Init(value, StepParserType.Real);
            SetEntityParameter(value);
        }

        protected override void SetStringValue(string value)
        {
            _propertyValue.Init(value, StepParserType.String);
            SetEntityParameter(value);
        }

        protected override void SetEnumValue(string value)
        {
            _propertyValue.Init(value.Trim('.'), StepParserType.Enum);
            SetEntityParameter(value);
        }

        protected override void SetBooleanValue(string value)
        {
            _propertyValue.Init(value, StepParserType.Boolean);
            SetEntityParameter(value);
        }

        protected override void SetNonDefinedValue()
        {
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void SetOverrideValue()
        {
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void SetObjectValue(string value)
        {
            var refId = Convert.ToInt32(value.TrimStart('#'));
            if (!TrySetObjectValue(_currentInstance.Entity, _currentInstance.CurrentParamIndex, refId, NestedIndex))
            {
                _deferredReferences.Add(new DeferredReference(_currentInstance.CurrentParamIndex,
                                                              _currentInstance.Entity, refId, NestedIndex));
                if (_listNestLevel != 0) _deferListItems = true;
            }
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void EndNestedType(string value)
        {
            try
            {
                _propertyValue.Init(_processStack.Pop().Entity);
                _currentInstance = _processStack.Peek();
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
            }
            catch (Exception )
            {
                if (_errorCount > MaxErrorCount)
                    throw new Exception("Too many errors in file, parser execution terminated");
                _errorCount++;
                var mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    var expressType = _metadata.ExpressType(mainEntity.Entity);
                    _logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2}-{3} value = {4}",
                                               mainEntity.EntityLabel, mainEntity.Entity.GetType().Name.ToUpper(),
                                               mainEntity.CurrentParamIndex + 1,
                                               expressType.Properties[mainEntity.CurrentParamIndex + 1].PropertyInfo.Name,
                                               value);
                }
                else
                {
                    _logger.Error("Unhandled Parser error, in Parser.cs EndNestedType");
                }
            }
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void BeginNestedType(string value)
        {
            int[] reqProps;
            if (EntityCreate != null)
                _currentInstance = new Part21Entity(EntityCreate(value, null, InHeader, out reqProps))
                {
                    RequiredParameters = reqProps
                };
            _processStack.Push(_currentInstance);
            if (Cancel) YYAccept();
        }

        private void SetEntityParameter(string value)
        {
            try
            {
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
            }
            catch (Exception )
            {
                if (_errorCount > MaxErrorCount)
                    throw new Exception("Too many errors in file, parser execution terminated");
                _errorCount++;
                var mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    if (_metadata != null)
                    {
                        var expressType = _metadata.ExpressType(mainEntity.Entity);


                        var propertyName = mainEntity.CurrentParamIndex + 1 > expressType.Properties.Count
                            ? "[UnknownProperty]"
                            : expressType.Properties[mainEntity.CurrentParamIndex + 1].PropertyInfo.Name;

                        _logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2}-{3} value = {4}",
                            mainEntity.EntityLabel,
                            mainEntity.Entity.GetType().Name.ToUpper(),
                            mainEntity.CurrentParamIndex + 1,
                            propertyName,
                            value);
                    }
                    else
                    {
                        _logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2} value = {3}",
                           mainEntity.EntityLabel,
                           mainEntity.Entity.GetType().Name.ToUpper(),
                           mainEntity.CurrentParamIndex + 1,                          
                           value);
                    }

                }
                else
                {
                    _logger.Error("Unhandled Parser error, in Parser.cs SetEntityParameter");
                }
            }
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        public Dictionary<long, IPersist> Entities
        {
            get { return _entities; }
        }

        internal bool TrySetObjectValue(IPersist host, int paramIndex, int refId, int[] listNextLevel)
        {
            if (_deferListItems) return false;
            try
            {
                IPersist refEntity;
                if (_entities.TryGetValue(refId, out refEntity) && host != null)
                {
                    _propertyValue.Init(refEntity);
                    (host).Parse(paramIndex, _propertyValue, listNextLevel);
                    return true;
                }
            }
            catch (Exception )
            {
                if (_errorCount > MaxErrorCount)
                    throw new Exception("Too many errors in file, parser execution terminated");
                _errorCount++;
                var expressType = _metadata.ExpressType(host);
                var propertyName = paramIndex+1 > expressType.Properties.Count ? "[UnknownProperty]" :
                        expressType.Properties[paramIndex+1].PropertyInfo.Name;
                _logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2}-{3}",
                                           refId, expressType.Type.Name.ToUpper(), paramIndex + 1,
                                           propertyName);
                
            }
            return false;
        }
    }

    public struct DeferredReference
    {
        public DeferredReference(int paramIndex, IPersist hostEntity, int refId, int[] nestedIndex)
        {
            ParameterIndex = paramIndex;
            HostEntity = hostEntity;
            ReferenceId = refId;
            NestedIndex = nestedIndex;
        }

        public int ParameterIndex;
        public IPersist HostEntity;
        public int ReferenceId; //the ID of the object to set at ParameterIndex of HostEntity
        public int[] NestedIndex;
        public ParameterSetter ParameterSetter
        {
            get { return (HostEntity).Parse; }
        }
    }
}