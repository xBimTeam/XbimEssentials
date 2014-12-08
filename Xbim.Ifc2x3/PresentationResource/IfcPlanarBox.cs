using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationResource
{
    /// <summary>
    /// A planar box specifies an arbitrary rectangular box and its location in a two dimensional Cartesian coordinate system.
    /// </summary>
    public class IfcPlanarBox : IfcPlanarExtent
    {
        #region Fields

        private IfcAxis2Placement _position;
        
        #endregion

        #region Properties

        /// <summary>
        ///  The IfcAxis2Placement positions a local coordinate system for the definition of the rectangle. The origin of this local coordinate system serves as the lower left corner of the rectangular box
        /// </summary>
        [Ifc(3, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement Position
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _position;
            }
            set { this.SetModelValue(this, ref _position, value, v => Position = v, "Position"); }
        }


        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                   
                case 1:
                   base.IfcParse(propIndex,value);
                    break;
                case 2:
                    _position = (IfcAxis2Placement)value.EntityVal;
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
