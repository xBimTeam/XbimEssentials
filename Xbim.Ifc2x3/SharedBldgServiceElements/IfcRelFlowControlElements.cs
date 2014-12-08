#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelFlowControlElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcRelFlowControlElements : IfcRelConnects
    {
        public IfcRelFlowControlElements()
        {
            _relatedControlElements = new XbimSet<IfcDistributionControlElement>(this);
        }

        #region Fields

        private XbimSet<IfcDistributionControlElement> _relatedControlElements;
        private IfcDistributionFlowElement _relatingFlowElement;

        #endregion

        /// <summary>
        ///   References control elements which may be used to impart control on the Distribution Element.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcDistributionControlElement> RelatedControlElements
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedControlElements;
            }
            set
            {
                this.SetModelValue(this, ref _relatedControlElements, value, v => RelatedControlElements = v,
                                           "RelatedControlElements");
            }
        }

        /// <summary>
        ///   Relationship to a distribution flow element
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcDistributionFlowElement RelatingFlowElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingFlowElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatingFlowElement, value, v => RelatingFlowElement = v,
                                           "RelatingFlowElement");
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
                    _relatedControlElements.Add((IfcDistributionControlElement) value.EntityVal);
                    break;
                case 5:
                    _relatingFlowElement = (IfcDistributionFlowElement) value.EntityVal;
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