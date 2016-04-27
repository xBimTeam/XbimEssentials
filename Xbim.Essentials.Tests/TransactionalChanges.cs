using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class TransactionalChanges
    {
        [TestMethod]
        public void TransactionLog()
        {
            using (var model = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
            {
                var changed = new List<string>();
                var valuesLog = new StringWriter();
                model.EntityModified += (entity, property) =>
                {
                    //use reflection to get property information for the changed property
                    var pInfo = entity.ExpressType.Properties[property];
                    changed.Add(pInfo.Name);

                    //express indices are 1 based
                    var propertyIndex = property - 1;

                    //you can use reflection to get the current (new) value
                    var value = pInfo.PropertyInfo.GetValue(entity, null);

                    //this is part of the serialization engine but you can use it for a single property as well
                    PersistEntityExtensions.WriteProperty(pInfo.PropertyInfo.PropertyType, value, valuesLog, null, model.Metadata);
                    valuesLog.WriteLine();
                };
                using (var txn = model.BeginTransaction())
                {
                    var wall = model.Instances.New<IfcWall>();
                    wall.Name = "New name";
                    wall.Description = null;

                    var pset = model.Instances.New<IfcPropertySet>();
                    pset.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>());

                    Assert.IsTrue(changed.SequenceEqual(new []{"Name", "Description", "HasProperties"}));
                    Assert.AreEqual(valuesLog.ToString(), "'New name'\r\n$\r\n(#3)\r\n");
                    
                    txn.Commit();
                }
            }
        }
    }
}
