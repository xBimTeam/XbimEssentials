using System.Collections.Generic;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.Ifc2x3.TimeSeriesResource;
using Xbim.Ifc2x3.CostResource;
using Xbim.Ifc2x3.PropertyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ObjectReferenceSelectExtensions
    {
        public static string GetValuesAsString(this IfcObjectReferenceSelect ifcObjectReferenceSelect)
        {
            if (ifcObjectReferenceSelect is IfcMaterial)
            {
                return (ifcObjectReferenceSelect as IfcMaterial).Name.ToString();
            }
            if (ifcObjectReferenceSelect is IfcPerson)
            {
                return (ifcObjectReferenceSelect as IfcPerson).GetFullName();
            }
            if (ifcObjectReferenceSelect is IfcDateAndTime)
            {
                return (ifcObjectReferenceSelect as IfcDateAndTime).GetAsString();
            }
            if (ifcObjectReferenceSelect is IfcMaterialList)
            {
                List<string> values = new List<string>();
                foreach (var item in (ifcObjectReferenceSelect as IfcMaterialList).Materials)
                {
                    values.Add(item.Name.ToString());
                }
                if (values.Count > 0)
                    return string.Join(", ", values);
                else
                    return string.Empty;
            }
            if (ifcObjectReferenceSelect is IfcOrganization)
            {
                return (ifcObjectReferenceSelect as IfcOrganization).Name.ToString();
            }
            if (ifcObjectReferenceSelect is IfcCalendarDate)
            {
                return (ifcObjectReferenceSelect as IfcCalendarDate).GetAsString();
                
            }
            if (ifcObjectReferenceSelect is IfcLocalTime)
            {
                return (ifcObjectReferenceSelect as IfcLocalTime).GetAsString();
            }
            if (ifcObjectReferenceSelect is IfcPersonAndOrganization)
            {
                IfcPersonAndOrganization ifcPersonAndOrganization = (ifcObjectReferenceSelect as IfcPersonAndOrganization);
                string value = ifcPersonAndOrganization.ThePerson.GetFullName();
                value = value.Trim();
                value += ", " + ifcPersonAndOrganization.TheOrganization.Name.ToString();
                return value;
            }
            if (ifcObjectReferenceSelect is IfcMaterialLayer)
            {
                IfcMaterialLayer ifcMaterialLayer = (ifcObjectReferenceSelect as IfcMaterialLayer);
                string value = string.Empty;
                if (ifcMaterialLayer.Material != null)
	            {
                    value += ifcMaterialLayer.Material.Name.ToString();
	            }
                if (string.IsNullOrEmpty(value))
                {
                    value = ifcMaterialLayer.LayerThickness.Value.ToString();
                }
                else
                {
                    value += "(" +ifcMaterialLayer.LayerThickness.Value.ToString() + ")";
                }
                return value;
            }
            if (ifcObjectReferenceSelect is IfcExternalReference)
            {
                IfcExternalReference ifcExternalReference = (ifcObjectReferenceSelect as IfcExternalReference);
                if (ifcExternalReference.Location.HasValue)
                {
                    return ifcExternalReference.Location.ToString();
                }
                return string.Empty;
            }
            if (ifcObjectReferenceSelect is IfcTimeSeries)
            {
                return (ifcObjectReferenceSelect as IfcTimeSeries).GetAsString();
            }
            if (ifcObjectReferenceSelect is IfcAddress)
            {
                return (ifcObjectReferenceSelect as IfcAddress).GetAsString();
            }
            if (ifcObjectReferenceSelect is IfcAppliedValue)
            {
                return (ifcObjectReferenceSelect as IfcAppliedValue).GetAsString(); 
            }

            return string.Empty;
        }
    }
}
