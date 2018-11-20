using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class EditModelTests
    {
		[TestMethod]
		public void BeginTransactionMustNotErasedHeaderEsentTest()
		{
			using (var model = IfcStore.Create(XbimSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
			{
				model.Header.FileDescription.Description.Add("ViewDefinition [TestView]");
				model.Header.FileName = new StepFileName();
				model.Header.FileName.Name = "TestingHeaderNotEmpty";

				using (var txn = model.BeginTransaction())
				{
					txn.Commit();
				}
				
				Assert.AreEqual(model.Header.FileName.Name, "TestingHeaderNotEmpty");
			}

			using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
			{
				model.Header.FileDescription.Description.Add("ViewDefinition [TestView]");
				model.Header.FileName = new StepFileName();
				model.Header.FileName.Name = "TestingHeaderNotEmpty";

				using (var txn = model.BeginTransaction())
				{
					txn.Commit();
				}
				
				Assert.AreEqual(model.Header.FileName.Name, "TestingHeaderNotEmpty");
			}

			using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4x1, XbimStoreType.EsentDatabase))
			{
				model.Header.FileDescription.Description.Add("ViewDefinition [TestView]");
				model.Header.FileName = new StepFileName();
				model.Header.FileName.Name = "TestingHeaderNotEmpty";

				using (var txn = model.BeginTransaction())
				{
					txn.Commit();
				}
				
				Assert.AreEqual(model.Header.FileName.Name, "TestingHeaderNotEmpty");
			}
		}

		[TestMethod]
		public void BeginTransactionMustNotErasedHeaderMemoryTest()
		{
			using (var model = IfcStore.Create(XbimSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
			{
				model.Header.FileDescription.Description.Add("ViewDefinition [TestView]");
				model.Header.FileName = new StepFileName();
				model.Header.FileName.Name = "TestingHeaderNotEmpty";

				using (var txn = model.BeginTransaction())
				{
					txn.Commit();
				}
				
				Assert.AreEqual(model.Header.FileName.Name, "TestingHeaderNotEmpty");
			}

			using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
			{
				model.Header.FileDescription.Description.Add("ViewDefinition [TestView]");
				model.Header.FileName = new StepFileName();
				model.Header.FileName.Name = "TestingHeaderNotEmpty";

				using (var txn = model.BeginTransaction())
				{
					txn.Commit();
				}
				
				Assert.AreEqual(model.Header.FileName.Name, "TestingHeaderNotEmpty");
			}

			using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4x1, XbimStoreType.InMemoryModel))
			{
				model.Header.FileDescription.Description.Add("ViewDefinition [TestView]");
				model.Header.FileName = new StepFileName();
				model.Header.FileName.Name = "TestingHeaderNotEmpty";

				using (var txn = model.BeginTransaction())
				{
					txn.Commit();
				}
				
				Assert.AreEqual(model.Header.FileName.Name, "TestingHeaderNotEmpty");
			}
		}
	}
}
