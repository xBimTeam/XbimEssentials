using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.Interfaces;


namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    /// <summary>
    /// The IfcConnectionPointEccentricity is used to describe the geometric constraints that facilitate the physical connection 
    /// of two objects at a point or vertex point with associated point coordinates. There is a physical distance, or eccentricity, 
    /// between the connection points of both object
    /// </summary>
    [IfcPersistedEntity]
    public class IfcConnectionPointEccentricity : IfcConnectionPointGeometry
    {
        #region Fields

        private IfcLengthMeasure? _eccentricityInX;
        private IfcLengthMeasure? _eccentricityInY;
        private IfcLengthMeasure? _eccentricityInZ;

        #endregion

        #region Properties

        /// <summary>
        /// Distance in x direction between the two points (or vertex points) engaged in the point connection. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLengthMeasure? EccentricityInX
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _eccentricityInX;
            }
            set { this.SetModelValue(this, ref _eccentricityInX, value, v => EccentricityInX = v, "EccentricityInX"); }
        }

        /// <summary>
        /// Distance in y direction between the two points (or vertex points) engaged in the point connection. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLengthMeasure? EccentricityInY
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _eccentricityInY;
            }
            set { this.SetModelValue(this, ref _eccentricityInY, value, v => EccentricityInY = v, "EccentricityInY"); }
        }

        /// <summary>
        /// Distance in y direction between the two points (or vertex points) engaged in the point connection. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLengthMeasure? EccentricityInZ
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _eccentricityInZ;
            }
            set { this.SetModelValue(this, ref _eccentricityInZ, value, v => EccentricityInZ = v, "EccentricityInZ"); }
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
                    _eccentricityInX = value.RealVal;
                    break;
                case 3:
                    _eccentricityInY = value.RealVal;
                    break;
                case 4:
                    _eccentricityInZ = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return base.WhereRule(); 
        }
    }
}
