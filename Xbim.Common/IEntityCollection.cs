using System.Collections.Generic;
using System;

namespace Xbim.Common
{
	public interface IEntityCollection: IReadOnlyEntityCollection
	{

        IPersistEntity New(Type t);
        T New<T>(Action<T> initPropertiesFunc) where T : IInstantiableEntity;
        T New<T>() where T : IInstantiableEntity;
        IPersistEntity this[int label] { get; }
        
	}

	public interface IReadOnlyEntityCollection : IEnumerable<IPersistEntity>
    {
        /// <summary>
        /// Returns all entities satysfying the condition
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="condition">Condition to evaluate</param>
        /// <returns></returns>
		IEnumerable<T> Where<T>(Func<T, bool> condition) where T : IPersistEntity;
        /// <summary>
        /// Returns all entities satysfying the condition utilizing secondary index for inverse relations.
        /// Always use this overload if you query for the entities on the other side of inverse relation (like all IfcRelations).
        /// It might be significantly optimized in the implementation of this interface especially if the data
        /// is stored in the database.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="condition">Condition</param>
        /// <param name="inverseProperty">Name of the property which is being evaluated</param>
        /// <param name="inverseArgument">Entity which should be equal/contained in the property</param>
        /// <returns></returns>
	    IEnumerable<T> Where<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument)
	        where T : IPersistEntity;

	    /// <summary>
	    /// Returns first or default entity satysfying the condition
	    /// </summary>
	    /// <typeparam name="T">Type of the result</typeparam>
	    /// <returns></returns>
	    T FirstOrDefault<T>() where T : IPersistEntity;
        /// <summary>
        /// Returns first or default entity satysfying the condition
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="condition">Condition to evaluate</param>
        /// <returns></returns>
        T FirstOrDefault<T>(Func<T, bool> condition) where T : IPersistEntity;
        /// <summary>
        /// Returns firs or default entitiy satysfying the condition utilizing secondary index for inverse relations.
        /// Always use this overload if you query for the entities on the other side of inverse relation (like all IfcRelations).
        /// It might be significantly optimized in the implementation of this interface especially if the data
        /// is stored in the database.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="condition">Condition</param>
        /// <param name="inverseProperty">Name of the property which is being evaluated</param>
        /// <param name="inverseArgument">Entity which should be equal/contained in the property</param>
        /// <returns></returns>
        T FirstOrDefault<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument) where T : IPersistEntity;
        IEnumerable<T> OfType<T>() where T : IPersistEntity;
        IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity;
        IEnumerable<IPersistEntity> OfType(string stringType, bool activate);
		long Count { get; }
        long CountOf<T>() where T : IPersistEntity;
    }
}