using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Essentials.Tests.Utilities;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class SubmittedFiles
    {
        [TestMethod]
        public void TolerateSurfaceStyleWithMultipleColors()
        {
            using (var model = new ModelFactory("SubmittedFiles\\SomeBadIFCSURFACESTYLE.ifc").FirstOrDefault())
            {
                var bad = model.Instances[7946] as IIfcSurfaceStyle;
                var created = XbimTexture.Create(bad);
            }
        }
    }
}
