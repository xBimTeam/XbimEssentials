#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTextLiteralWithExtent.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    public class IfcTextLiteralWithExtent : IfcTextLiteral
    {
        #region Fields

        private IfcPlanarExtent _extent;
        private IfcBoxAlignment _boxAlignment;

        #endregion

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPlanarExtent Extent
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _extent;
            }
            set { this.SetModelValue(this, ref _extent, value, v => Extent = v, "Extent"); }
        }

        /// <summary>
        ///   The writing direction of the text literal.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcBoxAlignment BoxAlignment
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _boxAlignment;
            }
            set { this.SetModelValue(this, ref _boxAlignment, value, v => BoxAlignment = v, "BoxAlignment"); }
        }
        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _extent = (IfcPlanarExtent) value.EntityVal;
                    break;
                case 4:
                    _boxAlignment = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
