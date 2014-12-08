#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelSequence.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This objectified relationship handles the concatenation of processes over time.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship handles the concatenation of processes over time. The sequence is defined as relationship between two processes. The related object is the successor of the relating object, being the predecessor. A time lag is assigned to a sequence, and the sequence type defines the way in which the time lag applies to the sequence. 
    ///   IfcRelSequence is defined as an one-to-one relationship, therefore it assigns one predecessor to one successor. However, each IfcProcess can have multiple predecessors and successors, as the sequence relationship is truly an N-to-M relationship. 
    ///   HISTORY: New entity in IFC Release 1.0.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelSequence : IfcRelConnects
    {
        private IfcTimeMeasure _timeLag;
        private IfcSequenceEnum _sequenceType;
        private IfcProcess _relatedProcess;
        private IfcProcess _relatingProcess;

        /// <summary>
        ///   Reference to the Process, that is the predecessor.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcProcess RelatingProcess
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingProcess;
            }
            set
            {
                this.SetModelValue(this, ref _relatingProcess, value, v => RelatingProcess = v,
                                           "RelatingProcess");
            }
        }

        /// <summary>
        ///   Reference to the Process, that is the successor.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcProcess RelatedProcess
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedProcess;
            }
            set { this.SetModelValue(this, ref _relatedProcess, value, v => RelatedProcess = v, "RelatedProcess"); }
        }

        /// <summary>
        ///   The way in which the time lag applies to the sequence.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcTimeMeasure TimeLag
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _timeLag;
            }
            set { this.SetModelValue(this, ref _timeLag, value, v => TimeLag = v, "TimeLag"); }
        }

        /// <summary>
        ///   Time Duration of the sequence, it is the time lag between the predecessor and the successor as specified by the SequenceType.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcSequenceEnum SequenceType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sequenceType;
            }
            set { this.SetModelValue(this, ref _sequenceType, value, v => SequenceType = v, "SequenceType"); }
        }

        #region Part 21 Step file Parse routines

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
                    _relatingProcess = (IfcProcess) value.EntityVal;
                    break;
                case 5:
                    _relatedProcess = (IfcProcess) value.EntityVal;
                    break;
                case 6:
                    _timeLag = value.RealVal;
                    break;
                case 7:
                    _sequenceType = (IfcSequenceEnum)Enum.Parse(typeof(IfcSequenceEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (_relatedProcess == _relatingProcess)
                return
                    "WR1 RelSequence : The RelatingProcess shall not point to the same instance as the RelatedProcess. ";
            else
                return "";
        }
    }
}