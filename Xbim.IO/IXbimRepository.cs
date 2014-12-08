// Xbim.IO
// Last Edited by Steve Lockley on 28/08/2014

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.IO
{
    public interface IXbimRepository
    {
        IfcFileHeader IfcHeader { get; set; }
        Version CreatingAssemblyVersion { get; }
        Version ApplicationAssemblyVersion { get; }

        /// <summary>
        /// Opens the repository, throws an error if open fails
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="accessMode"></param>
        /// <param name="progDelegate"></param>
        void Open(string connectionString, XbimDBAccess accessMode = XbimDBAccess.Read,
            ReportProgressDelegate progDelegate = null);

        /// <summary>
        /// Closes the repository if open, if the repository is closed already no action is taken and no error is thrown
        /// </summary>
        void Close();

        /// <summary>
        ///     Deletes the repository, an error is thrown if this fails, if no repository exists no error is thrown
        /// </summary>
        /// <param name="connectionString"></param>
        void Delete(string connectionString);

        /// <summary>
        /// Creates a new repository, the resulting repository is open for read / write operations
        /// </summary>
        /// <param name="connectionString"></param>
        void Create(string connectionString);

        /// <summary>
        ///     Creates a new container for the entity, If the entity label is >0 an attampt is made to use this label, if the
        ///     label exists an exceotion is thrown
        ///     If the label is 0 it is replaced by the label of the new container and returned from the function
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert(IPersistIfcEntity entity);

        /// <summary>
        /// Updates the repository to the latest state of the entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(IPersistIfcEntity entity);

        /// <summary>
        /// Retrieves and populates the properties of the entity
        /// </summary>
        /// <param name="entity"></param>
        void Retrieve(ref IPersistIfcEntity entity, int depth = 1);

        /// <summary>
        /// Gets the IfcType of the entity
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IfcType GetEntityType(int label);
    }
}