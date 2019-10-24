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
    public class XmlTests2X3
    {
        [TestMethod]
        public void Ifc2X3XMLSerialization()
        {
            const string output = "4walls1floorSite.ifcxml";
            using (var esent = new IO.Esent.EsentModel(new EntityFactoryIfc2x3()))
            {
                string fileName =  Guid.NewGuid() + ".xbim";
                esent.CreateFrom("TestFiles\\4walls1floorSite.ifc", fileName, null, true, true);
                esent.SaveAs(output, StorageType.IfcXml);
                var errs = ValidateIfc2X3(output);
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
            // this is to remove the DeploymentItem tag, above in order to comply with
            // new behaviour for netcore as documented, see
            // https://github.com/microsoft/testfx-docs/blob/master/RFCs/009-DeploymentItem-Attribute.md
            //            
            NeedSchemaFile("IFC2X3.xsd");
            NeedSchemaFile("ex.xsd");

            var errCount = 0;
            var dom = new XmlDocument();
            dom.Load(path);
            var schemas = new XmlSchemaSet();
            schemas.Add("http://www.iai-tech.org/ifcXML/IFC2x3/FINAL", "IFC2X3.xsd");
            schemas.Add("urn:iso.org:standard:10303:part(28):version(2):xmlschema:common", "ex.xsd");
            dom.Schemas = schemas;
            dom.Validate((sender, args) =>
            {
                Debug.WriteLine("Validation error: {0} \nLine: {1}, Position: {2}", args.Message, args.Exception.LineNumber, args.Exception.LinePosition);
                errCount++;
            });

            return errCount;
        }

        private static void NeedSchemaFile(string fileNeeded)
        {
            if (!File.Exists(fileNeeded))
                File.Copy("XsdSchemas\\" + fileNeeded, fileNeeded);
        }
    }
}
