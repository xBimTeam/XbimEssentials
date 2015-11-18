using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Metadata;

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

        private entity GetEntity(string name)
        {
            return
                    Items
                        .OfType<entity>().FirstOrDefault(e => string.Compare(e.EntityName, name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public IEnumerable<entity> GetEntities(ExpressType type)
        {
            //collect upper inheritance
            var types = new List<ExpressType>{type};
            while (type.SuperType != null)
            {
                types.Add(type.SuperType);
                type = type.SuperType;
            }

            return types.Select(expressType => GetEntity(expressType.ExpressName)).Where(entity => entity != null);

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

        public IEnumerable<attribute> Attributes
        {
            get
            {
                return Items.OfType<attribute>();
            }
        }

        public IEnumerable<attribute> TaggLessAttributes
        {
            get
            {
                return Items.OfType<attribute>().Where(a => a.tagless == "true");
            }
        }

        public string EntityName { get { return select.FirstOrDefault(); } }
    }
}
