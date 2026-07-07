using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace Xbim.Essentials.Tests
{
    internal class NetworkConnection
    {
        private bool _internalBool;

        internal NetworkConnection()
        {
            // this is slow, but apparently the best way to check for available connection.
            try
            {
                using (var client = new HttpClient())
                {
                    using (var stream = client.GetStreamAsync("http://www.google.com").GetAwaiter().GetResult())
                    {
                        _internalBool = true;
                    }
                }
            }
            catch
            {
                _internalBool = false;
            }
            Debug.Assert(_internalBool, "Several tests can only be executed online; try running the test suite again with network access before any commit.");
        }

        internal bool Available
        {
            get { return _internalBool; }
        }
    }
}