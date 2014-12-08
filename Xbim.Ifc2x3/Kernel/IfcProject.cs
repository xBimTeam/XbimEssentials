#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProject.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using System.Runtime.Serialization;

using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   The undertaking of some design, engineering, construction, or maintenance activities leading towards a product.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The undertaking of some design, engineering, construction, or maintenance activities leading towards a product. The project establishes the context for information to be exchanged or shared, and it may represent a construction project but does not have to. 
    ///   The representation context, in the case of a geometric representation context, which is referenced from the IfcProject, includes:
    ///   the default units used 
    ///   the world coordinate system 
    ///   the coordinate space dimension 
    ///   the precision used within the geometric representations, and 
    ///   optionally the indication of the true north relative to the world coordinate system 
    ///   HISTORY  New Entity in IFC Release 1.0
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcProject are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcProject are part of this IFC release:
    ///   Pset_ProjectCommon: common property set for the single project occurrence. 
    ///   Spatial Structure Use Definition
    ///   The IfcProject is used to reference the root of the spatial structure of a building (that serves as the primary project breakdown and is required to be hierarchical). The spatial structure elements are linked together, and to the IfcProject, by using the objectified relationship IfcRelAggregates. The IfcProject references them by its inverse relationship:
    ///   IfcBuilding.Decomposes -- referencing (IfcSite || IfcBuilding) by IfcRelAggregates.RelatingObject. The IfcSite or IrfcBuilding referenced shall be the root of the spatial structure. 
    ///   IfcBuilding.IsDecomposedBy -- it shall be NIL, i.e. the IfcProject shall not be decomposed into any parts. 
    ///   Informal propositions: 
    ///   There shall only be one project within the exchange context. This is enforced by the global rule IfcSingleProjectInstance. 
    ///   Formal Propositions:
    ///   WR31   :   The Name attribute has to be provided for the project. It is the short name for the project.  
    ///   WR32   :   There shall be no instance of IfcGeometricRepresentationSubContext directly included in the set of RepresentationContexts.  
    ///   WR33   :   The IfcProject represents the root of the any decomposition tree. It shall therefore not be used to decompose any other object definition.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcProject : IfcObject
    {
        public IfcProject()
        {
            _representationContexts = new RepresentationContextSet(this);
        }

        #region Fields

        private IfcLabel _longName;
        private IfcLabel _phase;
        private RepresentationContextSet _representationContexts;
        private IfcUnitAssignment _unitsInContext;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Long name for the project as used for reference purposes.
        /// </summary>

        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLabel LongName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _longName;
            }
            set { this.SetModelValue(this, ref _longName, value, v => LongName = v, "LongName"); }
        }

        /// <summary>
        ///   Optional. Current project phase, open to interpretation for all project partner, therefore given as IfcString.
        /// </summary>

        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLabel Phase
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _phase;
            }
            set { this.SetModelValue(this, ref _phase, value, v => Phase = v, "Phase"); }
        }

        /// <summary>
        ///   Context of the representations used within the project. When the project includes shape representations for its components, one or several geometric representation contexts need to be included that define e.g. the world coordinate system, the coordinate space dimensions, and/or the precision factor.
        /// </summary>


        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public RepresentationContextSet RepresentationContexts
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representationContexts;
            }
            set
            {
                this.SetModelValue(this, ref _representationContexts, value, v => RepresentationContexts = v,
                                           "RepresentationContexts");
            }
        }

        /// <summary>
        ///   Units globally assigned to measure types used within the context of this project.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcUnitAssignment UnitsInContext
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unitsInContext;
            }
            set { this.SetModelValue(this, ref _unitsInContext, value, v => UnitsInContext = v, "UnitsInContext"); }
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
                    _longName = value.StringVal;
                    break;
                case 6:
                    _phase = value.StringVal;
                    break;
                case 7:
                    _representationContexts.Add((IfcRepresentationContext) value.EntityVal);
                    break;
                case 8:
                    _unitsInContext = (IfcUnitAssignment) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}{1}", Name == null ? "Undefined Name" : (string) Name,
                                 LongName == null ? "" : " - " + LongName);
        }


        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!Name.HasValue)
                baseErr +=
                    "WR31 Project: The Name attribute has to be provided for the project. It is the short name for the project.\n";
            if (RepresentationContexts.OfType<IfcGeometricRepresentationSubContext>().Count() > 0)
                baseErr +=
                    "WR32 Project: There shall be no instance of IfcGeometricRepresentationSubContext directly included in the set of RepresentationContexts.\n";
            if (Decomposes.Count() != 0)
                baseErr +=
                    "WR33 Project:   The Project represents the root of the any decomposition tree. It shall therefore not be used to decompose any other object definition.\n";
            return baseErr;
        }
    }
}