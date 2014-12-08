using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ActorResource;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ActorSelectExtensions
    {

        public static String RoleName(this IfcActorSelect actor)
        {
            if (actor is IfcPerson)
            {
                return ((IfcPerson)actor).RolesString;
            }
            else if (actor is IfcPersonAndOrganization)
            {
                return ((IfcPersonAndOrganization)actor).RolesString;
               
            }
            else if (actor is IfcOrganization)
            {
                return ((IfcOrganization)actor).RolesString;
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
