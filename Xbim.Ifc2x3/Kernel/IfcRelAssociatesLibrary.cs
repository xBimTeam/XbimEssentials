#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatesLibrary.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This objectified relationship (IfcRelAssociatesLibrary) handles the assignment of a library item (items of the select IfcLibrarySelect) to objects (subtypes of IfcObject).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship (IfcRelAssociatesLibrary) handles the assignment of a library item (items of the select IfcLibrarySelect) to objects (subtypes of IfcObject).
    ///   The relationship is used to assign a library reference or a more detailed link to a library information to objects, property sets or types. A single library reference can be applied to multiple items.
    ///   The inherited attribute RelatedObjects define the items to which the library association is applied. The attribute RelatingLibrary is the reference to a library reference, applied to the item(s).
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssociatesLibrary : IfcRelAssociates
    {
        #region Fields

        private IfcLibrarySelect _relatingLibrary;

        #endregion

        /// <summary>
        ///   Reference to a library, from which the definition of the property set is taken.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcLibrarySelect RelatingLibrary
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingLibrary;
            }
            set
            {
                this.SetModelValue(this, ref _relatingLibrary, value, v => RelatingLibrary = v,
                                           "RelatingLibrary");
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
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _relatingLibrary = (IfcLibrarySelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}