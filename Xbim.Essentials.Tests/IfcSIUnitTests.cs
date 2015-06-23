using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.IO;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.Extensions;
using System.IO;
using System.Linq;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class IfcSIUnitTests
    {

        /// <summary>
        /// Bug GH Issue#4 (Codeplex 
        /// </summary>
        [TestMethod]
        public void ToString_Should_Equal_PrefixPlusName()
        {
            var modelFile = CreateModelWithUnit();
            var model = new XbimModel();
            // reopen the IFC we just created
            model.CreateFrom(modelFile, null, null, true);

            var unit = model.IfcProject.UnitsInContext.Units.First();

            // Bug was the this returned AMPERE unless Activate had been called.
            Assert.AreEqual("MILLIMETRE", unit.ToString());

            model.Close();
            model.Dispose();

            DeleteFile(modelFile);
        }



        private static string CreateModelWithUnit()
        {
            using (var model = XbimModel.CreateTemporaryModel())
            {
                CreateandInitModel(model);

                var tempModel = Path.ChangeExtension(Path.GetTempFileName(), ".ifc");

                model.SaveAs(tempModel);

                return tempModel;
            }
        }

        static private void CreateandInitModel(XbimModel model)
        {
            //Begin a transaction as all changes to a model are transacted
            using (XbimReadWriteTransaction txn = model.BeginTransaction("Initialise Model"))
            {
                //do once only initialisation of model application and editor values
                model.DefaultOwningUser.ThePerson.GivenName = "John";
                model.DefaultOwningUser.ThePerson.FamilyName = "Bloggs";
                model.DefaultOwningUser.TheOrganization.Name = "Department of Building";
                model.DefaultOwningApplication.ApplicationIdentifier = "Construction Software inc.";
                model.DefaultOwningApplication.ApplicationDeveloper.Name = "Construction Programmers Ltd.";
                model.DefaultOwningApplication.ApplicationFullName = "Ifc Unit Tests";
                model.DefaultOwningApplication.Version = "2.0.1";

                //set up a project and initialise the defaults

                var project = model.Instances.New<IfcProject>();

                // Add the standard Units
                project.Initialize(ProjectUnits.SIUnitsUK);
                project.Name = "testProject";
                project.OwnerHistory.OwningUser = model.DefaultOwningUser;
                project.OwnerHistory.OwningApplication = model.DefaultOwningApplication;

                // Sanity check first unit is what we expect
                var firstUnit = ((IfcSIUnit)model.IfcProject.UnitsInContext.Units.First);
                Assert.AreEqual(firstUnit.Name, IfcSIUnitName.METRE);
                Assert.AreEqual(firstUnit.Prefix, IfcSIPrefix.MILLI);

                txn.Commit();
            }
        }

        private void DeleteFile(string modelFile)
        {
            try
            {
                File.Delete(modelFile);
                File.Delete(Path.ChangeExtension(modelFile, ".xbim"));
            }
            catch
            { }
        }
    }
}
