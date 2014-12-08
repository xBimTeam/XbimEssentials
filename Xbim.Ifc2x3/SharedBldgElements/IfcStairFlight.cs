#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStairFlight.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    [IfcPersistedEntityAttribute]
    public class IfcStairFlight : IfcBuildingElement
    {
        #region Fields

        private int? _numberOfRiser;
        private int? _numberOfTreads;
        private IfcPositiveLengthMeasure? _riserHeight;
        private IfcPositiveLengthMeasure? _treadLength;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Number of the risers included in the stair flight.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public int? NumberOfRiser
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _numberOfRiser;
            }
            set { this.SetModelValue(this, ref _numberOfRiser, value, v => NumberOfRiser = v, "NumberOfRiser"); }
        }

        /// <summary>
        ///   Number of treads included in the stair flight.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public int? NumberOfTreads
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _numberOfTreads;
            }
            set { this.SetModelValue(this, ref _numberOfTreads, value, v => NumberOfTreads = v, "NumberOfTreads"); }
        }

        /// <summary>
        ///   Vertical distance from tread to tread. The riser height is supposed to be equal for all stairs in a stair flight.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? RiserHeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _riserHeight;
            }
            set { this.SetModelValue(this, ref _riserHeight, value, v => RiserHeight = v, "RiserHeight"); }
        }

        /// <summary>
        ///   Horizontal distance from the front to the back of the tread. The tread length is supposed to be equal for all steps of the stair flight.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? TreadLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _treadLength;
            }
            set { this.SetModelValue(this, ref _treadLength, value, v => TreadLength = v, "TreadLength"); }
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
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _numberOfRiser = (int) value.IntegerVal;
                    break;
                case 9:
                    _numberOfTreads = (int) value.IntegerVal;
                    break;
                case 10:
                    _riserHeight = value.RealVal;
                    break;
                case 11:
                    _treadLength = value.RealVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}