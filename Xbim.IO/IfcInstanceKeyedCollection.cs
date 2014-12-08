using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.Transactions.Extensions;
namespace Xbim.IO
{
    public interface IXbimInstance
    {
        long EntityLabel { get; }
        Type EntityType { get; }
        long FileOffset { get; }
        bool IsLoaded { get; }
    }
    public struct XbimInstance : IXbimInstance
    {
        private readonly long _fileOffset;
        private readonly IPersistIfcEntity _entity;

        public IPersistIfcEntity Entity
        {
            get { return _entity; }
        } 

        public XbimInstance(IPersistIfcEntity entity)
        {
            _entity = entity;
            _fileOffset = -1;
        }
        public XbimInstance(IPersistIfcEntity entity, long fileOffset)
        {
            _entity = entity;
            _fileOffset = fileOffset;
        }

        public long EntityLabel
        {
            get
            {
                return _entity.EntityLabel;
            }
        }
        /// <summary>
        /// This is a loaded Entity so always return -1 for the fiel offset
        /// </summary>
        public long FileOffset
        {
            get
            {
                return _fileOffset;
            }
        }
        public Type EntityType
        {
            get { return _entity.GetType(); }
        }
        public bool IsLoaded
        {
            get { return true; }
        }
    }
    public struct XbimInstanceHandle : IXbimInstance
    {
        private readonly long _entityLabel;
        private readonly long _fileOffset;
        private readonly Type _entityType;
       

        public XbimInstanceHandle(long entityLabel, Type type, long fileOffset)
        {
            _entityLabel = Math.Abs(entityLabel);
            _fileOffset = fileOffset;
            _entityType = type;
        }

        public XbimInstanceHandle(IXbimInstance item)
        {
            _entityLabel =  Math.Abs(item.EntityLabel);
            _fileOffset = item.FileOffset;
            _entityType = item.EntityType;
        }
        public Type EntityType
        {
            get { return _entityType; }
        }
        /// <summary>
        /// The entity label this will always be positive
        /// </summary>
        public long EntityLabel
        {
            get { return _entityLabel; }
        }

        public long FileOffset
        {
            get { return _fileOffset; }
        }

        public bool IsLoaded
        {
            get { return false; }
        }
    }

    [Serializable]
    public class IfcInstanceKeyedCollection : Dictionary<long, IXbimInstance>
    {
        

        internal void Add(IPersistIfcEntity instance)
        {
            this.Add(instance.EntityLabel, new XbimInstance(instance));
        }

        /// <summary>
        /// if the entity is loaded returns it or null if not
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        internal IPersistIfcEntity GetEntity(long label)
        {
            IXbimInstance inst;
            if (this.TryGetValue(label, out inst ))
            {
                if (inst.IsLoaded)
                    return ((XbimInstance)inst).Entity;
            }
            return null;
        }

        internal void Add_Reversible(IPersistIfcEntity instance)
        {
            this.Add_Reversible(new KeyValuePair<long,IXbimInstance>(Math.Abs(instance.EntityLabel), new XbimInstance(instance)));
        }

        internal bool Remove_Reversible(long p)
        {
            return this.Remove_Reversible(Math.Abs(p));
        }

        public IEnumerator<IPersistIfcEntity> GetEnumerator(IModel model)
        {
            return new IfcInstanceEnumerator(model, this);
        }
        /// <summary>
        /// Converts a XbimInstance, this is NOT and undoable action
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entityType"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        internal IPersistIfcEntity CreateEntity(IModel model, Type entityType, long handle)
        {
            return CreateEntity(model, new XbimInstanceHandle(handle,entityType,-1));
        }
        /// <summary>
        /// Converts a XbimInstanceHandle into an XbimInstance, this is NOT and undoable action
        /// </summary>
        /// <param name="model"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        internal IPersistIfcEntity CreateEntity(IModel model, XbimInstanceHandle handle)
        {
            IPersistIfcEntity entity = (IPersistIfcEntity)Activator.CreateInstance(handle.EntityType);
            entity.Bind(model, (handle.EntityLabel * -1)); //a negative handle determines that the attributes of this entity have not been loaded yet
            XbimInstance inst = new XbimInstance(entity, handle.FileOffset);
            this[handle.EntityLabel]= inst;
            return entity;
        }
        /// <summary>
        /// If the entity is in memory returns it, if not a blank instance is returned
        /// </summary>
        /// <param name="model"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        internal IPersistIfcEntity GetOrCreateEntity(IModel model, long label, out long fileOffset)
        {
            IXbimInstance xbimInst = this[Math.Abs(label)]; //this should never fail
            fileOffset = xbimInst.FileOffset;
            if (xbimInst.IsLoaded)
                return ((XbimInstance)xbimInst).Entity;
            else
                return CreateEntity(model, (XbimInstanceHandle)xbimInst);
        }

        /// <summary>
        /// If the entity is in memory returns it, if not a blank instance is returned
        /// </summary>
        /// <param name="model"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        internal IPersistIfcEntity GetOrCreateEntity(IModel model, long label)
        {
            IXbimInstance xbimInst = this[Math.Abs(label)]; //this should never fail
            if (xbimInst.IsLoaded)
                return ((XbimInstance)xbimInst).Entity;
            else
                return CreateEntity(model, (XbimInstanceHandle)xbimInst);
        }


        internal bool Contains(long p)
        {
            return Keys.Contains(Math.Abs(p));
        }

        internal long GetFileOffset(long label)
        {
            IXbimInstance xbimInst = this[Math.Abs(label)]; //this should never fail
            return xbimInst.FileOffset;
        }
    }

    internal class IfcInstanceEnumerator : IEnumerator<IPersistIfcEntity>
    {
        private IPersistIfcEntity _current;
        private readonly IfcInstanceKeyedCollection _instances;
        private readonly IModel _model;
        private IEnumerator<IXbimInstance> _instanceEnumerator;

        public IfcInstanceEnumerator(IModel model, IfcInstanceKeyedCollection instances)
        {
            _instances = instances;
            _instanceEnumerator = instances.Values.GetEnumerator();
            _model = model;
            Reset();
        }

        #region IEnumerator<ISupportIfcParser> Members

        public object Current
        {
            get { return _current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        public bool MoveNext()
        {
            if (_instanceEnumerator.MoveNext()) //can we get an instance
            {
                IXbimInstance  c = _instanceEnumerator.Current;
                if (c.IsLoaded)
                    _current = ((XbimInstance)c).Entity;
                else
                    _current = _instances.CreateEntity(_model, (XbimInstanceHandle)c);
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            _current = null;
           _instanceEnumerator.Reset();
        }

        #endregion

        #region IEnumerator<ISupportIfcParser> Members

        IPersistIfcEntity IEnumerator<IPersistIfcEntity>.Current
        {
            get { return _current; }
        }

        #endregion
    }



}
