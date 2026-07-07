using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Configuration;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Esent;
using Xunit;

namespace Xbim.Essentials.NetCore.Tests
{
    [CollectionDefinition(nameof(ValidationTests), DisableParallelization = true)]
    public class ValidationTests
    {
        private XbimServices configuration;

        public ValidationTests()
        {
            // Clear the singleton collection each test
            configuration = XbimServices.CreateInstanceInternal();
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1)]
        public void ShouldValidateOnEsentModel(double threshold)
        {
            // Configure on the local instance, not the global singleton
            configuration.ConfigureServices(services => services.AddXbimToolkit(opt => opt.AddEsentModel()));
            var provider = configuration.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();

            // Arrange
            var model = IfcStore.Open("TestFiles/CPM.ifc", null, threshold);
            Validator validator = new Validator()
            {
                ValidateLevel = Common.Enumerations.ValidationFlags.EntityWhereClauses
            };

            // Act
            List<ValidationResult> tot = [];
            foreach (var item in model.Instances)
            {
                var t = validator.Validate(item).ToList();
                tot.AddRange(t);
            }
            // Assert
            tot.Should().BeEmpty();
        }
    }
}
