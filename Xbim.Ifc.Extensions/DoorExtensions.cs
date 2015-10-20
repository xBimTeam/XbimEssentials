using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.SharedBldgElements;

namespace Xbim.Ifc2x3.Extensions
{
    public static class DoorExtensions
    {
        /// <summary>
        /// Returns the Reference ID for this specified type in this project (e.g. type 'A-1'), if known
        /// </summary>
        /// <param name="door"></param>
        /// <returns></returns>
        public static IfcIdentifier? GetReference(this IfcDoor door)
        {
            IfcValue val = door.GetPropertySingleNominalValue("Pset_DoorCommon", "Reference ");
            if (val != null && val is IfcIdentifier)
                return (IfcIdentifier)val;
            else
                return null;
        }


        /// <summary>
        /// Returns if the door is external, default is false if not specified
        /// </summary>
        /// <param name="door"></param>
        /// <returns></returns>
        public static IfcBoolean GetIsExternal(this IfcDoor door)
        {
            IfcValue val = door.GetPropertySingleNominalValue("Pset_DoorCommon", "IsExternal");
            if (val != null && val is IfcBoolean)
                return (IfcBoolean)val;
            else
                return new IfcBoolean(false); //default is to return false
        }
        /// <summary>
        /// Returns whether the door is a Fire Exit or not, null if not known
        /// </summary>
        /// <param name="door"></param>
        /// <returns></returns>
        public static IfcBoolean? GetFireExit(this IfcDoor door)
        {
            IfcValue val = door.GetPropertySingleNominalValue("Pset_DoorCommon", "FireExit ");
            if (val != null && val is IfcBoolean)
                return (IfcBoolean)val;
            else
                return null;
        }

        /// <summary>
        /// Returns the fire rating if defined
        /// </summary>
        /// <param name="door"></param>
        /// <returns></returns>
        public static IfcLabel? GetFireRating(this IfcDoor door)
        {
            IfcValue val = door.GetPropertySingleNominalValue("Pset_DoorCommon", "FireRating ");
            if (val != null && val is IfcLabel)
                return (IfcLabel)val;
            else
                return null;
        }
    }
}
