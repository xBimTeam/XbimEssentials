#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralAction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcStructuralAction : IfcStructuralActivity
    {
        #region Fields

        private Boolean _destabilizingLoad;
        private IfcStructuralReaction _causedBy;

        #endregion

        #region Properties

        /// <summary>
        ///   Indicates if this action may cause a stability problem. If it is 'FALSE', no further investigations regarding stability problems are necessary.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public Boolean DestabilizingLoad
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _destabilizingLoad;
            }
            set
            {
                this.SetModelValue(this, ref _destabilizingLoad, value, v => DestabilizingLoad = v,
                                           "DestabilizingLoad");
            }
        }


        /// <summary>
        ///   Optional reference to an instance of IfcStructuralReaction representing a result of another structural analysis model which creates this action upon the considered structural analysis model.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcStructuralReaction CausedBy
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _causedBy;
            }
            set { this.SetModelValue(this, ref _causedBy, value, v => CausedBy = v, "CausedBy"); }
        }

        #endregion

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
                    _destabilizingLoad = value.BooleanVal;
                    break;
                case 10:
                    _causedBy = (IfcStructuralReaction) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}