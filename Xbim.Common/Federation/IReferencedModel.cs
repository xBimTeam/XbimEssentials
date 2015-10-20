namespace Xbim.Common.Federation
{
    public interface IReferencedModel
    {
        IModel Model { get; }

        /// <summary>
        /// Returns the unique identifier for this reference within the scope of the referencing model. 
        /// </summary>
        string Identifier { get; }

        string Name { get; }
        
    }
}
