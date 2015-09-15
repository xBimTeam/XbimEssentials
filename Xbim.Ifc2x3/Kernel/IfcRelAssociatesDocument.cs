#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatesDocument.cs
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
    ///   This objectified relationship (IfcRelAssociatesDocument) handles the assignment of a document information (items of the select IfcDocumentSelect) to objects (subtypes of IfcObject).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship (IfcRelAssociatesDocument) handles the assignment of a document information (items of the select IfcDocumentSelect) to objects (subtypes of IfcObject).
    ///   The relationship is used to assign a document reference or a more detailed document information to objects. A single document reference can be applied to multiple objects.
    ///   The inherited attribute RelatedObjects define the objects to which the document association is applied. The attribute RelatingDocument is the reference to a document reference, applied to the object(s).
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssociatesDocument : IfcRelAssociates
    {
        #region Fields

        private IfcDocumentSelect _relatingDocument;

        #endregion

        /// <summary>
        ///   Document information or reference which is applied to the objects.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcDocumentSelect RelatingDocument
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingDocument;
            }
            set
            {
                this.SetModelValue(this, ref _relatingDocument, value, v => RelatingDocument = v,
                                           "RelatingDocument");
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
                    _relatingDocument = (IfcDocumentSelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return base.WhereRule();
        }
    }
}