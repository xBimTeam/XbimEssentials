#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTwoDirectionRepeatFactor.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcTwoDirectionRepeatFactor : IfcOneDirectionRepeatFactor
    {
        #region Fields

        private IfcVector _secondRepeatFactor;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcVector SecondRepeatFactor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _secondRepeatFactor;
            }
            set
            {
                this.SetModelValue(this, ref _secondRepeatFactor, value, v => SecondRepeatFactor = v,
                                           "RepeatFactor");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _secondRepeatFactor = (IfcVector) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}