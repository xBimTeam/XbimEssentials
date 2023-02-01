using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Configuration;
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

            XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.UseHeuristicModel()));

        }


        [TestMethod]
        public void IsSetup()
        {
            var provider = IfcStore.ModelProviderFactory.CreateProvider();
            Assert.IsInstanceOfType(provider, typeof(HeuristicModelProvider));
        }
    }
}
