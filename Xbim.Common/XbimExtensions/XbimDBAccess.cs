using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.XbimExtensions
{
    public enum XbimDBAccess
    {
        /// <summary>
        /// Opens the database for read only transactions
        /// </summary>
        Read,
        /// <summary>
        /// Opens the database for readonly 
        /// </summary>
        ReadWrite,
        /// <summary>
        /// Opens the database exclusively, prevents access from any other processes.
        /// </summary>
        Exclusive
    }
}
