using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xbim.Common.Metadata;

// ReSharper disable InconsistentNaming

namespace Xbim.IO.Xml.BsConf
{
    public partial class configuration
    {
        private static string GetData(string name)
        {
            var assembly = typeof(configuration).Assembly;
            var resourceName = $"Xbim.IO.MemoryModel.Xml.BsConf.{name}.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static configuration IFC4Add2
        {
            get
            {
                var data = GetData("IFC4_ADD2_config");
                return Deserialize(data);
            }
        }

        public static configuration IFC4Add1
        {
            get
            {
                var data = GetData("IFC4_ADD1_config");
                return Deserialize(data);
            }
        }

        public static configuration IFC4
        {
            get
            {
                var data = GetData("IFC4_config");
                return Deserialize(data);
            }
        }

        public option Option { get { return Items?.OfType<option>().FirstOrDefault(); } }

        public schema Schema { get { return Items?.OfType<schema>().FirstOrDefault(); } }

        public uosElement RootElement { get { return Items?.OfType<uosElement>().FirstOrDefault(); } }

        public IEnumerable<entity> Entities { get { return Items?.OfType<entity>(); } }

        public IEnumerable<type> Types { get { return Items?.OfType<type>(); } }

        public @namespace Namespace { get { return Schema?.Items.OfType<@namespace>().FirstOrDefault(); } }

        public IEnumerable<entity> ChangedInverses { get
        {
            return
                Items == null ? 
                Enumerable.Empty<entity>() :
                Items.OfType<entity>()
                    .Where(e => e.ChangedInverses.Any());
        } }

        public IEnumerable<entity> IgnoredAttributes
        {
            get
            {
                return
                    Items == null ?
                Enumerable.Empty<entity>() :
                Items.OfType<entity>()
                        .Where(e => e.IgnoredAttributes.Any());
            }
        }

        private entity GetEntity(string name)
        {
            return Items?.OfType<entity>().FirstOrDefault(e => string.Compare(e.EntityName, name, StringComparison.OrdinalIgnoreCase) == 0);
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

        public entity GetOrCreatEntity(string name)
        {
            if(Items == null) Items = new List<object>();

            var entity = Items.OfType<entity>()
                .FirstOrDefault(e => e.select != null && e.select.FirstOrDefault() == name);
            if (entity != null)
                return entity;

            entity = new entity { _select = name };
            Items.Add(entity);
            return entity;
        }


    }

    public partial class entity
    {
        public IEnumerable<inverse> ChangedInverses
        {
            get
            {
                return Items == null ?
                Enumerable.Empty<inverse>() :
                    Items.OfType<inverse>().Where(i => 
                        i.expattribute == expattribute.doubletag || 
                        i.expattribute == expattribute.attributetag);
            }
        }

        public IEnumerable<attribute> IgnoredAttributes
        {
            get
            {
                return Items == null ?
                Enumerable.Empty<attribute>() :
                Items.OfType<attribute>().Where(i => i.keep == false);
            }
        }

        public IEnumerable<attribute> Attributes
        {
            get
            {
                return Items == null ?
                Enumerable.Empty<attribute>() :
                Items.OfType<attribute>();
            }
        }

        public IEnumerable<attribute> TaggLessAttributes
        {
            get
            {
                return Items == null ?
                Enumerable.Empty<attribute>() :
                Items.OfType<attribute>().Where(a => a.tagless == "true");
            }
        }

        public string EntityName { get { return select?.FirstOrDefault(); } }

        public attribute GetOrCreateAttribute(string attributeName)
        {
            if (Items == null) Items = new List<object>();

            var attr = Items.OfType<attribute>().FirstOrDefault(a => a.select == attributeName);
            if (attr != null) return attr;

            attr = new attribute {select = attributeName};
            Items.Add(attr);
            return attr;
        }

        public inverse GetOrCreateInverse(string attributeName)
        {
            if (Items == null) Items = new List<object>();

            var attr = Items.OfType<inverse>().FirstOrDefault(a => a.select == attributeName);
            if (attr != null) return attr;

            attr = new inverse { select = attributeName };
            Items.Add(attr);
            return attr;
        }
    }
}
