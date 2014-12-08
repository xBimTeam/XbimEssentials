#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPile.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcPile : IfcBuildingElement
    {
        #region Fields

        private IfcPileTypeEnum _predefinedType;
        private IfcPileConstructionEnum? _constructionType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Predefined shape types for a stair that are specified in an Enum.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcPileTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

        /// <summary>
        ///   General designator for how the pile is constructed.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPileConstructionEnum? ConstructionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _constructionType;
            }
            set
            {
                this.SetModelValue(this, ref _constructionType, value, v => ConstructionType = v,
                                           "ConstructionType");
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
                case 5:
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _predefinedType = (IfcPileTypeEnum) Enum.Parse(typeof (IfcPileTypeEnum), value.EnumVal, true);
                    break;
                case 9:
                    _constructionType =
                        (IfcPileConstructionEnum) Enum.Parse(typeof (IfcPileConstructionEnum), value.EnumVal, true);
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
            if (_predefinedType == IfcPileTypeEnum.USERDEFINED && ObjectType == null)
                baseErr +=
                    "WR1 Pile: The attribute ObjectType shall be given, if the predefined type is set to USERDEFINED\n";
            return baseErr;
        }

        #endregion
    }
}