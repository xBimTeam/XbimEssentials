#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSectionedSpine.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A sectioned spine is a representation of the shape of a three dimensional object composed of a spine curve and a number of planar cross sections. The shape is defined between the first element of cross sections and the last element of this set. 
    ///   NOTE A sectioned spine may be used to represent a surface or a solid but the interpolation of the shape between the cross-sections is not defined.
    ///   For the representation of a solid all cross-sections are closed curves.
    ///   Definition from IAI: A sectioned spine (IfcSectionedSpine) is a representation of the shape of a three dimensional object composed by a 
    ///   number of planar cross sections, and a spine curve.
    ///   The shape is defined between the first element of cross sections and the last element of the cross sections. 
    ///   A sectioned spine may be used to represent a surface or a solid but the interpolation of the shape between the cross sections is not defined. 
    ///   For the representation of a solid all cross sections are areas. For representation of a surface all cross sections are curves. 
    ///   The cross sections are defined as profiles, whereas the consecutive profiles may be derived by a transformation of the 
    ///   start profile or the previous consecutive profile.
    ///   The spine curve shall be of type IfcCompositeCurve, each of its segments (IfcCompositeCurveSegment) 
    ///   shall correspond to the part between exactly two consecutive cross-sections.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcSectionedSpine : IfcGeometricRepresentationItem
    {
        public IfcSectionedSpine()
        {
            _crossSections = new XbimList<IfcProfileDef>(this);
            _crossSectionPositions = new XbimList<IfcAxis2Placement3D>(this);
        }

        #region Fields

        private IfcCompositeCurve _spineCurve;
        private XbimList<IfcProfileDef> _crossSections;
        private XbimList<IfcAxis2Placement3D> _crossSectionPositions;

        #endregion

        #region Properties

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCompositeCurve SpineCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _spineCurve;
            }
            set { this.SetModelValue(this, ref _spineCurve, value, v => SpineCurve = v, "SpineCurve"); }
        }

        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 2)]
        public XbimList<IfcProfileDef> CrossSections
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _crossSections;
            }
            set { this.SetModelValue(this, ref _crossSections, value, v => CrossSections = v, "CrossSections"); }
        }

        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 2)]
        public XbimList<IfcAxis2Placement3D> CrossSectionPositions
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _crossSectionPositions;
            }
            set
            {
                this.SetModelValue(this, ref _crossSectionPositions, value, v => CrossSectionPositions = v,
                                           "CrossSectionPositions");
            }
        }

        #endregion

        #region Part 21 Step file Parse routines

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _spineCurve = (IfcCompositeCurve) value.EntityVal;
                    break;
                case 1:
                    _crossSections.Add((IfcProfileDef)value.EntityVal);
                    break;
                case 2:
                    _crossSectionPositions.Add((IfcAxis2Placement3D)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string err = "";
            if (_crossSectionPositions.Count != _crossSections.Count)
                err +=
                    "WR1 SectionedSpine : The set of cross sections and the set of cross section positions shall be of the same size.\n";
            IfcProfileDef firstProfile = _crossSections.FirstOrDefault();
            if (firstProfile != null)
            {
                IfcProfileTypeEnum pType = firstProfile.ProfileType;
                foreach (IfcProfileDef prof in _crossSections)
                {
                    if (prof.ProfileType != pType)
                    {
                        err +=
                            "WR2 SectionedSpine : The profile type (either AREA or CURVE) shall be consistent within the list of the profiles defining the cross sections.\n";
                        break;
                    }
                }
            }
            if (_spineCurve != null && _spineCurve.Dim != 3)
                err +=
                    "WR3 SectionedSpine : The curve entity which is the underlying spine curve shall have the dimensionality of 3.\n";
            return err;
        }

        #endregion
    }
}