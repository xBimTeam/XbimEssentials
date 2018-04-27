using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                PropertyTranformDelegate propTransform = delegate (ExpressMetaProperty prop, object toCopy)
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
            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return to indicate that the files are the same.
                return;
            }

            var lines1 = GetLines(file1);
            var lines2 = GetLines(file2);

            var isNumExp = new Regex("[0-9\\-\\+]");
            var numExp = new Regex("[0-9.eE\\+\\-]+");

            foreach (var kvp in lines1)
            {
                var key = kvp.Key;
                var line1 = kvp.Value;
                Assert.IsTrue(lines2.TryGetValue(key, out string line2));

                var parts1 = line1.Split(new[] { ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                var parts2 = line2.Split(new[] { ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                Assert.AreEqual(parts1.Length, parts2.Length);
                var valuesSame = parts1.Zip(parts2, (a, b) => {
                    if (isNumExp.IsMatch(a[0].ToString()) && isNumExp.IsMatch(b[0].ToString()))
                    {
                        var sA = numExp.Match(a).Value;
                        var sB = numExp.Match(b).Value;

                        // compare numbers
                        var nA = double.Parse(sA, CultureInfo.InvariantCulture);
                        var nB = double.Parse(sB, CultureInfo.InvariantCulture);

                        var delta = Math.Abs(nA - nB);
                        return delta < 1e-6;
                    }
                    return a == b;
                }).All(v => v);
                Assert.IsTrue(valuesSame);
            }
        }

        private Dictionary<string, string> GetLines(string file)
        {
            var labelExp = new Regex("#(?<id>[0-9]+)");
            var result = new Dictionary<string, string>();
            using (var r = new StreamReader(file))
            {
                while(true)
                {
                    var line = GetNextNormalizedDataLine(r);
                    if (line == null)
                        break;
                    if (line[0] == '#')
                    {
                        var key = labelExp.Match(line).Groups["id"]?.Value;
                        if (string.IsNullOrWhiteSpace(key))
                            throw new System.Exception("Unexpected line");
                        result.Add(key, line);
                    }

                }
            }
            return result;
        }

        private string GetNextNormalizedDataLine(StreamReader reader)
        {
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    return null;

                //skip blank lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // skip comments
                if (line.StartsWith("*") || line.StartsWith("/"))
                    continue;

                return line.Replace(" ", "");
            }
        }

    }
}
