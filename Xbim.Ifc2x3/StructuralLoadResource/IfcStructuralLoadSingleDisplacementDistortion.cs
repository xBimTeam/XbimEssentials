#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadSingleDisplacementDistortion.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    /// <summary>
    ///   Instances of the entity IfcStructuralLoadSingleForceWarping, as a subtype of IfcStructuralLoadSingleForce, 
    ///   shall be used to define an action opertion on a single point. In addition to forces and moments defined by its supertype 
    ///   a warping moment can be defined. 
    ///   All values are given within the chosen coordinate system of the 'activity element' (subtypes of IfcStructuralActivity), 
    ///   either the local coordinate system of the activity element or the global project coordinate system which is referenced 
    ///   by the activity element as its geometric representation context. The units of the displacement and rotation values are 
    ///   given within the global unit assignment (IfcUnitAssignment).
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcStructuralLoadSingleDisplacementDistortion : IfcStructuralLoadSingleDisplacement
    {
        #region Fields

        private IfcCurvatureMeasure? _distortion;

        #endregion

        #region Properties

        /// <summary>
        ///   The distortion curvature given to the displacement load.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcCurvatureMeasure? Distortion
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _distortion;
            }
            set { this.SetModelValue(this, ref _distortion, value, v => Distortion = v, "Distortion"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _distortion = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}