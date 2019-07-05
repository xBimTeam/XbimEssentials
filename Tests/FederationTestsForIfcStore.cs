using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class FederationTestsForIfcStore
    {
        /// <summary>
        /// Creates a new Federation
        /// </summary>
        [TestMethod]
        public void CreateFederationFromStore()
        {

            //Xbim.IO.

            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "TestApp",
                ApplicationDevelopersName = "TestApp",
                EditorsOrganisationName = "Test"
            };
            TestFederation<Ifc4.Kernel.IfcProject>(XbimSchemaVersion.Ifc4, credentials, true);
            TestFederation<Ifc2x3.Kernel.IfcProject>(XbimSchemaVersion.Ifc2X3, credentials, true);
            TestFederation<Ifc4.Kernel.IfcProject>(XbimSchemaVersion.Ifc4, credentials, false);
            TestFederation<Ifc2x3.Kernel.IfcProject>(XbimSchemaVersion.Ifc2X3, credentials, false);
        }

        private static void TestFederation<T>(XbimSchemaVersion schema, XbimEditorCredentials credentials, bool useXbimFormat) where  T : IInstantiableEntity, IIfcProject
        {
            var d = new DirectoryInfo(".");
            Debug.WriteLine("Working directory is {0}", d.FullName);

            var modelsNames = CreateModels<T>(schema, credentials, useXbimFormat);
            var fedName = CreateFederation(schema, credentials, modelsNames);

            using (var ifcStore = IfcStore.Open(fedName))
            {
                Assert.IsTrue(ifcStore.IsFederation, "Federation not created");
                Assert.AreEqual(ifcStore.ReferencedModels.Count(), 2, "Should have two federated models.");
                foreach (var ifcStoreReferencedModel in ifcStore.ReferencedModels)
                {
                    Debug.WriteLine(ifcStoreReferencedModel.Name);
                }
                ifcStore.Close();
            }

            // clean all
            File.Delete(fedName);
            foreach (var model in modelsNames)
            {
                File.Delete(model);
            }
        }

        private static string CreateFederation(XbimSchemaVersion schema, XbimEditorCredentials credentials, List<string> modelsNames)
        {
            var fedName = string.Format(@"federation{0}.xbim", schema);
            using (var ifcStore = IfcStore.Create(fedName, credentials, schema))
            {
                foreach (var modelName in modelsNames)
                {
                    ifcStore.AddModelReference(modelName, "Organisation", "Role");
                }
                ifcStore.Close();
            }
            return fedName;
        }

        private static List<string> CreateModels<T>(XbimSchemaVersion schema, XbimEditorCredentials credentials, bool useXbimFormat)
            where T : IInstantiableEntity, IIfcProject
        {
            var modelsNames = new List<string>();
            // write the files to disk so that the federation finds them
            //
            for (int i = 1; i < 3; i++)
            {
                var esentFileName = string.Format("model{0}{1}.xbim", i, schema);
                var ifcFileName = Path.ChangeExtension(esentFileName, ".ifc");

                using (var model = IfcStore.Create(esentFileName, credentials, schema))
                {
                    using (var txn = model.BeginTransaction("Hello Wall"))
                    {
                        //there should always be one project in the model
                        var project = model.Instances.New<T>(p => p.Name = "Basic Creation");
                        //our shortcut to define basic default units
                        project.Initialize(ProjectUnits.SIUnitsUK);
                        txn.Commit();
                    }
                    model.SaveAs(ifcFileName, StorageType.Ifc);
                    model.Close();
                }
                if (useXbimFormat)
                {
                    modelsNames.Add(esentFileName);
                    File.Delete(ifcFileName);
                }
                else
                {
                    modelsNames.Add(ifcFileName);
                    File.Delete(esentFileName);
                }
            }
            return modelsNames;
        }
    }
}
