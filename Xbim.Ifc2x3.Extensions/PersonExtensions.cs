using System.Linq;
using Xbim.Ifc2x3.ActorResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class PersonExtensions
    {
        public static string GetFullName(this IfcPerson ifcPerson)
        {
            string name = string.Empty;
            if (ifcPerson.PrefixTitles != null)
            {
                foreach (var item in ifcPerson.PrefixTitles)
                {
                    name += string.IsNullOrEmpty(item) ? "" : item.ToString() + " ";
                } 
            }

            if (ifcPerson.GivenName.HasValue)
                name += ifcPerson.GivenName + " ";

            if (ifcPerson.MiddleNames != null)
            {
                foreach (var item in ifcPerson.MiddleNames)
                {
                    name += string.IsNullOrEmpty(item) ? "" : item.ToString() + " ";
                } 
            }

            if (ifcPerson.FamilyName.HasValue)
                name += ifcPerson.FamilyName + " ";

            if (ifcPerson.SuffixTitles != null)
            {
                foreach (var item in ifcPerson.SuffixTitles)
                {
                    name += string.IsNullOrEmpty(item) ? "" : item.ToString() + " ";
                } 
            }
            return name;
        }

        public static bool HasEmail(this IfcPerson ifcPerson, string emailAddress)
        {
            if(ifcPerson.Addresses != null)
            {
                return ifcPerson.Addresses.OfType<IfcTelecomAddress>().Select(address => address.ElectronicMailAddresses)
                    .Where(item => item != null).SelectMany(em => em)
                    .FirstOrDefault(em => string.Compare(emailAddress,em,true)==0)!=null;
            }
            return false;
        }
    }
}
