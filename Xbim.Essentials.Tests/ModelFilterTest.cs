using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.Extensions;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;

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
                            target.InsertCopy(item, copied, propTransform, true, true);
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

        /// <summary>
        ///  Model create in Revit
        /// </summary>
        [TestMethod]
        public void CopyAllEntitiesOfComplexModelOfRevitTest()
        {
            var sourceFile = "rac_basic_sample_project.ifczip";
            var copyFile = "rac_basic_sample_project_copy.ifc";
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
            var sourceFile = "House-ArchiCAD.ifc";
            var copyFile = "House-ArchiCAD_copy.ifc";
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
            var sourceFile = "House-Renga.ifc";
            var copyFile = "House-Renga_copy.ifc";
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
            var modelName = @"4walls1floorSite";
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

        private void Diff2IfcFiles(string file1, string file2)
        {
            var _firstModel = IfcStore.Open(file1);//GetXbimModelByFileName(firstModeFileName, true);
            var _secondModel = IfcStore.Open(file2);//GetXbimModelByFileName(secondModelFileName, true);
            if (_firstModel == null || _secondModel == null)
                return;
            if (_firstModel.Instances.OfType<IfcProject>().FirstOrDefault().GlobalId != _secondModel.Instances.OfType<IfcProject>().FirstOrDefault().GlobalId)
                Assert.Fail("GUID does not match the file");
            //if (_firstModel.IfcProject.Name != _secondModel.IfcProject.Name)
            //    Assert.Fail("Project Name does not match");
            var firstModelBuildingElements = _firstModel.Instances.OfType<IfcBuildingElement>().ToList();
            var secondModelBuildingElements = _secondModel.Instances.OfType<IfcBuildingElement>().ToList();
            if (secondModelBuildingElements.Count() != firstModelBuildingElements.Count())
                Assert.Fail("Quantity of of construction projects is different");

            var firstModelBuildings = _firstModel.Instances.OfType<IfcBuilding>().ToList();
            var firstModelBuildingStorys = _firstModel.Instances.OfType<IfcBuildingStorey>().ToList();
            var secondModelBuildings = _secondModel.Instances.OfType<IfcBuilding>().ToList();
            var secondModelBuildingStorys = _secondModel.Instances.OfType<IfcBuildingStorey>().ToList();

            var colectionBuildingsOneGuid = firstModelBuildings.Select(p => p.GlobalId).ToList();
            var colectionBuildingsTwoGuid = secondModelBuildings.Select(p => p.GlobalId).ToList();

            var crossingBuildingsGuid = colectionBuildingsOneGuid.Intersect(colectionBuildingsTwoGuid).ToList();
            if (crossingBuildingsGuid.Count() != 0)
            {
                if (!(crossingBuildingsGuid.Count() == colectionBuildingsOneGuid.Count() && colectionBuildingsOneGuid.Count() == colectionBuildingsTwoGuid.Count()))
                {
                    Assert.Fail("Various quantity buildings");
                }
            }
            else
            {
                Assert.Fail("different buildings");
            }
            var colectionBuildingStorysOneGuid = firstModelBuildingStorys.Select(p => p.GlobalId).ToList();
            var colectionBuildingStorysTwoGuid = secondModelBuildingStorys.Select(p => p.GlobalId).ToList();
            var crossingBuildingStorysGuid = colectionBuildingStorysOneGuid.Intersect(colectionBuildingStorysTwoGuid).ToList();
            if (crossingBuildingStorysGuid.Count() != 0)
            {
                if (!(crossingBuildingStorysGuid.Count() == colectionBuildingStorysOneGuid.Count() && colectionBuildingStorysOneGuid.Count() == colectionBuildingStorysTwoGuid.Count()))
                {
                    Assert.Fail("Different Quantity of floors");
                }
            }
            else
            {
                Assert.Fail("Разные уровни");
            }
            var modified = 0;
            var elementModified = 0;
            var elementDeleted = 0;
            var elementInserted = 0;
            var numberOfCoincidences = 0;
            var isCoincidence = false;
            foreach (var firstModelBuildingElement in firstModelBuildingElements)
            {
                foreach (var secondModelBuildingElement in secondModelBuildingElements)
                {
                    if (firstModelBuildingElement.GlobalId != secondModelBuildingElement.GlobalId) continue;
                    isCoincidence = true;
                    var iSElementModified = false;
                    numberOfCoincidences++;
                    if (firstModelBuildingElement.GetType() != secondModelBuildingElement.GetType())
                    {
                        Assert.Fail(string.Format("{0} The discrepancy between the types of objects 1: {1} 2: {2}"
                            , firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType(), secondModelBuildingElement.GetType()));
                    }
                    if (firstModelBuildingElement.OwnerHistory.CreationDate != secondModelBuildingElement.OwnerHistory.CreationDate)
                    {
                        //
                    }
                    if (firstModelBuildingElement.OwnerHistory.LastModifiedDate != null && secondModelBuildingElement.OwnerHistory.LastModifiedDate != null)
                    {
                        if (firstModelBuildingElement.OwnerHistory.LastModifiedDate < secondModelBuildingElement.OwnerHistory.LastModifiedDate)
                        {
                            Assert.Fail(string.Format("{0} In the second model element is changed {1}",
                                firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType()));
                            iSElementModified = true;
                            modified++;
                        }
                    }

                    if (firstModelBuildingElement.GetMaterial() != null && secondModelBuildingElement.GetMaterial() != null)
                    {
                        //if (firstModelBuildingElement.GetMaterial().Name != secondModelBuildingElement.GetMaterial().Name)
                        //{
                        //    Assert.Fail(string.Format("{0} changed material {1}", firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType()));
                        //    iSElementModified = true;
                        //    modified++;
                        //}
                    }

                    //var ifcObj1 = firstModelBuildingElement as IfcObject;
                    //var ifcObj2 = secondModelBuildingElement as IfcObject;
                    //{
                    //    var typeEntity1 = ifcObj1.GetType();
                    //    var typeEntity2 = ifcObj2.GetDefiningType();
                    //    if (typeEntity1 != null && typeEntity2 != null)
                    //    {
                    //        if (typeEntity1.GlobalId != typeEntity2.GlobalId)
                    //        {
                    //            Assert.Fail(string.Format("{0}  style changed {1}", firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType()));
                    //            iSElementModified = true;
                    //            modified++;
                    //        }
                    //    }
                    //}

                    if (firstModelBuildingElement.ObjectPlacement is IfcLocalPlacement && secondModelBuildingElement.ObjectPlacement is IfcLocalPlacement)
                    {
                        var localPlac1 = firstModelBuildingElement.ObjectPlacement as IfcLocalPlacement;
                        var localPlac2 = secondModelBuildingElement.ObjectPlacement as IfcLocalPlacement;
                        if (localPlac1.RelativePlacement is IfcAxis2Placement3D && localPlac2.RelativePlacement is IfcAxis2Placement3D)
                        {
                            var a1 = localPlac1.RelativePlacement as IfcAxis2Placement3D;
                            var a2 = localPlac2.RelativePlacement as IfcAxis2Placement3D;
                            var loc1 = a1.Location;
                            var loc2 = a2.Location;

                            if (loc1.X.ToString("N") != loc2.X.ToString("N")
                                || loc1.Y.ToString("N") != loc2.Y.ToString("N")
                                || loc1.Z.ToString("N") != loc2.Z.ToString("N"))
                            {
                                Assert.Fail((string.Format("{0} moved object {1}", firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType())));
                                iSElementModified = true;
                                modified++;
                            }
                        }
                    }

                    //var ifcType1 = IfcMetaData.IfcType(firstModelBuildingElement);
                    //var props1 = ifcType1.IfcProperties.Values.Where(p => !p.IfcAttribute.IsDerivedOverride).ToList();
                    //var ifcType2 = IfcMetaData.IfcType(secondModelBuildingElement);
                    //var props2 = ifcType2.IfcProperties.Values.Where(p => !p.IfcAttribute.IsDerivedOverride).ToList();
                    //if (ifcType1.Name == "IfcBuildingElementProxy" || ifcType2.Name == "IfcBuildingElementProxy")
                    //    continue;
                    //if (ifcType1.Name == "IfcSlab" || ifcType2.Name == "IfcSlab")
                    //    continue;
                    //var pr = props1.Intersect(props2).ToList();
                    //if (pr.Count() == props1.Count() && pr.Count() == props2.Count())
                    //{
                    //    int i = 0;
                    //    foreach (var prop1 in props1)
                    //    {
                    //        var prop2 = props2.ToList()[i];
                    //        i++;
                    //        if (prop1.PropertyInfo.Name != prop2.PropertyInfo.Name) continue;
                    //        if (prop1.PropertyInfo.Name == "CreationDate")
                    //            continue;
                    //        if (prop1.PropertyInfo.Name == "OwnerHistory")
                    //            continue;
                    //        if (prop1.PropertyInfo.Name == "Representation")
                    //            continue;
                    //        if (prop1.PropertyInfo.Name == "ObjectPlacement")
                    //            continue;
                    //        var val1 = prop1.PropertyInfo.GetValue(firstModelBuildingElement, null);
                    //        var val2 = prop2.PropertyInfo.GetValue(secondModelBuildingElement, null);
                    //        if (val1 == null || val2 == null) continue;
                    //        if (val1.Equals(val2)) continue;
                    //        Assert.Fail(string.Format("{0} property {1} object was changed: {2} : {3} : {4}"
                    //            , firstModelBuildingElement.GlobalId, prop1.PropertyInfo.Name, firstModelBuildingElement.GetType(), val1, val2));
                    //        iSElementModified = true;
                    //        modified++;
                    //    }
                    //}
                    //else
                    //{
                    //    Assert.Fail(string.Format("{0} Different number of properties {1}"
                    //        , firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType()));
                    //    iSElementModified = true;
                    //    modified++;
                    //}
                    if (iSElementModified)
                    {
                        elementModified++;
                    }
                    break;
                }
                if (!isCoincidence)
                {
                    Assert.Fail(string.Format("{0} In the second model has a deleted item {1}"
                        , firstModelBuildingElement.GlobalId, firstModelBuildingElement.GetType()));
                    elementDeleted++;
                }
                isCoincidence = false;
            }

            if (numberOfCoincidences == 0)
            {
                Assert.Fail("Matching objects not found");
            }

            foreach (var s2edItem in secondModelBuildingElements)
            {
                isCoincidence = false;
                foreach (var f2stItem in firstModelBuildingElements)
                {
                    if (f2stItem.GlobalId == s2edItem.GlobalId)
                    {
                        isCoincidence = true;
                    }
                }
                if (!isCoincidence)
                {
                    Assert.Fail(string.Format("{0} In the second model has the added element {1}"
                        , s2edItem.GlobalId, s2edItem.GetType()));
                    elementInserted++;
                }
            }
            //Assert.Fail(string.Format("changes: {0} modified elements:{1} added: {2} removed: {3}"
            //    , modified, elementModified, elementInserted, elementDeleted));
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
