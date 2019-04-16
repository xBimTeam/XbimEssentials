using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO;
using Xunit;

namespace Xbim.Essentials.Tests
{
    public class ModelProviderTests
    {
        [Fact]
        public void Can_Override_EntityFactory()
        {
            // Arrange
            var resolved = false;
            var modelProvider = new HeuristicModelProvider();
            modelProvider.EntityFactoryResolver = (vers) => {
                resolved = true;
                return new MockEntityFactory();
            };

            // Act
            var model = modelProvider.Create(XbimSchemaVersion.Unsupported, XbimStoreType.InMemoryModel);

            Assert.True(resolved);

        }

        [Fact]
        public void ByDefault_Creating_Unsupported_Schema_Throws()
        {
            // Arrange
            var modelProvider = new HeuristicModelProvider();

            Assert.Throws<NotSupportedException>(() => modelProvider.Create(XbimSchemaVersion.Unsupported, XbimStoreType.InMemoryModel) );

        }



        private class MockEntityFactory : IEntityFactory
        {
            public IEnumerable<string> SchemasIds => Enumerable.Empty<string>();

            public XbimSchemaVersion SchemaVersion => throw new NotImplementedException();

            public T New<T>(IModel model, int entityLabel, bool activated) where T : IInstantiableEntity
            {
                throw new NotImplementedException();
            }

            public T New<T>(IModel model, Action<T> init, int entityLabel, bool activated) where T : IInstantiableEntity
            {
                throw new NotImplementedException();
            }

            public IInstantiableEntity New(IModel model, Type t, int entityLabel, bool activated)
            {
                throw new NotImplementedException();
            }

            public IInstantiableEntity New(IModel model, string typeName, int entityLabel, bool activated)
            {
                throw new NotImplementedException();
            }

            public IInstantiableEntity New(IModel model, int typeId, int entityLabel, bool activated)
            {
                throw new NotImplementedException();
            }

            public IExpressValueType New(string typeName)
            {
                throw new NotImplementedException();
            }
        }
    }

}
