using System;
using System.ComponentModel;
using System.IO;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.IO.Step21;

namespace Xbim.Common
{
    public abstract class PersistEntity : IPersistEntity, INotifyPropertyChanged
    {
        protected PersistEntity(IModel model, int label, bool activated)
        {
            Model = model;
            EntityLabel = label;
            _activated = activated;
        }

        #region IPersistEntity
        public int EntityLabel { get; private set; }

        public IModel Model { get; private set; }

        /// <summary>
        /// This property is deprecated and likely to be removed. Use just 'Model' instead.
        /// </summary>
        [Obsolete("This property is deprecated and likely to be removed. Use just 'Model' instead.")]
        public IModel ModelOf { get { return Model; } }

        internal protected bool _activated;
        bool IPersistEntity.Activated { get { return _activated; } }

        protected void Activate()
        {
            //only activate once in a lifetime
            if (_activated)
                return;

            Model.Activate(this);
        }

        ExpressType IPersistEntity.ExpressType { get { return Model.Metadata.ExpressType(this); } }
        #endregion

        #region IPersist
        public abstract void Parse(int propIndex, IPropertyValue value, int[] nested);
        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Transactional property setting

        protected void SetValue<TProperty>(Action<TProperty> setter, TProperty oldValue, TProperty newValue, string notifyPropertyName, int propertyOrder)
        {
            //activate if it is not activated yet so that values don't get overwritten once activated
            if (!_activated)
                Activate();

            //just set the value if the model is marked as non-transactional
            if (!Model.IsTransactional)
            {
                setter(newValue);
                NotifyPropertyChanged(notifyPropertyName);
                return;
            }

            //check there is a transaction
            var txn = Model.CurrentTransaction;
            if (txn == null) throw new Exception("Operation out of transaction.");

            void doAction()
            {
                setter(newValue);
                NotifyPropertyChanged(notifyPropertyName);
            }
            void undoAction()
            {
                setter(oldValue);
                NotifyPropertyChanged(notifyPropertyName);
            }

            txn.DoReversibleAction(doAction, undoAction, this, ChangeType.Modified, propertyOrder);
        }

        #endregion

        #region Equals & GetHashCode
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            if (!(obj is IPersistEntity entity)) return false;

            return EntityLabel.Equals(entity.EntityLabel) && Model.Equals(entity.Model);
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode();
        }
        #endregion

        #region Equality operators
        public static bool operator ==(PersistEntity left, object right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            var entity = right as IPersistEntity;
            if (ReferenceEquals(entity, null))
                return false;

            return (left.EntityLabel == entity.EntityLabel) && left.Model.Equals(entity.Model);

        }

        public static bool operator !=(PersistEntity left, object right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString()
        {
            ExpressMetaData md = null;
            if (Model != null)
                md = Model.Metadata;
            else
                md = ExpressMetaData.GetMetadata(GetType().Module);

            using (var sw = new StringWriter())
            {
                Part21Writer.WriteEntity(this, sw, md);
                return sw.ToString();
            }
        }
    }
}
