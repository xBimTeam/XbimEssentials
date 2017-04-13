using System.Collections.Generic;
using System.Linq;
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
            var material = ifcObjectReferenceSelect as IfcMaterial;
            if (material != null)
            {
                return material.Name.ToString();
            }
            var person = ifcObjectReferenceSelect as IfcPerson;
            if (person != null)
            {
                return person.GetFullName();
            }
            var time = ifcObjectReferenceSelect as IfcDateAndTime;
            if (time != null) return time.AsString();

            var list = ifcObjectReferenceSelect as IfcMaterialList;
            if (list != null)
            {
                List<string> values = list.Materials.Select(item => item.Name.ToString()).ToList();
                return values.Count > 0 ? string.Join(", ", values) : string.Empty;
            }
            var @select = ifcObjectReferenceSelect as IfcOrganization;
            if (@select != null)
            {
                return @select.Name.ToString();
            }
            var date = ifcObjectReferenceSelect as IfcCalendarDate;
            if (date != null)
            {
                return date.AsString();
                
            }
            var localTime = ifcObjectReferenceSelect as IfcLocalTime;
            if (localTime != null)
            {
                return localTime.AsString();
            }
            var organization = ifcObjectReferenceSelect as IfcPersonAndOrganization;
            if (organization != null)
            {
                IfcPersonAndOrganization ifcPersonAndOrganization = organization;
                string value = ifcPersonAndOrganization.ThePerson.GetFullName();
                value = value.Trim();
                value += ", " + ifcPersonAndOrganization.TheOrganization.Name.ToString();
                return value;
            }
            var layer = ifcObjectReferenceSelect as IfcMaterialLayer;
            if (layer != null)
            {
                IfcMaterialLayer ifcMaterialLayer = layer;
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
                    value += "(" +ifcMaterialLayer.LayerThickness.Value + ")";
                }
                return value;
            }
            var reference = ifcObjectReferenceSelect as IfcExternalReference;
            if (reference != null)
            {
                if (reference.Location.HasValue)
                {
                    return reference.Location.ToString();
                }
                return string.Empty;
            }
            var series = ifcObjectReferenceSelect as IfcTimeSeries;
            if (series != null)
            {
                return series.GetAsString();
            }
            var address = ifcObjectReferenceSelect as IfcAddress;
            if (address != null)
            {
                return address.GetAsString();
            }
            var appliedValue = ifcObjectReferenceSelect as IfcAppliedValue;
            if (appliedValue != null)
            {
                return appliedValue.AsString(); 
            }

            return string.Empty;
        }
    }
}
