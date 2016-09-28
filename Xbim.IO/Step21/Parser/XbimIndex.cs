using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using System.IO;
using Xbim.XbimExtensions.Transactions;
using Xbim.XbimExtensions.Transactions.Extensions;
using System.Collections.ObjectModel;

namespace Xbim.IO.Parser
{
    public class XbimIndex : KeyedCollection<long, XbimIndexEntry>
    {
        private long _highestLabel;

        public long NextLabel
        {
            get { return _highestLabel + 1; }
        }

        public long HighestLabel
        {
            get { return _highestLabel; }
        }

        /// <summary>
        ///   Releases all objects that are cached in the index
        /// </summary>
        public void DropAll()
        {
            foreach (XbimIndexEntry item in this)
            {
                item.Drop();
            }
        }

        public XbimIndexEntry AddNew<T>(out IPersistIfcEntity newEntity)
        {
            newEntity = (IPersistIfcEntity)Activator.CreateInstance<T>();
            XbimIndexEntry entry = new XbimIndexEntry(NextLabel, newEntity);
            IList<XbimIndexEntry> entryList = this as IList<XbimIndexEntry>;
            entryList.Add_Reversible(entry);
            return entry;
        }

        protected override long GetKeyForItem(XbimIndexEntry item)
        {
            return item.EntityLabel;
        }

        protected override void InsertItem(int index, XbimIndexEntry item)
        {
            base.InsertItem(index, item);
            Transaction txn = Transaction.Current;
            if (txn != null)
                Transaction.AddPropertyChange<long>(h => _highestLabel = h, _highestLabel, Math.Max(_highestLabel, item.EntityLabel));
            _highestLabel = Math.Max(_highestLabel, item.EntityLabel);
        }

        protected override void SetItem(int index, XbimIndexEntry item)
        {
            base.SetItem(index, item);
            Transaction txn = Transaction.Current;
            if (txn != null)
                Transaction.AddPropertyChange<long>(h => _highestLabel = h, _highestLabel, Math.Max(_highestLabel, item.EntityLabel));
            _highestLabel = Math.Max(_highestLabel, item.EntityLabel);

        }

        protected override void ClearItems()
        {
            base.ClearItems();
            Transaction txn = Transaction.Current;
            if (txn != null)
                Transaction.AddPropertyChange<long>(h => _highestLabel = h, _highestLabel, 0);
            _highestLabel = 0;
        }


        public XbimIndex(long highestLabel)
        {
            _highestLabel = highestLabel;
        }

        public XbimIndex()
        {
            _highestLabel = 0;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((long)Count);
            writer.Write(HighestLabel);

            HashSet<Type> types = new HashSet<Type>();
            foreach (var item in this)
            {
                types.Add(item.Type);
            }
            Dictionary<Type, short> classNames = new Dictionary<Type, short>();
            writer.Write(types.Count);
            short i = 0;
            foreach (var eType in types)
            {
                classNames.Add(eType, i);
                writer.Write(eType.Name.ToUpper());
                writer.Write(i);
                i++;
            }
            // writer.Write(this.Count);
            foreach (var item in this)
            {
                writer.Write(item.EntityLabel);
                writer.Write(item.Offset);
                writer.Write(classNames[item.Type]);
            }
        }

        public static XbimIndex Read(Stream dataStream)
        {

            BinaryReader reader = new BinaryReader(dataStream);
            long count = reader.ReadInt64();
            XbimIndex index = new XbimIndex(reader.ReadInt64());


            HashSet<Type> types = new HashSet<Type>();

            int typeCount = reader.ReadInt32();
            Dictionary<short, string> classNames = new Dictionary<short, string>(typeCount);
            for (int i = 0; i < typeCount; i++)
            {
                string typeName = reader.ReadString();
                short typeId = reader.ReadInt16();
                classNames.Add(typeId, typeName);
            }
            //  int instanceCount = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                long label = reader.ReadInt64();
                long offset = reader.ReadInt64();
                short id = reader.ReadInt16();
                index.Add(new XbimIndexEntry(label, offset, IfcInstances.IfcTypeLookup[classNames[id]].Type));
            }
            return index;
        }

        public XbimIndexEntry AddNew(Type type, out IPersistIfcEntity newEntity, long label)
        {
            newEntity = (IPersistIfcEntity)Activator.CreateInstance(type);
            XbimIndexEntry entry = new XbimIndexEntry(label, newEntity);
            IList<XbimIndexEntry> entryList = this as IList<XbimIndexEntry>;
            entryList.Add_Reversible(entry);
            _highestLabel = Math.Max(label, _highestLabel);
            return entry;
        }

        public XbimIndexEntry AddNew(Type type, out IPersistIfcEntity newEntity)
        {
            newEntity = (IPersistIfcEntity)Activator.CreateInstance(type);
            XbimIndexEntry entry = new XbimIndexEntry(NextLabel, newEntity);
            IList<XbimIndexEntry> entryList = this as IList<XbimIndexEntry>;
            entryList.Add_Reversible(entry);
            return entry;
        }
    }


}
