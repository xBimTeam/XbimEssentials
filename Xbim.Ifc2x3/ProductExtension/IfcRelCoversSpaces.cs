#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelCoversSpaces.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The objectified relationship, IfcRelCoversSpace, relates a space object to one to many coverings, which faces (or are assigned) to the space.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The objectified relationship, IfcRelCoversSpace, relates a space object to one to many coverings, which faces (or are assigned) to the space.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelCoversSpaces : IfcRelConnects
    {
        public IfcRelCoversSpaces()
        {
            _relatedCoverings = new CoveringSet(this);
        }

        #region Fields

        private IfcSpace _relatedSpace;
        private readonly CoveringSet _relatedCoverings;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Relationship to the space object that is covered.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcSpace RelatedSpace
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedSpace;
            }
            set { this.SetModelValue(this, ref _relatedSpace, value, v => RelatedSpace = v, "RelatedSpace"); }
        }

        /// <summary>
        ///   Relationship to the set of coverings covering this space.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public CoveringSet RelatedCoverings
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedCoverings;
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
                    _relatedSpace = (IfcSpace) value.EntityVal;
                    break;
                case 5:
                    _relatedCoverings.Add((IfcCovering) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}