using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO.Parser
{
    public class XbimIndexEntry
    {
        public XbimIndexEntry(long entityLabel, long offset, Type type)
        {
            _entityLabel = entityLabel;
            _offset = offset;
            _type = type;
            _entityRef = null;
        }

        /// <summary>
        ///   Constructs an entry for a completely new instance of a type, offset == 0, Type == typeof entity
        /// </summary>
        public XbimIndexEntry(long entityLabel, IPersistIfcEntity entity)
        {
            _entityLabel = entityLabel;
            _offset = 0;
            _type = entity.GetType();
            _entityRef = null;
            Entity = entity;
        }

        public XbimIndexEntry(XbimIndexEntry xbimIndexEntry)
        {
            _entityLabel = xbimIndexEntry._entityLabel;
            _offset = xbimIndexEntry._offset;
            _type = xbimIndexEntry._type;
            _entityRef = null;
            Entity = xbimIndexEntry.Entity;
        }

        private long _entityLabel;

        public long EntityLabel
        {
            get { return _entityLabel; }
            set { _entityLabel = value; }
        }
        private long _offset;

        public long Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        private Type _type;

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private IPersistIfcEntity _entityRef;

        /// <summary>
        ///   Drops an object from memory cache
        /// </summary>
        public void Drop()
        {
            _entityRef = null;
        }

        public IPersistIfcEntity Entity
        {
            get
            {
                return _entityRef;

                //if (_entityRef != null)
                //    return (IPersistIfcEntity)_entityRef.Target;
                //else if (_offset == 0 && _type != null)
                ////we have a newly created entity but it has been released by garbage collector
                //{
                //    IPersistIfcEntity newEntity = (IPersistIfcEntity)Activator.CreateInstance(_type);
                //    _entityRef = new WeakReference(newEntity);
                //    return newEntity;
                //}
                //else
                //    return null;
            }
            set
            {
                _entityRef = value;
                //if (_entityRef != null)
                //    _entityRef.Target = value;
                //else
                //    _entityRef = new WeakReference(value);
            }
        }
    }

}
