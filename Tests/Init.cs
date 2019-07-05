using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Init
    {
        [AssemblyInitialize]
        public static void InitializeReferencedAssemblies(TestContext context)
        {
            var dummy = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            IfcStore.ModelProviderFactory.Use(() => new HeuristicModelProvider());
        }


        [TestMethod]
        public void IsSetup()
        {
            var provider = IfcStore.ModelProviderFactory.CreateProvider();
            Assert.IsInstanceOfType(provider, typeof(HeuristicModelProvider));
        }
    }
}
