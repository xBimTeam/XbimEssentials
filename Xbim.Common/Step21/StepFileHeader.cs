#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFileHeader.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace Xbim.Common.Step21
{
   
    public class StepFileDescription : IStepFileDescription
    {
        private void MakeValid()
        {
            //if (_description.Count == 0)
            //{
            //    _description.Add("ViewDefinition [CoordinationView]");

            //}
            if (string.IsNullOrWhiteSpace(_implementationLevel))
            {
                _implementationLevel = "2;1";
            }
            
        }

        public StepFileDescription()
        {
            Init();
        }

        public StepFileDescription(string implementationLevel)
        {
            ImplementationLevel = implementationLevel;
            //Description.Add("ViewDefinition [CoordinationView]");
            Init();
        }

        private readonly ObservableCollection<string> _description = new ObservableCollection<string>();
        private string _implementationLevel;
        private int _entityCount;

        private void Init()
        {
            _description.CollectionChanged += (sender, args) => OnPropertyChanged("Description");
        }

        #region ISupportIfcParser Members

        public void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
        {
            switch (propIndex)
            {
                case 0:
                    _description.Add(value.StringVal);
                    break;
                case 1:
                    _implementationLevel = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion


        
        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion

        internal void Write(BinaryWriter binaryWriter)
        {
            MakeValid();
            binaryWriter.Write(_description.Count);
            foreach (var desc in _description)
                binaryWriter.Write(desc);
            binaryWriter.Write(_implementationLevel);
        }

        internal void Read(BinaryReader binaryReader)
        {
            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                _description.Add(binaryReader.ReadString());
            }
            _implementationLevel = binaryReader.ReadString();
        }

        public IList<string> Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description.Clear();
                if(value == null || !value.Any()) return;
                foreach (var v in value)
                    _description.Add(v);
            }
        }

        public string ImplementationLevel
        {
            get
            {
                return _implementationLevel;
            }
            set
            {
                _implementationLevel = value;
                OnPropertyChanged("ImplementationLevel");
            }
        }

        public int EntityCount
        {
            get { return _entityCount; }
            set
            {
                _entityCount = value;
                OnPropertyChanged("EntityCount");
            }
        }


        void IStepFileDescription.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IStepFileDescription.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   
    public class StepFileName : IStepFileName
    {
        public StepFileName(DateTime time)
        {
            TimeStamp = string.Format(time.ToString("s"));
            Init();
        }

        public StepFileName()
        {
            SetTimeStampNow();
            Init();
        }

        private void Init()
        {
            _authorName.CollectionChanged += (sender, args) => OnPropertyChanged("AuthorName");
            _organization.CollectionChanged += (sender, args) => OnPropertyChanged("Organization");
            _authorizationMailingAddress.CollectionChanged +=
                (sender, args) => OnPropertyChanged("AuthorizationMailingAddress");
        }

        private string _name;
        private string _timeStamp;

        public void SetTimeStampNow()
        {
            var now = DateTime.Now;
            _timeStamp = string.Format(now.ToString("s"));
        }

        private readonly ObservableCollection<string> _authorName = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _organization = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _authorizationMailingAddress = new ObservableCollection<string>();
        private string _preprocessorVersion;
        private string _originatingSystem;
        private string _authorizationName = "";

        #region ISupportIfcParser Members

        public void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
        {
            switch (propIndex)
            {
                case 0:
                    _name = value.StringVal;
                    break;
                case 1:
                    _timeStamp = value.StringVal;
                    break;
                case 2:
                    _authorName.Add(value.StringVal);
                    break;
                case 3:
                    _organization.Add(value.StringVal);
                    break;
                case 4:
                    _preprocessorVersion = value.StringVal;
                    break;
                case 5:
                    _originatingSystem = value.StringVal;
                    break;
                case 6:
                    _authorizationName = value.StringVal;
                    break;
                case 7:
                    _authorizationMailingAddress.Add(value.StringVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            return "";
        }

        #endregion

        internal void Write(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(_name ?? "");
            binaryWriter.Write(_timeStamp ?? "");
            binaryWriter.Write(_authorName.Count);
            foreach (var item in _authorName)
                binaryWriter.Write(item);
            binaryWriter.Write(_organization.Count);
            foreach (var item in _organization)
                binaryWriter.Write(item);
            binaryWriter.Write(_preprocessorVersion ?? "");
            binaryWriter.Write(_originatingSystem ?? "");
            binaryWriter.Write(_authorizationName ?? "");
            binaryWriter.Write(_authorizationMailingAddress.Count);
            foreach (var item in _authorizationMailingAddress)
                binaryWriter.Write(item);
        }

        internal void Read(BinaryReader binaryReader)
        {
            _name = binaryReader.ReadString();
            _timeStamp = binaryReader.ReadString();
            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                _authorName.Add(binaryReader.ReadString());
            }
            count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                _organization.Add(binaryReader.ReadString());
            }
            _preprocessorVersion = binaryReader.ReadString();
            _originatingSystem = binaryReader.ReadString();
            _authorizationName = binaryReader.ReadString();
            count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                _authorizationMailingAddress.Add(binaryReader.ReadString());
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
                OnPropertyChanged("TimeStamp");
            }
        }

        public IList<string> AuthorName
        {
            get
            {
                return _authorName;
            }
            set
            {
                _authorName.Clear();
                if (value == null || !value.Any()) return;
                foreach (var v in value)
                    _authorName.Add(v);
            }
        }

        public IList<string> Organization
        {
            get
            {
                return _organization;
            }
            set
            {
                _organization.Clear();
                if (value == null || !value.Any()) return;
                foreach (var v in value)
                    _organization.Add(v);
            }
        }

        public string PreprocessorVersion
        {
            get
            {
                return _preprocessorVersion;
            }
            set
            {
                _preprocessorVersion = value;
                OnPropertyChanged("PreprocessorVersion");
            }
        }

        public string OriginatingSystem
        {
            get
            {
                return _originatingSystem;
            }
            set
            {
                _originatingSystem = value;
                OnPropertyChanged("OriginatingSystem");
            }
        }

        public string AuthorizationName
        {
            get
            {
                return _authorizationName;
            }
            set
            {
                _authorizationName = value;
                OnPropertyChanged("AuthorizationName");
            }
        }

        public IList<string> AuthorizationMailingAddress
        {
            get
            {
                return _authorizationMailingAddress;
            }
            set
            {
                _authorizationMailingAddress.Clear();
                if (value == null || !value.Any()) return;
                foreach (var v in value)
                    _authorizationMailingAddress.Add(v);
            }
        }


        void IStepFileName.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IStepFileName.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

  
    public class StepFileSchema : IStepFileSchema
    {
        private readonly ObservableCollection<string> _schemas = new ObservableCollection<string>();
        

        public StepFileSchema()
        {
            Init();
        }

        public StepFileSchema(string version)
        {
            _schemas.Add(version);
            Init();
        }

        public StepFileSchema(XbimSchemaVersion schemaVersion)
        {
            _schemas.Add(schemaVersion.ToString().ToUpper());
            Init();
        }

        private void Init()
        {
            _schemas.CollectionChanged += (sender, args) => OnPropertyChanged("Schemas");
        }

        #region ISupportIfcParser Members

        public void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
        {
            switch (propIndex)
            {
                case 0:
                    if (!_schemas.Contains(value.StringVal)) _schemas.Add(value.StringVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            return "";
        }

        #endregion

        internal void Write(BinaryWriter binaryWriter)
        {
            //if (_schemas.Count == 0) //if no schema is defined the use IFC2x3 for now
            //    _schemas.Add("IFC2X3");
            binaryWriter.Write(_schemas.Count);
            foreach (var item in _schemas)
                binaryWriter.Write(item);
        }

        internal void Read(BinaryReader binaryReader)
        {
            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                _schemas.Add(binaryReader.ReadString());
            }
        }

        IList<string> IStepFileSchema.Schemas
        {
            get
            {
                return _schemas;
            }
            set
            {
                _schemas.Clear();
                if (value == null || !value.Any()) return;
                foreach (var v in value)
                    _schemas.Add(v);
            }
        }


        void IStepFileSchema.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IStepFileSchema.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   
    public class StepFileHeader : IStepFileHeader
    {
        public enum HeaderCreationMode
        {
            LeaveEmpty,
            InitWithXbimDefaults
        }

        public StepFileHeader(HeaderCreationMode mode, IModel model)
        {
            if (mode == HeaderCreationMode.InitWithXbimDefaults)
            {
                var assembly = model.GetType().GetTypeInfo().Assembly; //get the assembly that has created the model
                FileDescription = new StepFileDescription("2;1");
                FileName = new StepFileName(DateTime.Now)
                {
                    PreprocessorVersion =$"Processor version {assembly.GetName().Version}",
                    OriginatingSystem = assembly.GetName().Name
                };
                FileSchema = new StepFileSchema();
            }
            else
            {
                // Please note do not put any value initialisation in here
                // Any value initialised here is added to ALL models read from IFC
                // 
                // Any information required before writing a file for schema constraint needs to be checked upon writing
                // e.g. cfr. _fileDescription.MakeValid();
                //
                FileDescription = new StepFileDescription();
                FileName = new StepFileName();
                FileSchema = new StepFileSchema();
            }
        }

        private IStepFileDescription _fileDescription;
        private IStepFileName _fileName;
        private IStepFileSchema _fileSchema;

        public void Write(BinaryWriter binaryWriter)
        {
            _fileDescription.Write(binaryWriter);
            _fileName.Write(binaryWriter);
            _fileSchema.Write(binaryWriter);
        }

        public void Read(BinaryReader binaryReader)
        {
            _fileDescription.Read(binaryReader);
            _fileName.Read(binaryReader);
            _fileSchema.Read(binaryReader);
        }

        public IStepFileDescription FileDescription
        {
            get
            {
                return _fileDescription;
            }
            set
            {
                _fileDescription = value;
                OnPropertyChanged("FileDescription");
                if (_fileDescription != null)
                    _fileDescription.PropertyChanged += 
                        (sender, args) => OnPropertyChanged("FileDescription");
            }
        }

        public IStepFileName FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
                if (_fileName != null)
                    _fileName.PropertyChanged += (sender, args) => OnPropertyChanged("FileName");
            }

        }

        public IStepFileSchema FileSchema
        {
            get
            {
                return _fileSchema;
            }
            set
            {
                _fileSchema = value;
                OnPropertyChanged("FileSchema");
                if (_fileSchema != null)
                    _fileSchema.PropertyChanged += (sender, args) => OnPropertyChanged("FileSchema");
            }

        }


        public string SchemaVersion
        {
            get
            {
                if (_fileSchema != null && _fileSchema.Schemas!=null && _fileSchema.Schemas.Count > 0)
                    return string.Join(", ", _fileSchema.Schemas);
                return "";
            }
        }

        public string CreatingApplication
        {
            get
            {
                if (_fileName != null && _fileName.OriginatingSystem != null)
                    return _fileName.OriginatingSystem;
                return "";
            }
        }

        public string ModelViewDefinition
        {
            get
            {
                if (_fileDescription != null && _fileDescription.Description != null)
                    return string.Join(", ", _fileDescription.Description);
                return "";
            }
        }

        public string Name
        {
            get
            {
                if (_fileName != null && _fileName.Name != null)
                    return _fileName.Name;
                return "";
            }
        }

        public string TimeStamp
        {
            get
            {
                if (_fileName != null && _fileName.TimeStamp != null)
                    return _fileName.TimeStamp;
                else
                    return "";
            }
        }

        public XbimSchemaVersion XbimSchemaVersion
        {
            get
            {                
                foreach (var schema in FileSchema.Schemas)
                {
                    if (string.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                        return XbimSchemaVersion.Ifc4;                    
                    if (schema.StartsWith("Ifc2x", StringComparison.OrdinalIgnoreCase)) //return this as 2x3
                        return XbimSchemaVersion.Ifc2X3;
                    if (schema.StartsWith("Cobie2X4", StringComparison.OrdinalIgnoreCase)) //return this as Cobie
                        return XbimSchemaVersion.Cobie2X4;
                }
                return XbimSchemaVersion.Unsupported;
            }
        }

        public void StampXbimApplication(XbimSchemaVersion schemaVersion, IModel model)
        {
            var assembly = model.GetType().GetTypeInfo().Assembly; //get the assembly that has created th emodel
            FileDescription = new StepFileDescription("2;1");
            FileName = new StepFileName(DateTime.Now)
            {
                PreprocessorVersion =
                           string.Format("Processor version {0}",
                                         assembly.GetName().Version),
                OriginatingSystem = assembly.GetName().Name

            };
            FileSchema = new StepFileSchema(schemaVersion);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}