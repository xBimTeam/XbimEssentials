using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.IO.Xml.BsConf;
using System.Reflection;

namespace Xbim.IO.Xml
{
    internal class XmlMetaProperty
    {
        public bool IsAttributeValue { get { return AttributeSetter != null; } }
        /// <summary>
        /// Delegate to be used to serialize value as an XML attribute. This has to be called
        /// BEFORE any nested elements are created.
        /// </summary>
        public XmlAttributeSetter AttributeSetter { get; private set; }
        
        public ExpressMetaProperty MetaProperty { get; private set; }

        public string Name { get { return MetaProperty.PropertyInfo.Name; } }

        public XmlMetaProperty(ExpressMetaProperty metaProperty)
        {
            MetaProperty = metaProperty;
        }

        public static List<XmlMetaProperty> GetProperties(ExpressType expressType, configuration configuration)
        {
            List<ExpressMetaProperty> properties;
            var typeConfs = configuration.GetEntities(expressType).ToList();
            if (typeConfs.Any())
            {
                //all properties which are not ignored
                properties = expressType.Properties.Values
                    .Where(p => !p.EntityAttribute.IsDerived)
                    .Where(p => typeConfs
                        .All(conf => conf.IgnoredAttributes
                        .All(ia => string.Compare(ia.select, p.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase) != 0)))
                        .ToList();

                //all inverses which are added
                var inverses = expressType.Inverses
                    .Where(i => typeConfs
                        .Any(conf => conf.ChangedInverses
                        .Any(
                            ci =>
                                string.Compare(ci.select, i.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase) == 0)));
                properties.AddRange(inverses);
            }
            else
            {
                //only get explicit properties by default
                properties = expressType.Properties.Values
                    .Where(p => !p.EntityAttribute.IsDerived)
                    .ToList();
            }

            //read configuration and meta property to set the right processing (attribute vs. element)
            var xmlProperties = properties.Select(p => new XmlMetaProperty(p)).ToList();
            foreach (var metaProperty in xmlProperties)
                SetAttributeValueHandler(metaProperty, typeConfs);

            //sort properties so that attribute values go first (value types and some string lists). 
            //They will be written as attributes and that has to
            //happen before any nested elements are written
            var attrProps = xmlProperties.Where(p => p.IsAttributeValue);
            var elemProps = xmlProperties.Where(p => !p.IsAttributeValue).OrderBy(p => p.MetaProperty.EntityAttribute.GlobalOrder);
            return attrProps.Concat(elemProps).ToList();
        }

        public static Type GetNonNullableType(Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
                return type;
            if (!type.GetTypeInfo().IsGenericType)
                return type;
            if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return type;
            return type.GetTypeInfo().GetGenericArguments()[0];

        }

        private static void SetAttributeValueHandler(XmlMetaProperty metaProperty, List<entity> typeConfigurarions)
        {
            //all value types will be serialized as attributes. Nullable<> is also value type which is correct for this.
            var type = metaProperty.MetaProperty.PropertyInfo.PropertyType;
            type = GetNonNullableType(type);
            var propName = metaProperty.MetaProperty.PropertyInfo.Name;
            if (type.GetTypeInfo().IsValueType || type == typeof (string))
            {
                if (typeof (IExpressComplexType).GetTypeInfo().IsAssignableFrom(type))
                {
                    metaProperty.AttributeSetter = (value, writer) =>
                    {
                        if (value == null) return;
                        writer.WriteAttributeString(propName, string.Join(" ", ((IExpressComplexType)value).Properties));
                    };
                }
                else if (type.GetTypeInfo().IsEnum)
                {
                    metaProperty.AttributeSetter = (value, writer) =>
                    {
                        if (value == null) return;
                        writer.WriteAttributeString(propName, (Enum.GetName(type, value) ?? "").ToLowerInvariant());
                    };
                }
                else
                {
                    metaProperty.AttributeSetter = (value, writer) =>
                    {
                        if (value == null) return;
                        writer.WriteAttributeString(propName, value.ToString());
                    };
                }
                return;
            }
            
            //lists of value types will be serialized as lists. If this is not an IEnumerable this is not the case
            if (!typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(type) || !type.GetTypeInfo().IsGenericType)
                return;

            var genType = type.GetTypeInfo().GetGenericArguments()[0];
            if (genType.GetTypeInfo().IsValueType || genType == typeof(string))
            {
                if (IsStringCompatible(genType))
                {
                    //check type configuration
                    if (typeConfigurarions != null &&
                        typeConfigurarions.Any(conf => conf.TaggLessAttributes
                            .Any(a => a.@select == propName)))
                    {
                        metaProperty.AttributeSetter = (value, writer) =>
                        {
                            if (value == null) return;
                            writer.WriteAttributeString(propName, string.Join(" ", ((IEnumerable)value).Cast<object>()));
                        };
                    }
                    //default for string is to be in a separate tags as strings might contain spaces which are
                    //used as a list separators in attribute lists
                    return;
                }
                metaProperty.AttributeSetter = (value, writer) =>
                {
                    if (value == null) return;
                    writer.WriteAttributeString(propName, string.Join(" ", ((IEnumerable)value).Cast<object>()));
                };
                return;
            }

            //rectangular nested lists shall also be serialized as attribute if defined in configuration
            if (typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(genType) && typeConfigurarions != null &&
                typeConfigurarions.Any(conf => conf.TaggLessAttributes
                    .Any(a => a.@select == propName)))
            {
                metaProperty.AttributeSetter = (value, writer) =>
                {
                    if (value == null) return;
                    var enumeration = ((IEnumerable) value).Cast<IEnumerable>().SelectMany(o => o.Cast<object>());
                    writer.WriteAttributeString(propName, string.Join(" ", enumeration));
                };
            }
        }

        public static bool IsStringCompatible(Type type)
        {
            if (type == typeof (string)) return true;

            //try type attributes
            var defTypeAttr = type.GetTypeInfo().GetCustomAttributes(typeof(DefinedTypeAttribute), false).FirstOrDefault() as DefinedTypeAttribute;
            if (defTypeAttr != null && defTypeAttr.UnderlyingType == typeof (string))
                return true;

            return false;
        }

    }

    public delegate void XmlAttributeSetter(object value, XmlWriter writer);
}
