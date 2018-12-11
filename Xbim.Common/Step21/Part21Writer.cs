#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Part21FileWriter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;


namespace Xbim.IO.Step21
{
    public static class Part21Writer 
    {
        /// <summary>
        /// Writes full model into output writer as STEP21 file
        /// </summary>
        /// <param name="model">Model to be serialized</param>
        /// <param name="output">Output writer</param>
        /// <param name="metadata">Metadata to be used for serialization</param>
        /// <param name="map">Optional map can be used to map occurrences in the file</param>
        /// <param name="progress">A progress delegate</param>
        public static void Write(IModel model, TextWriter output, ExpressMetaData metadata, IDictionary<int, int> map = null, ReportProgressDelegate progress = null)
        {
            var header = model.Header ?? new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults,model);
            string fallBackSchema = null;

            if (header.FileSchema == null || !header.FileSchema.Schemas.Any())
            {
                var instance = model.Instances.FirstOrDefault();
                if (instance != null)
                {
                    var eft = instance.GetType().GetTypeInfo().Assembly.GetTypes().Where(t => typeof(IEntityFactory).GetTypeInfo().IsAssignableFrom(t)).FirstOrDefault();
                    if (eft == null)
                        throw new XbimException("It wasn't possible to find valid schema definition");
                    var ef = Activator.CreateInstance(eft) as IEntityFactory;
                    if (ef == null)
                        throw new XbimException("It wasn't possible to find valid schema definition");

                    fallBackSchema = string.Join(",", ef.SchemasIds);

                }
            }

            WriteHeader(header, output, fallBackSchema);
                
            var count = model.Instances.Count;
            var counter = 0;
            var report = progress != null;

            foreach (var entity in model.Instances)
            {
                WriteEntity(entity, output, metadata, map);
                output.WriteLine();

                if (report)
                {
                    counter++;
                    // report every 1000 entities
                    if (counter % 1000 == 0)
                    {
                        var percent = (int)((float)counter / (float)count);
                        progress(percent, null);
                    }
                }
            }
            WriteFooter(output);

            if (progress != null)
                progress(100, null);
        }

        /// <summary>
        /// Writes STEP21 header to the output
        /// </summary>
        /// <param name="header">Header</param>
        /// <param name="output">Writer</param>
        /// <param name="overridingSchema">Schema to be written to the header instead of the schema defined in the header. 
        /// This is useful if the schema is not defined in the header.</param>
        public static void WriteHeader(IStepFileHeader header, TextWriter output, string overridingSchema = null)
        {
            output.WriteLine("ISO-10303-21;");
            output.WriteLine("HEADER;");
            //FILE_DESCRIPTION
            output.Write("FILE_DESCRIPTION ((");
            var i = 0;

            if (header.FileDescription.Description.Count == 0)
            {
                output.Write(@"''");
            }
            else
            {
                foreach (var item in header.FileDescription.Description)
                {
                    output.Write(@"{0}'{1}'", i == 0 ? "" : ",", item.ToPart21());
                    i++;
                }
            }
            output.Write(@"), '{0}');", header.FileDescription.ImplementationLevel);
            output.WriteLine();
            //FileName
            output.Write("FILE_NAME (");
            output.Write(@"'{0}'", (header.FileName != null && header.FileName.Name != null) ? header.FileName.Name.ToPart21() : "");
            output.Write(@", '{0}'", header.FileName != null ? header.FileName.TimeStamp : "");
            output.Write(", (");
            i = 0;
            if (header.FileName != null && header.FileName.AuthorName.Count == 0)
                output.Write(@"''");
            else
            {
                if (header.FileName != null)
                    foreach (var item in header.FileName.AuthorName)
                    {
                        output.Write(@"{0}'{1}'", i == 0 ? "" : ",", item.ToPart21());
                        i++;
                    }
            }
            output.Write("), (");
            i = 0;
            if (header.FileName != null && header.FileName.Organization.Count == 0)
                output.Write(@"''");
            else
            {
                if (header.FileName != null)
                    foreach (var item in header.FileName.Organization)
                    {
                        output.Write(@"{0}'{1}'", i == 0 ? "" : ",", item.ToPart21());
                        i++;
                    }
            }
            if (header.FileName != null)
                output.Write(@"), '{0}', '{1}', '{2}');", header.FileName.PreprocessorVersion.ToPart21(), header.FileName.OriginatingSystem.ToPart21(),
                    header.FileName.AuthorizationName.ToPart21());
            output.WriteLine();
            
            //FileSchema
            output.Write("FILE_SCHEMA (('{0}'));", overridingSchema ?? header.FileSchema.Schemas.FirstOrDefault());
            output.WriteLine();
            output.WriteLine("ENDSEC;");
            output.WriteLine("DATA;");
        }

        public static void WriteFooter(TextWriter output)
        {
            output.WriteLine("ENDSEC;");
            output.WriteLine("END-ISO-10303-21;");
        }

        /// <summary>
        /// Writes the entity to a TextWriter in the Part21 format
        /// </summary>
        /// <param name="output">The TextWriter</param>
        /// <param name="entity">The entity to write</param>
        /// <param name="metadata"></param>
        /// <param name="map"></param>
        public static void WriteEntity(IPersistEntity entity, TextWriter output, ExpressMetaData metadata, IDictionary<int, int> map = null)
        {
            var expressType = metadata.ExpressType(entity);
            if (map != null && map.Keys.Contains(entity.EntityLabel)) return; //if the entity is replaced in the map do not write it
            output.Write("#{0}={1}(", entity.EntityLabel, expressType.ExpressNameUpper);

            var first = true;

            foreach (var ifcProperty in expressType.Properties.Values)
            //only write out persistent attributes, ignore inverses
            {
                if (ifcProperty.EntityAttribute.State == EntityAttributeState.DerivedOverride)
                {
                    if (!first)
                        output.Write(',');
                    output.Write('*');
                    first = false;
                }
                else
                {
                    var propType = ifcProperty.PropertyInfo.PropertyType;
                    var propVal = ifcProperty.PropertyInfo.GetValue(entity, null);
                    if (!first)
                        output.Write(',');
                    WriteProperty(propType, propVal, output, map, metadata);
                    first = false;
                }
            }
            output.Write(");");

        }

        /// <summary>
        /// Writes a property of an entity to the TextWriter in the Part21 format
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="propVal"></param>
        /// <param name="output"></param>
        /// <param name="map"></param>
        /// <param name="metadata"></param>
        public static void WriteProperty(Type propType, object propVal, TextWriter output, IDictionary<int, int> map, ExpressMetaData metadata)
        {
            Type itemType;
            if (propVal == null) //null or a value type that maybe null
                output.Write('$');
            else if (propVal is IOptionalItemSet && !((IOptionalItemSet)propVal).Initialized)
                output.Write('$');
            else if (propType.GetTypeInfo().IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            //deal with undefined types (nullables)
            {
                var complexType = propVal as IExpressComplexType;
                if (complexType != null)
                {
                    output.Write('(');
                    var first = true;
                    foreach (var compVal in complexType.Properties)
                    {
                        if (!first)
                            output.Write(',');
                        WriteProperty(compVal.GetType(), compVal, output, map, metadata);
                        first = false;
                    }
                    output.Write(')');
                }
                else if ((propVal is IExpressValueType))
                {
                    var expressVal = (IExpressValueType)propVal;
                    WriteValueType(expressVal.UnderlyingSystemType, expressVal.Value, output);
                }
                else // if (propVal.GetType().IsEnum)
                {
                    WriteValueType(propVal.GetType(), propVal, output);
                }
            }
            else if (typeof(IExpressComplexType).GetTypeInfo().IsAssignableFrom(propType))
            {
                output.Write('(');
                var first = true;
                foreach (var compVal in ((IExpressComplexType)propVal).Properties)
                {
                    if (!first)
                        output.Write(',');
                    WriteProperty(compVal.GetType(), compVal, output, map, metadata);
                    first = false;
                }
                output.Write(')');
            }
            else if (typeof(IExpressValueType).GetTypeInfo().IsAssignableFrom(propType))
            //value types with a single property (IfcLabel, IfcInteger)
            {
                var realType = propVal.GetType();
                if (realType != propType)
                //we have a type but it is a select type use the actual value but write out explicitly
                {
                    output.Write(realType.Name.ToUpper());
                    output.Write('(');
                    WriteProperty(realType, propVal, output, map, metadata);
                    output.Write(')');
                }
                else //need to write out underlying property value
                {
                    var expressVal = (IExpressValueType)propVal;
                    WriteValueType(expressVal.UnderlyingSystemType, expressVal.Value, output);
                }
            }
            else if (typeof(IExpressEnumerable).GetTypeInfo().IsAssignableFrom(propType) &&
                     (itemType = propType.GetItemTypeFromGenericType()) != null)
            //only process lists that are real lists, see Cartesian point
            {
                output.Write('(');
                var first = true;
                foreach (var item in ((IExpressEnumerable)propVal))
                {
                    if (!first)
                        output.Write(',');
                    WriteProperty(itemType, item, output, map, metadata);
                    first = false;
                }
                output.Write(')');
            }
            else if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(propType))
            //all writable entities must support this interface and ExpressType have been handled so only entities left
            {
                output.Write('#');
                var label = ((IPersistEntity)propVal).EntityLabel;
                int mapLabel;
                if (map != null && map.TryGetValue(label, out mapLabel))
                    label = mapLabel;
                output.Write(label);
            }
            else if (propType.GetTypeInfo().IsValueType || propVal is string || propVal is byte[]) //it might be an in-built value type double, string etc.
            {
                WriteValueType(propVal.GetType(), propVal, output);
            }
            else if (typeof(IExpressSelectType).GetTypeInfo().IsAssignableFrom(propType))
            // a select type get the type of the actual value
            {
                if (propVal.GetType().GetTypeInfo().IsValueType) //we have a value type, so write out explicitly
                {
                    var type = metadata.ExpressType(propVal.GetType());
                    output.Write(type.ExpressNameUpper);
                    output.Write('(');
                    WriteProperty(propVal.GetType(), propVal, output, map, metadata);
                    output.Write(')');
                }
                else //could be anything so re-evaluate actual type
                {
                    WriteProperty(propVal.GetType(), propVal, output, map, metadata);
                }
            }
            else
                throw new Exception(string.Format("Entity  has illegal property {0} of type {1}",
                                                  propType.Name, propType.Name));
        }

        private static readonly IFormatProvider _p21Provider = new Part21Formatter();

        /// <summary>
        /// Writes the value of a property to the TextWriter in the Part 21 format
        /// </summary>
        /// <param name="pInfoType"></param>
        /// <param name="pVal"></param>
        /// <param name="output"></param>
        private static void WriteValueType(Type pInfoType, object pVal, TextWriter output)
        {
            if (pInfoType == typeof(Double))
                output.Write(string.Format(_p21Provider, "{0:R}", pVal));
            else if (pInfoType == typeof(String)) //convert  string
            {
                if (pVal == null)
                    output.Write('$');
                else
                {
                    output.Write('\'');
                    output.Write(((string)pVal).ToPart21());
                    output.Write('\'');
                }
            }
            else if (pInfoType == typeof(byte[])) //convert  string
            {
                output.Write("\"0");
                var bytes = (byte[])pVal;
                foreach (byte b in bytes)
                    output.Write("{0:X2}", b);
                output.Write('"');
            }
            else if (pInfoType == typeof(Int16) || pInfoType == typeof(Int32) || pInfoType == typeof(Int64))
                output.Write(pVal.ToString());
            else if (pInfoType.GetTypeInfo().IsEnum) //convert enum
                output.Write(".{0}.", pVal.ToString().ToUpper());
            else if (pInfoType == typeof(bool))
            {
                if (pVal != null)
                {
                    var b = (bool)pVal;
                    output.Write(".{0}.", b ? "T" : "F");
                }
            }
            else if (pInfoType == typeof(DateTime)) //convert  TimeStamp
                output.Write(string.Format(_p21Provider, "{0:T}", pVal));
            else if (pInfoType == typeof(Guid)) //convert  Guid string
            {
                if (pVal == null)
                    output.Write('$');
                else
                    output.Write(string.Format(_p21Provider, "{0:G}", pVal));
            }
            else if (pInfoType == typeof(bool?)) //convert  logical
            {
                var b = (bool?)pVal;
                output.Write(!b.HasValue ? ".U." : string.Format(".{0}.", b.Value ? "T" : "F"));
            }
            else
                throw new ArgumentException(string.Format("Invalid Value Type {0}", pInfoType.Name), "pInfoType");
        }

    }
}