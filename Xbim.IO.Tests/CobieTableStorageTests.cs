using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CobieExpress;
using Xbim.CobieExpress.IO;
using Xbim.IO.TableStore;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class CobieTableStorageTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/LakesideRestaurant.cobieZip")]
        public void StoreAsXLSX()
        {
            var model = CobieModel.OpenStep21Zip("LakesideRestaurant.cobieZip");
            //var mapping = GetSimpleMapping();
            var mapping = GetCobieMapping();
            mapping.Init(model.Metadata);

            var w = new Stopwatch();
            w.Start();
            var storage = new TableStore(model, mapping);
            storage.Store("..\\..\\Lakeside.xlsx");
            w.Stop();
            //Debug.WriteLine(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds);
            Trace.WriteLine(string.Format( @"{0}ms to store the data as a table.", w.ElapsedMilliseconds));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/LakesideRestaurant.cobieZip")]
        public void LoadFromXLSX()
        {
            //load back
            var loaded = new CobieModel();
            var mapping = GetCobieMapping();
            var storage = new TableStore(loaded, mapping);

            using (var txn = loaded.BeginTransaction("Loading XLSX"))
            {
                storage.LoadFrom("..\\..\\Lakeside.xlsx");
                txn.Commit();
            }
        }

        [TestMethod]
        public void SimpleSubObjectDeserialization()
        {
            const string file = "facility.xlsx";
            var test = new CobieModel();
            using (var txn = test.BeginTransaction("Sample data"))
            {
                test.SetDefaultNewEntityInfo(DateTime.Now, "martin.cerny@northumbria.ac.uk", "Martin", "Černý");
                test.SetDefaultModifiedEntityInfo(DateTime.Now, "martin.cerny@northumbria.ac.uk", "Martin", "Černý");
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
           

            var mapping = GetCobieMapping();
            var storage = new TableStore(test, mapping);
            storage.Store(file);

            var model = new CobieModel();
            storage = new TableStore(model, mapping);
            using (var txn = model.BeginTransaction("Loading XLSX"))
            {
                storage.LoadFrom(file);
                txn.Commit();
            }

            var context = new ReferenceContext(storage, mapping.ClassMappings.First(m => m.Class == "Attribute"));

            var facility = model.Instances.FirstOrDefault<CobieFacility>();
            var type = model.Instances.FirstOrDefault<CobieType>();

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

            Assert.IsNull(facility.VolumeUnits);
            Assert.IsTrue(facility.Attributes.Any());
        }

        private ModelMapping GetCobieMapping()
        {
            return ModelMapping.Load(CobieExpress.IO.Properties.Resources.COBieUK2012);
        }

        // ReSharper disable once UnusedMember.Local
        private ModelMapping GetSimpleMapping()
        {
            return new ModelMapping
            {
                Name = "Simple COBie mapping",
                PickTableName = "PickLists",
                StatusRepresentations = new List<StatusRepresentation>
                {
                    new StatusRepresentation
                    {
                        Status = DataStatus.Header,
                        Colour = "#CCCCCC",
                        FontWeight = FontWeight.Bold,
                        Border = true
                    },
                    new StatusRepresentation
                    {
                        Status = DataStatus.Required,
                        Colour = "#FFFF99",
                        FontWeight = FontWeight.Normal,
                        Border = true
                    },
                    new StatusRepresentation
                    {
                        Status = DataStatus.Reference,
                        Colour = "#FFCC99",
                        FontWeight = FontWeight.Normal,
                        Border = true
                    },
                    new StatusRepresentation
                    {
                        Status = DataStatus.PickValue,
                        Colour = "#FFCC99",
                        FontWeight = FontWeight.Normal,
                        Border = true
                    },
                    new StatusRepresentation
                    {
                        Status = DataStatus.ExternalReference,
                        Colour = "#CC99FF",
                        FontWeight = FontWeight.Normal,
                        Border = true
                    },
                    new StatusRepresentation
                    {
                        Status = DataStatus.Optional,
                        Colour = "#CCFFCC",
                        FontWeight = FontWeight.Normal,
                        Border = true
                    },
                },
                ClassMappings = new List<ClassMapping>
                {
                    new ClassMapping
                    {
                        Class = "Facility",
                        TableName = "Facility",
                        TableOrder = 0,
                        TableStatus = DataStatus.Required,
                        PropertyMappings = new List<PropertyMapping>
                        {
                            new PropertyMapping
                            {
                                Header = "Name",
                                MultiRow = MultiRow.None,
                                Status = DataStatus.Required,
                                //Colour = "#FFFF99",
                                Column = "A",
                                _Paths = "Name",
                                DefaultValue = "n/a",
                            }
                        }
                    },
                    new ClassMapping
                    {
                        Class = "Attribute",
                        TableName = "Attributes",
                        TableOrder = 1,
                        TableStatus = DataStatus.Optional,
                        ParentClass = "Asset",
                        ParentPath = "Attributes",
                        PropertyMappings = new List<PropertyMapping>
                        {
                            new PropertyMapping
                            {
                                Header = "Name",
                                MultiRow = MultiRow.None,
                                Status = DataStatus.Required,
                                Column = "A",
                                _Paths = "Name",
                                DefaultValue = "n/a",
                            },
                            new PropertyMapping
                            {
                                Header = "ParentSheet",
                                MultiRow = MultiRow.None,
                                Status = DataStatus.Reference,
                                Column = "B",
                                _Paths = "parent.[table]",
                                DefaultValue = "n/a",
                            },
                            new PropertyMapping
                            {
                                Header = "ParentName",
                                MultiRow = MultiRow.None,
                                Status = DataStatus.Reference,
                                Column = "C",
                                _Paths = "parent.Name",
                                DefaultValue = "n/a",
                            },
                            new PropertyMapping
                            {
                                Header = "Value",
                                MultiRow = MultiRow.None,
                                Column = "D",
                                _Paths = "Value",
                                DefaultValue = "n/a",
                            }
                        }
                    }
                },
                PickClassMappings = new List<PickClassMapping>
                {
                    new PickClassMapping{Header = "Categories"}
                }
            };
        }
    }
}
