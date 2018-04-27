using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Essentials.Tests.Utilities;
using Xbim.Ifc;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO.Memory;

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
            var newModel = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3());
            using (var model1 = MemoryModel.OpenRead(model1File))
            {
                PropertyTranformDelegate propTransform = delegate(ExpressMetaProperty prop, object toCopy)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };
               
                using (var model2 = MemoryModel.OpenRead(model2File))
                {
                    var rencontre = false;
                    using (var txn = newModel.BeginTransaction("test"))
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
                    using (var file = File.Create(copyFile))
                    {
                        newModel.SaveAsStep21(file);
                        file.Close();
                    }
                }
            }
        }

        [TestMethod]
        public void CopyAllEntitiesTest()
        {
            const string sourceFile = "4walls1floorSite.ifc";
            var copyFile = "copy.ifc";
            using (var source = MemoryModel.OpenRead(sourceFile))
            {
                PropertyTranformDelegate propTransform = delegate (ExpressMetaProperty prop, object toCopy)
                {
                    var value = prop.PropertyInfo.GetValue(toCopy, null);
                    return value;
                };
                using (var target = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
                {
                    using (var txn = target.BeginTransaction("Inserting copies"))
                    {
                        target.Header.FileDescription = source.Header.FileDescription;
                        target.Header.FileName = source.Header.FileName;
                        target.Header.FileSchema = source.Header.FileSchema;

                        var map = new XbimInstanceHandleMap(source, target);

                        foreach (var item in source.Instances)
                        {
                            target.InsertCopy(item, map, propTransform, true, true);
                        }
                        txn.Commit();
                    }
                    using (var outFile = File.Create(copyFile))
                    {
                        target.SaveAsStep21(outFile);
                        outFile.Close();
                    }
                }

                //the two files should be the same
               FileCompare(sourceFile, copyFile);
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
