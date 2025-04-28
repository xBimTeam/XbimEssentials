using System;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.IO.Memory;

namespace Xbim.Ifc.Fluent
{
#nullable enable

    /// <summary>
    /// A helper class to aid building valid cross-schema IFC models easily using a Fluent syntax
    /// </summary>
    /// <remarks>
    /// Example usage
    /// <code>
    /// XbimEditorCredentials editor = GetEditor();
    /// var builder = new FluentModelBuilder();
    /// builder.AssignEditor(editor)
    ///   .CreateModel(XbimSchemaVersion.Ifc2X3)
    ///   .SetHeaders()
    ///   .SetOwnerHistory()
    ///   .CreateEntities(cfg =>
    ///   {
    ///      var type = cfg.Create().WallType();
    ///      cfg.Create().Wall(o => o.WithAttributes(t => t with { PredefinedType = "XYZ" }))
    ///        .AddDefiningType(type);
    ///   })
    ///   .SaveAsIfc("wall-with-type.ifc");
    /// </code>
    /// </remarks>
    public class FluentModelBuilder
    {
        /// <summary>
        /// The global editor in use across all models created with this Builder
        /// </summary>
        public XbimEditorCredentials? Editor { get; private set; }

        /// <summary>
        /// Start building a new <see cref="IModel"/>
        /// </summary>
        /// <param name="schemaVersion"></param>
        /// <returns></returns>
        public IModelFileBuilder CreateModel(XbimSchemaVersion schemaVersion = XbimSchemaVersion.Ifc2X3)
        {
            var factory = GetFactory(schemaVersion);
            var model = new MemoryModel(factory);
            return new ModelFileBuilder(this, model);
        }

        /// <summary>
        /// Assign a global editor
        /// </summary>
        /// <remarks>This will automatically set the OwnerHistory on created items and populate the STEP Headers if you invoke <see cref="IModelFileBuilderExtensions.SetHeaders(IModelFileBuilder, Action{IStepFileName}?, Action{IStepFileDescription}?)"/> 
        /// or <see cref="IModelFileBuilderExtensions.SetOwnerHistory(IModelFileBuilder, XbimEditorCredentials?, Action{Ifc4.Interfaces.IIfcOwnerHistory}?)"/> on the
        /// Model</remarks>
        /// <param name="editor"></param>
        /// <returns></returns>
        public FluentModelBuilder AssignEditor(XbimEditorCredentials editor)
        {
            Editor = editor;
            return this;
        }

        private IEntityFactory GetFactory(XbimSchemaVersion schemaVersion)
        {
            return schemaVersion switch
            {
                XbimSchemaVersion.Ifc2X3 => new Xbim.Ifc2x3.EntityFactoryIfc2x3(),
                XbimSchemaVersion.Ifc4 => new Xbim.Ifc4.EntityFactoryIfc4(),
                XbimSchemaVersion.Ifc4x1 => new Xbim.Ifc4.EntityFactoryIfc4x1(),
                XbimSchemaVersion.Ifc4x3 => new Xbim.Ifc4x3.EntityFactoryIfc4x3Add2(),
                _ => throw new NotSupportedException(schemaVersion.ToString())
            };
        }
    }
}
