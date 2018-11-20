namespace Xbim.Common.Federation
{
    public interface IReferencedModel
    {
        IModel Model { get; }

        /// <summary>
        /// Returns the unique identifier for this reference within the scope of the referencing model. 
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Mostly URI of the federated model so that when this is serialized it can be used to reopen the federation
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The role of the organisation that created this model, i.e. Architect, Engineer etc
        /// </summary>
        string Role { get;  set; }
        /// <summary>
        /// The name of the organisation that created and owns this model
        /// </summary>
        string OwningOrganisation { get; set; }

        void Close();
        
    }
}
