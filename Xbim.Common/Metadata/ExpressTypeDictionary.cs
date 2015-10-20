using System;
using System.Collections.ObjectModel;

namespace Xbim.Common.Metadata
{
    public class ExpressTypeDictionary : KeyedCollection<Type, ExpressType>
    {
        protected override Type GetKeyForItem(ExpressType item)
        {
            return item.Type;
        }

        public ExpressType this[IPersist ent]
        {
            get { return this[ent.GetType()]; }
        }
    }
}
