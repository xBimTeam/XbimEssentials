#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConstructionResource.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcConstructionResource : IfcResource
    {
        private IfcIdentifier? _resourceIdentifier;
        private IfcLabel? _resourceGroup;
        private IfcResourceConsumptionEnum? _resourceConsumption;
        private IfcMeasureWithUnit _baseQuantity;

        #region Ifc Properties

        /// <summary>
        ///   Optional identification of a code or ID for the construction resource
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLabel? ResourceGroup
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _resourceGroup;
            }
            set { this.SetModelValue(this, ref _resourceGroup, value, v => ResourceGroup = v, "ResourceGroup"); }
        }


        /// <summary>
        ///   The group label, or title of the type resource, e.g. the title of a labour resource as carpenter, crane operator, superintendent, etc.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcIdentifier? ResourceIdentifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _resourceIdentifier;
            }
            set
            {
                this.SetModelValue(this, ref _resourceIdentifier, value, v => ResourceIdentifier = v,
                                           "ResourceIdentifier");
            }
        }

        /// <summary>
        ///   A value that indicates how the resource is consumed during its use in a process (see IfcResourceConsumptionEnum for more detail)
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcResourceConsumptionEnum? ResourceConsumption
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _resourceConsumption;
            }
            set
            {
                this.SetModelValue(this, ref _resourceConsumption, value, v => ResourceConsumption = v,
                                           "ResourceConsumption");
            }
        }

        /// <summary>
        ///   The basic (i.e. default, or recommended) unit that should be used for measuring the volume (or amount) of the resource and the basic quantity of the resource fully or partially consumed.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcMeasureWithUnit BaseQuantity
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _baseQuantity;
            }
            set { this.SetModelValue(this, ref _baseQuantity, value, v => BaseQuantity = v, "BaseQuantity"); }
        }

        #endregion

        #region Part 21 Step file Parse routines

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
                    _resourceIdentifier = value.StringVal;
                    break;
                case 6:
                    _resourceGroup = value.StringVal;
                    break;
                case 7:
                    _resourceConsumption =
                        (IfcResourceConsumptionEnum) Enum.Parse(typeof (IfcResourceConsumptionEnum), value.EnumVal);
                    break;
                case 8:
                    _baseQuantity = (IfcMeasureWithUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}