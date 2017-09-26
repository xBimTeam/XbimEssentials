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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Optimized;
using Xbim.IO.Parser;
using Xbim.IO.Step21.Parser;

#endregion

namespace Xbim.IO.Step21
{
    public class XbimP21Scanner
    {
        private ILogger _logger;
        public ILogger Logger { get { return _logger; } set { _logger = value; } }
        public event ReportProgressDelegate ProgressStatus;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        protected int ListNestLevel = -1;
        protected Part21Entity CurrentInstance;
        public CreateEntityDelegate EntityCreate;
        private Dictionary<long, IPersist> _entities;
        protected PropertyValue PropertyValue;
        private List<DeferredReference> _deferredReferences;
        private double _streamSize = -1;
        public static int MaxErrorCount = 100;
        private int _percentageParsed;
        private bool _deferListItems;
        private readonly List<int> _nestedIndex = new List<int>();

        public int[] NestedIndex
        {
            get { return ListNestLevel > 0 ? _nestedIndex.ToArray() : null; }
        }

        //private Optimized.Scanner _scanner;
        private IO.Parser.Scanner _scanner;
        private bool _inHeader;

        public XbimP21Scanner(Stream strm, long streamSize)
        {
            _scanner = new IO.Parser.Scanner(strm);
            //_scanner = new Optimized.Scanner();
            //_scanner.SetSource(new XbimScanBuffer(strm));

            var entityApproxCount = 50000;
            if (streamSize > 0)
            {
                _streamSize = streamSize;
                entityApproxCount = Convert.ToInt32(_streamSize / 50); //average 50 bytes per entity.
            }

            _entities = new Dictionary<long, IPersist>(entityApproxCount);
            _deferredReferences = new List<DeferredReference>(entityApproxCount / 2); //assume 50% deferred
        }

        //public XbimP21Scanner(string data)
        //{
        //    _scanner = new Scanner();
        //    _scanner.SetSource(data, 0);

        //    var entityApproxCount = 50000;
        //    _entities = new Dictionary<long, IPersist>(entityApproxCount);
        //    _deferredReferences = new List<DeferredReference>(entityApproxCount / 2); //assume 50% deferred
        //}

        public void Parse()
        { 
            var tok = _scanner.yylex();
            while (tok != (int)Tokens.EOF)
            {
                if (tok >= 63)
                {
                    Tokens t = (Tokens)tok;
                    switch (t)
                    {
                        case Tokens.HEADER:
                            BeginHeader();
                            break;
                        case Tokens.ENDSEC:
                            EndSec();
                            break;
                        case Tokens.DATA:
                            BeginData();
                            break;
                        case Tokens.ENTITY:
                            NewEntity(_scanner.yylval.strVal);
                            break;
                        case Tokens.TYPE:
                            SetType(_scanner.yylval.strVal);
                            break;
                        case Tokens.INTEGER:
                            SetIntegerValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.FLOAT:
                            SetFloatValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.STRING:
                            SetStringValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.BOOLEAN:
                            SetBooleanValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.IDENTITY:
                            SetObjectValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.HEXA:
                            SetHexValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.ENUM:
                            SetEnumValue(_scanner.yylval.strVal);
                            break;
                        case Tokens.NONDEF:
                            SetNonDefinedValue();
                            break;
                        case Tokens.OVERRIDE:
                            SetOverrideValue();
                            break;

                        case Tokens.TEXT:
                        case Tokens.error:
                        case Tokens.ILLEGALCHAR:
                            throw new XbimParserException("Unexpected scanner token 'TEXT'");

                        case Tokens.SCOPE:
                        case Tokens.ENDSCOPE:
                        case Tokens.ISOSTEPSTART:
                        case Tokens.ISOSTEPEND:
                        case Tokens.MISC:
                        case Tokens.EOF:
                        default:
                            break;
                    }
                }
                else
                {
                    char c = (char)tok;
                    switch (c)
                    {
                        case '(':
                            BeginList();
                            break;
                        case ')':
                            EndList();
                            break;
                        case ';':
                            EndEntity();
                            break;
                        case '/':
                        case ',':
                        case '=':
                        default:
                            break;
                    }
                }

                tok = _scanner.yylex();
            }
            EndParse();
        }


        public int ErrorCount { get; protected set; } = 0;

        public LexLocation CurrentPosition { get { return _scanner.yylloc; } }

        protected void EndParse()
        {
            foreach (var defRef in _deferredReferences)
            {
                if (!TrySetObjectValue(defRef.HostEntity, defRef.ParameterIndex, defRef.ReferenceId, defRef.NestedIndex))
                    Logger?.LogWarning("Entity #{0,-5} is referenced but could not be instantiated",
                                                      defRef.ReferenceId);
            }
            _deferredReferences.Clear();
        }

        protected void BeginHeader()
        {
            _inHeader = true;
        }

        protected void BeginData()
        {
            if (_inHeader)
            {
                _inHeader = false;
            }
        }

        protected void EndSec()
        {
            if (_inHeader)
            {
                _inHeader = false;
            }
        }

        protected void BeginList()
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

        protected void EndList()
        {
            ListNestLevel--;
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
            //Console.WriteLine("EndList");

            //we are finished with the list
            if (ListNestLevel <= 0)
                _nestedIndex.Clear();

            if (_isInNestedType)
                EndNestedType();
        }

        protected void NewEntity(string entityLabel)
        {
            CurrentInstance = new Part21Entity(entityLabel);
            // Console.WriteLine(CurrentSemanticValue.strVal);
            _processStack.Push(CurrentInstance);
            if (_streamSize == -1 || ProgressStatus == null)
                return;

            double pos = _scanner.Buffer.Pos;
            var newPercentage = Convert.ToInt32(pos / _streamSize * 100.0);
            if (newPercentage <= _percentageParsed) return;
            _percentageParsed = newPercentage;
            ProgressStatus(_percentageParsed, "Parsing");
        }

        protected void SetType(string entityTypeName)
        {
            if (_inHeader)
            {
                // instantiates an empty IPersist from the header information
                var t = EntityCreate(entityTypeName, null, _inHeader, out int[] reqParams);
                // then attaches it to a new Part21Entity, this will be processed later from the _processStack
                // to debug value initialisation place a breakpoint on the Parse() function of 
                // StepFileName, StepFileSchema or StepFileDescription classes.
                CurrentInstance = new Part21Entity(t)
                {
                    RequiredParameters = reqParams
                };
                if (CurrentInstance != null) _processStack.Push(CurrentInstance);
            }
            else
            {
                if (ListNestLevel == -1)
                {
                    var p21 = _processStack.Peek();
                    p21.Entity = EntityCreate(entityTypeName, p21.EntityLabel, _inHeader, out int[] reqProps);
                }
                else
                {
                    BeginNestedType(entityTypeName);
                }
            }
        }

        protected void EndEntity()
        {
            var p21 = _processStack.Pop();
            //Debug.Assert(_processStack.Count == 0);
            CurrentInstance = null;
            if (!_inHeader && p21.Entity != null)
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
        }

        protected void SetIntegerValue(string value)
        {
            PropertyValue.Init(value, StepParserType.Integer);
            SetEntityParameter(value);
        }

        protected void SetHexValue(string value)
        {
            PropertyValue.Init(value, StepParserType.HexaDecimal);
            SetEntityParameter(value);
        }

        protected void SetFloatValue(string value)
        {
            PropertyValue.Init(value, StepParserType.Real);
            SetEntityParameter(value);
        }

        protected void SetStringValue(string value)
        {
            PropertyValue.Init(value, StepParserType.String);
            SetEntityParameter(value);
        }

        protected void SetEnumValue(string value)
        {
            PropertyValue.Init(value.Trim('.'), StepParserType.Enum);
            SetEntityParameter(value);
        }

        protected void SetBooleanValue(string value)
        {
            PropertyValue.Init(value, StepParserType.Boolean);
            SetEntityParameter(value);
        }

        protected void SetNonDefinedValue()
        {
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected void SetOverrideValue()
        {
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        protected void SetObjectValue(string value)
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

        protected void EndNestedType()
        {
            _isInNestedType = false;
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
                    Logger?.LogError("Entity #{0,-5} {1}, error at parameter {2}",
                                               mainEntity.EntityLabel, mainEntity.Entity.GetType().Name.ToUpper(),
                                               mainEntity.CurrentParamIndex + 1
                                               );
                }
                else
                {
                    Logger?.LogError("Unhandled Parser error, in Parser.cs EndNestedType");
                }
            }
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        private bool _isInNestedType;
        protected void BeginNestedType(string value)
        {
            if (EntityCreate != null)
                CurrentInstance = new Part21Entity(EntityCreate(value, null, _inHeader, out int[] reqProps))
                {
                    RequiredParameters = reqProps
                };
            _processStack.Push(CurrentInstance);
            _isInNestedType = true;
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
                    Logger?.LogError("Entity #{0,-5} {1}, error at parameter {2} value = {3}",
                       mainEntity.EntityLabel,
                       mainEntity.Entity.GetType().Name.ToUpper(),
                       mainEntity.CurrentParamIndex + 1,
                       value);
                }
                else
                {
                    Logger?.LogError("Unhandled Parser error, in Parser.cs SetEntityParameter");
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
                Logger?.LogError("Entity #{0,-5} {1}, error at parameter {2}",
                                           refId, host.GetType().Name.ToUpper(), paramIndex + 1
                                           );

            }
            return false;
        }
    }

    public delegate IPersist CreateEntityDelegate(string className, long? label, bool headerEntity, out int[] i);

}