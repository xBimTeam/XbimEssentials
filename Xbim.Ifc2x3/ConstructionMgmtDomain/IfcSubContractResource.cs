#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSubContractResource.cs
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

namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcSubContractResource : IfcConstructionResource
    {
        private IfcActorSelect _subContractor;
        private IfcText? _jobDescription;

        /// <summary>
        ///   The actor performing the role of the subcontracted resource.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcActorSelect SubContractor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _subContractor;
            }
            set { this.SetModelValue(this, ref _subContractor, value, v => SubContractor = v, "SubContractor"); }
        }

        /// <summary>
        ///   The description of the jobs that this subcontract should complete.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcText? JobDescription
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _jobDescription;
            }
            set { this.SetModelValue(this, ref _jobDescription, value, v => JobDescription = v, "JobDescription"); }
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
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _subContractor = (IfcActorSelect) value.EntityVal;
                    break;
                case 10:
                    _jobDescription = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}