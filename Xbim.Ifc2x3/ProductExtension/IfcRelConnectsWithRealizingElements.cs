#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsWithRealizingElements.cs
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

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   IfcRelConnectsWithRealizingElements defines a generic relationship that is made between two elements that require the realization of that relationship by means of further realizing elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: IfcRelConnectsWithRealizingElements defines a generic relationship that is made between two elements that require the realization of that relationship by means of further realizing elements.
    ///   An IfcRelConnectsWithRealizingElements is a specialization of IfcRelConnectsElement where the connecting operation has the additional attribute of (one or many) realizing elements that may be used to realize or further qualify the relationship. It is defined as a ternary relationship. 
    ///   EXAMPLE: It may be used to describe the attachment of one element to another where the attachment is realized by a 'fixing' element such as a bracket. It may also be used to describe the mounting of one element onto another such as the requirement for the mounting major plant items onto builders work bases and/or anti-vibration isolators.
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsWithRealizingElements : IfcRelConnectsElements
    {

        ElementSet _realizingElements;
        private IfcLabel? _connectionType;


        public IfcRelConnectsWithRealizingElements()
        {
            _realizingElements = new ElementSet(this);
        }
        /// <summary>
        ///   Defines the elements that realize a connection relationship.
        /// </summary>
        [IndexedProperty]
        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public ElementSet RealizingElements
        {
            get 
            { 
                ((IPersistIfcEntity)this).Activate(false); 
                return _realizingElements; 
            }
            set { this.SetModelValue(this, ref _realizingElements, value, v => RealizingElements = v, "RealizingElements"); }
        }

        /// <summary>
        ///   The type of the connection given for informal purposes, it may include labels, like 'joint', 'rigid joint', 'flexible joint', etc.
        /// </summary>
       [IfcAttribute(9, IfcAttributeState.Optional)]   
       public IfcLabel? ConnectionType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _connectionType;
            }
            set { this.SetModelValue(this, ref _connectionType, value, v => ConnectionType = v, "ConnectionType"); }
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
               case 6:
                    base.IfcParse(propIndex, value);
                    break;
               case 7:
                    _realizingElements.Add((IfcElement)value.EntityVal);
                    break;
               case 8:
                    _connectionType = value.StringVal;
                    break;
               default:
                   this.HandleUnexpectedAttribute(propIndex, value); break;
           }
       }

    }
}