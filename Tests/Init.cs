using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Init
    {
        [AssemblyInitialize]
        public static void InitializeReferencedAssemblies(TestContext context)
        {
            var dummy = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        }
    }
}
