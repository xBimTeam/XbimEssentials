using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.IO;
using Xbim.Ifc2x3.Kernel;
using Xbim.IO.Esent;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(ModelA, Root)]
    [DeploymentItem(ModelB, Root)]
    [DeploymentItem(ModelC, Root)]
    [DeploymentItem(ModelFedP1, Root)]
    [DeploymentItem(ModelFedP2, Root)]
    public class Federation_Tests
    {
        private const string Root = "TestSourceFiles";
        private const string ModelA = Root + @"\BIM Logo-ExclaimationBody.xBIM";
        private const string ModelB = Root + @"\BIM Logo-LetterB.xBIM";
        private const string ModelC = Root + @"\BIM Logo-LetterM.xBIM";

        private const string ModelFedP1 = Root + @"\P1.xBIM";
        private const string ModelFedP2 = Root + @"\P2.xBIM";

        [TestMethod]
        public void ValidIdentityInFederation()
        {
            using (var fedModel = Xbim.Ifc2x3.IO.XbimModel.CreateTemporaryModel())
            {
                fedModel.Initialise("Federation Creating Author", "Federation Creating Organisation", "This Application", "This Developer", "v1.1");
                using (var txn = fedModel.BeginTransaction())
                {
                    fedModel.IfcProject.Name = "Federation Project Name";
                    txn.Commit();
                }
                //now add federated models
                fedModel.AddModelReference(ModelFedP1, "The Architects Name", IfcRoleEnum.ARCHITECT);
                fedModel.AddModelReference(ModelFedP2, "The Owners Name", IfcRoleEnum.BUILDINGOWNER);
                fedModel.SaveAs("P1P2Federation", XbimStorageType.Step21);
            } //close and automatically delete the temporary database
            //Now open the Ifc file and see what we have
            using (var fed = new Xbim.Ifc2x3.IO.XbimModel())
            {
                fed.CreateFrom("P1P2Federation.ifc", "P1P2Federation.xBIMF"); //use xBIMF to help us distinguish
                fed.Open("P1P2Federation.xBIMF");
                fed.EnsureUniqueUserDefinedId();

                var mustDiffer =
                    fed.Instances.OfType<IfcGeometricRepresentationSubContext>()
                        .Where(x => x.ContextIdentifier == @"Body").ToArray();
                
                // we are expecting two items (one body from each model loaded)
                // they happen to share the same entitylabel, but they have different models.

                var first = mustDiffer[0];
                var second = mustDiffer[1];

                Assert.IsFalse(first == second);

                var tst = new HashSet<IfcGeometricRepresentationContext>();
                tst.Add(first);
                tst.Add(second);
            }
        }

        /// <summary>
        /// Creates anew Federation with three models A,B and C
        /// </summary>
        [TestMethod]
        public void CreateFederation()
        {
            using (var fedModel = Xbim.Ifc2x3.IO.XbimModel.CreateTemporaryModel())
            {
                fedModel.Initialise("Federation Creating Author", "Federation Creating Organisation", "This Application", "This Developer", "v1.1");
                using (var txn = fedModel.BeginTransaction())
                {
                    fedModel.IfcProject.Name = "Federation Project Name";
                    txn.Commit();
                }
                //now add federated models
                fedModel.AddModelReference(ModelA, "The Architects Name", IfcRoleEnum.ARCHITECT);
                fedModel.AddModelReference(ModelB, "The Owners Name", IfcRoleEnum.BUILDINGOWNER);
                // fedModel.AddModelReference(ModelC, "The Cost Consultants Name", IfcRole.UserDefined, "Cost Consultant");
                fedModel.SaveAs("Federated Model", XbimStorageType.Step21);
            } //close and automatically delete the temporary database
            //Now open the Ifc file and see what we have
            using (var fed = new Xbim.Ifc2x3.IO.XbimModel())
            {
                fed.CreateFrom("Federated Model.ifc", "Federated Model.xBIMF"); //use xBIMF to help us distinguish
                fed.Open("Federated Model.xBIMF");

                //check the various ways of access objects give consistent results.
                var localInstances = fed.InstancesLocal.Count;
                var totalInstances = fed.Instances.Count;
                long refInstancesCount = 0;
                foreach (var refModel in fed.ReferencedModels)
                {
                    refInstancesCount += refModel.Model.Instances.Count;
                }

                Assert.IsTrue(totalInstances == refInstancesCount + localInstances);

                long enumeratingInstancesCount = 0;
                foreach (var item in fed.Instances)
                {
                    enumeratingInstancesCount++;
                }
                Assert.IsTrue(totalInstances == enumeratingInstancesCount);

                long fedProjectCount = fed.Instances.OfType<IfcProject>().Count();
                long localProjectCount = fed.InstancesLocal.OfType<IfcProject>().Count();
                Assert.IsTrue(fedProjectCount == 3);
                Assert.IsTrue(localProjectCount == 1);
            }
        }
    }
}
