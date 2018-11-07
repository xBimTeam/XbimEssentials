using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CobieExpress;
using Xbim.CobieExpress.IO;
using Xbim.Common;
using Xbim.IO.TableStore;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class CobieTests
    {
        //[TestMethod]
        //[DeploymentItem("TestFiles/LakesideRestaurant.cobieZip")]
        //public void CobieXmlSerialization()
        //{
        //    const string xmlFile = "..\\..\\LakesideRestaurantCobie.xml";
        //    var model = new IO.Memory.MemoryModel(ef);
        //    model.LoadZip("LakesideRestaurant.cobieZip");

        //    var writer = new XbimXmlWriter4(configuration.COBieExpress, XbimXmlSettings.COBieExpress);
        //    using (var xmlWriter = XmlWriter.Create(xmlFile, new XmlWriterSettings { IndentChars = "\t", Indent = true }))
        //    {
        //        writer.Write(model, xmlWriter, model.Instances.OfType<CobieFacility>().Concat(model.Instances));
        //        xmlWriter.Close();
        //    }

        //    var xmlModel = new IO.Memory.MemoryModel(ef);
        //    xmlModel.LoadXml(xmlFile);

        //    Assert.AreEqual(model.Instances.Count, xmlModel.Instances.Count);

        //}

        //[TestMethod]
        //public void CobieComparison()
        //{
        //    var file = @"c:\CODE\XbimGit\XbimExchange\TestResults\converted.cobie";
        //    using (var model = CobieModel.OpenStep21(file))
        //    {
        //        string report;
        //        model.ExportToTable(@"c:\CODE\XbimGit\XbimExchange\TestResults\converted.xlsx", out report);
        //    }
        //}

        private static readonly IEntityFactory ef = new EntityFactoryCobieExpress();

        [TestMethod]
        public void SerializeDeserialize()
        {
            var model = CreateTestModel();
            using (var fileStream = new StreamWriter("RandomModel.cobie"))
            {
                model.SaveAsStep21(fileStream);
            }

            using (var fileStream = File.Create("RandomModel.cobieZip"))
            {
                model.SaveAsStep21Zip(fileStream);
            }

            var stepModel = new IO.Memory.MemoryModel(ef);
            stepModel.LoadStep21("RandomModel.cobie");

            var zipModel = new IO.Memory.MemoryModel(ef);
            zipModel.LoadZip(File.OpenRead("RandomModel.cobieZip"));

            Assert.AreEqual(model.Instances.Count, stepModel.Instances.Count);
            Assert.AreEqual(model.Instances.OfType<CobieAttribute>().Count(), stepModel.Instances.OfType<CobieAttribute>().Count());
            Assert.AreEqual(model.Instances.OfType<CobieComponent>().Count(), stepModel.Instances.OfType<CobieComponent>().Count());

            Assert.AreEqual(model.Instances.Count, zipModel.Instances.Count);
            Assert.AreEqual(model.Instances.OfType<CobieAttribute>().Count(), zipModel.Instances.OfType<CobieAttribute>().Count());
            Assert.AreEqual(model.Instances.OfType<CobieComponent>().Count(), zipModel.Instances.OfType<CobieComponent>().Count());

            //because save operation is deterministic both files should match
            var data1 = new StringWriter();
            model.SaveAsStep21(data1);

            var data2 = new StringWriter();
            stepModel.SaveAsStep21(data2);

            var str1 = data1.ToString();
            var str2 = data2.ToString();

            Assert.AreEqual(str1.Length, str2.Length);
            Assert.AreEqual(str1, str2);
        }

        [TestMethod]
        public void Deletion()
        {
            var model = CreateTestModel();
            using (model.BeginTransaction("Deletion"))
            {
                var attribute = model.Instances.FirstOrDefault<CobieAttribute>();
                Assert.IsNotNull(attribute);
                var entity = model.Instances.FirstOrDefault<CobieAsset>(a => a.Attributes.Contains(attribute));
                Assert.IsNotNull(entity);
                var space = model.Instances.FirstOrDefault<CobieSpace>();
                var floor = space.Floor;
                Assert.IsNotNull(floor);

                
                //object should be removed from the collection
                model.Delete(attribute);
                Assert.IsFalse(entity.Attributes.Contains(attribute));

                //property should be nullified
                model.Delete(floor);
                Assert.IsNull(space.Floor);
            }
        }

        [TestMethod]
        public void AttributeIndexGetSet()
        {
            using (var model = new IO.Memory.MemoryModel(ef))
            {
                using (var txn = model.BeginTransaction("Creation"))
                {
                    var component = model.Instances.New<CobieComponent>(c => c.Name = "Boiler");
                    var bCode = component["BarCode"];
                    Assert.IsNull(bCode);

                    const int bc = 15789123;
                    component["BarCode"] = (IntegerValue)bc;
                    Assert.IsNotNull(component.Attributes.FirstOrDefault(a => a.Name == "BarCode"));

                    bCode = component["BarCode"];
                    Assert.IsNotNull(bCode);
                    Assert.IsTrue(bCode.Equals((IntegerValue)bc));

                    component["BarCode"] = (IntegerValue)5;
                    Assert.IsTrue(component["BarCode"].Equals((IntegerValue)5));

                    const string myPropName = "My property set.My property";
                    var myProp = component[myPropName];
                    Assert.IsNull(myProp);
                    component[myPropName] = (StringValue) "Testing value";
                    var myAttr =
                        component.Attributes.FirstOrDefault(
                            a => a.Name == "My property" && a.PropertySet.Name == "My property set");
                    Assert.IsNotNull(myAttr);

                    myProp = component[myPropName];
                    Assert.IsNotNull(myProp);
                    Assert.IsTrue(myProp.Equals((StringValue)"Testing value"));

                    txn.Commit();
                }
            }

        }

        [TestMethod]
        public void EsentDatabaseTest()
        {
            const string file = "SampleForEsent.stp";
            using (var model = CreateTestModel())
            {
                using (var fileStream = new StreamWriter(file))
                {
                    model.SaveAsStep21(fileStream);
                }
            }
            using (var db = new IO.Esent.EsentModel(ef))
            {
                db.CreateFrom(file, null, null, true);

                var spaces = db.Instances.OfType<CobieSpace>();
                Assert.IsTrue(spaces.Any());
            }
        }

        [TestMethod]
        public void ExcelRoundTrip()
        {
            using (var model = new CobieModel(CreateTestModel()))
            {
                string excelExported = "exported.xlsx";
                string report;
                model.ExportToTable(excelExported, out report);
                Assert.IsTrue(File.Exists(excelExported));
                Assert.IsTrue(string.IsNullOrWhiteSpace(report));

                using (var imported = CobieModel.ImportFromTable(excelExported, out report))
                {
                    //Assert.IsTrue(string.IsNullOrWhiteSpace(report));

                    CompareTrees(model, imported);

                    //CompareTrees(model, model);
                }
            }
        }

        private void CompareTrees(IModel left, IModel right)
        {
            var facilityLeft = left.Instances.FirstOrDefault<CobieFacility>();
            var facilityRight = right.Instances.FirstOrDefault<CobieFacility>();

            CompareTrees(facilityLeft, facilityRight);
        }

        private void CompareTrees(CobieFacility facilityLeft, CobieFacility facilityRight)
        {
            Assert.AreEqual(facilityLeft.ExternalId, facilityRight.ExternalId);
            Assert.AreEqual(facilityLeft.AltExternalId, facilityRight.AltExternalId);
            Assert.AreEqual(facilityLeft.Name, facilityRight.Name);

            Assert.AreEqual(facilityLeft.Floors.Count(), facilityRight.Floors.Count(), "Floor count mismatch");

            if (facilityLeft.Floors.Count() == facilityRight.Floors.Count())
            {
                for (int i=0; i<facilityLeft.Floors.Count(); ++i)
                {
                    var floorLeft = facilityLeft.Floors.ElementAt(i);
                    var floorRight = facilityLeft.Floors.ElementAt(i);

                    CompareTrees(floorLeft, floorRight);
                }
            }
        }

        private void CompareTrees(CobieFloor floorLeft, CobieFloor floorRight)
        {
            Assert.AreEqual(floorLeft.ExternalId, floorRight.ExternalId);
            Assert.AreEqual(floorLeft.AltExternalId, floorRight.AltExternalId);
            Assert.AreEqual(floorLeft.Name, floorRight.Name);

            Assert.AreEqual(floorLeft.Spaces.Count(), floorRight.Spaces.Count(), "Space count mismatch");

            if (floorLeft.Spaces.Count() == floorRight.Spaces.Count())
            {
                for (int i = 0; i < floorLeft.Spaces.Count(); ++i)
                {
                    var spaceLeft = floorLeft.Spaces.ElementAt(i);
                    var spaceRight = floorLeft.Spaces.ElementAt(i);

                    CompareTrees(spaceLeft, spaceRight);
                }
            }
        }

        private void CompareTrees(CobieSpace spaceLeft, CobieSpace spaceRight)
        {
            Assert.AreEqual(spaceLeft.ExternalId, spaceRight.ExternalId);
            Assert.AreEqual(spaceLeft.AltExternalId, spaceRight.AltExternalId);
            Assert.AreEqual(spaceLeft.Name, spaceRight.Name);

            Assert.AreEqual(spaceLeft.Components.Count(), spaceRight.Components.Count(), "Component count missmatch");
        }

        private IO.Memory.MemoryModel CreateTestModel()
        {
            var model = new IO.Memory.MemoryModel(ef);
            using (var txn = model.BeginTransaction("Model creation"))
            {
                var facility = model.Instances.New<CobieFacility>(f =>
                {
                    f.Name = "Testing facility";
                    f.Attributes.Add(model.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Attribute A";
                        a.Value = new BooleanValue(true);
                    }));
                    f.Attributes.Add(model.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Attribute B";
                        a.Value = new StringValue("Example value");
                    }));
                    f.Attributes.Add(model.Instances.New<CobieAttribute>(a =>
                    {
                        a.Name = "Attribute C";
                        a.Value = (DateTimeValue)DateTime.Now;
                    }));

                    f.Created = model.Instances.New<CobieCreatedInfo>(ci =>
                    {
                        ci.CreatedOn = DateTime.Now;
                        ci.CreatedBy = model.Instances.New<CobieContact>(c =>
                        {
                            c.FamilyName = "Cerny";
                            c.GivenName = "Martin";
                            c.Email = "martin.cerny@northumbria.ac.uk";
                        });
                    });

                    GenerateAttributes(f, 8);
                });

                AddContacts(model);
                AddFloorsAndSpaces(facility);
                txn.Commit();
            }
            
            return model;
        }

        private void AddContacts(IModel model)
        {
            var created = model.Instances.FirstOrDefault<CobieCreatedInfo>();
            foreach (var name in _rPersonNames)
            {
                var parts = name.Split(' ');
                var gName = parts[0];
                var fName = parts[1];
                var email = string.Format("{0}.{1}@cobie.com", gName, fName).ToLower();
                model.Instances.New<CobieContact>(c =>
                {
                    c.FamilyName = fName;
                    c.GivenName = gName;
                    c.Email = email;
                    c.Created = created;
                });
            }
        }

        private void AddFloorsAndSpaces(CobieFacility facility)
        {
            var model = facility.Model;
            var floorA = model.Instances.New<CobieFloor>(f =>
            {
                f.Name = "Base Floor";
                f.Facility = facility;
                f.Height = 4500;
                f.Elevation = 0;
                f.Created = facility.Created;
                GenerateAttributes(f, 23);
            });

            var floorB = model.Instances.New<CobieFloor>(f =>
            {
                f.Name = "First Floor";
                f.Facility = facility;
                f.Height = 3500;
                f.Elevation = 4700;
                f.Created = facility.Created;
                GenerateAttributes(f, 12);
            });

            foreach (var name in _rHeroNames)
            {
                var sName = name + "'s Hall";
                var name1 = name;
                model.Instances.New<CobieSpace>(s =>
                {
                    s.Name = sName;
                    s.Floor = _rHeroNames.IndexOf(name1) < 5 ? floorA : floorB;
                    GenerateAttributes(s, 10);
                    GenerateComponents(s, 15);
                });
            }
        }

        private int _typeCounter = 100;

        private void GenerateComponents(CobieSpace space, int count)
        {
            var model = space.Model;
            var rnd = new Random(79);
            var type = model.Instances.New<CobieType>(t =>
            {
                t.Name = "Type " + _typeCounter++;
                t.CodePerformance = "Great";
                t.AccessibilityPerformance = "Easy";
                t.Color = "Uncertain";
                t.ExpectedLife = 100;
                var cIndex = (int) (rnd.NextDouble()*10) -1;
                t.Manufacturer = model.Instances.OfType<CobieContact>().ToList()[cIndex];
            });
            for (var i = 0; i < count; i++)
            {
                var i1 = i;
                model.Instances.New<CobieComponent>(c =>
                {
                    c.Type = type;
                    c.Spaces.Add(space);
                    c.Name = _rThings[i1];
                    GenerateAttributes(c, 8);
                });
            }
        }

        private void GenerateAttributes(CobieAsset asset, int count)
        {
            var rnd = new Random(235);
            var model = asset.Model;
            for (var i = 0; i < count; i++)
            {
                asset.Attributes.Add(model.Instances.New<CobieAttribute>(a =>
                {
                    var index = (int) (rnd.NextDouble()*100);
                    a.Name = _rThings[index];
                    index = (int)(rnd.NextDouble() * 100);
                    if(index < _rHeroNames.Count - 1 ) 
                        a.Value = (StringValue) _rHeroNames[index];
                    else
                        a.Value = (IntegerValue)rnd.Next();
                }));
            }
        }

        private readonly List<string> _rPersonNames = new List<string> { "Reda Ligon", "Gwyn Franzone", "Mindy Ruder", "Eulah Beddingfield", "Zoila Skoglund", "Adrianna Schlatter", "Bonny Montesdeoca", "Seth Kroh", "Shelba Gehrke", "Norbert Brigham", "Jessi Roberie", "Joycelyn Newhart", "Sarita Triana", "Sanjuana Roloff", "Taryn Alred", "Katelynn Petty", "Alisia Hardrick", "Orval Rees", "Austin Thibert", "Brad Alameda", "Neva Sang", "Marilou Pitkin", "Thomasina Rabon", "Sheryl Bohannan", "Nathanial Cosper", "Kermit Rester", "Angeline Palau", "Lenny Isaac", "Veda Dyar", "Daniel Greenhill" };

        private readonly List<string> _rHeroNames = new List<string> { "Philip Poirrier", "Lamar Lofgren", "Enoch Entrekin", "Mark Mcginn", "Landon Learned", "Patrick Peralta", "Dominique Doak", "Armand Abeita", "Keneth Kelsey", "Samual Strite" }; 
    
        private readonly List<string> _rThings = new List<string>{"cinder block", "fake flowers", "conditioner", "video games", "greeting card", "chapter book", "sharpie", "book", "photo album", "brocolli", "face wash", "glass", "clock", "pants", "candy wrapper", "stop sign", "money", "CD", "packing peanuts", "purse", "street lights", "shoes", "ipod", "scotch tape", "pen", "puddle", "clamp", "rug", "window", "tomato", "deodorant", "magnet", "spring", "remote", "television", "playing card", "tooth picks", "toilet", "sand paper", "spoon", "bookmark", "piano", "clothes", "canvas", "cell phone", "beef", "wallet", "bottle", "rusty nail", "box", "eye liner", "phone", "soda can", "tire swing", "knife", "paper", "sun glasses", "truck", "apple", "hair brush", "house", "screw", "chocolate", "plastic fork", "sandal", "helmet", "computer", "sketch pad", "food", "picture frame", "white out", "floor", "perfume", "speakers", "bread", "fridge", "lamp shade", "newspaper", "monitor", "washing machine", "charger", "blouse", "mop", "earser", "pool stick", "bag", "seat belt", "slipper", "blanket", "bottle cap", "pencil", "headphones", "lip gloss", "stockings", "soap", "key chain", "candle", "pillow", "car", "grid paper"};
    }
}
