#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Edit.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Abstract base class for all reversible changes.
    /// </summary>
    public abstract class Edit
    {
        /// <summary>
        ///   Utility function to switch between two values handy when reversing some operations.
        /// </summary>
        /// <typeparam name = "T">Type of variable</typeparam>
        /// <param name = "var1">Reference to first variable</param>
        /// <param name = "var2">Reference to second variable</param>
        protected static void Switch<T>(ref T var1, ref T var2)
        {
            T oldvar1 = var1;
            var1 = var2;
            var2 = oldvar1;
        }

        /// <summary>
        ///   Reverses the operation (undoing or redoing depending on state in transaction)
        /// </summary>
        /// <returns>An Edit instance representing the operation that reverses the effect of the done reversal (usual the same instance).</returns>
        public abstract Edit Reverse();

        /// <summary>
        ///   Returns the name of this reversible operation (or null if no name is defined)
        /// </summary>
        public virtual string Name
        {
            get { return null; }
        }
    }
}