using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ProfilePropertyResource
{
    public class IfcStructuralSteelProfileProperties : IfcStructuralProfileProperties
    {

        private IfcAreaMeasure? _shearAreaZ;
        private IfcAreaMeasure? _shearAreaY;
        private IfcPositiveRatioMeasure? _plasticShapeFactorY;
        private IfcPositiveRatioMeasure? _plasticShapeFactorZ;


        [Ifc(23, IfcAttributeState.Optional)]
        public IfcAreaMeasure? ShearAreaZ
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return this._shearAreaZ;
            }
            set
            {
                this.SetModelValue(this, ref this._shearAreaZ, value, v => ShearAreaZ = v,
                                           "ShearAreaZ");
            }
        }

        [IfcAttribute(24, IfcAttributeState.Optional)]
        public IfcAreaMeasure? ShearAreaY
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return this._shearAreaY;
            }
            set
            {
                this.SetModelValue(this, ref this._shearAreaY, value, v => ShearAreaY = v,
                                           "ShearAreaY");
            }
        }

        [IfcAttribute(25, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? PlasticShapeFactorY
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return this._plasticShapeFactorY;
            }
            set
            {
                this.SetModelValue(this, ref this._plasticShapeFactorY, value, v => PlasticShapeFactorY = v,
                                           "PlasticShapeFactorY");
            }
        }

        [IfcAttribute(26, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? PlasticShapeFactorZ
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return this._plasticShapeFactorZ;
            }
            set
            {
                this.SetModelValue(this, ref this._plasticShapeFactorZ, value, v => PlasticShapeFactorZ = v,
                                           "PlasticShapeFactorZ");
            }
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
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                    base.IfcParse(propIndex, value); break;
                case 22:
                    _shearAreaZ = value.RealVal; break;
                case 23:
                    _shearAreaY = value.RealVal; break;
                case 24:
                    _plasticShapeFactorY = value.RealVal; break;
                case 25:
                    _plasticShapeFactorZ = value.RealVal; break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
