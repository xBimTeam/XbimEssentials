using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;
using Xbim.IO.Parser;
using Xbim.IO.Step21;
using Xbim.Ifc4.Interfaces;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\fileWithAbstractClass.ifc")]
        public void ToleratesFileWithAbstractClass()
        {
            // should survive parsing file with abstract class
            // (and use null for offending instances).
            using (var store = IfcStore.Open(@"fileWithAbstractClass.ifc"))
            {
                var inst = store.Instances[1240086];
                Assert.IsNotNull(inst, "Instance should exist.");

                var inst2 = store.Instances[1240084];
                Assert.IsNull(inst2, "Instance should not exist.");

                store.Close();
            }

            // try loading it in esent
            using (var store = IfcStore.Open(@"fileWithAbstractClass.ifc", null, 0.001))
            {
                var inst = store.Instances[1240086];
                Assert.IsNotNull(inst, "Instance should exist.");

                var inst2 = store.Instances[1240084];
                Assert.IsNull(inst2, "Instance should not exist.");

                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\InvalidType.ifc")]
        public void ToleratesFileWithInvalidTypeInList()
        {
            // should survive parsing file with invalid type in list
            using (var store = IfcStore.Open(@"InvalidType.ifc"))
            {
                var inst = store.Instances[582800] as IIfcBuildingStorey;
                Assert.IsNotNull(inst);
                var items = inst.ContainsElements.SelectMany(container => container.RelatedElements);
                Assert.AreEqual(items.Count(), 2, "Should find two items");
            }
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\InvalidType.ifc")]
        public void ToleratesFileWithInvalidEnumString()
        {
            // should survive parsing file with invalid type in list
            using (var store = IfcStore.Open(@"InvalidType.ifc"))
            {
                var role = store.Instances[2] as IIfcActorRole;
                Assert.IsNotNull(role);
                Assert.AreEqual(role.Role, IfcRoleEnum.ARCHITECT);
            }
        }
        
        /// <summary>
        /// This is only provided as a remainder of possible improvements in the tolerance of incorrect files.
        /// </summary>
        [TestMethod]
        [DeploymentItem("TestFiles\\FormallyIllegalFile.ifc")]
        public void AcceptAFormallyIllegalFile()
        {
            // todo: should some notification when the file is malformed be available?
            using (var store = IfcStore.Open("FormallyIllegalFile.ifc"))
            {
                // The file is formally illegal, 
                // see inside the file for comments on details.
                
                // illegal diameter string
                var st = store.Instances[1] as IIfcPropertySingleValue;
                var val = (Ifc4.MeasureResource.IfcDescriptiveMeasure)st.NominalValue;
                var valString = val.Value.ToString();
                Debug.WriteLine(valString);
                if (!val.Value.ToString().Contains("Ø"))
                {
                    throw new Exception("Diameter character misread from file.");
                }

                // illegal double numbers
                var point = store.Instances[2] as IIfcCartesianPoint;
                Assert.IsTrue(double.IsNegativeInfinity(point.X), "coordinate should be negative infinity.");
                Assert.IsTrue(double.IsNaN(point.Y), "coordinate should be NaN.");
                Assert.IsTrue(double.IsPositiveInfinity(point.Z), "coordinate should be positive infinity.");

                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreOpenAndCloseMemoryModelTest()
        {
            using (var store = IfcStore.Open("4walls1floorSite.ifc"))
            {
                var count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void DefaultsToIfcFormatOnUnrecognisedExtension()
        {
            const string fname = "4walls1floorSite.Cobie";
            using (var store = IfcStore.Open("4walls1floorSite.ifc"))
            {
                store.SaveAs(fname);
                store.Close();
            }
            Debug.Assert(File.Exists(fname + ".ifc"));
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreOpenAndCloseEsentModelTest()
        {
            using (var store = IfcStore.Open("4walls1floorSite.ifc",null,0))
            {
                var count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\Issue107.zip")]
        public void Issue107OnMemoryModel()
        {
            // a merged PR on issue 107 makes the memory model more tolerant of bad files.
            // an equivalent test for esent is available
            using (var model = new Xbim.IO.Memory.MemoryModel(new Ifc2x3.EntityFactory()))
            {
                var errCount = model.LoadZip("Issue107.zip");
                Assert.AreEqual(140, errCount);
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\DoubleBackSlashName.ifc")]
        public void DoubleBackSlashName()
        {
            // I've come across a file that has an ifclabel specified as 'TextEndingInEscapedBackslash\\'
            // this causes the parser to break.
            // the problem does not occur if there's any text after the double backslash (i.e. 'TextEndingInEscapedBackslash\\MoreText').
            using (var store = IfcStore.Open("DoubleBackSlashName.ifc"))
            {
                var mat1 = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[417];
                Assert.AreEqual(mat1.Name.ToString(), @"TextWithEscapedBackslash\MoreText", "String containing escaped backslash is not parsed correctly");

                var mat2 = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[418];
                Assert.IsTrue(mat2.Name.ToString().EndsWith(@"\"), "String ending in escaped backslash is not parsed correctly");

                var acc = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[419];
                Assert.IsTrue(acc.Name.ToString().EndsWith("à"), "Text with accented character is not parsed correctly");
                acc = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[420];
                Assert.IsTrue(acc.Name.ToString().EndsWith("à"), "Text with accented character is not parsed correctly");
                acc = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[421];
                Assert.IsTrue(acc.Name.ToString().EndsWith("à"), "Text with accented character is not parsed correctly");

                var beam = (IfcBeam)store.Instances[432];
                Assert.IsNotNull(beam, "element after double backslash is not read correctly");

                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\NewlinesInStrings.ifc")]
        public void CanParseNewlinesInStrings()
        {
            using (var model = new Xbim.IO.Memory.MemoryModel(new Ifc2x3.EntityFactory()))
            {
                var errCount = model.LoadStep21("NewlinesInStrings.ifc");
                Assert.AreEqual(0, errCount);
            }

            using (var model = new Xbim.IO.Esent.EsentModel(new Ifc2x3.EntityFactory()))
            {
                var errCount = model.CreateFrom("NewlinesInStrings.ifc");
                Assert.AreEqual(true, errCount);
            }
        }


        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcOpenIfcZipTest()
        {
            long count;
            //in memory model
            using (var store = IfcStore.Open("4walls1floorSite.ifczip"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count>0, "Should have instances");
                store.Close();
            }
            //esent database
            using (var store = IfcStore.Open("4walls1floorSite.ifczip", null, 0))
            {
                Assert.IsTrue(store.Instances.Count == count, "Should have same number of instances");
                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void ScannerTest()
        {
            using (var strm = File.OpenRead("Badly formed Ifc file.ifc"))
            {
                var scanner = new Scanner(strm);
                int tok;
                do
                {
                    tok = scanner.yylex();
                    var txt = scanner.yytext;
                    Console.WriteLine("Tok={0}, Txt = {1}", Enum.GetName(typeof(Tokens),tok),txt);
                }
                while ( tok!= (int) Tokens.EOF);
            }        
        }


        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void ErrorRecoveryOfParserTest()
        {
            //in memory model
            using (var store = IfcStore.Open("Badly formed Ifc file.ifc"))
            {
                store.Close();
            }
            //esent database
            using (var store = IfcStore.Open("Badly formed Ifc file.ifc", null, 0))
            {
                store.Close();
            }
        }
        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcOpenZipTest()
        {
            long count;
            //in memory model
            using (var store = IfcStore.Open("4walls1floorSite.zip"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have instances");
                store.Close();
            }
            //esent database
            using (var store = IfcStore.Open("4walls1floorSite.zip", null, 0))
            {
                Assert.IsTrue(store.Instances.Count == count, "Should have same number of instances");
                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreSaveAndOpenIfcZipTest()
        {
            long count;
            //create a zip file using esent
            using (var store = IfcStore.Open("4walls1floorSite.ifc", null, 0))
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteA", IfcStorageType.IfcZip | IfcStorageType.Ifc);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteA.ifczip", null, 0))
            {               
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSite.ifc")) //now with memory model
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteB", IfcStorageType.IfcZip | IfcStorageType.Ifc);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteB.ifczip"))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
        }

         [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreSaveAndOpenIfcXmlZipTest()
        {
            long count;
            //create a zip file using esent
            using (var store = IfcStore.Open("4walls1floorSite.ifc", null, 0))
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteX", IfcStorageType.IfcZip | IfcStorageType.IfcXml);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteX.ifczip", null, 0))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSite.ifc")) //now with memory model
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteY", IfcStorageType.IfcZip | IfcStorageType.IfcXml);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteY.ifczip"))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
        }
         [TestMethod]
         [DeploymentItem("TestFiles")]
         public void IfcStoreSaveAndOpenIfcXml4Test()
         {
             int percent = 0;
             ReportProgressDelegate progDelegate = delegate(int percentProgress, object userState)
             {
                 percent = percentProgress;

             };
             long count;
             //create a zip file using esent
             using (var store = IfcStore.Open("SampleHouse4.ifc",null,-1, progDelegate))
             {
                 count = store.Instances.Count;
                 store.SaveAs("SampleHouse4",  IfcStorageType.IfcXml);
                 store.Close();
             }
             using (var store = IfcStore.Open("SampleHouse4.ifcxml", null, -1, progDelegate))
             {
                 Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                 store.Close();
             }
             using (var store = IfcStore.Open("SampleHouse4.ifc",null,-1,progDelegate)) //now with memory model
             {
                 count = store.Instances.Count;
                 store.SaveAs("SampleHouse4",  IfcStorageType.IfcXml);
                 store.Close();
             }
             using (var store = IfcStore.Open("SampleHouse4.ifcxml"))
             {
                 Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                 store.Close();
             }
             Assert.IsTrue(percent == 100);
         }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreCreateStoreTest()
        {
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "XbimTest1",
                ApplicationDevelopersName = "Tester",
                EditorsOrganisationName = "XbimTeam"
            };

            using (var store = IfcStore.Create(credentials, IfcSchemaVersion.Ifc2X3,XbimStoreType.EsentDatabase))
            {
                using (var txn = store.BeginTransaction())
                {
                    var door = store.Instances.New<IfcDoor>();
                    door.Name = "Door 1";
                    txn.Commit();
                }
                store.SaveAs("esent2x3.ifc");
                store.Close();
            }
            using (var store = IfcStore.Create(credentials, IfcSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
            {
                using (var txn = store.BeginTransaction())
                {
                    var door = store.Instances.New<Ifc4.SharedBldgElements.IfcDoor>();
                    door.Name = "Door 1";
                    txn.Commit();
                }
                store.SaveAs("esent4.ifc");
                store.Close();
            }

            using (var store = IfcStore.Create(credentials, IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
            {
                using (var txn = store.BeginTransaction())
                {
                    var door = store.Instances.New<IfcDoor>();
                    door.Name = "Door 1";
                    txn.Commit();
                }
                store.SaveAs("Memory2X3.ifc");
                store.Close();
            }

            using (var store = IfcStore.Create(credentials, IfcSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = store.BeginTransaction())
                {
                    var door = store.Instances.New<Ifc4.SharedBldgElements.IfcDoor>();
                    door.Name = "Door 1";
                    txn.Commit();
                }
                store.SaveAs("Memory4.ifc");
                store.Close();
            }

            string schemaIdentifier;
            var schemaVersion = IfcStore.GetIfcSchemaVersion("Esent2X3.ifc", out schemaIdentifier);
            Assert.IsTrue(schemaVersion == IfcSchemaVersion.Ifc2X3);
            schemaVersion = IfcStore.GetIfcSchemaVersion("Esent4.ifc", out schemaIdentifier);
            Assert.IsTrue(schemaVersion == IfcSchemaVersion.Ifc4);
            schemaVersion = IfcStore.GetIfcSchemaVersion("Memory2X3.ifc", out schemaIdentifier);
            Assert.IsTrue(schemaVersion == IfcSchemaVersion.Ifc2X3);
            schemaVersion = IfcStore.GetIfcSchemaVersion("Memory4.ifc", out schemaIdentifier);
            Assert.IsTrue(schemaVersion == IfcSchemaVersion.Ifc4);
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void ReadIfcHeaderTest()
        {
            string schemaIdentifier;
            var schemaVersion = IfcStore.GetIfcSchemaVersion("SampleHouse4.ifc", out schemaIdentifier);
            Assert.IsTrue(schemaVersion==IfcSchemaVersion.Ifc4);
            schemaVersion = IfcStore.GetIfcSchemaVersion("4walls1floorSite.ifc", out schemaIdentifier);
            Assert.IsTrue(schemaVersion==IfcSchemaVersion.Ifc2X3);

            //first run with a memory model opeing Ifc4 file
            long count;
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "XbimTest1",
                ApplicationDevelopersName = "Tester",
                EditorsOrganisationName = "XbimTeam"
            };
            using (var store = IfcStore.Open("SampleHouse4.ifc"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
            //create xbim files and check them , a size of 1MB will cause Esent to be used       
            using (var store = IfcStore.Open("SampleHouse4.ifc", credentials, 1.0))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have the same number of instances as the memory model");
                store.Close();
            }
            
            //now repeat for Ifc2x3
            using (var store = IfcStore.Open("4walls1floorSite.ifc"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
            //create xbim files and check them , a size of 1MB will cause Esent to be used       
            using (var store = IfcStore.Open("4walls1floorSite.ifc", null, 0.1))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have the same number of instances as the memory model");
                store.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreTransactionTest()
        {
            var doorId = Guid.NewGuid().ToPart21();
            using (var ifcStore = IfcStore.Open("4walls1floorSite.ifc")) //test memory model databases first
            {
                var count = ifcStore.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances"); //read mode is working
                using (var txn = ifcStore.BeginTransaction())
                {
                    var door = ifcStore.Instances.New<IfcDoor>();
                    door.Name = "Door 1";
                    door.GlobalId = doorId;
                    txn.Commit();
                }
                Assert.IsTrue(ifcStore.Instances.Count == count +7); //door plus all the owner history objects
                ifcStore.SaveAs("4walls1floorSiteDoorMM.ifc");
                ifcStore.Close();
            }

            using (var ifcStore = IfcStore.Open("4walls1floorSite.ifc", null, 0)) //test esent databases first
            {
                var count = ifcStore.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances"); //read mode is working
                using (var txn = ifcStore.BeginTransaction())
                {
                    var door = ifcStore.Instances.New<IfcDoor>();
                    door.Name = "Door 1";
                    door.GlobalId = doorId;
                    txn.Commit();
                }
                Assert.IsTrue(ifcStore.Instances.Count == count + 7); //door plus all the owner history objects
                ifcStore.SaveAs("4walls1floorSiteDoorES.ifc");
                ifcStore.Close();
            }


            var diff =
                File.ReadAllLines("4walls1floorSiteDoorES.ifc").Except(File.ReadAllLines("4walls1floorSiteDoorMM.ifc"));
            var enumerable = diff as string[] ?? diff.ToArray();
            Assert.IsTrue(!enumerable.Any() || (enumerable.Count() == 1 && enumerable.First().Contains("FILE_NAME"))); //might be a slight time difference in the timestamp of this line
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreSaveAsXbimTest()
        {
            long originalCount;
            using (var ifcStore = IfcStore.Open("4walls1floorSite.ifc", null, 0)) //test esent databases first
            {
                var  count = originalCount = ifcStore.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances"); //read mode is working               
                ifcStore.SaveAs("4walls1floorSiteDoorES.xbim");
                ifcStore.Close();
            }

            using (var ifcStore = IfcStore.Open("4walls1floorSiteDoorES.xbim")) //test esent databases first
            {
                var count = ifcStore.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances"); //read mode is working                
                ifcStore.SaveAs("4walls1floorSiteDoorES2.Ifc");
                ifcStore.Close();
            }
            using (var ifcStore = IfcStore.Open("4walls1floorSiteDoorES2.ifc")) //test esent databases first
            {
                var count = ifcStore.Instances.Count;
                Assert.IsTrue(count == originalCount, "Should have more than zero instances"); //read mode is working                              
                ifcStore.Close();
            }
        }

        [TestMethod] [DeploymentItem("TestFiles")] 
        public void IfcStoreInitialisationTest()
        {
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "XbimTest1",
                ApplicationDevelopersName = "Tester",
                EditorsOrganisationName = "XbimTeam"
            };

            using (var store = IfcStore.Create(credentials, IfcSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
            {
                using (var txn = store.BeginTransaction())
                {
                    var project = store.Instances.New<Ifc4.Kernel.IfcProject>();
                    project.Initialize(ProjectUnits.SIUnitsUK);
                    txn.Commit();
                }
                store.SaveAs("esent4.ifc");
                store.Close();
            }

            using (var store = IfcStore.Create(credentials, IfcSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
            {
                using (var txn = store.BeginTransaction())
                {
                    var project = store.Instances.New<Ifc2x3.Kernel.IfcProject>();
                    project.Initialize(ProjectUnits.SIUnitsUK);
                    txn.Commit();
                }
                store.SaveAs("esent2x3.ifc");
                store.Close();
            }
        }

        [TestMethod]
        public void EncodeBackslash()
        {
            const string path = "C:\\Data\\Martin\\document.txt";
            const string encodedPath = "C:\\\\Data\\\\Martin\\\\document.txt";
            const string test = "BackslashEncoding.ifc";
            using (var store = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
            {
                using (var txn = store.BeginTransaction())
                {
                    store.Instances.New<Ifc2x3.ExternalReferenceResource.IfcDocumentInformation>(i => i.Description = path);
                    txn.Commit();
                }
                store.SaveAs(test);
                store.Close();
            }

            var file = File.ReadAllText(test);
            Assert.IsTrue(file.Contains(encodedPath));

            //replace with inescaped backslashes. This is illegal Step21 but we should process it anyway.
            file = file.Replace(encodedPath, path);
            File.WriteAllText(test, file);

            using (var model = IfcStore.Open(test))
            {
                var info = model.Instances.FirstOrDefault<Ifc2x3.ExternalReferenceResource.IfcDocumentInformation>();
                Assert.IsTrue(info.Description == path);
            }
        }

    }
}
