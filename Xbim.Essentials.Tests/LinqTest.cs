using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// The code in this class is inspired by a bug in Exchange, where it raised a doubt.
    /// </summary>
    [TestClass]
    public class LinqTest
    {
        [DeploymentItem(@"TestSourceFiles\email.ifc")]
        [TestMethod]
        public void TestingLinqWhere2()
        {
            using (var model1 = MemoryModel.OpenRead("email.ifc"))
            {
                
                var telecom = model1.Instances[28] as IIfcTelecomAddress;
                Assert.IsNotNull(telecom);
                Assert.IsNotNull(telecom.ElectronicMailAddresses);

                // the linq queries below should return the same value, 
                // indeed resharper suggest to transform one into the other
                // but the values returned are different.
                // 
                // Is this a problem with linq or Essentials?
                //
                // the problem goes away if we use "using Xbim.Ifc2x3.Interfaces;" instead of ifc4.
                //
                var add = telecom.ElectronicMailAddresses;

                var enum1 = add.Where(t => !string.IsNullOrWhiteSpace(t.ToString()));
                var enumFrom1 = enum1.FirstOrDefault();

                var ml1 = add.Where(t => !string.IsNullOrWhiteSpace(t.ToString())).FirstOrDefault();
                var ml2 = add.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.ToString()));
                
                Assert.AreEqual(ml1, ml2);
            }
        }
    }
}
