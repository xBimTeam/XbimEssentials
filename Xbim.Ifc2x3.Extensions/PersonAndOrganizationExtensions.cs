using Xbim.Ifc2x3.ActorResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class PersonAndOrganizationExtensions
    {
        public static bool HasEmail(this IfcPersonAndOrganization personOrg, string emailAddress)
        {
            return personOrg.TheOrganization.HasEmail(emailAddress) || personOrg.ThePerson.HasEmail(emailAddress);
        }
    }
}
