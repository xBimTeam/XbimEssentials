#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    RepresentationExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

using Xbim.Ifc2x3.RepresentationResource;
namespace Xbim.Ifc2x3.Extensions
{
    public static class RepresentationExtension
    {
        /// <summary>
        /// returns true if the representation is a 3D Shape (solid or surface), if it is a curve or curve set returns false
        /// </summary>
        /// <param name="rep"></param>
        /// <returns></returns>
        public static bool IsBodyRepresentation(this IfcRepresentation rep)
        {
            //in old models sometimes the representation is not defined so assume it is a candidate
            if (!rep.RepresentationIdentifier.HasValue || string.IsNullOrEmpty(rep.RepresentationIdentifier.Value)) return true;
            string repIdentifier = rep.RepresentationIdentifier.Value;
            //if it is defined as body then it is candidate but exclude if it is using a line base representation
            if (string.Compare(repIdentifier, "body", true) == 0 || string.Compare(repIdentifier, "facetation", true) == 0)
            {
                //this should always be defined in an ifc2x3 schema but if it is not assume a solid
                if (!rep.RepresentationType.HasValue) return true;
                string repType = rep.RepresentationType.Value;
                repType = repType.ToLowerInvariant();
                //ignore line based body representations
                //if (repType == "curve2d" || repType == "geometricset" || repType == "geometriccurveset" || repType == "annotation2d") return false;
                //make sure we have a valid solid
                switch (repType)
                {
                    case "solidmodel":
                    case "surfacemodel":
                    case "sweptsolid":
                    case "brep":
                    case "csg":
                    case "clipping":
                    case "advancedsweptsolid":
                    case "boundingbox":
                    case "sectionedspine":
                    case "mappedrepresentation":
                        return true;
                    case "geometricset":
                    case "geometriccurveset":
                    case "annotation2d":
                    case  "curve2d" :
                    default:
                        return false;
                }
            }
            else
                return false;

        }

    }
}