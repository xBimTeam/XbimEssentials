using System.IO;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.IO;

namespace Xbim.Ifc2x3.IO
{
    /// <summary>
    /// A model that is referenced by another XbimModel
    /// </summary>
    public class XbimReferencedModel : IReferencedModel
    {
        public IfcDocumentInformation DocumentInformation;

        private readonly XbimModel _model;
        public IModel Model { get { return _model; } }

        public ITransaction DocumentInfoTransaction
        {
             get {
                 return ((XbimModel)DocumentInformation.Model).BeginTransaction();
             }
        }

        public XbimReferencedModel(IfcDocumentInformation documentInformation)
        {
            DocumentInformation = documentInformation;
            if (!File.Exists(documentInformation.Name))
            {
                throw new XbimException("Reference model not found:" + documentInformation.Name);
            }
            _model = new XbimModel();
            if (!_model.Open(documentInformation.Name))
            {
                throw new XbimException("Unable to open reference model: " + documentInformation.Name);
            }
        }

        /// <summary>
        /// Returns the identifier for this reference within the scope of the referencing model
        /// </summary>
        public string Identifier
        {
            get
            {
                return DocumentInformation.DocumentId;
            }
        }

        //public string Owner
        //{
        //    get
        //    {
        //        return DocumentInformation.DocumentOwner.ToString();
        //    }
        //}

        public string Name
        {
            get
            {
                return DocumentInformation.Name;
            }
        }

        public string OrganisationName
        {
            get
            {
                var organization = DocumentInformation.DocumentOwner as ActorResource.IfcOrganization;
                if (organization != null)
                    return organization.Name;
                return null;
            }
        }

        // todo: this looks like nonsense to me (CB), if anything this returns owner role
        public string OwnerName
        {
            get
            {
                var organization = DocumentInformation.DocumentOwner as ActorResource.IfcOrganization;
                if (organization != null)
                {
                    var role = organization.Roles.FirstOrDefault();
                    if (role != null)
                        return role.UserDefinedRole;
                }
                return null;
            }
        }

        internal void Dispose()
        {
            _model.Dispose();
        }
    }
}
