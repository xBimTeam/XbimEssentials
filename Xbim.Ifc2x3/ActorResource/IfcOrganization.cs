#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOrganization.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ActorResource
{
    [IfcPersistedEntityAttribute]
    public class OrganizationCollection : XbimList<IfcOrganization>
    {
        internal OrganizationCollection(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        /// <summary>
        ///   Finds the organization with either the ID of id or if the ID is null the Name == id. Only returns the first match or null if none found
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        public IfcOrganization this[IfcIdentifier id]
        {
            get
            {
                foreach (IfcOrganization o in this)
                {
                    if (string.IsNullOrEmpty(o.Id.GetValueOrDefault()) && o.Name == id)
                        return o;
                    else if (o.Id == id) return o;
                }
                return null;
            }
        }
    }


    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcOrganization : IfcActorSelect, IPersistIfcEntity, IFormattable, ISupportChangeNotification,
                                   INotifyPropertyChanged, IfcObjectReferenceSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcOrganization root = (IfcOrganization)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcOrganization left, IfcOrganization right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcOrganization left, IfcOrganization right)
        {
            return !(left == right);
        }

        #region IPersistIfcEntity Members

        private int _entityLabel;
		bool _activated;

        private IModel _model;

        public IModel ModelOf
        {
            get { return _model; }
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _activated=activated;
			_model = model;
            _entityLabel = entityLabel;
        }

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        public int EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock(this) { if (_model != null && !_activated) _activated = _model.Activate(this, false)>0;  }
            if (write) _model.Activate(this, write);
        }

        #endregion


        #region Fields and Events

        //persistent fields
        private IfcIdentifier? _id;
        private IfcLabel _name;
        private IfcText? _description;
        private ActorRoleCollection _roles;
        private AddressCollection _addresses;

        #endregion

        #region Properties

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Optional. Identification of the organization.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional), Browsable(true)]
        public IfcIdentifier? Id
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _id;
            }
            set { this.SetModelValue(this, ref _id, value, v => Id = v, "Id"); }
        }

        /// <summary>
        ///   The word, or group of words, by which the organization is referred to.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory), Browsable(true)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }

        /// <summary>
        ///   Optional.   Text that relates the nature of the organization.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional), Browsable(true)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, value, v => Description = v, "Description"); }
        }


        /// <summary>
        ///   Optional.   Roles played by the organization.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional, IfcAttributeType.List, 1), Browsable(true)]
        public ActorRoleCollection Roles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _roles;
            }
            set { this.SetModelValue(this, ref _roles, value, v => Roles = v, "Roles"); }
        }

        /// <summary>
        ///   Optional.   Postal and telecom addresses of an organization
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional, IfcAttributeType.List, 1), Browsable(true)]
        public AddressCollection Addresses
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _addresses;
            }
            set { this.SetModelValue(this, ref _addresses, value, v => Addresses = v, "Addresses"); }
        }

        #endregion
        public string RolesString
        {
            get { return Roles == null ? null : Roles.ToString("D; ", null); }

        }
        #region Part 21 Step file Parse routines

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _id = value.StringVal;
                    break;
                case 1:
                    _name = value.StringVal;
                    break;
                case 2:
                    _description = value.StringVal;
                    break;
                case 3:
                    if (_roles == null) _roles = new ActorRoleCollection(this);
                    ((IXbimNoNotifyCollection)_roles).Add((IfcActorRole) value.EntityVal);
                    break;
                case 4:
                    if (_addresses == null) _addresses = new AddressCollection(this);
                    ((IXbimNoNotifyCollection)_addresses).Add((IfcAddress) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Inverse Relationships

        /// <summary>
        ///   The inverse relationship for relationship RelatedOrganizations of IfcOrganizationRelationship.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcOrganizationRelationship> IsRelatedBy
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcOrganizationRelationship>(
                        r => r.RelatedOrganizations.Contains(this));
            }
        }

        /// <summary>
        ///   The inverse relationship for relationship RelatingOrganization of IfcOrganizationRelationship.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcOrganizationRelationship> Relates
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcOrganizationRelationship>(
                        r => r.RelatingOrganization == this);
            }
        }

        /// <summary>
        ///   Inverse relationship to IfcPersonAndOrganization relationships in which IfcOrganization is engaged.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcPersonAndOrganization> Engages
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcPersonAndOrganization>(r => r.TheOrganization == this);
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        //none defined

        #endregion

        #region Properties

        //[Browsable(true)]
        //public string RolesString
        //{
        //    get 
        //    {
        //        return _roles == null ? null : _roles.ToString();
        //    }
        //    set
        //    {
        //        using (Transaction txn = BeginTransaction(string.Format("Roles = {0}", value), true))
        //        {
        //            if (string.IsNullOrEmpty(value))
        //            {
        //                Transaction.AddPropertyChange(v => _roles = v, _roles, null);
        //                _roles = null;
        //            }
        //            else
        //            {
        //                string[] roles = value.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
        //                if (string.Join("; ", roles) == RolesString) //no real change so exit transaction
        //                    return;
        //                if (_roles == null)
        //                {
        //                    ActorRoleCollection c = new ActorRoleCollection();
        //                    Transaction.AddPropertyChange(v => _roles = v, null, c);
        //                    _roles = c;
        //                }
        //                else
        //                    _roles.Clear_Reversible();

        //                if (roles != null)
        //                {
        //                    foreach (string item in roles)
        //                    {
        //                        ActorRole aRole = new ActorRole( item);
        //                        aRole.OnModelAdd(Model);
        //                        _roles.Add_Reversible(aRole);
        //                    }
        //                }
        //            }
        //            Transaction.AddTransactionReversedHandler(() => { NotifyPropertyChanged("Roles"); NotifyPropertyChanged("RolesString"); });
        //            if (txn != null) txn.Commit();
        //            NotifyPropertyChanged("Roles");
        //            NotifyPropertyChanged("RolesString");
        //        }
        //    }
        //}

        /// <summary>
        ///   A string representation of the Addresses Collection
        /// </summary>
        [Browsable(true)]
        public string AddressesString
        {
            get { return _addresses == null ? null : _addresses.ToString(); }
        }

        #endregion

        #region Add methods

        public void AddRole(IfcActorRole newRole)
        {
            if (Roles == null)
                _roles = new ActorRoleCollection(this);
            Roles.Add(newRole);
        }


        #endregion

        #region Set Collections
        ///<summary>
        ///  Sets the ActorRoleCollection to the array of ActorRole, deletes any previous values, initialises collection.
        ///</summary>
        public void SetRoles(params IfcActorRole[] actorRoles)
        {
            if (_roles == null) _roles = new ActorRoleCollection(this);
            else
                _roles.Clear();
            foreach (IfcActorRole item in actorRoles)
            {
                _roles.Add(item);
            }
        }
        #endregion

        #region IFormattable Members

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_name))
                return "Not defined";
            else
                return _name;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // If no format is passed, display like this: (x, y).
            if (string.IsNullOrEmpty(format)) return ToString();
            char prop = format[0];
            string delim = format.Substring(1);
            StringBuilder str = new StringBuilder();
            switch (prop)
            {
                case 'I':
                    return string.IsNullOrEmpty(Id.GetValueOrDefault()) ? "" : Id + delim;
                case 'N':
                    return string.IsNullOrEmpty(Name) ? "" : Name + delim;
                case 'D':
                    return string.IsNullOrEmpty(Description.GetValueOrDefault()) ? "" : Description + delim;
                case 'R':
                    if (Roles == null) return "";
                    foreach (IfcActorRole item in Roles)
                    {
                        if (str.Length != 0)
                            str.Append(delim);
                        str.Append(item);
                    }
                    return str.ToString();
                default:
                    throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Invalid format string: '{0}'.",
                                                            format));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        //TODO: Resolve

        //////internal void NotifyRelatesPropertyChanged(Organization relatedOrganization)
        //////{
        //////        relatedOrganization.NotifyPropertyChanged("IsRelatedBy");
        //////        relatedOrganization.NotifyPropertyChanged("Relates");
        //////        relatedOrganization.NotifyPropertyChanged("IsRelatedByString");
        //////        relatedOrganization.NotifyPropertyChanged("RelatesString");
        //////        NotifyPropertyChanged("IsRelatedBy");
        //////        NotifyPropertyChanged("Relates");
        //////        NotifyPropertyChanged("IsRelatedByString");
        //////        NotifyPropertyChanged("RelatesString");  
        //////}

        #endregion

        internal bool IsEquivalent(IfcOrganization o)
        {
            if (o == null) return false;
            return (o.Addresses.IsEquivalent(Addresses)
                    && o.Description == Description
                    && o.Id == Id
                    && o.Name ==Name
                    && o.Roles.IsEquivalent(Roles));
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        [field: NonSerialized] //don't serialize events
            private event PropertyChangingEventHandler PropertyChanging;

        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }

        #endregion

        #region ISupportChangeNotification Members

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}
