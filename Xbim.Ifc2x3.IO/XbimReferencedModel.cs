using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc2x3.IO
{
    /// <summary>
    /// A model that is referenced by another XbimModel
    /// </summary>
    public class XbimReferencedModel : IReferencedModel
    {
        public IIfcDocumentInformation DocumentInformation;

        private readonly IfcStore _model;
        public IModel Model { get { return _model; } }

        public ITransaction DocumentInfoTransaction
        {
             get {
                 return ((XbimModel)DocumentInformation.Model).BeginTransaction();
             }
        }

        public XbimReferencedModel(IIfcDocumentInformation documentInformation)
        {
            DocumentInformation = documentInformation;
            if (!File.Exists(documentInformation.Name))
            {
                throw new XbimException("Reference model not found:" + documentInformation.Name);
            }
            try
            {
                _model = IfcStore.Open(documentInformation.Name);
            }
            catch (Exception)
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
                return DocumentInformation.Identification;
            }
        }

        public string Name
        {
            get
            {
                return DocumentInformation.Name;
            }
        }      

        public string Role
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


        public string OwningOrganisation
        {
            get
            {
                var organization = DocumentInformation.DocumentOwner as ActorResource.IfcOrganization;
                if (organization != null)
                    return organization.Name;
                return null;
            }
        }
    }
}
