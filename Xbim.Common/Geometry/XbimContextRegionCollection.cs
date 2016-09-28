using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public class XbimContextRegionCollection : List<XbimRegionCollection>
    {
        XbimRegionCollection MostPopulated()
        {
            var mostPopulated = new XbimRegionCollection();
            //take the most populated region from each context
            foreach (var regionColl in this)
            {
                var mp = regionColl.MostPopulated();
                if (mp != null) mostPopulated.Add(mp);
            }
            return mostPopulated;
        }
    }
}
