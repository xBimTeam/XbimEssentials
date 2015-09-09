#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatesClassification.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This objectified relationship (IfcRelAssociatesClassification) handles the assignment of a classification object (items of the select IfcClassificationSelect) to objects (subtypes of IfcObject).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship (IfcRelAssociatesClassification) handles the assignment of a classification object (items of the select IfcClassificationSelect) to objects (subtypes of IfcObject).
    ///   The relationship is used to assign a classification notation or a classification reference to objects. A single notation can be applied to multiple objects. Depending on the type of the RelatingClassification, either a reference to a fully described classification system can be made, or just a reference using the classification code.
    ///   The inherited attribute RelatedObjects define the objects to which the classification is applied. The attribute RelatingClassification is the reference to a classification, applied to the object(s).
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssociatesClassification : IfcRelAssociates
    {
        #region Fields

        private IfcClassificationNotationSelect _relatingClassification;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Classification applied to the objects.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcClassificationNotationSelect RelatingClassification
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingClassification;
            }
            set
            {
#if DEBUG
                if (!(value == null || value is IfcClassificationNotation || value is IfcClassificationReference))
                    throw new ArgumentException(
                        "RelatingClassification must be of type ClassificationNotation or ClassificationReference");
#endif
                this.SetModelValue(this, ref _relatingClassification, value, v => RelatingClassification = v,
                                           "RelatingClassification");
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
                    _relatingClassification = (IfcClassificationNotationSelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}