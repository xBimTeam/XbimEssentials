using System;
using System.Collections.Generic;
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
    public class XbimReferencedModel : IReferencedModel, IDisposable
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

            // for legacy purposes we should be able to load files from the name inside the document information
            // but the correct approach seems to be to have that data using the HasDocumentReferences property:
            // in the specs we read that 
            // "The actual content of the document is not defined in IFC ; instead, it can be found following the reference given to IfcDocumentReference."
            //
            var searchPaths = new List<string>();
            if (documentInformation.Model is IfcStore)
            {
                var referencingStore = documentInformation.Model as IfcStore;
                var fi = new FileInfo(referencingStore.FileName);
                searchPaths.Add(fi.DirectoryName);
            }
            var headerFileName = documentInformation.Model?.Header?.FileName?.Name;
            if (Path.IsPathRooted(headerFileName))
            {
                searchPaths.Add(Path.GetDirectoryName(headerFileName));
            }

            // we will use the first valid document reference in a list that is evaluate as absolute or relative.
            //
            var candidates = DocumentInformation.HasDocumentReferences.Select(x => x.Location).ToList();
            candidates.Add(documentInformation.Name.ToString()); // this is for legacy in xbim federations

            string resourceReference = string.Empty;
            // evaluating absolute path
            foreach (var candidate in candidates)
            {
                var thisCandidate = candidate;
                // check if can be found as is
                if (File.Exists(thisCandidate))
                {
                    resourceReference = thisCandidate;
                    break;
                }
                // check if it can be relative
                if (!Path.IsPathRooted(candidate))
                {
                    foreach (var path in searchPaths)
                    {
                        thisCandidate = Path.Combine(
                            path, candidate
                            );
                        if (File.Exists(thisCandidate))
                        {
                            resourceReference = thisCandidate;
                            goto StopSearch;
                        }
                    }
                }
            }

            StopSearch:
            if (string.IsNullOrEmpty(resourceReference)) 
            {
                throw new XbimException($"Reference model not found for IfcDocumentInformation #{documentInformation.EntityLabel}.");
            }
            try
            {
                _model = IfcStore.Open(resourceReference);
            }
            catch (Exception ex)
            {                
                 throw new XbimException("Error opening referenced model: " + resourceReference, ex);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Model != null)
                    {
                        Model.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public void Close()
        {
            _model.Close(); // Close the Store, which will close the underlying model
        }
        #endregion



    }
}
