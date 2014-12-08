#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTypeProduct.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public class IfcTypeProduct : IfcTypeObject
    {
        #region Fields

        private RepresentationMapList _representationMaps;
        private IfcLabel _tag;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   List of unique representation maps. Each representation map describes a block definition of the shape of the product style.
        ///   By providing more than one representation map, a multi-view block definition can be given.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional, IfcAttributeType.ListUnique, IfcAttributeType.Class, 1)]
        public RepresentationMapList RepresentationMaps
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representationMaps;
            }
            set
            {
                this.SetModelValue(this, ref _representationMaps, value, v => RepresentationMaps = v,
                                           "RepresentationMaps");
            }
        }

        /// <summary>
        ///   The tag (or label) identifier at the particular type of a product, e.g. the article number (like the EAN). It is the identifier at the specific level.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel Tag
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _tag;
            }
            set { this.SetModelValue(this, ref _tag, value, v => Tag = v, "Tag"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    if (_representationMaps == null)
                        _representationMaps = new RepresentationMapList(this);
                    _representationMaps.Add((IfcRepresentationMap) value.EntityVal);
                    break;
                case 7:
                    _tag = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            IfcRelDefinesByType rel = ObjectTypeOf.FirstOrDefault();
            if (rel != null && rel.RelatedObjects.Where(o => !(o is IfcProduct || o is IfcProxy)).Count() != 0)
                baseErr +=
                    "WR41 TypeProduct: The product style, if assigned to an object, shall only be assigned to object of type Product or Proxy.";

            return baseErr;
        }
    }
}