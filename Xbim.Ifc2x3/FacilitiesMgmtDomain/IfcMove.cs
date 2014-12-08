using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.ProcessExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
    /// <summary>
    /// An IfcMove is an activity that moves people, groups within an organization or complete organizations 
    /// together with their associated furniture and equipment from one place to another. The objects to be moved, 
    /// normally people, equipment, and furniture, are assigned by the IfcRelAssignsToProcess relationship.
    /// </summary>
    /// <remarks>
    /// Actors, equipment and furniture are moved from one IfcSpatialStructureElement to another. 
    /// The IfcSpatialStructureElement to be moved from and that to be moved to are assigned to the IfcMove 
    /// class using the IfcRelAssignsToProcess relationship.
    /// The actors (as IfcActor), equipment and furnitures to be moved are assigned to the IfcMove using the IfcRelAssignsToProcess relationship
    /// Each IfcMove must have a name. This requirement is enforced by a rule.
    /// he inherited attribute OperatesOn refers to the IfcRelAssignsToProcess relationship, keeping the reference to IfcActor, IfcEquipmentElement
    /// and IfcFurnishingElement. The QuantityInProcess attribute at the relationship object can be used to specify a quantity of the objects to be moved.
    /// Constraints may be applied to a move through instances of the IfcConstraint class (or its subtypes) that are associated 
    /// through the IfcRelAssociatesConstraint relationship class.
    /// Moves can be nested, i.e. a move object can contain other (more detailed) move objects. This is handled by the IfcRelNests relationship 
    /// pointing (with RelatingObject) to the containing move and (with RelatedObjects) to the contained (sub)moves.
    /// Moves are assigned to a move schedule (represented as IfcWorkSchedule with Purpose attribute 'Move') by using the IfcRelAssignsTask relationship.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcMove : IfcTask
    {

        #region Fields
        
        private IfcSpatialStructureElement _moveFrom;
        private IfcSpatialStructureElement _moveTo;
        private XbimListUnique<IfcText> _punchList;

        #endregion

        #region Properties

        /// <summary>
        /// The place from which actors and their associated equipment are moving.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcSpatialStructureElement MoveFrom
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _moveFrom;
            }
            set { this.SetModelValue(this, ref _moveFrom, value, v => MoveFrom = v, "MoveFrom"); }
        }

        /// <summary>
        /// The place to which actors and their associated equipment are moving.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public IfcSpatialStructureElement MoveTo
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _moveTo;
            }
            set { this.SetModelValue(this, ref _moveTo, value, v => MoveTo = v, "MoveTo"); }
        }

        
        /// <summary>
        /// A list of points concerning a move that require attention.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Optional)]
        public XbimListUnique<IfcText> PunchList
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _punchList;
            }
            set { this.SetModelValue(this, ref _punchList, value, v => PunchList = v, "PunchList"); }
        }


        #endregion

        #region Methods

        
        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 10:
                    _moveFrom = (IfcSpatialStructureElement)value.EntityVal;
                    break;
                case 11:
                    _moveTo = (IfcSpatialStructureElement)value.EntityVal;
                    break;
                case 12:
                    if (_punchList == null) //optional so set here
                    {
                        _punchList = new XbimListUnique<IfcText>(this);
                    }
                    _punchList.Add((IfcText)value.StringVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public override string WhereRule()
        {
            string baseErr = base.WhereRule();

            if (this.OperatesOn.Count() < 1)
            {
                baseErr += "WR1 IfcMove : There has to be a reference to the IfcRelAssignsToProcess relationship referencing the objects to be moved.\n";
            }
            //TODO: Check validity of WR2 IfcMove 
            
            bool onErr = false;
            foreach (IfcRelAssignsToProcess item in this.OperatesOn)
            {
                if (!((item.RelatedObjects.OfType<IfcActor>().Count() > 0) ||
                     (item.RelatedObjects.OfType<IfcEquipmentElement>().Count() > 0) ||
                     (item.RelatedObjects.OfType<IfcFurnishingElement>().Count() > 0)
                     )
                    )
                {
                    onErr = true;
                    break;
                }
            }

            if (onErr)
            {
                baseErr += "WR2 IfcMove : At least on furnishing or equipment object should be assigned to the move.\n";
            }
            

            if (!this.Name.HasValue)
            {
                baseErr += "WR3 IfcMove : The Name attribute has to be provided for the move.\n";
            }

            return baseErr;
        }
        #endregion
      
    }
}
