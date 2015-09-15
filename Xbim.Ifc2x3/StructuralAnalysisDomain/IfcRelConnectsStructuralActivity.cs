#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsStructuralActivity.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsStructuralActivity : IfcRelConnects
    {
        #region Fields

        private IfcStructuralActivityAssignmentSelect _relatingElement;
        private IfcStructuralActivity _relatedStructuralActivity;

        #endregion
        [IfcAttribute(5, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcStructuralActivityAssignmentSelect RelatingElement
        {
            get { return _relatingElement; }
            set
            {
                this.SetModelValue(this, ref _relatingElement, value, v => RelatingElement = v,
                                           "RelatingElement");
            }
        }
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcStructuralActivity RelatedStructuralActivity
        {
            get { return _relatedStructuralActivity; }
            set
            {
                this.SetModelValue(this, ref _relatedStructuralActivity, value,
                                           v => RelatedStructuralActivity = v, "RelatedStructuralActivity");
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
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatingElement = (IfcStructuralActivityAssignmentSelect) value.EntityVal;
                    break;
                case 5:
                    _relatedStructuralActivity = (IfcStructuralActivity) value.EntityVal;
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