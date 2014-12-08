#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReinforcingMesh.cs
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

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcReinforcingMesh : IfcReinforcingElement
    {
        #region Fields

        private IfcPositiveLengthMeasure? _meshLength;
        private IfcPositiveLengthMeasure? _meshWidth;
        private IfcPositiveLengthMeasure _longitudinalBarNominalDiameter;
        private IfcPositiveLengthMeasure _transverseBarNominalDiameter;
        private IfcAreaMeasure _longitudinalBarCrossSectionArea;
        private IfcAreaMeasure _transverseBarCrossSectionArea;
        private IfcPositiveLengthMeasure _longitudinalBarSpacing;
        private IfcPositiveLengthMeasure _transverseBarSpacing;

        #endregion

        #region Properties

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MeshLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _meshLength;
            }
            set { this.SetModelValue(this, ref _meshLength, value, v => MeshLength = v, "MeshLength"); }
        }

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MeshWidth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _meshWidth;
            }
            set { this.SetModelValue(this, ref _meshWidth, value, v => MeshWidth = v, "MeshWidth"); }
        }

        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure LongitudinalBarNominalDiameter
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _longitudinalBarNominalDiameter;
            }
            set
            {
                this.SetModelValue(this, ref _longitudinalBarNominalDiameter, value,
                                           v => LongitudinalBarNominalDiameter = v, "LongitudinalBarNominalDiameter");
            }
        }

        [IfcAttribute(13, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure TransverseBarNominalDiameter
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transverseBarNominalDiameter;
            }
            set
            {
                this.SetModelValue(this, ref _transverseBarNominalDiameter, value,
                                           v => TransverseBarNominalDiameter = v, "TransverseBarNominalDiameter");
            }
        }

        [IfcAttribute(14, IfcAttributeState.Mandatory)]
        public IfcAreaMeasure LongitudinalBarCrossSectionArea
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _longitudinalBarCrossSectionArea;
            }
            set
            {
                this.SetModelValue(this, ref _longitudinalBarCrossSectionArea, value,
                                           v => LongitudinalBarCrossSectionArea = v, "LongitudinalBarCrossSectionArea");
            }
        }

        [IfcAttribute(15, IfcAttributeState.Mandatory)]
        public IfcAreaMeasure TransverseBarCrossSectionArea
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transverseBarCrossSectionArea;
            }
            set
            {
                this.SetModelValue(this, ref _transverseBarCrossSectionArea, value,
                                           v => TransverseBarCrossSectionArea = v, "TransverseBarCrossSectionArea");
            }
        }

        [IfcAttribute(16, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure LongitudinalBarSpacing
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _longitudinalBarSpacing;
            }
            set
            {
                this.SetModelValue(this, ref _longitudinalBarSpacing, value, v => LongitudinalBarSpacing = v,
                                           "LongitudinalBarSpacing");
            }
        }

        [IfcAttribute(17, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure TransverseBarSpacing
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transverseBarSpacing;
            }
            set
            {
                this.SetModelValue(this, ref _transverseBarSpacing, value, v => TransverseBarSpacing = v,
                                           "TransverseBarSpacing");
            }
        }

        #endregion

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
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _meshLength = value.RealVal;
                    break;
                case 10:
                    _meshWidth = value.RealVal;
                    break;
                case 11:
                    _longitudinalBarNominalDiameter = value.RealVal;
                    break;
                case 12:
                    _transverseBarNominalDiameter = value.RealVal;
                    break;
                case 13:
                    _longitudinalBarCrossSectionArea = value.RealVal;
                    break;
                case 14:
                    _transverseBarCrossSectionArea = value.RealVal;
                    break;
                case 15:
                    _longitudinalBarSpacing = value.RealVal;
                    break;
                case 16:
                    _transverseBarSpacing = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}