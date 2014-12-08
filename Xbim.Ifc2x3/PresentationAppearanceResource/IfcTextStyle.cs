#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTextStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcTextStyle : IfcPresentationStyle, IfcPresentationStyleSelect
    {
        #region Fields

        private IfcCharacterStyleSelect _textCharacterAppearance;
        private IfcTextStyleSelect _textStyle;
        private IfcTextFontSelect _textFontStyle;

        #endregion

        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcCharacterStyleSelect TextCharacterAppearance
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _textCharacterAppearance;
            }
            set
            {
                this.SetModelValue(this, ref _textCharacterAppearance, value, v => TextCharacterAppearance = v,
                                           "TextCharacterAppearance");
            }
        }

        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcTextStyleSelect TextStyle
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _textStyle;
            }
            set { this.SetModelValue(this, ref _textStyle, value, v => TextStyle = v, "TextStyle"); }
        }

        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcTextFontSelect TextFontStyle
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _textFontStyle;
            }
            set { this.SetModelValue(this, ref _textFontStyle, value, v => TextFontStyle = v, "TextFontStyle"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _textCharacterAppearance = (IfcCharacterStyleSelect) value.EntityVal;
                    break;
                case 2:
                    _textStyle = (IfcTextStyleSelect) value.EntityVal;
                    break;
                case 3:
                    _textFontStyle = (IfcTextFontSelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}