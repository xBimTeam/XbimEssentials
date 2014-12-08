#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcClassificationReference.cs
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

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   An IfcClassificationReference is a reference into a classification system or source (see IfcClassification).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcClassificationReference is a reference into a classification system or source (see IfcClassification). 
    ///   An optional inherited ItemReference key is also provided to allow more specific references to classification items (or tables) by type. 
    ///   The inherited Name attribute allows for a human interpretable designation of a classification notation (or code) - see use definition of "Lightweight Classification".
    ///   HISTORY: New entity in IFC 2x.
    ///   Use Definitions
    ///   Lightweight Classification
    ///   The IfcClassificationReference can be used as a form of 'lightweight' classification through the 'ItemReference' attribute inherited from the abstract 
    ///   IfcExternalReference class. In this case, the 'ItemReference' could take (for instance) the Uniclass notation "L6814" which, i
    ///   f the classification was well understood by all parties and was known to be taken from a particular classification source, would be sufficient. 
    ///   The Name attribute could be the title "Tanking". This would remove the need for the overhead of the more complete classification structure of the model.
    ///   However, it is not recommended that this lightweight method be used in cases where more than one classification system is in use or 
    ///   where there may be uncertainty as to the origin or meaning of the classification.
    ///   Referencing from an External Source
    ///   Classifications of an object may be referenced from an external source rather than being contained within the IFC model. 
    ///   This is done through the IfcClassificationReference class.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcClassificationReference : IfcExternalReference, IfcClassificationNotationSelect
    {
        #region Fields

        private IfcClassification _referencedSource;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. The classification system or source that is referenced.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcClassification ReferencedSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _referencedSource;
            }
            set
            {
                this.SetModelValue(this, ref _referencedSource, value, v => ReferencedSource = v,
                                           "ReferencedSource");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _referencedSource = (IfcClassification) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}