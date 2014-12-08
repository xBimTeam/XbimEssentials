#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricRepresentationItems.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcGeometricRepresentationItems
    {
        private readonly IModel _model;

        public IfcGeometricRepresentationItems(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcGeometricRepresentationItem> Items
        {
            get { return this._model.Instances.OfType<IfcGeometricRepresentationItem>(); }
        }

        public IfcPoints IfcPoints
        {
            get { return new IfcPoints(_model); }
        }

        public IfcSurfaces IfcSurfaces
        {
            get { return new IfcSurfaces(_model); }
        }

        public IfcDefinedSymbols IfcDefinedSymbols
        {
            get { return new IfcDefinedSymbols(_model); }
        }

        public IfcSolidModels IfcSolidModels
        {
            get { return new IfcSolidModels(_model); }
        }

        public IfcDirections IfcDirections
        {
            get { return new IfcDirections(_model); }
        }

        public IfcCurves IfcCurves
        {
            get { return new IfcCurves(_model); }
        }

        public IfcCartesianTransformationOperators IfcCartesianTransformationOperators
        {
            get { return new IfcCartesianTransformationOperators(_model); }
        }

        public IfcHalfSpaceSolids IfcHalfSpaceSolids
        {
            get { return new IfcHalfSpaceSolids(_model); }
        }

        public IfcGeometricSets IfcGeometricSets
        {
            get { return new IfcGeometricSets(_model); }
        }

        public IfcOneDirectionRepeatFactors IfcOneDirectionRepeatFactors
        {
            get { return new IfcOneDirectionRepeatFactors(_model); }
        }

        public IfcBoundingBoxs IfcBoundingBoxs
        {
            get { return new IfcBoundingBoxs(_model); }
        }

        public IfcPlacements IfcPlacements
        {
            get { return new IfcPlacements(_model); }
        }

        public IfcVectors IfcVectors
        {
            get { return new IfcVectors(_model); }
        }

        public IfcPlanarExtents IfcPlanarExtents
        {
            get { return new IfcPlanarExtents(_model); }
        }

        public IfcShellBasedSurfaceModels IfcShellBasedSurfaceModels
        {
            get { return new IfcShellBasedSurfaceModels(_model); }
        }

        public IfcTextLiterals IfcTextLiterals
        {
            get { return new IfcTextLiterals(_model); }
        }

        public IfcCompositeCurveSegments IfcCompositeCurveSegments
        {
            get { return new IfcCompositeCurveSegments(_model); }
        }

        public IfcDraughtingCallOuts IfcDraughtingCallOuts
        {
            get { return new IfcDraughtingCallOuts(_model); }
        }

        public IfcBooleanResults IfcBooleanResults
        {
            get { return new IfcBooleanResults(_model); }
        }

        public IfcFillAreaStyleHatchings IfcFillAreaStyleHatchings
        {
            get { return new IfcFillAreaStyleHatchings(_model); }
        }

        public IfcSectionedSpines IfcSectionedSpines
        {
            get { return new IfcSectionedSpines(_model); }
        }

        public IfcFaceBasedSurfaceModels IfcFaceBasedSurfaceModels
        {
            get { return new IfcFaceBasedSurfaceModels(_model); }
        }
    }
}