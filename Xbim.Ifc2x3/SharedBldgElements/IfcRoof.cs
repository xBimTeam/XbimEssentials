#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRoof.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    [IfcPersistedEntityAttribute]
    public class IfcRoof : IfcBuildingElement
    {
        #region Fields

        private IfcRoofTypeEnum _shapeType;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(9, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcRoofTypeEnum ShapeType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _shapeType;
            }
            set { this.SetModelValue(this, ref _shapeType, value, v => _shapeType = v, "ShapeType"); }
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
                case 5:
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _shapeType = (IfcRoofTypeEnum) Enum.Parse(typeof (IfcRoofTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (IsDecomposedBy.Count() == 0 || (IsDecomposedBy.Count() == 1 && Representation == null))
                return baseErr;
            else
                return
                    baseErr +=
                    "WR1 Roof: Either the roof is not decomposed into its roof slabs (the roof can have independent geometry), or the geometry shall not be given at IfcRoof directly.\n";
        }

        #endregion
    }
}