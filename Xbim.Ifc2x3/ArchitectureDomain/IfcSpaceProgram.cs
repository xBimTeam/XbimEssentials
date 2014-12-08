using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ArchitectureDomain
{
    /// <summary>
    /// Architectural program for a space in the building or facility being designed; essentially the requirements definition for such a building space.
    /// 
    ///     NOTE: that this 'program' defines the client requirements for the space before the building is designed. Space programs can change over the life cycle of a building, after the building is occupied. Changes to space programs take place in the facilities management/operations phase of the building life cycle.
    /// 
    /// The assignment of a person or an organization to a space program, e.g., as the anticipated occupants of the space, is handled through using the objectified relationship IfcRelAssignsToActor referring to IfcActor. Space programs can be nested, i.e. an IfcSpaceProgram can specify a program group up to any desired level. This is handled through using the objectified relationship IfcRelNests.
    /// 
    /// Property Set Use Definition:
    /// 
    /// The property sets relating to the IfcSpaceProgram are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcSpaceProgram are part of this IFC release:
    /// 
    ///     Pset_SpaceProgramCommon: common property set for all types of the space program
    /// 
    /// General Use Definition
    /// 
    /// The IfcSpaceProgram entity is used to define:
    /// 
    ///     the architectural program for a space in the building or facility being designed;
    ///     the standard for space allocation that can be assigned to persons within an organization.
    /// 
    /// As the architectural program, the IfcSpaceProgram class sets down the requirements definition for a space in the building or facility being designed. Used in this way, it defines the client requirements for the space before the building in designed. Space programs can change over the life cycle of a building, after the building is occupied. Changes to space programs take place in the facilities management/operations phase of the building life cycle.
    /// 
    /// As a space standard for facilities management (FM), the IfcSpaceProgram class defines the requirements for usage of a space according to the roles of persons that will occupy the space. This could take into account role driven elements such as whether the space should be a single person office, corner space, glazing on two sides etc. In order to use the class as an space standard within FM, a classification of spaces must have been established. This does not mean that each individual space needs to have a classification although for locating persons having an assigned space standard, this would be desirable.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcSpaceProgram : IfcControl
    {
        private IfcIdentifier _SpaceProgramIdentifier;

        /// <summary>
        /// Identifier for this space program. It often refers to a number (or code) assigned to the space program. Example: R-001. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier SpaceProgramIdentifier
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SpaceProgramIdentifier;
            }
            set { this.SetModelValue(this, ref _SpaceProgramIdentifier, value, v => SpaceProgramIdentifier = v, "SpaceProgramIdentifier"); }
        }

        private IfcAreaMeasure? _MaxRequiredArea;

        /// <summary>
        /// The maximum floor area programmed for this space (according to client requirements) 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcAreaMeasure? MaxRequiredArea
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _MaxRequiredArea;
            }
            set { this.SetModelValue(this, ref _MaxRequiredArea, value, v => MaxRequiredArea = v, "MaxRequiredArea"); }
        }

        private IfcAreaMeasure? _MinRequiredArea;

        /// <summary>
        /// The minimum floor area programmed for this space (according to client requirements) 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcAreaMeasure? MinRequiredArea
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _MinRequiredArea;
            }
            set { this.SetModelValue(this, ref _MinRequiredArea, value, v => MinRequiredArea = v, "MinRequiredArea"); }
        }

        private IfcSpatialStructureElement _RequestedLocation;

        /// <summary>
        /// Location within the building structure, requested for the space. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcSpatialStructureElement RequestedLocation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RequestedLocation;
            }
            set { this.SetModelValue(this, ref _RequestedLocation, value, v => RequestedLocation = v, "RequestedLocation"); }
        }

        private IfcAreaMeasure _StandardRequiredArea;

        /// <summary>
        /// The floor area programmed for this space (according to client requirements). 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcAreaMeasure StandardRequiredArea
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _StandardRequiredArea;
            }
            set { this.SetModelValue(this, ref _StandardRequiredArea, value, v => StandardRequiredArea = v, "StandardRequiredArea"); }
        }

        /// <summary>
        /// Set of inverse relationships to space or work interaction requirement objects (FOR RelatedObject). 
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public virtual IEnumerable<IfcRelInteractionRequirements> HasInteractionReqsFrom
        {
            get { return ModelOf.Instances.Where<IfcRelInteractionRequirements>(r => r.RelatedSpaceProgram == this); }
        }

        /// <summary>
        /// Set of inverse relationships to space or work interaction requirements (FOR RelatingObject). 
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public virtual IEnumerable<IfcRelInteractionRequirements> HasInteractionReqsTo
        {
            get { return ModelOf.Instances.Where<IfcRelInteractionRequirements>(r => r.RelatingSpaceProgram == this); }
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
                    _SpaceProgramIdentifier = value.StringVal;
                    break;
                case 6:
                    _MaxRequiredArea = value.RealVal;
                    break;
                case 7:
                    _MinRequiredArea = value.RealVal;
                    break;
                case 8:
                    _RequestedLocation = (IfcSpatialStructureElement)value.EntityVal;
                    break;
                case 9:
                    _StandardRequiredArea = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
