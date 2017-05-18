using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Step21;
using Xbim.Ifc;

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
            TestFederation(IfcSchemaVersion.Ifc4, credentials);
        }

        private static void TestFederation(IfcSchemaVersion schema, XbimEditorCredentials credentials)
        {
            var models = new List<string>();
            // write the files to disk so that the federation finds them
            //
            for (int i = 1; i < 3; i++)
            {
                var fName = string.Format("model{0}{1}.ifc", i, schema);
                using (var tmpFile = IfcStore.Create(fName, credentials, schema))
                {
                    // nothing to do; just creates the files.
                }
                models.Add(fName);
            }

            var fedName = string.Format(@"federation{0}.xbim", schema);

            using (var ifcStore = IfcStore.Create(fedName, credentials, Common.Step21.IfcSchemaVersion.Ifc4))
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
