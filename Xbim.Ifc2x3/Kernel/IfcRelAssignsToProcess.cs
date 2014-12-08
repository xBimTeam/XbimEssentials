#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssignsToProcess.cs
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
    ///   his objectified relationship (IfcRelAssignsToProcess) handles the assignment of an object as an item the process operates on
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship (IfcRelAssignsToProcess) handles the assignment of an object as an item the process operates on. Process is related to the product that it operate on (normally as input or output) through this relationship. Processes can operate on things other than products, and can operate in ways other than input and output. 
    ///   Example, it may be common to define processes during estimating or scheduling that describe design tasks (resulting in documents), procurement tasks (resulting in construction materials), planning tasks (resulting in processes), etc. Furthermore, the ways in which process can operate on something might include "installs", "finishes", "transports", "removes", etc. The ways are described as operation types.
    ///   The inherited attribute RelatedObjects gives the references to the objects, which the process operates on. The RelatingProcess is the process, that operates on the object. The operation types are captured in the inherited attribute Name.
    ///   HISTORY: New entity in IFC Release 1.5. Has been renamed from IfcRelProcessOperatesOn in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssignsToProcess : IfcRelAssigns
    {
        #region Fields

        private IfcProcess _relatingProcess;
        private IfcMeasureWithUnit _quantityInProcess;

        #endregion

        [IfcAttribute(7, IfcAttributeState.Mandatory)]
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

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcMeasureWithUnit QuantityInProcess
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _quantityInProcess;
            }
            set
            {
                this.SetModelValue(this, ref _quantityInProcess, value, v => QuantityInProcess = v,
                                           "QuantityInProcess");
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
                case 4:
                case 5:
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    _relatingProcess = (IfcProcess) value.EntityVal;
                    break;
                case 7:
                    _quantityInProcess = (IfcMeasureWithUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (base.RelatedObjects.Contains(_relatingProcess))
                baseErr +=
                    "WR1 RelAssignsToProcess : The instance to with the relation points shall not be contained in the List of RelatedObjects.\n";
            return baseErr;
        }
    }
}