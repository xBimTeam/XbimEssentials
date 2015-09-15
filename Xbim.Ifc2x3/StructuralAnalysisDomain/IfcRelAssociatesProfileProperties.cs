#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatesProfileProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfilePropertyResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   The IfcRelAssociatesProfileProperties is an objectified relationship between non geometric profile properties (subtypes of IfcProfileProperties) and elements to which these properties apply, e.g. building elements and building element types as used within the structural 
    ///   engineering domain for steel, timber or concrete structures.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssociatesProfileProperties : IfcRelAssociates
    {
        #region Fields

        private IfcProfileProperties _relatingProfileProperties;
        private IfcShapeAspect _profileSectionLocation;
        private IfcOrientationSelect _profileOrientation;

        #endregion

        /// <summary>
        ///   Profile property definition assigned to the instances.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcProfileProperties RelatingProfileProperties

        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingProfileProperties;
            }
            set
            {
                this.SetModelValue(this, ref _relatingProfileProperties, value,
                                           v => RelatingProfileProperties = v, "RelatingProfileProperties");
            }
        }

        /// <summary>
        ///   Reference to a shape aspect with a single member of the ShapeRepresentations list. This member holds the location at which the profile properties apply.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcShapeAspect ProfileSectionLocation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _profileSectionLocation;
            }
            set
            {
                this.SetModelValue(this, ref _profileSectionLocation, value, v => ProfileSectionLocation = v,
                                           "ProfileSectionLocation");
            }
        }

        /// <summary>
        ///   The provision of an plane angle or a direction as the measure to orient the profile definition within the elements coordinate system. 
        ///   For IfcStructuralCurveMember the IfcPlaneAngleMeasure defines the β angle, for columns the derivation from the structural x axis 
        ///   and for beams the derivation from the structural z axis. The IfcDirection precisely defines the orientation of the profile's 
        ///   structural z axis within the structural coordinate system of the analysis model.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcOrientationSelect ProfileOrientation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _profileOrientation;
            }
            set
            {
                this.SetModelValue(this, ref _profileOrientation, value, v => ProfileOrientation = v,
                                           "ProfileOrientation");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _relatingProfileProperties = (IfcProfileProperties) value.EntityVal;
                    break;
                case 6:
                    _profileSectionLocation = (IfcShapeAspect) value.EntityVal;
                    break;
                case 7:
                    _profileOrientation = (IfcOrientationSelect) value.EntityVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}