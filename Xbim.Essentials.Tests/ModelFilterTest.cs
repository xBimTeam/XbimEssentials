using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class ModelFilterTest
    {
        private static Ifc2x3.IO.XbimModel CreateAndInitModel(string projectName)
        {
            var model = Ifc2x3.IO.XbimModel.CreateModel(projectName + ".xBIM"); //create an empty model
            //Begin a transaction as all changes to a model are transacted
            using (var txn = model.BeginTransaction("Initialise Model"))
            {
                //do once only initialisation of model application and editor values
                model.DefaultOwningUser.ThePerson.GivenName = "John";
                model.DefaultOwningUser.ThePerson.FamilyName = "Bloggs";
                model.DefaultOwningUser.TheOrganization.Name = "Department of Building";
                model.DefaultOwningApplication.ApplicationIdentifier = "Construction Software inc.";
                model.DefaultOwningApplication.ApplicationDeveloper.Name = "Construction Programmers Ltd.";
                model.DefaultOwningApplication.ApplicationFullName = "Ifc sample programme";
                model.DefaultOwningApplication.Version = "2.0.1";
                //set up a project and initialise the defaults
                var project = model.Instances.New<IfcProject>();
                project.Initialize(ProjectUnits.SIUnitsUK);
                project.Name = "testProject";
                project.OwnerHistory.OwningUser = model.DefaultOwningUser;
                project.OwnerHistory.OwningApplication = model.DefaultOwningApplication;
				
                txn.Commit();
                return model;
            }
        }

        [TestMethod]
        public void MergeProductsTest()
        {
            const string model1File = "4walls1floorSite.ifc";
            const string copyFile = "copy.ifc";
            const string model2File = "House.ifc";
            var newModel = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel);
            using (var model1 = IfcStore.Open(model1File))
            {
                PropertyTranformDelegate propTransform = delegate(ExpressMetaProperty prop, object toCopy)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };
               

                using (var model2 = IfcStore.Open(model2File))
                {

                    var rencontre = false;
                    using (var txn = newModel.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(model1, newModel);
                        foreach (var item in model1.Instances.OfType<IfcProduct>())
                        {
                            newModel.InsertCopy(item, copied, propTransform, false, false);
                        }
                        copied = new XbimInstanceHandleMap(model2, newModel);
                        foreach (var item in model2.Instances.OfType<IfcProduct>())
                        {
                            var buildingElement = item as IfcBuildingElement;
                            if (model1.Instances.OfType<IfcBuildingElement>()
                                .Any(item1 => buildingElement != null && buildingElement.GlobalId == item1.GlobalId))
                            {
                                rencontre = true;
                            }
                            if (!rencontre)
                            {
                                newModel.InsertCopy(item, copied, propTransform, false, false);
                            }
                        }

                        txn.Commit();
                    }
                    newModel.SaveAs(copyFile);
                }
                newModel.Close();
            }
        }

        [TestMethod]
        public void CopyAllEntitiesTest()
        {
            var sourceFile = "source.ifc";
            var copyFile = "copy.ifc";
            using (var source = new Ifc2x3.IO.XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate (ExpressMetaProperty prop, object toCopy)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };
                //source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\crash\NBS_LakesideRestaurant_EcoBuild2015_Revit2014_.ifc","source.xbim",null,true);
               
                //source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\Wall with complex openings.ifc", "source.xbim",null,true);
                source.Open("BIM Logo-LetterM.xBIM");
                source.SaveAs(sourceFile);
                using (var target = Ifc2x3.IO.XbimModel.CreateTemporaryModel())
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        target.Header = source.Header;
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, txn, propTransform,true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
               FileCompare(sourceFile, copyFile);
            }
        }

        [TestMethod]
        public void ExtractIfcGeometryEntitiesTest()
        {
            using (var source = new Ifc2x3.IO.XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate (ExpressMetaProperty prop, object toCopy)
                {

                    if (toCopy is IfcProduct)
                    {
                        if (prop.PropertyInfo.Name == "ObjectPlacement" || prop.PropertyInfo.Name == "Representation")
                            return null;
                    }
                    if (toCopy is IfcTypeProduct)
                    {
                        if (prop.PropertyInfo.Name == "RepresentationMaps")
                            return null;
                    }
                    return prop.PropertyInfo.GetValue(toCopy, null);//just pass through the value               
                };

                //source.LoadStep21("BIM Logo-LetterM.xBIM");
                //source.SaveAs("WithGeometry.ifc");
                var modelName = @"4walls1floorSite";
                var xbimModelName = Path.ChangeExtension(modelName, "xbim");

                source.CreateFrom(Path.ChangeExtension(modelName, "ifc"), null, null, true);

                using (var target = Ifc2x3.IO.XbimModel.CreateModel(Path.ChangeExtension(modelName + "_NoGeom", "xbim")))
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances.OfType<IfcRoot>())
                        {
                            target.InsertCopy(item, copied, txn, propTransform, false);
                        }
                        txn.Commit();
                    }

                    target.SaveAs(Path.ChangeExtension(modelName + "_NoGeom", "ifc"));
                    target.Close();

                }

                source.Close();
                // XbimModel.Compact(Path.ChangeExtension(modelName + "_NoGeom", "xbim"), Path.ChangeExtension(modelName + "_NoGeom_Compacted", "xbim"));
                //the two files should be the same
            }
        }



        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private void FileCompare(string file1, string file2)
        {
            string file1Line;
            string file2Line;
            
            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return to indicate that the files are the same.
                return;
            }



            //// Check the file sizes. If they are not the same, the files 
            //// are not the same.
            //var l1 = new FileInfo(file1).Length;
            //var l2 = new FileInfo(file2).Length;
            //if (l1 != l2)
            //{
            //    // Return false to indicate files are different
            //    Assert.Fail("File Lengths are different, Source == {0}, Copy == {1}",l1,l2);
            //}
            // LoadStep21 the two files.
            using (var fs1 = new StreamReader(file1))
            {
                using (var fs2 = new StreamReader(file2))
                {
                    // Read and compare a line from each file until either a
                    // non-matching set of bytes is found or until the end of
                    // file1 is reached.
                    do
                    {
                        // Read one line from each file.
                        file1Line = fs1.ReadLine();
                        file2Line = fs2.ReadLine();
                        Assert.IsTrue(file1Line == file2Line, string.Format("'{0}' != '{1}'", file1Line, file2Line));
                    }
                    while (file1Line != null);
                    Assert.IsTrue(file2Line == null, "Copy file is longer than the source file");                   
                }
            }
        }

    }
}
