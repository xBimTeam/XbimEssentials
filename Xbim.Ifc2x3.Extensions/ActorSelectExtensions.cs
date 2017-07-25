using System;
using System.Linq;
using Xbim.Ifc2x3.ActorResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ActorSelectExtensions
    {

        public static String RoleName(this IfcActorSelect actor)
        {
            var person = actor as IfcPerson;
            if (person != null)
            {
                return string.Join("; ", person.Roles.Select(r => r.RoleString)); 
            }
            var personAndOrganization = actor as IfcPersonAndOrganization;
            if (personAndOrganization != null)
            {
                return string.Join("; ", personAndOrganization.Roles.Select(r => r.RoleString));
            }
            var organization = actor as IfcOrganization;
            if (organization != null)
            {
                return string.Join("; ", organization.Roles.Select(r => r.RoleString));
            }
            return "";
        }

        public static bool HasEmail(this IfcActorSelect actor, string emailAddress)
        {
            var personOrg = actor as IfcPersonAndOrganization;
            if (personOrg != null)
                return personOrg.HasEmail(emailAddress);
            var person = actor as IfcPerson;
            if (person != null)
                return person.HasEmail(emailAddress);
            var organisation = actor as IfcOrganization;
            if (organisation != null) 
                return organisation.HasEmail(emailAddress);
            return false;
        }
    }
}
