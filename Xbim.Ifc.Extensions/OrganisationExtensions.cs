using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ActorResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class OrganisationExtensions
    {
        public static bool HasEmail(this IfcOrganization ifcOrg, string emailAddress)
        {
            if (ifcOrg.Addresses != null)
            {
                return ifcOrg.Addresses.TelecomAddresses.Select(address => address.ElectronicMailAddresses)
                    .Where(item => item != null).SelectMany(em => em)
                    .FirstOrDefault(em => string.Compare(emailAddress, em, true) == 0) != null;
            }
            return false;
        }
    }
}
