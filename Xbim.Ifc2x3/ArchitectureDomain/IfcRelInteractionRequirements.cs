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
    [IfcPersistedEntity]
    public class IfcRelInteractionRequirements : IfcRelConnects
    {
        private IfcCountMeasure? _DailyInteraction;

        /// <summary>
        /// Number of interactions occurring on a daily basis. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcCountMeasure? DailyInteraction
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _DailyInteraction;
            }
            set { this.SetModelValue(this, ref _DailyInteraction, value, v => DailyInteraction = v, "DailyInteraction"); }
        }

        private IfcNormalisedRatioMeasure? _ImportanceRating;

        /// <summary>
        /// Represents the level of importance of interaction. 0 represents lowest importance, 1 represents highest importance. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? ImportanceRating
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ImportanceRating;
            }
            set { this.SetModelValue(this, ref _ImportanceRating, value, v => ImportanceRating = v, "ImportanceRating"); }
        }

        private IfcSpatialStructureElement _LocationOfInteraction;

        /// <summary>
        /// The location where this interaction happens. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcSpatialStructureElement LocationOfInteraction
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LocationOfInteraction;
            }
            set { this.SetModelValue(this, ref _LocationOfInteraction, value, v => LocationOfInteraction = v, "LocationOfInteraction"); }
        }

        private IfcSpaceProgram _RelatedSpaceProgram;

        /// <summary>
        /// Related space program for the interaction requirement. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcSpaceProgram RelatedSpaceProgram
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RelatedSpaceProgram;
            }
            set { this.SetModelValue(this, ref _RelatedSpaceProgram, value, v => RelatedSpaceProgram = v, "RelatedSpaceProgram"); }
        }

        private IfcSpaceProgram _RelatingSpaceProgram;

        /// <summary>
        /// Relating space program for the interaction requirement. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcSpaceProgram RelatingSpaceProgram
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RelatingSpaceProgram;
            }
            set { this.SetModelValue(this, ref _RelatingSpaceProgram, value, v => RelatingSpaceProgram = v, "RelatingSpaceProgram"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _DailyInteraction = value.NumberVal;
                    break;
                case 5:
                    _ImportanceRating = value.RealVal;
                    break;
                case 6:
                    _LocationOfInteraction = (IfcSpatialStructureElement)value.EntityVal;
                    break;
                case 7:
                    _RelatedSpaceProgram = (IfcSpaceProgram)value.EntityVal;
                    break;
                case 8:
                    _RelatingSpaceProgram = (IfcSpaceProgram)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}
