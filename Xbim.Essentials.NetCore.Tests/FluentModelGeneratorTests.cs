using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common.Exceptions;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc.Fluent;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;
using Xunit;


namespace Xbim.Essentials.NetCore.Tests
{
    public class FluentModelGeneratorTests :IDisposable
    {
        // Latest file
        public string IfcFile { get => files.Peek(); }

        // Stack of files to clean up in Dispose
        public Stack<string> files = new();

        static readonly Guid BaseGuid = new Guid("5F7546A1-00E8-44E9-A2A9-0FD1EB87EFE9");

        public FluentModelGeneratorTests()
        {
            NewTestFile();
        }

        private void NewTestFile(string file = null)
        {
            file ??= Path.ChangeExtension(Path.GetTempFileName(), "ifc");
            files.Push(file);
        }

        [Fact]
        public void NonFluentTest()
        {
            // How we'd normally build a model, with some helpers to populate default attribs (GlobalID etc)
            using var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3());
            using var trans = model.BeginTransaction("Create");

            model.AddHeaders(IfcFile);
      
            var type = model.Build().WallType().WithDefaults();

            var instance = model.Build().Wall().WithDefaults(t => t with { PredefinedType = "X" });
            instance.AddDefiningType(type).WithDefaults();

            trans.Commit();
            using var sw = new FileStream(IfcFile, FileMode.Create);
            model.SaveAsIfc(sw);
        }

        [Fact]
        public void FluentEquivalentTest()
        {
            new FluentModelBuilder()
                .CreateModel(XbimSchemaVersion.Ifc2X3)
                .CreateEntities(cfg =>
                {
                    var type = cfg.WallType();
                    cfg.Wall().WithDefaults(t => t with { PredefinedType = "X" })
                        .AddDefiningType(type);
                })
                .SaveAsIfc(IfcFile);
        }

        [Fact]
        public void FluentBuilderTest()
        {
            var builder = new FluentModelBuilder();
            XbimEditorCredentials editor = GetEditor();

            NewTestFile("wall.ifc");
            builder.AssignEditor(editor)
                .UseStableGuids(BaseGuid)
                .UseStableDateTime(new DateTime(2025, 1, 1));
            builder
                .CreateModel(XbimSchemaVersion.Ifc2X3)
                .SetHeaders()
                .SetOwnerHistory()
                .CreateEntities(cfg =>
                {
                    var type = cfg.WallType(o => o.WithDefaults(t => t with { Name = "Block"}));
                    cfg.Wall(o => o.WithDefaults(t => t with { PredefinedType = "XYZ" }))
                        .AddDefiningType(type);
                })
                .AssertValid()
                .SaveAsIfc(IfcFile);

            NewTestFile("sensor.ifc");
            builder.CreateModel(XbimSchemaVersion.Ifc4)
                .SetHeaders()
                .SetOwnerHistory()
                .CreateEntities((factory, ctx) =>
                {
                    var type = factory.SensorType(o => o.WithDefaults(t => t with { Name = "Light Sensor", PredefinedType= "LIGHTSENSOR" }));
                    // using context for more precise control. (Builder doesn't support IFC4+ types)
                    ctx.Instances.New<Ifc4.BuildingControlsDomain.IfcSensor>()
                        .AddDefiningType(type);
                })
                .AssertValid()
                .SaveAsIfc(IfcFile);
        }

        [Fact]
        public void ShouldPopulateAttributes_Automatically()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Door());

            var door = file.Model.Instances.OfType<IIfcDoor>().First();

            door.GlobalId.Should().NotBeNull();
        }

        [Fact]
        public void Can_CreateStable_Guids()
        {
            var builder = new FluentModelBuilder();

            Guid baseGuid = new Guid("05BC61EB-C2F1-47C5-93CE-BF931D7A8301");

            var file = builder
                .UseStableGuids(baseGuid)
                .CreateModel()
                .CreateEntities(c =>
                {
                    c.Door();
                    c.Window();
                    c.Wall();
                });

            var door = file.Model.Instances.OfType<IIfcDoor>().First();
            var window = file.Model.Instances.OfType<IIfcWindow>().First();
            var wall = file.Model.Instances.OfType<IIfcWall>().First();

            
            door.GlobalId.ToString().Should().Be(  "05l67hml57nG0000000001");
            window.GlobalId.ToString().Should().Be("05l67hml57nG0000000002");
            wall.GlobalId.ToString().Should().Be("05l67hml57nG0000000003");
        }

        [Fact]
        public void Can_CreateStable_DateStamps()
        {
            var builder = new FluentModelBuilder();
            var baseDate = new DateTime(2025, 04, 01);

            var file = builder
                .UseStableDateTime(baseDate)
                .CreateModel()
                .SetHeaders()
                .SetOwnerHistory(GetEditor())
                ;

            file.Model.Header.TimeStamp.Should().Be("2025-04-01T00:00:00");
            var owner = file.Model.Instances.OfType<IIfcOwnerHistory>().First();
            owner.LastModifiedDate.Value.ToDateTime().Should().Be(baseDate);
            owner.CreationDate.ToDateTime().Should().Be(baseDate);
        }

        [InlineData(XbimSchemaVersion.Ifc2X3)]
        [InlineData(XbimSchemaVersion.Ifc4)]
        [InlineData(XbimSchemaVersion.Ifc4x3)]
        [Theory]
        public void ShouldDefaultPredefinedTypesConsistently(XbimSchemaVersion schema)
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel(schema)
                .CreateEntities(c => c.WallType());

            var beam = file.Model.Instances.OfType<IIfcWallType>().First();
            beam.GetPredefinedTypeValue().Should().Be("NOTDEFINED", "We override the default which is essentially random");
        }

        [Fact]
        public void Should_AllowAttributes_To_Be_Set()
        {
            var builder = new FluentModelBuilder();
            
            var file = builder.CreateModel()
                .CreateEntities(c => c.Beam().WithDefaults(a => a with { Name = "A", Description="B" }));

            var beam = file.Model.Instances.OfType<IIfcBeam>().First();

            beam.Name.ToString().Should().Be("A");
            beam.Description.ToString().Should().Be("B");
            beam.OwnerHistory.Should().BeNull("Not set explicitly");
        }

        [Fact]
        public void Should_Allow_PredefinedType_To_Be_Sets()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Pile().WithDefaults(a => a with { PredefinedType = "COHESION" }));

            var pile = file.Model.Instances.OfType<IIfcPile>().First();

            pile.PredefinedType.Should().Be(IfcPileTypeEnum.COHESION);
        }


        [Fact]
        public void Should_Allow_ObjectType_To_Be_Set_Implicitly()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Pile().WithDefaults(a => a with { PredefinedType = "Precast123" }));

            var pile = file.Model.Instances.OfType<IIfcPile>().First();

            pile.ObjectType.ToString().Should().Be("Precast123");
            pile.PredefinedType.Should().Be(IfcPileTypeEnum.USERDEFINED);
        }

        [Fact]
        public void Should_Allow_ObjectType_To_Be_Set_Implicitly_Without_PDT()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Wall().WithDefaults(a => a with { PredefinedType = "Partition" }));

            var wall = file.Model.Instances.OfType<IIfcWall>().First();

            wall.ObjectType.ToString().Should().Be("Partition");
            wall.PredefinedType.Should().BeNull("IfcWall.PredefinedType doesn't exist in 2x3");
        }


        [Fact]
        public void Should_Allow_PredefinedType_To_Be_Set_ForTypes()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.BeamType().WithDefaults(a => a with { PredefinedType = "Joist" }));

            var beam = file.Model.Instances.OfType<IIfcBeamType>().First();

            beam.PredefinedType.Should().Be(IfcBeamTypeEnum.JOIST);
        }

        [Fact]
        public void Should_Allow_ElementType_To_Be_Set_Implicitly()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.BeamType().WithDefaults(a => a with { PredefinedType = "I-Beam123" }));

            var beam = file.Model.Instances.OfType<IIfcBeamType>().First();

            beam.ElementType.ToString().Should().Be("I-Beam123");
            beam.PredefinedType.Should().Be(IfcBeamTypeEnum.USERDEFINED);
        }

        [Fact]
        public void Can_Set_OwnerHistory_OnModel()
        {
            var builder = new FluentModelBuilder();
            var editor = GetEditor();

            var file = builder.CreateModel()
                .SetOwnerHistory(editor)
                .CreateEntities(c => c.BeamType());

            var beam = file.Model.Instances.OfType<IIfcBeamType>().First();

            beam.OwnerHistory.Should().NotBeNull();
            beam.OwnerHistory.ChangeAction.Should().Be(IfcChangeActionEnum.ADDED);
            beam.OwnerHistory.OwningUser.ThePerson.GivenName.ToString().Should().Be("John");
            beam.OwnerHistory.OwningUser.TheOrganization.Name.ToString().Should().Be("Acme");
            beam.OwnerHistory.OwningApplication.ApplicationFullName.ToString().Should().Be("Sample app");
        }

        [Fact]
        public void Can_Set_OwnerHistory_Globally()
        {
            var builder = new FluentModelBuilder();
            var editor = GetEditor();

            var file = builder
                .AssignEditor(editor)   // Set once, each model inherits
                .CreateModel()
                .SetOwnerHistory()
                .CreateEntities(c => c.Wall());

            var item1 = file.Model.Instances.OfType<IIfcWall>().First();

            item1.OwnerHistory.Should().NotBeNull();
            item1.OwnerHistory.ChangeAction.Should().Be(IfcChangeActionEnum.ADDED);
            item1.OwnerHistory.OwningUser.ThePerson.GivenName.ToString().Should().Be("John");

            file.Discard(); // Create next File
            file = builder
                // Editor still in place
                .CreateModel()
                .SetOwnerHistory()
                .CreateEntities(c => c.Window());

            var item2 = file.Model.Instances.OfType<IIfcWindow>().First();

            item2.OwnerHistory.Should().NotBeNull();
            item2.OwnerHistory.ChangeAction.Should().Be(IfcChangeActionEnum.ADDED);
            item2.OwnerHistory.OwningUser.ThePerson.GivenName.ToString().Should().Be("John");
        }


        [Fact]
        public void Saving_Sets_FileName()
        {
            var builder = new FluentModelBuilder();
            var editor = GetEditor();

            builder.CreateModel()
                .SetOwnerHistory(editor)
                .CreateEntities(c => c.BeamType())
                .SaveAsIfc(IfcFile);

            var model = MemoryModel.OpenRead(IfcFile);
            model.Header.FileName.Name.Should().Be(IfcFile);
        }

        [Fact]
        public void ModelTransaction_Disposed_After_Saving()
        {
            var builder = new FluentModelBuilder();
            var editor = GetEditor();

            var fileBuilder = builder.CreateModel()
                .SetOwnerHistory(editor)
                .CreateEntities(c => c.BeamType())
                .SaveAsIfc(IfcFile);

            fileBuilder.Model.CurrentTransaction.Should().BeNull();

            var ex = Record.Exception(() => fileBuilder.Model.Instances.New<Ifc2x3.SharedBldgElements.IfcBeam>());

            ex.Should().NotBeNull().And.BeOfType<Exception>();
            ex.Message.Should().Be("Operation out of transaction");
        }

        [Fact]
        public void Can_Keep_Model_open_after_saving()
        {
            var builder = new FluentModelBuilder();
            var editor = GetEditor();

            var fileBuilder = builder.CreateModel()
                .SetOwnerHistory(editor)
                .CreateEntities(c => c.BeamType().WithDefaults(t => t with { Name = "A"}))
                .SaveAsIfc(IfcFile, keepOpen: true);

            NewTestFile();
            fileBuilder
                .CreateEntities((c, ctx) =>
                {
                    var beam = ctx.Instances.OfType<IIfcBeamType>().First();
                    beam.SetAttributes(t => t with { Name = "B" });
                    // Or 
                    // beam.Name = "B";
                })
                .SaveAsIfc(IfcFile);

            var model = MemoryModel.OpenRead(IfcFile);

            var beamType = model.Instances.OfType<IIfcBeamType>().First();
            beamType.Name.Value.Value.Should().Be("B");

        }

        [Fact]
        public void Can_Set_Headers_from_Editor()
        {
            var builder = new FluentModelBuilder();
            var editor = GetEditor();

            builder
                .AssignEditor(editor)
                .CreateModel()
                .SetHeaders()
                .CreateEntities(c => c.BeamType())
                .SaveAsIfc(IfcFile);
            var dateNow = DateTime.UtcNow;

            var model = MemoryModel.OpenRead(IfcFile);
            model.Header.FileName.AuthorName.Should().HaveCount(1).And.BeEquivalentTo(new[] { "John Doe" });
            model.Header.FileName.Organization.Should().HaveCount(1).And.BeEquivalentTo(new[] { "Acme" });

            model.Header.FileName.OriginatingSystem.Should().Be("Sample app 1.2.3-alpha");
            model.Header.FileName.PreprocessorVersion.Should().StartWith("xbim Toolkit v");
            model.Header.FileName.TimeStamp.Should().StartWith(dateNow.ToString("yyyy-MM-ddTHH"));
        }

        [Fact]
        public void Can_Assert_Model_Validity()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                // Don't set Owner History
               // .SetOwnerHistory(GetEditor())
                .CreateEntities(c => c.BeamType());

            var ex = Record.Exception(() => file.AssertValid());

            ex.Should().NotBeNull().And.BeOfType<XbimException>();
            ex.Message.Should().StartWith("Model has 2 validation error(s):");

        }

        [Fact]
        public void Can_ViewValidation_Results()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                // Don't set Owner History
                //.SetOwnerHistory(GetEditor())
                .CreateEntities(c => c.BeamType());

            var results = file.ValidateIfc(Common.Enumerations.ValidationFlags.Properties);

            results.Should().HaveCount(1).And.Satisfy(v => v.IssueSource == "OwnerHistory");
        }

        [Fact]
        public void Can_AddPropertySet()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Wall().WithPropertySet("Pset_WallCommon"));

            var wall = file.Model.Instances.OfType<IIfcWall>().First();

            var pset = wall.GetPropertySet("Pset_WallCommon");

            pset.Should().NotBeNull();
            pset.HasProperties.Should().BeEmpty();

        }

        [Fact]
        public void Can_SetTextProperties()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Wall().WithPropertySingle("Pset_WallCommon", "Reference", new Ifc4.MeasureResource.IfcText("Test")));

            var wall = file.Model.Instances.OfType<IIfcWall>().First();

            var prop = wall.GetPropertySingleValue("Pset_WallCommon", "Reference");

            prop.Should().NotBeNull();
            prop.NominalValue.Value.Should().Be("Test");   
        }

        [Fact]
        public void Can_SetBooleanProperties()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Wall().WithPropertySingle("Pset_WallCommon", "IsExternal", new Ifc4.MeasureResource.IfcBoolean(true)));

            var wall = file.Model.Instances.OfType<IIfcWall>().First();

            var prop = wall.GetPropertySingleValue("Pset_WallCommon", "IsExternal");

            prop.Should().NotBeNull();
            prop.NominalValue.Value.Should().Be(true);
        }

        [Fact]
        public void Can_SetLengthProperties()
        {
            var builder = new FluentModelBuilder();

            var file = builder.CreateModel()
                .CreateEntities(c => c.Wall().WithPropertySingle("Pset_WallCommon","NominalHeight", new Ifc4.MeasureResource.IfcLengthMeasure(1.23d)));

            var wall = file.Model.Instances.OfType<IIfcWall>().First();

            var prop = wall.GetPropertySingleValue("Pset_WallCommon", "NominalHeight");

            prop.Should().NotBeNull();
            prop.NominalValue.Value.Should().Be(1.23d);
        }

        private static XbimEditorCredentials GetEditor()
        {
            var editor = new XbimEditorCredentials
            {
                EditorsGivenName = "John",
                EditorsFamilyName = "Doe",
                EditorsIdentifier = "john.doe@acme.com",
                EditorsOrganisationName = "Acme",
                EditorsOrganisationIdentifier = "com.acme",

                ApplicationFullName = "Sample app",
                ApplicationVersion = "1.2.3-alpha",
                ApplicationDevelopersName = "Acme",  // When same as EditorOrg => reuses the org
                ApplicationIdentifier = "sample-app"
            };
            return editor;
        }

        public void Dispose()
        {
            while(files.Any())
            {
                var file = files.Pop();
                if(Path.IsPathRooted(file) && File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }
    }

    public static class WIP
    {
        
    }

}
