using System.Collections.ObjectModel;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public enum StandardColourMaps
    {
        /// <summary>
        /// Creates a colour map based on the IFC product types
        /// </summary>
        IfcProductTypeMap,
        /// <summary>
        /// Creates colour map sutiable to diversify models by role
        /// </summary>
        Federation,
        /// <summary>
        /// Creates an empty colour map
        /// </summary>
        Empty
    }

    /// <summary>
    /// Provides a map for obtaining a colour for a keyed type, the colour is an ARGB value
    /// </summary>
   
    public class XbimColourMap : KeyedCollection<string, XbimColour>
    {

        public override int GetHashCode()
        {
            var hash = 0;
            foreach (var colour in this)
            {
                hash ^= colour.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            var map = obj as XbimColourMap;
            if (map == null) return false;
            if(map.Count!=Count) return false;
            foreach (var colour in map)
            {
                if (!Contains(colour)) return false;
                if (!colour.Equals( this[colour.Name])) return false;
            }
            return true;
        }


        protected override string GetKeyForItem(XbimColour item)
        {
            return item.Name;
        }

      
        public bool IsTransparent
        {
            get
            {
                return this.Any(c => c.IsTransparent);
            }
        }

        public XbimColourMap(StandardColourMaps initMap= StandardColourMaps.IfcProductTypeMap)
        {
            switch (initMap)
            {
                case StandardColourMaps.IfcProductTypeMap:
                    SetProductTypeColourMap();
                    break;
                case StandardColourMaps.Federation:
                    SetFederationRoleColourMap();
                    break;
            }
        }

        public void SetFederationRoleColourMap()
        {
            Clear();
            Add(new XbimColour("Default", 0.98, 0.92, 0.74)); //grey

            // previously assigned colors
            Add(new XbimColour(IfcRoleEnum.ARCHITECT.ToString(), 1.0 , 1.0 , 1.0 , .5)); //white
            Add(new XbimColour(IfcRoleEnum.MECHANICALENGINEER.ToString(), 1.0, 0.5, 0.25));
            Add(new XbimColour(IfcRoleEnum.ELECTRICALENGINEER.ToString(), 0.0, 0, 1.0)); //blue
            Add(new XbimColour(IfcRoleEnum.STRUCTURALENGINEER.ToString(), 0.2, 0.2, 0.2)); //dark

            // new colours assigned from wheel
            double wheelAngle = 0;
            Add(XbimColour.FromHSV(IfcRoleEnum.BUILDINGOPERATOR.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.BUILDINGOWNER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.CIVILENGINEER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.CLIENT.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.COMMISSIONINGENGINEER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.CONSTRUCTIONMANAGER.ToString(), wheelAngle += 15, 1, 1)); 
            Add(XbimColour.FromHSV(IfcRoleEnum.CONSULTANT.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.CONTRACTOR.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.COSTENGINEER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.ENGINEER.ToString(), wheelAngle += 15, 1, 1)); 
            Add(XbimColour.FromHSV(IfcRoleEnum.FACILITIESMANAGER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.FIELDCONSTRUCTIONMANAGER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.MANUFACTURER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.OWNER.ToString(), wheelAngle += 15, 1, 1)); 
            Add(XbimColour.FromHSV(IfcRoleEnum.PROJECTMANAGER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.RESELLER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.SUBCONTRACTOR.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.SUPPLIER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(IfcRoleEnum.USERDEFINED.ToString(), wheelAngle + 15, 1, 1));
        }
       
        public new XbimColour this[string key]
        {
            get
            {
                if (Contains(key))
                    return base[key];
                if (Contains("Default"))
                    return base["Default"];
                return XbimColour.DefaultColour;
            }
        }

        /// <summary>
        /// Initialises the colour map on default Xbim Type Colours
        /// </summary>
        public void SetProductTypeColourMap()
        {
            Clear();
            Add(new XbimColour("Default", 0.98, 0.92, 0.74));
            Add(new XbimColour("IfcWall", 0.98, 0.92, 0.74));
            Add(new XbimColour("IfcWallStandardCase", 0.98, 0.92, 0.74));
            Add(new XbimColour("IfcRoof", 0.28, 0.24, 0.55));
            Add(new XbimColour("IfcBeam", 0.0, 0.0, 0.55));
            Add(new XbimColour("IfcBuildingElementProxy", 0.95, 0.94, 0.74));
            Add(new XbimColour("IfcColumn", 0.0, 0.0, 0.55));
            Add(new XbimColour("IfcSlab", 0.47, 0.53, 0.60));
            Add(new XbimColour("IfcWindow", 0.68, 0.85, 0.90, 0.5));
            Add(new XbimColour("IfcCurtainWall", 0.68, 0.85, 0.90, 0.4));
            Add(new XbimColour("IfcPlate", 0.68, 0.85, 0.90, 0.4));
            Add(new XbimColour("IfcDoor", 0.97, 0.19, 0));
            Add(new XbimColour("IfcSpace", 0.68, 0.85, 0.90, 0.4));
            Add(new XbimColour("IfcMember", 0.34, 0.34, 0.34));
            Add(new XbimColour("IfcDistributionElement", 0.0, 0.0, 0.55));
            Add(new XbimColour("IfcFurnishingElement", 1, 0, 0));
            Add(new XbimColour("IfcOpeningElement", 0.200000003, 0.200000003, 0.800000012, 0.2));
            Add(new XbimColour("IfcFeatureElementSubtraction", 1.0, 1.0, 1.0));
            Add(new XbimColour("IfcFlowTerminal", 0.95, 0.94, 0.74));
            Add(new XbimColour("IfcFlowSegment", 0.95, 0.94, 0.74));
            Add(new XbimColour("IfcDistributionFlowElement", 0.95, 0.94, 0.74));
            Add(new XbimColour("IfcFlowFitting", 0.95, 0.94, 0.74));
            Add(new XbimColour("IfcRailing", 0.95, 0.94, 0.74));
        }
    }
}
