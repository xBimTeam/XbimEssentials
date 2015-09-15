#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsStructuralElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   The one-to-one relationship assigns a structural member (as instance of IfcStructuralMember or its subclasses) to a physical element  (as instance of IfcElement or its subclasses) to keep the association between the design or detailing element and the structural analysis element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The one-to-one relationship assigns a structural member (as instance of IfcStructuralMember or its subclasses) to a physical element  (as instance of IfcElement or its subclasses) to keep the association between the design or detailing element and the structural analysis element. 
    ///   Both, the IfcElement and the IfcStructuralMember, may involve any number (zero, one, or many) associations between physical and analytical element. Multiple instances of IfcRelConnectsStructuralElement can therefore be used to reflect the many-to-many nature of the association between physical and analytical elements.
    ///   HISTORY  New entity in Release IFC2x Edition 3.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsStructuralElement : IfcRelConnects
    {
        #region Fields

        private IfcElement _relatingElement;
        private IfcStructuralMember _relatedStructuralMember;

        #endregion

        /// <summary>
        ///   The physical element, representing a design or detailing part, that is connected to the structural member as its (partial) analytical idealization.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcElement RelatingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatingElement, value, v => RelatingElement = v,
                                           "RelatingElement");
            }
        }

        /// <summary>
        ///   The structural member that is associated with the element of which it represents the analytical idealization.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcStructuralMember RelatedStructuralMember
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedStructuralMember;
            }
            set
            {
                this.SetModelValue(this, ref _relatedStructuralMember, value, v => RelatedStructuralMember = v,
                                           "RelatedStructuralMember");
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
                    _relatingElement = (IfcElement) value.EntityVal;
                    break;
                case 5:
                    _relatedStructuralMember = (IfcStructuralMember) value.EntityVal;
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