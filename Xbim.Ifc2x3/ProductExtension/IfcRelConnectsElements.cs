#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   This objectified relationship (IfcRelConnectsElements) provides the generalization of the connectivity between elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship (IfcRelConnectsElements) provides the generalization of the connectivity between elements. It is a 1 to 1 relationship. The concept of two elements being physically or logically connected is described independently from the connecting elements. Currently the connectivity is related to geometric entities on which the connection of the underlying basic geometry of the connecting elements occurs. 
    ///   The geometrical constraints of the connection are provided by the optional relationship to the IfcConnectionGeometry. If it is omitted then the connection is provided as a logical connection. Under this circumstance, the connection point, curve or surface has to be recalculated by the receiving application. 
    ///   HISTORY New entity in IFC Release 1.0.
    ///   Formal Propositions:
    ///   WR31   :   The instance of the relating element shall not be the same instance as the related element.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsElements : IfcRelConnects
    {
        #region Part 21 Step file Parse routines

        private IfcConnectionGeometry _connectionGeometry;
        private IfcElement _relatingElement;
        private IfcElement _relatedElement;

        /// <summary>
        ///   Optional. Relationship to the control class, that provides the geometrical constraints of the connection.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcConnectionGeometry ConnectionGeometry
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _connectionGeometry;
            }
            set
            {
                this.SetModelValue(this, ref _connectionGeometry, value, v => ConnectionGeometry = v,
                                           "ConnectionGeometry");
            }
        }

        /// <summary>
        ///   Reference to an Element that is connected by the objectified relationship.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
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
        ///   Reference to an Element that is connected by the objectified relationship.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcElement RelatedElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedElement;
            }
            set { this.SetModelValue(this, ref _relatedElement, value, v => RelatedElement = v, "RelatedElement"); }
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
                    _connectionGeometry = (IfcConnectionGeometry) value.EntityVal;
                    break;
                case 5:
                    _relatingElement = (IfcElement) value.EntityVal;
                    break;
                case 6:
                    _relatedElement = (IfcElement) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (_relatingElement == _relatedElement)
                return
                    "WR31 RelConnectsElements : The instance of the relating element shall not be the same instance as the related element.\n";
            else
                return "";
        }
    }
}