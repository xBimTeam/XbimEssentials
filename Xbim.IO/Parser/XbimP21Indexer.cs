#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    P21toIndexParser.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Microsoft.Isam.Esent.Interop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xbim.Common.Logging;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
#endregion

namespace Xbim.IO.Parser
{


    public enum P21ParseAction
    {
        BeginList, //0
        EndList, //1
        BeginComplex, //2
        EndComplex, //3
        SetIntegerValue, //4
        SetHexValue, //5
        SetFloatValue, //6
        SetStringValue, //7
        SetEnumValue, //8
        SetBooleanValue, //9
        SetNonDefinedValue, //0x0A
        SetOverrideValue, //x0B
        BeginNestedType, //0x0C
        EndNestedType, //0x0D
        EndEntity, //0x0E
        NewEntity, //0x0F
        SetObjectValueUInt16,
        SetObjectValueInt32,
        SetObjectValueInt64
    }

    public class P21toIndexParser : P21Parser, IDisposable
    {

        private readonly ILogger Logger = LoggerFactory.GetLogger();

        public event ReportProgressDelegate ProgressStatus;
        private int _percentageParsed;
        private long _streamSize = -1;
        private BlockingCollection<Tuple<int,Type,byte[]>> toProcess;
        private BlockingCollection<Tuple<int, short, List<int>, byte[], bool>> toStore;
        Task cacheProcessor;
        Task storeProcessor;

        private CancellationTokenSource _cancellationTokenSource;
        private BinaryWriter _binaryWriter;
        
        private int _currentLabel;
        private string _currentType;
        private List<int> _indexKeys = null;
        private List<int> _indexKeyValues = new List<int>();
        private Part21Entity _currentInstance;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private PropertyValue _propertyValue;
        private int _listNestLevel = -1;
        private readonly IfcFileHeader _header = new IfcFileHeader(IfcFileHeader.HeaderCreationMode.LeaveEmpty);

        public IfcFileHeader Header
        {
            get { return _header; }
        } 

   
       
        private XbimEntityCursor table;
       
        private IfcPersistedInstanceCache modelCache;
        const int _transactionBatchSize = 100;
        private int _entityCount = 0;
        private int _codePageOverride = -1;

        public int EntityCount
        {
            get { return _entityCount; }
        }

        
        
        internal P21toIndexParser(Stream inputP21,  XbimEntityCursor table,  IfcPersistedInstanceCache cache, int codePageOverride = -1)
            : base(inputP21)
        {
           
            this.table = table;
          //  this.transaction = transaction;
            this.modelCache = cache;
            _entityCount = 0;
            if (inputP21.CanSeek)
                _streamSize = inputP21.Length;
            _codePageOverride = codePageOverride;
        }

        internal override void SetErrorMessage()
        {
            Debug.WriteLine("Parse error at : #{0} - Error at {1}", this._currentInstance.EntityLabel, this.CurrentSemanticValue.strVal);
        }

        internal override void CharacterError()
        {
            Debug.WriteLine("Character error at : #{0} - Error at {1}", this._currentInstance.EntityLabel, this.CurrentSemanticValue.strVal);
        }

        internal override void BeginParse()
        {
            Logger.Debug("Parsing Beginning");
            _binaryWriter = new BinaryWriter(new MemoryStream(0x7FFF));
            _cancellationTokenSource = new CancellationTokenSource();
            toStore = new BlockingCollection<Tuple<int, short, List<int>, byte[], bool>>(512);
            if (this.modelCache.IsCaching)
            {
                toProcess = new BlockingCollection<Tuple<int, Type, byte[]>>();
                cacheProcessor = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Tuple<int, Type, byte[]> h;
                            // Consume the BlockingCollection 
                            while (!toProcess.IsCompleted && !_cancellationTokenSource.IsCancellationRequested)
                            {

                                if (toProcess.TryTake(out h))
                                    this.modelCache.GetOrCreateInstanceFromCache(h.Item1, h.Item2, h.Item3);
                            }

                        }
                        catch (InvalidOperationException ex)
                        {
                            Logger.Debug("Error creating instances from cache", ex);
                        }
                    },
                    _cancellationTokenSource.Token
                    );

            }
            storeProcessor = Task.Factory.StartNew(() =>
            {

                using (var transaction = table.BeginLazyTransaction())
                {
                    Tuple<int, short, List<int>, byte[], bool> h;
                    while (!toStore.IsCompleted && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            if (toStore.TryTake(out h))
                            {
                                table.AddEntity(h.Item1, h.Item2, h.Item3, h.Item4, h.Item5, transaction);
                                if (toStore.IsCompleted)
                                    table.WriteHeader(Header);
                                long remainder = _entityCount % _transactionBatchSize;
                                if (remainder == _transactionBatchSize - 1)
                                {
                                    transaction.Commit();
                                    transaction.Begin();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn("Error in parsing", ex);

                            // An InvalidOperationException means that Take() was called on a completed collection
                            //OperationCanceledException can also be called

                        }
                    } // End while

                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        transaction.Commit();
                    }
                } // End Transaction

            }, _cancellationTokenSource.Token);
        }

        internal override void EndParse()
        {
            Logger.Debug("Parsing Ended");
            toStore.CompleteAdding();
            storeProcessor.Wait();
            if (this.modelCache.IsCaching)
            {
                toProcess.CompleteAdding();
                cacheProcessor.Wait();
                cacheProcessor.Dispose();
                cacheProcessor = null;
                while (this.modelCache.ForwardReferences.Count > 0)
                {
                    IfcForwardReference forwardRef;
                    if(this.modelCache.ForwardReferences.TryTake(out forwardRef))
                        forwardRef.Resolve(this.modelCache.Read);
                }
            }
            storeProcessor.Dispose();
            storeProcessor = null;
            Dispose();
            
        }

        internal override void BeginHeader()
        {
            // Debug.WriteLine("TODO");
        }

        internal override void EndHeader()
        {
           // _header.Write(_binaryWriter);
        }

        internal override void BeginScope()
        {
            // Debug.WriteLine("TODO");
        }

        internal override void EndScope()
        {
            // Debug.WriteLine("TODO");
        }

        internal override void EndSec()
        {
            // Debug.WriteLine("TODO");
        }

        internal override void BeginList()
        {
            Part21Entity p21 = _processStack.Peek();
            if (p21.CurrentParamIndex == -1)
                p21.CurrentParamIndex++; //first time in take the first argument
            _listNestLevel++;
            if (!InHeader)
                _binaryWriter.Write((byte)P21ParseAction.BeginList);

        }

        internal override void EndList()
        {
            _listNestLevel--;
            Part21Entity p21 = _processStack.Peek();
            p21.CurrentParamIndex++;
            if (!InHeader)
                _binaryWriter.Write((byte)P21ParseAction.EndList);
        }

        internal override void BeginComplex()
        {
            _binaryWriter.Write((byte)P21ParseAction.BeginComplex);
        }

        internal override void EndComplex()
        {
            _binaryWriter.Write((byte)P21ParseAction.EndComplex);
        }

        internal override void NewEntity(string entityLabel)
        {
            _currentInstance = new Part21Entity(entityLabel);
            _processStack.Push(_currentInstance);
            _entityCount++;
            _indexKeyValues.Clear();
            _currentLabel = Convert.ToInt32(entityLabel.TrimStart('#'));
            MemoryStream data = _binaryWriter.BaseStream as MemoryStream;
            data.SetLength(0);

          
            if (_streamSize != -1 && ProgressStatus != null)
            {
                Scanner sc = (Scanner)this.Scanner;
                double pos = sc.Buffer.Pos;
                int newPercentage = Convert.ToInt32(pos / _streamSize * 100.0);
                if (newPercentage > _percentageParsed)
                {
                    _percentageParsed = newPercentage;
                    ProgressStatus(_percentageParsed, "Parsing");
                }
            }
        }

        internal override void SetType(string entityTypeName)
        {
            if (InHeader)
            {
                IPersistIfc currentHeaderEntity;
                switch (entityTypeName)
                {
                    case "FILE_DESCRIPTION":
                        currentHeaderEntity = _header.FileDescription;
                        break;
                    case "FILE_NAME":
                        currentHeaderEntity = _header.FileName;
                        break;
                    case "FILE_SCHEMA":
                        currentHeaderEntity = _header.FileSchema;
                        break;
                    default:
                        throw new ArgumentException(string.Format("Invalid Header entity type {0}", entityTypeName));
                }
                _currentInstance = new Part21Entity(currentHeaderEntity);
                _processStack.Push(_currentInstance);
            }
            else
            {

                _currentType = entityTypeName;
                IfcType ifcType = IfcMetaData.IfcType(_currentType);
                if(ifcType==null)
                    throw new ArgumentException(string.Format("Invalid entity type {0}", _currentType));

                _indexKeys = ifcType.IndexedValues;
            }
        }

        internal override void EndEntity()
        {
            Part21Entity p21 = _processStack.Pop();
            Debug.Assert(_processStack.Count == 0);
            _currentInstance = null;
            if (_currentType != null)
            {
                _binaryWriter.Write((byte)P21ParseAction.EndEntity);
                IfcType ifcType = IfcMetaData.IfcType(_currentType);
                MemoryStream data = _binaryWriter.BaseStream as MemoryStream;
                byte[] bytes =  data.ToArray();
                List<int> keys = new List<int>(_indexKeyValues);
                toStore.Add(new Tuple<int, short, List<int>, byte[], bool>(_currentLabel, ifcType.TypeId, keys, bytes, ifcType.IndexedClass));
                if (this.modelCache.IsCaching) toProcess.Add(new Tuple<int, Type, byte[]>(_currentLabel, ifcType.Type, bytes)); 
            }

        }

        internal override void EndHeaderEntity()
        {
            _processStack.Pop();
            _currentInstance = null;
        }

        internal override void SetIntegerValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, IfcParserType.Integer);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
               
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetIntegerValue);
                _binaryWriter.Write(Convert.ToInt64(value));
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal override void SetHexValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, IfcParserType.HexaDecimal);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
                
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetHexValue);
                _binaryWriter.Write(Convert.ToInt64(value, 16));
                
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal override void SetFloatValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, IfcParserType.Real);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
               
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetFloatValue);
                _binaryWriter.Write(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal override void SetStringValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, IfcParserType.String);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
                
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetStringValue);
                string ret = value.Substring(1, value.Length - 2); //remove the quotes
                if (ret.Contains("\\") || ret.Contains("'")) //"''" added to remove extra ' added in IfcText Escape() method
                {
                    XbimP21StringDecoder d = new XbimP21StringDecoder();
                    ret = d.Unescape(ret, _codePageOverride);
                }
                _binaryWriter.Write(ret);
            }
            if (_listNestLevel == 0)
                _currentInstance.CurrentParamIndex++;
        }

        internal override void SetEnumValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, IfcParserType.Enum);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
                
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetEnumValue);
                _binaryWriter.Write(value.Trim('.'));
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal override void SetBooleanValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, IfcParserType.Boolean);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue);
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetBooleanValue);
                _binaryWriter.Write(value == ".T.");
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal override void SetNonDefinedValue()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
            _binaryWriter.Write((byte)P21ParseAction.SetNonDefinedValue);
        }

        internal override void SetOverrideValue()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
            _binaryWriter.Write((byte)P21ParseAction.SetOverrideValue);
        }

        internal override void SetObjectValue(string value)
        {
            int val = Convert.ToInt32(value.TrimStart('#'));

            if (_indexKeys != null && _indexKeys.Contains(_currentInstance.CurrentParamIndex + 1)) //current param index is 0 based and ifcKey is 1 based
                _indexKeyValues.Add(val);

            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
           
            if (val <= Int16.MaxValue)
            {
                _binaryWriter.Write((byte)P21ParseAction.SetObjectValueUInt16);
                _binaryWriter.Write(Convert.ToUInt16(val));
            }
            else if (val <= Int32.MaxValue)
            {
                _binaryWriter.Write((byte)P21ParseAction.SetObjectValueInt32);
                _binaryWriter.Write(Convert.ToInt32(val));
            }
            //else if (val <= Int64.MaxValue)
            //{
            //    throw new Exception("Entity Label exceeds maximim value for a long number, it is greater than an int32");
            //    //_binaryWriter.Write((byte)P21ParseAction.SetObjectValueInt64);
            //    //_binaryWriter.Write(val);
            //}
            else
                throw new Exception("Entity Label exceeds maximim value for a long number, it is greater than an int32");


        }

        internal override void EndNestedType(string value)
        {
            _binaryWriter.Write((byte)P21ParseAction.EndNestedType);
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        internal override void BeginNestedType(string value)
        {
            _binaryWriter.Write((byte)P21ParseAction.BeginNestedType);
            _binaryWriter.Write(value);
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Tidy up in exceptional cases where EndParse is never called
#if !foo
            if(storeProcessor!=null && !storeProcessor.IsCompleted)
            {
                Logger.DebugFormat("Disposing of Esent resources after failure");
                _cancellationTokenSource.Cancel(true);
                try
                {
                    storeProcessor.Wait(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException) { }

                Logger.DebugFormat("storeProcessor state: {0}", storeProcessor.Status);
                storeProcessor.Dispose();
                storeProcessor = null;
                if (this.modelCache.IsCaching)
                {
                    try
                    {
                        cacheProcessor.Wait(_cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException) { }
                    cacheProcessor.Dispose();
                    cacheProcessor = null;
                }
                _cancellationTokenSource.Dispose();
            }
#endif

            if (_binaryWriter != null) _binaryWriter.Close();
            _binaryWriter = null;
        }

#endregion
    }
}