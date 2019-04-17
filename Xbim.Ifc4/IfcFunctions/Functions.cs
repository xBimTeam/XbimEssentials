using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.IfcFunctions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationAppearanceResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.TopologyResource;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc4
{
    public static class Functions
    {
        #region "Implementation hacks"

        internal static TypesArray NewTypesArray(params string[] args)
        {
            return new TypesArray(args);
        }
        
        internal static IfcEdgeLoop AsIfcEdgeLoop(this IfcLoop toCast)
        {
            return toCast as IfcEdgeLoop;
        }

        internal static bool AsBool(this IfcBoolean toCast)
        {
            var val = (bool?)toCast.Value;
            if (!val.HasValue)
                throw new Exception("IfcLogical value not defined attempting bool conversion.");
            return val.Value;
        }
        internal static Direction IfcDirection(double x, double y, double z)
        {
            return new Direction(x, y, z);
        }

        internal static double IfcDotProduct(Direction dir1, IfcDirection dir2)
        {
            return
                dir1.X*dir2.X +
                dir1.Y*dir2.Y +
                dir1.Z*dir2.Z;
        }

        internal static bool AsBool(this IfcLogical toCast)
        {
            var val = (bool?)toCast.Value;
            if (!val.HasValue)
                throw new Exception("IfcLogical value not defined attempting bool conversion.");
            return val.Value;
        }

        internal static IfcRelAssociatesMaterial AsIfcRelAssociatesMaterial(this IPersistEntity toCast)
        {
            return toCast as IfcRelAssociatesMaterial;
        }

        internal static IfcDescriptiveMeasure AsIfcDescriptiveMeasure(this IfcSizeSelect toCast)
        {
            return (IfcDescriptiveMeasure) toCast;
        }

        internal static IfcLengthMeasure AsIfcLengthMeasure(this IfcSizeSelect toCast)
        {
            return (IfcLengthMeasure) toCast;
        }

        internal static IfcEdgeCurve AsIfcEdgeCurve(this IfcEdge toCast)
        {
            return  toCast as IfcEdgeCurve;
        }

        internal static IEnumerable<double> DirectionRatios(this XbimVector3D vector3D)
        {
            yield return vector3D.X;
            yield return vector3D.Y;
            yield return vector3D.Z;
        }
        
        internal static T ItemAt<T>(this IEnumerable<T> enumerable, long index) // where T : class
        {
            if (enumerable == null)
                return default(T);
            var asArr = enumerable.ToArray();
            if (index < asArr.Length)
                return asArr[index];
            return default(T); 
        }
        
        #endregion

        #region "Express Built-in functions"
        internal static T NVL<T>(T obj1, T obj2) where T : class
        {
            return obj1 ?? obj2;
        }
        
        internal static IEnumerable<IPersistEntity> USEDIN(IPersistEntity ifcObject, string v)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (v)
            {
                case "IFC4.IFCRELASSOCIATES.RELATEDOBJECTS":
                    return ifcObject.Model.Instances.OfType<IfcRelAssociates>().Where(x => x.RelatedObjects.Contains(ifcObject));
            }
            throw new Exception(string.Format("NotImplemented: USEDIN does not support role {0}.", v));
        }
        
        internal static bool EXISTS(object o)
        {
            return o != null;
        }
        
        internal static int SIZEOF<T>(IEnumerable<T> source)
        {
            return source.Count();
        }
        
        
        internal static int SIZEOF(TypesArray array)
        {
            return array.Count();
        }

        internal static bool INTYPEOF(IPersist obj, string typeString)
        {
            return TYPEOF(obj).Contains(typeString);
        }

        internal static bool INTYPEOF(IVectorOrDirection obj, string typeString)
        {
            if (obj is Vector && typeString.ToLowerInvariant().Contains("vector"))
                return true;
            if (obj is Direction && typeString.ToLowerInvariant().Contains("direction"))
                return true;
            return false;
        }

        internal static double SQRT(double mag)
        {
            return Math.Sqrt(mag);
        }

        internal static double ABS(double mag)
        {
            return Math.Abs(mag);
        }

        internal static int BLENGTH(IfcBinary value)
        {
            // todo: blenght is the size in bits of the ifcbinary value provided
            throw new NotImplementedException();
        }

        internal static TypesArray TYPEOF(IPersist instance)
        {
            return new TypesArray(instance);
        }

        internal static int HIINDEX<T>(IEnumerable<T> source)
        {
            return source.Count();
        }

        #endregion

        #region "Ifc4 functions"
        
        internal static T IfcBooleanChoose<T>(bool B, T Choice1, T Choice2)
        {
            return B 
                ? (Choice1) 
                : (Choice2);
        }
        
        internal static bool IfcCorrectUnitAssignment(IItemSet<IfcUnit> Units)
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

            // index ok
            for (var i = 0; i < SIZEOF(Units); i++)
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

        internal static Vector IfcCrossProduct(IfcDirection Arg1, IfcDirection Arg2)
        {
            // using ifc4 defined code

            // local variables
            double Mag;
            Direction Res;
            double[] V1;
            double[] V2;
            Vector Result;
            
            if ((!EXISTS(Arg1) || (Arg1.Dim == 2)) || (!EXISTS(Arg2) || (Arg2.Dim == 2)))
            {
                return null;
            }

            var dArg1 = new Direction(Arg1);
            var dArg2 = new Direction(Arg2);

            V1 = IfcNormalise(dArg1).DirectionRatios;
            V2 = IfcNormalise(dArg2).DirectionRatios;
            Res = new Direction(
                (V1[1] * V2[2] - V1[2] * V2[1]),
                (V1[2] * V2[0] - V1[0] * V2[2]),
                (V1[0] * V2[1] - V1[1] * V2[0])
            );
            Mag = 0.0;
            for (var i = 0; i < 3; i++)
            {
                Mag = Mag + Res.DirectionRatios[i] * Res.DirectionRatios[i];
            }
            Result = (Mag > 0.0)
                ? new Vector(Res, SQRT(Mag)) 
                : new Vector(dArg1, 0.0);
            return (Result);
        }

        internal static bool IfcTopologyRepresentationTypes(IfcLabel? RepType, IItemSet<IfcRepresentationItem> Items)
        {
            // local variables
            int Count = 0;
            if (!RepType.HasValue)
                return true; // deduced from Undefined clause below 

            switch (RepType.Value)
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
        
        internal static bool IfcTaperedSweptAreaProfiles(IfcProfileDef StartArea, IfcProfileDef EndArea)
        {

            // local variables
            bool Result = false;


            if (INTYPEOF(StartArea, "IFC4.IFCPARAMETERIZEDPROFILEDEF"))
            {
                if (INTYPEOF(EndArea, "IFC4.IFCDERIVEDPROFILEDEF"))
                {
                    var end = EndArea as IIfcDerivedProfileDef;
                    // todo: handle null case?
                    Result = StartArea == end.ParentProfile;
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
                    // todo: handle null case?
                    Result = StartArea == end.ParentProfile;
                }
                else
                {
                    Result = false;
                }
            }
            return (Result);
        }
        
        internal static bool IfcShapeRepresentationTypes(IfcLabel? RepType, IItemSet<IfcRepresentationItem> Items)
        {
            // local variables
            int Count = 0;
            if (!RepType.HasValue)
                return (Count == Items.Count);
            switch (RepType.Value)
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
                    Count = Items.Count(x => x is IIfcCurve && ((IIfcCurve) x).Dim == 2);
                    break;
                case "Curve3D":
                    Count = Items.Count(x => x is IIfcCurve && ((IIfcCurve) x).Dim == 3);
                    break;

                case "Surface":
                    Count = Items.Count(x => x is IIfcSurface);
                    break;

                case "Surface2D":
                    Count = Items.Count(x => x is IIfcSurface && ((IIfcSurface) x).Dim == 2);
                    break;


                case "Surface3D":
                    Count = Items.Count(x => x is IIfcSurface && ((IIfcSurface) x).Dim == 3);
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

        internal static bool IfcConsecutiveSegments(IOptionalItemSet<IfcSegmentIndexSelect> Segments)
        {
            throw  new NotImplementedException();
            //// todo: complete implementation
            //bool Result = true;
            //for (int iSegment = 1; iSegment <= HIINDEX(Segments); iSegment++)
            //{
            //    var tSegment = iSegment - 1;
               
            //    //if (Segments[iSegment][HIINDEX(Segments[iSegment])] != Segments[iSegment + 1][1])
            //    //{
            //    //    Result = false;
            //    //    continue;
            //    //}
            //}
            //return Result;
        }

        internal static bool IfcUniquePropertySetNames(IOptionalItemSet<IfcPropertySetDefinition> hasPropertySets)
        {
            var asList = hasPropertySets.ToList();
            var values = asList.Select(x => x.Name).ToList();
            var isUnique = values.Distinct().Count() == asList.Count();
            return isUnique;
        }

        internal static bool IfcUniqueDefinitionNames(IEnumerable<IfcRelDefinesByProperties> RelDefinesByProperties)
        {
            var values = RelDefinesByProperties.Select(x => x.Name).ToList();
            var isUnique = values.Distinct().Count() == RelDefinesByProperties.Count();
            return isUnique; 
        }

        internal static bool IfcUniquePropertyName(IItemSet<IfcProperty> Properties)
        {
            // note: this function has been substantially rewritten to take advangate of linq.
            //
            var propNames = Properties.Select(x => x.Name).ToList();
            var isUnique = propNames.Distinct().Count() == Properties.Count();
            return isUnique;
        }

        internal static bool IfcUniquePropertyTemplateNames(IItemSet<IfcPropertyTemplate> PropertyTemplates)
        {

            // note: this function has been substantially rewritten to take advangate of linq.
            //
            var propNames = PropertyTemplates.Select(x => x.Name).ToList();
            var isUnique = propNames.Distinct().Count() == PropertyTemplates.Count();
            return isUnique;

        }

        internal static bool IfcUniqueQuantityNames(IItemSet<IfcPhysicalQuantity> Quantities)
        {
            // note: this function has been substantially rewritten to take advangate of linq.
            //
            var propNames = Quantities.Select(x => x.Name).ToList();
            var isUnique = propNames.Distinct().Count() == Quantities.Count();
            return isUnique;
        }

        internal static bool IfcCurveWeightsPositive(IfcRationalBSplineCurveWithKnots B)
        {
            
            // local variables
            bool Result = true;


            // todo: check indices range from specs ... possible bug in express
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

        internal static bool IfcCorrectObjectAssignment(IfcObjectTypeEnum? Constraint, IItemSet<IfcObjectDefinition> Objects)
        {
            if (!Constraint.HasValue)
                return true;

            var val = IfcCorrectObjectAssignment(Constraint.Value, Objects.ToArray());
            if (!val.HasValue)
            {
                throw new ArgumentException("Undetermined value in where clause.");
            }
            return val.Value;
        }

        internal static bool? IfcCorrectObjectAssignment(IfcObjectTypeEnum Constraint, IEnumerable<IfcObjectDefinition> Objects)
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
        
        /// <summary>
        /// Todo: add summary
        /// </summary>
        /// <param name="Degree">degree</param>
        /// <param name="UpKnots">upperIndexOnKnots</param>
        /// <param name="UpCp">upperIndexOnControlPoints</param>
        /// <param name="KnotMult">knotMultiplicities</param>
        /// <param name="Knots">knots</param>
        /// <returns></returns>
        internal static bool IfcConstraintsParamBSpline(IfcInteger Degree, IfcInteger UpKnots, IfcInteger UpCp, IItemSet<IfcInteger> KnotMult, IItemSet<IfcParameterValue> Knots)
        {
            // local variables
            bool Result = true;
            int K;
            int Sum;
            
            Sum = (int) KnotMult[0];
            for (var i = 2; i <= UpKnots; i++)
            {
                Sum = (int) (Sum + KnotMult[i-1]);
            }


            if ((Degree < 1) || (UpKnots < 2) || (UpCp < Degree) ||
                (Sum != (Degree + UpCp + 2)))
            {
                Result = false;
                return (Result);
            }

            K = (int) KnotMult[1];
            if ((K < 1) || (K > Degree + 1))
            {
                Result = false;
                return (Result);
            }

            for (var i = 2; i <= UpKnots; i++)
            {
                if ((KnotMult[i-1] < 1) || (Knots[i-1] <= Knots[i - 2]))
                {
                    Result = false;
                    return (Result);
                }
                K = (int) KnotMult[i-1];
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

        private static bool HasIfcDimensionalExponents(IIfcDimensionalExponents dim, int len, int mass, int time, int elec, int temp, int substance, int lum)
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

        internal static bool IfcCorrectDimensions(IfcUnitEnum unitType, IfcDimensionalExponents dimensions)
        {
            var val = NullableIfcCorrectDimensions(unitType, dimensions);
            if (!val.HasValue)
            {
                throw new ArgumentException("Undetermined value in where clause.");
            }
            return val.Value;
        }

        private static bool? NullableIfcCorrectDimensions(IfcUnitEnum m, IfcDimensionalExponents Dim)
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



        internal static bool IfcCorrectLocalPlacement(IfcAxis2Placement relativePlacement, IfcObjectPlacement placementRelTo)
        {
            var val = NullableIfcCorrectLocalPlacement(relativePlacement, placementRelTo);
            if (!val.HasValue)
            {
                throw new ArgumentException("Undetermined value in where clause.");
            }
            return val.Value;
        }

        private static bool? NullableIfcCorrectLocalPlacement(IfcAxis2Placement AxisPlacement, IfcObjectPlacement RelPlacement)
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

        internal static bool IfcPathHeadToTail(IfcPath ifcPath)
        {
            var val = NullableIfcPathHeadToTail(ifcPath);
            if (!val.HasValue)
            {
                throw new ArgumentException("Undetermined value in where clause.");
            }
            return val.Value;
        }

        private static bool? NullableIfcPathHeadToTail(IfcPath APath)
        {
            // local variables
            int N = 0;
            bool? P = null;
            
            N = SIZEOF(APath.EdgeList);
            for (var i = 2; i <= N; i++)
            {
                if (!P.HasValue)
                    P = true;
                P = P.Value && (APath.EdgeList[i - 2].EdgeEnd == APath.EdgeList[i - 1].EdgeStart);
            }
            return (P);
        }

        internal static bool IfcLoopHeadToTail(IfcEdgeLoop ALoop)
        {
            // local variables
            int N;
            bool P = true;
            
            N = SIZEOF(ALoop.EdgeList);
            for (var i = 2; i <= N; i++)
            {
                P = P && (ALoop.EdgeList[i - 2].EdgeEnd == ALoop.EdgeList[i-1].EdgeStart);
            }
            return (P);
        }

        internal static bool IfcCorrectFillAreaStyle(IItemSet<IfcFillStyleSelect> Styles)
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

        internal static bool IfcSurfaceWeightsPositive(IfcRationalBSplineSurfaceWithKnots B)
        {
            // local variables
            bool Result = true;

            // todo: check indices range from specs... possible bug in express
            for (var i = 1; i <= (B as IIfcBSplineSurface).UUpper; i++)
            {
                for (var j = 1; j <= (B as IIfcBSplineSurface).VUpper; j++)
                {
                    if ((B.Weights[i-1][j-1] <= 0.0))
                    {
                        Result = false;
                        return (Result);
                    }
                }
            }
            return (Result);
        }

        #endregion

        #region  "Called function"

        static IVectorOrDirection IfcNormalise(IVectorOrDirection Arg)
        {
            // local variables
            int Ndim;
            Direction V = new Direction(1.0, 0.0);
            Vector Vec = new Vector(new Direction(1.0, 0.0), 1.0);
            double Mag;
            IVectorOrDirection Result = V;
            
            if (!EXISTS(Arg))
            {
                return (null);
            }
            else
            {
                if (Arg is Vector)
                {
                    Ndim = Arg.Dim;
                    var vArg = (Arg as Vector);
                    V.DirectionRatios = vArg.Orientation.DirectionRatios;
                    Vec.Magnitude = vArg.Magnitude;
                    Vec.Orientation = V;
                    if (vArg.Magnitude == 0.0)
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
                    var dArg = Arg as Direction;
                    Ndim = dArg.Dim;
                    V.DirectionRatios = dArg.DirectionRatios;                   
                }

                Mag = 0.0;
                for (var i = 0; i < Ndim; i++)
                {
                    Mag = Mag + V.DirectionRatios[i] * V.DirectionRatios[i];
                }
                if (Mag > 0.0)
                {
                    Mag = SQRT(Mag);
                    for (var i = 0; i < Ndim; i++)
                    {
                        V.DirectionRatios[i] = V.DirectionRatios[i] / Mag;
                    }
                    if (Arg is Vector)
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

        #endregion

        
    }
}
