#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLibraryReference.cs
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
    ///   An IfcLibraryReference is a reference into a library of information by location (as an URL).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcLibraryReference is a reference into a library of information by location (as an URL). It also provides an optional inherited ItemReference key to allow more specific references to library sections or tables, and the inherited Name attribute allows for a human interpretable identification of the library item. Also, general information on the external library can be given through IfcLibraryInformation, accessed by ReferenceIntoLibrary.
    ///   HISTORY: New Entity in IFC Release 2.0, restructured in IFC 2x .
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcLibraryReference : IfcExternalReference, IfcLibrarySelect
    {
        /// <summary>
        ///   Optional. The library information that is being referenced.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcLibraryInformation> ReferenceIntoLibrary
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcLibraryInformation>(
                        lr => lr.LibraryReference.Contains(this));
            }
        }
    }
}