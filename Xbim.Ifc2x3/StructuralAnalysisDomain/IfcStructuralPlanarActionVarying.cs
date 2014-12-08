#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralPlanarActionVarying.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.StructuralLoadResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcStructuralPlanarActionVarying : IfcStructuralPlanarAction
    {
        #region Fields

        private IfcShapeAspect _varyingAppliedLoadLocation;
        private XbimList<IfcStructuralLoad> _subsequentAppliedLoads;

        #endregion

        #region Properties

        /// <summary>
        ///   A shape aspect, containing a list of shape representations, each defining either one Cartesian point or one 
        ///   point on curve (by parameter values) which are needed to provide the positions of the VaryingAppliedLoads. 
        ///   The values contained in the list of IfcShapeAspect.ShapeRepresentations correspond to the values at the same position in the list VaryingAppliedLoads.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Mandatory)]
        public IfcShapeAspect VaryingAppliedLoadLocation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _varyingAppliedLoadLocation;
            }
            set
            {
                this.SetModelValue(this, ref _varyingAppliedLoadLocation, value,
                                           v => VaryingAppliedLoadLocation = v, "VaryingAppliedLoadLocation");
            }
        }


        /// <summary>
        ///   A list containing load values which are assigned to the position defined through the shape aspect. 
        ///   The first load is already defined by the inherited attribute AppliedLoad and shall not be contained in this list.
        /// </summary>
        [IfcAttribute(14, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 2)]
        public XbimList<IfcStructuralLoad> SubsequentAppliedLoads
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _subsequentAppliedLoads;
            }
            set
            {
                this.SetModelValue(this, ref _subsequentAppliedLoads, value, v => SubsequentAppliedLoads = v,
                                           "SubsequentAppliedLoads");
            }
        }

        /// <summary>
        ///   Derived list of all varying applied loads by pushing the inherited AppliedLoad value to 
        ///   he beginning of the list of SubsequentAppliedLoads.
        /// </summary>
        public IEnumerable<IfcStructuralLoad> VaryingAppliedLoads
        {
            get
            {
                List<IfcStructuralLoad> val = new List<IfcStructuralLoad>(_subsequentAppliedLoads.Count + 1);
                val.Add(AppliedLoad);
                val.Concat(_subsequentAppliedLoads);
                return val;
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
                case 9:
                case 10:
                case 11:
                    base.IfcParse(propIndex, value);
                    break;
                case 12:
                    _varyingAppliedLoadLocation = (IfcShapeAspect) value.EntityVal;
                    break;
                case 13:
                    _subsequentAppliedLoads.Add((IfcStructuralLoad) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}