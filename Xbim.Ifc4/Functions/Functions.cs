using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationAppearanceResource;
using Xbim.Ifc4.TopologyResource;

namespace Xbim.Ifc4.Functions
{
    internal class Function
    {
        // ReSharper disable once InconsistentNaming
        internal static T NVL<T>(T obj1, T obj2) where T : class
        {
            return obj1 ?? obj2;
        }

        // ReSharper disable once InconsistentNaming
        internal static bool EXISTS(object o)
        {
            return o != null;
        }

        internal bool HasIfcDimensionalExponents(IIfcDimensionalExponents dim, int len, int mass, int time, int elec, int temp, int substance, int lum)
        {
            return
                dim.LengthExponent == len
                && dim.MassExponent == mass
                && dim.TimeExponent == time
                && dim.ElectricCurrentExponent == elec
                && dim.ThermodynamicTemperatureExponent == temp
                && dim.AmountOfSubstanceExponent == substance
                && dim.LuminousIntensityExponent == lum;
        }

        internal int SIZEOF<T>(IEnumerable<T> source)
        {
            return source.Count();
        }

        internal bool INTYPEOF(object obj, string typeString)
        {
            throw new NotImplementedException();
        }

        internal double SQRT(double mag)
        {
            return Math.Sqrt(mag);
        }
        

        internal int LOINDEX<T>(IEnumerable<T> source)
        {
            return 0;
        }

        internal int HIINDEX<T>(IEnumerable<T> source)
        {
            return source.Count() - 1;
        }

        // ========================================== IfcBaseAxis
        List<Direction> IfcBaseAxis(int Dim, Direction Axis1, Direction Axis2, Direction Axis3)
        {
            // local variables
            List<Direction> U;
            double Factor;
            Direction D1;
            Direction D2;

            if ((Dim == 3))
            {
                D1 = NVL<Direction>(IfcNormalise(Axis3.ToVectorOrDirection()), New.Direction(0.0, 0.0, 1.0));
                D2 = IfcFirstProjAxis(D1, Axis1);
                U = new List<Direction>()
                {
                    D2,
                    IfcSecondProjAxis(D1, D2, Axis2),
                    D1
                };
            }
            else
            {
                if (EXISTS(Axis1))
                {
                    D1 = IfcNormalise(Axis1.ToVectorOrDirection());
                    U = new List<Direction>()
                    {
                        D1,
                        IfcOrthogonalComplement(D1)
                    };
                    if (EXISTS(Axis2))
                    {
                        Factor = (double)IfcDotProduct(Axis2, U[2]);
                        if (Factor < 0.0)
                        {
                            // index replaced
                            U[1].SetDirectionRatios(0, -U[1].GetDirectionRatios(0));
                            U[1].SetDirectionRatios(1, -U[1].GetDirectionRatios(1));
                        }
                    }
                }
                else
                {
                    if (EXISTS(Axis2))
                    {
                        D1 = IfcNormalise(Axis2.ToVectorOrDirection());
                        U = new List<Direction>()
                        {
                            IfcOrthogonalComplement(D1),
                            D1
                        };
                        U[0].SetDirectionRatios(0, -U[0].GetDirectionRatios(0));
                        U[0].SetDirectionRatios(1, -U[0].GetDirectionRatios(1));
                    }
                    else
                    {
                        U = new List<Direction>()
                        {
                            New.Direction(1.0, 0.0),
                            New.Direction(0.0, 1.0)
                        };
                    }
                }
            }
            return (U);
        }

        // ========================================== IfcBooleanChoose
        T IfcBooleanChoose<T>(bool B, T Choice1, T Choice2)
        {
            if (B)
            {
                return (Choice1);
            }
            else
            {
                return (Choice2);
            }
        }

        // ========================================== IfcBuild2Axes

        List<Direction> IfcBuild2Axes(Direction RefDirection)
        {
            // local variables
            var D = NVL(IfcNormalise(RefDirection.ToVectorOrDirection()), New.Direction(1.0, 0.0));
            return new List<Direction>()
            {
                D,
                IfcOrthogonalComplement(D)
            };
        }

        // ========================================== IfcBuildAxes

        List<Direction> IfcBuildAxes(Direction Axis, Direction RefDirection)
        {
            throw new NotImplementedException();

            // local variables
            Direction D1;
            Direction D2;

            // todo: restore the \ operation below.

            D1 = NVL((Direction)IfcNormalise(Axis), New.Direction(0.0, 0.0, 1.0));
            D2 = IfcFirstProjAxis(D1, RefDirection);
            return new List<Direction>()
            {
                D2,
                // IfcNormalise(IfcCrossProduct(D1, D2)) \ Vector.Orientation,
                D1
            };
        }

        // ========================================== IfcConstraintsParamBSpline

        bool IfcConstraintsParamBSpline(int Degree, int UpKnots, int UpCp, List<int> KnotMult,
            List<IfcParameterValue> Knots)
        {
            // local variables
            bool Result = true;
            int K;
            int Sum;



            Sum = KnotMult[1];
            for (var i = 2; i <= UpKnots; i++)
            {
                Sum = Sum + KnotMult[i];
            }


            if ((Degree < 1) || (UpKnots < 2) || (UpCp < Degree) ||
                (Sum != (Degree + UpCp + 2)))
            {
                Result = false;
                return (Result);
            }

            K = KnotMult[1];
            if ((K < 1) || (K > Degree + 1))
            {
                Result = false;
                return (Result);
            }

            for (var i = 2; i <= UpKnots; i++)
            {
                if ((KnotMult[i] < 1) || (Knots[i] <= Knots[i - 1]))
                {
                    Result = false;
                    return (Result);
                }
                K = KnotMult[i];
                if ((i < UpKnots) && (K > Degree))
                {
                    Result = false;
                    return (Result);
                }
                if ((i == UpKnots) && (K > Degree + 1))
                {
                    Result = false;
                    return (Result);
                }
            }

            return (Result);
        }

        // ========================================== IfcConvertDirectionInto2D

        Direction IfcConvertDirectionInto2D(Direction Direction)
        {
            // local variables
            var Direction2D = New.Direction(
                Direction.GetDirectionRatios(0),
                Direction.GetDirectionRatios(1)
                );
            return Direction2D;
        }

        // ========================================== IfcCorrectDimensions

        bool? IfcCorrectDimensions(IfcUnitEnum m, IfcDimensionalExponents Dim)
        {
            switch (m)
            {

                case IfcUnitEnum.LENGTHUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 1, 0, 0, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.MASSUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 1, 0, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.TIMEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 1, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ELECTRICCURRENTUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 1, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 0, 1, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.AMOUNTOFSUBSTANCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 0, 0, 1, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.LUMINOUSINTENSITYUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 0, 0, 0, 1))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.PLANEANGLEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.SOLIDANGLEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.AREAUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 0, 0, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.VOLUMEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 3, 0, 0, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ABSORBEDDOSEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 0, -2, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.RADIOACTIVITYUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, -1, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ELECTRICCAPACITANCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, -2, -1, 4, 2, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.DOSEEQUIVALENTUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 0, -2, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ELECTRICCHARGEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 1, 1, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ELECTRICCONDUCTANCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, -2, -1, 3, 2, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ELECTRICVOLTAGEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 1, -3, -1, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ELECTRICRESISTANCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 1, -3, -2, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ENERGYUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 1, -2, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.FORCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 1, 1, -2, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.FREQUENCYUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, -1, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.INDUCTANCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 1, -2, -2, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.ILLUMINANCEUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, -2, 0, 0, 0, 0, 0, 1))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.LUMINOUSFLUXUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 0, 0, 0, 0, 0, 1))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.MAGNETICFLUXUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 1, -2, -1, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.MAGNETICFLUXDENSITYUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 0, 1, -2, -1, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.POWERUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, 2, 1, -3, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                case IfcUnitEnum.PRESSUREUNIT:
                    if (
                        (HasIfcDimensionalExponents(Dim, -1, 1, -2, 0, 0, 0, 0))
                    )
                    {
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                default:
                    return null;
            }
        }

        // ========================================== IfcCorrectFillAreaStyle

        bool IfcCorrectFillAreaStyle(List<IfcFillStyleSelect> Styles)
        {
            // local variables
            int Hatching = 0;
            int Tiles = 0;
            int Colour = 0;
            int External = 0;
            External = SIZEOF(Styles.Where(Style => INTYPEOF(Style, "IFC4.IFCEXTERNALLYDEFINEDHATCHSTYLE")));
            Hatching = SIZEOF(Styles.Where(Style => INTYPEOF(Style, "IFC4.IFCFILLAREASTYLEHATCHING")));
            Tiles = SIZEOF(Styles.Where(Style => INTYPEOF(Style, "IFC4.IFCFILLAREASTYLETILES")));
            Colour = SIZEOF(Styles.Where(Style => INTYPEOF(Style, "IFC4.IFCCOLOUR")));
            if ((External > 1))
            {
                return (false);
            }
            if (((External == 1) && ((Hatching > 0) || (Tiles > 0) || (Colour > 0))))
            {
                return (false);
            }
            if ((Colour > 1))
            {
                return (false);
            }
            if (((Hatching > 0) && (Tiles > 0)))
            {
                return (false);
            }
            return (true);
        }

        // ========================================== IfcCorrectLocalPlacement

        bool? IfcCorrectLocalPlacement(IfcAxis2Placement AxisPlacement, IfcObjectPlacement RelPlacement)
        {

            if ((EXISTS(RelPlacement)))
            {
                if (((INTYPEOF(RelPlacement, "IFC4.IFCGRIDPLACEMENT"))))
                {
                    return null;
                }
                if (((INTYPEOF(RelPlacement, "IFC4.IFCLOCALPLACEMENT"))))
                {
                    if (((INTYPEOF(AxisPlacement, "IFC4.IFCAXIS2PLACEMENT2D"))))
                    {
                        return (true);
                    }
                    if (((INTYPEOF(AxisPlacement, "IFC4.IFCAXIS2PLACEMENT3D"))))
                    {
                        if (((RelPlacement as IfcLocalPlacement).RelativePlacement.Dim == 3))
                        {
                            return (true);
                        }
                        else
                        {
                            return (false);
                        }
                    }
                }
            }
            else
            {
                return (true);
            }
            return null;
        }

        // ========================================== IfcCorrectObjectAssignment

        bool? IfcCorrectObjectAssignment(IfcObjectTypeEnum Constraint, List<IfcObjectDefinition> Objects)
        {

            // local variables
            int Count = 0;


            if (!(EXISTS(Constraint)))
            {
                return (true);
            }

            switch (Constraint)
            {
                case IfcObjectTypeEnum.NOTDEFINED:
                    return (true);
                case IfcObjectTypeEnum.PRODUCT:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCPRODUCT")))));
                        return (Count == 0);
                    }
                case IfcObjectTypeEnum.PROCESS:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCPROCESS")))));
                        return (Count == 0);
                    }
                    ;
                case IfcObjectTypeEnum.CONTROL:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCCONTROL")))));
                        return (Count == 0);
                    }
                    ;
                case IfcObjectTypeEnum.RESOURCE:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCRESOURCE")))));
                        return (Count == 0);
                    }
                    ;
                case IfcObjectTypeEnum.ACTOR:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCACTOR")))));
                        return (Count == 0);
                    }
                    ;
                case IfcObjectTypeEnum.GROUP:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCGROUP")))));
                        return (Count == 0);
                    }
                    ;
                case IfcObjectTypeEnum.PROJECT:
                    {
                        Count = SIZEOF(Objects.Where(temp => !((INTYPEOF(temp, "IFC4.IFCPROJECT")))));
                        return (Count == 0);
                    }
                    ;
                default:
                    return null;

            }

        }

        // ========================================== IfcCorrectUnitAssignment

        bool IfcCorrectUnitAssignment(List<IfcUnit> Units)
        {
            // local variables
            int NamedUnitNumber = 0;
            int DerivedUnitNumber = 0;
            int MonetaryUnitNumber = 0;
            var NamedUnitNames = new List<IfcUnitEnum>();
            var DerivedUnitNames = new List<IfcDerivedUnitEnum>();


            NamedUnitNumber =
                SIZEOF(
                    Units.Where(
                        temp =>
                            ((INTYPEOF(temp, "IFC4.IFCNAMEDUNIT"))) &&
                            !((temp as IfcNamedUnit).UnitType == IfcUnitEnum.USERDEFINED)));
            DerivedUnitNumber =
                SIZEOF(
                    Units.Where(
                        temp =>
                            ((INTYPEOF(temp, "IFC4.IFCDERIVEDUNIT"))) &&
                            !((temp as IfcDerivedUnit).UnitType == IfcDerivedUnitEnum.USERDEFINED)));
            MonetaryUnitNumber = SIZEOF(Units.Where(temp => (INTYPEOF(temp, "IFC4.IFCMONETARYUNIT"))));

            for (var i = 1; i <= SIZEOF(Units); i++)
            {
                if ((((INTYPEOF(Units[i], "IFC4.IFCNAMEDUNIT"))) &&
                     !((Units[i] as IfcNamedUnit).UnitType == IfcUnitEnum.USERDEFINED)))
                {
                    NamedUnitNames.Add((Units[i] as IfcNamedUnit).UnitType);
                }
                if ((((INTYPEOF(Units[i], "IFC4.IFCDERIVEDUNIT"))) &&
                     !((Units[i] as IfcDerivedUnit).UnitType == IfcDerivedUnitEnum.USERDEFINED)))
                {
                    DerivedUnitNames.Add((Units[i] as IfcDerivedUnit).UnitType);
                }
            }

            return ((SIZEOF(NamedUnitNames) == NamedUnitNumber) && (SIZEOF(DerivedUnitNames) == DerivedUnitNumber) &&
                    (MonetaryUnitNumber <= 1));
        }

        // ========================================== IfcCrossProduct

        Vector IfcCrossProduct(Direction Arg1, Direction Arg2)
        {
            // local variables
            double Mag;
            Direction Res;
            List<double> V1;
            List<double> V2;
            Vector Result;


            if ((!EXISTS(Arg1) || (Arg1.Dim == 2)) || (!EXISTS(Arg2) || (Arg2.Dim == 2)))
            {
                return null;
            }
            else
            {
                V1 = IfcNormalise(Arg1.ToVectorOrDirection()).ToDirection().GetDirectionRatios();
                V2 = IfcNormalise(Arg2.ToVectorOrDirection()).ToDirection().GetDirectionRatios();
                Res = New.Direction(
                    (V1[2] * V2[3] - V1[3] * V2[2]),
                    (V1[3] * V2[1] - V1[1] * V2[3]),
                    (V1[1] * V2[2] - V1[2] * V2[1])
                );
                Mag = 0.0;
                for (var i = 1; i <= 3; i++)
                {
                    Mag = Mag + Res.GetDirectionRatios(i-1) * Res.GetDirectionRatios(i);
                }
                if ((Mag > 0.0))
                {
                    Result = New.Vector(Res, SQRT(Mag));
                }
                else
                {
                    Result = New.Vector(Arg1, 0.0);
                }
                return (Result);
            }
        }



        // ========================================== IfcCurveDim

        IfcDimensionCount? IfcCurveDim(IfcCurve Curve)
        {
            if (((INTYPEOF(Curve, "IFC4.IFCLINE"))))
            {
                return ((Curve as IfcLine).Pnt.Dim);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCCONIC"))))
            {
                return ((Curve as IfcConic).Position.Dim);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCPOLYLINE"))))
            {
                return ((Curve as IfcPolyline).Points[1].Dim);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCTRIMMEDCURVE"))))
            {
                return (IfcCurveDim((Curve as IfcTrimmedCurve).BasisCurve));
            }
            if (((INTYPEOF(Curve, "IFC4.IFCCOMPOSITECURVE"))))
            {
                return ((Curve as IfcCompositeCurve).Segments[1].Dim);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCBSPLINECURVE"))))
            {
                return ((Curve as IfcBSplineCurve).ControlPointsList[1].Dim);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCOFFSETCURVE2D"))))
            {
                return (2);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCOFFSETCURVE3D"))))
            {
                return (3);
            }
            if (((INTYPEOF(Curve, "IFC4.IFCPCURVE"))))
            {
                return (3);
            }
            return (null);
        }

        // ========================================== IfcCurveWeightsPositive

        bool IfcCurveWeightsPositive(IfcRationalBSplineCurveWithKnots B)
        {

            // local variables
            bool Result = true;


            for (var i = 0; i <= B.UpperIndexOnControlPoints; i++)
            {
                if (B.Weights[i] <= 0.0)
                {
                    Result = false;
                    return (Result);
                }
            }
            return (Result);
        }

        // ========================================== IfcDeriveDimensionalExponents

        DimensionalExponents IfcDeriveDimensionalExponents(List<IfcDerivedUnitElement> UnitElements)
        {
            // local variables
            var Result = New.DimensionalExponents(0, 0, 0, 0, 0, 0, 0);

            for (var i = LOINDEX(UnitElements); i <= HIINDEX(UnitElements); i++)
            {
                Result.LengthExponent = Result.LengthExponent +
                                        (UnitElements[i].Exponent *
                                         UnitElements[i].Unit.Dimensions.LengthExponent);
                Result.MassExponent = Result.MassExponent +
                                      (UnitElements[i].Exponent *
                                       UnitElements[i].Unit.Dimensions.MassExponent);
                Result.TimeExponent = Result.TimeExponent +
                                      (UnitElements[i].Exponent *
                                       UnitElements[i].Unit.Dimensions.TimeExponent);
                Result.ElectricCurrentExponent = Result.ElectricCurrentExponent +
                                                 (UnitElements[i].Exponent *
                                                  UnitElements[i].Unit.Dimensions.ElectricCurrentExponent);
                Result.ThermodynamicTemperatureExponent = Result.ThermodynamicTemperatureExponent +
                                                          (UnitElements[i].Exponent *
                                                           UnitElements[i].Unit.Dimensions
                                                               .ThermodynamicTemperatureExponent);
                Result.AmountOfSubstanceExponent = Result.AmountOfSubstanceExponent +
                                                   (UnitElements[i].Exponent *
                                                    UnitElements[i].Unit.Dimensions.AmountOfSubstanceExponent);
                Result.LuminousIntensityExponent = Result.LuminousIntensityExponent +
                                                   (UnitElements[i].Exponent *
                                                    UnitElements[i].Unit.Dimensions.LuminousIntensityExponent);
            }
            return (Result);
        }

        // ========================================== IfcDimensionsForSiUnit

        DimensionalExponents IfcDimensionsForSiUnit(IfcSIUnitName n)
        {
            switch (n)
            {
                case IfcSIUnitName.METRE:
                    return (New.DimensionalExponents
                        (1, 0, 0, 0, 0, 0, 0));
                case IfcSIUnitName.SQUARE_METRE:
                    return (New.DimensionalExponents
                        (2, 0, 0, 0, 0, 0, 0));
                case IfcSIUnitName.CUBIC_METRE:
                    return (New.DimensionalExponents
                        (3, 0, 0, 0, 0, 0, 0));
                case IfcSIUnitName.GRAM:
                    return (New.DimensionalExponents
                        (0, 1, 0, 0, 0, 0, 0));
                case IfcSIUnitName.SECOND:
                    return (New.DimensionalExponents
                        (0, 0, 1, 0, 0, 0, 0));
                case IfcSIUnitName.AMPERE:
                    return (New.DimensionalExponents
                        (0, 0, 0, 1, 0, 0, 0));
                case IfcSIUnitName.KELVIN:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 1, 0, 0));
                case IfcSIUnitName.MOLE:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 0, 1, 0));
                case IfcSIUnitName.CANDELA:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 0, 0, 1));
                case IfcSIUnitName.RADIAN:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 0, 0, 0));
                case IfcSIUnitName.STERADIAN:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 0, 0, 0));
                case IfcSIUnitName.HERTZ:
                    return (New.DimensionalExponents
                        (0, 0, -1, 0, 0, 0, 0));
                case IfcSIUnitName.NEWTON:
                    return (New.DimensionalExponents
                        (1, 1, -2, 0, 0, 0, 0));
                case IfcSIUnitName.PASCAL:
                    return (New.DimensionalExponents
                        (-1, 1, -2, 0, 0, 0, 0));
                case IfcSIUnitName.JOULE:
                    return (New.DimensionalExponents
                        (2, 1, -2, 0, 0, 0, 0));
                case IfcSIUnitName.WATT:
                    return (New.DimensionalExponents
                        (2, 1, -3, 0, 0, 0, 0));
                case IfcSIUnitName.COULOMB:
                    return (New.DimensionalExponents
                        (0, 0, 1, 1, 0, 0, 0));
                case IfcSIUnitName.VOLT:
                    return (New.DimensionalExponents
                        (2, 1, -3, -1, 0, 0, 0));
                case IfcSIUnitName.FARAD:
                    return (New.DimensionalExponents
                        (-2, -1, 4, 2, 0, 0, 0));
                case IfcSIUnitName.OHM:
                    return (New.DimensionalExponents
                        (2, 1, -3, -2, 0, 0, 0));
                case IfcSIUnitName.SIEMENS:
                    return (New.DimensionalExponents
                        (-2, -1, 3, 2, 0, 0, 0));
                case IfcSIUnitName.WEBER:
                    return (New.DimensionalExponents
                        (2, 1, -2, -1, 0, 0, 0));
                case IfcSIUnitName.TESLA:
                    return (New.DimensionalExponents
                        (0, 1, -2, -1, 0, 0, 0));
                case IfcSIUnitName.HENRY:
                    return (New.DimensionalExponents
                        (2, 1, -2, -2, 0, 0, 0));
                case IfcSIUnitName.DEGREE_CELSIUS:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 1, 0, 0));
                case IfcSIUnitName.LUMEN:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 0, 0, 1));
                case IfcSIUnitName.LUX:
                    return (New.DimensionalExponents
                        (-2, 0, 0, 0, 0, 0, 1));
                case IfcSIUnitName.BECQUEREL:
                    return (New.DimensionalExponents
                        (0, 0, -1, 0, 0, 0, 0));
                case IfcSIUnitName.GRAY:
                    return (New.DimensionalExponents
                        (2, 0, -2, 0, 0, 0, 0));
                case IfcSIUnitName.SIEVERT:
                    return (New.DimensionalExponents
                        (2, 0, -2, 0, 0, 0, 0));
                default:
                    return (New.DimensionalExponents
                        (0, 0, 0, 0, 0, 0, 0));
            }
        }

        // ========================================== IfcDotProduct

        double? IfcDotProduct(Direction Arg1, Direction Arg2)
        {
            // local variables
            double? Scalar;
            Direction Vec1;
            Direction Vec2;
            int Ndim;


            if (!EXISTS(Arg1) || !EXISTS(Arg2))
            {
                Scalar = null;
            }
            else
            {
                if ((Arg1.Dim != Arg2.Dim))
                {
                    Scalar = null;
                }
                else
                {
                    Vec1 = IfcNormalise(Arg1.ToVectorOrDirection());
                    Vec2 = IfcNormalise(Arg2.ToVectorOrDirection());
                    Ndim = (int) Arg1.Dim;
                    Scalar = 0.0;
                    for (var i = 1; i <= Ndim; i++)
                    {
                        Scalar = Scalar + Vec1.GetDirectionRatios(i - 1)*Vec2.GetDirectionRatios(i - 1);
                    }
                }
            }
            return (Scalar);
        }

        // ========================================== IfcFirstProjAxis

        Direction IfcFirstProjAxis(Direction ZAxis, Direction Arg)
        {
            // local variables
            Direction XAxis;
            Direction V;
            Direction Z;
            Vector XVec;


            if ((!EXISTS(ZAxis)))
            {
                return (null);
            }
            else
            {
                Z = IfcNormalise(ZAxis.ToVectorOrDirection());
                if (!EXISTS(Arg))
                {
                    if (
                        // index ok
                        Math.Abs(Z.GetDirectionRatios(0) - 1.0) > double.Epsilon ||
                        Math.Abs(Z.GetDirectionRatios(1)) > double.Epsilon ||
                        Math.Abs(Z.GetDirectionRatios(2)) > double.Epsilon)
                    {
                        V = New.Direction(1.0, 0.0, 0.0);
                    }
                    else
                    {
                        V = New.Direction(0.0, 1.0, 0.0);
                    }
                }
                else
                {
                    if ((Arg.Dim != 3))
                    {
                        return (null);
                    }
                    if (Math.Abs((IfcCrossProduct(Arg, Z).Magnitude)) < double.Epsilon)
                    {
                        return (null);
                    }
                    else
                    {
                        V = IfcNormalise(Arg.ToVectorOrDirection());
                    }
                }
                XVec = IfcScalarTimesVector((double)IfcDotProduct(V, Z), Z.ToVectorOrDirection());
                XAxis = VectorDifference(V, XVec).Orientation;
                XAxis = IfcNormalise(XAxis.ToVectorOrDirection());
            }
            return (XAxis);
        }

        internal Vector VectorDifference(Direction v, Vector xVec)
        {
            // todo: remove, this should be available below
            return null;
        }

        // ========================================== IfcGetBasisSurface

        List<IfcSurface> IfcGetBasisSurface(IfcCurveOnSurface C)
        {
            // local variables
            List<IfcSurface> Surfs;
            int N;

            Surfs = new List<IfcSurface>();
            if (INTYPEOF(C, "IFC4.IFCPCURVE"))
            {
                Surfs = new List<IfcSurface>() { (C as IfcPcurve).BasisSurface };
            }
            else
            {
                if (INTYPEOF(C, "IFC4.IFCCOMPOSITECURVEONSURFACE"))
                {

                    N = SIZEOF((C as IfcCompositeCurve).Segments);
                    Surfs = IfcGetBasisSurface((IfcCurveOnSurface)(C as IfcCompositeCurve).Segments[1].ParentCurve);

                    if (N > 1)
                    {
                        for (var i = 2; i <= N; i++)
                        {
                            throw new NotImplementedException();
                            // todo: restore
                            // Surfs = Surfs*IfcGetBasisSurface((C as IfcCompositeCurve).Segments[1].ParentCurve);
                        }
                    }
                }
            }
            return (Surfs);
        }

        // ========================================== IfcListToArray
        T[] IfcListToArray<T>(List<T> Lis, int Low, int U)
        {
            // local variables
            int N;
            T[] Res;

            N = SIZEOF(Lis);
            if ((N != (U - Low + 1)))
            {
                return null;
            }
            else
            {
                // todo: use CopyTo
                throw new NotImplementedException();
                //Res = [Lis[1] : N];
                //for (var i = 2; i <= N; i++)
                //{
                //    Res[Low + i - 1] = Lis[i];
                //}
                //return (Res);
                return null;
            }
        }

        // ========================================== IfcLoopHeadToTail

        bool IfcLoopHeadToTail(IfcEdgeLoop ALoop)
        {
            // local variables
            int N;
            bool P = true;


            N = SIZEOF(ALoop.EdgeList);
            for (var i = 2; i <= N; i++)
            {
                P = P && (ALoop.EdgeList[i - 1].EdgeEnd == ALoop.EdgeList[i].EdgeStart);
            }
            return (P);
        }

        // ========================================== IfcMakeArrayOfArray
        // todo: restore
        // ARRAY[Low1:U1] OF ARRAY[Low2:U2] OF GENERIC : T IfcMakeArrayOfArray(List<LIST[1:?] OF GENERIC : T> Lis, int Low1, int U1, int Low2, int U2)
        //{
        //    // local variables
        //    ARRAY[Low1: U1] OF ARRAY[Low2:U2] OF GENERIC : T Res;
        //    if ((U1 - Low1 + 1) != SIZEOF(Lis))
        //    {
        //        return (null);
        //    }
        //    if ((U2 - Low2 + 1) != SIZEOF(Lis[1]))
        //    {
        //        return (null);
        //    }
        //    Res = [IfcListToArray(Lis[1], Low2, U2) : (U1 - Low1 + 1)];
        //    for (var i = 2; i <= HIINDEX(Lis); i++)
        //    {
        //        if ((U2 - Low2 + 1) != SIZEOF(Lis[i]))
        //        {
        //            return (null);
        //        }
        //        Res[Low1 + i - 1] = IfcListToArray(Lis[i], Low2, U2);
        //    }
        //    return (Res);
        //}

        // ========================================== IfcMlsTotalThickness

        IfcLengthMeasure IfcMlsTotalThickness(IfcMaterialLayerSet LayerSet)
        {
            // local variables
            IfcLengthMeasure Max = new IfcLengthMeasure(LayerSet.MaterialLayers[1].LayerThickness);

            if (SIZEOF(LayerSet.MaterialLayers) > 1)
            {
                for (var i = 2; i <= HIINDEX(LayerSet.MaterialLayers); i++)
                {
                    Max = Max + LayerSet.MaterialLayers[i].LayerThickness;
                }
            }
            return (Max);
        }

        // ========================================== IfcNormalise

        VectorOrDirection IfcNormalise(VectorOrDirection Arg)
        {
            // local variables
            int Ndim;
            Direction V = New.Direction(1.0, 0.0);
            Vector Vec = New.Vector(New.Direction(1.0, 0.0), 1.0);
            double Mag;
            VectorOrDirection Result = New.VectorOrDirection(V);
            
            if (!EXISTS(Arg))
            {
                return (null);
            }
            else
            {
                if (INTYPEOF(Arg, "IFC4.Vector"))
                {
                    Ndim = Arg.Dim;
                    V.DirectionRatios = Arg.ToVector().Orientation.DirectionRatios;
                    Vec.Magnitude = (Arg as Vector).Magnitude;
                    Vec.Orientation = V;
                    if ((Arg as Vector).Magnitude == 0.0)
                    {
                        return null;
                    }
                    else
                    {
                        Vec.Magnitude = 1.0;
                    }
                }
                else
                {
                    {
                        Ndim = (int)(Arg as Direction).Dim;
                        throw new NotImplementedException();
                        // todo: restore
                        // V.DirectionRatios = (Arg as Direction).DirectionRatios;
                    }
                    ;
                }

                Mag = 0.0;
                for (var i = 1; i <= Ndim; i++)
                {
                    Mag = Mag + V.GetDirectionRatios(i-1) * V.GetDirectionRatios(i-1);
                }
                if (Mag > 0.0)
                {
                    Mag = SQRT(Mag);
                    for (var i = 1; i <= Ndim; i++)
                    {
                        V.DirectionRatios[i] = V.DirectionRatios[i] / Mag;
                    }
                    if ((INTYPEOF(Arg, "IFC4.Vector")))
                    {
                        Vec.Orientation = V;
                        Result = Vec;
                    }
                    else
                    {
                        Result = V;
                    }
                }
                else
                {
                    return null;
                }
            }
            return (Result);
        }

        // ========================================== IfcOrthogonalComplement

        Direction IfcOrthogonalComplement(Direction Vec)
        {
            // local variables
            Direction Result;

            if (!EXISTS(Vec) || (Vec.Dim != 2))
            {
                return null;
            }
            else
            {
                Result = New.Direction(-Vec.GetDirectionRatios(1), Vec.GetDirectionRatios(0));
                return (Result);
            }
        }

        // ========================================== IfcPathHeadToTail

        bool IfcPathHeadToTail(IfcPath APath)
        {
            // local variables
            int N = 0;
            // todo: restore; how does the UNKNOWN keywork behave?
            throw new NotImplementedException();
            // bool P = UNKNOWN;
            bool P;

            N = SIZEOF(APath.EdgeList);
            for (var i = 2; i <= N; i++)
            {
                P = P && (APath.EdgeList[i - 1].EdgeEnd == APath.EdgeList[i].EdgeStart);
            }
            return (P);
        }



        // ========================================== IfcSameAxis2Placement

        bool IfcSameAxis2Placement(IfcAxis2Placement ap1, IfcAxis2Placement ap2, double Epsilon)
        {

            return (IfcSameDirection(ap1.P[1], ap2.P[1], Epsilon) &&
                    IfcSameDirection(ap1.P[2], ap2.P[2], Epsilon) &&
                    // todo: there's a bug in the equivalent express; notify BuildingSMART
                    IfcSameCartesianPoint((ap1 as IfcPlacement).Location, (ap2 as IfcPlacement).Location, Epsilon));
        }

        // ========================================== IfcSameCartesianPoint

        bool IfcSameCartesianPoint(IfcCartesianPoint cp1, IfcCartesianPoint cp2, double Epsilon)
        {

            // local variables
            double cp1x = cp1.Coordinates[1];
            double cp1y = cp1.Coordinates[2];
            double cp1z = 0;
            double cp2x = cp2.Coordinates[1];
            double cp2y = cp2.Coordinates[2];
            double cp2z = 0;


            if ((SIZEOF(cp1.Coordinates) > 2))
            {
                cp1z = cp1.Coordinates[3];
            }

            if ((SIZEOF(cp2.Coordinates) > 2))
            {
                cp2z = cp2.Coordinates[3];
            }

            return (IfcSameValue(cp1x, cp2x, Epsilon) &&
                    IfcSameValue(cp1y, cp2y, Epsilon) &&
                    IfcSameValue(cp1z, cp2z, Epsilon));
        }

        // ========================================== IfcSameDirection

        bool IfcSameDirection(Direction dir1, Direction dir2, double Epsilon)
        {
            // local variables
            double dir1x = dir1.DirectionRatios[1];
            double dir1y = dir1.DirectionRatios[2];
            double dir1z = 0;
            double dir2x = dir2.DirectionRatios[1];
            double dir2y = dir2.DirectionRatios[2];
            double dir2z = 0;


            if ((SIZEOF(dir1.DirectionRatios) > 2))
            {
                dir1z = dir1.DirectionRatios[3];
            }

            if ((SIZEOF(dir2.DirectionRatios) > 2))
            {
                dir2z = dir2.DirectionRatios[3];
            }

            return (IfcSameValue(dir1x, dir2x, Epsilon) &&
                    IfcSameValue(dir1y, dir2y, Epsilon) &&
                    IfcSameValue(dir1z, dir2z, Epsilon));
        }

        // ========================================== IfcSameValidPrecision

        bool IfcSameValidPrecision(double Epsilon1, double Epsilon2)
        {
            // local variables
            double ValidEps1;
            double ValidEps2;
            double DefaultEps = 0.000001;
            double DerivationOfEps = 1.001;
            double UpperEps = 1.0;

            // todo: bacause double is non nullable NVL is redundant
            // check all usages

            ValidEps1 = Epsilon1;
            ValidEps2 = Epsilon2;

            // todo: restore
            //ValidEps1 = NVL(Epsilon1, DefaultEps);
            //ValidEps2 = NVL(Epsilon2, DefaultEps);
            return ((0.0 < ValidEps1) && (ValidEps1 <= (DerivationOfEps * ValidEps2)) &&
                    (ValidEps2 <= (DerivationOfEps * ValidEps1)) && (ValidEps2 < UpperEps));
        }

        // ========================================== IfcSameValue

        bool IfcSameValue(double Value1, double Value2, double Epsilon)
        {
            // local variables
            double ValidEps;
            double DefaultEps = 0.000001;

            // todo: bacause double is non nullable NVL is redundant
            // check all usages
            throw new NotImplementedException();
            // ValidEps = NVL(Epsilon, DefaultEps);
            ValidEps = Epsilon;

            return ((Value1 + ValidEps > Value2) && (Value1 < Value2 + ValidEps));
        }

        // ========================================== IfcScalarTimesVector

        Vector IfcScalarTimesVector(double Scalar, VectorOrDirection Vec)
        {
            return IfcScalarTimesVector(Scalar, Vec.ToVector());
        }

        Vector IfcScalarTimesVector(double Scalar, Vector Vec)
        {
            // local variables
            Direction V;
            double Mag;
            Vector Result;


            if (!EXISTS(Scalar) || !EXISTS(Vec))
            {
                return (null);
            }
            else
            {
                if (INTYPEOF(Vec, "IFC4.Vector"))
                {
                    V = (Vec as Vector).Orientation;
                    Mag = Scalar * (Vec as Vector).Magnitude;
                }
                else
                {
                    V = (Direction)Vec;
                    Mag = Scalar;
                }
                if ((Mag < 0.0))
                {
                    for (var i = 1; i <= SIZEOF(V.DirectionRatios); i++)
                    {
                        V.DirectionRatios[i] = -V.DirectionRatios[i];
                    }
                    Mag = -Mag;
                }
                Result = New.Vector(IfcNormalise(V), Mag);
            }
            return (Result);
        }

        // ========================================== IfcSecondProjAxis
        Direction IfcSecondProjAxis(Direction ZAxis, Direction XAxis, Direction Arg)
        {
            // local variables
            Vector YAxis;
            Direction V;
            Vector Temp;

            if (!EXISTS(Arg))
            {
                V = New.Direction(0.0, 1.0, 0.0);
            }
            else
            {
                V = Arg;
            }
            Temp = IfcScalarTimesVector((double)IfcDotProduct(V, ZAxis), ZAxis);
            YAxis = VectorDifference(V, Temp);
            Temp = IfcScalarTimesVector((double)IfcDotProduct(V, XAxis), XAxis);

            //todo: whats the right function signature?
            throw new NotImplementedException();
            // todo: restore lines
            // YAxis = VectorDifference(YAxis, Temp);
            YAxis = (Vector)IfcNormalise(YAxis);
            return (YAxis.Orientation);
        }

        // ========================================== IfcShapeRepresentationTypes


        bool IfcShapeRepresentationTypes(IfcLabel RepType, List<IfcRepresentationItem> Items)
        {
            // local variables
            int Count = 0;
            switch (RepType)
            {
                case "Point":
                    Count = Items.Count(x => x is IIfcPoint);
                    break;
                case "PointCloud":
                    Count = Items.Count(x => x is IIfcCartesianPointList3D);
                    break;
                case "Curve":
                    Count = Items.Count(x => x is IIfcCurve);
                    break;
                case "Curve2D":
                    Count = Items.Count(x => x is IIfcCurve && ((IIfcCurve)x).Dim == 2);
                    break;
                case "Curve3D":
                    Count = Items.Count(x => x is IIfcCurve && ((IIfcCurve)x).Dim == 3);
                    break;

                case "Surface":
                    Count = Items.Count(x => x is IIfcSurface);
                    break;

                case "Surface2D":
                    Count = Items.Count(x => x is IIfcSurface && ((IIfcSurface)x).Dim == 2);
                    break;


                case "Surface3D":
                    Count = Items.Count(x => x is IIfcSurface && ((IIfcSurface)x).Dim == 3);
                    break;

                case "FillArea":
                    Count = Items.Count(x => x is IIfcAnnotationFillArea);
                    break;

                case "Text":
                    Count = Items.Count(x => x is IIfcTextLiteral);
                    break;
                case "AdvancedSurface":
                    Count = Items.Count(x => x is IIfcBSplineSurface);
                    break;

                case "Annotation2D":
                    Count = Items.Count(x =>
                            x is IIfcPoint
                            || x is IIfcCurve
                            || x is IIfcGeometricCurveSet
                            || x is IIfcAnnotationFillArea
                            || x is IIfcTextLiteral
                    );
                    break;


                case "GeometricSet":
                    Count = Items.Count(x =>
                        x is IIfcGeometricSet
                        || x is IIfcPoint
                        || x is IIfcCurve
                        || x is IIfcSurface);
                    break;
                case "GeometricCurveSet":
                    Count = Items.Count(x =>
                        x is IIfcGeometricCurveSet
                        || x is IIfcGeometricSet
                        || x is IIfcPoint
                        || x is IIfcCurve);
                    foreach (var ifcRepresentationItem in Items)
                    {
                        if (ifcRepresentationItem is IIfcGeometricSet)
                        {
                            var asIIfcGeometricSet = ifcRepresentationItem as IIfcGeometricSet;
                            if (asIIfcGeometricSet.Elements.Count(temp => temp is IfcSurface) > 0)
                            {
                                Count--;
                            }
                        }
                    }
                    break;

                case "Tessellation":
                    Count = Items.Count(x => x is IIfcTessellatedItem);
                    break;
                case "SurfaceOrSolidModel":
                    Count = Items.Count(x =>
                            x is IIfcTessellatedItem
                            || x is IIfcShellBasedSurfaceModel
                            || x is IIfcFaceBasedSurfaceModel
                            || x is IIfcSolidModel
                    );
                    break;
                case "SurfaceModel":
                    Count = Items.Count(x =>
                            x is IIfcTessellatedItem
                            || x is IIfcShellBasedSurfaceModel
                            || x is IIfcFaceBasedSurfaceModel
                    );
                    break;
                case "SolidModel":
                    Count = Items.Count(x => x is IIfcSolidModel);
                    break;
                case "SweptSolid":
                    // todo: check this, i'm not sure this is how to interpret the clause
                    Count = Items.Count(x =>
                            (x is IIfcExtrudedAreaSolid
                             || x is IIfcRevolvedAreaSolid)
                            && !(
                                x is IIfcExtrudedAreaSolidTapered
                                || x is IIfcRevolvedAreaSolidTapered)
                    );
                    break;
                case "AdvancedSweptSolid":
                    Count = Items.Count(x =>
                            x is IIfcSweptAreaSolid
                            || x is IIfcSweptDiskSolid
                    );
                    break;
                case "CSG":
                    Count = Items.Count(x =>
                        x is IIfcBooleanResult
                        || x is IIfcCsgPrimitive3D
                        || x is IIfcCsgSolid);
                    break;
                case "Clipping":
                    Count = Items.Count(x => x is IIfcBooleanClippingResult);
                    break;

                case "Brep":

                    Count = Items.Count(x => x is IIfcFacetedBrep);
                    break;


                case "AdvancedBrep":
                    Count = Items.Count(x => x is IIfcManifoldSolidBrep);
                    break;

                case "BoundingBox":
                    Count = Items.Count(x => x is IIfcBoundingBox);
                    if (Items.Count > 1)
                        Count = 0;
                    break;
                case "SectionedSpine":
                    Count = Items.Count(x => x is IIfcSectionedSpine);
                    break;
                case "LightSource":
                    Count = Items.Count(x => x is IIfcLightSource);
                    break;
                case "MappedRepresentation":
                    Count = Items.Count(x => x is IIfcMappedItem);
                    break;
            }
            return (Count == Items.Count);
        }

        // ========================================== IfcSurfaceWeightsPositive

        bool IfcSurfaceWeightsPositive(IIfcRationalBSplineSurfaceWithKnots B)
        {
            // local variables
            bool Result = true;


            for (var i = 0; i <= (B as IIfcBSplineSurface).UUpper; i++)
            {
                for (var j = 0; j <= (B as IIfcBSplineSurface).VUpper; j++)
                {
                    if ((B.Weights[i][j] <= 0.0))
                    {
                        Result = false;
                        return (Result);
                    }
                }
            }
            return (Result);
        }

        // ========================================== IfcTaperedSweptAreaProfiles

        bool IfcTaperedSweptAreaProfiles(IIfcProfileDef StartArea, IIfcProfileDef EndArea)
        {

            // local variables
            bool Result = false;


            if (INTYPEOF(StartArea, "IFC4.IFCPARAMETERIZEDPROFILEDEF"))
            {
                if (INTYPEOF(EndArea, "IFC4.IFCDERIVEDPROFILEDEF"))
                {
                    var end = EndArea as IIfcDerivedProfileDef;
                    Result = StartArea == end?.ParentProfile;
                }
                else
                {
                    Result = (StartArea.GetType() == EndArea.GetType());
                }
            }
            else
            {
                if (INTYPEOF(EndArea, "IFC4.IFCDERIVEDPROFILEDEF"))
                {
                    var end = EndArea as IIfcDerivedProfileDef;
                    Result = StartArea == end?.ParentProfile;
                }
                else
                {
                    Result = false;
                }
            }
            return (Result);
        }

        // ========================================== IfcTopologyRepresentationTypes

        bool IfcTopologyRepresentationTypes(IfcLabel RepType, List<IfcRepresentationItem> Items)
        {

            // local variables
            int Count = 0;

            switch (RepType)
            {

                case "Vertex":
                    Count = Items.Count(x => x is IIfcVertex);
                    break;

                case "Edge":
                    Count = Items.Count(x => x is IIfcEdge);
                    break;
                case "Path":
                    Count = Items.Count(x => x is IIfcPath);
                    break;

                case "Face":
                    Count = Items.Count(x => x is IIfcFace);
                    break;

                case "Shell":
                    Count = Items.Count(x => x is IIfcOpenShell
                                             || x is IIfcClosedShell
                    );
                    break;
                case "Undefined":
                    return (true);
            }

            return Count == Items.Count;
        }

        // ========================================== IfcUniquePropertyName

        bool IfcUniquePropertyName(List<IIfcProperty> Properties)
        {
            // note: this function has been substantially rewritten to take advangate of linq.
            //
            var propNames = Properties.Select(x => x.Name).ToList();
            var isUnique = propNames.Distinct().Count() == Properties.Count();
            return isUnique;
        }

        // ========================================== IfcUniquePropertyTemplateNames

        bool IfcUniquePropertyTemplateNames(List<IIfcPropertyTemplate> Properties)
        {

            // note: this function has been substantially rewritten to take advangate of linq.
            //
            var propNames = Properties.Select(x => x.Name).ToList();
            var isUnique = propNames.Distinct().Count() == Properties.Count();
            return isUnique;

        }

        // ========================================== IfcUniqueQuantityNames

        bool IfcUniqueQuantityNames(List<IIfcPhysicalQuantity> Properties)
        {

            // note: this function has been substantially rewritten to take advangate of linq.
            //
            var propNames = Properties.Select(x => x.Name).ToList();
            var isUnique = propNames.Distinct().Count() == Properties.Count();
            return isUnique;
        }

        // ========================================== VectorDifference

        Vector VectorDifference(VectorOrDirection Arg1, VectorOrDirection Arg2)
        {
            // local variables
            Vector Result;
            Direction Res;
            Direction Vec1;
            Direction Vec2;
            double Mag;
            double Mag1;
            double Mag2;
            int Ndim;

            if (!EXISTS(Arg1) || !EXISTS(Arg2) || (Arg1.Dim() != Arg2.Dim()))
            {
                return (null);
            }
            else
            {
                {
                    if (INTYPEOF(Arg1, "IFC4.Vector"))
                    {
                        Mag1 = (Arg1 as Vector).Magnitude;
                        Vec1 = (Arg1 as Vector).Orientation;
                    }
                    else
                    {
                        Mag1 = 1.0;
                        Vec1 = (Direction)Arg1;
                    }
                    if ((INTYPEOF(Arg2, "IFC4.Vector")))
                    {
                        Mag2 = (Arg2 as Vector).Magnitude;
                        Vec2 = (Arg2 as Vector).Orientation;
                    }
                    else
                    {
                        Mag2 = 1.0;
                        Vec2 = (Direction)Arg2;
                    }
                    Vec1 = (Direction)IfcNormalise(Vec1);
                    Vec2 = (Direction)IfcNormalise(Vec2);
                    Ndim = SIZEOF(Vec1.DirectionRatios);
                    Mag = 0.0;
                    // todo: does this mean that it needs to repeat 0.0 for Ndim times?
                    throw new NotImplementedException();
                    // Res =  Direction([0.0: Ndim]);

                    for (var i = 1; i <= Ndim; i++)
                    {
                        Res.DirectionRatios[i] = Mag1 * Vec1.DirectionRatios[i] - Mag2 * Vec2.DirectionRatios[i];
                        Mag = Mag + (Res.DirectionRatios[i] * Res.DirectionRatios[i]);
                    }
                    if ((Mag > 0.0))
                    {
                        Result = New.Vector(Res, SQRT(Mag));
                    }
                    else
                    {
                        Result = New.Vector(Vec1, 0.0);
                    }
                }

            }
            return (Result);
        }

        // ========================================== VectorSum

        Vector VectorSum(VectorOrDirection Arg1, VectorOrDirection Arg2)
        {
            // local variables
            Vector Result;
            Direction Res;
            Direction Vec1;
            Direction Vec2;
            double Mag;
            double Mag1;
            double Mag2;
            int Ndim;


            if (((!EXISTS(Arg1)) || (!EXISTS(Arg2))) || (Arg1.Dim() != Arg2.Dim()))
            {
                return (null);
            }
            else
            {
                {
                    if ((INTYPEOF(Arg1, "IFC4.Vector")))
                    {
                        Mag1 = (Arg1 as Vector).Magnitude;
                        Vec1 = (Arg1 as Vector).Orientation;
                    }
                    else
                    {
                        Mag1 = 1.0;
                        Vec1 = (Direction)Arg1;
                    }
                    if ((INTYPEOF(Arg2, "IFC4.Vector")))
                    {
                        Mag2 = (Arg2 as Vector).Magnitude;
                        Vec2 = (Arg2 as Vector).Orientation;
                    }
                    else
                    {
                        Mag2 = 1.0;
                        Vec2 = (Direction)Arg2;
                    }
                    Vec1 = (Direction)IfcNormalise(Vec1);
                    Vec2 = (Direction)IfcNormalise(Vec2);
                    Ndim = SIZEOF(Vec1.DirectionRatios);
                    Mag = 0.0;

                    Res = (Ndim == 2)
                        ? New.Direction(0.0, 0.0)
                        : New.Direction(0.0, 0.0, 0.0);


                    for (var i = 1; i <= Ndim; i++)
                    {
                        Res.DirectionRatios[i] = Mag1 * Vec1.DirectionRatios[i] + Mag2 * Vec2.DirectionRatios[i];
                        Mag = Mag + (Res.DirectionRatios[i] * Res.DirectionRatios[i]);
                    }
                    Result = (Mag > 0.0)
                        ? New.Vector(Res, SQRT(Mag))
                        : New.Vector(Vec1, 0.0);
                }

            }
            return (Result);
        }
    }
}
