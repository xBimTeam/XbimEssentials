using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Memory;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class InMemoryInsertionTests
    {
        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void CopyWallsOver()
        {
            const string original = "4walls1floorSite.ifc";
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

            using (var model = new MemoryModel<EntityFactory>())
            {
                model.Open(original);
                var wall = model.Instances.FirstOrDefault<IfcWall>();
                using (var iModel = new MemoryModel<EntityFactory>())
                {
                    using (var txn = iModel.BeginTransaction("Insert copy"))
                    {
                        var w = new Stopwatch();
                        w.Start();
                        var mappings = new Dictionary<int, IPersistEntity>();
                        iModel.InsertCopy(wall, mappings, true);
                        txn.Commit();
                        w.Stop();

                        var iWalls = iModel.Instances.OfType<IfcWall>().ToList();
                        Debug.WriteLine("Time to insert {0} walls (Overall {1} entities): {2}ms", iWalls.Count, iModel.Instances.Count, w.ElapsedMilliseconds);
                        
                        Assert.IsTrue(iWalls.Count >= 1);
                        iModel.Save(inserted);
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

    }
}
