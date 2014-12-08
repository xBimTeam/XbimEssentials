#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPerson.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ActorResource
{
    [IfcPersistedEntityAttribute]
    public class PersonCollection : XbimList<IfcPerson>
    {
        internal PersonCollection(IPersistIfcEntity owner)
            : base(owner)
        {
        }


        /// <summary>
        ///   Finds the person with either the ID of id or if the ID is null the FamilyName == id. Only returns the first match or null if none found
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        public IfcPerson this[IfcIdentifier id]
        {
            get
            {
                foreach (IfcPerson p in this)
                {
                    if (string.IsNullOrEmpty(p.Id.GetValueOrDefault()) && p.FamilyName.GetValueOrDefault() == id)
                        return p;
                    else if (p.Id == id) return p;
                }
                return null;
            }
        }
    }

    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcPerson : IfcActorSelect, IPersistIfcEntity, IFormattable, ISupportChangeNotification,
                             INotifyPropertyChanged, IfcObjectReferenceSelect, INotifyPropertyChanging
    {

        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcPerson root = (IfcPerson)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcPerson left, IfcPerson right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcPerson left, IfcPerson right)
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

        private IfcIdentifier? _id;
        private IfcLabel? _familyName;
        private IfcLabel? _givenName;
        private LabelCollection _middleNames;
        private LabelCollection _prefixTitles;
        private LabelCollection _suffixTitles;
        private ActorRoleCollection _roles;
        private AddressCollection _addresses;

        #endregion

        #region Constructors & Initialisers

        #endregion

        #region Properties

     


        #endregion

        #region Methods

        protected string GivenNameString(string appendIfNotEmpty)
        {
            if (!string.IsNullOrEmpty(GivenName.GetValueOrDefault()))
                return GivenName + appendIfNotEmpty;
            else
                return "";
        }

        protected string FamilyNameString(string appendIfNotEmpty)
        {
            if (!string.IsNullOrEmpty(FamilyName.GetValueOrDefault()))
                return FamilyName + appendIfNotEmpty;
            else
                return "";
        }


        protected string PrefixTitlesDelimited(string delimiter)
        {
            return PrefixTitlesDelimited(delimiter, "");
        }

        protected string PrefixTitlesDelimited(string delimiter, string appendIfNotEmpty)
        {
            string ret = PrefixTitles == null ? "" : PrefixTitles.ToString("D" + delimiter, null);
            //return delimited string
            if (!string.IsNullOrEmpty(ret))
                ret += appendIfNotEmpty;
            return ret;
        }


        protected string SuffixTitlesDelimited(string delimiter)
        {
            return SuffixTitlesDelimited(delimiter, "");
        }

        protected string SuffixTitlesDelimited(string delimiter, string appendIfNotEmpty)
        {
            string ret = SuffixTitles == null ? "" : SuffixTitles.ToString("D" + delimiter, null);
            //return delimited string
            if (!string.IsNullOrEmpty(ret))
                ret += appendIfNotEmpty;
            return ret;
        }


        protected string MiddleNamesDelimited(string delimiter)
        {
            return MiddleNamesDelimited(delimiter, "");
        }

        protected string MiddleNamesDelimited(string delimiter, string appendIfNotEmpty)
        {
            string ret = MiddleNames == null ? "" : MiddleNames.ToString("D" + delimiter, null);
            //return delimited string 
            if (!string.IsNullOrEmpty(ret))
                ret += appendIfNotEmpty;
            return ret;
        }

        public string RolesString
        {
            get { return Roles == null ? null : Roles.ToString("D; ", null); }

        }
 
        protected string RolesDelimited(string delimiter)
        {
            return RolesDelimited(delimiter, "");
        }

        protected string RolesDelimited(string delimiter, string appendIfNotEmpty)
        {
            string ret = Roles == null ? "" : Roles.ToString("D" + delimiter, null); //return delimited string
            if (!string.IsNullOrEmpty(ret))
                ret += appendIfNotEmpty;
            return ret;
        }

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Optional Identification of the person
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
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
        ///   Optional.   The name by which the family identity of the person may be recognized.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLabel? FamilyName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _familyName;
            }
            set { this.SetModelValue(this, ref _familyName, value, v => FamilyName = v, "FamilyName"); }
        }

        /// <summary>
        ///   Optional. The name by which a person is known within a family and by which he or she may be familiarly recognized.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? GivenName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _givenName;
            }
            set { this.SetModelValue(this, ref _givenName, value, v => GivenName = v, "GivenName"); }
        }

        /// <summary>
        ///   Optional. Additional names given to a person that enable their identification apart from others who may have the same or similar family and given names.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional, IfcAttributeType.List)]
        public LabelCollection MiddleNames
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _middleNames;
            }
            set { this.SetModelValue(this, ref _middleNames, value, v => MiddleNames = v, "MiddleNames"); }
        }

        /// <summary>
        ///   Optional The word, or group of words, which specify the person's social and/or professional standing and appear before his/her names.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional, IfcAttributeType.List)]
        public LabelCollection PrefixTitles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _prefixTitles;
            }
            set { this.SetModelValue(this, ref _prefixTitles, value, v => PrefixTitles = v, "PrefixTitles"); }
        }

        /// <summary>
        ///   Optional. The word, or group of words, which specify the person's social and/or professional standing and appear after his/her names.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional, IfcAttributeType.List)]
        public LabelCollection SuffixTitles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _suffixTitles;
            }
            set { this.SetModelValue(this, ref _suffixTitles, value, v => SuffixTitles = v, "SuffixTitles"); }
        }

        /// <summary>
        ///   Optional. Roles played by the person
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional, IfcAttributeType.List)]
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
        ///   Optional. Postal and telecommunication addresses of a person.
        /// </summary>
        /// <remarks>
        ///   NOTE - A person may have several addresses.
        /// </remarks>

        [IfcAttribute(8, IfcAttributeState.Optional, IfcAttributeType.List)]
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

        #region Part 21 Step file Parse routines

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _id = value.StringVal;
                    break;
                case 1:
                    _familyName = value.StringVal;
                    break;
                case 2:
                    _givenName = value.StringVal;
                    break;
                case 3:
                    if (_middleNames == null) _middleNames = new LabelCollection(this);
                    ((IXbimNoNotifyCollection)_middleNames).Add(new IfcLabel(value.StringVal));
                    break;
                case 4:
                    if (_prefixTitles == null) _prefixTitles = new LabelCollection(this);
                    ((IXbimNoNotifyCollection)_prefixTitles).Add(new IfcLabel(value.StringVal));
                    break;
                case 5:
                    if (_suffixTitles == null) _suffixTitles = new LabelCollection(this);
                    ((IXbimNoNotifyCollection)_suffixTitles).Add(new IfcLabel(value.StringVal));
                    break;
                case 6:
                    if (_roles == null) _roles = new ActorRoleCollection(this);
                    ((IXbimNoNotifyCollection)_roles).Add((IfcActorRole)value.EntityVal);
                    break;
                case 7:
                    if (_addresses == null) _addresses = new AddressCollection(this);
                    ((IXbimNoNotifyCollection)_addresses).Add((IfcAddress)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverse Relationships

        //TODO: Resolve
        //////[Browsable(false)]
        //////public IEnumerable<PersonAndOrganization> EngagedIn
        //////{
        //////    get
        //////    {
        //////        if (AddressBook == null) return null;
        //////        return from PersonAndOrganization instance in AddressBook.PersonAndOrganizations
        //////               where instance.ThePerson == this
        //////               select instance;
        //////    }
        //////}

        #endregion

        #region IFormattable Members

        public override string ToString()
        {
            string str = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}{4}{5}",
                                       PrefixTitlesDelimited(", ", ", "),
                                       GivenNameString(", "),
                                       MiddleNamesDelimited(", ", ", "),
                                       FamilyNameString(", "),
                                       SuffixTitlesDelimited(", ", ", "),
                                       RolesDelimited(", ", ""));
            return str.TrimEnd(',', ' ');
        }

        /// <summary>
        ///   Special format method for the properties of a Person
        /// </summary>
        /// <remarks>
        ///   Format string in two parts. {FormatChar}{Text}. i.e. "F,"
        ///   Text is any arbitrary text to appear after the formatted text. If the formatted text is an empty string the arbitrary text is not appended. Where there is a list of values these are listed and delimited by the arbitrary text.
        /// </remarks>
        /// <param name = "format">
        ///   Format string in two parts. {FormatChar}{Text}. i.e. "F,"
        ///   Text is any arbitrary text to appear after the formatted text. If the formatted text is an empty string the arbitrary text is not appended. Where there is a list of values these are listed and delimited by the arbitrary text.
        ///   'I' = Identifier
        ///   'F' = FirstName
        ///   'M' = List of Middle names
        ///   'G' = Given or first name
        ///   'P' = List of Prefix Titles (Dr. Mr. etc)
        ///   'S' = List of Suffix titles
        ///   'R' = List of Roles (Engineer, Architect)
        /// </param>
        /// <param name = "formatProvider">
        /// </param>
        /// <returns>String with the formatted result.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // If no format is passed, display like this: (x, y).
            if (string.IsNullOrEmpty(format)) return ToString();
            char prop = format[0];
            string delim = format.Substring(1);

            switch (prop)
            {
                case 'I':
                    return string.IsNullOrEmpty(Id.GetValueOrDefault()) ? "" : Id + delim;
                case 'F':
                    return FamilyNameString(delim);
                case 'G':
                    return GivenNameString(delim);
                case 'M':
                    return MiddleNamesDelimited(delim);
                case 'P':
                    return PrefixTitlesDelimited(delim);
                case 'S':
                    return SuffixTitlesDelimited(delim);
                case 'R':
                    return RolesDelimited(delim);
                default:
                    throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Invalid format string: '{0}'.",
                                                            format));
            }
        }

        #endregion

        #region Add Methods

        #endregion

        #region Set Collections

        public void SetMiddleNames(params string[] middleNames)
        {
            if (_middleNames == null) _middleNames = new LabelCollection(this);
            else
                _middleNames.Clear();
            foreach (string item in middleNames)
            {
                _middleNames.Add(item);
            }
        }

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

        ///<summary>
        ///  Sets the AddressCollection to the array of IfcTelecomAddress, deletes any previous values, initialises collection.
        ///  Should test to see if Addresses exists before calling unless a new list is required, if not use Add or Add_Reversible if within a transaction 
        ///</summary>
        public void SetTelecomAddresss(params IfcTelecomAddress[] telecomAddress)
        {
            if (_addresses == null) _addresses = new AddressCollection(this);
            else
                _addresses.Clear();
            foreach (IfcTelecomAddress item in telecomAddress)
            {
                _addresses.Add(item);
            }
        }

        ///<summary>
        ///  Sets the AddressCollection to the array of IfcPostalAddress, deletes any previous values, initialises collection.
        ///  Should test to see if Addresses exists before calling unless a new list is required, if not use Add or Add_Reversible if within a transaction 
        ///</summary>
        public void SetPostalAddresss(params IfcPostalAddress[] telecomAddress)
        {
            if (_addresses == null) _addresses = new AddressCollection(this);
            else
                _addresses.Clear();
            foreach (IfcPostalAddress item in telecomAddress)
            {
                _addresses.Add(item);
            }
        }



        #endregion

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
            if (!_givenName.HasValue && !_familyName.HasValue)
                return "WR1 Person: Requires that either the family name or the given name is used.\n";
            else
                return "";
        }

        #endregion
    }
}
