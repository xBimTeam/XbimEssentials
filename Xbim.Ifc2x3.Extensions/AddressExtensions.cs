using System.Text;
using Xbim.Ifc2x3.ActorResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class AddressExtensions
    {
        public static string GetAsString(this IfcAddress ifcAddress)
        {
            StringBuilder address = new StringBuilder();
            if (ifcAddress is IfcPostalAddress)
            {
                IfcPostalAddress ifcPostalAddress = (ifcAddress as IfcPostalAddress);
                if (ifcPostalAddress.PostalBox.HasValue)
                {
                    address.Append(ifcPostalAddress.PostalBox);
                    address.Append(", ");
                }
                if (ifcPostalAddress.InternalLocation.HasValue)
                {
                    address.Append(ifcPostalAddress.InternalLocation);
                    address.Append(", ");
                }
                if (ifcPostalAddress.AddressLines != null)
                {
                    foreach (var item in ifcPostalAddress.AddressLines)
                    {
                        address.Append(item);
                        address.Append(", ");
                    }
                }
                
                if (ifcPostalAddress.Town.HasValue)
                {
                    address.Append(ifcPostalAddress.Town);
                    address.Append(", ");
                }
                if (ifcPostalAddress.Region.HasValue)
                {
                    address.Append(ifcPostalAddress.Region);
                    address.Append(", ");
                }
                if (ifcPostalAddress.PostalCode.HasValue)
                {
                    address.Append(ifcPostalAddress.PostalCode);
                    address.Append(", ");
                }
                if (ifcPostalAddress.Country.HasValue)
                {
                    address.Append(ifcPostalAddress.Country);
                }
            }
            if (ifcAddress is IfcTelecomAddress)
            {
                IfcTelecomAddress ifcTelecomAddress = (ifcAddress as IfcTelecomAddress);
                if (ifcTelecomAddress.TelephoneNumbers != null)
                {
                    foreach (var item in ifcTelecomAddress.TelephoneNumbers)
                    {
                        address.Append(item);
                        address.Append(", ");
                    }
                }
                if (ifcTelecomAddress.FacsimileNumbers != null)
                {
                    foreach (var item in ifcTelecomAddress.FacsimileNumbers)
                    {
                        address.Append("FAX:");
                        address.Append(item);
                        address.Append(", ");
                    }
                }
                if (ifcTelecomAddress.ElectronicMailAddresses != null)
                {
                    foreach (var item in ifcTelecomAddress.ElectronicMailAddresses)
                    {
                        address.Append("EMAIL:");
                        address.Append(item);
                        address.Append(", ");
                    }
                }
                
                if (ifcTelecomAddress.WWWHomePageURL.HasValue)
                {
                    address.Append("WEB:");
                    address.Append(ifcTelecomAddress.WWWHomePageURL);
                }
            }
            return address.ToString();
        }
    }
}
