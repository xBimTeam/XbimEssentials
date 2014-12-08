using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.IO
{
    /// <summary>
    /// A model that is referenced by another XbimModel
    /// </summary>
    public class XbimReferencedModel
    {
        public IfcDocumentInformation DocumentInformation;
        public XbimModel Model;

        public XbimReadWriteTransaction DocumentInfoTransaction
        {
             get {
                 return ((XbimModel)DocumentInformation.ModelOf).BeginTransaction();
             }
        }

        public XbimReferencedModel(IfcDocumentInformation documentInformation)
        {
            DocumentInformation = documentInformation;
            if (!File.Exists(documentInformation.Name))
            {
                throw new XbimException("Reference model not found:" + documentInformation.Name);
            }
            Model = new XbimModel();
            if (!Model.Open(documentInformation.Name, XbimDBAccess.Read))
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
                var organization = DocumentInformation.DocumentOwner as Xbim.Ifc2x3.ActorResource.IfcOrganization;
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
                var organization = DocumentInformation.DocumentOwner as Xbim.Ifc2x3.ActorResource.IfcOrganization;
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
            Model.Dispose();
        }
    }
}
