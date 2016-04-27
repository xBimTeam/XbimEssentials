using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class ModelFilterTest
    {
        [TestMethod]
        public void MergeProductsTest()
        {
            const string model1File = "4walls1floorSite.ifc";
            const string copyFile = "copy.ifc";
            const string model2File = "House-Renga.ifc";
            var newModel = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel);
            using (var model1 = IfcStore.Open(model1File))
            {
                PropertyTranformDelegate propTransform = PropTransform;
                using (var model2 = IfcStore.Open(model2File))
                {
                    var rencontre = false;
                    using (var txn = newModel.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(model1, newModel);
                        foreach (var item in model1.Instances)
                        {
                            newModel.InsertCopy(item, copied, propTransform, false, false);
                        }
                        copied = new XbimInstanceHandleMap(model2, newModel);
                        foreach (var item in model2.Instances)
                        {
                            if (item is IfcProduct)
                            {
                                var product2 = item as IfcProduct;
                                if (model1.Instances.OfType<IfcProduct>()
                                    .Any(product1 => product2.GlobalId == product1.GlobalId))
                                {
                                    rencontre = true;
                                }    
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
            const string sourceFile = "source.ifc";
            const string copyFile = "copy.ifc";
            using (var source = IfcStore.Open("BIM Logo-LetterM.xBIM"))
            {
                PropertyTranformDelegate propTransform = PropTransform;
                //source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\crash\NBS_LakesideRestaurant_EcoBuild2015_Revit2014_.ifc","source.xbim",null,true);
                //source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\Wall with complex openings.ifc", "source.xbim",null,true);
                source.SaveAs(sourceFile);
                using (var target = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, propTransform, false, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
                //Diff2IfcFiles(sourceFile, copyFile);
                FileCompare(sourceFile, copyFile);
            }
        }
		
		/// <summary>
        ///  Model create in Revit MEP
        /// </summary>
        [TestMethod]
        public void CopyAllEntitiesOfModelOfRevitMepTest()
        {
            const string sourceFile = "HVAC-Mech.ifc";
            const string copyFile = "HVAC-Mech_copy.ifc";
            using (var source = IfcStore.Open(sourceFile))
            {
                PropertyTranformDelegate propTransform = PropTransform;

                using (var target = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, propTransform, true, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
                //FileCompare(sourceFile, copyFile);
                Diff2IfcFiles(sourceFile, copyFile);
            }
        }

        /// <summary>
        ///  Model create in Revit
        /// </summary>
        [TestMethod]
        public void CopyAllEntitiesOfComplexModelOfRevitTest()
        {
            const string sourceFile = "rac_basic_sample_project.ifczip";
            const string copyFile = "rac_basic_sample_project_copy.ifc";
            using (var source = IfcStore.Open(sourceFile))
            {
                PropertyTranformDelegate propTransform = PropTransform;

                using (var target = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, propTransform, true, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
                //FileCompare(sourceFile, copyFile);
                Diff2IfcFiles(sourceFile, copyFile);
            }
        }

        /// <summary>
        ///  Model create in ArchiCAD
        /// </summary>
        [TestMethod]
        public void CopyAllEntitiesOfComplexModelOfArchiCADTest()
        {
            const string sourceFile = "House-ArchiCAD.ifc";
            const string copyFile = "House-ArchiCAD_copy.ifc";
            using (var source = IfcStore.Open(sourceFile))
            {
                PropertyTranformDelegate propTransform = PropTransform;

                using (var target = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, propTransform, true, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
                //FileCompare(sourceFile, copyFile);
                Diff2IfcFiles(sourceFile, copyFile);
            }
        }

        /// <summary>
        ///  Model create in Renga Architecture http://rengacad.com/ru/
        /// </summary>
        [TestMethod]
        public void CopyAllEntitiesOfComplexModelOfRengaTest()
        {
            const string sourceFile = "House-Renga.ifc";
            const string copyFile = "House-Renga_copy.ifc";
            using (var source = IfcStore.Open(sourceFile))
            {
                PropertyTranformDelegate propTransform = PropTransform;

                using (var target = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, copied, propTransform, true, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                //the two files should be the same
                Diff2IfcFiles(sourceFile, copyFile);
                //FileCompare(sourceFile, copyFile);
            }
        }

        [TestMethod]
        public void ExtractIfcGeometryEntitiesTest()
        {
            const string modelName = @"4walls1floorSite";
            var xbimModelName = Path.ChangeExtension(modelName, "xbim");
            using (var source = IfcStore.Open(xbimModelName))
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
                using (var target = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances.OfType<IfcRoot>())
                        {
                            target.InsertCopy(item, copied, propTransform, true, false);
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

        private static object PropTransform(ExpressMetaProperty prop, object toCopy)
        {
            var value = prop.PropertyInfo.GetValue(toCopy, null);
            return value;
        }

        private static void Diff2IfcFiles(string file1, string file2)
        {
            var firstModel = IfcStore.Open(file1);
            var secondModel = IfcStore.Open(file2);
            if (firstModel == null || secondModel == null)
                Assert.Fail("File not opened");
            if (firstModel.Instances.OfType<IfcProject>().FirstOrDefault().GlobalId != secondModel.Instances.OfType<IfcProject>().FirstOrDefault().GlobalId)
                Assert.Fail("GUID does not match the file");
            if (firstModel.Instances.Count() != secondModel.Instances.Count())
                Assert.Fail("Different amount of entities");
            if (firstModel.Instances.OfType<IfcProduct>().Count() != secondModel.Instances.OfType<IfcProduct>().Count())
                Assert.Fail("Different amount of product");
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
                    var i = 0;
                    do
                    {
                        i++;  
                        // Read one line from each file.
                        file1Line = fs1.ReadLine();
                        file2Line = fs2.ReadLine();
                        // Ignore the header
                        if (i != 3 && i != 4)
                        {
                            Assert.IsTrue(file1Line == file2Line, string.Format("'{0}' != '{1}'", file1Line, file2Line));
                        }
                    }
                    while (file1Line != null);
                    Assert.IsTrue(file2Line == null, "Copy file is longer than the source file");                   
                }
            }
        }

    }
}
