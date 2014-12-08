#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.StructuralAnalysisDomain;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Generalization of all components that make up an AEC product. Those elements can be logically contained by a spatial structure element that constitutes a certain level within a project structure hierarchy (e.g., site, building, storey or space).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Generalization of all components that make up an AEC product. Those elements can be logically contained by a spatial structure element that constitutes a certain level within a project structure hierarchy (e.g., site, building, storey or space). This is done by using the IfcRelContainedInSpatialStructure relationship. 
    ///   Elements are physically existent objects, although they might be void elements, such as holes. Elements either remain permanently in the AEC product, or only temporarily, as formwork does. Elements can be either assembled on site or pre-manufactured and built in on site. 
    ///   EXAMPLEs of elements in a building construction context are walls, floors, windows and recesses. 
    ///   An element can have material and quantity information assigned through the IfcRelAssociatesMaterial and IfcRelDefinesByProperties relationship. 
    ///   In addition an element can be declared to be a specific occurrence of an element type (and thereby be defined by the element type properties) using the IfcRelDefinesByType relationship.
    ///   An element can also be defined as an element assembly that is a group of semantically and topologically related elements that form a higher level part of the AEC product. Those element assemblies are defined by virtue of the IfcRelAggregates relationship.
    ///   EXAMPLEs for element assembly are complete Roof Structures, made by several Roof Areas, or a Stair, composed by Flights and Landings. 
    ///   Elements that performs the same function may be grouped by an "Element Group By Function". It is realized by an instance of IfcGroup with the ObjectType = 'ElementGroupByFunction". 
    ///   HISTORY New entity in IFC Release 1.0 
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcElement are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcElement are part of this IFC release:
    ///   Pset_Draughting: common property set for elements introduced to handle the assignement of CAD related information (here layer name and object colour). 
    ///   Pset_QuantityTakeOff: common property set for elements introduced to handle additional description of quantity take off. 
    ///   Pset_ElementShading: common property set for elements that have shading properties to be used in energy calculations or simulations 
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcElement are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. A detailed specification for individual quantities is introduced at the level of subtypes of IfcElement.
    ///   Geometry Use Definitions
    ///   The geometric representation of any IfcElement is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. A detailed specification for the shape representaion is introduced at the level of subtypes of IfcElement.
    ///   information is provided at the level of the subtypes.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcElement : IfcProduct, IfcStructuralActivityAssignmentSelect
    {
        #region Fields

        private IfcLabel? _tag;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. The tag (or label) identifier at the particular instance of a product, e.g. the serial number, or the position number. It is the identifier at the occurrence level.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel? Tag
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _tag;
            }
            set { this.SetModelValue(this, ref _tag, value, v => _tag = v, "Tag"); }
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
                    _tag = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Inverse. Reference to the Fills Relationship that puts the Element into the Opening within another Element.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelFillsElement> FillsVoids
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelFillsElement>(r => r.RelatedBuildingElement == this);
            }
        }

        /// <summary>
        ///   Inverse. Reference to the element connection relationship. The relationship then refers to the other element to which this element is connected to.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsElements> ConnectedTo
        {
            get { return ModelOf.Instances.Where<IfcRelConnectsElements>(r => r.RelatingElement == this); }
        }

        /// <summary>
        ///   Inverse. Reference to IfcCovering by virtue of the objectified relationship IfcRelCoversBldgElement. It defines the concept of an element having coverings attached.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelCoversBldgElements> HasCoverings
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelCoversBldgElements>(
                        r => r.RelatingBuildingElement == this);
            }
        }

        /// <summary>
        ///   Inverse. Projection relationship that adds a feature (using a Boolean union) to the IfcBuildingElement.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelProjectsElement> HasProjections
        {
            get { return ModelOf.Instances.Where<IfcRelProjectsElement>(r => r.RelatingElement == this); }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///   IFC2x Edition 3 CHANGE  The inverse attribute has been added with upward compatibility for file based exchange.
        /// </remarks>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsStructuralElement> HasStructuralMember
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsStructuralElement>(
                        r => r.RelatingElement == this);
            }
        }

        /// <summary>
        ///   Inverse. Reference relationship to the spatial structure element, to which the element is additionally associated.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelReferencedInSpatialStructure> ReferencedInStructures
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelReferencedInSpatialStructure>(r => r.RelatedElements.Contains(this));
            }
        }

        /// <summary>
        ///   Inverse. Reference to the element to port connection relationship. The relationship then refers to the port which is contained in this element.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsPortToElement> HasPorts
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsPortToElement>(r => r.RelatedElement == this);
            }
        }

        /// <summary>
        ///   Inverse. Reference to the Voids Relationship that creates an opening in an element. An element can incorporate zero-to-many openings.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelVoidsElement> HasOpenings
        {
            get
            {
                
                return
                    ModelOf.Instances.Where<IfcRelVoidsElement>(r => r.RelatingBuildingElement == this);
            }
        }

        /// <summary>
        ///   Inverse. Reference to the connection relationship with realizing element. The relationship then refers to the realizing element which provides the physical manifestation of the connection relationship.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsWithRealizingElements> IsConnectionRealization
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsWithRealizingElements>(r => r.RealizingElements.Contains(this));
            }
        }

        /// <summary>
        ///   Inverse. Reference to Space Boundaries by virtue of the objectified relationship IfcRelSeparatesSpaces. It defines the concept of an Building Element bounding Spaces.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelSpaceBoundary> ProvidesBoundaries
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelSpaceBoundary>(r => r.RelatedBuildingElement == this);
            }
        }

        /// <summary>
        ///   Inverse. Reference to the element connection relationship. The relationship then refers to the other element that is connected to this element.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsElements> ConnectedFrom
        {
            get { return ModelOf.Instances.Where<IfcRelConnectsElements>(r => r.RelatedElement == this); }
        }

        /// <summary>
        ///   Inverse. Containment relationship to the spatial structure element, to which the element is primarily associated.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelContainedInSpatialStructure> ContainedInStructure
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelContainedInSpatialStructure>(r => r.RelatedElements.Contains(this));
            }
        }

        #endregion

        public override string ToString()
        {
            //return string.Format("{0} - {1}{2}", this.Tag.HasValue?this.Tag.Value.ToString():"", this.GetType().Name, Name == null ? "" : " - " + Name);
            return (Name.HasValue) ? Name.ToString() :this.GetType().Name;
        }
    }
}