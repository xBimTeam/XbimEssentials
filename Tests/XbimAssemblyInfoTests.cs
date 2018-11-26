using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xbim.Common;
using Xbim.Ifc4.SharedBldgElements;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class XbimAssemblyInfoTests
    {

        [TestMethod]
        public void CanReadAssemblyBuildTime()
        {
            var a = new XbimAssemblyInfo(typeof(IfcWall));
            var dt = a.CompilationTime;
            Assert.AreNotEqual(dt, DateTime.MinValue);
        }

    }
}
