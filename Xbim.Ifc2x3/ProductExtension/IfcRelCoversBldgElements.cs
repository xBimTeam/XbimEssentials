#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelCoversBldgElements.cs
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

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Objectified relationship between an element and one to many coverings, which do cover the building element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Objectified relationship between an element and one to many coverings, which do cover the building element. 
    ///   HISTORY New Entity in IFC Release 1.5 
    ///   IFC2x PLATFORM CHANGE: The data type of the attributeRelatingElement has been changed from IfcBuildingElement to its supertype IfcElement with upward compatibility for file based exchange. 
    ///   EXPRESS specification:
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelCoversBldgElements : IfcRelConnects
    {
        public IfcRelCoversBldgElements()
        {
            _relatedCoverings = new XbimSet<IfcCovering>(this);
        }

        #region Fields

        private IfcElement _relatingBuildingElement;
        private XbimSet<IfcCovering> _relatedCoverings;

        #endregion

        /// <summary>
        ///   Relationship to the element that is covered.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcElement RelatingBuildingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingBuildingElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatingBuildingElement, value, v => RelatingBuildingElement = v,
                                           "RelatingBuildingElement");
            }
        }

        /// <summary>
        ///   Relationship to the set of coverings at this element.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcCovering> RelatedCoverings
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedCoverings;
            }
            set
            {
                this.SetModelValue(this, ref _relatedCoverings, value, v => RelatedCoverings = v,
                                           "RelatedCoverings");
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
                    _relatingBuildingElement = (IfcElement) value.EntityVal;
                    break;
                case 5:
                    _relatedCoverings.Add((IfcCovering) value.EntityVal);
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