#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpatialStructureElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   A spatial structure element (IfcSpatialStructureElement) is the generalization of all spatial elements that might be used to define a spatial structure.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A spatial structure element (IfcSpatialStructureElement) is the generalization of all spatial elements that might be used to define a spatial structure. That spatial structure is often used to provide a project structure to organize a building project.
    ///   A spatial project structure might define as many levels of decomposition as necessary for the building project. Elements within the spatial project structure are:
    ///   site as IfcSite 
    ///   building as IfcBuilding 
    ///   storey as IfcBuildingStorey 
    ///   space as IfcSpace 
    ///   or aggregations or parts thereof. The composition type declares an element to be either an element itself, or an aggregation (complex) or a decomposition (part). The interpretation of these types is given at each subtype of IfcSpatialStructureElement.
    ///   The IfcRelAggregates is defined as an 1-to-many relationship and used to establish the relationship between exactly two levels within the spatial project structure. Finally the highest level of the spatial structure is assigned to IfcProject using the IfcRelAggregates.
    ///   Informal proposition:
    ///   The spatial project structure, established by the IfcRelAggregates, shall be acyclic. 
    ///   A site should not be (directly or indirectly) associated to a building, storey or space. 
    ///   A building should not be (directly or indirectly) associated to a storey or space. 
    ///   A storey should not be (directly or indirectly) associated to a space. 
    ///   HISTORY New entity in IFC Release 2x.
    ///   Spatial Structure Use Definition
    ///  
    ///   The figure shows the use of IfcRelAggregates to establish a spatial structure including site, building, building section and storey. More information is provided at the level of the subtypes. 
    ///   EXPRESS specification
    ///   Formal Propositions:
    ///   WR41   :   All spatial structure elements shall be associated (using the IfcRelAggregates relationship) with another spatial structure element, or with IfcProject.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcSpatialStructureElement : IfcProduct
    {
        #region Fields and Events

        private IfcLabel? _longName;
        private IfcElementCompositionEnum? _compositionType;

        #endregion

        #region Constructors and Initializers

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Long name for a spatial structure element, used for informal purposes. Maybe used in conjunction with the inherited Name attribute.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel? LongName
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _longName;
            }
            set { this.SetModelValue(this, ref _longName, value, v => _longName = v, "LongName"); }
        }

        /// <summary>
        ///   Denotes, whether the predefined spatial structure element represents itself, or an aggregate (complex) or a part (part). The interpretation is given separately for each subtype of spatial structure element.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcElementCompositionEnum? CompositionType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _compositionType;
                ////otherwise calculate it
                //IEnumerable<ObjectDefinition> children = this.SpatialStructuralElementChildren;
                //ObjectDefinition parentObj = this.SpatialStructuralElementParent;

                //bool careAboutParent = (parentObj != null) ? (parentObj.GetType() == this.GetType()) : false;//only interested in parent composition if it is of the same type
                //if (children == null)
                //{
                //    if (!careAboutParent)
                //        return ElementCompositionEnum.ELEMENT;
                //    else
                //    {
                //        switch (((SpatialStructureElement)parentObj).CompositionType)
                //        {
                //            case ElementCompositionEnum.COMPLEX:
                //                return ElementCompositionEnum.ELEMENT;
                //            case ElementCompositionEnum.ELEMENT:
                //                return ElementCompositionEnum.PARTIAL;
                //            default:
                //                throw new ArgumentException("The spatial structure violates Ifc decomposition rules. Parent is CompositionType = PARTIAL");
                //        }
                //    }

                //}
                //else
                //{
                //    foreach (SpatialStructureElement child in this.SpatialStructuralElementChildren)//check each child to see if it is complex etc.
                //    {
                //        if (child.GetType() == this.GetType()) //if any childs is of this type then this is COMPLEX or ELEMENT
                //        {
                //            if (careAboutParent)
                //            {
                //                switch (((SpatialStructureElement)parentObj).CompositionType)
                //                {
                //                    case ElementCompositionEnum.COMPLEX:
                //                        return ElementCompositionEnum.ELEMENT;
                //                    case ElementCompositionEnum.ELEMENT:
                //                        return ElementCompositionEnum.PARTIAL;
                //                    default:
                //                        throw new ArgumentException("The spatial structure violates Ifc decomposition rules. Parent is CompositionType = PARTIAL or NULL");
                //                }
                //            }
                //            else //has a child of same type but parent is different type so return complex
                //            {
                //                return ElementCompositionEnum.COMPLEX;
                //            }
                //        }
                //    }
                //    if (careAboutParent) //has no children of same type so check its parent type cannot be a project
                //    {
                //        switch (((SpatialStructureElement)parentObj).CompositionType)
                //        {
                //            case ElementCompositionEnum.COMPLEX:
                //                return ElementCompositionEnum.ELEMENT;
                //            case ElementCompositionEnum.ELEMENT:
                //                return ElementCompositionEnum.PARTIAL;
                //            default:
                //                throw new ArgumentException("The spatial structure violates Ifc decomposition rules. Parent is CompositionType = PARTIAL or NULL");
                //        }

                //    }
                //    else // neither parent nor children are of the same type as this
                //    {
                //        return ElementCompositionEnum.ELEMENT;
                //    }

                //}
            }
            set
            {
                this.SetModelValue(this, ref _compositionType, value, v => _compositionType = v,
                                           "CompositionType");
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
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _longName = value.StringVal;
                    break;
                case 8:
                    _compositionType =
                        (IfcElementCompositionEnum?)Enum.Parse(typeof(IfcElementCompositionEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string ToString()
        {
            string n;
            if (string.IsNullOrEmpty(Name))
                n = GetType().Name.Substring(3);
            else
                n = Name;
            if (!string.IsNullOrEmpty(LongName) && Name != LongName) n += " - " + LongName.ToString();
            return n;
        }

        #region Inverse Relationships

        /// <summary>
        ///   Set of spatial reference relationships, that holds those elements, which are referenced, but not contained, within this element of the project spatial structure.
        /// </summary>
        /// <remarks>
        ///   NOTE  The spatial reference relationship, established by IfcRelReferencedInSpatialStructure, is not required to be an hierarchical relationship, i.e. each element can be assigned to 0, 1 or many spatial structure elements.
        ///   EXAMPLE  A curtain wall maybe contained in the ground floor, but maybe referenced in all floors, it reaches.
        ///   IFC2x Edition 3 CHANGE  The inverse attribute has been added with upward compatibility for file based exchange.
        /// </remarks>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelReferencedInSpatialStructure> ReferencesElements
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelReferencedInSpatialStructure>(
                        r => r.RelatingStructure == this);
            }
        }

        /// <summary>
        ///   Set of relationships to Systems, that provides a certain service to the Building. The relationship is handled by the objectified relationship IfcRelServicesBuildings.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelServicesBuildings> ServicedBySystems
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelServicesBuildings>(
                        r => r.RelatedBuildings.Contains(this));
            }
        }

        /// <summary>
        ///   Set of spatial containment relationships, that holds those elements, which are contained within this element of the project spatial structure.
        /// </summary>
        /// <remarks>
        ///   NOTE  The spatial containment relationship, established by IfcRelContainedInSpatialStructure, is required to be an hierarchical relationship, i.e. each element can only be assigned to 0 or 1 spatial structure element.
        /// </remarks>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelContainedInSpatialStructure> ContainsElements
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelContainedInSpatialStructure>(
                        r => r.RelatingStructure == this);
            }
        }


        /// <summary>
        ///   returns the parent spatial structural element that this element decomposes
        /// </summary>
        
        public IfcObjectDefinition SpatialStructuralElementParent
        {
            get
            {
                IEnumerable<IfcRelDecomposes> result = this.Decomposes;
                if (result != null)
                {
                    IfcRelDecomposes decomp = result.FirstOrDefault();
                    if (decomp != null) return decomp.RelatingObject;
                }
                return null;
            }
        }

        #endregion

        /// <summary>
        ///   returns a list of spatial structural elements which decompose this spatial object
        /// </summary>
        
        public IEnumerable<IfcObjectDefinition> SpatialStructuralElementChildren
        {
            get
            {
                IEnumerable<IfcRelDecomposes> ret = IsDecomposedBy;
                if (ret != null)
                    return ret.SelectMany(rel => rel.RelatedObjects).Cast<IfcObjectDefinition>();
                else
                    return Enumerable.Empty<IfcObjectDefinition>();
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Decomposes.Count() != 1)
                baseErr +=
                    "WR41 SpatialStructureElement: All spatial structure elements shall be associated with another spatial structure element, or with a Project\n";
            IfcRelDecomposes rel = Decomposes.FirstOrDefault();
            if (!(rel is IfcRelAggregates))
                baseErr +=
                    "WR41 SpatialStructureElement: All spatial structure elements shall be associated using the RelAggregates relationship.\n";
            if (rel != null && !(rel.RelatingObject is IfcProject || rel.RelatingObject is IfcSpatialStructureElement))
                baseErr +=
                    "WR41 SpatialStructureElement: All spatial structure elements shall be associated with another spatial structure element, or with a Project\n";
            return baseErr;
        }
    }
}