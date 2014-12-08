#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelReferencedInSpatialStructure.cs
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
    ///   This objectified relationship, IfcRelReferencedInSpatialStructure, is used to assign elements in addition to those levels of the project spatial structure, in which they are referenced, but not primarily contained.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship, IfcRelReferencedInSpatialStructure, is used to assign elements in addition to those levels of the project spatial structure, in which they are referenced, but not primarily contained. 
    ///   NOTE  The primary containment relationship between an element and the spatial structure is handled by IfcRelContainsInSpatialStructure.
    ///   Any element can be referenced to zero, one or several levels of the spatial structure.
    ///   EXAMPLE  A wall might be normally contained within a storey, and since it does not span through several stories, it is not referenced in any additional storey. However a curtain wall might span through several stories, in this case it can be contained within the ground floor, but it would be referenced by all additional stories, it spans.
    ///   Predefined spatial structure elements to which elements can be assigned are
    ///   site as IfcSite 
    ///   building as IfcBuilding 
    ///   storey as IfcBuildingStorey 
    ///   space as IfcSpace 
    ///   The same element class can be assigned to different spatial structure elements depending on the context.
    ///   HISTORY New entity in Release IFC2x Edition 3.
    ///   Use case:
    ///   
    ///   The figure shows the use of IfcRelContainedInSpatialStructure and IfcRelReferencedInSpatialStructure to assign an IfcCurtainWall to two different levels within the spatial structure. It is primarily contained within the ground floor, and additionally referenced within the first floor.  
    ///   Formal Propositions:
    ///   WR31   :   The relationship object shall not be used to include other spatial structure elements into a spatial structure element. The hierarchy of the spatial structure is defined using IfcRelAggregates.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelReferencedInSpatialStructure : IfcRelConnects
    {
        public IfcRelReferencedInSpatialStructure()
        {
            _relatedElements = new XbimSet<IfcProduct>(this);
        }

        #region Fields

        private XbimSet<IfcProduct> _relatedElements;
        private IfcSpatialStructureElement _relatingStructure;

        #endregion

        /// <summary>
        ///   Set of products, which are referenced within this level of the spatial structure hierarchy.
        /// </summary>
        /// <remarks>
        ///   NOTE  Referenced elements are contained elsewhere within the spatial structure, they are referenced additionally by this spatial structure element, e.g., because they span several stories.
        /// </remarks>
        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
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
        ///   Spatial structure element, within which the element is referenced. Any element can be contained within zeor, one or many elements of the project spatial structure.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
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

        public override string WhereRule()
        {
            if (_relatedElements.OfType<IfcSpatialStructureElement>().Count() > 0)
                return
                    "WR31 RelReferencedInSpatialStructure :   The relationship object shall not be used to include other spatial structure elements into a spatial structure element.\n";
            else
                return "";
        }
    }
}