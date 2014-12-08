using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntity]
    public class IfcRightCircularCone : IfcCsgPrimitive3D
    {
        #region Fields

        private IfcPositiveLengthMeasure _height;
        private IfcPositiveLengthMeasure _bottomRadius;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///    Height of the Cone
        /// </summary>
        [Ifc(2, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Height
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _height;
            }
            set { this.SetModelValue(this, ref _height, value, v => Height = v, "Height"); }
        }

        /// <summary>
        ///    Radius of the Cone
        /// </summary>
        [Ifc(3, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure BottomRadius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _bottomRadius;
            }
            set { this.SetModelValue(this, ref _bottomRadius, value, v => BottomRadius = v, "BottomRadius"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _height = value.RealVal;
                    break;
                case 2:
                    _bottomRadius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
        #endregion
    }
}
