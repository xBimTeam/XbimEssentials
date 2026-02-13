using System;

namespace Xbim.IO.Esent
{
    public class EsentEngineOptions
    {
        /// <summary>
        /// The defauylt version for newly created Esent models 
        /// </summary>
        public EngineFormatVersion FormatVersion { get; set; } = EngineFormatVersion.Default;
    }
}
