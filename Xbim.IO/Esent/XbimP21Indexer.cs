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

    public class P21toIndexParser : P21Parser, IDisposable
    {
        public event ReportProgressDelegate ProgressStatus;
        private int _percentageParsed;
        private long _streamSize = -1;
        private BlockingCollection<Tuple<int, Type, byte[]>> toProcess;
        private BlockingCollection<Tuple<int, short, List<int>, byte[], bool>> toStore;
        Task cacheProcessor;
        Task storeProcessor;
        private BinaryWriter _binaryWriter;

        private int _currentLabel;
        private string _currentType;
        private IList<int> _indexKeys = null;
        private List<int> _indexKeyValues = new List<int>();
        private Part21Entity _currentInstance;
        private readonly Stack<Part21Entity> _processStack = new Stack<Part21Entity>();
        private PropertyValue _propertyValue;
        private int _listNestLevel = -1;
        private readonly StepFileHeader _header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty);

        public StepFileHeader Header
        {
            get { return _header; }
        }



        private XbimEntityCursor table;

        private readonly PersistedEntityInstanceCache _modelCache;
        const int TransactionBatchSize = 100;
        private int _entityCount = 0;
        private readonly int _codePageOverride = -1;

        public int EntityCount
        {
            get { return _entityCount; }
        }



        internal P21toIndexParser(Stream inputP21, XbimEntityCursor table, PersistedEntityInstanceCache cache, int codePageOverride = -1)
            : base(inputP21)
        {

            this.table = table;
            //  this.transaction = transaction;
            _modelCache = cache;
            _entityCount = 0;
            if (inputP21.CanSeek)
                _streamSize = inputP21.Length;
            _codePageOverride = codePageOverride;
        }

        internal override void SetErrorMessage()
        {
            Debug.WriteLine("TODO");
        }

        internal override void CharacterError()
        {
            Debug.WriteLine("TODO");
        }

        internal override void BeginParse()
        {
            _binaryWriter = new BinaryWriter(new MemoryStream(0x7FFF));
            toStore = new BlockingCollection<Tuple<int, short, List<int>, byte[], bool>>(512);
            if (_modelCache.IsCaching)
            {
                toProcess = new BlockingCollection<Tuple<int, Type, byte[]>>();
                cacheProcessor = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Tuple<int, Type, byte[]> h;
                        // Consume the BlockingCollection 
                        while (!toProcess.IsCompleted)
                        {

                            if (toProcess.TryTake(out h))
                                _modelCache.GetOrCreateInstanceFromCache(h.Item1, h.Item2, h.Item3);
                        }

                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
                    );

            }
            storeProcessor = Task.Factory.StartNew(() =>
            {

                using (var transaction = table.BeginLazyTransaction())
                {
                    Tuple<int, short, List<int>, byte[], bool> h;
                    while (!toStore.IsCompleted)
                    {
                        try
                        {
                            if (toStore.TryTake(out h))
                            {
                                table.AddEntity(h.Item1, h.Item2, h.Item3, h.Item4, h.Item5, transaction);
                                if (toStore.IsCompleted)
                                    table.WriteHeader(Header);
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

        internal override void EndParse()
        {
            toStore.CompleteAdding();
            storeProcessor.Wait();
            if (_modelCache.IsCaching)
            {
                toProcess.CompleteAdding();
                cacheProcessor.Wait();
                cacheProcessor.Dispose();
                cacheProcessor = null;
                while (_modelCache.ForwardReferences.Count > 0)
                {
                    StepForwardReference forwardRef;
                    if (_modelCache.ForwardReferences.TryTake(out forwardRef))
                        forwardRef.Resolve(_modelCache.Read, _modelCache.Model.Metadata);
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

        private readonly List<int> _nestedIndex = new List<int>();
        public int[] NestedIndex { get { return _listNestLevel > 0 ? _nestedIndex.ToArray() : null; } }

        internal override void BeginList()
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

        internal override void EndList()
        {
            _listNestLevel--;
            if (_listNestLevel == 0)
                _currentInstance.CurrentParamIndex++;
            
            if (!InHeader)
                _binaryWriter.Write((byte)P21ParseAction.EndList);

            //we are finished with the list
            if (_listNestLevel <= 0) _nestedIndex.Clear();
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
            var data = _binaryWriter.BaseStream as MemoryStream;
            data.SetLength(0);


            if (_streamSize != -1 && ProgressStatus != null)
            {
                var sc = (Scanner)Scanner;
                double pos = sc.Buffer.Pos;
                var newPercentage = Convert.ToInt32(pos / _streamSize * 100.0);
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

        internal override void EndEntity()
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
                toStore.Add(new Tuple<int, short, List<int>, byte[], bool>(_currentLabel, type.TypeId, keys, bytes, type.IndexedClass));
                if (this._modelCache.IsCaching) toProcess.Add(new Tuple<int, Type, byte[]>(_currentLabel, type.Type, bytes));
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
                _propertyValue.Init(value, StepParserType.Integer);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

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
                _propertyValue.Init(value, StepParserType.HexaDecimal);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

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
                _propertyValue.Init(value, StepParserType.Real);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

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
                _propertyValue.Init(value, StepParserType.String);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

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

        internal override void SetEnumValue(string value)
        {
            if (InHeader)
            {
                _propertyValue.Init(value, StepParserType.Enum);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);

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
                _propertyValue.Init(value, StepParserType.Boolean);
                if (_currentInstance.Entity != null)
                    _currentInstance.ParameterSetter(_currentInstance.CurrentParamIndex, _propertyValue, NestedIndex);
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
            if (_binaryWriter != null) _binaryWriter.Close();
            _binaryWriter = null;
        }

        #endregion
    }
}