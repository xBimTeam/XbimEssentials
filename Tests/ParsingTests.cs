using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;
using Xbim.IO.Parser;
using Xbim.IO.Step21;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ParsingTests
    {
        private static readonly IEntityFactory ef4 = new Ifc4.EntityFactoryIfc4();
        private static readonly IEntityFactory ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();

        [TestMethod]
        public void ToleratesFileWithAbstractClass()
        {
            // should survive parsing file with abstract class
            // (and use null for offending instances).
            using (var store = IfcStore.Open(@"TestFiles\fileWithAbstractClass.ifc"))
            {
                var inst = store.Instances[1240086];
                Assert.IsNotNull(inst, "Instance should exist.");

                var inst2 = store.Instances[1240084];
                Assert.IsNull(inst2, "Instance should not exist.");

                store.Close();
            }

            // try loading it in esent
            using (var store = IfcStore.Open(@"TestFiles\fileWithAbstractClass.ifc", null, 0.001))
            {
                var inst = store.Instances[1240086];
                Assert.IsNotNull(inst, "Instance should exist.");

                var inst2 = store.Instances[1240084];
                Assert.IsNull(inst2, "Instance should not exist.");

                store.Close();
            }
        }

        [TestMethod]
        public void ToleratesListOfListsOfInts()
        {
            // should survive parsing file with list of lists of ints instead of list of ints
            using (var store = IfcStore.Open(@"TestFiles\InvalidTriangulatedFaceSet.ifc"))
            {
                var faceset = store.Instances.First() as IIfcTriangulatedFaceSet;
                Assert.IsNotNull(faceset);
                var pnIndex = faceset.PnIndex;
                Assert.IsNotNull(pnIndex);
                Assert.IsTrue(pnIndex.Count > 0);
            }

            // make sure Esent will work in the same way
            using (var store = IfcStore.Open(@"TestFiles\InvalidTriangulatedFaceSet.ifc", null, 0))
            {
                var faceset = store.Instances.First() as IIfcTriangulatedFaceSet;
                Assert.IsNotNull(faceset);
                var pnIndex = faceset.PnIndex;
                Assert.IsNotNull(pnIndex);
                Assert.IsTrue(pnIndex.Count > 0);
            }
        }

        [TestMethod]
        public void ToleratesStringInsteadOfEnum()
        {
            // should survive parsing file with enum value encoded as a string
            using (var store = IfcStore.Open(@"TestFiles\InvalidMonetaryUnit.ifc"))
            {
                var mUnit = store.Instances.First() as Ifc2x3.MeasureResource.IfcMonetaryUnit;
                Assert.IsNotNull(mUnit);
                Assert.AreEqual(Ifc2x3.MeasureResource.IfcCurrencyEnum.AUD, mUnit.Currency);
            }

            // make sure Esent will work in the same way
            using (var store = IfcStore.Open(@"TestFiles\InvalidMonetaryUnit.ifc", null, 0))
            {
                var mUnit = store.Instances.First() as Ifc2x3.MeasureResource.IfcMonetaryUnit;
                Assert.IsNotNull(mUnit);
                Assert.AreEqual(Ifc2x3.MeasureResource.IfcCurrencyEnum.AUD, mUnit.Currency);
            }
        }


        [TestMethod]
        public void ToleratesFileWithInvalidTypeInList()
        {
            // should survive parsing file with invalid type in list
            using (var store = IfcStore.Open(@"TestFiles\InvalidType.ifc"))
            {
                var inst = store.Instances[582800] as IIfcBuildingStorey;
                Assert.IsNotNull(inst);
                var items = inst.ContainsElements.SelectMany(container => container.RelatedElements);
                Assert.AreEqual(items.Count(), 2, "Should find two items");
            }
        }

        [TestMethod]
        public void ToleratesFileWithInvalidEnumString()
        {
            // should survive parsing file with invalid type in list
            using (var store = IfcStore.Open(@"TestFiles\InvalidType.ifc"))
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
        public void AcceptAFormallyIllegalFile()
        {
            // todo: should some notification when the file is malformed be available?
            using (var store = IfcStore.Open("TestFiles\\FormallyIllegalFile.ifc"))
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
        public void ToleratesEmptyMultibyteStringsTest()
        {
            // I have stumbled across a file containing empty multibyte string sequences '\X2\\X0\'.
            using (IfcStore store = IfcStore.Open(@"TestFiles\EmptyMultibyteString.ifc")) {
                IIfcProject project = store.Instances.OfType<IIfcProject>().SingleOrDefault();
                Assert.AreEqual("Test Test Test", (string)project.Name);
            }
        }
        
        [TestMethod]
        public void IfcStoreOpenAndCloseMemoryModelTest()
        {
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc"))
            {
                var count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
        }

        [TestMethod]
        public void DefaultsToIfcFormatOnUnrecognisedExtension()
        {
            const string fname = "4walls1floorSite.Cobie";
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc"))
            {
                store.SaveAs(fname);
                store.Close();
            }
            Debug.Assert(File.Exists(fname + ".ifc"));
        }

        [TestMethod]
        public void IfcStoreOpenAndCloseEsentModelTest()
        {
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null,0))
            {
                var count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
        }

        [TestMethod]
        public void Issue107OnMemoryModel()
        {
            // a merged PR on issue 107 makes the memory model more tolerant of bad files.
            // an equivalent test for esent is available
            using (var model = new IO.Memory.MemoryModel(ef2x3))
            {
                model.LoadZip("TestFiles\\Issue107.zip");
                //Assert.IsTrue(errCount <= 120);
            }
        }

        [TestMethod]
        public void LowerCaseExponentTest()
        {
            using (var model = new IO.Memory.MemoryModel(ef2x3))
            {
                var errs = model.LoadStep21("TestFiles\\RealWithExponent.ifc");
                Assert.IsTrue(errs == 0);
                Assert.IsTrue(model.Instances.Count == 1);
            }
        }

        [TestMethod]
        public void DoubleBackSlashName()
        {
            // I've come across a file that has an ifclabel specified as 'TextEndingInEscapedBackslash\\'
            // this causes the parser to break.
            // the problem does not occur if there's any text after the double backslash (i.e. 'TextEndingInEscapedBackslash\\MoreText').
            using (var store = IfcStore.Open("TestFiles\\DoubleBackSlashName.ifc"))
            {
                var mat1 = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[417];
                Assert.AreEqual(mat1.Name.ToString(), @"TextWithEscapedBackslash\MoreText", "String containing escaped backslash is not parsed correctly");

                var mat2 = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[418];
                Assert.IsTrue(mat2.Name.ToString().EndsWith(@"\"), "String ending in escaped backslash is not parsed correctly");

                var acc = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[419];
                Assert.IsTrue(acc.Name.ToString().EndsWith("à"), "String with \\X escaping is not parsed correctly");
                acc = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[420];
                Assert.IsTrue(acc.Name.ToString().EndsWith("à"), "String with \\X2 escaping is not parsed correctly");
                acc = (Ifc2x3.MaterialResource.IfcMaterial)store.Instances[421];
                Assert.IsTrue(acc.Name.ToString().EndsWith("à"), "String with \\X4 escaping is not parsed correctly");

                var beam = (IfcBeam)store.Instances[432];
                Assert.IsNotNull(beam, "element after escaped strings is not read correctly");

                store.Close();
            }
        }

        [TestMethod]
        public void CanParseNewlinesInStrings()
        {
            using (var model = new Xbim.IO.Memory.MemoryModel( ef2x3))
            {
                var errCount = model.LoadStep21("TestFiles\\NewlinesInStrings.ifc");
                Assert.AreEqual(0, errCount);
            }

            using (var model = new Xbim.IO.Esent.EsentModel( ef2x3))
            {
                var errCount = model.CreateFrom("TestFiles\\NewlinesInStrings.ifc");
                Assert.AreEqual(true, errCount);
            }
        }

        [TestMethod]
        public void CanParseSpecicalSolidusEscape()
        {
            using (var model = new Xbim.IO.Memory.MemoryModel(ef2x3))
            {
                var errCount = model.LoadStep21("TestFiles\\SpecicalSolidusEscape.ifc");
                Assert.AreEqual(0, errCount);
            }
        }


        [TestMethod]
        public void IfcOpenIfcZipTest()
        {
            long count;
            //in memory model
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifczip"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count>0, "Should have instances");
                store.Close();
            }
            //esent database
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifczip", null, 0))
            {
                Assert.IsTrue(store.Instances.Count == count, "Should have same number of instances");
                store.Close();
            }
        }

        [TestMethod]
        public void ScannerTest()
        {
            using (var strm = File.OpenRead("TestFiles\\Badly formed Ifc file.ifc"))
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
        public void ErrorRecoveryOfParserTest()
        {
            //in memory model
            using (var store = IfcStore.Open("TestFiles\\Badly formed Ifc file.ifc"))
            {
                store.Close();
            }
            //esent database
            using (var store = IfcStore.Open("TestFiles\\Badly formed Ifc file.ifc", null, 0))
            {
                store.Close();
            }
        }
        [TestMethod]
        public void IfcOpenZipTest()
        {
            long count;
            //in memory model
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.zip"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have instances");
                store.Close();
            }
            //esent database
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.zip", null, 0))
            {
                Assert.IsTrue(store.Instances.Count == count, "Should have same number of instances");
                store.Close();
            }
        }

        [TestMethod]
        public void IfcStoreSaveAndOpenIfcZipTest()
        {
            long count;
            //create a zip file using esent
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0))
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteA", StorageType.IfcZip | StorageType.Ifc);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteA.ifczip", null, 0))
            {               
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc")) //now with memory model
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteB", StorageType.IfcZip | StorageType.Ifc);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteB.ifczip"))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
        }

         [TestMethod]
        public void IfcStoreSaveAndOpenIfcXmlZipTest()
        {
            long count;
            //create a zip file using esent
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0))
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteX", StorageType.IfcZip | StorageType.IfcXml);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteX.ifczip", null, 0))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc")) //now with memory model
            {
                count = store.Instances.Count;
                store.SaveAs("4walls1floorSiteY", StorageType.IfcZip | StorageType.IfcXml);
                store.Close();
            }
            using (var store = IfcStore.Open("4walls1floorSiteY.ifczip"))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void IfcStoreSaveAndOpenIfcXml4Test()
        {
            int percent = 0;
            ReportProgressDelegate progDelegate = delegate(int percentProgress, object userState)
            {
                percent = percentProgress;

            };
            long count;
            
            using (var store = IfcStore.Open("TestFiles\\SampleHouse4.ifc", null,-1, progDelegate))
            {
                count = store.Instances.Count;
                store.SaveAs("SampleHouse4",  StorageType.IfcXml);
                store.Close();
            }
            using (var store = IfcStore.Open("SampleHouse4.ifcxml", null, -1, progDelegate))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have same number of instances");
                store.Close();
            }
            //now with memory model
            using (var store = IfcStore.Open("TestFiles\\SampleHouse4.ifc", null,-1,progDelegate)) 
            {
                count = store.Instances.Count;
                store.SaveAs("SampleHouse4",  StorageType.IfcXml);
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
        public void IfcStoreCreateStoreTest()
        {
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "XbimTest1",
                ApplicationDevelopersName = "Tester",
                EditorsOrganisationName = "XbimTeam"
            };

            using (var store = IfcStore.Create(credentials, XbimSchemaVersion.Ifc2X3,XbimStoreType.EsentDatabase))
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
            using (var store = IfcStore.Create(credentials, XbimSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
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

            using (var store = IfcStore.Create(credentials, XbimSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
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

            using (var store = IfcStore.Create(credentials, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
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

            var modelStore = new HeuristicModelProvider();
            var schemaVersion = modelStore.GetXbimSchemaVersion("Esent2X3.ifc");
            Assert.IsTrue(schemaVersion == XbimSchemaVersion.Ifc2X3);
            schemaVersion = modelStore.GetXbimSchemaVersion("Esent4.ifc");
            Assert.IsTrue(schemaVersion == XbimSchemaVersion.Ifc4);
            schemaVersion = modelStore.GetXbimSchemaVersion("Memory2X3.ifc");
            Assert.IsTrue(schemaVersion == XbimSchemaVersion.Ifc2X3);
            schemaVersion = modelStore.GetXbimSchemaVersion("Memory4.ifc");
            Assert.IsTrue(schemaVersion == XbimSchemaVersion.Ifc4);
        }

        

        [TestMethod]
        public void ReadIfcHeaderTest()
        {
            var modelStore = new HeuristicModelProvider();

            var schemaVersion = modelStore.GetXbimSchemaVersion("TestFiles\\SampleHouse4.ifc");
            Assert.IsTrue(schemaVersion==XbimSchemaVersion.Ifc4);
            schemaVersion = modelStore.GetXbimSchemaVersion("TestFiles\\4walls1floorSite.ifc");
            Assert.IsTrue(schemaVersion==XbimSchemaVersion.Ifc2X3);

            //first run with a memory model opeing Ifc4 file
            long count;
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "XbimTest1",
                ApplicationDevelopersName = "Tester",
                EditorsOrganisationName = "XbimTeam"
            };
            using (var store = IfcStore.Open("TestFiles\\SampleHouse4.ifc"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
            //create esent files and check them , a size of 1MB will cause Esent to be used       
            using (var store = IfcStore.Open("TestFiles\\SampleHouse4.ifc", credentials, 1.0))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have the same number of instances as the memory model");
                store.Close();
            }
            
            //now repeat for Ifc2x3
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc"))
            {
                count = store.Instances.Count;
                Assert.IsTrue(count > 0, "Should have more than zero instances");
                store.Close();
            }
            //create xbim files and check them , a size of 1MB will cause Esent to be used       
            using (var store = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0.1))
            {
                Assert.IsTrue(count == store.Instances.Count, "Should have the same number of instances as the memory model");
                store.Close();
            }
        }

        [TestMethod]
        public void IfcStoreTransactionTest()
        {
            var memoryModelFile = "4walls1floorSiteDoorMM.ifc";
            var esentModelFile = "4walls1floorSiteDoorES.ifc";

            var doorId = Guid.NewGuid().ToPart21();
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc")) //test memory model first
            {
                var count = ifcStore.Instances.Count;
                var origLabels = ifcStore.Instances.Select(x => x.EntityLabel).ToList();
                Assert.IsTrue(count > 0, "Should have more than zero instances"); //read mode is working
                using (var txn = ifcStore.BeginTransaction())
                {
                    var door = ifcStore.Instances.New<IfcDoor>();
                    door.Name = "Door 1";
                    door.GlobalId = doorId;
                    txn.Commit();
                }
                var diffCount = ifcStore.Instances.Count - count;
                var NewLabels = ifcStore.Instances.Select(x => x.EntityLabel).ToList();
                var newLabelList = NewLabels.Except(origLabels);
                foreach (var item in newLabelList)
                {
                    Debug.WriteLine(ifcStore.Instances[item].ToString());
                }

                Assert.AreEqual(6, diffCount, "Unexpected number of elements created for new door."); //door plus all the owner history objects

                // it seems right that there should be a few more items:
                //Xbim.Ifc2x3.SharedBldgElements.IfcDoor
                //Xbim.Ifc2x3.UtilityResource.IfcOwnerHistory
                //Xbim.Ifc2x3.ActorResource.IfcPerson
                //Xbim.Ifc2x3.ActorResource.IfcOrganization
                //Xbim.Ifc2x3.ActorResource.IfcPersonAndOrganization
                //Xbim.Ifc2x3.UtilityResource.IfcApplication

                ifcStore.SaveAs(memoryModelFile);
                ifcStore.Close();
            }

            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0)) //test esent databases
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
                Assert.IsTrue(ifcStore.Instances.Count == count + 6); //door plus all the owner history objects
                ifcStore.SaveAs(esentModelFile);
                ifcStore.Close();
            }
            FilesAreEqual(memoryModelFile, esentModelFile);
            FilesAreEqual(esentModelFile, memoryModelFile);
        }

        private static void FilesAreEqual(string memoryModelFile, string esentModelFile)
        {
            var diffArray = File.ReadAllLines(esentModelFile).Except(File.ReadAllLines(memoryModelFile));
            foreach (var line in diffArray)
            {
                if (line.Contains("FILE_NAME")) // there might be a difference in the timestamp of this line
                    continue;
                if (line.Contains("IFCOWNERHISTORY")) // IfcOwnerHistory is also timestamped
                    continue;
                throw new Exception($"Diff in line '{line}' is not expected.");
            }
        }

        [TestMethod]
        public void IfcStoreSaveAsXbimTest()
        {
            long originalCount;
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0)) //test esent databases first
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

        [TestMethod]
        public void FileBasedStore_Should_TidyUp_OnClose()
        {
            string xbimFile;
            // Load with Esent/File-based store - creates a temp xbim file in %TEMP%
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0))
            {
                xbimFile = ifcStore.Location;
                Assert.IsTrue(File.Exists(xbimFile));
                ifcStore.Close();
            }

            Assert.IsFalse(File.Exists(xbimFile));
        }

        [TestMethod]
        public void Issue206_FileBasedStore_Should_TidyUp_JFM_on_Close()
        {
            string xbimFile;
            string flushmapFile;
            // Load with Esent/File-based store - creates a temp xbim file in %TEMP%
            using (var ifcStore = IfcStore.Open("TestFiles\\Issue206.zip", ifcDatabaseSizeThreshHold: 0))
            {
                xbimFile = ifcStore.Location;
                flushmapFile = Path.ChangeExtension(xbimFile, ".jfm");
                Assert.IsTrue(File.Exists(xbimFile), "XBIM file expected to be found");
                Assert.IsTrue(File.Exists(flushmapFile), "JFM file expected to be found");
                ifcStore.Close();
            }

            Assert.IsFalse(File.Exists(xbimFile), "XBIM file expected to be deleted");
            Assert.IsFalse(File.Exists(flushmapFile), "JFM file expected to be deleted");
        }


        [TestMethod]
        public void FileBasedStore_Should_Retain_Saved_XBIM_Files()
        {
            string transientXbimFile;
            string savedXBimFile = Path.ChangeExtension(Guid.NewGuid().ToString(), ".xbim");

            // Load with Esent/File-based store - creates a temp xbim file in %TEMP%
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0))
            {
                transientXbimFile = ifcStore.Location;
                
                ifcStore.SaveAs(savedXBimFile);
                ifcStore.Close();
            }
            Assert.IsTrue(File.Exists(savedXBimFile));
            Assert.IsFalse(File.Exists(transientXbimFile));
        }

        [TestMethod]
        public void FileBasedStore_Should_Not_retain_Saving_as_Same_file()
        {
            // Tests a special case - saving a transient xbim over itself
            // TODO: In theory could make this disable the transient behaviour
            string transientXbimFile;
            
            // Load with Esent/File-based store - creates a temp xbim file in %TEMP%
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0))
            {
                transientXbimFile = ifcStore.Location;

                ifcStore.SaveAs(transientXbimFile);
                ifcStore.Close();
            }
            Assert.IsFalse(File.Exists(transientXbimFile));
        }

        [TestMethod]        public void IfcStoreInitialisationTest()
        {
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "XbimTest1",
                ApplicationDevelopersName = "Tester",
                EditorsOrganisationName = "XbimTeam"
            };

            using (var store = IfcStore.Create(credentials, XbimSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
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

            using (var store = IfcStore.Create(credentials, XbimSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
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
        [Ignore("We removed the ability to read illegal step files in order to read legal files with the \"\\S\\'\" sequence, that would otherwise be parsed by the normal backslash,S,backslash sequence, causing an early closure of the string when hitting the \"'\".")]
        public void EncodeBackslash()
        {
            const string path = "C:\\Data\\Martin\\document.txt";
            const string encodedPath = "C:\\\\Data\\\\Martin\\\\document.txt";
            const string test = "BackslashEncoding.ifc";
            using (var store = IfcStore.Create(XbimSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
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

        [TestMethod]
        public void Ifc2x3FinalSchemaTest()
        {
            using (var model = new Xbim.IO.Memory.MemoryModel( ef2x3))
            {
                var errCount = model.LoadStep21("TestFiles\\ifc2x3_final_wall.ifc");
                Assert.AreEqual(0, errCount);
            }

            using (var model = new Xbim.IO.Esent.EsentModel( ef2x3))
            {
                var errCount = model.CreateFrom("TestFiles\\ifc2x3_final_wall.ifc");
                Assert.AreEqual(true, errCount);
            }
        }

        [TestMethod]
        // this is a meta tast of the large stream mock-up
        public void LargeStreamTest()
        {
            using (var stream = File.OpenRead("TestFiles\\ifc2x3_final_wall.ifc"))
            using (var data = new LargeStream(stream))
            {
                data.Position = LargeStream.offset;
                var buffer = new byte[stream.Length + 100];
                var read = data.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(stream.Length, read);
                Assert.AreEqual((byte)'I', buffer[0]);

                data.Position = LargeStream.offset - 100;
                buffer = new byte[stream.Length + 100];
                read = data.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(stream.Length + 100, read);
                Assert.AreEqual((byte)' ', buffer[99]);
                Assert.AreEqual((byte)'I', buffer[100]);

                data.Position = data.Length - 1;
                buffer = new byte[1000000];
                read = data.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(1, read);
                read = data.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(0, read);
            }
        }

        [TestMethod]
        public void ParsingLargeFile()
        {
            using (var model = new Xbim.IO.Memory.MemoryModel(ef2x3))
            {
                using (var stream = File.OpenRead("TestFiles\\ifc2x3_final_wall.ifc"))
                using (var data = new LargeStream(stream))
                {
                    var errCount = model.LoadStep21(data, data.Length);
                    Assert.AreEqual(0, errCount);
                    Assert.AreEqual(1, model.Instances.OfType<Ifc2x3.SharedBldgElements.IfcWallStandardCase>().Count());
                }
            }
        }

        /// <summary>
        /// This class pretends that it is a large stream but first int.MaxValue bytes are just white spaces
        /// </summary>
        private class LargeStream : Stream
        {
            internal readonly Stream inner;
            internal const long offset = int.MaxValue;
            private static readonly byte[] line = "                                                                                                    \r\n"
                .Select(c => Convert.ToByte(c)).ToArray();

            public LargeStream(Stream stream)
            {
                inner = stream;
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => offset + inner.Length;

            public override long Position { get; set; }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int bufferOffset, int count)
            {
                // adjust count to length
                count = (int)Math.Min(count, Length - Position);
                if (count == 0)
                    return count;

                // all in offset
                if ((Position + count) < offset)
                {
                    for (int i = 0; i < count; i++)
                        buffer[bufferOffset + i] = line[((Position + i) % line.Length)];

                    Position += count;
                    return count;
                }

                // all in actual data
                if (Position >= offset)
                {
                    var position = Position - offset;
                    if (position != inner.Position)
                        inner.Seek(position, SeekOrigin.Begin);

                    Position += count;
                    return inner.Read(buffer, bufferOffset, count);
                }

                // overlap
                { 
                    var spaceCount = (int)(offset - Position);
                    int idx = 0;
                    for (; idx < spaceCount; idx++)
                        buffer[bufferOffset + idx] = (byte)' ';
                    var dataCount = count - spaceCount;
                    
                    // adjust
                    var position = Position - offset + spaceCount;
                    if (position != inner.Position)
                        inner.Seek(position, SeekOrigin.Begin);
                    dataCount = inner.Read(buffer, spaceCount, dataCount);

                    Position += count;
                    return count;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            protected override void Dispose(bool disposing)
            {
                inner.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
