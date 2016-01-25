using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            mapping.Save(File.Create("..\\..\\SimpleMapping.xml"));
        }

        private ModelMapping GetCobieMapping()
        {
            return ModelMapping.Load(CobieExpress.IO.Properties.Resources.COBieUK2012);
        }

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
                                Paths = "Name",
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
                                Paths = "Name",
                                DefaultValue = "n/a",
                            },
                            new PropertyMapping
                            {
                                Header = "ParentSheet",
                                MultiRow = MultiRow.None,
                                Status = DataStatus.Reference,
                                Column = "B",
                                Paths = "parent.[table]",
                                DefaultValue = "n/a",
                            },
                            new PropertyMapping
                            {
                                Header = "ParentName",
                                MultiRow = MultiRow.None,
                                Status = DataStatus.Reference,
                                Column = "C",
                                Paths = "parent.Name",
                                DefaultValue = "n/a",
                            },
                            new PropertyMapping
                            {
                                Header = "Value",
                                MultiRow = MultiRow.None,
                                Column = "D",
                                Paths = "Value",
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
