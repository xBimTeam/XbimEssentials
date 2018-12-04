using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.IO.Step21;

namespace Xbim.Common.Delta
{
    public class TransactionLog: IDisposable
    {
        private ITransaction _transaction;
        private readonly Dictionary<int, EntityChange> _log = new Dictionary<int, EntityChange>();

        /// <summary>
        /// All new, changed or deleted entities except for these which were created and deleted in transaction
        /// </summary>
        public IEnumerable<EntityChange> Changes { get { return _log.Values.Where(v => !v.IsNewDeleted); } } 

        /// <summary>
        /// Transaction log which listens to all changes within a transaction. 
        /// Thic constructor should always be used in 'using' statement so it releases reference to transaction.
        /// </summary>
        /// <param name="transaction">Transaction to be used as a scope of this log</param>
        public TransactionLog(ITransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");

            _transaction = transaction;
            _transaction.EntityChanging += EntityChanging;
            _transaction.EntityChanged+= EntityChanged;
        }

        private void EntityChanged(IPersistEntity entity, ChangeType change, int property)
        {
            _log[entity.EntityLabel].AddChanged(change, property);
        }

        private void EntityChanging(IPersistEntity entity, ChangeType change, int property)
        {
            EntityChange entry;
            if (!_log.TryGetValue(entity.EntityLabel, out entry))
            {
                entry = new EntityChange(entity);
                _log.Add(entity.EntityLabel, entry);
            }

            entry.AddChanging(change, property);
        }

        public void Dispose()
        {
            if (_transaction == null) return;

            _transaction.EntityChanged -= EntityChanged;
            _transaction.EntityChanging -= EntityChanging;
            _transaction = null;
        }
    }

    public class EntityChange
    {
        private readonly IPersistEntity _entity;
        private readonly ExpressType _eType;
        private readonly string _oldEntity;
        private readonly List<ChangeType> _changeTypes = new List<ChangeType>();
        private readonly Dictionary<int, PropertyInfo> _changedProperties = new Dictionary<int, PropertyInfo>();
        private readonly Dictionary<int, string> _oldValues = new Dictionary<int, string>();
        private readonly Dictionary<int, int> _dummy = new Dictionary<int, int>(); 
        private readonly StringWriter _writer = new StringWriter();

        internal EntityChange(IPersistEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _entity = entity;
            _eType = _entity.ExpressType;
            _oldEntity = GetEntityString();
        }

        internal void AddChanging(ChangeType type, int property)
        {
            //add change type for every change
            _changeTypes.Add(type);

            //it is not a change or it is not a property from this schema
            if (type != ChangeType.Modified || property <= 0)
                return;

            //it was changed before so we don't need to get the old value again
            if (_changedProperties.ContainsKey(property))
                return;

            //add property info for a future use
            var prop = _eType.Properties[property];
            _changedProperties.Add(property, prop.PropertyInfo);

            //keep old value of the property
            var value = GetPropertyString(property);
            _oldValues.Add(property, value);
        }

        internal void AddChanged(ChangeType type, int property)
        {
            if (property <= 0)
                return;

            if (_changedProperties.ContainsKey(property))
                return;

            throw new XbimException("Property change wasn't notified before the actual change. Change log will be inconsistent.");
        }

        /// <summary>
        /// Entity which changes are described in this log entry
        /// </summary>
        public IPersistEntity Entity { get { return _entity; } }

        /// <summary>
        /// Prioritized log. Priorities are as follows: Deleted > New > Modified.
        /// This assumes that if entity is deleted at the end it is not very important what happened in the meantime.
        /// If it is new it is not that important if it was modified because full entity is interesting.
        /// Modifications are interesting in all other cases. Special case is entity which was created and deleted within a scope
        /// of one transaction but this should already be filtered out from the transaction log.
        /// </summary>
        public ChangeType ChangeType
        {
            get
            {
                if (_changeTypes.Any(t => t == ChangeType.Deleted))
                    return ChangeType.Deleted;
                if (_changeTypes.Any(t => t == ChangeType.New))
                    return ChangeType.New;
                return ChangeType.Modified;
            }
        }

        internal bool IsNewDeleted { get
        {
            return _changeTypes.Contains(ChangeType.New) && _changeTypes.Contains(ChangeType.Deleted);
        } }

        /// <summary>
        /// String representation of the entity before the first change was made to it. If this is a new entity this is empty string.
        /// </summary>
        public string OriginalEntity { get { return  ChangeType == ChangeType.New ? "" : _oldEntity; } }
        /// <summary>
        /// Current string representation of the entity. This is computed dynamically when you ask for 
        /// this so make sure not to ask for it after further modifications.
        /// </summary>
        public string CurrentEntity { get { return ChangeType == ChangeType.Deleted ? "" : GetEntityString(); } }

        /// <summary>
        /// Changed properties within a scope of transaction. Current values are evaluated when you enumerate this property so
        /// make sure to ask for it before you do any further modifications. This property will return empty enumeration if 
        /// ChangeType is anything else than 'Modified'.
        /// </summary>
        public IEnumerable<PropertyChange> ChangedProperties
        {
            get
            {
                if (ChangeType != ChangeType.Modified)
                    return Enumerable.Empty<PropertyChange>();

                return _changedProperties.Select(kvp => new PropertyChange
                {
                    PropertyInfo = kvp.Value,
                    Name = kvp.Value.Name,
                    Order = kvp.Key,
                    OriginalValue = _oldValues[kvp.Key],
                    CurrentValue = GetPropertyString(kvp.Key)
                });
            }
        }




        private string GetPropertyString(int order)
        {
            var info = _changedProperties[order];
            Part21Writer.WriteProperty(info.PropertyType, info.GetValue(_entity, null), _writer, _dummy, _entity.Model.Metadata);
            var value = _writer.ToString();
            ClearWriter();
            return value;
        }

        private string GetEntityString()
        {
            Part21Writer.WriteEntity(_entity, _writer, _entity.Model.Metadata, _dummy);
            var value = _writer.ToString();
            ClearWriter();
            return value;
        }

        private void ClearWriter()
        {
            var sb = _writer.GetStringBuilder();
            sb.Remove(0, sb.Length);
        }
    }

    public class PropertyChange
    {
        internal PropertyChange() { }

        public string Name { get; internal set; }
        public int Order { get; set; }
        public PropertyInfo PropertyInfo { get; internal set; }
        public string OriginalValue { get; internal set; }
        public string CurrentValue { get; internal set; }
    }
}
