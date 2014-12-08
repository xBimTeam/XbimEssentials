using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntityAttribute]
    public class IfcSphere:IfcCsgPrimitive3D
    {
        #region Fields

        private IfcPositiveLengthMeasure _radius;
       
        #endregion

        #region Part 21 Step file Parse routines


        /// <summary>
        ///    Radius of the Sphere
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
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
                    base.IfcParse(propIndex,value);
                    break;
                case 1:
                    _radius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
        #endregion
    }
}
