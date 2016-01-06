namespace Xbim.IO.Xml
{
    public class XbimXmlSettings
    {
        public string Namespace { get; set; }
        public string NamespacePrefix { get; set; }
        public string NamespaceLocation { get; set; }
        public bool Indent { get; set; }
        public string ExpressUri { get; set; }
        public string Configuration { get; set; }
        public string RootName { get; set; }

        public static XbimXmlSettings IFC4
        {
            get
            {
                return new XbimXmlSettings
                {
                    Namespace = "http://www.buildingsmart-tech.org/ifcXML/IFC4/final",
                    NamespacePrefix = "ifc",
                    NamespaceLocation = "http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd",
                    ExpressUri = "http://www.buildingsmart-tech.org/ifc/IFC4/final/IFC4.exp",
                    Configuration = "http://www.buildingsmart-tech.org/ifcXML/IFC4/final/config/ifcXML4_config.xml",
                    RootName = "ifcXML",
                    Indent = true
                };
            }
        }

        public static XbimXmlSettings IFC4Add1
        {
            get
            {
                return new XbimXmlSettings
                {
                    Namespace = "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1", //"http://www.buildingsmart-tech.org/ifcXML/MVD4/IFC4",
                    NamespacePrefix = "ifc",
                    NamespaceLocation = "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1/IFC4_ADD1.xsd",
                    ExpressUri = "http://www.buildingsmart-tech.org/ifc/IFC4/Add1/IFC4_ADD1.exp",
                    Configuration = "http://www.buildingsmart-tech.org/ifcXML/IFC4/Add1/IFC4_ADD1_config.xml",
                    RootName = "ifcXML",
                    Indent = true
                };
            }
        }
    }
}
