#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProductDefinitionShape.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   A product definition shape identifies a product’s shape as the conceptual idea of the form of a product.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A product definition shape identifies a product’s shape as the conceptual idea of the form of a product.
    ///   Definition from IAI: The IfcProductDefinitionShape defines all shape relevant information about an IfcProduct. It allows for multiple geometric shape representations of the same product. 
    ///   NOTE: The definition of this entity relates to the STEP entity product_definition_shape. Please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard. 
    ///   HISTORY: New Entity in IFC Release 1.5
    ///   Formal Propositions:
    ///   WR11   :   Only representations of type IfcShapeModel, i.e. either IfcShapeRepresentation or IfcTopologyRepresentation should be used to represent a product through the IfcProductDefinitionShape.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcProductDefinitionShape : IfcProductRepresentation
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Inverse. The IfcProductDefinitionShape shall be used to provide a representation for a single instance of IfcProduct.
        /// </summary>
       
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1, 1)]
        public IEnumerable<IfcProduct> ShapeOfProduct
        {
            get { return ModelOf.Instances.Where<IfcProduct>(p => p.Representation == this); }
        }

        /// <summary>
        ///   Inverse. Reference to the shape aspect that represents part of the shape or its feature distinctively.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcShapeAspect> HasShapeAspects
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcShapeAspect>(
                        s => s.PartOfProductDefinitionShape == this);
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (Representations.OfType<IfcShapeModel>().Count() != Representations.Count)
                return
                    "WR11 ProductDefinitionShape: Only representations of type IfcShapeModel, i.e. either IfcShapeRepresentation or IfcTopologyRepresentation should be used to represent a product through the IfcProductDefinitionShape";
            return "";
        }

        #endregion
    }
}