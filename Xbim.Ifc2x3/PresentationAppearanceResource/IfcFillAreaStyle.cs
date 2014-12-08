#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFillAreaStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcFillAreaStyle : IfcPresentationStyle, IfcPresentationStyleSelect
    {
        public IfcFillAreaStyle()
        {
            _fillStyles = new XbimSet<IfcFillStyleSelect>(this);
        }

        #region Fields

        private XbimSet<IfcFillStyleSelect> _fillStyles;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public XbimSet<IfcFillStyleSelect> FillStyles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fillStyles;
            }
            set { this.SetModelValue(this, ref _fillStyles, value, v => FillStyles = v, "FillStyles "); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _fillStyles.Add((IfcFillStyleSelect) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (
                FillStyles.Where(instance => instance is IfcColourSpecification || instance is IfcPreDefinedColour).
                    Count() > 1)
                return "WR11 FillAreaStyle: There shall be a maximum of one colour assignment to the fill area style. ";
            return "";
        }
    }
}