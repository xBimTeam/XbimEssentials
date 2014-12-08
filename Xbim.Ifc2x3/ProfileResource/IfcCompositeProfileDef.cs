#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCompositeProfileDef.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProfileResource
{
    [IfcPersistedEntityAttribute]
    public class IfcCompositeProfileDef : IfcProfileDef
    {
        public IfcCompositeProfileDef()
        {
            _profiles = new XbimSet<IfcProfileDef>(this);
        }

        #region Fields

        private XbimSet<IfcProfileDef> _profiles;
        private IfcLabel? _label;

        #endregion

        #region Properties

        /// <summary>
        ///   The profiles which are used to define the composite profile.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 2)]
        public XbimSet<IfcProfileDef> Profiles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _profiles;
            }
            set { this.SetModelValue(this, ref _profiles, value, v => Profiles = v, "Profiles"); }
        }

        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLabel? Label
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _label;
            }
            set { this.SetModelValue(this, ref _label, value, v => Label = v, "Label"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _profiles.Add((IfcProfileDef) value.EntityVal);
                    break;
                case 3:
                    _label = value.StringVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            IfcProfileDef pDef = _profiles.FirstOrDefault();
            string err = "";
            if (pDef != null)
            {
                IfcProfileTypeEnum pType = pDef.ProfileType;
                foreach (IfcProfileDef profile in _profiles)
                {
                    if (pType != profile.ProfileType)
                    {
                        err += "WR1 CompositeProfileDef : Either all profiles are areas or all profiles are curves\n";
                        break;
                    }
                }
            }
            if (_profiles.OfType<IfcCompositeProfileDef>().Count() > 0)
                err +=
                    "WR2 CompositeProfileDef :   A composite profile should not include another composite profile, i.e. no recursive definitions should be allowed.\n";
            return err;
        }
    }
}