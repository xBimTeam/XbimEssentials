#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelFillsElement.cs
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
    ///   Objectified relationship between an opening element and an building element that fills (or partially fills) the opening element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Objectified relationship between an opening element and an building element that fills (or partially fills) the opening element.
    ///   EXAMPLE  The insertion of a window into a wall is represented by two separate relationships. First the window opening is created within the wall by IfcWall(StandardCase) o-- IfcRelVoidsElement --o IfcOpeningElement, then the window is inserted within the opening by IfcOpeningElement o-- IfcRelFillsElement --o IfcWindow. 
    ///   HISTORY New entity in IFC Release 1.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelFillsElement : IfcRelConnects
    {
        #region Fields

        private IfcOpeningElement _relatingOpeningElement;
        private IfcElement _relatedBuildingElement;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Opening Element being filled by virtue of this relationship.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcOpeningElement RelatingOpeningElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingOpeningElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatingOpeningElement, value, v => RelatingOpeningElement = v,
                                           "RelatingOpeningElement");
            }
        }

        /// <summary>
        ///   Reference to element that occupies fully or partially the associated opening.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcElement RelatedBuildingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedBuildingElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatedBuildingElement, value, v => RelatedBuildingElement = v,
                                           "RelatedBuildingElement");
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
                    _relatingOpeningElement = (IfcOpeningElement) value.EntityVal;
                    break;
                case 5:
                    _relatedBuildingElement = (IfcElement) value.EntityVal;
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