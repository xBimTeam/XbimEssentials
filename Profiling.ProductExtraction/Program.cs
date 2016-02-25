using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Profiling.ProductExtraction
{
    internal class Program
    {
        private static void Main()
        {
            using (var source = IfcStore.Open("..\\..\\..\\Xbim.IO.Tests\\TestFiles\\SampleHouse4.ifc"))
            {
                using (var target = IfcStore.Create(source.IfcSchemaVersion, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction("Copy"))
                    {
                        var products = source.Instances.OfType<IIfcBuildingElement>();
                        target.InsertProductsWithContext(products, true, true, new XbimInstanceHandleMap(source, target));
                        txn.Commit();
                    }
                }    
            }
            
        }
    }
}
