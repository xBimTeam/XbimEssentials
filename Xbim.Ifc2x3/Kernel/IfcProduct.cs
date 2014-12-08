#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProduct.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   Any object, or any aid to define, organize and annotate an object, that relates to a geometric or spatial context.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Any object, or any aid to define, organize and annotate an object, that relates to a geometric or spatial context. Subtypes of IfcProduct usually hold a shape representation and a local placement within the project structure. 
    ///   This includes manufactured, supplied or created objects (referred to as elements) for incorporation into an AEC/FM project. This also includes objects that are created indirectly by other products, as spaces are defined by bounding elements. Products can be designated for permanent use or temporary use, an example for the latter is formwork. Products are defined by their properties and representations.
    ///   In addition to physical products (covered by the subtype IfcElement) and spatial items (covered by the subtype IfcSpatialStructureElement) the IfcProduct also includes non-physical items, that relate to a geometric or spatial contexts, such as grid, port, annotation, structural actions, etc.
    ///   HISTORY New Entity in IFC Release 1.0
    ///   Use Definition
    ///   Any instance of IfcProduct defines a particular occurrence of a product, the common type information, that relates to many similar (or identical) occurrences of IfcProduct, is handled by the IfcTypeProduct (and its subtypes), assigned to one or many occurrences of IfcProduct by using the objectified relationship IfcRelDefinesByType. The IfcTypeProduct may provide, in addition to common properties, also a common geometric representation for all occurrences.
    ///   An IfcProduct occurs at a specific location in space if it has a geometric representation assigned. It can be placed relatively to other products, but ultimately relative to the world coordinate system defined for this project.
    ///   The inherited ObjectType attribute can be used to designate a particular type of the product instance. If subtypes of IfcProduct have a PredefinedType defined, the ObjectType is used to provide the user defined, particular type of the product instance, if the PredefinedType is set to USERDEFINED.
    ///   Formal Propositions:
    ///   WR1   :   If an Representation is given, then also a LocalPlacement has to be given.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcProduct : IfcObject
    {
        #region Fields and Events

        private IfcObjectPlacement _objectPlacement;
        private IfcProductRepresentation _representation;

        #endregion

        #region Constructors and Initializers

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Placement of the product in space, the placement can either be absolute (relative to the world coordinate system), relative (relative to the object placement of another product), or constraint (e.g. relative to grid axes)
        /// </summary>
        /// <remarks>
        ///   Placement of the product in space, the placement can either be absolute (relative to the world coordinate system), relative (relative to the object placement of another product), or constraint (e.g. relative to grid axes). It is determined by the various subtypes of IfcObjectPlacement, which includes the axis placement information to determine the transformation for the object coordinate system.
        /// </remarks>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcObjectPlacement ObjectPlacement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _objectPlacement;
            }
            set
            {
                this.SetModelValue(this, ref _objectPlacement, value, v => _objectPlacement = v,
                                           "ObjectPlacement");
            }
        }

        /// <summary>
        ///   Optional. Reference to the representations of the product, being either a representation (IfcProductRepresentation) or as a special case a shape representations (IfcProductDefinitionShape).
        /// </summary>
        /// <remarks>
        ///   Reference to the representations of the product, being either a representation (IfcProductRepresentation) or as a special case a shape representations (IfcProductDefinitionShape). The product definition shape provides for multiple geometric representations of the shape property of the object within the same object coordinate system, defined by the object placement.
        /// </remarks>
        [IfcAttribute(7, IfcAttributeState.Optional),IndexedProperty]
        public IfcProductRepresentation Representation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representation;
            }
            set { this.SetModelValue(this, ref _representation, value, v => _representation = v, "Representation"); }
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
                    _objectPlacement = (IfcObjectPlacement) value.EntityVal;
                    break;
                case 6:
                    _representation = (IfcProductRepresentation) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Methods

        public void AddRepresentation(IfcRepresentation representation)
        {
            if (this.Representation == null)
                this.Representation = new IfcProductDefinitionShape();
            this.Representation.Representations.Add(representation);
        }

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Reference to the IfcRelAssignsToProduct relationship, by which other subtypes of IfcObject can be related to the product.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelAssignsToProduct> ReferencedBy
        {
            get { return ModelOf.Instances.Where<IfcRelAssignsToProduct>(a => a.RelatingProduct == this); }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Representation != null && ObjectPlacement == null)
                return
                    baseErr +=
                    "WR1 Product: If a Representation is given, then also a LocalPlacement has to be given.\n";
            else if (Representation != null && !(Representation is IfcProductDefinitionShape))
                return
                    baseErr +=
                    "WR1 Product: If a Representation is given, then Representation must be a ProductDefinitionShape.\n";
            else
                return baseErr;
        }

        #endregion
    }
}