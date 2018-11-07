using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;
using Xbim.IO.Step21;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;

namespace Xbim.Ifc
{
    public static class IfcStore2
    {
        private static IModel SetUp(IModel model, XbimEditorCredentials editorDetails, string fileName = null, string xbimFileName = null)
        {
            if (editorDetails == null)
                editorDetails = new XbimEditorCredentials()
                {
                    ApplicationDevelopersName = "Unspecified",
                    ApplicationVersion = "Unspecified",
                    ApplicationFullName = "Unspecified",
                    EditorsFamilyName = Environment.UserName,
                    EditorsOrganisationName = "Unspecified",
                    EditorsGivenName = ""
                };

            IIfcOwnerHistory added = null;
            IIfcOwnerHistory modified = null;
            model.EntityNew += (entity) => {
                if (entity is IIfcRoot root)
                {
                    if (added == null)
                        added = OwnerHistoryAddObject(model, editorDetails);

                    root.OwnerHistory = added;
                    root.GlobalId = Guid.NewGuid().ToPart21();
                    added.LastModifiedDate = DateTime.Now;
                }
            };
            model.EntityModified += (entity, property) => {
                if (entity is IIfcRoot root && root.OwnerHistory != modified)
                {
                    if (modified == null)
                        modified = OwnerHistoryAddObject(model, editorDetails);

                    root.OwnerHistory = modified;
                    root.GlobalId = Guid.NewGuid().ToPart21();
                    modified.LastModifiedDate = DateTime.Now;
                }
            };
            CalculateModelFactors(model);
            return model;
        }

        private static EsentModel CreateEsentModel(XbimSchemaVersion schema, int codePageOverride)
        {
            var ef = GetFactory(schema);
            var model = new EsentModel(ef)
            {
                CodePageOverride = codePageOverride
            };
            return model;
        }

        private static MemoryModel CreateMemoryModel(XbimSchemaVersion schema)
        {
            var ef = GetFactory(schema);
            return new MemoryModel(ef);
        }

        /// <summary>
        /// You can use this function to open IFC model from stream. You need to know file type (IFC, IFCZIP, IFCXML) and schema type (IFC2x3 or IFC4) to be able to use this function.
        /// If you don't know you should the overloaded function which takes file pats as an argument. That will automatically detect schema and file type. If you want to open *.xbim
        /// file you should also use the path based overload because Esent database needs to operate on the file and this function will have to create temporal file if it is not a file stream.
        /// If it is a FileStream this will close it to keep exclusive access.
        /// </summary>
        /// <param name="data">Stream of data</param>
        /// <param name="dataType">Type of data (*.ifc, *.ifcxml, *.ifczip)</param>
        /// <param name="schema">IFC schema (IFC2x3, IFC4). Other schemas are not supported by this class.</param>
        /// <param name="modelType">Type of morel to be used. You can choose between EsentModel and MemoryModel</param>
        /// <param name="editorDetails">Optional details. You should always pass these if you are going to change the data.</param>
        /// <param name="accessMode">Access mode to the stream. This is only important if you choose EsentModel. MemoryModel is completely in memory so this is not relevant</param>
        /// <param name="progDelegate">Progress reporting delegate</param>
        /// <param name="codePageOverride">
        /// A CodePage that will be used to read implicitly encoded one-byte-char strings. If -1 is specified the default ISO8859-1
        /// encoding will be used accoring to the Ifc specification. </param>/// <returns></returns>
        public static IModel Open(Stream data, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, XbimEditorCredentials editorDetails = null, XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
        {
            //any Esent model needs to run from the file so we need to create a temporal one
            var xbimFilePath = Path.GetTempFileName();
            xbimFilePath = Path.ChangeExtension(xbimFilePath, ".xbim");

            switch (dataType)
            {
                case StorageType.Xbim:
                    //xBIM file has to be opened from the file so we need to create temporal file if it is not a local file stream
                    var fStream = data as FileStream;
                    var localFile = false;
                    if (fStream != null)
                    {
                        var name = fStream.Name;
                        //if it is an existing local file, just use it
                        if (File.Exists(name))
                        {
                            xbimFilePath = name;
                            //close the stream from argument to have an exclusive access to the file
                            data.Close();
                            localFile = true;
                        }
                    }
                    if (!localFile)
                    {
                        using (var tempFile = File.Create(xbimFilePath))
                        {
                            data.CopyTo(tempFile);
                            tempFile.Close();
                        }
                    }
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        model.Open(xbimFilePath, accessMode, progDelegate);
                        return SetUp(model, editorDetails, xbimFilePath);
                    }
                case StorageType.IfcXml:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(data, data.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return SetUp(model, editorDetails, xbimFilePath);
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadXml(data, data.Length, progDelegate);
                        return SetUp(model, editorDetails);
                    }
                    throw new ArgumentOutOfRangeException("IfcStore only supports EsentModel and MemoryModel");
                case StorageType.Stp:
                case StorageType.Ifc:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(data, data.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return SetUp(model, editorDetails, xbimFilePath);
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadStep21(data, data.Length, progDelegate);
                        return SetUp(model, editorDetails);
                    }
                    throw new ArgumentOutOfRangeException("IfcStore only supports EsentModel and MemoryModel");
                case StorageType.IfcZip:
                case StorageType.StpZip:
                case StorageType.Zip:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(data, data.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return SetUp(model, editorDetails, xbimFilePath);
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadZip(data, progDelegate);
                        return SetUp(model, editorDetails);
                    }
                    throw new ArgumentOutOfRangeException("IfcStore only supports EsentModel and MemoryModel");
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        //public static IfcStore Open(string path, XbimEditorCredentials editorDetails,bool writeAccess = true,
        //double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null)
        //{
        //    return Open(path, editorDetails, ifcDatabaseSizeThreshHold, progDelegate, writeAccess?XbimDBAccess.ReadWrite : XbimDBAccess.Read);
        //}

        /// <summary>
        /// Opens an IFC file, Ifcxml, IfcZip, xbim
        /// </summary>
        /// <param name="path">the file name of the ifc, ifczip, ifcxml or xbim file to be opened</param>
        /// <param name="editorDetails">This is only required if the store is opened for editing</param>
        /// <param name="ifcDatabaseSizeThreshHold">Expressed in MB. If not defined the DefaultIfcDatabaseSizeThreshHold is used, 
        /// IFC files below this size will be opened in memory, above this size a database will be created. If -1 is specified an in memory model will be 
        /// created for all IFC files that are opened. Xbim files are always opened as databases</param>
        /// <param name="progDelegate"></param>
        /// <param name="accessMode"></param>
        /// <param name="codePageOverride">
        /// A CodePage that will be used to read implicitly encoded one-byte-char strings. If -1 is specified the default ISO8859-1
        /// encoding will be used accoring to the Ifc specification. </param>
        public static IModel Open(string path, XbimEditorCredentials editorDetails = null, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
        {
            path = Path.GetFullPath(path);

            if (!Directory.Exists(Path.GetDirectoryName(path) ?? ""))
                throw new DirectoryNotFoundException(Path.GetDirectoryName(path) + " directory was not found");
            if (!File.Exists(path))
                throw new FileNotFoundException(path + " file was not found");
            var storageType = path.StorageType();
            var ifcVersion = GetXbimSchemaVersion(path);
            if (ifcVersion == XbimSchemaVersion.Unsupported)
            {
                throw new FileLoadException(path + " is not a valid IFC file format, ifc, ifcxml, ifczip and xBIM are supported.");
            }

            if (storageType == StorageType.Xbim) //open the XbimFile
            {
                var model = CreateEsentModel(ifcVersion, codePageOverride);
                model.Open(path, accessMode, progDelegate);
                return SetUp(model, editorDetails, path);
            }
            else //it will be an IFC file if we are at this point
            {
                var fInfo = new FileInfo(path);
                double ifcMaxLength = (ifcDatabaseSizeThreshHold ?? DefaultIfcDatabaseSizeThreshHold) * 1024 * 1024;
                if (ifcMaxLength >= 0 && fInfo.Length > ifcMaxLength) //we need to make an Esent database, if ifcMaxLength<0 we use in memory
                {
                    var tmpFileName = Path.GetTempFileName();
                    tmpFileName = Path.ChangeExtension(tmpFileName, ".xbim");
                    var model = CreateEsentModel(ifcVersion, codePageOverride);
                    if (model.CreateFrom(path, tmpFileName, progDelegate, true))
                        return SetUp(model, editorDetails, path, tmpFileName);
                    throw new FileLoadException(path + " file was not a valid IFC format");
                }
                else //we can use a memory model
                {
                    var ef = GetFactory(ifcVersion);
                    var model = new MemoryModel(ef);
                    if (storageType.HasFlag(StorageType.IfcZip) || storageType.HasFlag(StorageType.Zip))
                    {
                        model.LoadZip(path, progDelegate);
                    }
                    else if (storageType.HasFlag(StorageType.Ifc))
                        model.LoadStep21(path, progDelegate);
                    else if (storageType.HasFlag(StorageType.IfcXml))
                        model.LoadXml(path, progDelegate);

                    // if we are looking at a memory model loaded from a file it might be safe to fix the file name in the 
                    // header with the actual file loaded
                    //
                    FileInfo f = new FileInfo(path);
                    model.Header.FileName.Name = f.FullName;
                    return SetUp(model, editorDetails, path);
                }
            }
        }

        private static IEntityFactory GetFactory(XbimSchemaVersion type)
        {
            switch (type)
            {
                case XbimSchemaVersion.Ifc4:
                    return new Ifc4.EntityFactoryIfc4();
                case XbimSchemaVersion.Ifc4x1:
                    return new Ifc4.EntityFactoryIfc4x1();
                case XbimSchemaVersion.Ifc2X3:
                    return new Ifc2x3.EntityFactoryIfc2x3();
                case XbimSchemaVersion.Cobie2X4:
                case XbimSchemaVersion.Unsupported:
                default:
                    throw new NotSupportedException("Schema '" + type + "' is not supported");
            }
        }

        public static XbimSchemaVersion GetXbimSchemaVersion(string path)
        {
            var storageType = path.StorageType();
            if (storageType == StorageType.Invalid)
            {
                return XbimSchemaVersion.Unsupported;
            }
            IList<string> schemas = null;
            if (storageType != StorageType.Xbim)
            {
                return MemoryModel.GetSchemaVersion(path);
            }

            var stepHeader = EsentModel.GetStepFileHeader(path);
            schemas = stepHeader.FileSchema.Schemas;
            var schemaIdentifier = string.Join(", ", schemas);
            foreach (var schema in schemas)
            {
                if (string.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4;
                if (string.Compare(schema, "Ifc4x1", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4x1;
                if (string.Compare(schema, "Ifc2x3", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc2X3;
                if (schema.StartsWith("Ifc2x", StringComparison.OrdinalIgnoreCase)) //return this as 2x3
                    return XbimSchemaVersion.Ifc2X3;

            }

            return XbimSchemaVersion.Unsupported;
        }

        public static double DefaultIfcDatabaseSizeThreshHold { get; private set; }



        /// <summary>
        /// Creates an Database store at the specified location
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="editorDetails"></param>
        /// <param name="ifcVersion"></param>
        /// <returns></returns>
        public static EsentModel Create(string filePath, XbimEditorCredentials editorDetails, XbimSchemaVersion ifcVersion)
        {
            var ef = GetFactory(ifcVersion);
            var model = EsentModel.CreateModel(ef, filePath);
            SetUp(model, editorDetails, model.DatabaseName);
            return model;
        }

        public static IModel Create(XbimEditorCredentials editorDetails, XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            var ef = GetFactory(ifcVersion);
            if (storageType == XbimStoreType.EsentDatabase)
            {
                var model = EsentModel.CreateTemporaryModel(ef);
                return SetUp(model, editorDetails, model.DatabaseName); //it will delete itself anyway
            }

            var memoryModel = new MemoryModel(ef);
            return SetUp(memoryModel, editorDetails);
        }

        public static IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            return Create(null, ifcVersion, storageType);

        }

        #region OwnerHistory Management
        /// <summary>
        /// Returns default user used to fill in owner history on new or modified objects. This object is only populated if
        /// you provide XbimEditorCredentials in one of constructors
        /// </summary>
        public static IIfcPersonAndOrganization DefaultOwningUser(IModel model, XbimEditorCredentials editorDetails)
        {
            var c = new Create(model);
            var person = c.Person(p =>
            {
                p.GivenName = editorDetails.EditorsGivenName;
                p.FamilyName = editorDetails.EditorsFamilyName;
            });
            var organization = model.Instances.OfType<IIfcOrganization>().FirstOrDefault(o => o.Name == editorDetails.EditorsOrganisationName)
                ?? c.Organization(o => o.Name = editorDetails.EditorsOrganisationName);

            return c.PersonAndOrganization(po =>
            {
                po.TheOrganization = organization;
                po.ThePerson = person;
            });
        }

        /// <summary>
        /// Returns default application used to fill in owner history on new or modified objects. This object is only populated if
        /// you provide XbimEditorCredentials in one of constructors
        /// </summary>
        public static IIfcApplication DefaultOwningApplication(IModel model, XbimEditorCredentials editorDetails)
        {
            var c = new Create(model);
            return c.Application(a =>
            {
                a.ApplicationDeveloper = model.Instances.OfType<Ifc4.ActorResource.IfcOrganization>().FirstOrDefault(o => o.Name == editorDetails.EditorsOrganisationName)
                ?? c.Organization(o => o.Name = editorDetails.EditorsOrganisationName);
                a.ApplicationFullName = editorDetails.ApplicationFullName;
                a.ApplicationIdentifier = editorDetails.ApplicationIdentifier;
                a.Version = editorDetails.ApplicationVersion;
            });

        }

        public static IIfcOwnerHistory OwnerHistoryAddObject(IModel model, XbimEditorCredentials editorDetails)
        {
            var c = new Create(model);
            return c.OwnerHistory(histAdd =>
            {
                histAdd.OwningUser = DefaultOwningUser(model, editorDetails);
                histAdd.OwningApplication = DefaultOwningApplication(model, editorDetails);
                histAdd.ChangeAction = IfcChangeActionEnum.ADDED;
            });

        }

        public static IIfcOwnerHistory OwnerHistoryModifyObject(IModel model, XbimEditorCredentials editorDetails)
        {
            var c = new Create(model);
            return c.OwnerHistory(histAdd =>
            {
                histAdd.OwningUser = DefaultOwningUser(model, editorDetails);
                histAdd.OwningApplication = DefaultOwningApplication(model, editorDetails);
                histAdd.ChangeAction = IfcChangeActionEnum.MODIFIED;
            });

        }
        #endregion

        /// <summary>
        /// Saves the model to the specified file
        /// </summary>
        /// <param name="fileName">Name of the file to save to, if no format is specified the extension is used to determine the format</param>
        /// <param name="format">if specified saves in the required format and changes the extension to the correct one</param>
        /// <param name="progDelegate">reports on progress</param>
        public static void SaveAs(IModel model, string fileName, ReportProgressDelegate progDelegate = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            StorageType format = StorageType.Invalid;
                if (extension == ".ifczip")
                {
                    format = StorageType.IfcZip;
                    format |= StorageType.Ifc; //the default
                }
                else if (extension == ".ifcxml")
                    format = StorageType.IfcXml;
                else if (extension == ".xbim")
                    format = StorageType.Xbim;
                else if (extension == ".ifc")
                {
                    extension = ".ifc";
                    format = StorageType.Ifc; //the default
                }
                else
                {
                    // we don't want to loose the original extension required by the user, but we need to add .ifc 
                    // and set StorageType.Ifc as default
                    extension = extension + ".ifc";
                    format = StorageType.Ifc; //the default
                }
            var actualFileName = Path.ChangeExtension(fileName, extension);
            if (model is EsentModel esentModel)
            {
                var xbimTarget = !string.IsNullOrEmpty(extension) &&
                                 string.Compare(extension, ".xbim", StringComparison.OrdinalIgnoreCase) == 0;
                if (format == StorageType.Xbim)
                {
                    var fullSourcePath = Path.GetFullPath(esentModel.DatabaseName);
                    var fullTargetPath = Path.GetFullPath(fileName);
                    if (string.Compare(fullSourcePath, fullTargetPath, StringComparison.OrdinalIgnoreCase) == 0)
                        return; //do nothing it is already saved
                }
            }
            SaveAs(model, actualFileName, format, progDelegate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualFileName"></param>
        /// <param name="format">this will be correctly set</param>
        /// <param name="progDelegate"></param>
        private static void SaveAs(IModel model, string actualFileName, StorageType format, ReportProgressDelegate progDelegate)
        {
            if (format.HasFlag(StorageType.Xbim)) //special case for xbim
            {
                var ef = GetFactory(model.SchemaVersion);
                using (var esentDb = new EsentModel(ef))
                {
                    esentDb.CreateFrom(model, actualFileName, progDelegate);
                    esentDb.Close();
                }
            }
            else
            {
                using (var fileStream = new FileStream(actualFileName, FileMode.Create, FileAccess.Write))
                {
                    if (format.HasFlag(StorageType.IfcZip))
                        //do zip first so that xml and ifc are not confused by the combination of flags
                        SaveAsIfcZip(model, fileStream, Path.GetFileName(actualFileName), format, progDelegate);
                    else if (format.HasFlag(StorageType.Ifc))
                        SaveAsIfc(model, fileStream, progDelegate);
                    else if (format.HasFlag(StorageType.IfcXml))
                        SaveAsIfcXml(model, fileStream, progDelegate);

                }
            }
        }

        public static void SaveAsIfcXml(IModel model, Stream stream, ReportProgressDelegate progDelegate = null)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                if (model.SchemaVersion == XbimSchemaVersion.Ifc2X3)
                {
                    var writer = new IfcXmlWriter3();
                    writer.Write(model, xmlWriter, model.Instances);

                }
                else
                {
                    var writer = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                    var project = model.Instances.OfType<Ifc4.Kernel.IfcProject>();
                    var products = model.Instances.OfType<Ifc4.Kernel.IfcObject>();
                    var relations = model.Instances.OfType<Ifc4.Kernel.IfcRelationship>();

                    var all =
                        new IPersistEntity[] { }
                        //start from root
                            .Concat(project)
                            //add all products not referenced in the project tree
                            .Concat(products)
                            //add all relations which are not inversed
                            .Concat(relations)
                            //make sure all other objects will get written
                            .Concat(model.Instances);

                    writer.Write(model, xmlWriter, all);
                }
                xmlWriter.Close();
            }
        }

        public static void SaveAsIfc(IModel model, Stream stream, ReportProgressDelegate progDelegate = null)
        {

            using (TextWriter tw = new StreamWriter(stream))
            {
                Part21Writer.Write(model, tw, model.Metadata, null, progDelegate);
                tw.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">The stream will be closed and flushed on return</param>
        /// <param name="zipEntryName">The name of the file zipped inside the file</param>
        /// <param name="storageType">Specify IfcZip and then either IfcXml or Ifc</param>
        /// <param name="progDelegate"></param>
        public static void SaveAsIfcZip(IModel model, Stream stream, string zipEntryName, StorageType storageType, ReportProgressDelegate progDelegate = null)
        {
            Debug.Assert(storageType.HasFlag(StorageType.IfcZip));
            var fileBody = Path.ChangeExtension(zipEntryName,
                storageType.HasFlag(StorageType.IfcXml) ? "ifcXml" : "ifc"
                );

            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var newEntry = zipStream.CreateEntry(fileBody);
                using (var writer = newEntry.Open())
                {

                    if (storageType.HasFlag(StorageType.IfcXml))
                        SaveAsIfcXml(model, writer, progDelegate);
                    else //assume it is Ifc
                        SaveAsIfc(model, writer, progDelegate);
                }

            }
        }

        /// <summary>
        /// If translation is defined, returns matrix translated by the vector
        /// </summary>
        /// <param name="matrix">Input matrix</param>
        /// <param name="translation">Translation</param>
        /// <returns>Translated matrix</returns>
        private static XbimMatrix3D Translate(XbimMatrix3D matrix, IVector3D translation)
        {
            if (translation == null) return matrix;
            var translationMatrix = XbimMatrix3D.CreateTranslation(translation.X, translation.Y, translation.Z);
            return XbimMatrix3D.Multiply(matrix, translationMatrix);
        }

        /// <summary>
        /// This function is used to generate the .wexbim model files.
        /// </summary>
        /// <param name="binaryStream">An open writable streamer.</param>
        /// <param name="products">Optional products to be written to the wexBIM file. If null, all products from the model will be saved</param>
        public static void SaveAsWexBim(IModel model, BinaryWriter binaryStream, IEnumerable<IIfcProduct> products = null, IVector3D translation = null)
        {
            products = products ?? model.Instances.OfType<IIfcProduct>();
            // ReSharper disable RedundantCast
            if (model.GeometryStore == null) throw new XbimException("Geometry store has not been initialised");
            // ReSharper disable once CollectionNeverUpdated.Local
            var colourMap = new XbimColourMap();
            using (var geomRead = model.GeometryStore.BeginRead())
            {

                var lookup = geomRead.ShapeGeometries;
                var styles = geomRead.StyleIds;
                var regions = geomRead.ContextRegions.SelectMany(r => r).ToList();
                //we need to get all the default styles for various products
                var defaultStyles = geomRead.ShapeInstances.Select(i => -(int)i.IfcTypeId).Distinct();
                var allStyles = defaultStyles.Concat(styles).ToList();
                int numberOfGeometries = 0;
                int numberOfVertices = 0;
                int numberOfTriangles = 0;
                int numberOfMatrices = 0;
                int numberOfProducts = 0;
                int numberOfStyles = allStyles.Count;
                //start writing out

                binaryStream.Write((Int32)WexBimId); //magic number

                binaryStream.Write((byte)2); //version of stream, arrays now packed as doubles
                var start = (int)binaryStream.Seek(0, SeekOrigin.Current);
                binaryStream.Write((Int32)0); //number of shapes
                binaryStream.Write((Int32)0); //number of vertices
                binaryStream.Write((Int32)0); //number of triangles
                binaryStream.Write((Int32)0); //number of matrices
                binaryStream.Write((Int32)0); //number of products
                binaryStream.Write((Int32)numberOfStyles); //number of styles
                binaryStream.Write(Convert.ToSingle(model.ModelFactors.OneMetre));
                //write out conversion to meter factor

                binaryStream.Write(Convert.ToInt16(regions.Count)); //write out the population data
                var t = XbimMatrix3D.Identity;
                t = Translate(t, translation);
                foreach (var r in regions)
                {
                    binaryStream.Write((Int32)(r.Population));
                    var bounds = r.ToXbimRect3D();
                    var centre = t.Transform(r.Centre);
                    //write out the centre of the region
                    binaryStream.Write((Single)centre.X);
                    binaryStream.Write((Single)centre.Y);
                    binaryStream.Write((Single)centre.Z);
                    //bounding box of largest region
                    binaryStream.Write(bounds.ToFloatArray());
                }
                //textures

                foreach (var styleId in allStyles)
                {
                    XbimColour colour;
                    if (styleId > 0)
                    {
                        var ss = (IIfcSurfaceStyle)model.Instances[styleId];
                        var texture = XbimTexture.Create(ss);
                        colour = texture.ColourMap.FirstOrDefault();
                    }
                    else //use the default in the colour map for the enetity type
                    {
                        var theType = model.Metadata.GetType((short)Math.Abs(styleId));
                        colour = colourMap[theType.Name];
                    }
                    if (colour == null) colour = XbimColour.DefaultColour;
                    binaryStream.Write((Int32)styleId); //style ID                       
                    binaryStream.Write((Single)colour.Red);
                    binaryStream.Write((Single)colour.Green);
                    binaryStream.Write((Single)colour.Blue);
                    binaryStream.Write((Single)colour.Alpha);

                }

                //write out all the product bounding boxes
                var prodIds = new HashSet<int>();
                foreach (var product in products)
                {
                    if (product is IIfcFeatureElement) continue;
                    prodIds.Add(product.EntityLabel);

                    var bb = XbimRect3D.Empty;
                    foreach (var si in geomRead.ShapeInstancesOfEntity(product))
                    {
                        var transformation = Translate(si.Transformation, translation);
                        var bbPart = XbimRect3D.TransformBy(si.BoundingBox, transformation);
                        //make sure we put the box in the right place and then convert to axis aligned
                        if (bb.IsEmpty) bb = bbPart;
                        else
                            bb.Union(bbPart);
                    }
                    //do not write out anything with no geometry
                    if (bb.IsEmpty) continue;

                    binaryStream.Write((Int32)product.EntityLabel);
                    binaryStream.Write((UInt16)model.Metadata.ExpressTypeId(product));
                    binaryStream.Write(bb.ToFloatArray());
                    numberOfProducts++;
                }

                //projections and openings have already been applied, 

                var toIgnore = new short[4];
                toIgnore[0] = model.Metadata.ExpressTypeId("IFCOPENINGELEMENT");
                toIgnore[1] = model.Metadata.ExpressTypeId("IFCPROJECTIONELEMENT");
                if (model.SchemaVersion == XbimSchemaVersion.Ifc4 || model.SchemaVersion == XbimSchemaVersion.Ifc4x1)
                {
                    toIgnore[2] = model.Metadata.ExpressTypeId("IFCVOIDINGFEATURE");
                    toIgnore[3] = model.Metadata.ExpressTypeId("IFCSURFACEFEATURE");
                }

                foreach (var geometry in lookup)
                {
                    if (geometry.ShapeData.Length <= 0) //no geometry to display so don't write out any products for it
                        continue;
                    var instances = geomRead.ShapeInstancesOfGeometry(geometry.ShapeLabel);



                    var xbimShapeInstances = instances.Where(si => !toIgnore.Contains(si.IfcTypeId) &&
                                                                 si.RepresentationType ==
                                                                 XbimGeometryRepresentationType
                                                                     .OpeningsAndAdditionsIncluded && prodIds.Contains(si.IfcProductLabel)).ToList();
                    if (!xbimShapeInstances.Any()) continue;
                    numberOfGeometries++;
                    binaryStream.Write(xbimShapeInstances.Count); //the number of repetitions of the geometry
                    if (xbimShapeInstances.Count > 1)
                    {
                        foreach (IXbimShapeInstanceData xbimShapeInstance in xbimShapeInstances)
                        //write out each of the ids style and transforms
                        {
                            binaryStream.Write(xbimShapeInstance.IfcProductLabel);
                            binaryStream.Write((UInt16)xbimShapeInstance.IfcTypeId);
                            binaryStream.Write((UInt32)xbimShapeInstance.InstanceLabel);
                            binaryStream.Write((Int32)xbimShapeInstance.StyleLabel > 0
                                ? xbimShapeInstance.StyleLabel
                                : xbimShapeInstance.IfcTypeId * -1);

                            var transformation = Translate(XbimMatrix3D.FromArray(xbimShapeInstance.Transformation), translation);
                            binaryStream.Write(transformation.ToArray());
                            numberOfTriangles +=
                                XbimShapeTriangulation.TriangleCount(((IXbimShapeGeometryData)geometry).ShapeData);
                            numberOfMatrices++;
                        }
                        numberOfVertices +=
                            XbimShapeTriangulation.VerticesCount(((IXbimShapeGeometryData)geometry).ShapeData);
                        // binaryStream.Write(geometry.ShapeData);
                        var ms = new MemoryStream(((IXbimShapeGeometryData)geometry).ShapeData);
                        var br = new BinaryReader(ms);
                        var tr = br.ReadShapeTriangulation();

                        tr.Write(binaryStream);
                    }
                    else //now do the single instances
                    {
                        var xbimShapeInstance = xbimShapeInstances[0];

                        // IXbimShapeGeometryData geometry = ShapeGeometry(kv.Key);
                        binaryStream.Write((Int32)xbimShapeInstance.IfcProductLabel);
                        binaryStream.Write((UInt16)xbimShapeInstance.IfcTypeId);
                        binaryStream.Write((Int32)xbimShapeInstance.InstanceLabel);
                        binaryStream.Write((Int32)xbimShapeInstance.StyleLabel > 0
                            ? xbimShapeInstance.StyleLabel
                            : xbimShapeInstance.IfcTypeId * -1);

                        //Read all vertices and normals in the geometry stream and transform

                        var ms = new MemoryStream(((IXbimShapeGeometryData)geometry).ShapeData);
                        var br = new BinaryReader(ms);
                        var tr = br.ReadShapeTriangulation();
                        var transformation = Translate(xbimShapeInstance.Transformation, translation);
                        var trTransformed = tr.Transform(transformation);
                        trTransformed.Write(binaryStream);
                        numberOfTriangles += XbimShapeTriangulation.TriangleCount(((IXbimShapeGeometryData)geometry).ShapeData);
                        numberOfVertices += XbimShapeTriangulation.VerticesCount(((IXbimShapeGeometryData)geometry).ShapeData);
                    }
                }


                binaryStream.Seek(start, SeekOrigin.Begin);
                binaryStream.Write((Int32)numberOfGeometries);
                binaryStream.Write((Int32)numberOfVertices);
                binaryStream.Write((Int32)numberOfTriangles);
                binaryStream.Write((Int32)numberOfMatrices);
                binaryStream.Write((Int32)numberOfProducts);
                binaryStream.Seek(0, SeekOrigin.End); //go back to end
                // ReSharper restore RedundantCast
            }
        }

        public const int WexBimId = 94132117;

        /// <summary>
        /// Calculates and sets the model factors, call every time a unit of measurement is changed
        /// </summary>
        public static void CalculateModelFactors(IModel model)
        {
            double angleToRadiansConversionFactor = 1; //assume radians
            double lengthToMetresConversionFactor = 1; //assume metres
            var instOfType = model.Instances.OfType<IIfcUnitAssignment>();
            var ua = instOfType.FirstOrDefault();
            if (ua != null)
            {
                foreach (var unit in ua.Units)
                {
                    var value = 1.0;
                    var siUnit = unit as IIfcSIUnit;
                    if (unit is IIfcConversionBasedUnit cbUnit)
                    {
                        var mu = cbUnit.ConversionFactor;
                        if (mu.UnitComponent is IIfcSIUnit component)
                            siUnit = component;
                        var et = ((IExpressValueType)mu.ValueComponent);

                        if (et.UnderlyingSystemType == typeof(double))
                            value *= (double)et.Value;
                        else if (et.UnderlyingSystemType == typeof(int))
                            value *= (int)et.Value;
                        else if (et.UnderlyingSystemType == typeof(long))
                            value *= (long)et.Value;
                    }
                    if (siUnit == null) continue;
                    value *= siUnit.Power;
                    switch (siUnit.UnitType)
                    {
                        case IfcUnitEnum.LENGTHUNIT:
                            lengthToMetresConversionFactor = value;
                            break;
                        case IfcUnitEnum.PLANEANGLEUNIT:
                            angleToRadiansConversionFactor = value;
                            //need to guarantee precision to avoid errors in boolean operations
                            if (Math.Abs(angleToRadiansConversionFactor - (Math.PI / 180)) < 1e-9)
                                angleToRadiansConversionFactor = Math.PI / 180;
                            break;
                    }
                }
            }

            var gcs =
                model.Instances.OfType<IIfcGeometricRepresentationContext>();
            var defaultPrecision = 1e-5;
            //get the Model precision if it is correctly defined
            foreach (var gc in gcs.Where(g => !(g is IIfcGeometricRepresentationSubContext)))
            {
                if (!gc.ContextType.HasValue || string.Compare(gc.ContextType.Value, "model", true) != 0) continue;
                if (!gc.Precision.HasValue) continue;
                if (gc.Precision == 0) continue;
                defaultPrecision = gc.Precision.Value;
                break;
            }

            //check if angle units are incorrectly defined, this happens in some old models
            if (Math.Abs(angleToRadiansConversionFactor - 1) < 1e-10)
            {
                var trimmed = model.Instances.Where<IIfcTrimmedCurve>(trimmedCurve => trimmedCurve.BasisCurve is IIfcConic);
                foreach (var trimmedCurve in trimmed)
                {
                    if (trimmedCurve.MasterRepresentation != IfcTrimmingPreference.PARAMETER)
                        continue;
                    if (
                        !trimmedCurve.Trim1.Concat(trimmedCurve.Trim2)
                            .OfType<IfcParameterValue>()
                            .Select(trim => (double)trim.Value)
                            .Any(val => val > Math.PI * 2)) continue;
                    angleToRadiansConversionFactor = Math.PI / 180;
                    break;
                }
            }

            model.ModelFactors.Initialise(angleToRadiansConversionFactor, lengthToMetresConversionFactor,
                defaultPrecision);

            SetWorkArounds(model);
        }

        /// <summary>
        /// Code to determine model specific workarounds (BIM tool IFC exporter quirks)
        /// </summary>
        private static void SetWorkArounds(IModel model)
        {
            //try Revit first
            string revitPattern = @"- Exporter\s(\d*.\d*.\d*.\d*)";
            if (model.Header.FileName == null || string.IsNullOrWhiteSpace(model.Header.FileName.OriginatingSystem))
                return; //nothing to do
            var matches = Regex.Matches(model.Header.FileName.OriginatingSystem, revitPattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0) //looks like Revit
            {
                if (matches[0].Groups.Count == 2) //we have the build versions
                {
                    if (Version.TryParse(matches[0].Groups[1].Value, out Version modelVersion))
                    {
                        //SurfaceOfLinearExtrusion bug found in version 17.2.0 and earlier
                        var surfaceOfLinearExtrusionVersion = new Version(17, 2, 0, 0);
                        if (modelVersion <= surfaceOfLinearExtrusionVersion)
                            ((XbimModelFactors)model.ModelFactors).AddWorkAround("#SurfaceOfLinearExtrusion");
                    }

                }
            }
        }


        /// <summary>
        /// This is a higher level function which uses InsertCopy function alongside with the knowledge of IFC schema to copy over
        /// products with their types and other related information (classification, aggregation, documents, properties) and optionally
        /// geometry. It will also bring in spatial hierarchy relevant to selected products. However, resulting model is not guaranteed 
        /// to be compliant with any Model View Definition unless you explicitly check the compliance. Context of a single product tend to 
        /// consist from hundreds of objects which need to be identified and copied over so this operation might be potentially expensive.
        /// You should never call this function more than once between two models. It not only selects objects to be copied over but also
        /// excludes other objects from being coppied over so that it doesn't bring the entire model in a chain dependencies. This means
        /// that some objects are modified (like spatial relations) and won't get updated which would lead to an inconsistent copy.
        /// </summary>
        /// <param name="products">Products from other model to be inserted into this model</param>
        /// <param name="includeGeometry">If TRUE, geometry of the products will be copied over.</param>
        /// <param name="keepLabels">If TRUE, entity labels from original model will be used. Always set this to FALSE
        /// if you are going to insert products from multiple source models or if you are going to insert products to a non-empty model</param>
        /// <param name="mappings">Mappings to avoid multiple insertion of objects. Keep a single instance for insertion between two models.
        /// If you also use InsertCopy() function for some other insertions, use the same instance of mappings.</param>
        public static void InsertCopy(IModel target, IEnumerable<IIfcProduct> products, bool includeGeometry, bool keepLabels, IProgress<double> progress = null)
        {
            var e = products.FirstOrDefault();
            if (e == null)
                return;

            var ext = new Extractor(e.Model);
            ext.InsertCopy(target, products, includeGeometry, keepLabels, progress);
        }
    }
}
