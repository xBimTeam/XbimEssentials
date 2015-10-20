using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CobieExpress;
using Xbim.Common;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class CobieTests
    {
        

        [TestMethod]
        public void SerializeDeserialize()
        {
            var model = CreateTestModel();
            
            model.Save("RandomModel.cobie");

            var model2 = new MemoryModel<EntityFactory>();
            model2.Open("RandomModel.cobie");

            Assert.AreEqual(model2.Instances.Count, model2.Instances.Count);
            Assert.AreEqual(model2.Instances.OfType<CobieAttribute>().Count(), model2.Instances.OfType<CobieAttribute>().Count());
            Assert.AreEqual(model2.Instances.OfType<CobieComponent>().Count(), model2.Instances.OfType<CobieComponent>().Count());

            //because save operation is deterministic both files should match
            var data1 = new StringWriter();
            model2.Save(data1);

            var data2 = new StringWriter();
            model2.Save(data2);

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
        public void EsentDatabaseTest()
        {
            const string file = "SampleForEsent.stp";
            var model = CreateTestModel();
            model.Save(file);

            var db = new EsentModel(new EntityFactory());
            db.CreateFrom(file, null, null, true);

            var spaces = db.Instances.OfType<CobieSpace>();
            Assert.IsTrue(spaces.Any());
        }

        private MemoryModel<EntityFactory> CreateTestModel()
        {
            var model = new MemoryModel<EntityFactory>();
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
                model.Instances.New<CobieSpace>(s =>
                {
                    s.Name = sName;
                    s.Floor = _rHeroNames.IndexOf(name) < 5 ? floorA : floorB;
                    GenerateAttributes(s, 10);
                    GenerateComponents(s, 15);
                });
            }
        }

        private int _typeCounter = 100;

        private void GenerateComponents(CobieSpace space, int count)
        {
            var model = space.Model;
            var facility = space.Floor.Facility;
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
                model.Instances.New<CobieComponent>(c =>
                {
                    c.Type = type;
                    c.Space = space;
                    c.Name = _rThings[i];
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
