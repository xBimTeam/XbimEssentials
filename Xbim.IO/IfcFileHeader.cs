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
using System.IO;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using System.Reflection;

#endregion

namespace Xbim.IO
{
    [Serializable]
    public class FileDescription : IIfcFileDescription
    {
        private void MakeValid()
        {
            if (Description.Count == 0)
            {
                Description.Add("ViewDefinition [CoordinationView]");
                
            }
            if (string.IsNullOrWhiteSpace(ImplementationLevel))
            {
                ImplementationLevel = "2;1";
            }
            
        }

        public FileDescription()
        {
        }

        public FileDescription(string implementationLevel)
        {
            ImplementationLevel = implementationLevel;
            Description.Add("ViewDefinition [CoordinationView]");
        }

        public List<string> Description = new List<string>(2);
        private string ImplementationLevel;
        public int EntityCount;

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    Description.Add(value.StringVal);
                    break;
                case 1:
                    ImplementationLevel = value.StringVal;
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
            this.MakeValid();
            binaryWriter.Write(Description.Count);
            foreach (string desc in Description)
                binaryWriter.Write(desc);
            binaryWriter.Write(ImplementationLevel);
        }

        internal void Read(BinaryReader binaryReader)
        {
            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Description.Add(binaryReader.ReadString());
            }
            ImplementationLevel = binaryReader.ReadString();
        }

        List<string> IIfcFileDescription.Description
        {
            get
            {
                return Description;
            }
            set
            {
                Description = value;
            }
        }

        string IIfcFileDescription.ImplementationLevel
        {
            get
            {
                return ImplementationLevel;
            }
            set
            {
                ImplementationLevel = value;
            }
        }

        int IIfcFileDescription.EntityCount
        {
            get
            {
                return EntityCount;
            }
            set
            {
                EntityCount = value;
            }
        }


        void IIfcFileDescription.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IIfcFileDescription.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }
    }

    [Serializable]
    public class FileName : IIfcFileName
    {
        public FileName(DateTime time)
        {
            TimeStamp = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day,
                                      time.Hour, time.Minute, time.Second);
        }

        public FileName()
        {
            SetTimeStampNow();
        }

        public string Name;
        public string TimeStamp;

        public void SetTimeStampNow()
        {
            DateTime now = DateTime.Now;
            TimeStamp = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", now.Year, now.Month, now.Day,
                                      now.Hour, now.Minute, now.Second);
        }

        public List<string> AuthorName = new List<string>(2);
        public List<string> Organization = new List<string>(6);

        public string PreprocessorVersion;
        public string OriginatingSystem;
        public string AuthorizationName = "";
        public List<string> AuthorizationMailingAddress = new List<string>(6);

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    Name = value.StringVal;
                    break;
                case 1:
                    TimeStamp = value.StringVal;
                    break;
                case 2:
                    AuthorName.Add(value.StringVal);
                    break;
                case 3:
                    Organization.Add(value.StringVal);
                    break;
                case 4:
                    PreprocessorVersion = value.StringVal;
                    break;
                case 5:
                    OriginatingSystem = value.StringVal;
                    break;
                case 6:
                    AuthorizationName = value.StringVal;
                    break;
                case 7:
                    AuthorizationMailingAddress.Add(value.StringVal);
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
            binaryWriter.Write(Name ?? "");
            binaryWriter.Write(TimeStamp ?? "");
            binaryWriter.Write(AuthorName.Count);
            foreach (string item in AuthorName)
                binaryWriter.Write(item);
            binaryWriter.Write(Organization.Count);
            foreach (string item in Organization)
                binaryWriter.Write(item);
            binaryWriter.Write(PreprocessorVersion ?? "");
            binaryWriter.Write(OriginatingSystem ?? "");
            binaryWriter.Write(AuthorizationName ?? "");
            binaryWriter.Write(AuthorizationMailingAddress.Count);
            foreach (string item in AuthorizationMailingAddress)
                binaryWriter.Write(item);
        }

        internal void Read(BinaryReader binaryReader)
        {
            Name = binaryReader.ReadString();
            TimeStamp = binaryReader.ReadString();
            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                AuthorName.Add(binaryReader.ReadString());
            }
            count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Organization.Add(binaryReader.ReadString());
            }
            PreprocessorVersion = binaryReader.ReadString();
            OriginatingSystem = binaryReader.ReadString();
            AuthorizationName = binaryReader.ReadString();
            count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                AuthorizationMailingAddress.Add(binaryReader.ReadString());
            }
        }

        string IIfcFileName.Name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        string IIfcFileName.TimeStamp
        {
            get
            {
                return TimeStamp;
            }
            set
            {
                TimeStamp = value;
            }
        }

        List<string> IIfcFileName.AuthorName
        {
            get
            {
                return AuthorName;
            }
            set
            {
                AuthorName = value;
            }
        }

        List<string> IIfcFileName.Organization
        {
            get
            {
                return Organization;
            }
            set
            {
                Organization = value;
            }
        }

        string IIfcFileName.PreprocessorVersion
        {
            get
            {
                return PreprocessorVersion;
            }
            set
            {
                PreprocessorVersion = value;
            }
        }

        string IIfcFileName.OriginatingSystem
        {
            get
            {
                return OriginatingSystem;
            }
            set
            {
                OriginatingSystem = value;
            }
        }

        string IIfcFileName.AuthorizationName
        {
            get
            {
                return AuthorizationName;
            }
            set
            {
                AuthorizationName = value;
            }
        }

        List<string> IIfcFileName.AuthorizationMailingAddress
        {
            get
            {
                return AuthorizationMailingAddress;
            }
            set
            {
                AuthorizationMailingAddress = value;
            }
        }


        void IIfcFileName.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IIfcFileName.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }
    }

    [Serializable]
    public class FileSchema : IIfcFileSchema
    {
        public List<string> Schemas = new List<string>();

        public FileSchema()
        {
        }

        public FileSchema(string version)
        {
            Schemas.Add(version);
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    Schemas.Add(value.StringVal);
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
            if (Schemas.Count == 0) //if no schema is defined the use IFC2x3 for now
                Schemas.Add("IFC2X3");
            binaryWriter.Write(Schemas.Count);
            foreach (string item in Schemas)
                binaryWriter.Write(item);
        }

        internal void Read(BinaryReader binaryReader)
        {
            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Schemas.Add(binaryReader.ReadString());
            }
        }

        List<string> IIfcFileSchema.Schemas
        {
            get
            {
                return Schemas;
            }
            set
            {
                Schemas = value;
            }
        }


        void IIfcFileSchema.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IIfcFileSchema.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }
    }

    [Serializable]
    public class IfcFileHeader : IIfcFileHeader
    {
        public enum HeaderCreationMode
        {
            LeaveEmpty,
            InitWithXbimDefaults
        }

        public IfcFileHeader(HeaderCreationMode Mode)
        {
            if (Mode == HeaderCreationMode.InitWithXbimDefaults)
            {
                FileDescription = new FileDescription("2;1");
                FileName = new FileName(DateTime.Now)
                    {
                        PreprocessorVersion =
                            string.Format("Xbim.Ifc File Processor version {0}",
                                          Assembly.GetExecutingAssembly().GetName().Version),
                        OriginatingSystem =
                            string.Format("Xbim version {0}",
                                          Assembly.GetExecutingAssembly().GetName().Version),
                    };
                FileSchema = new FileSchema("IFC2X3");
            }
            else
            {
                // Please note do not put any value initialisation in here
                // Any value initialised here is added to ALL models read from IFC
                // 
                // Any information required before writing a file for schema constraint needs to be checked upon writing
                // e.g. cfr. FileDescription.MakeValid();
                //
                FileDescription = new FileDescription();
                FileName = new FileName();
                FileSchema = new FileSchema();
            }
        }

        public IIfcFileDescription FileDescription;
        public IIfcFileName FileName;
        public IIfcFileSchema FileSchema;

        public void Write(BinaryWriter binaryWriter)
        {
            FileDescription.Write(binaryWriter);
            FileName.Write(binaryWriter);
            FileSchema.Write(binaryWriter);
        }

        public void Read(BinaryReader binaryReader)
        {
            FileDescription.Read(binaryReader);
            FileName.Read(binaryReader);
            FileSchema.Read(binaryReader);
        }

        IIfcFileDescription IIfcFileHeader.FileDescription
        {
            get
            {
                return FileDescription;
            }
            set
            {
                FileDescription = value;
            }
        }

        IIfcFileName IIfcFileHeader.FileName
        {
            get
            {
                return FileName;
            }
            set
            {
                FileName = value;
            }

        }

        IIfcFileSchema IIfcFileHeader.FileSchema
        {
            get
            {
                return FileSchema;
            }
            set
            {
                FileSchema = value;
            }

        }


        public string SchemaVersion
        {
            get
            {
                if (FileSchema != null && FileSchema.Schemas!=null && FileSchema.Schemas.Count > 0)
                    return string.Join(", ", FileSchema.Schemas);
                else
                    return "";
            }
        }

        public string CreatingApplication
        {
            get
            {
                if (FileName != null && FileName.OriginatingSystem != null)
                    return FileName.OriginatingSystem;
                else
                    return "";
            }
        }

        public string ModelViewDefinition
        {
            get
            {
                if (FileDescription != null && FileDescription.Description != null)
                    return string.Join(", ", FileDescription.Description);
                else
                    return "";

            }
        }

        public string Name
        {
            get 
            {
                if (FileName != null && FileName.Name != null)
                    return FileName.Name;
                else
                    return "";
            }
        }

        public String TimeStamp
        {
            get
            {
                if (FileName != null && FileName.TimeStamp != null)
                    return FileName.TimeStamp;
                else
                    return "";
            }
        }
    }
}