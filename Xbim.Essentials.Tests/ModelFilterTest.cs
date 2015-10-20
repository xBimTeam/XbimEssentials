using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;
using Xbim.Ifc2x3.Kernel;
using System.IO;
using Xbim.Common.Metadata;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class ModelFilterTest
    {
        [TestMethod]
        public void CopyAllEntitiesTest()
        {
            var sourceFile = "source.ifc";
            var copyFile = "copy.ifc";
            using (var source = new Xbim.Ifc2x3.IO.XbimModel())
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
                using (var target = Xbim.Ifc2x3.IO.XbimModel.CreateTemporaryModel())
                {
                    target.AutoAddOwnerHistory = false;
                    using (var txn = target.BeginTransaction())
                    {
                        target.Header = source.Header;
                        var copied = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            var cpy =  target.InsertCopy(item, copied, txn, propTransform,true);
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
            using (var source = new Xbim.Ifc2x3.IO.XbimModel())
            {
                PropertyTranformDelegate propTransform = delegate (ExpressMetaProperty prop, object toCopy)
                {

                    if (typeof(IfcProduct).IsAssignableFrom(toCopy.GetType()))
                    {
                        if (prop.PropertyInfo.Name == "ObjectPlacement" || prop.PropertyInfo.Name == "Representation")
                            return null;
                    }
                    if (typeof(IfcTypeProduct).IsAssignableFrom(toCopy.GetType()))
                    {
                        if (prop.PropertyInfo.Name == "RepresentationMaps")
                            return null;
                    }
                    return prop.PropertyInfo.GetValue(toCopy, null);//just pass through the value               
                };

                //source.Open("BIM Logo-LetterM.xBIM");
                //source.SaveAs("WithGeometry.ifc");
                string modelName = @"4walls1floorSite";
                string xbimModelName = Path.ChangeExtension(modelName, "xbim");

                source.CreateFrom(Path.ChangeExtension(modelName, "ifc"), null, null, true);

                using (var target = Xbim.Ifc2x3.IO.XbimModel.CreateModel(Path.ChangeExtension(modelName + "_NoGeom", "xbim")))
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
            // Open the two files.
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
