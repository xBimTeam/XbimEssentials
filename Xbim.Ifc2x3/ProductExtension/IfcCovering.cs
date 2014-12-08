#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCovering.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    public class CoveringSet : XbimSet<IfcCovering>
    {
        internal CoveringSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    [IfcPersistedEntityAttribute]
    public class IfcCovering : IfcBuildingElement
    {
        #region Fields

        private IfcCoveringTypeEnum? _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Predefined shape types for a stair that are specified in an Enum.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcCoveringTypeEnum? PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                    _predefinedType =
                        (IfcCoveringTypeEnum) Enum.Parse(typeof (IfcCoveringTypeEnum), value.EnumVal, true);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (PredefinedType == null)
                return baseErr;
            if (PredefinedType == IfcCoveringTypeEnum.USERDEFINED && ObjectType == null)
                baseErr +=
                    "WR61 Covering:   Either the PredefinedType attribute is unset (e.g. because an IfcCoveringType is associated), or the inherited attribute ObjectType shall be given, if the PredefinedType is set to USERDEFINED.";
            return baseErr;
        }
    }
}