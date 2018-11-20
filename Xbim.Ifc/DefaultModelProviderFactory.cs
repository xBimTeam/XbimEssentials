using System;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.Ifc
{

    // Poor-man's DI 

    /// <summary>
    /// A default factory that provides an <see cref="IModelProvider"/>
    /// </summary>
    /// <remarks>By default, this factory returns the basic <see cref="MemoryModelProvider"/></remarks>
    public class DefaultModelProviderFactory : IModelProviderFactory
    {

        static Func<IModelProvider> _modelProvider = null;

        /// <summary>
        /// Creates a new <see cref="IModelProvider"/>
        /// </summary>
        /// <returns></returns>
        public IModelProvider CreateProvider()
        {
            return _modelProvider != null ? _modelProvider(): 
                new MemoryModelProvider();
        }

        /// <summary>
        /// Hook to allow 3rd parties to configure another <see cref="IModelProvider"/> implementation
        /// </summary>
        /// <param name="providerFn"></param>
        public static void Configure(Func<IModelProvider> providerFn)
        {
            if (providerFn == null)
            {
                throw new ArgumentNullException(nameof(providerFn));
            }
            _modelProvider = providerFn;
        }
    }
}
