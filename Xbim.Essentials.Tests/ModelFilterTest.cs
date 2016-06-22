using System.Collections.Generic;
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
                                    .Any(product => product2.GlobalId == product.GlobalId))
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
            using (var source = new Ifc2x3.IO.XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate(ExpressMetaProperty prop, object toCopy)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };
                //source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\crash\NBS_LakesideRestaurant_EcoBuild2015_Revit2014_.ifc","source.xbim",null,true);

                //source.CreateFrom(@"C:\Users\Steve\Downloads\Test Models\Wall with complex openings.ifc", "source.xbim",null,true);
                // todo: If use ifc file It does not work! 
                //const string modelName = @"House";
                //source.CreateFrom(Path.ChangeExtension(modelName, "ifc"), null, null, true);
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
                            target.InsertCopy(item, copied, txn, propTransform, true);
                        }
                        txn.Commit();
                    }
                    target.SaveAs(copyFile);
                }
                source.Close();
                IfcFileCompare(sourceFile, copyFile);
                //the two files should be the same
                FileCompare(sourceFile, copyFile);
            }
        }

        [TestMethod]
        public void ExtractIfcGeometryEntitiesTest()
        {
            using (var source = new Ifc2x3.IO.XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate(ExpressMetaProperty prop, object toCopy)
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
                const string modelName = @"4walls1floorSite";

                source.CreateFrom(Path.ChangeExtension(modelName, "ifc"), null, null, true);

                using (var target = Ifc2x3.IO.XbimModel.CreateModel(Path.ChangeExtension(modelName + "_NoGeom", "xbim")))
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances.OfType<IfcRoot>())
                        {
                            target.InsertCopy(item, copied, txn, propTransform);
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

        // For the case when the Entity Label are not sorted
        private static void IfcFileCompare(string file1, string file2)
        {
            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return to indicate that the files are the same.
                return;
            }
            var file2List = new List<string>();
            using (var fs2 = new StreamReader(file2))
            {
                while (!fs2.EndOfStream)
                {
                    var file2Line = fs2.ReadLine();
                    if (file2Line == null) continue;
                    file2Line = file2Line.Replace(" ", string.Empty).Trim();
                    file2List.Add(file2Line);
                }
            }
            using (var fs1 = new StreamReader(file1))
            {   
                while (!fs1.EndOfStream)
                {
                    var file1Line = fs1.ReadLine();
                    if (file1Line == null) continue;
                    file1Line = file1Line.Replace(" ", string.Empty).Trim();
                    if (file1Line.Contains("FILE_DESCRIPTION")) continue;
                    if (file1Line.Contains("FILE_NAME")) continue;
                    // ignore comments
                    if (file1Line.First() == '/') continue;
                    if (file1Line.First() == '*') continue;
                    Assert.IsTrue(file2List.Any(s => s.Equals(file1Line)), 
                        string.Format("file1 != file2 problem in line: {0}", file1Line));
                }
            }
        }

        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private static void FileCompare(string file1, string file2)
        {
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
                    string file1Line;
                    string file2Line;
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