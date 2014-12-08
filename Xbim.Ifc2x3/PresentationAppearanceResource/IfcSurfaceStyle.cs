#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcSurfaceStyle : IfcPresentationStyle, IfcPresentationStyleSelect
    {
        public IfcSurfaceStyle()
        {
            _styles = new XbimSet<IfcSurfaceStyleElementSelect>(this);
        }

        #region Fields

        private IfcSurfaceSide _side;
        private XbimSet<IfcSurfaceStyleElementSelect> _styles;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcSurfaceSide Side
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _side;
            }
            set { this.SetModelValue(this, ref _side, value, v => Side = v, "Side"); }
        }

        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1, 5)]
        public XbimSet<IfcSurfaceStyleElementSelect> Styles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _styles;
            }
            set { this.SetModelValue(this, ref _styles, value, v => Styles = v, "Styles"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _side = (IfcSurfaceSide) Enum.Parse(typeof (IfcSurfaceSide), value.EnumVal, true);
                    break;
                case 2:
                    _styles.Add((IfcSurfaceStyleElementSelect) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string err = "";
            if (Styles.OfType<IfcSurfaceStyleShading>().Count() > 1)
                err +=
                    "WR11 SurfaceStyle: The SurfaceStyleShading shall only be used zero or one time within the set of Styles";
            if (Styles.OfType<IfcSurfaceStyleLighting>().Count() > 1)
                err +=
                    "WR12 SurfaceStyle: The SurfaceStyleLighting shall only be used zero or one time within the set of Styles";
            if (Styles.OfType<IfcSurfaceStyleRefraction>().Count() > 1)
                err +=
                    "WR13 SurfaceStyle: The SurfaceStyleRefraction shall only be used zero or one time within the set of Styles";
            if (Styles.OfType<IfcSurfaceStyleWithTextures>().Count() > 1)
                err +=
                    "WR11 SurfaceStyle: The SurfaceStyleWithTextures shall only be used zero or one time within the set of Styles";
            if (Styles.OfType<IfcExternallyDefinedSurfaceStyle>().Count() > 1)
                err +=
                    "WR11 SurfaceStyle: The ExternallyDefinedSurfaceStyle shall only be used zero or one time within the set of Styles";
            return err;
        }
    }
}