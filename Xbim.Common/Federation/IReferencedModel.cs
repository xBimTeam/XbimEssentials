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
        string Name { get; }

        string Role { get; }
        
    }
}
