using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace Xbim.IO.Xml.BsConf
{
    public partial class configuration
    {
        public static configuration LoadIFC4()
        {
            var data = Properties.Resources.IFC4_ADD1_config;
            return Deserialize(data);
        }

        public IEnumerable<entity> ChangedInverses { get
        {
            return
                Items.OfType<entity>()
                    .Where(e => e.ChangedInverses.Any());
        } }  
    }

    public partial class entity
    {
        public IEnumerable<inverse> ChangedInverses
        {
            get
            {
                return
                    Items.OfType<inverse>().Where(i => i.expattribute == expattribute.doubletag);
            }
        }

        public IEnumerable<attribute> IgnoredAttributes
        {
            get
            {
                return Items.OfType<attribute>().Where(i => i.keep == false);
            }
        } 
    }
}
