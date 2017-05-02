using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
// using Xbim.Ifc2x3.Interfaces; // replacing ifc2x3 would make test pass.

namespace Tests
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
            using (var model1 = IfcStore.Open("email.ifc"))
            {
                var p = model1.Instances[1] as IIfcPerson;
                Assert.IsNotNull(p);
                var telecom = p.Addresses.OfType<IIfcTelecomAddress>().FirstOrDefault();
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
                var ml1 = telecom.ElectronicMailAddresses.Where(t => !string.IsNullOrWhiteSpace(t.ToString())).FirstOrDefault();
                var ml2 = telecom.ElectronicMailAddresses.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.ToString()));
                Assert.AreEqual(ml1, ml2);
            }
        }
    }
}
