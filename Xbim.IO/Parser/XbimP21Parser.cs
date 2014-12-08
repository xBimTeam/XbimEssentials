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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions;
using Xbim.Common.Logging;

#endregion

namespace Xbim.IO.Parser
{
    public class XbimP21Parser : P21Parser
    {
        private readonly ILogger Logger = LoggerFactory.GetLogger();
        public event ReportProgressDelegate ProgressStatus;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private int _listNestLevel = -1;
        private Part21Entity _currentInstance;
        public event CreateEntityEventHandler EntityCreate;
        private readonly Dictionary<long, IPersistIfc> _entities;
        private PropertyValue _propertyValue;
        private readonly List<DeferredReference> _deferredReferences;
        private readonly double _streamSize = -1;
        private int _errorCount;
        public static int MaxErrorCount = 100;
        private int _percentageParsed;
        private bool _deferListItems;

       


        public XbimP21Parser(Stream strm)
            : base(strm)
        {
            int entityApproxCount = 5000;
            if (strm.CanSeek)
            {
                _streamSize = strm.Length;
                entityApproxCount = Convert.ToInt32(_streamSize/50); //average 50 bytes per entity.
            }

            _entities = new Dictionary<long, IPersistIfc>(entityApproxCount);
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

        internal override void CharacterError()
        {
        }

        internal override void BeginParse()
        {
        }

        internal override void EndParse()
        {
            foreach (DeferredReference defRef in _deferredReferences)
            {
                if (!TrySetObjectValue(defRef.HostEntity, defRef.ParameterIndex, defRef.ReferenceID))
                    Logger.WarnFormat("Entity #{0,-5} is referenced but could not be instantiated",
                                                      defRef.ReferenceID);
            }
        }

        internal override void BeginHeader()
        {
        }

        internal override void EndHeader()
        {
        }

        internal override void BeginScope()
        {
        }

        internal override void EndScope()
        {
        }

        internal override void EndSec()
        {
        }

        internal override void BeginList()
        {
            Part21Entity p21 = _processStack.Peek();
            if (p21.CurrentParamIndex == -1)
                p21.CurrentParamIndex++; //first time in take the first argument

            _listNestLevel++;
            //  Console.WriteLine("BeginList");
        }

        internal override void EndList()
        {
            _listNestLevel--;
            Part21Entity p21 = _processStack.Peek();
            p21.CurrentParamIndex++;
            //Console.WriteLine("EndList");
        }

        internal override void BeginComplex()
        {
        }

        internal override void EndComplex()
        {
        }

        internal override void NewEntity(string entityLabel)
        {
            _currentInstance = new Part21Entity(entityLabel);
            // Console.WriteLine(CurrentSemanticValue.strVal);
            _processStack.Push(_currentInstance);
            if (_streamSize != -1 && ProgressStatus != null)
            {
                Scanner sc = (Scanner) this.Scanner;
                double pos = sc.Buffer.Pos;
                int newPercentage = Convert.ToInt32(pos/_streamSize*100.0);
                if (newPercentage > _percentageParsed)
                {
                    _percentageParsed = newPercentage;
                    ProgressStatus(_percentageParsed, "Parsing");
                }
            }
        }

        internal override void SetType(string entityTypeName)
        {
            Scanner sc = (Scanner) this.Scanner;

            if (InHeader)
            {
                int[] reqParams;
                _currentInstance = new Part21Entity(EntityCreate(entityTypeName, null, InHeader, out reqParams));
                _currentInstance.RequiredParameters = reqParams;
                _processStack.Push(_currentInstance);
            }
            else
            {
                Part21Entity p21 = _processStack.Peek();
                int[] reqProps;
                p21.Entity = EntityCreate(entityTypeName, p21.EntityLabel, InHeader, out reqProps);
                p21.RequiredParameters = reqProps;
            }
        }

        internal override void EndEntity()
        {
            Part21Entity p21 = _processStack.Pop();
            //Debug.Assert(_processStack.Count == 0);
            _currentInstance = null;
            if (p21.Entity != null)
                _entities.Add(p21.EntityLabel, p21.Entity);


            // Console.WriteLine("EndEntity - " + CurrentSemanticValue.strVal);
        }

        internal override void EndHeaderEntity()
        {
            _processStack.Pop();

            _currentInstance = null;
            // Console.WriteLine("EndHeaderEntity - " + CurrentSemanticValue.strVal);
        }

        internal override void SetIntegerValue(string value)
        {
            _propertyValue.Init(value, IfcParserType.Integer);
            SetEntityParameter(value);
        }

        internal override void SetHexValue(string value)
        {
            _propertyValue.Init(value, IfcParserType.HexaDecimal);
            SetEntityParameter(value);
        }

        internal override void SetFloatValue(string value)
        {
            _propertyValue.Init(value, IfcParserType.Real);
            SetEntityParameter(value);
        }

        internal override void SetStringValue(string value)
        {
            _propertyValue.Init(value, IfcParserType.String);
            SetEntityParameter(value);
        }

        internal override void SetEnumValue(string value)
        {
            _propertyValue.Init(value.Trim('.'), IfcParserType.Enum);
            SetEntityParameter(value);
        }

        internal override void SetBooleanValue(string value)
        {
            _propertyValue.Init(value, IfcParserType.Boolean);
            SetEntityParameter(value);
        }

        internal override void SetNonDefinedValue()
        {
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        internal override void SetOverrideValue()
        {
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        internal override void SetObjectValue(string value)
        {
            int refID = Convert.ToInt32(value.TrimStart('#'));
            if (!TrySetObjectValue(_currentInstance.Entity, _currentInstance.CurrentParamIndex, refID))
            {
                _deferredReferences.Add(new DeferredReference(_currentInstance.CurrentParamIndex,
                                                              _currentInstance.Entity, refID));
                if (_listNestLevel != 0) _deferListItems = true;
            }
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        internal override void EndNestedType(string value)
        {
            try
            {
                _propertyValue.Init(_processStack.Pop().Entity);
                _currentInstance = _processStack.Peek();
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
            }
            catch (Exception )
            {
                if (_errorCount > MaxErrorCount)
                    throw new Exception("Too many errors in file, parser execution terminated");
                _errorCount++;
                Part21Entity mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    IfcType ifcType = IfcMetaData.IfcType(mainEntity.Entity);
                    Logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2}-{3} value = {4}",
                                               mainEntity.EntityLabel, mainEntity.Entity.GetType().Name.ToUpper(),
                                               mainEntity.CurrentParamIndex + 1,
                                               ifcType.IfcProperties[mainEntity.CurrentParamIndex + 1].PropertyInfo.Name,
                                               value);
                }
                else
                {
                    Logger.Error("Unhandled Parser error, in Parser.cs EndNestedType");
                }
            }
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        internal override void BeginNestedType(string value)
        {
            int[] reqProps;
            _currentInstance = new Part21Entity(EntityCreate(value, null, InHeader, out reqProps));
            _currentInstance.RequiredParameters = reqProps;
            _processStack.Push(_currentInstance);
        }

        private void SetEntityParameter(string value)
        {
            try
            {
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
            }
            catch (Exception )
            {
                if (_errorCount > MaxErrorCount)
                    throw new Exception("Too many errors in file, parser execution terminated");
                _errorCount++;
                Part21Entity mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    IfcType ifcType = IfcMetaData.IfcType(mainEntity.Entity);

                    string propertyName = mainEntity.CurrentParamIndex + 1 > ifcType.IfcProperties.Count ? "[UnknownProperty]" :
                        ifcType.IfcProperties[mainEntity.CurrentParamIndex + 1].PropertyInfo.Name;

                    Logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2}-{3} value = {4}",
                                               mainEntity.EntityLabel, 
                                               mainEntity.Entity.GetType().Name.ToUpper(),
                                               mainEntity.CurrentParamIndex + 1,
                                               propertyName,
                                               value);
                   
                }
                else
                {
                    Logger.Error("Unhandled Parser error, in Parser.cs SetEntityParameter");
                }
            }
            if (_listNestLevel == 0)
            {
                _currentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        public Dictionary<long, IPersistIfc> Entities
        {
            get { return _entities; }
        }

        internal bool TrySetObjectValue(IPersistIfc host, int paramIndex, int refID)
        {
            if (_deferListItems) return false;
            try
            {
                IPersistIfc refEntity;
                if (_entities.TryGetValue(refID, out refEntity) && host != null)
                {
                    _propertyValue.Init(refEntity);
                    (host).IfcParse(paramIndex, _propertyValue);
                    return true;
                }
            }
            catch (Exception )
            {
                if (_errorCount > MaxErrorCount)
                    throw new Exception("Too many errors in file, parser execution terminated");
                _errorCount++;
                IfcType ifcType = IfcMetaData.IfcType(host);
                string propertyName = paramIndex+1 > ifcType.IfcProperties.Count ? "[UnknownProperty]" :
                        ifcType.IfcProperties[paramIndex+1].PropertyInfo.Name;
                Logger.ErrorFormat("Entity #{0,-5} {1}, error at parameter {2}-{3}",
                                           refID, ifcType.Type.Name.ToUpper(), paramIndex + 1,
                                           propertyName);
                
            }
            return false;
        }
    }

    public struct DeferredReference
    {
        public DeferredReference(int paramIndex, IPersistIfc hostEntity, int refID)
        {
            ParameterIndex = paramIndex;
            HostEntity = hostEntity;
            ReferenceID = refID;
        }

        public int ParameterIndex;
        public IPersistIfc HostEntity;
        public int ReferenceID; //the ID of the object to set at ParameterIndex of HostEntity

        public ParameterSetter ParameterSetter
        {
            get { return (HostEntity).IfcParse; }
        }
    }
}