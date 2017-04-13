using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace Xbim.Ifc2x3.IO
{
    public class XbimModelSummary
    {
        readonly XbimModel _model;

        public XbimModelSummary(XbimModel model)
        {
            _model = model;
        }

        public string SchemaVersion
        {
            get { return _model.Header.SchemaVersion; }
        }

        public string FileTitle
        {
            get { return _model.Header.FileName.Name; }
        }

        public string TimeStamp
        {
            get { return _model.Header.FileName.TimeStamp; }
        }

        public string OriginatingSystem
        {
            get { return _model.Header.FileName.OriginatingSystem; }
        }

        // ReSharper disable once InconsistentNaming
        public string IFCProcessor
        {
            get { return _model.Header.FileName.PreprocessorVersion; }
        }

        public string ViewDefinition
        {
            get { return string.Join(",",_model.Header.FileDescription.Description); }
        }

        public string ProjectName
        {
            get
            {
                return  _model.IfcProject.FriendlyName; 
            }
        }

        public string SiteAddress
        {
            get
            {
                var site =  _model.Instances.OfType<IfcSite>().FirstOrDefault();
                if (site != null && site.SiteAddress != null)
                    return string.Join(",",site.SiteAddress.SummaryString());
                return "";
            }
        }
        public long StoreyCount
        {
            get
            {
                return _model.Instances.CountOf<IfcBuildingStorey>();
            }
        }

        public double GrossFloorArea
        {
            get
            {
                return _model.Instances.OfType<IfcBuilding>()
                    .Select(GetGrossFloorArea)
                    .Where(a => a.HasValue)
                    .Aggregate<IfcAreaMeasure?, double>(0, (current, a) => current + a.Value);
            }
        }

        /// <summary>
        /// Gets the Gross Floor Area, if the element base quantity GrossFloorArea is defined this has precedence
        /// If no property is defined the GFA is returned as the sume of the building storeys GFA
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        private static IfcAreaMeasure? GetGrossFloorArea(IfcBuilding building)
        {
            var qArea = GetQuantity<IfcQuantityArea>(building, "BaseQuantities", "GrossFloorArea") ??
                        GetQuantity<IfcQuantityArea>(building, "GrossFloorArea");
            if (qArea != null) return qArea.AreaValue;
            IfcAreaMeasure? area = GetBuildingStoreys(building)
                .Select(GetGrossFloorArea)
                .Where(bsArea => bsArea.HasValue)
                .Aggregate<IfcAreaMeasure?, IfcAreaMeasure?>(0, (current, bsArea) => (IfcAreaMeasure?) (current + bsArea));
            return area != 0 ? area : null;
        }

        public static IfcAreaMeasure? GetGrossFloorArea(IfcBuildingStorey buildingStorey)
        {
            var qArea = GetQuantity<IfcQuantityArea>(buildingStorey, "BaseQuantities", "GrossFloorArea") ??
                                    GetQuantity<IfcQuantityArea>(buildingStorey, "GrossFloorArea");
            if (qArea != null) return qArea.AreaValue;
            return null;
        }

        public static TQType GetQuantity<TQType>(IfcObject elem, string pSetName, string qName) where TQType : IfcPhysicalQuantity
        {
            var rel = elem.IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition.Name == pSetName && r.RelatingPropertyDefinition is IfcElementQuantity);
            if (rel == null) return default(TQType);
            var eQ = rel.RelatingPropertyDefinition as IfcElementQuantity;
            if (eQ == null) return default(TQType);
            var result = eQ.Quantities.FirstOrDefault<TQType>(q => q.Name == qName);
            return result;
        }

        public static TQType GetQuantity<TQType>(IfcObject elem, string qName) where TQType : IfcPhysicalQuantity
        {
            var rel = elem.IsDefinedByProperties.FirstOrDefault(r => r.RelatingPropertyDefinition is IfcElementQuantity);
            if (rel == null) return default(TQType);
            var eQ = rel.RelatingPropertyDefinition as IfcElementQuantity;
            if (eQ == null) return default(TQType);
            var result = eQ.Quantities.FirstOrDefault<TQType>(q => q.Name == qName);
            return result;
        }

        public static IEnumerable<IfcBuildingStorey> GetBuildingStoreys(IfcBuilding building)
        {
            return building.IsDecomposedBy.SelectMany(r => r.RelatedObjects).OfType<IfcBuildingStorey>();
        }

        public long BuildingElementCount
        {
            get
            {
                return _model.Instances.CountOf<IfcBuildingElement>();
            }
        }

        public long FurnishingElementCount
        {
            get
            {
                return _model.Instances.CountOf<IfcFurnishingElement>();
            }
        }

        public long ElectricalElementCount
        {
            get
            {
                return _model.Instances.CountOf<IfcElectricalElement>();
            }
        }

        public long DistributionElementCount
        {
            get
            {
                return _model.Instances.CountOf<IfcDistributionElement>();
            }
        }

        public long ParticipantCount
        {
            get
            {
                return _model.Instances.CountOf<IfcActor>();
            }
        }

        public long DoorCount
        {
            get
            {
                return _model.Instances.CountOf<IfcDoor>();
            }
        }

        public long WindowCount
        {
            get
            {
                return _model.Instances.CountOf<IfcWindow>();
            }
        }

        public long SpaceCount
        {
            get
            {
                return _model.Instances.CountOf<IfcSpace>();
            }
        }
    }
}
