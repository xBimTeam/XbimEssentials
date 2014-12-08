#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDocumentReference.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcDocumentReference is a reference to the location of a document.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcDocumentReference is a reference to the location of a document. The reference is given by a system interpretable Location attribute (e.g., an URL string) or by a human readable location, where the document can be found, and an optional inherited internal reference ItemReference, which refers to a system interpretable position within the document. The optional inherited Name attribute is meant to have meaning for human readers. Optional document metadata can also be captured through reference to IfcDocumentInformation.
    ///   HISTORY: New Entity in IFC Release 2.0. Modified in IFC 2x.
    ///   Use Definitions
    ///   Provides a lightweight capability that enables a document to be identified solely by reference to a name by which it is commonly known. The reference can also be used to point to document information for more detail as required.
    ///   For example, the IAI mission statement in a document "Introduction to IAI" can be referenced by IfcDocumentReference.Location = 'http://iai-international.org/intro.html', and IfcDocumentReference = 'Mission statement'. Additionally: IfcDocumentReference.ReferenceToDocument[1].Name = 'Introduction to IAI', and IfcDocumentReference.ReferenceToDocument[1].Description = 'Basic document to introduce the aims of IAI'.
    ///   Formal Propositions:
    ///   WR1   :   A name should only be given, if no document information (including the document name) is attached
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcDocumentReference : IfcExternalReference, IfcDocumentSelect
    {
        /// <summary>
        ///   The document information that is being referenced.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcDocumentInformation> ReferenceToDocument
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcDocumentInformation>(
                        d => d.DocumentReferences.Contains(this));
            }
        }
    }
}