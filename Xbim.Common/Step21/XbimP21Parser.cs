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

using Microsoft.Extensions.Logging;
using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Parser;
using Xbim.IO.Step21.Parser;

#endregion

namespace Xbim.IO.Step21
{
    public class XbimP21Parser : P21Parser
    {
        public ILogger Logger { get; private set; }
        public event ReportProgressDelegate ProgressStatus;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        protected int ListNestLevel = -1;
        protected Part21Entity CurrentInstance;
        public event CreateEntityEventHandler EntityCreate;
        private readonly Dictionary<long, IPersist> _entities;
        protected PropertyValue PropertyValue;
        private readonly List<DeferredReference> _deferredReferences;
        private readonly double _streamSize = -1;
        public static int MaxErrorCount = 200;
        private int _percentageParsed;
        private bool _deferListItems;
        public bool Cancel = false;
        private readonly List<int> _nestedIndex = new List<int>();

        public int[] NestedIndex
        {
            get { return ListNestLevel > 0 ? _nestedIndex.ToArray() : null; }
        }

        public XbimP21Parser(Stream strm, long streamSize , ILogger logger)
            : base(strm)
        {
            Logger = logger ?? XbimLogging.CreateLogger<XbimP21Parser>();
            var entityApproxCount = 5000;
            if (streamSize > 0)
            {
                _streamSize = streamSize;
                entityApproxCount = Convert.ToInt32(_streamSize / 50); //average 50 bytes per entity.
            }

            _entities = new Dictionary<long, IPersist>(entityApproxCount);
            _deferredReferences = new List<DeferredReference>(entityApproxCount / 2); //assume 50% deferred
            ErrorCount = 0;
        }

        protected XbimP21Parser(ILogger logger)
        {
            Logger = logger ?? XbimLogging.CreateLogger<XbimP21Parser>();
            const int entityApproxCount = 5000;
            _entities = new Dictionary<long, IPersist>(entityApproxCount);
            _deferredReferences = new List<DeferredReference>(entityApproxCount / 2); //assume 50% deferred
            ErrorCount = 0;
        }

        public int ErrorCount { get; protected set; }

        /// <summary>
        /// Returns current position as [line, column] integer array. This is usefull when debuging misformatted files.
        /// </summary>
        public LexLocation CurrentPosition
        {
            get
            {
                return Scanner.yylloc;
            }
        }

        protected override void SetErrorMessage()
        {
        }

        protected override void CharacterError()
        {
            Logger?.LogWarning("Error parsing IFC File, illegal character found");
        }

        protected override void BeginParse()
        {
        }

        protected override void EndParse()
        {
            foreach (var defRef in _deferredReferences)
            {
                if (!TrySetObjectValue(defRef.HostEntity, defRef.ParameterIndex, defRef.ReferenceId, defRef.NestedIndex))
                    Logger?.LogWarning("Entity #{0,-5} is referenced but could not be instantiated",
                                                      defRef.ReferenceId);
            }
            _deferredReferences.Clear();
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

            ListNestLevel++;
            //  Console.WriteLine("BeginList");
            if (ListNestLevel < 2) return;

            if (ListNestLevel - 1 > _nestedIndex.Count)
                _nestedIndex.Add(0);
            else
                _nestedIndex[ListNestLevel - 2]++;
        }

        protected override void EndList()
        {
            ListNestLevel--;
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
            //Console.WriteLine("EndList");

            //we are finished with the list
            if (ListNestLevel <= 0) _nestedIndex.Clear();
        }

        protected override void BeginComplex()
        {
        }

        protected override void EndComplex()
        {
        }

        protected override void NewEntity(string entityLabel)
        {
            CurrentInstance = new Part21Entity(entityLabel);
            // Console.WriteLine(CurrentSemanticValue.strVal);
            _processStack.Push(CurrentInstance);
            if (_streamSize == -1 || ProgressStatus == null) return;

            var sc = (Scanner)Scanner;
            double pos = sc.Buffer.Pos;
            var newPercentage = Convert.ToInt32(pos / _streamSize * 100.0);
            if (newPercentage <= _percentageParsed) return;
            _percentageParsed = newPercentage;
            ProgressStatus(_percentageParsed, "Parsing");
            if (Cancel) YYAccept();
        }

        protected override void SetType(string entityTypeName)
        {
            if (InHeader)
            {
                // instantiates an empty IPersist from the header information
                var t = EntityCreate(entityTypeName, null, InHeader, out int[] reqParams);
                // then attaches it to a new Part21Entity, this will be processed later from the _processStack
                // to debug value initialisation place a breakpoint on the Parse() function of 
                // StepFileName, StepFileSchema or StepFileDescription classes.
                CurrentInstance = new Part21Entity(t);
                if (CurrentInstance != null) _processStack.Push(CurrentInstance);
            }
            else
            {
                var p21 = _processStack.Peek();
                p21.Entity = EntityCreate(entityTypeName, p21.EntityLabel, InHeader, out int[] reqProps);
                //p21.RequiredParameters = reqProps;
            }
            if (Cancel) YYAccept();
        }

        protected override void EndEntity()
        {
            // Check if stack is empty to avoid exception
            if (0 == _processStack.Count)
            {
                Logger.LogError("Stack is empty");
                return;
            }
            var p21 = _processStack.Pop();
            //Debug.Assert(_processStack.Count == 0);
            CurrentInstance = null;
            if (p21.Entity != null)
            {
                try
                {
                    _entities.Add(p21.EntityLabel, p21.Entity);
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Duplicate entity label: #{0}", p21.EntityLabel);
                    Logger?.LogError(msg, ex);
                }
            }
            // Console.WriteLine("EndEntity - " + CurrentSemanticValue.strVal);
        }

        protected override void EndHeaderEntity()
        {
            _processStack.Pop();

            CurrentInstance = null;
            // Console.WriteLine("EndHeaderEntity - " + CurrentSemanticValue.strVal);
        }

        protected override void SetIntegerValue(string value)
        {
            PropertyValue.Init(value, StepParserType.Integer);
            SetEntityParameter(value);
        }

        protected override void SetHexValue(string value)
        {
            PropertyValue.Init(value, StepParserType.HexaDecimal);
            SetEntityParameter(value);
        }

        protected override void SetFloatValue(string value)
        {
            PropertyValue.Init(value, StepParserType.Real);
            SetEntityParameter(value);
        }

        protected override void SetStringValue(string value)
        {
            PropertyValue.Init(value, StepParserType.String);
            SetEntityParameter(value);
        }

        protected override void SetEnumValue(string value)
        {
            PropertyValue.Init(value.Trim('.'), StepParserType.Enum);
            SetEntityParameter(value);
        }

        protected override void SetBooleanValue(string value)
        {
            PropertyValue.Init(value, StepParserType.Boolean);
            SetEntityParameter(value);
        }

        protected override void SetNonDefinedValue()
        {
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void SetOverrideValue()
        {
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void SetObjectValue(string value)
        {
            var refId = Convert.ToInt32(value.TrimStart('#'));
            if (!TrySetObjectValue(CurrentInstance.Entity, CurrentInstance.CurrentParamIndex, refId, NestedIndex))
            {
                _deferredReferences.Add(new DeferredReference(CurrentInstance.CurrentParamIndex,
                                                              CurrentInstance.Entity, refId, NestedIndex));
                if (ListNestLevel != 0) _deferListItems = true;
            }
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void EndNestedType(string value)
        {
            try
            {
                PropertyValue.Init(_processStack.Pop().Entity);
                CurrentInstance = _processStack.Peek();
                if (CurrentInstance.Entity != null)
                    CurrentInstance.Entity.Parse(CurrentInstance.CurrentParamIndex, PropertyValue, NestedIndex);
            }
            catch (Exception)
            {
                if (ErrorCount > MaxErrorCount)
                    throw new XbimParserException("Too many errors in file, parser execution terminated");
                ErrorCount++;
                var mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    Logger?.LogWarning("Entity #{0,-5} {1}, error at parameter {2} value = {4}",
                                               mainEntity.EntityLabel, mainEntity.Entity.GetType().Name.ToUpper(),
                                               mainEntity.CurrentParamIndex + 1,
                                               value);
                }
                else
                {
                    Logger?.LogWarning("Unhandled Parser error, in Parser.cs EndNestedType");
                }
            }
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected override void BeginNestedType(string value)
        {
            if (EntityCreate != null)
                CurrentInstance = new Part21Entity(EntityCreate(value, null, InHeader, out int[] reqProps));
            _processStack.Push(CurrentInstance);
            if (Cancel) YYAccept();
        }

        private void SetEntityParameter(string value)
        {
            try
            {
                if (CurrentInstance.Entity != null)
                    CurrentInstance.Entity.Parse(CurrentInstance.CurrentParamIndex, PropertyValue, NestedIndex);
            }
            catch (Exception)
            {
                if (ErrorCount > MaxErrorCount)
                    throw new XbimParserException("Too many errors in file, parser execution terminated");
                ErrorCount++;
                var mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    Logger?.LogWarning("Entity #{0,-5} {1}, error at parameter {2} value = {3}",
                       mainEntity.EntityLabel,
                       mainEntity.Entity.GetType().Name.ToUpper(),
                       mainEntity.CurrentParamIndex + 1,
                       value);
                }
                else
                {
                    Logger?.LogWarning("Unhandled Parser error, in Parser.cs SetEntityParameter");
                }
            }
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
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
                if (host != null && _entities.TryGetValue(refId, out IPersist refEntity))
                {
                    PropertyValue.Init(refEntity);
                    (host).Parse(paramIndex, PropertyValue, listNextLevel);
                    return true;
                }
            }
            catch (Exception)
            {
                if (ErrorCount > MaxErrorCount)
                    throw new XbimParserException("Too many errors in file, parser execution terminated");
                ErrorCount++;
                Logger?.LogWarning("Entity #{0,-5} {1}, error at parameter {2}",
                                           refId, host.GetType().Name.ToUpper(), paramIndex + 1);

                // this means the case is handled. It would be added to defered references and caused new errors
                return true;

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
    }
}