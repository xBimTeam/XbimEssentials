using System;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal abstract class PersistEntityTransient : IPersistEntity
    {
        public void Parse(int propIndex, IPropertyValue value, int[] nested)
        {
            throw new NotSupportedException("Transient object");
        }

        public string WhereRule()
        {
            return "";
        }

        /// <summary>
        /// This is a transient object which is not a part of the model
        /// </summary>
        public int EntityLabel { get { return -1; } }

        /// <summary>
        /// If not overwritten in derived classes this always returns null
        /// </summary>
        public virtual IModel Model { get { return null; } }

        /// <summary>
        /// Transient objects are only for reading.
        /// </summary>
        public bool Activated { get { return true; } }


        /// <summary>
        /// This property will always return null if 'Model' property is not overwritten in derived class
        /// </summary>
        public ExpressType ExpressType
        {
            get
            {
                return Model != null
                    ? Model.Metadata.ExpressType(this)
                    : null;
            }
        }

        [Obsolete("Use Model instead.")]
        public IModel ModelOf { get { return Model; } }
    }
}
