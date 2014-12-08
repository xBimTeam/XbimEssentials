#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFailureConnectionCondition.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    [IfcPersistedEntityAttribute]
    public class IfcFailureConnectionCondition : IfcStructuralConnectionCondition
    {
        #region Fields

        private IfcForceMeasure? _tensionFailureX;
        private IfcForceMeasure? _tensionFailureY;
        private IfcForceMeasure? _tensionFailureZ;
        private IfcForceMeasure? _compressionFailureX;
        private IfcForceMeasure? _compressionFailureY;
        private IfcForceMeasure? _compressionFailureZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Tension force in x-direction leading to failure of the connection.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcForceMeasure? TensionFailureX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _tensionFailureX;
            }
            set
            {
                this.SetModelValue(this, ref _tensionFailureX, value, v => TensionFailureX = v,
                                           "TensionFailureX");
            }
        }

        /// <summary>
        ///   Tension force in y-direction leading to failure of the connection.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcForceMeasure? TensionFailureY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _tensionFailureY;
            }
            set
            {
                this.SetModelValue(this, ref _tensionFailureY, value, v => TensionFailureY = v,
                                           "TensionFailureY");
            }
        }

        /// <summary>
        ///   Tension force in z-direction leading to failure of the connection.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcForceMeasure? TensionFailureZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _tensionFailureZ;
            }
            set
            {
                this.SetModelValue(this, ref _tensionFailureZ, value, v => TensionFailureZ = v,
                                           "TensionFailureZ");
            }
        }

        /// <summary>
        ///   Compression force in x-direction leading to failure of the connection.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcForceMeasure? CompressionFailureX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _compressionFailureX;
            }
            set
            {
                this.SetModelValue(this, ref _compressionFailureX, value, v => CompressionFailureX = v,
                                           "CompressionFailureX");
            }
        }

        /// <summary>
        ///   Compression force in y-direction leading to failure of the connection.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcForceMeasure? CompressionFailureY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _compressionFailureY;
            }
            set
            {
                this.SetModelValue(this, ref _compressionFailureY, value, v => CompressionFailureY = v,
                                           "CompressionFailureY");
            }
        }

        /// <summary>
        ///   Compression force in z-direction leading to failure of the connection.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcForceMeasure? CompressionFailureZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _compressionFailureZ;
            }
            set
            {
                this.SetModelValue(this, ref _compressionFailureZ, value, v => CompressionFailureZ = v,
                                           "CompressionFailureZ");
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _tensionFailureX = value.RealVal;
                    break;
                case 2:
                    _tensionFailureY = value.RealVal;
                    break;
                case 3:
                    _tensionFailureZ = value.RealVal;
                    break;
                case 4:
                    _compressionFailureX = value.RealVal;
                    break;
                case 5:
                    _compressionFailureY = value.RealVal;
                    break;
                case 6:
                    _compressionFailureZ = value.RealVal;
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