using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace Xbim.IO.Xml.BsConf
{
    public partial class configuration
    {
        public static configuration IFC4Add1
        {
            get
            {
                var data = Properties.Resources.IFC4_ADD1_config;
                return Deserialize(data);    
            }
        }

        public static configuration IFC4
        {
            get
            {
                var data = Properties.Resources.IFC4_config;
                return Deserialize(data);
            }
        }

        public IEnumerable<entity> ChangedInverses { get
        {
            return
                Items.OfType<entity>()
                    .Where(e => e.ChangedInverses.Any());
        } }

        public IEnumerable<entity> IgnoredAttributes
        {
            get
            {
                return
                    Items.OfType<entity>()
                        .Where(e => e.IgnoredAttributes.Any());
            }
        }

        public entity GetEntity(string name)
        {
            return
                    Items
                        .OfType<entity>().FirstOrDefault(e => string.Compare(e.EntityName, name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
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

        public string EntityName { get { return select.FirstOrDefault(); } }
    }
}
