#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDerivedProfileDef.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProfileResource
{
    [IfcPersistedEntityAttribute]
    public class IfcDerivedProfileDef : IfcProfileDef
    {
        #region Fields

        private IfcProfileDef _parentProfile;
        private IfcCartesianTransformationOperator2D _operator;
        private IfcLabel? _label;

        #endregion

        #region Properties

        /// <summary>
        ///   The parent profile provides the origin of the transformation.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcProfileDef ParentProfile
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _parentProfile;
            }
            set { this.SetModelValue(this, ref _parentProfile, value, v => ParentProfile = v, "ParentProfile"); }
        }

        /// <summary>
        ///   Transformation operator applied to the parent profile.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcCartesianTransformationOperator2D Operator
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _operator;
            }
            set { this.SetModelValue(this, ref _operator, value, v => Operator = v, "Operator"); }
        }

        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLabel? Label
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _label;
            }
            set { this.SetModelValue(this, ref _label, value, v => Label = v, "Label"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _parentProfile = (IfcProfileDef) value.EntityVal;
                    break;
                case 3:
                    _operator = (IfcCartesianTransformationOperator2D) value.EntityVal;
                    break;
                case 4:
                    _label = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            if (_parentProfile.ProfileType != ProfileType)
                return
                    "WR1 DerivedProfileDef : The profile type of the derived profile shall be the same as the type of the parent profile, i.e. both shall be either AREA or CURVE. ";
            else return "";
        }
    }
}