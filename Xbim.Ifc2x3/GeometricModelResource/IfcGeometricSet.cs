#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricSet.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntityAttribute]
    public class GeometricSetElementsSet : XbimSet<IfcGeometricSetSelect>
    {
        internal GeometricSetElementsSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    /// <summary>
    ///   This entity is intended for the transfer of models when a topological structure is not available.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: This entity is intended for the transfer of models when a topological structure is not available.
    ///   Definition from IAI: The IfcGeometricSet is used for the exchange of shape representations consisting of (2D or 3D) points, curves, and/or surfaces, which do not have a topological structure (such as connected face sets or shells) and are not solid models (such as swept solids, CSG or Brep)
    ///   NOTE: Corresponding STEP entity: geometric_set. The derived attribute Dim has been added at this level and was therefore demoted from the geometric_representation_item. Please refer to ISO/IS 10303-42:1994, p. 190 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcGeometricSet : IfcGeometricRepresentationItem
    {
        public IfcGeometricSet()
        {
            _elements = new GeometricSetElementsSet(this);
        }

        #region Fields

        private GeometricSetElementsSet _elements;

        #endregion

        /// <summary>
        ///   The geometric elements which make up the geometric set, these may be points, curves or surfaces; but are required to be of the same coordinate space dimensionality.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public GeometricSetElementsSet Elements
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elements;
            }
            set { this.SetModelValue(this, ref _elements, value, v => Elements = v, "Elements"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _elements.Add((IfcGeometricSetSelect) value.EntityVal);
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        /// <summary>
        ///   The space dimensionality of this class, it is identical to the first element in the set. A where rule ensures that all elements have the same dimensionality.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get
            {
                if (_elements.Count > 0)
                    return DimElem(_elements.First);
                else //no valid match
                    return 0;
            }
        }

        private IfcDimensionCount DimElem(object elem)
        {
            IfcPoint pt = elem as IfcPoint;
            IfcCurve cv = elem as IfcCurve;
            IfcSurface sf = elem as IfcSurface;
            if (pt != null) return pt.Dim;
            if (cv != null) return cv.Dim;
            if (sf != null) return sf.Dim;
            return 0;
        }

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            IfcDimensionCount dim = Dim;
            string err = "";
            if (dim <= 0)
                err += "WR21 GeometricSet: There must be at least one Element in the set with a valid dimension count\n";
            foreach (IfcGeometricSetSelect item in _elements)
            {
                if (DimElem(item) != dim)
                {
                    err += "WR21 GeometricSet: All elements within a geometric set shall have the same dimensionality.";
                    return err;
                }
            }
            return err;
        }

        #endregion
    }
}