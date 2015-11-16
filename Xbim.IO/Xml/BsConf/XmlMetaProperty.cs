using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Metadata;

namespace Xbim.IO.Xml.BsConf
{
    class XmlMetaProperty
    {
        public ExpressMetaProperty MetaProperty { get; private set; }

        public XmlMetaProperty(ExpressMetaProperty metaProperty)
        {
            MetaProperty = metaProperty;
        }

        public static List<XmlMetaProperty> GetProperties(ExpressType expressType, configuration configuration)
        {
            List<ExpressMetaProperty> properties;
            var typeConf = configuration.GetEntity(expressType.ExpressName);
            if (typeConf != null)
            {
                //all properties which are not ignored
                properties = expressType.Properties.Values
                    .Where(p => typeConf.IgnoredAttributes
                        .All(ia => string.Compare(ia.select, p.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase) != 0))
                        .ToList();

                //all inverses which are added
                var inverses = expressType.Inverses
                    .Where(i => typeConf.ChangedInverses
                        .Any(
                            ci =>
                                string.Compare(ci.select, i.PropertyInfo.Name, StringComparison.CurrentCultureIgnoreCase) == 0));
                properties.AddRange(inverses);
            }
            else
            {
                //only get explicit properties by default
                properties = expressType.Properties.Values.ToList();
            }

            //read configuration and meta property to set the right processing (attribute vs. element)
            var result = properties.Select(p => new XmlMetaProperty(p)).ToList();


            //sort properties so that attribute values go first (value types and some string lists). 
            //They will be written as attributes and that has to
            //happen before any nested elements are written
            //toWrite = toWrite.OrderByDescending(p => IsAttributeValue(p.PropertyInfo.PropertyType)).ToList();
            return result;
        }

    }
}
