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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.IO.Parser;
using Xbim.IO.Step21;
using Xbim.IO.Step21.Parser;

#endregion

namespace Xbim.IO.Esent
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

    public class P21ToIndexParser : P21Parser, IDisposable
    {
        public event ReportProgressDelegate ProgressStatus;
        private int _percentageParsed;
        private readonly long _streamSize = -1;
        private BlockingCollection<Tuple<int, Type, byte[]>> _toProcess;
        private BlockingCollection<Tuple<int, short, List<int>, byte[], bool>> _toStore;
        Task _cacheProcessor;
        Task _storeProcessor;
        private BinaryWriter _binaryWriter;

        private int _currentLabel;
        private string _currentType;
        private IList<int> _indexKeys = null;
        private readonly List<int> _indexKeyValues = new List<int>();
        private Part21Entity _currentInstance;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private PropertyValue _propertyValue;
        private int _listNestLevel = -1;
        private readonly StepFileHeader _header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, new EsentModel());

        public StepFileHeader Header
        {
            get { return _header; }
        }



        private readonly EsentEntityCursor _table;

        private readonly PersistedEntityInstanceCache _modelCache;
        const int TransactionBatchSize = 100;
        private int _entityCount = 0;
        private readonly int _codePageOverride = -1;

        public int EntityCount
        {
            get { return _entityCount; }
        }



        internal P21ToIndexParser(Stream inputP21, long streamSize,  EsentEntityCursor table, PersistedEntityInstanceCache cache, int codePageOverride = -1)
            : base(inputP21)
        {

            this._table = table;
            //  this.transaction = transaction;
            _modelCache = cache;
            _entityCount = 0;
            _streamSize = streamSize;
            _codePageOverride = codePageOverride;
        }

        protected override void SetErrorMessage()
        {
            Debug.WriteLine("TODO");
        }

        protected override void CharacterError()
        {
            Debug.WriteLine("TODO");
        }

        protected override void BeginParse()
        {
            _binaryWriter = new BinaryWriter(new MemoryStream(0x7FFF));
            _toStore = new BlockingCollection<Tuple<int, short, List<int>, byte[], bool>>(512);
            if (_modelCache.IsCaching)
            {
                _toProcess = new BlockingCollection<Tuple<int, Type, byte[]>>();
                _cacheProcessor = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // Consume the BlockingCollection 
                        while (!_toProcess.IsCompleted)
                        {
                            Tuple<int, Type, byte[]> h;
                            if (_toProcess.TryTake(out h))
                                _modelCache.GetOrCreateInstanceFromCache(h.Item1, h.Item2, h.Item3);
                        }
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
                    );

            }
            _storeProcessor = Task.Factory.StartNew(() =>
            {

                using (var transaction = _table.BeginLazyTransaction())
                {
                    while (!_toStore.IsCompleted)
                    {
                        try
                        {
                            Tuple<int, short, List<int>, byte[], bool> h;
                            if (_toStore.TryTake(out h))
                            {
                                _table.AddEntity(h.Item1, h.Item2, h.Item3, h.Item4, h.Item5, transaction);
                                if (_toStore.IsCompleted)
                                    _table.WriteHeader(Header);
                                long remainder = _entityCount % TransactionBatchSize;
                                if (remainder == TransactionBatchSize - 1)
                                {
                                    transaction.Commit();
                                    transaction.Begin();
                                }
                            }
                        }
                        catch (SystemException)
                        {

                            // An InvalidOperationException means that Take() was called on a completed collection
                            //OperationCanceledException can also be called

                        }
                    }
                    transaction.Commit();
                }
            }
            );
        }

        protected override void EndParse()
        {
            _toStore.CompleteAdding();
            _storeProcessor.Wait();
            if (_modelCache.IsCaching)
            {
                _toProcess.CompleteAdding();
                _cacheProcessor.Wait();
                _cacheProcessor.Dispose();
                _cacheProcessor = null;
                while (_modelCache.ForwardReferences.Count > 0)
                {
                    if (_modelCache.ForwardReferences.TryTake(out StepForwardReference forwardRef))
                        forwardRef.Resolve(_modelCache.Read, _modelCache.Model.Metadata);
                }
            }
            _storeProcessor.Dispose();
            _storeProcessor = null;
            Dispose();
        }

        protected override void BeginHeader()
        {
            // Debug.WriteLine("TODO");
        }

        protected override void EndHeader()
        {
            // _header.Write(_binaryWriter);
        }

        protected override void BeginScope()
        {
            // Debug.WriteLine("TODO");
        }

        protected override void EndScope()
        {
            // Debug.WriteLine("TODO");
        }

        protected override void EndSec()
        {
            // Debug.WriteLine("TODO");
        }

        private readonly List<int> _nestedIndex = new List<int>();
        public int[] NestedIndex { get { return _listNestLevel > 0 ? _nestedIndex.ToArray() : null; } }

        protected override void BeginList()
        {
            var p21 = _processStack.Peek();
            if (p21.CurrentParamIndex == -1)
                p21.CurrentParamIndex++; //first time in take the first argument
            _listNestLevel++;
            if (!InHeader)
                _binaryWriter.Write((byte)P21ParseAction.BeginList);

            if (_listNestLevel < 2) return;

            if (_listNestLevel - 1 > _nestedIndex.Count)
                _nestedIndex.Add(0);
            else
                _nestedIndex[_listNestLevel - 2]++;

        }

        protected override void EndList()
        {
            _listNestLevel--;
            if (_listNestLevel == 0)
                _currentInstance.CurrentParamIndex++;
            
            if (!InHeader)
                _binaryWriter.Write((byte)P21ParseAction.EndList);

            //we are finished with the list
            if (_listNestLevel <= 0) _nestedIndex.Clear();
        }

        protected override void BeginComplex()
        {
            _binaryWriter.Write((byte)P21ParseAction.BeginComplex);
        }

        protected override void EndComplex()
        {
            _binaryWriter.Write((byte)P21ParseAction.EndComplex);
        }

        protected override void NewEntity(string entityLabel)
        {
            _currentInstance = new Part21Entity(entityLabel);
            _processStack.Push(_currentInstance);
            _entityCount++;
            _indexKeyValues.Clear();
            _currentLabel = Convert.ToInt32(entityLabel.TrimStart('#'));
            var data = _binaryWriter.BaseStream as MemoryStream;
            if (data != null) data.SetLength(0);


            if (_streamSize == -1 || ProgressStatus == null) 
                return;

            var sc = (Scanner)Scanner;
            double pos = sc.Buffer.Pos;
            var newPercentage = Convert.ToInt32(pos / _streamSize * 100.0);
            if (newPercentage <= _percentageParsed) 
                return;

            _percentageParsed = newPercentage;
            ProgressStatus(_percentageParsed, "Parsing");
        }

        protected override void SetType(string entityTypeName)
        {
            if (InHeader)
            {
                IPersist currentHeaderEntity;
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
                var type = _modelCache.Model.Metadata.ExpressType(_currentType);
                if (type == null)
                    throw new ArgumentException(string.Format("Invalid entity type {0}", _currentType));
                _indexKeys = type.IndexedValues;
            }
        }

        protected override void EndEntity()
        {
            var p21 = _processStack.Pop();
            Debug.Assert(_processStack.Count == 0);
            _currentInstance = null;
            if (_currentType != null)
            {
                _binaryWriter.Write((byte)P21ParseAction.EndEntity);
                var type = _modelCache.Model.Metadata.ExpressType(_currentType);
                var data = _binaryWriter.BaseStream as MemoryStream;
                var bytes = data.ToArray();
                var keys = new List<int>(_indexKeyValues);
                _toStore.Add(new Tuple<int, short, List<int>, byte[], bool>(_currentLabel, type.TypeId, keys, bytes, type.IndexedClass));
                if (this._modelCache.IsCaching) _toProcess.Add(new Tuple<int, Type, byte[]>(_currentLabel, type.Type, bytes));
            }

        }

        protected override void EndHeaderEntity()
        {
            _processStack.Pop();
            _currentInstance = null;
        }

        protected override void SetIntegerValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.Integer);
                if (_currentInstance.Entity != null)
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetIntegerValue);
                _binaryWriter.Write(Convert.ToInt64(value));
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        protected override void SetHexValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.HexaDecimal);
                if (_currentInstance.Entity != null)
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetHexValue);
                var data = value.Substring(1, value.Length - 2);
                if (string.IsNullOrWhiteSpace(data))
                {
                    _binaryWriter.Write((Int32)0);
                }
                else
                {
                    //decode data into byte array and write it
                    var hex = data.Substring(1);
                    int numChars = hex.Length;
                    byte[] bytes = new byte[numChars / 2];
                    for (int i = 0; i < numChars; i += 2)
                        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

                   _binaryWriter.Write(bytes.Length);
                   _binaryWriter.Write(bytes);
                }
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        protected override void SetFloatValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.Real);
                if (_currentInstance.Entity != null)
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetFloatValue);
                _binaryWriter.Write(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        protected override void SetStringValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.String);
                if (_currentInstance.Entity != null)
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetStringValue);
                var ret = value.Substring(1, value.Length - 2); //remove the quotes
                if (ret.Contains("\\") || ret.Contains("'")) //"''" added to remove extra ' added in IfcText Escape() method
                {
                    var d = new XbimP21StringDecoder();
                    ret = d.Unescape(ret, _codePageOverride);
                }
                _binaryWriter.Write(ret);
            }
            if (_listNestLevel == 0)
                _currentInstance.CurrentParamIndex++;
        }

        protected override void SetEnumValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.Enum);
                if (_currentInstance.Entity != null)
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetEnumValue);
                _binaryWriter.Write(value.Trim('.'));
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        protected override void SetBooleanValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.Boolean);
                if (_currentInstance.Entity != null)
                    _currentInstance.Entity.Parse(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
            }
            else
            {
                _binaryWriter.Write((byte)P21ParseAction.SetBooleanValue);
                _binaryWriter.Write(value == ".T.");
            }
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        protected override void SetNonDefinedValue()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
            _binaryWriter.Write((byte)P21ParseAction.SetNonDefinedValue);
        }

        protected override void SetOverrideValue()
        {
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
            _binaryWriter.Write((byte)P21ParseAction.SetOverrideValue);
        }

        protected override void SetObjectValue(string value)
        {
            var val = Convert.ToInt32(value.TrimStart('#'));

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

        protected override void EndNestedType(string value)
        {
            _binaryWriter.Write((byte)P21ParseAction.EndNestedType);
            if (_listNestLevel == 0) _currentInstance.CurrentParamIndex++;
        }

        protected override void BeginNestedType(string value)
        {
            _binaryWriter.Write((byte)P21ParseAction.BeginNestedType);
            _binaryWriter.Write(value);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_binaryWriter != null) _binaryWriter.Close();
            _binaryWriter = null;
        }

        #endregion
    }
}