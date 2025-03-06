using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4x3.Kernel
{
    public partial class IfcObject
    {
        /// <summary>
        /// Returns all property sets related to this object
        /// </summary>
        public IEnumerable<IIfcPropertySet> PropertySets => IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions).OfType<IIfcPropertySet>();


        /// <summary>
        /// Returns all element quantities related to this object
        /// </summary>
        /// <returns>All related element quantities</returns>
        public IEnumerable<IIfcElementQuantity> ElementQuantities => IsDefinedBy.SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions).OfType<IIfcElementQuantity>();


        /// <summary>
        /// Returne all physical simple quantities (like length, area, volume, count, etc.)
        /// </summary>
        /// <returns>All physical simple quantities (like length, area, volume, count, etc.)</returns>
        public IEnumerable<IIfcPhysicalSimpleQuantity> PhysicalSimpleQuantities => ElementQuantities.SelectMany(eq => eq.Quantities).OfType<IIfcPhysicalSimpleQuantity>();
    }
    
}
