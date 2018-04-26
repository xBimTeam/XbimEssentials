using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;

namespace Xbim.IO.Tests
{
	[TestClass]
	public class ReadingWronglyEncodedFile
	{
		[TestMethod]
		[DeploymentItem("TestFiles\\Wrongly1251Encoded.ifc")]
		public void WhenOpeningFile_UsesCodePageOverride()
		{
			// "Wrongly1251Encoded.ifc" - the file with IfcProject.Name containing a one-byte-string encoded using the 
			// Windows-1251 encoding instead of the ISO-8859-1 encoding.
			using (var model = IfcStore.Open("Wrongly1251Encoded.ifc", null, 0, null, XbimDBAccess.Read, 1251)) {
				var project = model.Instances.OfType<IIfcProject>().Single();
				Assert.AreEqual("дом", project.Name.ToString());
			}
		}

		[TestMethod]
		[DeploymentItem("TestFiles\\Wrongly1251Encoded.ifc")]
		public void WhenOpeningFile_UsesDefaultCodePage_IfNoOverrideIsSpecified()
		{
			using (var model = IfcStore.Open("Wrongly1251Encoded.ifc", null, 0)) {
				var project = model.Instances.OfType<IIfcProject>().Single();

				// Windows-1251 string 'дом' encoded using the default IFC encoding (ISO-8859-1).
				string correctString = "äîì";

				Assert.AreEqual(correctString, project.Name.ToString());
			}
		}
	}
}
