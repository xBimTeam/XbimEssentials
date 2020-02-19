using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Ifc;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Memory;
using IfcRelSpaceBoundary = Xbim.Ifc4.ProductExtension.IfcRelSpaceBoundary;
using IfcTypeProduct = Xbim.Ifc4.Kernel.IfcTypeProduct;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class InMemoryInsertionTests
    {
        private static readonly IEntityFactory ef4 = new Ifc4.EntityFactoryIfc4();
        private static readonly IEntityFactory ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();

        [TestMethod]
        public void Ifc4InsertCopyTest()
        {
            using (var model = IfcStore.Open(@"TestFiles\\Ifc4WithNestedLists.ifcZip"))
            {
                               
                using (var iModel = new IO.Memory.MemoryModel(ef4))
                {
                    using (var txn = iModel.BeginTransaction("Insert copy"))
                    {
                        var w = new Stopwatch();
                        w.Start();
                        iModel.InsertCopy(model.Instances[61828], new XbimInstanceHandleMap(model, iModel), null, true, true);
                        txn.Commit();
                        w.Stop();
                     
                       // Debug.WriteLine("Time to insert {0} walls (Overall {1} entities): {2}ms", iWalls.Count, iModel.Instances.Count, w.ElapsedMilliseconds);

                        //Assert.IsTrue(iWalls.Count >= 1);
                       
                    }
                    var tw = File.Create("Ifc4WithNestedListsExtract.ifc");
                    iModel.SaveAsStep21(tw);
                    tw.Close();
                    

                }
            }
        }

        [TestMethod]
        public void CompleteProductInsert()
        {
            const string original = "TestFiles\\4walls1floorSite.ifc";
            using (var model = new IO.Memory.MemoryModel(ef2x3))
            {
                var errs = model.LoadStep21(original);
                Assert.AreEqual(0, errs);
                using (model.BeginEntityCaching())
                using (model.BeginInverseCaching())
                {
                    var products = model.Instances.OfType<IfcProduct>();
                    using (var iModel = new IO.Memory.MemoryModel(ef2x3))
                    {
                        var map = new XbimInstanceHandleMap(model, iModel);
                        using (var txn = iModel.BeginTransaction("Insert copy"))
                        {
                            var w = new Stopwatch();
                            w.Start();
                            iModel.InsertCopy(products, true, false, map);
                            txn.Commit();
                            w.Stop();

                            var copies = Path.ChangeExtension(original, ".copy.ifc");
                            using (var f = File.Create(copies))
                            {
                                iModel.SaveAsIfc(f);
                                f.Close();
                            }
                        }

                        // use all caching we can for this
                        using (iModel.BeginEntityCaching())
                        using (iModel.BeginInverseCaching())
                        {
                            // number of products should be the same
                            var origProdCount = model.Instances.CountOf<IfcProduct>();
                            var prodCount = iModel.Instances.CountOf<IfcProduct>();
                            Assert.AreEqual(origProdCount, prodCount);

                            // number of geometry representations should be the same
                            var origRepCount = model.Instances.CountOf<IfcProductRepresentation>();
                            var repCount = model.Instances.CountOf<IfcProductRepresentation>();
                            Assert.AreEqual(origRepCount, repCount);

                            // number of geometry representations should be the same
                            var origRepItemCount = model.Instances.CountOf<IfcRepresentationItem>();
                            var repItemCount = model.Instances.CountOf<IfcRepresentationItem>();
                            Assert.AreEqual(origRepItemCount, repItemCount);

                            // number of representation items in every product should be the same
                            foreach (var product in model.Instances.OfType<IfcProduct>())
                            {
                                var iProduct = map[new XbimInstanceHandle(product)].GetEntity() as IfcProduct;

                                var count = product.Representation?.Representations.SelectMany(r => r.Items).Count();
                                var iCount = iProduct.Representation?.Representations.SelectMany(r => r.Items).Count();

                                Assert.AreEqual(count, iCount);
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void CopyWallsOver()
        {
            const string original = "TestFiles\\4walls1floorSite.ifc";
            const string inserted = "..\\..\\Inserted.ifc";

            PropertyTranformDelegate semanticFilter = (property, parentObject) =>
            {
                //leave out geometry and placement
                if ((property.PropertyInfo.Name == "Representation" || property.PropertyInfo.Name == "ObjectPlacement") &&
                    parentObject is IfcProduct)
                    return null;

                //only bring over IsDefinedBy and IsTypedBy inverse relationships
                if (property.EntityAttribute.Order < 0 && !(
                    property.PropertyInfo.Name == "IsDefinedBy" ||
                    property.PropertyInfo.Name == "IsTypedBy"
                    ))
                    return null;

                return property.PropertyInfo.GetValue(parentObject, null);
            };

            using (var model = new IO.Memory.MemoryModel(ef2x3))
            {
                var errs = model.LoadStep21(original);
                Assert.AreEqual(0, errs);
                var wall = model.Instances.FirstOrDefault<IfcWall>();
                using (var iModel = new IO.Memory.MemoryModel(ef2x3))
                {
                    using (var txn = iModel.BeginTransaction("Insert copy"))
                    {
                        var w = new Stopwatch();
                        w.Start();
                        iModel.InsertCopy(wall, new XbimInstanceHandleMap(model, iModel), null, true, true);
                        txn.Commit();
                        w.Stop();

                        var iWalls = iModel.Instances.OfType<IfcWall>().ToList();
                        Debug.WriteLine("Time to insert {0} walls (Overall {1} entities): {2}ms", iWalls.Count, iModel.Instances.Count, w.ElapsedMilliseconds);
                        
                        Assert.IsTrue(iWalls.Count >= 1);
                        using (var fileStream = new StreamWriter(inserted))
                        {
                            iModel.SaveAsStep21(fileStream);
                        }
                    }

                    
                }
            }

            CompareEntityLines(inserted, original);
        }



        private void CompareEntityLines(string insertedFile, string originalFile)
        {
            var inserted = GetEntities(insertedFile);
            var original = GetEntities(originalFile);

            //all entities from inserter should exist in original
            foreach (var iPair in inserted)
            {
                var o = original[iPair.Key];
                Assert.IsTrue(string.CompareOrdinal(o, iPair.Value) == 0);
            }
        }

        private readonly List<string> _toIgnore = new List<string>
        {
            "IFCCARTESIANPOINT", 
            "IFCDIRECTION", 
            "IFCGEOMETRICREPRESENTATIONCONTEXT"
        }; 

        private Dictionary<int, string> GetEntities(string path)
        {
            var entities = new Dictionary<int, string>();
            using (var reader = File.OpenText(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;

                    if (!line.StartsWith("#")) continue;
                    var sepIndex = line.IndexOf("=", StringComparison.Ordinal);

                    var label = line.Substring(0, sepIndex).Trim('#', '=', ' ');
                    var entity = line.Substring(sepIndex + 1).Trim();

                    //Ignore some of the entities where mismatch is caused by slightly different float strings
                    if (_toIgnore.Any(i => entity.StartsWith(i)))
                        continue;

                    entities.Add(int.Parse(label), entity);
                }
            }
            return entities;
        }

        [TestMethod]
        public void ExtractSemanticModel()
        {
            const string original = "TestFiles\\SampleHouse4.ifc";
            using (var model = new IO.Memory.MemoryModel(ef4))
            {
                model.LoadStep21(original);
                var roots = model.Instances.OfType<Ifc4.Kernel.IfcRoot>();
                using (var iModel = new IO.Memory.MemoryModel(ef4))
                {
                    using (var txn = iModel.BeginTransaction("Insert copy"))
                    {
                        var mappings = new XbimInstanceHandleMap(model, iModel);
                        foreach (var root in roots)
                        {
                            iModel.InsertCopy(root, mappings, Filter, false, true);
                        }
                        txn.Commit();
                        using (var fileStream = new StreamWriter("..\\..\\SampleHouseSemantic4.ifc"))
                        {
                            iModel.SaveAsStep21(fileStream);
                        }
                    }
                }
            }
        }

        private static object Filter(ExpressMetaProperty property, object parentObject)
        {
            //leave out geometry and placement of products
            if (parentObject is Ifc4.Kernel.IfcProduct &&
                (property.PropertyInfo.Name == "Representation" || 
                property.PropertyInfo.Name == "ObjectPlacement"))
                return null;

            //leave out representation maps
            if (parentObject is IfcTypeProduct && 
                property.PropertyInfo.Name == "RepresentationMaps")
                return null;

            //leave out eventual connection geometry
            if (parentObject is IfcRelSpaceBoundary && 
                property.PropertyInfo.Name == "ConnectionGeometry")
                return null;

            //return the value for anything else
            return property.PropertyInfo.GetValue(parentObject, null);
        }
    }
}
