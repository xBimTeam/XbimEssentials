using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntity]
    public class IfcBlock : IfcCsgPrimitive3D
    {
        #region Fields

        private IfcPositiveLengthMeasure _xLength;
        private IfcPositiveLengthMeasure _yLength;
        private IfcPositiveLengthMeasure _zLength;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///     Length attribute (measured along the edge parallel to the X Axis)
        /// </summary>
        [Ifc(2, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure XLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _xLength;
            }
            set { this.SetModelValue(this, ref _xLength, value, v => XLength = v, "XLength"); }
        }

        /// <summary>
        ///     Width attribute (measured along the edge parallel to the Y Axis)
        /// </summary>
        [Ifc(3, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure YLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _yLength;
            }
            set { this.SetModelValue(this, ref _yLength, value, v => YLength = v, "YLength"); }
        }

        /// <summary>
        ///     Height attribute (measured along the edge parallel to the Z Axis).
        /// </summary>
        [Ifc(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure ZLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _zLength;
            }
            set { this.SetModelValue(this, ref _zLength, value, v => ZLength = v, "ZLength"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _xLength = value.RealVal;
                    break;
                case 2:
                    _yLength = value.RealVal;
                    break;
                case 3:
                    _zLength = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value);
                    break;
            }
        }

        #endregion

       
    }
}
