#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConstructionMaterialResource.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcConstructionMaterialResource : IfcConstructionResource
    {
        private XbimSet<IfcActorSelect> _suppliers;
        private IfcRatioMeasure? _usageRatio;

        /// <summary>
        ///   The actor performing the role of the subcontracted resource.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional, IfcAttributeType.Set, IfcAttributeType.Class, 1, -1)]
        public XbimSet<IfcActorSelect> Suppliers
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _suppliers;
            }
            set { this.SetModelValue(this, ref _suppliers, value, v => Suppliers = v, "Suppliers"); }
        }

        /// <summary>
        ///   The description of the jobs that this subcontract should complete.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcRatioMeasure? UsageRatio
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _usageRatio;
            }
            set { this.SetModelValue(this, ref _usageRatio, value, v => UsageRatio = v, "UsageRatio"); }
        }

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
                case 5:
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    if (_suppliers == null)
                        _suppliers = new XbimSet<IfcActorSelect>(this);
                    _suppliers.Add((IfcActorSelect)value.EntityVal);
                    break;
                case 10:
                    _usageRatio = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            int cnt = this.ResourceOf.Count();
            if (cnt > 1)
                baseErr +=
                    "WR1 IfcConstructionMaterialResource : There should only be a single relationship, assigning products to the product resource.\n";
            if (cnt == 1 &&
                (((!this.ResourceOf.First().RelatedObjectsType.HasValue) ||
                  (this.ResourceOf.First().RelatedObjectsType.HasValue &&
                   this.ResourceOf.First().RelatedObjectsType.Value != IfcObjectType.Product))))
                baseErr +=
                    "WR2 IfcConstructionMaterialResource :  	 If a reference to a resource is given, then through the IfcRelAssignsToResource relationship with the RelatedObjectType PRODUCT.\n";
            return baseErr;
        }

        #endregion

        public void SetSuppliers(params IfcActorSelect[] suppliers)
        {
            if (_suppliers == null) _suppliers = new XbimSet<IfcActorSelect>(this);
            else
                _suppliers.Clear();
            foreach (IfcActorSelect item in suppliers)
            {
                _suppliers.Add(item);
            }
        }
    }
}