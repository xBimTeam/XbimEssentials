#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMechanicalFastener.cs
// Published:   05, 2012
// Last Edited: 13:00 PM on 23 05 2012
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedComponentElements
{
     [IfcPersistedEntityAttribute]
    public class IfcMechanicalFastener : IfcFastener
    {
        #region Fields
         IfcPositiveLengthMeasure? _nominalDiameter;
         IfcPositiveLengthMeasure? _nominalLength;
        #endregion

         #region Ifc Properties
         /// <summary>
         /// The nominal diameter describing the cross-section size of the fastener.
         /// </summary>
         [IfcAttribute(9, IfcAttributeState.Optional)]
         public IfcPositiveLengthMeasure? NominalDiameter
         {
             get
             {
                 ((IPersistIfcEntity)this).Activate(false);
                 return _nominalDiameter;
             }
             set { this.SetModelValue(this, ref _nominalDiameter, value, v => NominalDiameter = v, "NominalDiameter"); }
         }

         /// <summary>
         /// The nominal length describing the longitudinal dimensions of the fastener.
         /// </summary>
         [IfcAttribute(10, IfcAttributeState.Optional)]
         public IfcPositiveLengthMeasure? NominalLength
         {
             get
             {
                 ((IPersistIfcEntity)this).Activate(false);
                 return _nominalLength;
             }
             set { this.SetModelValue(this, ref _nominalLength, value, v => NominalLength = v, "NominalLength"); }
         }
         #endregion

        #region IfcParse

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
                 case 6:
                 case 7:
                     base.IfcParse(propIndex, value);
                     break;
                 case 8:
                     _nominalDiameter = value.RealVal; break;
                 case 9:
                     _nominalLength = value.RealVal; break;
                 default:
                     this.HandleUnexpectedAttribute(propIndex, value);
                     break;
             }
         }
        #endregion
    }
}
