using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.ProductExtension
{
    public partial class IfcElement
    {

        public IEnumerable<IfcOpeningElement> Openings
        {
            get
            {               
                return Model.Instances.Where<IfcRelVoidsElement>(r => r.RelatingBuildingElement == this).Select(rv => rv.RelatedOpeningElement).OfType<IfcOpeningElement>();
            }
        }

        /// <summary>
        /// Adds an element type to the object if it doesn't already have one, return the new or existing relationship that holds the type and this element. If there is a relationship for this type but this element is not related it adds it to the exosting relationship
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public IfcRelDefinesByType AddDefiningType(IfcElementType theType)
        {
            var typedefs = Model.Instances.Where<IfcRelDefinesByType>(r => r.RelatingType == theType).ToList();
            var thisTypeDef = typedefs.FirstOrDefault(r => r.RelatedObjects.Contains((this)));
            if (thisTypeDef != null) return thisTypeDef; // it is already type related
            var anyTypeDef = typedefs.FirstOrDefault(); //take any one of the rels of the type
            if (anyTypeDef != null)
            {
                anyTypeDef.RelatedObjects.Add(this);
                return anyTypeDef;
            }
            var newdef = Model.Instances.New<IfcRelDefinesByType>(); //create one
            newdef.RelatedObjects.Add(this);
            newdef.RelatingType = theType;
            return newdef;
        }
    }
}
