using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntity]
    public class IfcRightCircularCylinder : IfcCsgPrimitive3D
    {
        #region Fields

        private IfcPositiveLengthMeasure _height;
        private IfcPositiveLengthMeasure _radius;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///    Height of the Cylinder
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
        ///    Radius of the Cylinder
        /// </summary>
        [Ifc(3, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Radius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _radius;
            }
            set { this.SetModelValue(this, ref _radius, value, v => Radius = v, "Radius"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _height= value.RealVal;
                    break;
                case 2:
                    _radius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
        #endregion
    }
    
   
}
