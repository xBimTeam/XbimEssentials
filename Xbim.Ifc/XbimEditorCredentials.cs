using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    /// <summary>
    /// Defines baseline details of any user and  application used to make changes to a model.
    /// </summary>
    /// <remarks>Used to build the <see cref="IIfcOwnerHistory"/>linked to an additions or modifications 
    /// made on a model</remarks>
    public class XbimEditorCredentials
    {
        /// <summary>
        /// The unique Identifier of the end user - e.g. email address or UUID
        /// </summary>
        public string EditorsIdentifier;
        /// <summary>
        /// The family name of the end user
        /// </summary>
        public string EditorsFamilyName;
        /// <summary>
        /// The given name of the end user
        /// </summary>
        public string EditorsGivenName;
        /// <summary>
        /// The name of the organisation the editor belongs to
        /// </summary>
        public string EditorsOrganisationName;
        /// <summary>
        /// Identification of the organization - e.g. an Org code or UUID
        /// </summary>
        public string EditorsOrganisationIdentifier;
        /// <summary>
        /// The full name of the application as specified by the application developer.
        /// </summary>
        public string ApplicationFullName;
        /// <summary>
        /// The version number of this software as specified by the developer of the application.
        /// </summary>
        public string ApplicationVersion;
        /// <summary>
        /// A short identifying name for the application
        /// </summary>
        public string ApplicationIdentifier;
        /// <summary>
        /// The name of the organisation responsible for the IFC authoring tool
        /// </summary>
        public string ApplicationDevelopersName;
    }
}

                