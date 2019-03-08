using System;

namespace Xbim.Ifc4.Interfaces
{
    public static class IfcValueHelper
    {
        public static IIfcValue ToIfc4(this IfcRail.MeasureResource.IfcValue member)
        {
            if (member == null)
                return null;
            switch (member.GetType().Name)
            {
                case "IfcAbsorbedDoseMeasure":
                    return new MeasureResource.IfcAbsorbedDoseMeasure((IfcRail.MeasureResource.IfcAbsorbedDoseMeasure)member);
                case "IfcAccelerationMeasure":
                    return new MeasureResource.IfcAccelerationMeasure((IfcRail.MeasureResource.IfcAccelerationMeasure)member);
                case "IfcAmountOfSubstanceMeasure":
                    return new MeasureResource.IfcAmountOfSubstanceMeasure((IfcRail.MeasureResource.IfcAmountOfSubstanceMeasure)member);
                case "IfcAngularVelocityMeasure":
                    return new MeasureResource.IfcAngularVelocityMeasure((IfcRail.MeasureResource.IfcAngularVelocityMeasure)member);
                case "IfcAreaDensityMeasure":
                    return new MeasureResource.IfcAreaDensityMeasure((IfcRail.MeasureResource.IfcAreaDensityMeasure)member);
                case "IfcAreaMeasure":
                    return new MeasureResource.IfcAreaMeasure((IfcRail.MeasureResource.IfcAreaMeasure)member);
                case "IfcBinary":
                    return new MeasureResource.IfcBinary((IfcRail.MeasureResource.IfcBinary)member);
                case "IfcBoolean":
                    return new MeasureResource.IfcBoolean((IfcRail.MeasureResource.IfcBoolean)member);
                case "IfcComplexNumber":
                    return new MeasureResource.IfcComplexNumber((IfcRail.MeasureResource.IfcComplexNumber)member);
                case "IfcCompoundPlaneAngleMeasure":
                    return new MeasureResource.IfcCompoundPlaneAngleMeasure((IfcRail.MeasureResource.IfcCompoundPlaneAngleMeasure)member);
                case "IfcContextDependentMeasure":
                    return new MeasureResource.IfcContextDependentMeasure((IfcRail.MeasureResource.IfcContextDependentMeasure)member);
                case "IfcCountMeasure":
                    return new MeasureResource.IfcCountMeasure((IfcRail.MeasureResource.IfcCountMeasure)member);
                case "IfcCurvatureMeasure":
                    return new MeasureResource.IfcCurvatureMeasure((IfcRail.MeasureResource.IfcCurvatureMeasure)member);
                case "IfcDate":
                    return new DateTimeResource.IfcDate((IfcRail.DateTimeResource.IfcDate)member);
                case "IfcDateTime":
                    return new DateTimeResource.IfcDateTime((IfcRail.DateTimeResource.IfcDateTime)member);
                case "IfcDescriptiveMeasure":
                    return new MeasureResource.IfcDescriptiveMeasure((IfcRail.MeasureResource.IfcDescriptiveMeasure)member);
                case "IfcDoseEquivalentMeasure":
                    return new MeasureResource.IfcDoseEquivalentMeasure((IfcRail.MeasureResource.IfcDoseEquivalentMeasure)member);
                case "IfcDuration":
                    return new DateTimeResource.IfcDuration((IfcRail.DateTimeResource.IfcDuration)member);
                case "IfcDynamicViscosityMeasure":
                    return new MeasureResource.IfcDynamicViscosityMeasure((IfcRail.MeasureResource.IfcDynamicViscosityMeasure)member);
                case "IfcElectricCapacitanceMeasure":
                    return new MeasureResource.IfcElectricCapacitanceMeasure((IfcRail.MeasureResource.IfcElectricCapacitanceMeasure)member);
                case "IfcElectricChargeMeasure":
                    return new MeasureResource.IfcElectricChargeMeasure((IfcRail.MeasureResource.IfcElectricChargeMeasure)member);
                case "IfcElectricConductanceMeasure":
                    return new MeasureResource.IfcElectricConductanceMeasure((IfcRail.MeasureResource.IfcElectricConductanceMeasure)member);
                case "IfcElectricCurrentMeasure":
                    return new MeasureResource.IfcElectricCurrentMeasure((IfcRail.MeasureResource.IfcElectricCurrentMeasure)member);
                case "IfcElectricResistanceMeasure":
                    return new MeasureResource.IfcElectricResistanceMeasure((IfcRail.MeasureResource.IfcElectricResistanceMeasure)member);
                case "IfcElectricVoltageMeasure":
                    return new MeasureResource.IfcElectricVoltageMeasure((IfcRail.MeasureResource.IfcElectricVoltageMeasure)member);
                case "IfcEnergyMeasure":
                    return new MeasureResource.IfcEnergyMeasure((IfcRail.MeasureResource.IfcEnergyMeasure)member);
                case "IfcForceMeasure":
                    return new MeasureResource.IfcForceMeasure((IfcRail.MeasureResource.IfcForceMeasure)member);
                case "IfcFrequencyMeasure":
                    return new MeasureResource.IfcFrequencyMeasure((IfcRail.MeasureResource.IfcFrequencyMeasure)member);
                case "IfcHeatFluxDensityMeasure":
                    return new MeasureResource.IfcHeatFluxDensityMeasure((IfcRail.MeasureResource.IfcHeatFluxDensityMeasure)member);
                case "IfcHeatingValueMeasure":
                    return new MeasureResource.IfcHeatingValueMeasure((IfcRail.MeasureResource.IfcHeatingValueMeasure)member);
                case "IfcIdentifier":
                    return new MeasureResource.IfcIdentifier((IfcRail.MeasureResource.IfcIdentifier)member);
                case "IfcIlluminanceMeasure":
                    return new MeasureResource.IfcIlluminanceMeasure((IfcRail.MeasureResource.IfcIlluminanceMeasure)member);
                case "IfcInductanceMeasure":
                    return new MeasureResource.IfcInductanceMeasure((IfcRail.MeasureResource.IfcInductanceMeasure)member);
                case "IfcInteger":
                    return new MeasureResource.IfcInteger((IfcRail.MeasureResource.IfcInteger)member);
                case "IfcIntegerCountRateMeasure":
                    return new MeasureResource.IfcIntegerCountRateMeasure((IfcRail.MeasureResource.IfcIntegerCountRateMeasure)member);
                case "IfcIonConcentrationMeasure":
                    return new MeasureResource.IfcIonConcentrationMeasure((IfcRail.MeasureResource.IfcIonConcentrationMeasure)member);
                case "IfcIsothermalMoistureCapacityMeasure":
                    return new MeasureResource.IfcIsothermalMoistureCapacityMeasure((IfcRail.MeasureResource.IfcIsothermalMoistureCapacityMeasure)member);
                case "IfcKinematicViscosityMeasure":
                    return new MeasureResource.IfcKinematicViscosityMeasure((IfcRail.MeasureResource.IfcKinematicViscosityMeasure)member);
                case "IfcLabel":
                    return new MeasureResource.IfcLabel((IfcRail.MeasureResource.IfcLabel)member);
                case "IfcLengthMeasure":
                    return new MeasureResource.IfcLengthMeasure((IfcRail.MeasureResource.IfcLengthMeasure)member);
                case "IfcLinearForceMeasure":
                    return new MeasureResource.IfcLinearForceMeasure((IfcRail.MeasureResource.IfcLinearForceMeasure)member);
                case "IfcLinearMomentMeasure":
                    return new MeasureResource.IfcLinearMomentMeasure((IfcRail.MeasureResource.IfcLinearMomentMeasure)member);
                case "IfcLinearStiffnessMeasure":
                    return new MeasureResource.IfcLinearStiffnessMeasure((IfcRail.MeasureResource.IfcLinearStiffnessMeasure)member);
                case "IfcLinearVelocityMeasure":
                    return new MeasureResource.IfcLinearVelocityMeasure((IfcRail.MeasureResource.IfcLinearVelocityMeasure)member);
                case "IfcLogical":
                    return new MeasureResource.IfcLogical((IfcRail.MeasureResource.IfcLogical)member);
                case "IfcLuminousFluxMeasure":
                    return new MeasureResource.IfcLuminousFluxMeasure((IfcRail.MeasureResource.IfcLuminousFluxMeasure)member);
                case "IfcLuminousIntensityDistributionMeasure":
                    return new MeasureResource.IfcLuminousIntensityDistributionMeasure((IfcRail.MeasureResource.IfcLuminousIntensityDistributionMeasure)member);
                case "IfcLuminousIntensityMeasure":
                    return new MeasureResource.IfcLuminousIntensityMeasure((IfcRail.MeasureResource.IfcLuminousIntensityMeasure)member);
                case "IfcMagneticFluxDensityMeasure":
                    return new MeasureResource.IfcMagneticFluxDensityMeasure((IfcRail.MeasureResource.IfcMagneticFluxDensityMeasure)member);
                case "IfcMagneticFluxMeasure":
                    return new MeasureResource.IfcMagneticFluxMeasure((IfcRail.MeasureResource.IfcMagneticFluxMeasure)member);
                case "IfcMassDensityMeasure":
                    return new MeasureResource.IfcMassDensityMeasure((IfcRail.MeasureResource.IfcMassDensityMeasure)member);
                case "IfcMassFlowRateMeasure":
                    return new MeasureResource.IfcMassFlowRateMeasure((IfcRail.MeasureResource.IfcMassFlowRateMeasure)member);
                case "IfcMassMeasure":
                    return new MeasureResource.IfcMassMeasure((IfcRail.MeasureResource.IfcMassMeasure)member);
                case "IfcMassPerLengthMeasure":
                    return new MeasureResource.IfcMassPerLengthMeasure((IfcRail.MeasureResource.IfcMassPerLengthMeasure)member);
                case "IfcModulusOfElasticityMeasure":
                    return new MeasureResource.IfcModulusOfElasticityMeasure((IfcRail.MeasureResource.IfcModulusOfElasticityMeasure)member);
                case "IfcModulusOfLinearSubgradeReactionMeasure":
                    return new MeasureResource.IfcModulusOfLinearSubgradeReactionMeasure((IfcRail.MeasureResource.IfcModulusOfLinearSubgradeReactionMeasure)member);
                case "IfcModulusOfRotationalSubgradeReactionMeasure":
                    return new MeasureResource.IfcModulusOfRotationalSubgradeReactionMeasure((IfcRail.MeasureResource.IfcModulusOfRotationalSubgradeReactionMeasure)member);
                case "IfcModulusOfSubgradeReactionMeasure":
                    return new MeasureResource.IfcModulusOfSubgradeReactionMeasure((IfcRail.MeasureResource.IfcModulusOfSubgradeReactionMeasure)member);
                case "IfcMoistureDiffusivityMeasure":
                    return new MeasureResource.IfcMoistureDiffusivityMeasure((IfcRail.MeasureResource.IfcMoistureDiffusivityMeasure)member);
                case "IfcMolecularWeightMeasure":
                    return new MeasureResource.IfcMolecularWeightMeasure((IfcRail.MeasureResource.IfcMolecularWeightMeasure)member);
                case "IfcMomentOfInertiaMeasure":
                    return new MeasureResource.IfcMomentOfInertiaMeasure((IfcRail.MeasureResource.IfcMomentOfInertiaMeasure)member);
                case "IfcMonetaryMeasure":
                    return new MeasureResource.IfcMonetaryMeasure((IfcRail.MeasureResource.IfcMonetaryMeasure)member);
                case "IfcNonNegativeLengthMeasure":
                    return new MeasureResource.IfcNonNegativeLengthMeasure((IfcRail.MeasureResource.IfcNonNegativeLengthMeasure)member);
                case "IfcNormalisedRatioMeasure":
                    return new MeasureResource.IfcNormalisedRatioMeasure((IfcRail.MeasureResource.IfcNormalisedRatioMeasure)member);
                case "IfcNumericMeasure":
                    return new MeasureResource.IfcNumericMeasure((IfcRail.MeasureResource.IfcNumericMeasure)member);
                case "IfcParameterValue":
                    return new MeasureResource.IfcParameterValue((IfcRail.MeasureResource.IfcParameterValue)member);
                case "IfcPHMeasure":
                    return new MeasureResource.IfcPHMeasure((IfcRail.MeasureResource.IfcPHMeasure)member);
                case "IfcPlanarForceMeasure":
                    return new MeasureResource.IfcPlanarForceMeasure((IfcRail.MeasureResource.IfcPlanarForceMeasure)member);
                case "IfcPlaneAngleMeasure":
                    return new MeasureResource.IfcPlaneAngleMeasure((IfcRail.MeasureResource.IfcPlaneAngleMeasure)member);
                case "IfcPositiveInteger":
                    return new MeasureResource.IfcPositiveInteger((IfcRail.MeasureResource.IfcPositiveInteger)member);
                case "IfcPositiveLengthMeasure":
                    return new MeasureResource.IfcPositiveLengthMeasure((IfcRail.MeasureResource.IfcPositiveLengthMeasure)member);
                case "IfcPositivePlaneAngleMeasure":
                    return new MeasureResource.IfcPositivePlaneAngleMeasure((IfcRail.MeasureResource.IfcPositivePlaneAngleMeasure)member);
                case "IfcPositiveRatioMeasure":
                    return new MeasureResource.IfcPositiveRatioMeasure((IfcRail.MeasureResource.IfcPositiveRatioMeasure)member);
                case "IfcPowerMeasure":
                    return new MeasureResource.IfcPowerMeasure((IfcRail.MeasureResource.IfcPowerMeasure)member);
                case "IfcPressureMeasure":
                    return new MeasureResource.IfcPressureMeasure((IfcRail.MeasureResource.IfcPressureMeasure)member);
                case "IfcRadioActivityMeasure":
                    return new MeasureResource.IfcRadioActivityMeasure((IfcRail.MeasureResource.IfcRadioActivityMeasure)member);
                case "IfcRatioMeasure":
                    return new MeasureResource.IfcRatioMeasure((IfcRail.MeasureResource.IfcRatioMeasure)member);
                case "IfcReal":
                    return new MeasureResource.IfcReal((IfcRail.MeasureResource.IfcReal)member);
                case "IfcRotationalFrequencyMeasure":
                    return new MeasureResource.IfcRotationalFrequencyMeasure((IfcRail.MeasureResource.IfcRotationalFrequencyMeasure)member);
                case "IfcRotationalMassMeasure":
                    return new MeasureResource.IfcRotationalMassMeasure((IfcRail.MeasureResource.IfcRotationalMassMeasure)member);
                case "IfcRotationalStiffnessMeasure":
                    return new MeasureResource.IfcRotationalStiffnessMeasure((IfcRail.MeasureResource.IfcRotationalStiffnessMeasure)member);
                case "IfcSectionalAreaIntegralMeasure":
                    return new MeasureResource.IfcSectionalAreaIntegralMeasure((IfcRail.MeasureResource.IfcSectionalAreaIntegralMeasure)member);
                case "IfcSectionModulusMeasure":
                    return new MeasureResource.IfcSectionModulusMeasure((IfcRail.MeasureResource.IfcSectionModulusMeasure)member);
                case "IfcShearModulusMeasure":
                    return new MeasureResource.IfcShearModulusMeasure((IfcRail.MeasureResource.IfcShearModulusMeasure)member);
                case "IfcSolidAngleMeasure":
                    return new MeasureResource.IfcSolidAngleMeasure((IfcRail.MeasureResource.IfcSolidAngleMeasure)member);
                case "IfcSoundPowerLevelMeasure":
                    return new MeasureResource.IfcSoundPowerLevelMeasure((IfcRail.MeasureResource.IfcSoundPowerLevelMeasure)member);
                case "IfcSoundPowerMeasure":
                    return new MeasureResource.IfcSoundPowerMeasure((IfcRail.MeasureResource.IfcSoundPowerMeasure)member);
                case "IfcSoundPressureLevelMeasure":
                    return new MeasureResource.IfcSoundPressureLevelMeasure((IfcRail.MeasureResource.IfcSoundPressureLevelMeasure)member);
                case "IfcSoundPressureMeasure":
                    return new MeasureResource.IfcSoundPressureMeasure((IfcRail.MeasureResource.IfcSoundPressureMeasure)member);
                case "IfcSpecificHeatCapacityMeasure":
                    return new MeasureResource.IfcSpecificHeatCapacityMeasure((IfcRail.MeasureResource.IfcSpecificHeatCapacityMeasure)member);
                case "IfcTemperatureGradientMeasure":
                    return new MeasureResource.IfcTemperatureGradientMeasure((IfcRail.MeasureResource.IfcTemperatureGradientMeasure)member);
                case "IfcTemperatureRateOfChangeMeasure":
                    return new MeasureResource.IfcTemperatureRateOfChangeMeasure((IfcRail.MeasureResource.IfcTemperatureRateOfChangeMeasure)member);
                case "IfcText":
                    return new MeasureResource.IfcText((IfcRail.MeasureResource.IfcText)member);
                case "IfcThermalAdmittanceMeasure":
                    return new MeasureResource.IfcThermalAdmittanceMeasure((IfcRail.MeasureResource.IfcThermalAdmittanceMeasure)member);
                case "IfcThermalConductivityMeasure":
                    return new MeasureResource.IfcThermalConductivityMeasure((IfcRail.MeasureResource.IfcThermalConductivityMeasure)member);
                case "IfcThermalExpansionCoefficientMeasure":
                    return new MeasureResource.IfcThermalExpansionCoefficientMeasure((IfcRail.MeasureResource.IfcThermalExpansionCoefficientMeasure)member);
                case "IfcThermalResistanceMeasure":
                    return new MeasureResource.IfcThermalResistanceMeasure((IfcRail.MeasureResource.IfcThermalResistanceMeasure)member);
                case "IfcThermalTransmittanceMeasure":
                    return new MeasureResource.IfcThermalTransmittanceMeasure((IfcRail.MeasureResource.IfcThermalTransmittanceMeasure)member);
                case "IfcThermodynamicTemperatureMeasure":
                    return new MeasureResource.IfcThermodynamicTemperatureMeasure((IfcRail.MeasureResource.IfcThermodynamicTemperatureMeasure)member);
                case "IfcTime":
                    return new DateTimeResource.IfcTime((IfcRail.DateTimeResource.IfcTime)member);
                case "IfcTimeMeasure":
                    return new MeasureResource.IfcTimeMeasure((IfcRail.MeasureResource.IfcTimeMeasure)member);
                case "IfcTimeStamp":
                    return new DateTimeResource.IfcTimeStamp((IfcRail.DateTimeResource.IfcTimeStamp)member);
                case "IfcTorqueMeasure":
                    return new MeasureResource.IfcTorqueMeasure((IfcRail.MeasureResource.IfcTorqueMeasure)member);
                case "IfcVaporPermeabilityMeasure":
                    return new MeasureResource.IfcVaporPermeabilityMeasure((IfcRail.MeasureResource.IfcVaporPermeabilityMeasure)member);
                case "IfcVolumeMeasure":
                    return new MeasureResource.IfcVolumeMeasure((IfcRail.MeasureResource.IfcVolumeMeasure)member);
                case "IfcVolumetricFlowRateMeasure":
                    return new MeasureResource.IfcVolumetricFlowRateMeasure((IfcRail.MeasureResource.IfcVolumetricFlowRateMeasure)member);
                case "IfcWarpingConstantMeasure":
                    return new MeasureResource.IfcWarpingConstantMeasure((IfcRail.MeasureResource.IfcWarpingConstantMeasure)member);
                case "IfcWarpingMomentMeasure":
                    return new MeasureResource.IfcWarpingMomentMeasure((IfcRail.MeasureResource.IfcWarpingMomentMeasure)member);
                default:
                    throw new NotSupportedException();
            }
        }
        public static IfcRail.MeasureResource.IfcValue ToIfc3(this IIfcValue member)
        {
            if (member == null)
                return null;
            var name = member.GetType().Name;
            switch (name)
            {
                case "IfcAbsorbedDoseMeasure":
                    return new IfcRail.MeasureResource.IfcAbsorbedDoseMeasure((MeasureResource.IfcAbsorbedDoseMeasure)member);
                case "IfcAccelerationMeasure":
                    return new IfcRail.MeasureResource.IfcAccelerationMeasure((MeasureResource.IfcAccelerationMeasure)member);
                case "IfcAmountOfSubstanceMeasure":
                    return new IfcRail.MeasureResource.IfcAmountOfSubstanceMeasure((MeasureResource.IfcAmountOfSubstanceMeasure)member);
                case "IfcAngularVelocityMeasure":
                    return new IfcRail.MeasureResource.IfcAngularVelocityMeasure((MeasureResource.IfcAngularVelocityMeasure)member);
                case "IfcAreaDensityMeasure":
                    return new IfcRail.MeasureResource.IfcAreaDensityMeasure((MeasureResource.IfcAreaDensityMeasure)member);
                case "IfcAreaMeasure":
                    return new IfcRail.MeasureResource.IfcAreaMeasure((MeasureResource.IfcAreaMeasure)member);
                case "IfcBinary":
                    return new IfcRail.MeasureResource.IfcBinary((MeasureResource.IfcBinary)member);
                case "IfcBoolean":
                    return new IfcRail.MeasureResource.IfcBoolean((MeasureResource.IfcBoolean)member);
                case "IfcComplexNumber":
                    return new IfcRail.MeasureResource.IfcComplexNumber((MeasureResource.IfcComplexNumber)member);
                case "IfcCompoundPlaneAngleMeasure":
                    return new IfcRail.MeasureResource.IfcCompoundPlaneAngleMeasure((MeasureResource.IfcCompoundPlaneAngleMeasure)member);
                case "IfcContextDependentMeasure":
                    return new IfcRail.MeasureResource.IfcContextDependentMeasure((MeasureResource.IfcContextDependentMeasure)member);
                case "IfcCountMeasure":
                    return new IfcRail.MeasureResource.IfcCountMeasure((MeasureResource.IfcCountMeasure)member);
                case "IfcCurvatureMeasure":
                    return new IfcRail.MeasureResource.IfcCurvatureMeasure((MeasureResource.IfcCurvatureMeasure)member);
                case "IfcDate":
                    return new IfcRail.DateTimeResource.IfcDate((DateTimeResource.IfcDate)member);
                case "IfcDateTime":
                    return new IfcRail.DateTimeResource.IfcDateTime((DateTimeResource.IfcDateTime)member);
                case "IfcDescriptiveMeasure":
                    return new IfcRail.MeasureResource.IfcDescriptiveMeasure((MeasureResource.IfcDescriptiveMeasure)member);
                case "IfcDoseEquivalentMeasure":
                    return new IfcRail.MeasureResource.IfcDoseEquivalentMeasure((MeasureResource.IfcDoseEquivalentMeasure)member);
                case "IfcDuration":
                    return new IfcRail.DateTimeResource.IfcDuration((DateTimeResource.IfcDuration)member);
                case "IfcDynamicViscosityMeasure":
                    return new IfcRail.MeasureResource.IfcDynamicViscosityMeasure((MeasureResource.IfcDynamicViscosityMeasure)member);
                case "IfcElectricCapacitanceMeasure":
                    return new IfcRail.MeasureResource.IfcElectricCapacitanceMeasure((MeasureResource.IfcElectricCapacitanceMeasure)member);
                case "IfcElectricChargeMeasure":
                    return new IfcRail.MeasureResource.IfcElectricChargeMeasure((MeasureResource.IfcElectricChargeMeasure)member);
                case "IfcElectricConductanceMeasure":
                    return new IfcRail.MeasureResource.IfcElectricConductanceMeasure((MeasureResource.IfcElectricConductanceMeasure)member);
                case "IfcElectricCurrentMeasure":
                    return new IfcRail.MeasureResource.IfcElectricCurrentMeasure((MeasureResource.IfcElectricCurrentMeasure)member);
                case "IfcElectricResistanceMeasure":
                    return new IfcRail.MeasureResource.IfcElectricResistanceMeasure((MeasureResource.IfcElectricResistanceMeasure)member);
                case "IfcElectricVoltageMeasure":
                    return new IfcRail.MeasureResource.IfcElectricVoltageMeasure((MeasureResource.IfcElectricVoltageMeasure)member);
                case "IfcEnergyMeasure":
                    return new IfcRail.MeasureResource.IfcEnergyMeasure((MeasureResource.IfcEnergyMeasure)member);
                case "IfcForceMeasure":
                    return new IfcRail.MeasureResource.IfcForceMeasure((MeasureResource.IfcForceMeasure)member);
                case "IfcFrequencyMeasure":
                    return new IfcRail.MeasureResource.IfcFrequencyMeasure((MeasureResource.IfcFrequencyMeasure)member);
                case "IfcHeatFluxDensityMeasure":
                    return new IfcRail.MeasureResource.IfcHeatFluxDensityMeasure((MeasureResource.IfcHeatFluxDensityMeasure)member);
                case "IfcHeatingValueMeasure":
                    return new IfcRail.MeasureResource.IfcHeatingValueMeasure((MeasureResource.IfcHeatingValueMeasure)member);
                case "IfcIdentifier":
                    return new IfcRail.MeasureResource.IfcIdentifier((MeasureResource.IfcIdentifier)member);
                case "IfcIlluminanceMeasure":
                    return new IfcRail.MeasureResource.IfcIlluminanceMeasure((MeasureResource.IfcIlluminanceMeasure)member);
                case "IfcInductanceMeasure":
                    return new IfcRail.MeasureResource.IfcInductanceMeasure((MeasureResource.IfcInductanceMeasure)member);
                case "IfcInteger":
                    return new IfcRail.MeasureResource.IfcInteger((MeasureResource.IfcInteger)member);
                case "IfcIntegerCountRateMeasure":
                    return new IfcRail.MeasureResource.IfcIntegerCountRateMeasure((MeasureResource.IfcIntegerCountRateMeasure)member);
                case "IfcIonConcentrationMeasure":
                    return new IfcRail.MeasureResource.IfcIonConcentrationMeasure((MeasureResource.IfcIonConcentrationMeasure)member);
                case "IfcIsothermalMoistureCapacityMeasure":
                    return new IfcRail.MeasureResource.IfcIsothermalMoistureCapacityMeasure((MeasureResource.IfcIsothermalMoistureCapacityMeasure)member);
                case "IfcKinematicViscosityMeasure":
                    return new IfcRail.MeasureResource.IfcKinematicViscosityMeasure((MeasureResource.IfcKinematicViscosityMeasure)member);
                case "IfcLabel":
                    return new IfcRail.MeasureResource.IfcLabel((MeasureResource.IfcLabel)member);
                case "IfcLengthMeasure":
                    return new IfcRail.MeasureResource.IfcLengthMeasure((MeasureResource.IfcLengthMeasure)member);
                case "IfcLinearForceMeasure":
                    return new IfcRail.MeasureResource.IfcLinearForceMeasure((MeasureResource.IfcLinearForceMeasure)member);
                case "IfcLinearMomentMeasure":
                    return new IfcRail.MeasureResource.IfcLinearMomentMeasure((MeasureResource.IfcLinearMomentMeasure)member);
                case "IfcLinearStiffnessMeasure":
                    return new IfcRail.MeasureResource.IfcLinearStiffnessMeasure((MeasureResource.IfcLinearStiffnessMeasure)member);
                case "IfcLinearVelocityMeasure":
                    return new IfcRail.MeasureResource.IfcLinearVelocityMeasure((MeasureResource.IfcLinearVelocityMeasure)member);
                case "IfcLogical":
                    return new IfcRail.MeasureResource.IfcLogical((MeasureResource.IfcLogical)member);
                case "IfcLuminousFluxMeasure":
                    return new IfcRail.MeasureResource.IfcLuminousFluxMeasure((MeasureResource.IfcLuminousFluxMeasure)member);
                case "IfcLuminousIntensityDistributionMeasure":
                    return new IfcRail.MeasureResource.IfcLuminousIntensityDistributionMeasure((MeasureResource.IfcLuminousIntensityDistributionMeasure)member);
                case "IfcLuminousIntensityMeasure":
                    return new IfcRail.MeasureResource.IfcLuminousIntensityMeasure((MeasureResource.IfcLuminousIntensityMeasure)member);
                case "IfcMagneticFluxDensityMeasure":
                    return new IfcRail.MeasureResource.IfcMagneticFluxDensityMeasure((MeasureResource.IfcMagneticFluxDensityMeasure)member);
                case "IfcMagneticFluxMeasure":
                    return new IfcRail.MeasureResource.IfcMagneticFluxMeasure((MeasureResource.IfcMagneticFluxMeasure)member);
                case "IfcMassDensityMeasure":
                    return new IfcRail.MeasureResource.IfcMassDensityMeasure((MeasureResource.IfcMassDensityMeasure)member);
                case "IfcMassFlowRateMeasure":
                    return new IfcRail.MeasureResource.IfcMassFlowRateMeasure((MeasureResource.IfcMassFlowRateMeasure)member);
                case "IfcMassMeasure":
                    return new IfcRail.MeasureResource.IfcMassMeasure((MeasureResource.IfcMassMeasure)member);
                case "IfcMassPerLengthMeasure":
                    return new IfcRail.MeasureResource.IfcMassPerLengthMeasure((MeasureResource.IfcMassPerLengthMeasure)member);
                case "IfcModulusOfElasticityMeasure":
                    return new IfcRail.MeasureResource.IfcModulusOfElasticityMeasure((MeasureResource.IfcModulusOfElasticityMeasure)member);
                case "IfcModulusOfLinearSubgradeReactionMeasure":
                    return new IfcRail.MeasureResource.IfcModulusOfLinearSubgradeReactionMeasure((MeasureResource.IfcModulusOfLinearSubgradeReactionMeasure)member);
                case "IfcModulusOfRotationalSubgradeReactionMeasure":
                    return new IfcRail.MeasureResource.IfcModulusOfRotationalSubgradeReactionMeasure((MeasureResource.IfcModulusOfRotationalSubgradeReactionMeasure)member);
                case "IfcModulusOfSubgradeReactionMeasure":
                    return new IfcRail.MeasureResource.IfcModulusOfSubgradeReactionMeasure((MeasureResource.IfcModulusOfSubgradeReactionMeasure)member);
                case "IfcMoistureDiffusivityMeasure":
                    return new IfcRail.MeasureResource.IfcMoistureDiffusivityMeasure((MeasureResource.IfcMoistureDiffusivityMeasure)member);
                case "IfcMolecularWeightMeasure":
                    return new IfcRail.MeasureResource.IfcMolecularWeightMeasure((MeasureResource.IfcMolecularWeightMeasure)member);
                case "IfcMomentOfInertiaMeasure":
                    return new IfcRail.MeasureResource.IfcMomentOfInertiaMeasure((MeasureResource.IfcMomentOfInertiaMeasure)member);
                case "IfcMonetaryMeasure":
                    return new IfcRail.MeasureResource.IfcMonetaryMeasure((MeasureResource.IfcMonetaryMeasure)member);
                case "IfcNonNegativeLengthMeasure":
                    return new IfcRail.MeasureResource.IfcNonNegativeLengthMeasure((MeasureResource.IfcNonNegativeLengthMeasure)member);
                case "IfcNormalisedRatioMeasure":
                    return new IfcRail.MeasureResource.IfcNormalisedRatioMeasure((MeasureResource.IfcNormalisedRatioMeasure)member);
                case "IfcNumericMeasure":
                    return new IfcRail.MeasureResource.IfcNumericMeasure((MeasureResource.IfcNumericMeasure)member);
                case "IfcParameterValue":
                    return new IfcRail.MeasureResource.IfcParameterValue((MeasureResource.IfcParameterValue)member);
                case "IfcPHMeasure":
                    return new IfcRail.MeasureResource.IfcPHMeasure((MeasureResource.IfcPHMeasure)member);
                case "IfcPlanarForceMeasure":
                    return new IfcRail.MeasureResource.IfcPlanarForceMeasure((MeasureResource.IfcPlanarForceMeasure)member);
                case "IfcPlaneAngleMeasure":
                    return new IfcRail.MeasureResource.IfcPlaneAngleMeasure((MeasureResource.IfcPlaneAngleMeasure)member);
                case "IfcPositiveInteger":
                    return new IfcRail.MeasureResource.IfcPositiveInteger((MeasureResource.IfcPositiveInteger)member);
                case "IfcPositiveLengthMeasure":
                    return new IfcRail.MeasureResource.IfcPositiveLengthMeasure((MeasureResource.IfcPositiveLengthMeasure)member);
                case "IfcPositivePlaneAngleMeasure":
                    return new IfcRail.MeasureResource.IfcPositivePlaneAngleMeasure((MeasureResource.IfcPositivePlaneAngleMeasure)member);
                case "IfcPositiveRatioMeasure":
                    return new IfcRail.MeasureResource.IfcPositiveRatioMeasure((MeasureResource.IfcPositiveRatioMeasure)member);
                case "IfcPowerMeasure":
                    return new IfcRail.MeasureResource.IfcPowerMeasure((MeasureResource.IfcPowerMeasure)member);
                case "IfcPressureMeasure":
                    return new IfcRail.MeasureResource.IfcPressureMeasure((MeasureResource.IfcPressureMeasure)member);
                case "IfcRadioActivityMeasure":
                    return new IfcRail.MeasureResource.IfcRadioActivityMeasure((MeasureResource.IfcRadioActivityMeasure)member);
                case "IfcRatioMeasure":
                    return new IfcRail.MeasureResource.IfcRatioMeasure((MeasureResource.IfcRatioMeasure)member);
                case "IfcReal":
                    return new IfcRail.MeasureResource.IfcReal((MeasureResource.IfcReal)member);
                case "IfcRotationalFrequencyMeasure":
                    return new IfcRail.MeasureResource.IfcRotationalFrequencyMeasure((MeasureResource.IfcRotationalFrequencyMeasure)member);
                case "IfcRotationalMassMeasure":
                    return new IfcRail.MeasureResource.IfcRotationalMassMeasure((MeasureResource.IfcRotationalMassMeasure)member);
                case "IfcRotationalStiffnessMeasure":
                    return new IfcRail.MeasureResource.IfcRotationalStiffnessMeasure((MeasureResource.IfcRotationalStiffnessMeasure)member);
                case "IfcSectionalAreaIntegralMeasure":
                    return new IfcRail.MeasureResource.IfcSectionalAreaIntegralMeasure((MeasureResource.IfcSectionalAreaIntegralMeasure)member);
                case "IfcSectionModulusMeasure":
                    return new IfcRail.MeasureResource.IfcSectionModulusMeasure((MeasureResource.IfcSectionModulusMeasure)member);
                case "IfcShearModulusMeasure":
                    return new IfcRail.MeasureResource.IfcShearModulusMeasure((MeasureResource.IfcShearModulusMeasure)member);
                case "IfcSolidAngleMeasure":
                    return new IfcRail.MeasureResource.IfcSolidAngleMeasure((MeasureResource.IfcSolidAngleMeasure)member);
                case "IfcSoundPowerLevelMeasure":
                    return new IfcRail.MeasureResource.IfcSoundPowerLevelMeasure((MeasureResource.IfcSoundPowerLevelMeasure)member);
                case "IfcSoundPowerMeasure":
                    return new IfcRail.MeasureResource.IfcSoundPowerMeasure((MeasureResource.IfcSoundPowerMeasure)member);
                case "IfcSoundPressureLevelMeasure":
                    return new IfcRail.MeasureResource.IfcSoundPressureLevelMeasure((MeasureResource.IfcSoundPressureLevelMeasure)member);
                case "IfcSoundPressureMeasure":
                    return new IfcRail.MeasureResource.IfcSoundPressureMeasure((MeasureResource.IfcSoundPressureMeasure)member);
                case "IfcSpecificHeatCapacityMeasure":
                    return new IfcRail.MeasureResource.IfcSpecificHeatCapacityMeasure((MeasureResource.IfcSpecificHeatCapacityMeasure)member);
                case "IfcTemperatureGradientMeasure":
                    return new IfcRail.MeasureResource.IfcTemperatureGradientMeasure((MeasureResource.IfcTemperatureGradientMeasure)member);
                case "IfcTemperatureRateOfChangeMeasure":
                    return new IfcRail.MeasureResource.IfcTemperatureRateOfChangeMeasure((MeasureResource.IfcTemperatureRateOfChangeMeasure)member);
                case "IfcText":
                    return new IfcRail.MeasureResource.IfcText((MeasureResource.IfcText)member);
                case "IfcThermalAdmittanceMeasure":
                    return new IfcRail.MeasureResource.IfcThermalAdmittanceMeasure((MeasureResource.IfcThermalAdmittanceMeasure)member);
                case "IfcThermalConductivityMeasure":
                    return new IfcRail.MeasureResource.IfcThermalConductivityMeasure((MeasureResource.IfcThermalConductivityMeasure)member);
                case "IfcThermalExpansionCoefficientMeasure":
                    return new IfcRail.MeasureResource.IfcThermalExpansionCoefficientMeasure((MeasureResource.IfcThermalExpansionCoefficientMeasure)member);
                case "IfcThermalResistanceMeasure":
                    return new IfcRail.MeasureResource.IfcThermalResistanceMeasure((MeasureResource.IfcThermalResistanceMeasure)member);
                case "IfcThermalTransmittanceMeasure":
                    return new IfcRail.MeasureResource.IfcThermalTransmittanceMeasure((MeasureResource.IfcThermalTransmittanceMeasure)member);
                case "IfcThermodynamicTemperatureMeasure":
                    return new IfcRail.MeasureResource.IfcThermodynamicTemperatureMeasure((MeasureResource.IfcThermodynamicTemperatureMeasure)member);
                case "IfcTime":
                    return new IfcRail.DateTimeResource.IfcTime((DateTimeResource.IfcTime)member);
                case "IfcTimeMeasure":
                    return new IfcRail.MeasureResource.IfcTimeMeasure((MeasureResource.IfcTimeMeasure)member);
                case "IfcTimeStamp":
                    return new IfcRail.DateTimeResource.IfcTimeStamp((DateTimeResource.IfcTimeStamp)member);
                case "IfcTorqueMeasure":
                    return new IfcRail.MeasureResource.IfcTorqueMeasure((MeasureResource.IfcTorqueMeasure)member);
                case "IfcVaporPermeabilityMeasure":
                    return new IfcRail.MeasureResource.IfcVaporPermeabilityMeasure((MeasureResource.IfcVaporPermeabilityMeasure)member);
                case "IfcVolumeMeasure":
                    return new IfcRail.MeasureResource.IfcVolumeMeasure((MeasureResource.IfcVolumeMeasure)member);
                case "IfcVolumetricFlowRateMeasure":
                    return new IfcRail.MeasureResource.IfcVolumetricFlowRateMeasure((MeasureResource.IfcVolumetricFlowRateMeasure)member);
                case "IfcWarpingConstantMeasure":
                    return new IfcRail.MeasureResource.IfcWarpingConstantMeasure((MeasureResource.IfcWarpingConstantMeasure)member);
                case "IfcWarpingMomentMeasure":
                    return new IfcRail.MeasureResource.IfcWarpingMomentMeasure((MeasureResource.IfcWarpingMomentMeasure)member);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}