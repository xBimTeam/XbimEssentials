#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralSurfaceMemberVarying.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcStructuralSurfaceMemberVarying : IfcStructuralSurfaceMember
    {
        public IfcStructuralSurfaceMemberVarying()
        {
            _subsequentThickness = new XbimList<IfcPositiveLengthMeasure>(this);
        }

        #region Fields

        private XbimList<IfcPositiveLengthMeasure> _subsequentThickness;
        private IfcShapeAspect _varyingThicknessLocation;

        #endregion

        #region Properties

        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public XbimList<IfcPositiveLengthMeasure> SubsequentThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _subsequentThickness;
            }
            set
            {
                this.SetModelValue(this, ref _subsequentThickness, value, v => SubsequentThickness = v,
                                           "SubsequentThickness");
            }
        }

        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcShapeAspect VaryingThicknessLocation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _varyingThicknessLocation;
            }
            set
            {
                this.SetModelValue(this, ref _varyingThicknessLocation, value, v => VaryingThicknessLocation = v,
                                           "VaryingThicknessLocation");
            }
        }

        /// <summary>
        ///   Derived list of all varying thickness values by pushing the inherited starting thickness to the beginning of the list of SubsequentThickness.
        /// </summary>
        public IEnumerable<IfcPositiveLengthMeasure> VaryingThickness
        {
            get
            {
                List<IfcPositiveLengthMeasure> newList = new List<IfcPositiveLengthMeasure>(_subsequentThickness);
                newList.Insert(0, Thickness.Value);
                return newList;
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _subsequentThickness.Add(value.RealVal);
                    break;
                case 10:
                    _varyingThicknessLocation = (IfcShapeAspect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!Thickness.HasValue)
                baseErr +=
                    "WR61 StructuralSurfaceMemberVarying : The Thickness attribute shall have a value assigned.\n";

            foreach (IfcShapeModel item in _varyingThicknessLocation.ShapeRepresentations)
            {
                if (item.Items.Count != 1)
                {
                    baseErr +=
                        "WR62 StructuralSurfaceMemberVarying : All point locations shall be given by only one shape representation item within the list of shape representations of the shape aspect.\n";
                    break;
                }
                else if (item.Items.Where(i => i is IfcCartesianPoint || i is IfcPointOnSurface).Count() != 1)
                {
                    baseErr +=
                        "WR63 StructuralSurfaceMemberVarying : Each shape representation, representing the point at which a varying thickness applies, shall be represented by either a Cartesian point or by a point on surface.\n";
                    break;
                }
            }
            return baseErr;
        }
    }
}