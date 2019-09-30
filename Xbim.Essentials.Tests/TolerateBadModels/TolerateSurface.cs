using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Essentials.Tests.TolerateBadModels
{
    [TestClass]
    public class TolerateBadModels
    {
        [TestMethod]
        [DeploymentItem(@"BadModels\SomeBadIFCSURFACESTYLE.ifc")]
        public void TolerateBadSurfaceStyle()
        {
            using (var s = IfcStore.Open("SomeBadIFCSURFACESTYLE.ifc"))
            {
                var bad = s.Instances[7946] as IIfcSurfaceStyle;
                var created = XbimTexture.Create(bad);
            }
        }
    }
}
