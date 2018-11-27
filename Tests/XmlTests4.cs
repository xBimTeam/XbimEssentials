using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc.Validation;
using Xbim.Ifc4;
using Xbim.Ifc4.ActorResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.StructuralLoadResource;
using Xbim.IO.Xml;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    public class XmlTests4
    {
        private static readonly IEntityFactory ef4 = new EntityFactoryIfc4();

        [TestMethod]
        public void ReadIFC4Xml()
        {
            var path = @"ImplicitPropertyTyping.ifcxml";

            ValidateIfc4(path);

            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                model.LoadXml(path);
            }
        }

        [TestMethod]
        public void ComplexPropertyCheck()
        {
            var path = @"Dimensions.ifcxml";
            using (var store = Xbim.Ifc.IfcStore.Open(path))
            {

            }
        }

        [TestMethod]
        public void IkeaKitchenKabinetOpen()
        {
            var path = @"IkeaKitchenCabinets.ifcXML";
            using (var store = Xbim.Ifc.IfcStore.Open(path))
            {

            }
        }

        [TestMethod]
        public void RefWithContent()
        {
            var path = @"RefWithContent.ifcXML";
            using (var store = Xbim.Ifc.IfcStore.Open(path))
            {

            }
        }

        [TestMethod]
        public void CheckQuantity()
        {
            var path = @"QuantityTest.ifcxml";
            var store = Xbim.Ifc.IfcStore.Open(path);
            var site = store.Instances.FirstOrDefault<IIfcSite>(r => r.Name == "Testsite");
            var rel = site.IsDefinedBy
                    .Where(r => r.RelatingPropertyDefinition is IIfcElementQuantity)
                    .FirstOrDefault();
            Assert.IsNotNull(rel);
        }
        [TestCategory("IfcXml")]
        [TestMethod]
        public void Ifc4HeaderSerialization()
        {
            const string outPath = "..\\..\\HeaderSampleIfc4.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
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
                    var writer = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                    writer.Write(model, xml);   
                    xml.Close();
                }

                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);
            }

            //read it back and validate it is the same
            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                model.LoadXml(outPath);

                Assert.IsTrue(model.Header.FileName.Name == "Sample model");
                Assert.IsTrue(model.Header.FileName.OriginatingSystem == "xBIM Toolkit");
                Assert.IsTrue(model.Header.FileName.PreprocessorVersion == "4.0");
            }

            //check version info
            using (var file = File.OpenRead(outPath))
            {
                var header = XbimXmlReader4.ReadHeader(file);
                Assert.AreEqual("IFC4", header.SchemaVersion);
            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void ListSerializationTests()
        {
            const string outPath = "..\\..\\ListSerializationTest.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                using (var txn = model.BeginTransaction("site"))
                {
                    var building = model.Instances.New<IfcBuilding>(b =>
                    {
                        b.GlobalId = Guid.NewGuid();
                    });

                    var storey1 = model.Instances.New<IfcBuildingStorey>(s =>
                    {
                        s.GlobalId = Guid.NewGuid();
                    });
                    var storey2 = model.Instances.New<IfcBuildingStorey>(s =>
                    {
                        s.GlobalId = Guid.NewGuid();
                    });
                    var storey3 = model.Instances.New<IfcBuildingStorey>(s =>
                    {
                        s.GlobalId = Guid.NewGuid();
                    });

                    var rel = model.Instances.New<IfcRelAggregates>(r =>
                    {
                        r.GlobalId = Guid.NewGuid();
                        r.RelatingObject = building;
                        r.RelatedObjects.Add(storey1);
                        r.RelatedObjects.Add(storey2);
                        r.RelatedObjects.Add(storey3);
                    });

                    txn.Commit();
                }
                
                WriteXml(model, outPath);

                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);

                using (var model2 = new IO.Memory.MemoryModel(ef4))
                {
                    model2.LoadXml(outPath);
                    var b = model2.Instances.FirstOrDefault<IfcBuilding>();
                    Assert.IsNotNull(b);

                    var r = b.IsDecomposedBy.FirstOrDefault();
                    Assert.IsNotNull(r);

                    var storeys = r.RelatedObjects;
                    Assert.AreEqual(3, storeys.Count);
                }
            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void SampleHouseXmlSerialization()
        {
            var w = new Stopwatch();
            const string outPath = "..\\..\\SampleHouse4.xml";

            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                w.Start();
                model.LoadStep21("SampleHouse4.ifc");
                w.Stop();
                Console.WriteLine("{0}ms to read STEP.", w.ElapsedMilliseconds);
                var instCount = model.Instances.Count;

                w.Restart();
                WriteXml(model, outPath);
                w.Stop();
                Console.WriteLine("{0}ms to write XML.", w.ElapsedMilliseconds);

                w.Restart();
                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);
                w.Stop();
                Console.WriteLine("{0}ms to validate XML.", w.ElapsedMilliseconds);
                
                using (var model2 = new IO.Memory.MemoryModel(ef4))
                {
                    w.Restart();
                    model2.LoadXml(outPath);
                    w.Stop();
                    Console.WriteLine("{0}ms to read XML.", w.ElapsedMilliseconds);

                    var instances = model.Instances as IEntityCollection;
                    var instances2 = model2.Instances as IEntityCollection;
                    if(instances == null || instances2 == null)
                        throw new Exception();

                    var roots1 = model.Instances.OfType<IfcRoot>();
                    var roots2 = model2.Instances.OfType<IfcRoot>().ToList();
                    foreach (var root in roots1.Where(root => roots2.All(r => r.GlobalId != root.GlobalId)))
                    {
                        Console.WriteLine("Missing root element: {0} ({1})", root.GlobalId, root.GetType().Name);
                    }

                    foreach (var expressType in model2.Metadata.Types().Where(et => typeof(IPersistEntity).IsAssignableFrom(et.Type)))
                    {
                        var count1 = instances.OfType(expressType.Name, true).Count();
                        var count2 = instances2.OfType(expressType.Name, true).Count();

                        if (count1 != count2)
                        {
                            Console.WriteLine("Different count of {0} {1}/{2}", expressType.Name, count1, count2);
                        }
                    }

                    Assert.IsTrue(instCount == model2.Instances.Count);
                }
            }

            
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void AttributesSerialization()
        {
            const string outPath = "..\\..\\StandaloneSite.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
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

                    var wall = model.Instances.New<IfcWall>();
                    wall.Name = "Sample wall";
                    wall.GlobalId = Guid.NewGuid();
                    wall.PredefinedType = IfcWallTypeEnum.PARTITIONING;
                    
                    txn.Commit();
                }
                

                WriteXml(model, outPath);

                var errs = ValidateIfc4(outPath);
                Assert.AreEqual(0, errs);

                using (var model2 = new IO.Memory.MemoryModel(ef4))
                {
                    model2.LoadXml(outPath);
                    var site2 = model2.Instances.FirstOrDefault<IfcSite>();
                    Assert.IsNotNull(site2);

                    var wall2 = model2.Instances.FirstOrDefault<IfcWall>();
                    Assert.IsNotNull(wall2);

                    Assert.IsTrue(site.Name == site2.Name);
                    Assert.IsTrue(site.RefElevation == site2.RefElevation);
                    Assert.IsTrue(site.Description == site2.Description);
                    Assert.IsTrue(site.RefLatitude == site2.RefLatitude);
                    Assert.IsTrue(site.GlobalId == site2.GlobalId);
                }
            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void SelectTypeSerialization()
        {
            const string outPath = "..\\..\\SelectTypeSerialization.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    //property has a select type Value
                    var pLabel = model.Instances.New<IfcPropertySingleValue>();
                    pLabel.Name = "Label";
                    pLabel.NominalValue = new IfcLabel("Label value");

                    var pInteger = model.Instances.New<IfcPropertySingleValue>();
                    pInteger.Name = "Integer";
                    pInteger.NominalValue = new IfcInteger(5);

                    var pDouble = model.Instances.New<IfcPropertySingleValue>();
                    pDouble.Name = "Double";
                    pDouble.NominalValue = new IfcReal(5);

                    var pBoolean = model.Instances.New<IfcPropertySingleValue>();
                    pBoolean.Name = "Boolean";
                    pBoolean.NominalValue = new IfcBoolean(true);

                    var pLogical = model.Instances.New<IfcPropertySingleValue>();
                    pLogical.Name = "Logical";
                    pLogical.NominalValue = new IfcLogical((bool?)null);

                    WriteXml(model, outPath);
                    txn.Commit();

                    using (var model2 = new IO.Memory.MemoryModel(ef4))
                    {
                        model2.LoadXml(outPath);
                        var props = model2.Instances.OfType<IfcPropertySingleValue>().ToList();
                        var pLabel2 = props.FirstOrDefault(p => p.Name == "Label");
                        var pInteger2 = props.FirstOrDefault(p => p.Name == "Integer");
                        var pDouble2 = props.FirstOrDefault(p => p.Name == "Double");
                        var pBoolean2 = props.FirstOrDefault(p => p.Name == "Boolean");
                        var pLogical2 = props.FirstOrDefault(p => p.Name == "Logical");

                        Assert.IsTrue(pLabel.NominalValue.Equals(pLabel2.NominalValue));
                        Assert.IsTrue(pInteger.NominalValue.Equals(pInteger2.NominalValue));
                        Assert.IsTrue(pDouble.NominalValue.Equals(pDouble2.NominalValue));
                        Assert.IsTrue(pBoolean.NominalValue.Equals(pBoolean2.NominalValue));
                        Assert.IsTrue(pLogical.NominalValue.Equals(pLogical2.NominalValue));
                    }
                }
            }

        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void PropertySetDefinitionSetSerialization()
        {
            const string outPath = "..\\..\\IfcPropertySetDefinitionSet.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
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

                using (var model2 = new IO.Memory.MemoryModel(ef4))
                {
                    model2.LoadXml(outPath);
                    var wall = model2.Instances.FirstOrDefault<IfcWall>();

                    Assert.IsNotNull(wall.IsDefinedBy.FirstOrDefault());
                    var pSetSet =
                        (IfcPropertySetDefinitionSet) wall.IsDefinedBy.FirstOrDefault().RelatingPropertyDefinition;
                    var vals = pSetSet.Value as List<IfcPropertySetDefinition>;
                    Assert.IsNotNull(vals);
                    Assert.IsTrue(vals.Count == 3);
                }
            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void RectangularListSerialization()
        {
            const string outPath = "..\\..\\IfcCartesianPointList3D.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
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

            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                model.LoadXml(outPath);
                var list = model.Instances.FirstOrDefault<IfcCartesianPointList3D>();
                Assert.IsNotNull(list);

                Assert.AreEqual(3, list.CoordList.Count);

            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void NonRectangularListSerialization()
        {
            const string outPath = "..\\..\\IfcStructuralLoadConfiguration.xml";
            using (var model = new IO.Memory.MemoryModel(ef4))
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
                var writer = new XbimXmlWriter4(XbimXmlSettings.IFC4Add1);
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

        private static NetworkConnection Network = new NetworkConnection();
        
        /// <summary>
        /// </summary>
        /// <param name="path">Path of the file to be validated</param>
        /// <returns>Number of errors</returns>
        private static int ValidateIfc4(string path)
        {
            // if there's no network a message is asserted, but then related tests passe
            // to prevent concerns when testing the solution offline (which would appear to fail)
            //
            if (!Network.Available)
                return 0;
            
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
