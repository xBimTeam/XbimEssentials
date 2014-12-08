#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOneDirectionRepeatFactor.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcOneDirectionRepeatFactor : IfcGeometricRepresentationItem, IfcHatchLineDistanceSelect
    {
        #region Fields

        private IfcVector _repeatFactor;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcVector RepeatFactor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _repeatFactor;
            }
            set { this.SetModelValue(this, ref _repeatFactor, value, v => RepeatFactor = v, "RepeatFactor"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _repeatFactor = (IfcVector) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}