using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
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
            var credentials = new XbimEditorCredentials
            {
                ApplicationIdentifier = "TestApp",
                ApplicationDevelopersName = "TestApp",
                EditorsOrganisationName = "Test"
            };
            TestFederation<Ifc4.Kernel.IfcProject>(IfcSchemaVersion.Ifc4, credentials);
            TestFederation<Ifc2x3.Kernel.IfcProject>(IfcSchemaVersion.Ifc2X3, credentials);
        }

        private static void TestFederation<T>(IfcSchemaVersion schema, XbimEditorCredentials credentials) where  T : IInstantiableEntity, IIfcProject
        {
            var models = new List<string>();
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
                    model.SaveAs(ifcFileName, IfcStorageType.Ifc);
                }
                var d = new DirectoryInfo(".");
                models.Add(ifcFileName);
                File.Delete(esentFileName);
            }
            var fedName = string.Format(@"federation{0}.xbim", schema);
            using (var ifcStore = IfcStore.Create(fedName, credentials, schema))
            {
                foreach (var modelName in models)
                {
                    ifcStore.AddModelReference(modelName, "Organisation", "Role");
                }
            }
            using (var ifcStore = IfcStore.Open(fedName))
            {
                Assert.IsTrue(ifcStore.IsFederation, "Federation not created");
                Assert.AreEqual(ifcStore.ReferencedModels.Count(), 2, "Should have two federated models.");
            }

            // clean all
            File.Delete(fedName);
            foreach (var model in models)
            {
                File.Delete(model);
            }
        }
    }
}
