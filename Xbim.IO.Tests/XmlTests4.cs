using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc4;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.StructuralLoadResource;
using Xbim.Ifc4.UtilityResource;
using Xbim.IO.JSON;
using Xbim.IO.Memory;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    public class XmlTests4
    {
       

        [TestMethod]
        public void Ifc4HeaderSerialization()
        {
            const string outPath = "..\\..\\HeaderSampleIfc4.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                model.Header.FileName.Name = "Sample model";
                model.Header.FileName.AuthorName.Add("Martin");
                model.Header.FileName.AuthorizationMailingAddress.Add("martin.cerny@northumbria.ac.uk");
                model.Header.FileName.AuthorizationName = "Martin Cerny, xBIM Team";
                model.Header.FileName.Organization.Add("xBIM Team");
                model.Header.FileName.OriginatingSystem = "xBIM Toolkit";
                model.Header.FileName.PreprocessorVersion = "4.0";
                model.Header.FileName.TimeStamp = DateTime.Now.ToString("s");
                model.Header.FileDescription.Description.Add("xBIM Team Model View Definition");
                model.Header.FileDescription.ImplementationLevel = "1.0";

                using (var xml = XmlWriter.Create(outPath, new XmlWriterSettings{Indent = true}))
                {
                    var writer = new XbimXmlWriter4(configuration.IFC4Add1);
                    writer.Write(model, xml);   
                    xml.Close();
                }

                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);
            }

            //read it back and validate it is the same
            using (var model = new MemoryModel<EntityFactory>())
            {
                model.OpenXml(outPath);

                Assert.IsTrue(model.Header.FileName.Name == "Sample model");
                Assert.IsTrue(model.Header.FileName.OriginatingSystem == "xBIM Toolkit");
                Assert.IsTrue(model.Header.FileName.PreprocessorVersion == "4.0");
            }

        }

        [TestMethod]
        public void SampleHouseXmlSerialization()
        {
            const string outPath = "..\\..\\SampleHouse4.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                model.Open("SampleHouse4.ifc");
                WriteXml(model, outPath);

                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);
                
                WriteJSON(model, "..\\..\\SampleHouse4.json");
            }
        }

        [TestMethod]
        public void AttributesSerialization()
        {
            const string outPath = "..\\..\\StandaloneSite.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                IfcSite site;
                using (var txn = model.BeginTransaction("site"))
                {
                    site = model.Instances.New<IfcSite>();
                    site.Name = "Site name";
                    site.RefElevation = 100.0;
                    site.Description = "Site description";
                    site.RefLatitude = new List<long> { 1, 2, 3, 4 };
                    site.GlobalId = Guid.NewGuid();    
                }
                

                using (var xml = XmlWriter.Create(outPath, new XmlWriterSettings { Indent = true }))
                {
                    var writer = new XbimXmlWriter4(configuration.IFC4Add1);
                    writer.Write(model, xml);
                    xml.Close();
                }

                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);

                using (var model2 = new MemoryModel<EntityFactory>())
                {
                    model2.OpenXml(outPath);
                    var site2 = model2.Instances.FirstOrDefault<IfcSite>();
                    Assert.IsNotNull(site2);

                    Assert.IsTrue(site.Name == site2.Name);
                    Assert.IsTrue(site.RefElevation == site2.RefElevation);
                    Assert.IsTrue(site.Description == site2.Description);
                    Assert.IsTrue(site.RefLatitude == site2.RefLatitude);
                    Assert.IsTrue(site.GlobalId == site2.GlobalId);
                }
            }
        }

        [TestMethod]
        public void PropertySetDefinitionSetSerialization()
        {
            const string outPath = "..\\..\\IfcPropertySetDefinitionSet.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                using (var txn = model.BeginTransaction("IfcPropertySetDefinitionSet"))
                {
                    var pSet1 = model.Instances.New<IfcPropertySet>(p =>
                    {
                        p.Name = "pSet_1";
                        p.GlobalId = Guid.NewGuid();
                        p.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(s => s.Name = "Property"));
                    });
                    var pSet2 = model.Instances.New<IfcPropertySet>(p =>
                    {
                        p.Name = "pSet_2";
                        p.GlobalId = Guid.NewGuid();
                        p.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(s => s.Name = "Property"));
                    });
                    var pSet3 = model.Instances.New<IfcPropertySet>(p =>
                    {
                        p.Name = "pSet_3";
                        p.GlobalId = Guid.NewGuid();
                        p.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(s => s.Name = "Property"));
                    });

                    var set = new IfcPropertySetDefinitionSet(new List<IfcPropertySetDefinition>{pSet1, pSet2, pSet3});
                    var wall = model.Instances.New<IfcWall>(w =>
                    {
                        w.Name = "Sample wall";
                        w.GlobalId = Guid.NewGuid();
                    });

                    model.Instances.New<IfcRelDefinesByProperties>(r =>
                    {
                        r.RelatingPropertyDefinition = set;
                        r.RelatedObjects.Add(wall);
                        r.GlobalId = Guid.NewGuid();
                    });

                    txn.Commit();
                }


                WriteXml(model, outPath);
                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);

                WriteJSON(model, "..\\..\\properties.json");
            }
        }
        [TestMethod]
        public void RectangularListSerialization()
        {
            const string outPath = "..\\..\\IfcCartesianPointList3D.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                using (var txn = model.BeginTransaction("Rect"))
                {
                    var pl = model.Instances.New<IfcCartesianPointList3D>();
                    var a1 = pl.CoordList.GetAt(0);
                    var a2 = pl.CoordList.GetAt(1);
                    var a3 = pl.CoordList.GetAt(2);

                    a1.Add(1.0);
                    a1.Add(2.0);
                    a1.Add(3.0);
                    a2.Add(4.0);
                    a2.Add(5.0);
                    a2.Add(6.0);
                    a3.Add(7.0);
                    a3.Add(8.0);
                    a3.Add(9.0);

                    txn.Commit();
                }


                WriteXml(model, outPath);
                var errs = ValidateIfc4(outPath);
                
                Assert.AreEqual(0, errs);

                var xmlString = File.ReadAllText(outPath);
                Assert.IsTrue(xmlString.Contains("CoordList=\"1 2 3 4 5 6 7 8 9\""));
            }
        }

        [TestMethod]
        public void NonRectangularListSerialization()
        {
            const string outPath = "..\\..\\IfcStructuralLoadConfiguration.xml";
            using (var model = new MemoryModel<EntityFactory>())
            {
                using (var txn = model.BeginTransaction("Rect"))
                {
                    var slc = model.Instances.New<IfcStructuralLoadConfiguration>();
                    var a1 = slc.Locations.GetAt(0);
                    var a2 = slc.Locations.GetAt(1);
                    var a3 = slc.Locations.GetAt(2);

                    a1.Add(1.0);
                    a1.Add(2.0);
                    a2.Add(4.0);
                    a2.Add(5.0);
                    a3.Add(7.0);
                    a3.Add(8.0);

                    slc.Values.Add(model.Instances.New<IfcStructuralLoadLinearForce>());
                    slc.Values.Add(model.Instances.New<IfcStructuralLoadLinearForce>());
                    slc.Values.Add(model.Instances.New<IfcStructuralLoadLinearForce>());

                    txn.Commit();
                }


                WriteXml(model, outPath);
                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);

                var xmlString = File.ReadAllText(outPath);
                Assert.IsTrue(xmlString.Contains("pos=\"0 0\""));
                Assert.IsTrue(xmlString.Contains("pos=\"0 1\""));
                Assert.IsTrue(xmlString.Contains("pos=\"1 0\""));
                Assert.IsTrue(xmlString.Contains("pos=\"1 1\""));
                Assert.IsTrue(xmlString.Contains("pos=\"2 0\""));
                Assert.IsTrue(xmlString.Contains("pos=\"2 1\""));
            }
        }



        private static void WriteXml(IModel model, string path)
        {
            using (var xml = XmlWriter.Create(path, new XmlWriterSettings { Indent = true }))
            {
                var writer = new XbimXmlWriter4(configuration.IFC4Add1);
                var project = model.Instances.OfType<IfcProject>();
                var products = model.Instances.OfType<IfcObject>();
                var relations = model.Instances.OfType<IfcRelationship>();

                var all =
                    new IPersistEntity[] {}
                        //start from root
                        .Concat(project)
                        //add all products not referenced in the project tree
                        .Concat(products)
                        //add all relations which are not inversed
                        .Concat(relations)
                        //make sure all other objects will get written
                        .Concat(model.Instances);
                
                writer.Write(model, xml, all);
                xml.Close();
            }
        }

        private static void WriteJSON(IModel model, string path)
        {
            using (var xml = new JSONWritter(File.CreateText(path)))
            {
                var writer = new XbimXmlWriter4(configuration.IFC4Add1);
                var project = model.Instances.OfType<IfcProject>();
                var products = model.Instances.OfType<IfcObject>();
                var relations = model.Instances.OfType<IfcRelationship>();

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

                writer.Write(model, xml, all);
                xml.Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="path">Path of the file to be validated</param>
        /// <returns>Number of errors</returns>
        private static int ValidateIfc4(string path)
        {
            var logPath = Path.ChangeExtension(path, ".log");
            var errCount = 0;

            using (var file = File.OpenText(path))
            {
                using (var logFile = File.CreateText(logPath))
                {
                    var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                    settings.ValidationEventHandler += (sender, args) =>
                    {
                        Debug.WriteLine("Validation {0}: {1} \nLine: {2}, Position: {3}", args.Severity, args.Message, args.Exception.LineNumber, args.Exception.LinePosition);
                        logFile.WriteLine("Validation {0}: {1} \nLine: {2}, Position: {3}", args.Severity, args.Message, args.Exception.LineNumber, args.Exception.LinePosition);
                        errCount++;
                    };
                    var reader = XmlReader.Create(file, settings);

                    while (reader.Read())
                    {

                    }   

                    logFile.Close();
                    file.Close();
                }
            }

            return errCount;
        }
    }
}
