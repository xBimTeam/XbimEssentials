using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.SharedBldgElements;

namespace Xbim.Ifc2x3.Extensions
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Returns if the door is external, default is false if not specified
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IfcBoolean GetIsExternal(this IfcWindow window)
        {
            IfcValue val = window.GetPropertySingleNominalValue("Pset_WindowCommon", "IsExternal");
            if (val != null && val is IfcBoolean)
                return (IfcBoolean)val;
            else
                return new IfcBoolean(false); //default is to return false
        }
        /// <summary>
        /// Returns the Reference ID for this specified type in this project (e.g. type 'A-1'), if known
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IfcIdentifier? GetReference(this IfcWindow window)
        {
            IfcValue val = window.GetPropertySingleNominalValue("Pset_WindowCommon", "Reference ");
            if (val != null && val is IfcIdentifier)
                return (IfcIdentifier)val;
            else
                return null;
        }

        /// <summary>
        /// Returns the fire rating if defined
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IfcLabel? GetFireRating(this IfcWindow window)
        {
            IfcValue val = window.GetPropertySingleNominalValue("Pset_WindowCommon", "FireRating ");
            if (val != null && val is IfcLabel)
                return (IfcLabel)val;
            else
                return null;
        }
    }
}
