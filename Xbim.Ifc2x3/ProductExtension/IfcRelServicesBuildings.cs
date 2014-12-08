#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelServicesBuildings.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   An objectified relationship that defines the relationship between a system and the sites, buildings, storeys or spaces, it serves.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An objectified relationship that defines the relationship between a system and the sites, buildings, storeys or spaces, it serves. Examples of systems are:
    ///   building service systems (heating, cooling, waste water system) represented by instances of IfcSystem 
    ///   idealized structural analysis systems represented by instances of IfcStructuralAnalysisSystem 
    ///   HISTORY New entity in IFC Release 1.0 
    ///   IFC2x PLATFORM CHANGE  The data type of the attributeRelatedBuildings has been changed from IfcBuilding to its supertype IfcSpatialStructureElement with upward compatibility for file based exchange. The name IfcRelServicesBuildings is a known anomaly, as the relationship is not restricted to buildings anymore.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelServicesBuildings : IfcRelConnects
    {
        public IfcRelServicesBuildings()
        {
            _relatedBuildings = new XbimList<IfcSpatialStructureElement>(this);
        }

        #region Fields

        private IfcSystem _relatingSystem;
        private XbimList<IfcSpatialStructureElement> _relatedBuildings;

        #endregion

        /// <summary>
        ///   System that services the Buildings.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcSystem RelatingSystem
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingSystem;
            }
            set { this.SetModelValue(this, ref _relatingSystem, value, v => RelatingSystem = v, "RelatingSystem"); }
        }

        /// <summary>
        ///   Spatial structure elements (including site, building, storeys) that are serviced by the system.
        /// </summary>
        /// <remarks>
        ///   IFC2x PLATFORM CHANGE  The data type has been changed from IfcBuilding to IfcSpatialStructureElement with upward compatibility for file based exchange.
        /// </remarks>
        [IfcAttribute(6, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimList<IfcSpatialStructureElement> RelatedBuildings
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedBuildings;
            }
            set
            {
                this.SetModelValue(this, ref _relatedBuildings, value, v => RelatedBuildings = v,
                                           "RelatedBuildings");
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
                    _relatingSystem = (IfcSystem) value.EntityVal;
                    break;
                case 5:
                    _relatedBuildings.Add((IfcSpatialStructureElement) value.EntityVal);
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