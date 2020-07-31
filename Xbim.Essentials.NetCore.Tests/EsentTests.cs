using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc4;

namespace Xbim.Essentials.NetCore.Tests
{
    [TestClass]
    public class EsentTests
    {
        [TestMethod]
        public void CanCreateEsentModel()
        {
            var esentModel = new Xbim.IO.Esent.EsentModel(new EntityFactoryIfc4());

            Assert.IsNotNull(esentModel);
        }
    }
}
