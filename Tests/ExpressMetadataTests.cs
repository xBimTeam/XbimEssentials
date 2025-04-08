﻿using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xunit;

namespace Xbim.Essentials.Tests
{
    public class ExpressMetadataTests
    {
        [InlineData(typeof(Xbim.Common.PersistEntity), 0)]  // Nothing defined in this schema
        [InlineData(typeof(Xbim.Ifc2x3.EntityFactoryIfc2x3), 771)]
        [InlineData(typeof(Xbim.Ifc4.EntityFactoryIfc4), 932)]
        [InlineData(typeof(Xbim.Ifc4.EntityFactoryIfc4x1), 932)]
        [InlineData(typeof(Xbim.Ifc4x3.EntityFactoryIfc4x3Add2), 1007)]
        [Theory]
        [Obsolete]
        public void HasExpectedSchemaTypesByModule(Type moduleType, int expectedTypes)
        {

            var m = moduleType.GetTypeInfo().Module;
            var metaData = ExpressMetaData.GetMetadata(moduleType.Module);
            metaData.Types()
                .Where(t => t.Name != "IfcStrippedOptional")
                .Should().HaveCount(expectedTypes);
        }

        [InlineData(typeof(Xbim.Ifc2x3.EntityFactoryIfc2x3), 771)]
        [InlineData(typeof(Xbim.Ifc4.EntityFactoryIfc4), 932)]
        [InlineData(typeof(Xbim.Ifc4.EntityFactoryIfc4x1), 932)]
        [InlineData(typeof(Xbim.Ifc4x3.EntityFactoryIfc4x3Add2), 1008)]
        [Theory]
        public void HasExpectedSchemaTypesByFactory(Type moduleType, int expectedTypes)
        {

            var factory = Activator.CreateInstance(moduleType) as IEntityFactory;
            factory.Should().NotBeNull();
            var metaData = ExpressMetaData.GetMetadata(factory);
            metaData.Types().Should().HaveCount(expectedTypes);
        }


        [InlineData(typeof(Xbim.Ifc2x3.EntityFactoryIfc2x3))]
        [InlineData(typeof(Xbim.Ifc4.EntityFactoryIfc4))]
        [InlineData(typeof(Xbim.Ifc4.EntityFactoryIfc4x1))]
        [InlineData(typeof(Xbim.Ifc4x3.EntityFactoryIfc4x3Add2))]
        [Theory]
        [Obsolete]
        public void HasSameAcrossMethods(Type moduleType)
        {
            var module = moduleType.GetTypeInfo().Module;
            var factory = Activator.CreateInstance(moduleType) as IEntityFactory;

            var originalMeta = ExpressMetaData.GetMetadata(module);
            var metaData = ExpressMetaData.GetMetadata(factory);

            foreach (var type in originalMeta.Types().Where(t => t.Name != "IfcStrippedOptional"))
            {
                metaData.Types().Select(t => t.Name).Should().Contain(type.Name);
            }

        }

        [Fact]
        public void InvalidExpressTypeHandled()
        {
            var expressType = new ExpressType(typeof(Ifc2x3.ActorResource.IfcActorRole));
            expressType.Should().NotBeNull();


            Action act = () => new ExpressType(typeof(IPersistEntity));

            act.Should().Throw<Exception>().WithMessage("Express Type is not defined for IPersistEntity");

        }
    }
}
