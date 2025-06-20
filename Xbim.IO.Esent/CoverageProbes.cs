using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Xbim.Essentials.NetCore.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010029a3c6da60efcb3ebe48c3ce14a169b5fa08ffbf5f276392ffb2006a9a2d596f5929cf0e68568d14ac7cbe334440ca0b182be7fa6896d2a73036f24bca081b2427a8dec5689a97f3d62547acd5d471ee9f379540f338bbb0ae6a165b44b1ae34405624baa4388404bce6d3e30de128cec379147af363ce9c5845f4f92d405ed0")]

namespace Xbim.IO.Esent
{
    /// <summary>
    /// This class is used to hold coverage probes for the Xbim.IO.Esent library.
    /// </summary>
    internal static class CoverageProbes
    {
        /// <summary>
        /// Ensures that the heuristic model provider uses the efficient save-as path.
        /// </summary>
        public static bool HeuristicModelProvider_EfficientEsentSaveasHit { get; set; } = false;
    }
}
