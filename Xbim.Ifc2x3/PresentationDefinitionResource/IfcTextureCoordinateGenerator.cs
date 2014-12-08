using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    /// <summary>
    /// The IfcTextureCoordinateGenerator describes a procedurally defined mapping function with input parameter to map 2D texture coordinates to 3D geometry vertices. The allowable Mode values and input Parameter need to be agreed upon in implementer agreements.
    /// 
    /// The following additional definitions from ISO/IEC FCD 19775:200x, the Extensible 3D (X3D) specification, apply:
    /// 
    /// TextureCoordinateGenerator supports the automatic generation of texture coodinates for geometric shapes. This node can be used to set the texture coordinates. The mode field describes the algorithm used to compute texture coordinates, the following modes are foreseen in X3D:
    /// SPHERE, CAMERASPACENORMAL, CAMERASPACEPOSITION, CAMERASPACEREFLECTIONVECTOR, SPHERE-LOCAL, COORD, COORD-EYE, NOISE, NOISE-EYE, SPHERE-REFLECT, SPHERE-REFLECT-LOCAL
    /// </summary>
    [IfcPersistedEntity]
    public class IfcTextureCoordinateGenerator : IfcTextureCoordinate
    {
        public IfcTextureCoordinateGenerator()
        {
            _Parameter = new XbimSet<IfcSimpleValue>(this);
        }

        private IfcLabel _Mode;

        /// <summary>
        /// The mode describes the algorithm used to compute texture coordinates. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcLabel Mode
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Mode;
            }
            set { this.SetModelValue(this, ref _Mode, value, v => Mode = v, "Mode"); }
        }

        private XbimSet<IfcSimpleValue> _Parameter;

        /// <summary>
        /// The parameter used by the function as specified by Mode. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcSimpleValue> Parameter
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Parameter;
            }
            set { this.SetModelValue(this, ref _Parameter, value, v => Parameter = v, "Parameter"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _Mode = value.StringVal;
                    break;
                case 1:
                    _Parameter.Add((IfcSimpleValue)value.EntityVal);
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
