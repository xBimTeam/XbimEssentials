using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// Definition from IAI: A dimension curve terminator is an annotated symbol,
    /// which is used at a dimension curve. It normally indicates the origin or 
    /// target of the dimension curve.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcDimensionCurveTerminator : IfcTerminatorSymbol
    {
        #region fields
        IfcDimensionExtentUsage _role;
        #endregion

        /// <summary>
        /// Role of the dimension curve terminator within a dimension curve (being either an origin or target). 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcDimensionExtentUsage Role
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _role;
            }
            set { this.SetModelValue(this, ref _role, value, v => Role = v, "Role"); }
        }

        public override string WhereRule()
        {
            var result = base.WhereRule();
            if (!(AnnotatedCurve is IfcDimensionCurve))
                result += "WR61: A dimension curve terminator shall only be assigned to a dimension curve. \n";
            return result;
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
                    _role = (IfcDimensionExtentUsage)Enum.Parse(typeof(IfcDimensionExtentUsage), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
