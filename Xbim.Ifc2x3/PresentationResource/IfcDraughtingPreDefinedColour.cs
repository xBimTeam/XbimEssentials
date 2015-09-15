#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDraughtingPreDefinedColour.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

#endregion

namespace Xbim.Ifc2x3.PresentationResource
{
    [IfcPersistedEntityAttribute]
    public class IfcDraughtingPreDefinedColour : IfcPreDefinedColour, IfcFillStyleSelect
    {
       

        public static string[] ValidColourNames = new[]
                                                      {
                                                          "black", "red", "green", "blue", "yellow", "magenta", "cyan",
                                                          "white", "by layer"
                                                      };

       

        /// <summary>
        ///   Valid names for draughting colours are "black","red","green","blue","yellow", "magenta","cyan","white","by layer"
        /// </summary>
        /// 
        [IfcAttribute(1, IfcAttributeState.Mandatory)]        
        public override IfcLabel Name
        {
            get { return base.Name; }
            set
            {
                if (ValidColourNames.Contains((string) value))
                    SetName(value);
                else
                    throw new ArgumentException("Invalid draughting colour name");
            }
        }

        public IfcColourRgb AsColourRgb
        {
            get
            {
                //split case statements to overcome bug in db4o, which cannot traverse more than 5 deep in a case statement
                switch (Name)
                {
                    case "black":
                        return new IfcColourRgb {Name = "black", Red = 0.0, Green = 0.0, Blue = 0.0};
                    case "red":
                        return new IfcColourRgb {Name = "red", Red = 1.0, Green = 0.0, Blue = 0.0};
                    case "green":
                        return new IfcColourRgb {Name = "green", Red = 0.0, Green = 1.0, Blue = 0.0};
                    case "blue":
                        return new IfcColourRgb {Name = "blue", Red = 0.0, Green = 0.0, Blue = 1.0};
                }
                switch (Name)
                {
                    case "yellow":
                        return new IfcColourRgb {Name = "yellow", Red = 1.0, Green = 1.0, Blue = 0.0};
                    case "magenta":
                        return new IfcColourRgb {Name = "magenta", Red = 1.0, Green = 0.0, Blue = 1.0};
                    case "cyan":
                        return new IfcColourRgb {Name = "cyan", Red = 0.0, Green = 1.0, Blue = 1.0};
                    case "white":
                        return new IfcColourRgb {Name = "white", Red = 1.0, Green = 1.0, Blue = 1.0};
                    default:
                        return null;
                }
            }
        }

        public override string ToString()
        {
            return base.Name;
        }

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (!ValidColourNames.Contains((string) Name))
                return
                    "WR31 DraughtingPreDefinedColour: The inherited name for pre defined items shall only have the value of one of the following words. 'black','red','green','blue','yellow', 'magenta','cyan','white','by layer'";
            return null;
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }

}
