using System;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
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
            get
            {

                if (DocumentInformation.Model.CurrentTransaction != null)
                    return new PlaceboTransaction(); //don't start one we are already    
                return DocumentInformation.Model.BeginTransaction("Update ReferenceModel");
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
            set
            {
                using (var txn = DocumentInfoTransaction)
                {
                    if (DocumentInformation is Ifc2x3.ExternalReferenceResource.IfcDocumentInformation)
                        ((Ifc2x3.ExternalReferenceResource.IfcDocumentInformation) DocumentInformation).Name = value;
                    else if (DocumentInformation is Ifc4.ExternalReferenceResource.IfcDocumentInformation)
                        ((Ifc4.ExternalReferenceResource.IfcDocumentInformation)DocumentInformation).Name = value;
                    else
                    {
                        throw new Exception("Invalid IFC schema");
                    }
                    txn.Commit();
                }
            }
        }      

        public string Role
        {
            get
            {
                var organisation = DocumentInformation.DocumentOwner as IIfcOrganization;
                if (organisation == null) return null;

                var role = organisation.Roles.FirstOrDefault();
                if (role == null) return null;

                if (role.Role == IfcRoleEnum.USERDEFINED)
                    return role.UserDefinedRole;
                return role.Role.ToString();
            }
            set
            {

                using (var txn = DocumentInfoTransaction)
                {
                    if (DocumentInformation is Ifc2x3.ExternalReferenceResource.IfcDocumentInformation)
                    {
                        var organisation = DocumentInformation.DocumentOwner as Ifc2x3.ActorResource.IfcOrganization;
                        if (organisation == null)
                        {
                            organisation =
                                DocumentInformation.Model.Instances.New<Ifc2x3.ActorResource.IfcOrganization>();
                            ((Ifc2x3.ExternalReferenceResource.IfcDocumentInformation)DocumentInformation).DocumentOwner = organisation;
                        }
                        var role = organisation.Roles.FirstOrDefault();
                        organisation.Roles.Clear();
                        if (role == null) role = DocumentInformation.Model.Instances.New<Ifc2x3.ActorResource.IfcActorRole>();
                        Ifc2x3.ActorResource.IfcRoleEnum roleEnum;
                        if (!Enum.TryParse(value, true, out roleEnum))
                        {
                            role.UserDefinedRole = value;
                            role.Role = Ifc2x3.ActorResource.IfcRoleEnum.USERDEFINED;
                        }
                        else
                        {
                            role.Role = roleEnum;
                        }
                        organisation.Roles.Add(role);
                    }
                    else if (DocumentInformation is Ifc4.ExternalReferenceResource.IfcDocumentInformation)
                    {
                        var organisation = DocumentInformation.DocumentOwner as Ifc4.ActorResource.IfcOrganization;
                        if (organisation == null)
                        {
                            organisation =
                                DocumentInformation.Model.Instances.New<Ifc4.ActorResource.IfcOrganization>();
                            ((Ifc4.ExternalReferenceResource.IfcDocumentInformation)DocumentInformation).DocumentOwner = organisation;
                        }
                        var role = organisation.Roles.FirstOrDefault();
                        organisation.Roles.Clear();
                        if (role == null) role = DocumentInformation.Model.Instances.New<Ifc4.ActorResource.IfcActorRole>();
                        Ifc4.Interfaces.IfcRoleEnum roleEnum;
                        if (!Enum.TryParse(value, true, out roleEnum))
                        {
                            role.UserDefinedRole = value;
                            role.Role = Ifc4.Interfaces.IfcRoleEnum.USERDEFINED;
                        }
                        else
                        {
                            role.Role = roleEnum;
                        }
                        organisation.Roles.Add(role);
                    }
                    else
                    {
                        throw new Exception("Invalid IFC schema");
                    }
                    txn.Commit();
                }
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
                var organisation = DocumentInformation.DocumentOwner as IIfcOrganization;
                if (organisation != null)
                    return organisation.Name;
                return null;
            }
            set
            {
                using (var txn = DocumentInfoTransaction)
                {
                    if (DocumentInformation is Ifc2x3.ExternalReferenceResource.IfcDocumentInformation)
                    {
                        var organisation = DocumentInformation.DocumentOwner as Ifc2x3.ActorResource.IfcOrganization;
                        if (organisation == null)
                        {
                            organisation =
                                DocumentInformation.Model.Instances.New<Ifc2x3.ActorResource.IfcOrganization>();
                            ((Ifc2x3.ExternalReferenceResource.IfcDocumentInformation)DocumentInformation).DocumentOwner = organisation;
                        }
                        organisation.Name = value;
                    }
                    else if (DocumentInformation is Ifc4.ExternalReferenceResource.IfcDocumentInformation)
                    {
                        var organisation = DocumentInformation.DocumentOwner as Ifc4.ActorResource.IfcOrganization;
                        if (organisation == null)
                        {
                            organisation =
                                DocumentInformation.Model.Instances.New<Ifc4.ActorResource.IfcOrganization>();
                            ((Ifc4.ExternalReferenceResource.IfcDocumentInformation)DocumentInformation).DocumentOwner = organisation;
                        }
                        organisation.Name = value;
                    }
                    else
                    {
                        throw new Exception("Invalid IFC schema");
                    }
                    txn.Commit();
                }
                
            }
        }


       
    }
}
