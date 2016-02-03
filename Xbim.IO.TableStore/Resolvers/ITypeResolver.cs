using System;
using NPOI.SS.UserModel;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore.Resolvers
{
    /// <summary>
    /// Implementatios of ITypeResolver can be used to resolve abstract types when data is being read into object model.
    /// You can add as many resolvers as necessary to TableStore.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Checks if this resolver can resolve the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanResolve(Type type);
        bool CanResolve(ExpressType type);
        /// <summary>
        /// Implementation should return a non-abstract type which can be initialized and used for a deserialization
        /// </summary>
        /// <param name="type">Abstract type or interface to be resolved</param>
        /// <param name="cell">Cell containing target value. This might be used if you need to chect format and type of input data</param>
        /// <param name="cMapping">Mapping for the table and class</param>
        /// <param name="pMapping">Mapping for the current cell and property</param>
        /// <returns>non-abstract type</returns>
        Type Resolve(Type type, ICell cell, ClassMapping cMapping, PropertyMapping pMapping);
    }
}
