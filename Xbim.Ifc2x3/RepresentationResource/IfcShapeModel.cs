#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcShapeModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    [IfcPersistedEntityAttribute]
    public class ShapeModelList : XbimList<IfcShapeModel>
    {
        internal ShapeModelList(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        public IfcShapeModel Lookup(string identifier)
        {
            foreach (IfcShapeModel item in this)
            {
                if (string.Compare(item.RepresentationIdentifier.GetValueOrDefault(), identifier, true) == 0)
                    return item;
            }
            return null;
        }

        public IfcShapeModel this[string identifier]
        {
            get { return Lookup(identifier); }
        }
    }

    /// <summary>
    ///   The IfcShapeModel represents the concept of a particular geometric and/or topological representation of a product's shape or a product component's shape within a representation context.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcShapeModel represents the concept of a particular geometric and/or topological representation of a product's shape or a product component's shape within a representation context. This representation context has to be a geometric representation context (with the exception of topology representations without associated geometry). The two subtypes are IfcShapeRepresentation to cover the geometric models (or sets) that represent a shape, and IfcTopologyRepresentation to cover the conectivity of a product or product component. The topology may or may not have geometry associated.
    ///   The IfcShapeModel can be a shape representation (geometric and/or topologogical) of a product (via IfcProductDefinitionShape), or a shape representation (geometric and/or topologogical)  of a component of a product shape (via IfcShapeAspect).
    ///   HISTORY  New entity in Release IFC 2x Edition 3. 
    ///   EXPRESS specification:
    ///   Formal Propositions:
    ///   WR11   :   The IfcShapeModel shall either be used by an IfcProductRepresentation, an IfcRepresentationMap or by an IfcShapeAspect
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcShapeModel : IfcRepresentation
    {
        #region Inverse Relationships

        /// <summary>
        ///   Reference to the shape aspect, for which it is the shape representation.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcShapeAspect> OfShapeAspect
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcShapeAspect>(
                        s =>  s.ShapeRepresentations.Contains(this));
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (OfProductRepresentation.Count() == 1 ^ OfShapeAspect.Count() == 1 ^ RepresentationMap.Count() == 1)
                return baseErr;
            else
                return
                    baseErr +=
                    "WR11 ShapeModel: The ShapeModel shall either be used by an IfcProductRepresentation, an IfcRepresentationMap or by an IfcShapeAspect.\n";
        }
    }
}