using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Memory;
using Xunit;
using FluentAssertions;

namespace Xbim.Essentials.Tests
{
    public class MemoryModelTests
    {
        [Theory]
        [InlineData(@"IFC4X3_RC2")]
        [InlineData(@"IFC4")]
        [InlineData(@"Ifc4RC")]
        public void Should_Return_Correct_IFC4_Schema(string schema)
        {
            //Arrange
            List<string> schemas = new List<string>() {schema};
            //Act
            var sutschema = MemoryModel.GetStepFileXbimSchemaVersion(schemas);
            //Assert
            sutschema.Should().Be(XbimSchemaVersion.Ifc4);
        }
        
        [Theory]
        [InlineData(@"Ifc4x1")]
        public void Should_Return_Correct_IFC4X1_Schema(string schema)
        {
            //Arrange
            List<string> schemas = new List<string>() {schema};
            //Act
            var sutschema = MemoryModel.GetStepFileXbimSchemaVersion(schemas);
            //Assert
            sutschema.Should().Be(XbimSchemaVersion.Ifc4x1);
        }
        
        [Theory]
        [InlineData(@"Ifc2x3_RC2")]
        public void Should_Return_Correct_Ifc2x_Schema(string schema)
        {
            //Arrange
            List<string> schemas = new List<string>() {schema};
            //Act
            var sutschema = MemoryModel.GetStepFileXbimSchemaVersion(schemas);
            //Assert
            sutschema.Should().Be(XbimSchemaVersion.Ifc2X3);
        }
        
        [Theory]
        [InlineData(@"Cobie2X4RC2")]
        public void Should_Return_Correct_Cobie2X4_Schema(string schema)
        {
            //Arrange
            List<string> schemas = new List<string>() {"Cobie2X4RC2"};
            //Act
            var sutschema = MemoryModel.GetStepFileXbimSchemaVersion(schemas);
            //Assert
            sutschema.Should().Be(XbimSchemaVersion.Cobie2X4);
        }
    }
}