using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;



namespace Xbim.Ifc4.Interfaces
{
    /// <summary>
    /// Readonly interface for IfcActorRole
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial interface @IIfcActorRole
    {
        string RoleString { get; }
    }
}

namespace Xbim.Ifc4.ActorResource
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class IfcActorRole
    {

        /// <summary>
        ///   Converts a string to a Role or a User defined role if necessary
        /// </summary>
        /// <param name = "value"></param>
        /// <param name="role"></param>
        /// <param name="userDefinedRole"></param>
        private static void ConvertRoleString(string value, ref IfcRoleEnum role, ref IfcLabel? userDefinedRole)
        {
            if (string.IsNullOrEmpty(value)) return; //illegal to set a role to nothing
            var roleStr = value.Trim();

            var roleWithoutSpaces = roleStr.Replace(" ", "");
            if (Enum.IsDefined(typeof(IfcRoleEnum), roleWithoutSpaces))
            {
                var roleEnum = (IfcRoleEnum)Enum.Parse(typeof(IfcRoleEnum), roleWithoutSpaces, true);
                role = roleEnum; //call this to ensure correct change notification
                userDefinedRole = null;
            }
            else
            {
                userDefinedRole = roleStr;
                role = IfcRoleEnum.USERDEFINED;
            }
        }

        /// <summary>
        ///   Gets or Sets the Role, if the name provided matches on of the Role enums, the enum is selected, otherwise a userdefined role is created. Use this to simplify binding
        /// </summary>
        public string RoleString
        {
            get
            {
                if (Role == IfcRoleEnum.USERDEFINED)
                    return UserDefinedRole;
                return Role.ToString();
            }
            set
            {
                IfcLabel? userDefinedRole = "";
                var role = new IfcRoleEnum();
                ConvertRoleString(value, ref role, ref userDefinedRole);
                Role = role;
                UserDefinedRole = userDefinedRole;
            }
        }
    }

}
