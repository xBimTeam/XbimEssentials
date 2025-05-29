using System;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
#nullable enable
    /// <summary>
    /// Interface used in building individual Model files
    /// </summary>
    public interface IModelFileBuilder
    {
        /// <summary>
        /// The <see cref="IModel"/> being built
        /// </summary>
        IModel Model { get; }
        /// <summary>
        /// The current <see cref="ITransaction"/>
        /// </summary>
        ITransaction Transaction { get; }
        /// <summary>
        /// A Factory that can create cross schema entities for the model
        /// </summary>
        EntityCreator Factory { get; }
        /// <summary>
        /// The inherited editor details
        /// </summary>
        XbimEditorCredentials? Editor { get; }
        /// <summary>
        /// The current <see cref="IIfcOwnerHistory"/>
        /// </summary>
        IIfcOwnerHistory? OwnerHistory { get; set; }
        /// <summary>
        /// Sets a new <see cref="ITransaction"/> on the model
        /// </summary>
        /// <param name="name"></param>
        void NewTransaction(string name = "");

        /// <summary>
        /// Provides a date for IFC Header and OwnerHistory timestamps
        /// </summary>
        DateTime EffectiveDateTime { get; }
    }
}
