#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcShellBasedSurfaceModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A shell based surface model is described by a set of open or closed shells of dimensionality 2.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A shell based surface model is described by a set of open or closed shells of dimensionality 2. The shells shall not intersect except at edges and vertices. In particular, distinct faces may not intersect. A complete face of one shell may be shared with another shell. Coincident portions of shells shall both reference the same faces, edges and vertices defining the coincident region. There shall be at least one shell.
    ///   A shell may exist independently of a surface model.
    ///   NOTE Corresponding STEP entity: shell_based_surface_model. Please refer to ISO/IS 10303-42:1994, p. 187 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 2x.
    ///   Informal propositions
    ///   The dimensionality of the shell based surface model is 2. 
    ///   The shells shall not overlap or intersect except at common faces, edges or vertices.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcShellBasedSurfaceModel : IfcGeometricRepresentationItem, IFaceBasedModelCollection
    {
        public IfcShellBasedSurfaceModel()
        {
            _sbsmBoundary = new XbimSet<IfcShell>(this);
        }

        #region Part 21 Step file Parse routines

        private XbimSet<IfcShell> _sbsmBoundary;

        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimSet<IfcShell> SbsmBoundary
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sbsmBoundary;
            }
            set { this.SetModelValue(this, ref _sbsmBoundary, value, v => SbsmBoundary = v, "SbsmBoundary"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _sbsmBoundary.Add((IfcShell) value.EntityVal);
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        /// <summary>
        ///   Derived.   The space dimensionality of this class, it is always 3.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return new IfcDimensionCount(3); }
        }

        public override string WhereRule()
        {
            return "";
        }

        #region IFaceBasedModelCollection Members

        IEnumerable<IFaceBasedModel> IFaceBasedModelCollection.FaceModels
        {
            get { return SbsmBoundary.Cast<IFaceBasedModel>(); }
        }

        #endregion
    }
}