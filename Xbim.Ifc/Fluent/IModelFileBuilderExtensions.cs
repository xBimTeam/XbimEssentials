using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common.Exceptions;
using Xbim.Common.Step21;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
    public static class IModelFileBuilderExtensions
    {
#nullable enable

        /// <summary>
        /// Sets up a new OwnerHistory for this model, based on the supplied <see cref="XbimEditorCredentials"/>, which will be applied to all
        /// subsequently added entities.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="editor">Optional editor. Falls back to <see cref="FluentModelBuilder.Editor"/> when omitted</param>
        /// <param name="action">Optional Action to amend the default OwnerHistory</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IModelFileBuilder SetOwnerHistory(this IModelFileBuilder builder, XbimEditorCredentials? editor = null, Action<IIfcOwnerHistory>? action = null)
        {
            editor ??= builder.Editor ?? throw new Exception("An editor must be defined to set OwnerHistory");
            var app = builder.Model.GetOrCreateApplication(editor);
            var user = builder.Model.GetOrCreateDefaultUser(editor);

            Func<IIfcOwnerHistory, IIfcOwnerHistory>? defaultValue = o =>
            {
                o.ChangeAction = IfcChangeActionEnum.ADDED;
                o.State = IfcStateEnum.READWRITE;
                o.OwningApplication = app;
                o.OwningUser = user;
                o.CreationDate = builder.EffectiveDateTime;
                o.LastModifiedDate = builder.EffectiveDateTime;
                return o;
            };

            action ??= _ => { };

            builder.OwnerHistory = builder.Factory.OwnerHistory(o => action(defaultValue(o)));

            return builder;
        }

        /// <summary>
        /// Sets the IFC STEP headers using the default <see cref="FluentModelBuilder.Editor"/> parameters for Author, Organisation and OrignatingSystem
        /// when supplied
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="stepFileName">Action to amend the FileName explicitly</param>
        /// <param name="fileDescription">Action to amend the Files Description (MVD) explicitly</param>
        /// <returns></returns>
        public static IModelFileBuilder SetHeaders(this IModelFileBuilder builder, Action<IStepFileName>? stepFileName = null, Action<IStepFileDescription>? fileDescription = null)
        {
            fileDescription ??= fd => fd.Description.Add("ViewDefinition [CoordinationView]");
            stepFileName ??= f =>
            {
                f.OriginatingSystem = $"{builder.Editor?.ApplicationFullName ?? "xbim Toolkit"} {builder.Editor?.ApplicationVersion}";
                f.TimeStamp = string.Format(builder.EffectiveDateTime.ToString("s")); 
                if (builder.Editor != null)
                {
                    f.AuthorName.Add($"{builder.Editor.EditorsGivenName} {builder.Editor.EditorsFamilyName}");
                    f.Organization.Add($"{builder.Editor.EditorsOrganisationName}");
                }
            };
            builder.Model.AddHeaders(stepFileName, fileDescription);
            return builder;
        }

        /// <summary>
        /// Saves the current Model to STEP IFC to the specified <paramref name="fileName"/> path.
        /// </summary>
        /// <remarks><para>Any existing file will be over-written.</para> 
        /// <para>By default the model and transaction will be closed. This 
        /// behaviour can be over-ridden by setting <paramref name="keepOpen"/> to <c>true</c></para>
        /// </remarks>
        /// <param name="fileBuilder"></param>
        /// <param name="fileName">The *.ifc filename &amp; path to save to</param>
        /// <param name="keepOpen"></param>
        public static IModelFileBuilder SaveAsIfc(this IModelFileBuilder fileBuilder, string fileName, bool keepOpen = false)
        {
            fileBuilder.Model.Header.FileName.Name = fileName;
            fileBuilder.Transaction.Commit();

            using var sw = new FileStream(fileName, FileMode.Create);
            fileBuilder.Model.SaveAsIfc(sw);
            if (keepOpen == false)
                fileBuilder.Discard();
            else
                fileBuilder.NewTransaction();

            return fileBuilder;
        }

        /// <summary>
        /// Initiates building of the models entities using provided Factory 
        /// </summary>
        /// <param name="fileBuilder"></param>
        /// <param name="config">Action to build the entities, using <see cref="IModelFileBuilder.Factory"/></param>
        /// <returns>The <see cref="IModelFileBuilder"/></returns>
        public static IModelFileBuilder CreateEntities(this IModelFileBuilder fileBuilder, Action<EntityCreator> config)
        {
            var builder = new ModelInstanceBuilder(fileBuilder);
            config(builder.Factory);
            return fileBuilder;
        }

        /// <summary>
        /// Initiates building of the models entities using provided Factory 
        /// </summary>
        /// <param name="fileBuilder"></param>
        /// <param name="config">Action to build the entities, using <see cref="IModelFileBuilder.Factory"/> and the parent <see cref="IModelInstanceBuilder"/></param>
        /// <returns>The <see cref="IModelFileBuilder"/></returns>
        public static IModelFileBuilder CreateEntities(this IModelFileBuilder fileBuilder, Action<EntityCreator, IModelInstanceBuilder> config)
        {
            var builder = new ModelInstanceBuilder(fileBuilder);
            config(builder.Factory, builder);
            return fileBuilder;
        }

        /// <summary>
        /// Discards the built model &amp; transaction, freeing up resources
        /// </summary>
        /// <remarks>For test purposes.</remarks>
        /// <param name="fileBuilder"></param>
        public static void Discard(this IModelFileBuilder fileBuilder)
        {
            (fileBuilder as IDisposable)!.Dispose();
        }

        /// <summary>
        /// Validates the model and provides the set of errors for the given <paramref name="validationFlags"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validationFlags">Flags determining level of validation to undertake. Defaults to <see cref="Common.Enumerations.ValidationFlags.All"/></param>
        /// <returns>The set of validation results</returns>
        public static IEnumerable<Common.ExpressValidation.ValidationResult> ValidateIfc(this IModelFileBuilder builder, Common.Enumerations.ValidationFlags validationFlags = Common.Enumerations.ValidationFlags.All)
        {
            var validator = new Xbim.Common.ExpressValidation.Validator();
            validator.ValidateLevel = validationFlags;
            var results = validator.Validate(builder.Model);

            return results;
        }

        /// <summary>
        /// Asserts the model is valid IFC to the level specified by the <paramref name="validationFlags"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validationFlags"></param>
        /// <returns></returns>
        /// <exception cref="XbimException">Thrown when errors exist</exception>
        public static IModelFileBuilder AssertValid(this IModelFileBuilder builder, Common.Enumerations.ValidationFlags validationFlags = Common.Enumerations.ValidationFlags.All)
        {
            var results = builder.ValidateIfc(validationFlags);
            if (results.Any())
            {
                var topErrors = results.Take(10).Select(e => $"[{e.IssueSource}] {e.Message}: {e.Item}").ToList();
                var errors = string.Join("\n", topErrors);
                throw new XbimException($"Model has {results.Count()} validation error(s): \n{errors}");
            }
            return builder;
        }
    }
}
