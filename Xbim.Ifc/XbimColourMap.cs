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
            int hash = 0;
            foreach (var colour in this)
            {
                hash ^= colour.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            XbimColourMap map = obj as XbimColourMap;
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
            Add(new XbimColour("Default", 0.98, 0.92, 0.74, 1)); //grey

            // previously assigned colors
            Add(new XbimColour(Ifc4.ActorResource.IfcRoleEnum.ARCHITECT.ToString(), 1.0 , 1.0 , 1.0 , .5)); //white
            Add(new XbimColour(Ifc4.ActorResource.IfcRoleEnum.MECHANICALENGINEER.ToString(), 1.0, 0.5, 0.25, 1));
            Add(new XbimColour(Ifc4.ActorResource.IfcRoleEnum.ELECTRICALENGINEER.ToString(), 0.0, 0, 1.0, 1)); //blue
            Add(new XbimColour(Ifc4.ActorResource.IfcRoleEnum.STRUCTURALENGINEER.ToString(), 0.2, 0.2, 0.2, 1.0)); //dark

            // new colours assigned from wheel
            double wheelAngle = 0;
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.BUILDINGOPERATOR.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.BUILDINGOWNER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.CIVILENGINEER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.CLIENT.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.COMMISSIONINGENGINEER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.CONSTRUCTIONMANAGER.ToString(), wheelAngle += 15, 1, 1)); 
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.CONSULTANT.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.CONTRACTOR.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.COSTENGINEER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.ENGINEER.ToString(), wheelAngle += 15, 1, 1)); 
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.FACILITIESMANAGER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.FIELDCONSTRUCTIONMANAGER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.MANUFACTURER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.OWNER.ToString(), wheelAngle += 15, 1, 1)); 
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.PROJECTMANAGER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.RESELLER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.SUBCONTRACTOR.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.SUPPLIER.ToString(), wheelAngle += 15, 1, 1));
            Add(XbimColour.FromHSV(Ifc4.ActorResource.IfcRoleEnum.USERDEFINED.ToString(), wheelAngle + 15, 1, 1));
        }
       
        new public XbimColour this[string key]
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

        public void SetProductTypeColourMap()
        {
            Clear();
            Add(new XbimColour("Default", 0.98, 0.92, 0.74, 1));
            Add(new XbimColour(typeof(IIfcWall).Name, 0.98, 0.92, 0.74, 1));
            Add(new XbimColour(typeof(IIfcWallStandardCase).Name, 0.98, 0.92, 0.74, 1));
            Add(new XbimColour(typeof(IIfcRoof).Name, 0.28, 0.24, 0.55, 1));
            Add(new XbimColour(typeof(IIfcBeam).Name, 0.0, 0.0, 0.55, 1));
            Add(new XbimColour(typeof(IIfcBuildingElementProxy).Name, 0.95, 0.94, 0.74, 1));
            Add(new XbimColour(typeof(IIfcColumn).Name, 0.0, 0.0, 0.55, 1));
            Add(new XbimColour(typeof(IIfcSlab).Name, 0.47, 0.53, 0.60, 1));
            Add(new XbimColour(typeof(IIfcWindow).Name, 0.68, 0.85, 0.90, 0.5));
            Add(new XbimColour(typeof(IIfcCurtainWall).Name, 0.68, 0.85, 0.90, 0.4));
            Add(new XbimColour(typeof(IIfcPlate).Name, 0.68, 0.85, 0.90, 0.4));
            Add(new XbimColour(typeof(IIfcDoor).Name, 0.97, 0.19, 0, 1));
            Add(new XbimColour(typeof(IIfcSpace).Name, 0.68, 0.85, 0.90, 0.4));
            Add(new XbimColour(typeof(IIfcMember).Name, 0.34, 0.34, 0.34, 1));
            Add(new XbimColour(typeof(IIfcDistributionElement).Name, 0.0, 0.0, 0.55, 1));
            Add(new XbimColour(typeof(IIfcFurnishingElement).Name, 1, 0, 0, 1));
            Add(new XbimColour(typeof(IIfcOpeningElement).Name, 0.200000003, 0.200000003, 0.800000012, 0.2));
            Add(new XbimColour(typeof(IIfcFeatureElementSubtraction).Name, 1.0, 1.0, 1.0));
            Add(new XbimColour(typeof(IIfcFlowTerminal).Name, 0.95, 0.94, 0.74, 1));
            Add(new XbimColour(typeof(IIfcFlowSegment).Name, 0.95, 0.94, 0.74, 1));
            Add(new XbimColour(typeof(IIfcDistributionFlowElement).Name, 0.95, 0.94, 0.74, 1));
            Add(new XbimColour(typeof(IIfcFlowFitting).Name, 0.95, 0.94, 0.74, 1));
            Add(new XbimColour(typeof(IIfcRailing).Name, 0.95, 0.94, 0.74, 1));
        }
    }
}
