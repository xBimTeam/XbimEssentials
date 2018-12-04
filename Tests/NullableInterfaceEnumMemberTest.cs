using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Common.Step21;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class NullableInterfaceEnumMemberTest
    {
        [TestMethod]
        public void NullableEnumMemberTest()
        {
            using (var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var a3 = model.Instances.New<IfcPostalAddress>();
                    var a4 = a3 as IIfcAddress;
                    Assert.IsFalse(a4.Purpose.HasValue);

                    a3.Purpose = Ifc2x3.ActorResource.IfcAddressTypeEnum.OFFICE;
                    Assert.AreEqual(a4.Purpose, Ifc4.Interfaces.IfcAddressTypeEnum.OFFICE);

                    a4.Purpose = null;
                    Assert.IsFalse(a3.Purpose.HasValue);
                }
            }
        }
    }
}
