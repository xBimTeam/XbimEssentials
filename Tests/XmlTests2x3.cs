using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;
using Xbim.IO.Xml;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    public class XmlTests2X3
    {
        private static readonly NetworkConnection Network = new NetworkConnection();

        [TestMethod]
        public void Ifc2X3XMLSerialization()
        {
            // if there's no network a message is asserted, but then this test passes 
            // to prevent concerns when testing the solution offline (which would appear to fail)
            //
            if (!Network.Available)
                return;

            const string output = "..\\..\\4walls1floorSite.ifcxml";
            using (var esent = new IO.Esent.EsentModel(new EntityFactoryIfc2x3()))
            {
                string fileName =  Guid.NewGuid() + ".xbim";
                esent.CreateFrom("4walls1floorSite.ifc", fileName, null, true, true);
                esent.SaveAs(output, StorageType.IfcXml);
                var errs = ValidateIfc2X3("..\\..\\4walls1floorSite.ifcxml");
                Assert.AreEqual(0, errs);
                esent.Close();
            }

            using (var esent = new IO.Esent.EsentModel(new EntityFactoryIfc2x3()))
            {
                string fileName =  Guid.NewGuid() + ".xbim";
                var success = esent.CreateFrom(output, fileName, null, true, true);
                Assert.IsTrue(success);
                Assert.AreEqual(4, esent.Instances.CountOf<IfcWall>());
                esent.Close();
            }

            //check version info
            using (var file = File.OpenRead(output))
            {
                var header = XbimXmlReader4.ReadHeader(file);
                Assert.AreEqual("IFC2X3", header.SchemaVersion);
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="path">Path of the file to be validated</param>
        /// <returns>Number of errors</returns>
        private int ValidateIfc2X3(string path)
        {
            var errCount = 0;
            var dom = new XmlDocument();
            dom.Load(path);
            var schemas = new XmlSchemaSet();
            schemas.Add("http://www.iai-tech.org/ifcXML/IFC2x3/FINAL", "http://www.iai-tech.org/ifcXML/IFC2x3/FINAL/IFC2X3.xsd");
            schemas.Add("urn:iso.org:standard:10303:part(28):version(2):xmlschema:common","http://www.iai-tech.org/ifcXML/IFC2x3/FINAL/ex.xsd");
            dom.Schemas = schemas;
            dom.Validate((sender, args) =>
            {
                Debug.WriteLine("Validation error: {0} \nLine: {1}, Position: {2}", args.Message, args.Exception.LineNumber, args.Exception.LinePosition);
                errCount++;
            });

            return errCount;
        }
    }
}
