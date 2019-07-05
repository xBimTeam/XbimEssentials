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
using Xbim.IO.Parser;
using Xbim.IO.Step21.Parser;

#endregion

namespace Xbim.IO.Step21
{
    /// <summary>
    /// This class is meant to replace original parser which was generated using GPPG.
    /// Actual implementation of the original parser was actually only scanner with little
    /// added value but it had significant overhead on processing spead. This implementation
    /// uses the same methods but binds them to the scanner directly. This cuts off part of the
    /// structure security but it also cuts off processing time by about 20%.
    /// </summary>
    public class XbimP21Scanner: IDisposable
    {
        public ILogger Logger { get; private set; }
        private event ReportProgressDelegate _progressStatus;
        private List<ReportProgressDelegate> _progressStatusEvents = new List<ReportProgressDelegate>();
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        protected int ListNestLevel = -1;
        protected Part21Entity CurrentInstance;
        public CreateEntityDelegate EntityCreate;
        protected PropertyValue PropertyValue;
        private List<DeferredReference> _deferredReferences;
        private double _streamSize = -1;
        public static int MaxErrorCount = 200;
        private bool _deferListItems;
        private readonly List<int> _nestedIndex = new List<int>();

        private readonly ParserErrorRegistry _errors = new ParserErrorRegistry();

        /// <summary>
        /// Missing references will not be reported if this is true
        /// </summary>
        public bool AllowMissingReferences { get; set; } = false;

        public HashSet<string> SkipTypes { get; } = new HashSet<string>();
        

        public bool Cancel = false;

        public int[] NestedIndex
        {
            get { return ListNestLevel > 0 ? _nestedIndex.ToArray() : null; }
        }

        public event ReportProgressDelegate ProgressStatus {
            add
            {
                _progressStatus += value;
                _progressStatusEvents.Add(value);
            }
            remove
            {
                _progressStatus += value;
                _progressStatusEvents.Remove(value);
            }
        }

        private Scanner _scanner;
        private bool _inHeader;

        public XbimP21Scanner(Stream strm, long streamSize, IEnumerable<string> ignoreTypes = null)
        {
            Logger = XbimLogging.CreateLogger<XbimP21Scanner>();
            _scanner = new Scanner(strm);
            //_scanner = new Scanner(new XbimScanBuffer(strm));
            if (ignoreTypes != null) SkipTypes = new HashSet<string>(ignoreTypes);
            var entityApproxCount = 50000;
            if (streamSize > 0)
            {
                _streamSize = streamSize;
                entityApproxCount = Convert.ToInt32(_streamSize / 50); //average 50 bytes per entity.
            }
            //adjust for skipped entities
            if(SkipTypes.Any())
            {
                //about a 600 entities
                double adjustRatio = 1.0 - (SkipTypes.Count / 600d);
                if (adjustRatio < 0) adjustRatio = 0;
                entityApproxCount = (int)( entityApproxCount * adjustRatio);
            }

            // make it 4 at least
            if (entityApproxCount < 1) entityApproxCount = 4;

            Entities = new Dictionary<int, IPersist>(entityApproxCount);
            _deferredReferences = new List<DeferredReference>(entityApproxCount / 4); //assume 50% deferred
        }

        public XbimP21Scanner(string data, IEnumerable<string> ignoreTypes = null)
        {
            Logger = XbimLogging.CreateLogger<XbimP21Scanner>();
            _scanner = new Scanner();
            _scanner.SetSource(data, 0);
            _streamSize = data.Length;
            if (ignoreTypes != null) SkipTypes = new HashSet<string>(ignoreTypes);
            var entityApproxCount = (int)_streamSize / 50;
            //adjust for skipped entities
            if (SkipTypes.Any())
            {
                //about a 560 entities
                double adjustRatio = 1-((double)(SkipTypes.Count)) / 560;
                entityApproxCount = (int)(entityApproxCount * adjustRatio);
            }
           
            Entities = new Dictionary<int, IPersist>(entityApproxCount);
            _deferredReferences = new List<DeferredReference>(entityApproxCount / 4); //assume 50% deferred
        }

        public bool Parse(bool onlyHeader = false)
        {
            var skipping = SkipTypes.Any();
            var eofToken = (int)Tokens.EOF;
            var tok = _scanner.yylex();
            int endEntityToken = ';';
            while (tok != eofToken && !Cancel)
            {
                try
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
                                if (_inHeader && onlyHeader)
                                    return true;
                                EndSec();
                                break;
                            case Tokens.DATA:
                                BeginData();
                                break;
                            case Tokens.ENTITY:
                                NewEntity(_scanner.yylval.strVal);
                                break;
                            case Tokens.TYPE:
                                var type = _scanner.yylval.strVal;
                                if (skipping && SkipTypes.Contains(type))
                                {
                                    var current = _processStack.Pop();
                                    //SkipEntities.Add(current.EntityLabel);
                                    while (tok != endEntityToken && tok != eofToken)
                                        tok = _scanner.yylex();
                                    break;
                                }

                                if (!SetType(type))
                                {
                                    // move to the end of entity if we couldn't create it
                                    while (tok != endEntityToken && tok != eofToken)
                                        tok = _scanner.yylex();
                                }
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
                                throw new XbimParserException($"Unexpected scanner token {t.ToString()}, line {_scanner.yylloc.StartLine}, column {_scanner.yylloc.StartColumn}");
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

                    // get next token
                    tok = _scanner.yylex();
                }
                //XbimParserException is a reason to terminate execution
                catch (XbimParserException e)
                {
                    Logger?.LogError(LogEventIds.ParserFailure, e, e.Message);
                    return false;
                }
                //other exceptions might occure but those should just make the parser to wait for the next start of entity
                //and start from there
                catch (Exception e)
                {
                    Logger?.LogError(LogEventIds.FailedEntity, e, e.Message);
                    ErrorCount++;

                    // clear current entity stack to make sure there are no residuals
                    _processStack.Clear();

                    // scan until the beginning of next entity
                    var entityToken = (int)Tokens.ENTITY;
                    while (tok != eofToken && tok != entityToken)
                    {
                        tok = _scanner.yylex();
                    }
                }
            }
            EndParse();
            return ErrorCount == 0;
        }

        private int _errorCount = 0;
        public int ErrorCount
        {
            get { return _errorCount; }
            protected set
            {
                _errorCount = value;
                if (_errorCount > MaxErrorCount)
                {
                    throw new XbimParserException($"Too many errors in the input file ({_errorCount})");
                }
            }
        }

        public LexLocation CurrentPosition { get { return _scanner.yylloc; } }

        protected void EndParse()
        {
            var skipping = SkipTypes.Any(); //if we are skipping then we will have some references unresolved
            foreach (var defRef in _deferredReferences/*.Where(dr => !SkipEntities.Contains(dr.ReferenceId))*/)
            {
                if (!TrySetObjectValue(defRef.HostEntity, defRef.ParameterIndex, defRef.ReferenceId, defRef.NestedIndex) && !skipping && !AllowMissingReferences)
                    Logger?.LogWarning("Entity #{0,-5} is referenced but could not be instantiated",
                                                      defRef.ReferenceId);
            }
            _deferredReferences.Clear();

            if (_errors.Any)
                Logger?.LogWarning(_errors.Summary);

            _progressStatus?.Invoke(100, "Parsing finished.");
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

        /// <summary>
        /// From the resiliance point of view this is a check point.
        /// It needs to make sure that previous entity was finished correctly.
        /// If it wasn't it should log the error, clear the state and start from local point.
        /// </summary>
        /// <param name="entityLabel">Entity Label to create for the new entity</param>
        protected void NewEntity(string entityLabel)
        {
            //last entity wasn't properly finished
            if (_processStack.Count > 0)
            {
                var last = _processStack.Pop();
                Logger.LogError(LogEventIds.FailedEntity, $"Entity #{last.EntityLabel}={last.Entity?.GetType().Name.ToUpperInvariant()} wasn't closed and finished properly.");
                _processStack.Clear();

                ErrorCount++;
            }

            //continue processing anyway because this is a great new start
            var label = GetLabel(entityLabel);
            NewEntity(label);
        }

        private int _reportEntityCount = 0;
        private void NewEntity(int entityLabel)
        {
            CurrentInstance = new Part21Entity(entityLabel);
            // Console.WriteLine(CurrentSemanticValue.strVal);
            _processStack.Push(CurrentInstance);
            if (_streamSize < 0 || _progressStatus == null)
                return;

            if (_reportEntityCount++ < 500)
                return;

            double pos = _scanner.Buffer.Pos;
            var percentage = Convert.ToInt32(pos / _streamSize * 100.0);
            _reportEntityCount = 0;
            _progressStatus?.Invoke(percentage, "Parsing");
        }

        protected bool SetType(string entityTypeName)
        {
            try
            {
                if (_inHeader)
                {
                    // instantiates an empty IPersist from the header information
                    var t = EntityCreate(entityTypeName, null, _inHeader);
                    // then attaches it to a new Part21Entity, this will be processed later from the _processStack
                    // to debug value initialisation place a breakpoint on the Parse() function of 
                    // StepFileName, StepFileSchema or StepFileDescription classes.
                    CurrentInstance = new Part21Entity(t);
                    if (CurrentInstance != null) _processStack.Push(CurrentInstance);
                }
                else
                {
                    if (ListNestLevel == -1)
                    {
                        var p21 = _processStack.Peek();
                        p21.Entity = EntityCreate(entityTypeName, p21.EntityLabel, _inHeader);
                    }
                    else
                    {
                        BeginNestedType(entityTypeName);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                if (_errors.AddTypeNotCreated(entityTypeName))
                {
                    Logger?.LogError(LogEventIds.FailedEntity, e, $"Could not create type {entityTypeName}");
                    ErrorCount++;
                }
                return false;
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
                    Entities.Add(p21.EntityLabel, p21.Entity);
                }
                catch (ArgumentException)
                {
                    var exist = Entities[p21.EntityLabel];
                    var existType = exist.GetType().Name.ToUpperInvariant();
                    var duplType = p21.Entity.GetType().Name.ToUpperInvariant();
                    if (!string.Equals(existType, duplType, StringComparison.Ordinal))
                    {
                        Logger?.LogError(LogEventIds.FailedEntity, $"Duplicate entity label #{p21.EntityLabel} with different types: ({existType}/{duplType})");
                        ErrorCount++;
                    }
                    else
                    {
                        Logger?.LogWarning(LogEventIds.FailedEntity, $"Duplicate entity label #{p21.EntityLabel}={existType}");
                    }
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

        /// <summary>
        /// We already know the shape of the input data exacly
        /// from scanner regex matching so we can convert directly
        /// without any security checks. This is significantly faster
        /// than Convert.ToInt32() which has to examine data again.
        /// This is very frequently called piece of code at the same time
        /// and brings considerable performance savings.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetLabel(string value)
        {
            var label = 0;
            var order = 0;
            // iterate from the end, skip thi first character '#'
            for (int i = value.Length - 1; i > 0; i--)
            {
                var component = value[i] - '0';

                // check if it is actually an integral number
                if (component < 0 || component > 9)
                    continue;

                label += component * magnitudes[order++];
            }
            return label;
        }

        /// <summary>
        /// Direct magnitudes list for positions in the integer string
        /// </summary>
        private static readonly int[] magnitudes = new int[]
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        protected void SetObjectValue(string value)
        {
            var label = GetLabel(value);
            SetObjectValue(label);
        }

        protected void SetObjectValue(int value)
        {
            if (!TrySetObjectValue(CurrentInstance.Entity, CurrentInstance.CurrentParamIndex, value, NestedIndex))
            {
                _deferredReferences.Add(new DeferredReference(CurrentInstance.CurrentParamIndex,
                                                              CurrentInstance.Entity, value, NestedIndex));
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
            catch (Exception e)
            {
                // return silently if this kind of error has already been reported
                if (!_errors.AddPropertyNotSet(CurrentInstance.Entity, CurrentInstance.CurrentParamIndex, PropertyValue, e))
                    return;

                var mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    Logger?.LogError(LogEventIds.FailedEntity, e,  "Entity #{0,-5} {1}, error at parameter {2}",
                                               mainEntity.EntityLabel, mainEntity.Entity.GetType().Name.ToUpper(),
                                               mainEntity.CurrentParamIndex + 1
                                               );
                }
                else
                {
                    Logger?.LogError(LogEventIds.FailedEntity, e, "Unhandled Parser error, in Parser.cs EndNestedType");
                }
                ErrorCount++;
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
                CurrentInstance = new Part21Entity(EntityCreate(value, null, _inHeader));
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
            catch (Exception e)
            {
                // return silently if this kind of error has already been reported
                if (!_errors.AddPropertyNotSet(CurrentInstance.Entity, CurrentInstance.CurrentParamIndex, PropertyValue, e))
                    return;

                var mainEntity = _processStack.Last();
                if (mainEntity != null)
                {
                    Logger?.LogError(LogEventIds.FailedPropertySetter, e, "Entity #{0,-5} {1}, error at parameter {2} value = {3}",
                       mainEntity.EntityLabel,
                       mainEntity.Entity.GetType().Name.ToUpper(),
                       mainEntity.CurrentParamIndex + 1,
                       value);
                }
                else
                {
                    Logger?.LogError(LogEventIds.FailedPropertySetter, e, "Unhandled Parser error, in Parser.cs SetEntityParameter");
                }
                ErrorCount++;
            }
            if (ListNestLevel == 0)
            {
                CurrentInstance.CurrentParamIndex++;
                _deferListItems = false;
            }
        }

        public Dictionary<int, IPersist> Entities { get; private set; }

        internal bool TrySetObjectValue(IPersist host, int paramIndex, int refId, int[] listNextLevel)
        {
            if (_deferListItems) return false;
            try
            {
                if (host != null && Entities.TryGetValue(refId, out IPersist refEntity))
                {
                    PropertyValue.Init(refEntity);
                    host.Parse(paramIndex, PropertyValue, listNextLevel);
                    return true;
                }
            }
            catch (Exception e)
            {
                // return silently if this kind of error has already been reported
                if (_errors.AddPropertyNotSet(host, paramIndex, PropertyValue, e))
                {
                    Logger?.LogError(LogEventIds.FailedPropertySetter, e, "Entity #{0,-5} {1}, error at parameter {2}", refId, host.GetType().Name.ToUpper(), paramIndex + 1 );
                    ErrorCount++;
                }

            }
            return false;
        }

        public void Dispose()
        {
            if (Entities.Count > 0)
                Entities.Clear();
            if (_progressStatusEvents.Count > 0)
            {
                _progressStatusEvents.ForEach(e => _progressStatus -= e);
                _progressStatusEvents.Clear();
            }
        }
    }

    public delegate IPersist CreateEntityDelegate(string className, long? label, bool headerEntity);

}