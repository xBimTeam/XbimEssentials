using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Step21;
using Xbim.Essentials.Tests.Utilities;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class TransactionalChanges
    {
        [TestMethod]
        public void TransactionLog()
        {
            using (var models = new ModelFactory(XbimSchemaVersion.Ifc2X3))
            {
                models.Do(model =>
                {
                    var changed = new List<string>();
                    var valuesLog = new StringWriter();
                    var initialLog = new StringWriter();
                    model.EntityModified += (entity, property) =>
                    {
                        //use reflection to get property information for the changed property
                        var pInfo = entity.ExpressType.Properties[property];
                        changed.Add(pInfo.Name);

                        //express indices are 1 based
                        var propertyIndex = property - 1;

                        //overriden attributes have to be treated specially. But these are not present in CobieExpress.
                        if (pInfo.EntityAttribute.State == EntityAttributeState.DerivedOverride)
                            initialLog.Write("*");
                        else
                        {
                            //you can use reflection to get the current (new) value
                            var value = pInfo.PropertyInfo.GetValue(entity, null);

                            //this is part of the serialization engine but you can use it for a single property as well
                            Part21Writer.WriteProperty(pInfo.PropertyInfo.PropertyType, value, valuesLog, null, model.Metadata);
                        }

                        valuesLog.WriteLine();
                    };
                    model.EntityNew += entity =>
                    {
                        //iterate over all properties. These are sorted in the right order already.
                        foreach (var property in entity.ExpressType.Properties.Values)
                        {
                            //overriden attributes have to be treated specially. But these are not present in CobieExpress.
                            if (property.EntityAttribute.State == EntityAttributeState.DerivedOverride)
                                initialLog.Write("*");
                            else
                            {
                                var value = property.PropertyInfo.GetValue(entity, null);
                                Part21Writer.WriteProperty(property.PropertyInfo.PropertyType, value, initialLog, null, model.Metadata);
                            }
                            initialLog.WriteLine();
                        }
                    };
                    using (var txn = model.BeginTransaction("Test"))
                    {
                        var wall = model.Instances.New<IfcWall>();
                        wall.Name = "New name";
                        wall.Description = null;

                        var pset = model.Instances.New<IfcPropertySet>();
                        pset.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>());

                        Assert.IsTrue(changed.SequenceEqual(new[] { "Name", "Description", "HasProperties" }));
                        Assert.AreEqual(valuesLog.ToString(), "'New name'\r\n$\r\n(#3)\r\n");

                        txn.Commit();
                    }
                });
            }
        }
    }
}
