using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    /// <summary>
    /// The IfcConnectionPortGeometry is used to describe the geometric constraints that facilitate the physical
    /// connection of two objects at a port having a profile geometry (here IfcProfile). It is envisioned as a control 
    /// that applies to the element connection relationships.
    ///
    /// This entity defines the geometric location and configuration of a port on a distribution element. 
    /// This information can be used to determine how to physically connect distribution elements. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcConnectionPortGeometry : IfcConnectionGeometry
    {
        private IfcAxis2Placement _LocationAtRelatingElement;

        /// <summary>
        /// Local placement of the port relative to its distribution element's local placement. The element in question is that, which plays the role of the 
        /// relating element in the connectivity relationship. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement LocationAtRelatingElement
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LocationAtRelatingElement;
            }
            set { this.SetModelValue(this, ref _LocationAtRelatingElement, value, v => LocationAtRelatingElement = v, "LocationAtRelatingElement"); }
        }

        private IfcAxis2Placement _LocationAtRelatedElement;

        /// <summary>
        ///  	Local placement of the port relative to its distribution element's local placement. The element in question is that, 
        ///  	which plays the role of the related element in the connectivity relationship. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcAxis2Placement LocationAtRelatedElement
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LocationAtRelatedElement;
            }
            set { this.SetModelValue(this, ref _LocationAtRelatedElement, value, v => LocationAtRelatedElement = v, "LocationAtRelatedElement"); }
        }

        private IfcProfileDef _ProfileOfPort;

        /// <summary>
        /// Profile that defines the port connection geometry. It is placed inside the XY plane of the location, 
        /// given at the relating and (optionally) related distribution element. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcProfileDef ProfileOfPort
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ProfileOfPort;
            }
            set { this.SetModelValue(this, ref _ProfileOfPort, value, v => ProfileOfPort = v, "ProfileOfPort"); }
        }

        public override string WhereRule()
        {
            return "";
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _LocationAtRelatingElement = (IfcAxis2Placement)value.EntityVal;
                    break;
                case 1:
                    _LocationAtRelatedElement = (IfcAxis2Placement)value.EntityVal;
                    break;
                case 2:
                    _ProfileOfPort = (IfcProfileDef)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
