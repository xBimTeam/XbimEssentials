#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelContainedInSpatialStructure.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   This objectified relationship, IfcRelContainedInSpatialStructure, is used to assign elements to a certain level of the spatial project structure.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship, IfcRelContainedInSpatialStructure, is used to assign elements to a certain level of the spatial project structure. Any element can only be assigned once to a certain level of the spatial structure. The question, which level is relevant for which type of element, can only be answered within the context of a particular project and might vary within the various regions.
    ///   EXAMPLE A multi-storey space is contained (or belongs to) the building storey at which its ground level is, but it is referenced by all the other building storeys, in which it spans. A lift shaft might be contained by the basement, but referenced by all storeys, through which it spans. The reference relationship can be expressed by the general grouping mechanism estabished in IFC.
    ///   Predefined spatial structure elements to which elements can be assigned are
    ///   site as IfcSite 
    ///   building as IfcBuilding 
    ///   storey as IfcBuildingStorey 
    ///   space as IfcSpace 
    ///   The same element can be assigned to different spatial structure elements depending on the context.
    ///   EXAMPLE A wall might be normally assigned to a storey, however the curtain wall might be assigned to the building and the retaining wall in the terrain might be assigned to the site.
    ///   HISTORY New entity in IFC Release 2x. 
    ///   IFC2x PLATFORM CHANGE: The data type of the attribute RelatedElements has been changed from IfcElement to its supertype IfcProduct with upward compatibility for file based exchange.
    ///   Formal Propositions:
    ///   WR31   :   The relationship object shall not be used to include other spatial structure elements into a spatial structure element. The hierarchy of the spatial structure is defined using IfcRelAggregates.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelContainedInSpatialStructure : IfcRelConnects
    {
        public IfcRelContainedInSpatialStructure()
        {
            _relatedElements = new XbimSet<IfcProduct>(this);
        }

        #region Part 21 Step file Parse routines

        private XbimSet<IfcProduct> _relatedElements;
        private IfcSpatialStructureElement _relatingStructure;

        /// <summary>
        ///   Set of products, which are contained within this level of the spatial structure hierarchy.
        /// </summary>
        /// <remarks>
        ///   IFC2x PLATFORM CHANGE  The data type has been changed from IfcElement to IfcProduct with upward compatibility
        /// </remarks>
        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        [IndexedProperty]
        public XbimSet<IfcProduct> RelatedElements
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedElements;
            }
            set
            {
                this.SetModelValue(this, ref _relatedElements, value, v => RelatedElements = v,
                                           "RelatedElements");
            }
        }

        /// <summary>
        ///   Spatial structure element, within which the el
        ///   Element is contained. Any element can only be contained within one element of the project spatial structure.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcSpatialStructureElement RelatingStructure
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingStructure;
            }
            set
            {
                this.SetModelValue(this, ref _relatingStructure, value, v => RelatingStructure = v,
                                           "RelatingStructure");
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
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatedElements.Add((IfcProduct) value.EntityVal);
                    break;
                case 5:
                    _relatingStructure = (IfcSpatialStructureElement) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (_relatedElements.OfType<IfcSpatialStructureElement>().Count() > 0)
                return
                    "WR31 RelContainedInSpatialStructure : The relationship object shall not be used to include other spatial structure elements into a spatial structure element.  ";
            else
                return "";
        }
    }
}