#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelVoidsElement.cs
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
    ///   Objectified Relationship between an building element and one opening element that creates a void in the element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Objectified Relationship between an building element and one opening element that creates a void in the element. This relationship implies a Boolean Operation of subtraction for the geometric bodies of Element and Opening Element. 
    ///   HISTORY New entity in IFC Release 1.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelVoidsElement : IfcRelConnects
    {
        #region Fields

        private IfcElement _relatingBuildingElement;
        private IfcFeatureElementSubtraction _relatedOpeningElement;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Reference to element in which a void is created by associated feature subtraction element.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        [IndexedProperty]
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
        ///   Reference to the feature subtraction element which defines a void in the associated element.
        /// </summary>
        ///
       
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcFeatureElementSubtraction RelatedOpeningElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedOpeningElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatedOpeningElement, value, v => RelatedOpeningElement = v,
                                           "RelatedOpeningElement");
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
                    _relatedOpeningElement = (IfcFeatureElementSubtraction) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}