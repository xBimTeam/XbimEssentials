using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using Xbim.Ifc4;
using Xbim.Ifc4.ActorResource;
using Xbim.IO.Memory;

namespace Xbim.Essentials.NetCore.Tests
{
    [TestClass]
    public class XmlSerializationTests
    {
        [TestMethod]
        public void SerializeXML()
        {
            using (var model = new MemoryModel(new EntityFactoryIfc4()))
            {
                using (var txn = model.BeginTransaction("Create"))
                {
                    model.Instances.New<IfcPerson>(p => p.GivenName = "Martin");
                    txn.Commit();
                }
                using (var output = File.Create("xml.ifcxml"))
                {
                    model.SaveAsXml(output, new XmlWriterSettings { Indent = true, IndentChars = "  "});
                }
            }
        }
    }
}
