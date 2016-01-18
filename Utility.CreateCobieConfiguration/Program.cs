using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xbim.CobieExpress;
using Xbim.Common.Metadata;
using Xbim.IO.Xml.BsConf;

namespace Utility.CreateCobieConfiguration
{
    internal class Program
    {
        private static void Main()
        {
            //load IFC4 as a template for most of the settings
            var conf = configuration.LoadFromFile(@"..\..\..\Xbim.IO\Xml\BsConf\IFC4_config.xml");

            //change global settings to be COBieExpress specific
            conf.id = "COBieExpress";
            conf.Schema.targetNamespace = "http://www.openbim.org/COBieExpress/1.0";
            conf.Namespace.prefix = "cb";
            conf.Namespace.alias = "http://www.openbim.org/COBieExpress/1.0";
            conf.RootElement.name = "COBie";

            //remove all IFC4 entities
            foreach (var entity in conf.Entities.ToList())
                conf.Items.Remove(entity);

            //remove all IFC4 types
            foreach (var type in conf.Types.ToList())
                conf.Items.Remove(type);

            //add mapping for NUMBER type (<cnf:type select="NUMBER" map="xs:double" />)
            conf.Items.Add(new type{ select = new List<string>{"NUMBER"}, map = "xs:double"});

            //set all COBieExpress non-agregate and non-select attributes to be "exp-attribute="attribute-tag""
            var metadata = ExpressMetaData.GetMetadata(typeof (CobieExternalObject).Module);
            foreach (var expType in metadata.Types())
            {
                var type = expType.Type;
                var instProps = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

                //all explicit non-enumerable own properties
                var expProps =
                    expType.Properties.Where(p => 
                        p.Value.IsExplicit && 
                        p.Value.EnumerableType != null && 
                        instProps.Any(i => i.Name == p.Value.Name))
                        .Select(kv => kv.Value).ToList();
                if(!expProps.Any()) continue;

                var entity = conf.GetOrCreatEntity(expType.ExpressName);
                foreach (var attribute in expProps.Select(prop => entity.GetOrCreateAttribute(prop.Name)))
                {
                    attribute.expattribute = expattribute.attributetag;
                    attribute.expattributeSpecified = true;
                }
            }

            //manual inverse changes
            foreach (var invertedInverse in InvertedInverses)
            {
                var path = invertedInverse.Split('.');
                var entityName = path[0];
                var propName = path[1];

                var expType = metadata.ExpressType(entityName.ToUpperInvariant());
                var expProp = expType.Inverses.First(p => p.Name == propName);
                
                var invType = expProp.PropertyInfo.PropertyType;
                while (invType.IsGenericType)
                    invType = invType.GetGenericArguments()[0];
                var invExpType = metadata.ExpressType(invType);
                var invExpProp =
                    invExpType.Properties.FirstOrDefault(
                        p => p.Value.Name == expProp.InverseAttributeProperty.RemoteProperty).Value;

                var invertedEntity = conf.GetOrCreatEntity(expType.ExpressName);
                var invertedEntityAttr = invertedEntity.GetOrCreateInverse(expProp.Name);
                invertedEntityAttr.expattribute = expattribute.doubletag;
                invertedEntityAttr.expattributeSpecified = true;

                var origEntity = conf.GetOrCreatEntity(invExpType.ExpressName);
                var origAttribute = origEntity.GetOrCreateAttribute(invExpProp.Name);
                origAttribute.keep = false;
            }

            conf.SaveToFile(@"..\..\..\Xbim.IO\Xml\BsConf\COBieExpress_config.xml");
        }

        private static IEnumerable<string> InvertedInverses
        {
            get
            {
                return new List<string>
                {
                    "Asset.AffectedBy",
                    "Facility.Floors",
                    "Facility.Systems",
                    "Floor.Spaces",
                    "Space.Components"
                };
            }
        }
    }
}
