#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcColourRgb.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationResource
{
    [IfcPersistedEntityAttribute]
    public class IfcColourRgb : IfcColourSpecification, IfcFillStyleSelect, IfcColourOrFactor
    {
        #region Fields
        static string ifc2x2Colour = "2x2 Colour";
        private IfcNormalisedRatioMeasure _red = 0;
        private IfcNormalisedRatioMeasure _green = 0;
        private IfcNormalisedRatioMeasure _blue = 0;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcNormalisedRatioMeasure Red
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _red;
            }
            set { this.SetModelValue(this, ref _red, value, v => Red = v, "Red"); }
        }


        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcNormalisedRatioMeasure Green
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _green;
            }
            set { this.SetModelValue(this, ref _green, value, v => Green = v, "Green"); }
        }


        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcNormalisedRatioMeasure Blue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _blue;
            }
            set { this.SetModelValue(this, ref _blue, value, v => Blue = v, "Blue"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0 && value.Type == IfcParserType.Real) //we have a 2x2 colour with no name
            {
                theName = ifc2x2Colour;
            }
            if (theName == ifc2x2Colour) propIndex++; //move all indices along 

            switch (propIndex)
            {
                case 0:    
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _red = value.RealVal;
                    break;
                case 2:
                    _green = value.RealVal;
                    break;
                case 3:
                    _blue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Red, Green, Blue);
        }


        public override string WhereRule()
        {
            return "";
        }
    }
}