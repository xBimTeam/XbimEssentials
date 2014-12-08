#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcActorRole.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ActorResource
{
    [IfcPersistedEntityAttribute]
    public class ActorRoleCollection : XbimList<IfcActorRole>, IFormattable
    {
        internal ActorRoleCollection(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        #region IFormattable Members

        /// <summary>
        ///   List the actor roles delimited by a comma and a space
        /// </summary>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (IfcActorRole item in this)
            {
                if (str.Length != 0)
                    str.AppendLine();
                str.Append(item.ToString());
            }
            return str.ToString();
        }

        /// <summary>
        ///   Special format method for the properties of a Label collection
        /// </summary>
        /// <remarks>
        ///   Format strings as a delimited list, use {D} followed by any sequence of characters to act as the delimiter
        /// </remarks>
        /// <param name = "format">
        ///   use {D} followed by any sequence of characters to act as the delimiter
        /// </param>
        /// <param name = "formatProvider">
        ///   Provide an interface to an object to contro formatting
        /// </param>
        /// <returns>String with the formatted result.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return ToString();
            if (format[0] == 'D') //delimited list
            {
                string delim = format.Substring(1);
                StringBuilder str = new StringBuilder();
                foreach (IfcActorRole item in this)
                {
                    if (str.Length != 0)
                        str.Append(delim);
                    str.Append(item.ToString());
                }
                return str.ToString();
            }
            else
                throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Invalid format string: '{0}'.",
                                                        format));
        }

        #endregion

        [Browsable(true)]
        public string Summary
        {
            get { return this.ToString(); }
        }


        internal bool IsEquivalent(ActorRoleCollection roles)
        {
            return this.ToString() == roles.ToString();
        }
    }

    /// <summary>
    ///   A role which is performed by an actor, either a person, an organization or a person related to an organization. 
    ///   NOTE: The list of roles of the enumeration values of the Role attribute can never be complete. 
    ///   Therefore using enumeration value USERDEFINED, the user can provide his/her own role as a value of the attribute UserDefinedRole. 
    ///   NOTE Corresponds to STEP names: organization_role and person_role, please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard.
    /// </summary>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcActorRole : IFormattable, IPersistIfcEntity, ISupportChangeNotification, INotifyPropertyChanged,
                                INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcActorRole root = (IfcActorRole)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcActorRole left, IfcActorRole right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcActorRole left, IfcActorRole right)
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

        private IfcRole _role = IfcRole.Architect;
        private IfcLabel? _userDefinedRole;
        private IfcText? _description;

        #endregion

        #region Constructors & Initialisers

        public IfcActorRole()
        {
        }

        /// <summary>
        ///   Creates a new actor role with the string role, will match a role or create a user-defined if necessary
        /// </summary>
        public IfcActorRole(IfcLabel roleString)
        {
            ConvertRoleString(roleString, ref _role, ref _userDefinedRole);
        }

        /// <summary>
        ///   Creates a new actorrole and initializes with the role
        /// </summary>
        public IfcActorRole(IfcRole role)
        {
            this.Role = role;
        }

        public IfcActorRole(IfcActorRole cloneFrom, bool deep)
        {
            _role = cloneFrom.Role;
            _userDefinedRole = cloneFrom.UserDefinedRole;
            _description = cloneFrom.Description;
        }

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   The name of the role played by an actor. If the Role requires the value USERDEFINED, then set the UserDefinedRole property, this will automatically set this value to UserDefined.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory), Browsable(false)]
        public IfcRole Role
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _role;
            }
            set { this.SetModelValue(this, ref _role, value, v => Role = v, "Role"); }
        }

        /// <summary>
        ///   Allows for specification of user defined roles beyond the values in the Role enumeration
        /// </summary>
        /// <value>it should be restricted to max. 255 characters.</value>
        [IfcAttribute(2, IfcAttributeState.Optional), Browsable(false)]
        public IfcLabel? UserDefinedRole
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedRole;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedRole, value, v => UserDefinedRole = v,
                                           "UserDefinedRole");
            }
        }

        /// <summary>
        ///   A textual description relating the nature of the role played by an actor.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional), Browsable(true)]
        public IfcText? Description
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _description;
            }
            set { this.SetModelValue(this, ref _description, null, v => Description = v, "Description"); }
        }

        #endregion

        #region Part 21 Step file Parse routines

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _role = (IfcRole) Enum.Parse(typeof (IfcRole), value.EnumVal, true);
                    break;
                case 1:
                    _userDefinedRole = value.StringVal;
                    break;
                case 2:
                    _description = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Converts a string to a Role or a User defined role if necessary
        /// </summary>
        /// <param name = "value"></param>
        private void ConvertRoleString(string value, ref IfcRole role, ref IfcLabel? userDefinedRole)
        {
            if (string.IsNullOrEmpty(value)) return; //illegal to set a role to nothing
            string roleStr = value.Trim();

            string roleWithoutSpaces = roleStr.Replace(" ", "");
            if (Enum.IsDefined(typeof (IfcRole), roleWithoutSpaces))
            {
                IfcRole roleEnum = (IfcRole) Enum.Parse(typeof (IfcRole), roleWithoutSpaces, true);
                role = roleEnum; //call this to ensure correct change notification
                userDefinedRole = null;
            }
            else
            {
                userDefinedRole = roleStr;
                role = IfcRole.UserDefined;
            }
        }

        /// <summary>
        ///   Gets or Sets the Role, if the name provided matches on of the Role enums, the enum is selected, otherwise a userdefined role is created. Use this to simplify binding
        /// </summary>
        public string RoleString
        {
            get { return this.ToString(); }
            set
            {
                string old = RoleString;
                IfcLabel? userDefinedRole = "";
                IfcRole role = new IfcRole();
                ConvertRoleString(value, ref role, ref userDefinedRole);
                Role = role;
                UserDefinedRole = userDefinedRole;
            }
        }

        [Browsable(true)]
        public string Summary
        {
            get { return this.ToString(); }
        }

        #endregion

        #region Ifc Inverse Relationships

        #endregion

        #region IFormattable Members

        public override string ToString()
        {
            if (this.Role == IfcRole.UserDefined && UserDefinedRole.HasValue)
                return UserDefinedRole.GetValueOrDefault();
            else
                return this.Role.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return ToString();
            char prop = format[0];
            string delim = format.Substring(1);
            switch (prop)
            {
                case 'R':
                    return Role + delim;
                case 'D':
                    return string.IsNullOrEmpty(Description.GetValueOrDefault()) ? "" : Description + delim;
                case 'U':
                    return string.IsNullOrEmpty(UserDefinedRole.GetValueOrDefault()) ? "" : UserDefinedRole + delim;
                case 'X':
                    return string.Format(CultureInfo.CurrentCulture, "{0}, {1}", ToString(), Description);

                default:
                    throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Invalid format string: '{0}'.",
                                                            format));
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
            if (_role == IfcRole.UserDefined && !_userDefinedRole.HasValue)
                return
                    "WR1 ActorRole: When attribute Role has enumeration value USERDEFINED then attribute UserDefinedRole shall also have a value.";
            else
                return "";
        }

        #endregion
    }
}