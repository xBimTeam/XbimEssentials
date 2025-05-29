using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IfcStoreEditorExtensions
    {
        /// <summary>
        /// Gets a matching <see cref="IIfcApplication"/> creating a new one where no match exists
        /// </summary>
        /// <param name="model"></param>
        /// <param name="editor"></param>
        /// <param name="addDefaultRole">Flag indicating whether a default role is added</param>
        /// <returns></returns>
        public static IIfcApplication GetOrCreateApplication(this IModel model, XbimEditorCredentials editor, bool addDefaultRole = false)
        {

            if (editor == null)
                return null;

            var existingApplication = model.Instances.OfType<IIfcApplication>()
                .Where(a => a.ApplicationFullName == editor.ApplicationFullName)
                .Where(a => a.ApplicationIdentifier == editor.ApplicationIdentifier)
                .Where(a => a.Version == editor.ApplicationVersion)
                .FirstOrDefault();
            if (existingApplication != null)
            {
                // use existing to avoid duplicate applications
                return existingApplication;
            }
            else
            {
                // Create new application
                var factory = new EntityCreator(model);
                return factory.Application(a =>
                {
                    a.ApplicationDeveloper = model.Instances.OfType<IIfcOrganization>().FirstOrDefault(o => o.Name == editor.ApplicationDevelopersName)
                        ?? factory.Organization(o =>
                        {
                            o.Name = editor.ApplicationDevelopersName;
                            if(addDefaultRole)
                            { 
                                o.Roles.Add(factory.ActorRole(r =>
                                {
                                    r.Role = IfcRoleEnum.USERDEFINED;
                                    r.UserDefinedRole = "Software Provider";
                                }));
                            }
                        });
                    a.ApplicationFullName = editor.ApplicationFullName;
                    a.ApplicationIdentifier = editor.ApplicationIdentifier;
                    a.Version = editor.ApplicationVersion;
                });
            }
        }

        /// <summary>
        /// Gets <see cref="IIfcPersonAndOrganization"/> based on the <paramref name="editor"/>, creating a new one 
        /// if no match exists.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static IIfcPersonAndOrganization GetOrCreateDefaultUser(this IModel model, XbimEditorCredentials editor)
        {
            // data wasn't supplied to create default user and application
            if (editor == null)
                return null;

            var factory = new EntityCreator(model);

            var person = model.Instances.OfType<IIfcPerson>()
                .FirstOrDefault(p => // Look up by identifier and then Given/Family name
                    editor.EditorsIdentifier != null && p.Identification == editor.EditorsIdentifier ||
                    (p.GivenName == editor.EditorsGivenName && p.FamilyName == editor.EditorsFamilyName))
                ?? factory.Person(p =>
                {
                    p.Identification = editor.EditorsIdentifier;
                    p.GivenName = editor.EditorsGivenName;
                    p.FamilyName = editor.EditorsFamilyName;
                });

            var organization = model.Instances.OfType<IIfcOrganization>()
                .FirstOrDefault(o => // Look up by identifier and then Organisation name
                    editor.EditorsOrganisationIdentifier != null && o.Identification == editor.EditorsOrganisationIdentifier ||
                    (o.Name == editor.EditorsOrganisationName))
                ?? factory.Organization(o =>
                {
                    o.Name = editor.EditorsOrganisationName;
                    o.Identification = editor.EditorsOrganisationIdentifier;
                });
            organization.Identification ??= editor.EditorsOrganisationIdentifier;
            return
                model.Instances.OfType<IIfcPersonAndOrganization>()
                .FirstOrDefault(po => po.ThePerson == person && po.TheOrganization == organization)
                ?? factory.PersonAndOrganization(po =>
                {
                    po.TheOrganization = organization;
                    po.ThePerson = person;
                });

        }
    }
}
