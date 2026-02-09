using System;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.IO;


namespace Xbim.Essentials.NetCore.Tests
{
    /// <summary>
    /// Reinstate DI after a xunit fixture may have cleared the shared DI
    /// </summary>
    public class xUnitReinit : IDisposable
    {

        public xUnitReinit()
        {
            // Nothing to do on setup fixture
        }

        public static void Reset()
        {
            if (!XbimServices.Current.IsBuilt)
            {
                XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddHeuristicModel()));
            }
        }

        public void Dispose()
        {
            // Reinitialise the DI
            Reset();
            _ = IfcStore.Create(Common.Step21.XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        }
    }
}
