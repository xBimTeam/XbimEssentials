using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CobieExpress;
using Xbim.CobieExpress.IO;
using Xbim.CobieExpress.IO.Resolvers;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO.TableStore;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class TableStorageTests
    {
        //[TestMethod]
        public void SplitAndExport()
        {
            const string file = @"c:\Users\mxfm2\Desktop\Jeff\CFH-IBI-B01-ZZ-M3-BA-001_MainBuilding_v3_2016.cobie";
            using (var cobie = CobieModel.OpenStep21(file))
            {
                var floors = cobie.Instances.OfType<CobieFloor>();
                foreach (var floor in floors)
                {
                    var components = floor.Spaces.SelectMany(s => s.Components);
                    var floorName = floor.Name;
                    var output = Path.ChangeExtension(file, "_" + floorName + ".cobie");
                    var outputXlsx = Path.ChangeExtension(file, "_" + floorName + ".xlsx");
                    using (var cobieFloor = new CobieModel())
                    {
                        using (var txn = cobieFloor.BeginTransaction("Insertion of a single floor"))
                        {
                            cobieFloor.InsertCopy(components, false, new XbimInstanceHandleMap(cobie, cobieFloor));
                            MakeUniqueNames<CobieFacility>(cobieFloor);
                            MakeUniqueNames<CobieFloor>(cobieFloor);
                            MakeUniqueNames<CobieSpace>(cobieFloor);
                            MakeUniqueNames<CobieZone>(cobieFloor);
                            MakeUniqueNames<CobieComponent>(cobieFloor);
                            MakeUniqueNames<CobieSystem>(cobieFloor);
                            MakeUniqueNames<CobieType>(cobieFloor);
                            txn.Commit();                            
                        }
                        cobieFloor.SaveAsStep21(output);
                        string report;
                        cobieFloor.ExportToTable(outputXlsx, out report);
                    }
                }
            }
        }

        private static void MakeUniqueNames<T>(IModel model) where T : CobieAsset
        {
            var groups = model.Instances.OfType<T>().GroupBy(a => a.Name);
            foreach (var @group in groups)
            {
                if (group.Count() == 1)
                {
                    var item = group.First();
                    if (string.IsNullOrEmpty(item.Name))
                        item.Name = item.ExternalObject.Name;
                    continue;
                }

                var counter = 1;
                foreach (var item in group)
                {
                    if (string.IsNullOrEmpty(item.Name))
                        item.Name = item.ExternalObject.Name;
                    item.Name = string.Format("{0} ({1})", item.Name, counter++);
                }
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\SampleHouse4.ifc")]
        public void Ifc4TableStorageTest()
        {
            const string file = "..\\..\\SampleHouseFromIfc.xlsx";
            var model = IfcStore.Open("SampleHouse4.ifc");
            var mapping = ModelMapping.Load(Properties.Resources.IFC4SampleMapping);
            mapping.Init(model.Metadata);

            var w = new Stopwatch();
            w.Start();
            var storage = new TableStore(model, mapping);
            storage.Store(file);
            w.Stop();
            //Debug.WriteLine(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds);
            Trace.WriteLine(string.Format(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds));

            var loaded = new IO.Memory.MemoryModel(new Ifc4.EntityFactory());
            using (var txn = loaded.BeginTransaction("Import from XLSX"))
            {
                storage = new TableStore(loaded, mapping);
                storage.LoadFrom(file);
                txn.Commit();
            }

        }

        [TestMethod]
        public void Ifc4PSetsTest()
        {
            const string file = "PSetsSample.xlsx";
            var model = new IO.Memory.MemoryModel(new Ifc4.EntityFactory());
            using (var txn = model.BeginTransaction("Sample data"))
            {
                model.EntityNew += entity =>
                {
                    var root = entity as IfcRoot;
                    if (root != null)
                        root.GlobalId = Guid.NewGuid();
                };
                var slab = model.Instances.New<IfcSlab>(s => s.Name = "Tremendous slab");
                model.Instances.New<IfcRelDefinesByProperties>(r =>
                {
                    r.RelatedObjects.Add(slab);
                    r.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(ps =>
                    {
                        ps.Name = "Slab properties A";
                        ps.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(p =>
                        {
                            p.Name = "Property AA";
                            p.NominalValue = new IfcLengthMeasure(5.5);
                        }));
                        ps.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(p =>
                        {
                            p.Name = "Property AB";
                            p.NominalValue = new IfcLogical(true);
                        }));
                    });
                });
                model.Instances.New<IfcRelDefinesByProperties>(r =>
                {
                    r.RelatedObjects.Add(slab);
                    r.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(ps =>
                    {
                        ps.Name = "Slab properties B";
                        ps.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(p =>
                        {
                            p.Name = "Property BA";
                            p.NominalValue = new IfcInteger(5);
                        }));
                        ps.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(p =>
                        {
                            p.Name = "Property BB";
                            p.NominalValue = new IfcLabel("LabellebaL");
                        }));
                    });
                });
                txn.Commit();
            }


            var mapping = ModelMapping.Load(Properties.Resources.IFC4SampleMapping);
            mapping.Init(model.Metadata);

            var w = new Stopwatch();
            w.Start();
            var storage = new TableStore(model, mapping);
            storage.Store(file);
            w.Stop();
            //Debug.WriteLine(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds);
            Trace.WriteLine(string.Format(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds));

            var loaded = new IO.Memory.MemoryModel(new Ifc4.EntityFactory());
            using (var txn = loaded.BeginTransaction("Import from XLSX"))
            {
                storage = new TableStore(loaded, mapping);
                storage.LoadFrom(file);
                txn.Commit();
            }

            var sl = loaded.Instances.FirstOrDefault<IfcSlab>();
            Assert.IsNotNull(sl);

            var rels = sl.IsDefinedBy.ToList();
            Assert.AreEqual(2, rels.Count);

            foreach (var rel in rels)
            {
                var pSet = rel.RelatingPropertyDefinition as IfcPropertySet;
                Assert.IsNotNull(pSet);
                Assert.IsNotNull(pSet.Name);

                if (pSet.Name == "Slab properties A")
                {
                    var propA = pSet.HasProperties.OfType<IfcPropertySingleValue>().First(p => p.Name == "Property AA");
                    Assert.IsNotNull(propA);
                    Assert.IsNotNull(propA.NominalValue);
                    Assert.IsTrue(Math.Abs((IfcLengthMeasure)propA.NominalValue - 5.5) < 1e-9);
                    var propB = pSet.HasProperties.OfType<IfcPropertySingleValue>().First(p => p.Name == "Property AB");
                    Assert.IsNotNull(propB);
                    Assert.IsNotNull(propB.NominalValue);
                    Assert.IsTrue(((IfcLogical)propB.NominalValue).Equals((IfcLogical)true));
                }
                if (pSet.Name == "Slab properties B")
                {
                    var propA = pSet.HasProperties.OfType<IfcPropertySingleValue>().First(p => p.Name == "Property BA");
                    Assert.IsNotNull(propA);
                    Assert.IsNotNull(propA.NominalValue);
                    Assert.IsTrue(((IfcInteger)propA.NominalValue).Equals((IfcInteger)5));
                    var propB = pSet.HasProperties.OfType<IfcPropertySingleValue>().First(p => p.Name == "Property BB");
                    Assert.IsNotNull(propB);
                    Assert.IsNotNull(propB.NominalValue);
                    Assert.IsTrue(((IfcLabel)propB.NominalValue).Equals((IfcLabel)"LabellebaL"));
                }
            }

        }

        //[TestMethod]
        //[DeploymentItem("TestFiles/LakesideRestaurant.cobieZip")]
        //public void StoreAsXLSX()
        //{
        //    var model = CobieModel.OpenStep21Zip("LakesideRestaurant.cobieZip");
        //    //var mapping = GetSimpleMapping();
        //    var mapping = GetCobieMapping();
        //    mapping.Init(model.Metadata);

        //    var w = new Stopwatch();
        //    w.Start();
        //    var storage = new TableStore(model, mapping);
        //    storage.Store("..\\..\\Lakeside.xlsx");
        //    w.Stop();
        //    //Debug.WriteLine(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds);
        //    Trace.WriteLine(string.Format( @"{0}ms to store the data as a table.", w.ElapsedMilliseconds));
        //}

        //[TestMethod]
        //[DeploymentItem("TestFiles/LakesideRestaurant.cobieZip")]
        //public void LoadFromXLSX()
        //{
        //    //load back
        //    var loaded = new CobieModel();
        //    var mapping = GetCobieMapping();
        //    var storage = new TableStore(loaded, mapping);
        //    storage.Resolvers.Add(new AttributeTypeResolver());

        //    using (var txn = loaded.BeginTransaction("Loading XLSX"))
        //    {
        //        storage.LoadFrom("..\\..\\Lakeside.xlsx");
        //        txn.Commit();
        //    }

        //    storage.Store("..\\..\\Lakeside2.xlsx");
        //}

        [TestMethod]
        public void AssemblyRoundTrip()
        {
            const string file = "assembly.xlsx";
            var test = new CobieModel();
            using (var txn = test.BeginTransaction("Sample data"))
            {
                test.SetDefaultEntityInfo(DateTime.Now, "martin.cerny@northumbria.ac.uk", "Martin", "Černý");
                test.Instances.New<CobieComponent>(c =>
                {
                    c.Name = "Component A";
                    c.AssemblyOf.Add(test.Instances.New<CobieComponent>(c1 =>
                    {
                        c1.Name = "Component B";
                    }));
                });

                txn.Commit();
            }

            string report;
            test.ExportToTable(file, out report);
            Assert.IsTrue(string.IsNullOrWhiteSpace(report));

            var model = CobieModel.ImportFromTable(file, out report);
            Assert.IsTrue(string.IsNullOrWhiteSpace(report));

            var a = model.Instances.FirstOrDefault<CobieComponent>(c => c.Name.Contains("A"));
            var b = model.Instances.FirstOrDefault<CobieComponent>(c => c.Name.Contains("B"));

            Assert.IsTrue(a.AssemblyOf.Contains(b));
            
            //purge after test
            File.Delete(file);
        }


        [TestMethod]
        public void SimpleSubObjectDeserialization()
        {
            const string file = "facility.xlsx";
            var test = new CobieModel();
            using (var txn = test.BeginTransaction("Sample data"))
            {
                test.SetDefaultEntityInfo(DateTime.Now, "martin.cerny@northumbria.ac.uk", "Martin", "Černý");
                test.Instances.New<CobieFacility>(f =>
                {
                    f.Name = "Superb Facility";
                    f.VolumeUnits = test.Instances.New<CobieVolumeUnit>(u => u.Value = "square meters");
                    f.Site = test.Instances.New<CobieSite>(s =>
                    {
                        s.Name = "Spectacular site";
                        s.Description = "The best site you can imagine";
                        s.ExternalId = "156";
                    });
                    f.Attributes.Add(test.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "String attribute";
                        a.Description = "Perfect description";
                        a.Value = new StringValue("Martin");
                    }));
                    f.Attributes.Add(test.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Boolean attribute";
                        a.Description = "Perfect description";
                        a.Value = new BooleanValue(true);
                    }));
                    f.Attributes.Add(test.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Float attribute";
                        a.Description = "Perfect description";
                        a.Value = new FloatValue(15.5d);
                    }));
                    f.Attributes.Add(test.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Date attribute";
                        a.Description = "Perfect description";
                        a.Value = new DateTimeValue("2009-06-15T13:45:30");
                    }));
                    f.Attributes.Add(test.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Integer attribute";
                        a.Description = "Perfect description";
                        a.Value = new IntegerValue(15);
                    }));
                });
                test.Instances.New<CobieType>(t =>
                {
                    t.Name = "Boiler";
                    t.Description = "Very performant boiler which doesn't use almost any energy";
                    t.Warranty = test.Instances.New<CobieWarranty>(w =>
                    {
                        w.Description = "Warranty information for a boiler";
                        w.DurationLabor = 45;
                        w.DurationParts = 78;
                    });
                });
                txn.Commit();
            }

            string report;
            test.ExportToTable(file, out report);
            Assert.IsTrue(string.IsNullOrWhiteSpace(report));

            var model = CobieModel.ImportFromTable(file, out report);
            Assert.IsTrue(string.IsNullOrWhiteSpace(report));
            
            var facility = model.Instances.FirstOrDefault<CobieFacility>();
            var type = model.Instances.FirstOrDefault<CobieType>();
            var createdInfo = model.Instances.OfType<CobieCreatedInfo>();

            Assert.IsNotNull(facility);
            Assert.IsNotNull(type);

            Assert.IsNotNull(facility.Site);
            Assert.IsNotNull(facility.Site.Name);
            Assert.IsNotNull(facility.Site.Description);
            Assert.IsNotNull(facility.Site.ExternalId);

            Assert.IsNotNull(type.Warranty);
            Assert.IsNotNull(type.Warranty.Description);
            Assert.IsNotNull(type.Warranty.DurationParts);
            Assert.IsNotNull(type.Warranty.DurationLabor);

            Assert.IsNotNull(facility.VolumeUnits);
            Assert.IsTrue(facility.Attributes.Count == 5);
            Assert.IsTrue(createdInfo.Count() == 1);

            //check converted values of attributes (that uses custom resolver)
            var str = (StringValue)facility.Attributes.FirstOrDefault(a => a.Name == "String attribute").Value;
            var bl = (BooleanValue)facility.Attributes.FirstOrDefault(a => a.Name == "Boolean attribute").Value;
            var fl = (FloatValue)facility.Attributes.FirstOrDefault(a => a.Name == "Float attribute").Value;
            var dt = (DateTimeValue)facility.Attributes.FirstOrDefault(a => a.Name == "Date attribute").Value;
            var i = (IntegerValue)facility.Attributes.FirstOrDefault(a => a.Name == "Integer attribute").Value;

            Assert.IsTrue(str == "Martin");
            Assert.IsTrue(bl == true);
            Assert.IsTrue(Math.Abs(fl - 15.5d) < 1e-5);
            Assert.IsTrue(dt == "2009-06-15T13:45:30");
            Assert.IsTrue(i == 15);

            //purge after test
            File.Delete(file);
        }

        private static ModelMapping GetCobieMapping()
        {
            return ModelMapping.Load(CobieExpress.IO.Properties.Resources.COBieUK2012);
        }
    }
}
