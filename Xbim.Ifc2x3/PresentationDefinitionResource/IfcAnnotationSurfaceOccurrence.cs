#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnnotationSurfaceOccurrence.cs
// Published:   05, 2012
// Last Edited: 13:00 PM on 23 05 2012
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions; 

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    [IfcPersistedEntityAttribute]
    public class IfcAnnotationSurfaceOccurrence : IfcAnnotationOccurrence, IfcDraughtingCalloutElement
    {
        #region Fields
        
        #endregion

        #region IfcProperties
        
        #endregion

        #region IfcParse
        
        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Item != null && !(Item is IfcSurface || Item is IfcFaceBasedSurfaceModel || Item is IfcShellBasedSurfaceModel || Item is IfcSolidModel))
                baseErr +=
                    "WR31 AnnotationSurfaceOccurrence : 	The Item that is styled by an IfcAnnotationSurfaceOccurrence relation shall be (if provided) a subtype of IfcSurface, IfcSolidModel, IfcShellBasedSurfaceModel, IfcFaceBasedSurfaceModel. ";
            return baseErr;
        }

        #endregion

    }
}
