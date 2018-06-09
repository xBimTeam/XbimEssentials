using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.Ifc;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Profiling.Parsing
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string name = "SampleHouse.ifc";
            //var modelName = "Lakeside.ifc";
            var modelName = @"C:\Users\Steve\Documents\IfcModels\Hospital_NL_Arch.ifc";
            //var modelName = @"c:\CODE\SampleData\UniversityOfAuckland\20160125WestRiverSide Hospital - IFC4-Autodesk_Hospital_Metric_Electrical.ifc";
            //var modelName = @"c:\CODE\SampleData\UniversityOfAuckland\20160125WestRiverSide Hospital - IFC4-Autodesk_Hospital_Metric_Plumbing.ifc";
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                    modelName = args[0];
            }
            var w = new Stopwatch();
            w.Start();
            using (var fileStrm = File.OpenRead(modelName))
            {

                var ignored = GetTypesToIgnore();
                using (var model = MemoryModel.OpenReadStep21(fileStrm, null, null,ignored ))
                {
                    w.Stop();
                    Console.WriteLine(@"{0:F2} ms to create in memory model", w.ElapsedMilliseconds);
                }
            }
        }

        private static HashSet<string> GetTypesToIgnore()
        {
            var ifc2x3MetaData = ExpressMetaData.GetMetadata((new Xbim.Ifc2x3.EntityFactoryIfc2x3()).GetType().GetTypeInfo().Module);
            var ifc4MetaData = ExpressMetaData.GetMetadata((new Xbim.Ifc4.EntityFactoryIfc4()).GetType().GetTypeInfo().Module);
            var allMetadata = new[] { ifc2x3MetaData, ifc4MetaData };
            var requiredTypes = new HashSet<string>();
            void addSubtypes<T>(Func<ExpressType, bool> condition = null) where T : IPersist
            {
                foreach (var metadata in allMetadata)
                {
                    if (condition == null)
                        foreach (var t in metadata.ExpressTypesImplementing(typeof(T)))
                            requiredTypes.Add(t.ExpressNameUpper);
                    else
                        foreach (var t in metadata.ExpressTypesImplementing(typeof(T)).Where(condition))
                            requiredTypes.Add(t.ExpressNameUpper);
                }
            }

            foreach (var metadata in allMetadata)
            {
                addSubtypes<IIfcProject>();
                addSubtypes<IIfcAsset>();
                addSubtypes<IIfcSystem>(t => !typeof(IIfcStructuralAnalysisModel).IsAssignableFrom(t.Type));
                addSubtypes<IIfcActor>();
                addSubtypes<IIfcElement>(t => !typeof(IIfcFeatureElement).IsAssignableFrom(t.Type)
                             && !typeof(IIfcVirtualElement).IsAssignableFrom(t.Type)
                             && !typeof(IIfcOpeningElement).IsAssignableFrom(t.Type));
                addSubtypes<IIfcSpatialElement>();
                addSubtypes<IIfcProxy>();
                addSubtypes<IIfcRelContainedInSpatialStructure>();
                addSubtypes<IIfcTypeProduct>();
                addSubtypes<IIfcPropertySetDefinitionSelect>();
                addSubtypes<IIfcRelDefinesByProperties>();
                addSubtypes<IIfcSimpleProperty>();
                addSubtypes<IIfcElementQuantity>();
                addSubtypes<IIfcPhysicalSimpleQuantity>();
                addSubtypes<IIfcRelDefinesByType>();
                addSubtypes<IIfcUnitAssignment>();
                addSubtypes<IIfcNamedUnit>();
                addSubtypes<IIfcMeasureWithUnit>();
                addSubtypes<IIfcDimensionalExponents>();
                addSubtypes<IIfcRelAssociatesClassification>();
                addSubtypes<IIfcClassificationReference>();
                addSubtypes<IIfcClassification>();

                addSubtypes<IIfcActorSelect>();
                addSubtypes<IIfcAddress>();
                addSubtypes<IIfcApplication>();
                addSubtypes<IIfcActorRole>();
                addSubtypes<IIfcDocumentSelect>();
                addSubtypes<IIfcAddress>();
                // materials to be converted into attributes
                addSubtypes<IIfcMaterialSelect>();
                addSubtypes<IIfcRelAssociatesMaterial>();
            }


            //need this for legacy and Ifc2x3 classification
            foreach (var t in ifc2x3MetaData.ExpressTypesImplementing(typeof(IfcCalendarDate))) requiredTypes.Add(t.ExpressNameUpper);
            foreach (var t in ifc2x3MetaData.ExpressTypesImplementing(typeof(IfcDoorStyle))) requiredTypes.Add(t.ExpressNameUpper);
            foreach (var t in ifc2x3MetaData.ExpressTypesImplementing(typeof(IfcWindowStyle))) requiredTypes.Add(t.ExpressNameUpper);
            var allTypes =
                   ifc2x3MetaData.Types().Where(et => typeof(IPersistEntity).IsAssignableFrom(et.Type) && !et.Type.IsAbstract).Select(et => et.ExpressNameUpper)
                   .Concat(ifc2x3MetaData.Types().Where(et => typeof(IPersistEntity).IsAssignableFrom(et.Type) && !et.Type.IsAbstract).Select(et => et.ExpressNameUpper));
            return new HashSet<string>(allTypes.Except(requiredTypes));

        }
    }
}
