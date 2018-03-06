using System;
using System.Collections.Generic;
using System.IO;
using Xbim.Common.Exceptions;
using Xbim.Common;
using Xbim.Common.Metadata;
using Xbim.IO.Esent;
using Xbim.IO.Step21;
using Xbim.IO.Step21.Parser;

namespace Xbim.IO
{
    public static class PersistEntityExtensions
    {

        #region Write the properties of an IPersistEntity to a stream
        //internal static void WriteEntity(this IPersistEntity entity, TextWriter tw, byte[] propertyData, ExpressMetaData metadata)
        //{
        //    var type = metadata.ExpressType(entity);
        //    tw.Write("#{0}={1}", entity.EntityLabel, type.ExpressNameUpper);
        //    var br = new BinaryReader(new MemoryStream(propertyData));
        //    var action = (P21ParseAction)br.ReadByte();
        //    var comma = false; //the first property
        //    while (action != P21ParseAction.EndEntity)
        //    {
        //        switch (action)
        //        {
        //            case P21ParseAction.BeginList:
        //                tw.Write("(");
        //                break;
        //            case P21ParseAction.EndList:
        //                tw.Write(")");
        //                break;
        //            case P21ParseAction.BeginComplex:
        //                tw.Write("&SCOPE");
        //                break;
        //            case P21ParseAction.EndComplex:
        //                tw.Write("ENDSCOPE");
        //                break;
        //            case P21ParseAction.SetIntegerValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write(br.ReadInt64().ToString());
        //                break;
        //            case P21ParseAction.SetHexValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write(br.ReadString());
        //                break;
        //            case P21ParseAction.SetFloatValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write(br.ReadDouble().AsPart21());
        //                break;
        //            case P21ParseAction.SetStringValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write(br.ReadString());
        //                break;
        //            case P21ParseAction.SetEnumValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write("." + br.ReadString() + ".");
        //                break;
        //            case P21ParseAction.SetBooleanValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write(br.ReadBoolean() ? ".T." : ".F.");
        //                break;
        //            case P21ParseAction.SetNonDefinedValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write("$");
        //                break;
        //            case P21ParseAction.SetOverrideValue:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write("*");
        //                break;
        //            case P21ParseAction.SetObjectValueUInt16:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write("#"+ br.ReadUInt16());
        //                break;
        //            case P21ParseAction.SetObjectValueInt32:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write("#" + br.ReadInt32());
        //                break;
        //            case P21ParseAction.SetObjectValueInt64:
        //                if (comma) tw.Write(",");
        //                comma = true;
        //                tw.Write("#" + br.ReadInt64());
        //                break;
        //            case P21ParseAction.BeginNestedType:
        //                if (comma) tw.Write(",");
        //                comma = false;
        //                tw.Write(br.ReadString()+"(");
        //                break;
        //            case P21ParseAction.EndNestedType:
        //                comma = true;
        //                tw.Write(")");
        //                break;
        //            case P21ParseAction.EndEntity:
        //                tw.Write(");");
        //                break;
        //            case P21ParseAction.NewEntity:
        //                comma = false;
        //                tw.Write("(");
        //                break;
        //            default:
        //                throw new Exception("Invalid Property Record #" + entity.EntityLabel + " EntityType: " + entity.GetType().Name);
        //        }
        //        action = (P21ParseAction)br.ReadByte();
        //    }
        //    tw.WriteLine();
        //}

        internal static XbimInstanceHandle GetHandle(this IPersistEntity entity)
        {
            return new XbimInstanceHandle(entity);
        }

        internal static void WriteEntity(this IPersistEntity entity, BinaryWriter entityWriter, ExpressMetaData metadata)
        {

            var expressType = metadata.ExpressType(entity);
            // entityWriter.Write(Convert.ToByte(P21ParseAction.NewEntity));
            entityWriter.Write(Convert.ToByte(P21ParseAction.BeginList));
            foreach (var ifcProperty in expressType.Properties.Values)
            //only write out persistent attributes, ignore inverses
            {
                if (ifcProperty.EntityAttribute.State == EntityAttributeState.DerivedOverride)
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetOverrideValue));
                else
                {
                    var propType = ifcProperty.PropertyInfo.PropertyType;
                    var propVal = ifcProperty.PropertyInfo.GetValue(entity, null);
                    WriteProperty(propType, propVal, entityWriter, metadata);
                }
            }
            entityWriter.Write(Convert.ToByte(P21ParseAction.EndList));
            entityWriter.Write(Convert.ToByte(P21ParseAction.EndEntity));
        }

        private static void WriteProperty(Type propType, object propVal, BinaryWriter entityWriter, ExpressMetaData metadata)
        {
            Type itemType;
            if (propVal == null) //null or a value type that maybe null
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetNonDefinedValue));
            else if (propVal is IOptionalItemSet && !((IOptionalItemSet)propVal).Initialized)
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetNonDefinedValue));
            }
            else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            //deal with undefined types (nullables)
            {
                var complexType = propVal as IExpressComplexType;
                if (complexType != null)
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.BeginList));
                    foreach (var compVal in complexType.Properties)
                        WriteProperty(compVal.GetType(), compVal, entityWriter, metadata);
                    entityWriter.Write(Convert.ToByte(P21ParseAction.EndList));
                }
                else if ((propVal is IExpressValueType))
                {
                    var expressVal = (IExpressValueType)propVal;
                    WriteValueType(expressVal.UnderlyingSystemType, expressVal.Value, entityWriter);
                }
                else
                {
                    WriteValueType(propVal.GetType(), propVal, entityWriter);
                }
            }
            else if (typeof(IExpressComplexType).IsAssignableFrom(propType))
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.BeginList));
                foreach (var compVal in ((IExpressComplexType)propVal).Properties)
                    WriteProperty(compVal.GetType(), compVal, entityWriter, metadata);
                entityWriter.Write(Convert.ToByte(P21ParseAction.EndList));
            }
            else if (typeof(IExpressValueType).IsAssignableFrom(propType))
            //value types with a single property (IfcLabel, IfcInteger)
            {
                var realType = propVal.GetType();
                if (realType != propType)
                //we have a type but it is a select type use the actual value but write out explicitly
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.BeginNestedType));
                    entityWriter.Write(realType.Name.ToUpper());
                    entityWriter.Write(Convert.ToByte(P21ParseAction.BeginList));
                    WriteProperty(realType, propVal, entityWriter, metadata);
                    entityWriter.Write(Convert.ToByte(P21ParseAction.EndList));
                    entityWriter.Write(Convert.ToByte(P21ParseAction.EndNestedType));
                }
                else //need to write out underlying property value
                {
                    var expressVal = (IExpressValueType)propVal;
                    WriteValueType(expressVal.UnderlyingSystemType, expressVal.Value, entityWriter);
                }
            }
            else if (typeof(IExpressEnumerable).IsAssignableFrom(propType) &&
                     (itemType = propType.GetItemTypeFromGenericType()) != null)
            //only process lists that are real lists, see cartesianpoint
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.BeginList));
                foreach (var item in ((IExpressEnumerable)propVal))
                    WriteProperty(itemType, item, entityWriter, metadata);
                entityWriter.Write(Convert.ToByte(P21ParseAction.EndList));
            }
            else if (typeof(IPersistEntity).IsAssignableFrom(propType))
            //all writable entities must support this interface and ExpressType have been handled so only entities left
            {
                var val = ((IPersistEntity)propVal).EntityLabel;
                if (val <= UInt16.MaxValue)
                {
                    entityWriter.Write((byte)P21ParseAction.SetObjectValueUInt16);
                    entityWriter.Write(Convert.ToUInt16(val));
                }
                else if (val <= Int32.MaxValue)
                {
                    entityWriter.Write((byte)P21ParseAction.SetObjectValueInt32);
                    entityWriter.Write(Convert.ToInt32(val));
                }
                //else if (val <= Int64.MaxValue)
                //{
                //    //This is a very large model and it is unlikely we will be able to handle this number of entities,
                //    //it is possible they have just created big labels and it needs to be renumbered
                //    //Entity Label could be redfined as a long bu this is a large overhead if never required, let's see...
                //    throw new XbimException("Entity Label is Init64, this is not currently supported");
                //    //entityWriter.Write((byte)P21ParseAction.SetObjectValueInt64);
                //    //entityWriter.Write(val);
                //}
                else
                    throw new Exception("Entity Label exceeds maximim value for a an int32 long number");
            }
            else if (propType.IsValueType || propType == typeof(string) || propType == typeof(byte[])) //it might be an in-built value type double, string etc
            {
                WriteValueType(propVal.GetType(), propVal, entityWriter);
            }
            else if (typeof(IExpressSelectType).IsAssignableFrom(propType))
            // a select type get the type of the actual value
            {
                if (propVal.GetType().IsValueType) //we have a value type, so write out explicitly
                {
                    var type = metadata.ExpressType(propVal.GetType());
                    entityWriter.Write(Convert.ToByte(P21ParseAction.BeginNestedType));
                    entityWriter.Write(type.ExpressNameUpper);
                    entityWriter.Write(Convert.ToByte(P21ParseAction.BeginList));
                    WriteProperty(propVal.GetType(), propVal, entityWriter, metadata);
                    entityWriter.Write(Convert.ToByte(P21ParseAction.EndList));
                    entityWriter.Write(Convert.ToByte(P21ParseAction.EndNestedType));
                }
                else //could be anything so re-evaluate actual type
                {
                    WriteProperty(propVal.GetType(), propVal, entityWriter, metadata);
                }
            }
            else
                throw new Exception(string.Format("Entity  has illegal property {0} of type {1}",
                    propType.Name, propType.Name));
        }

        private static void WriteValueType(Type pInfoType, object pVal, BinaryWriter entityWriter)
        {
            if (pInfoType == typeof(Double))
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetFloatValue));
                entityWriter.Write((double)pVal);
            }
            else if (pInfoType == typeof(String)) //convert  string
            {
                if (pVal == null)
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetNonDefinedValue));
                else
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetStringValue));
                    entityWriter.Write((string)pVal);
                }
            }
            else if (pInfoType == typeof(Int16))
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetIntegerValue));
                entityWriter.Write((long)(Int16)pVal);
            }
            else if (pInfoType == typeof(Int32))
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetIntegerValue));
                entityWriter.Write((long)(Int32)pVal);
            }
            else if (pInfoType == typeof(Int64))
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetIntegerValue));
                entityWriter.Write((long)pVal);
            }
            else if (pInfoType.IsEnum) //convert enum
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetEnumValue));
                entityWriter.Write(pVal.ToString().ToUpper());
            }
            else if (pInfoType == typeof(bool))
            {
                if (pVal == null) //we have a logical
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetNonDefinedValue));
                }
                else
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetBooleanValue));
                    entityWriter.Write((bool)pVal);
                }
            }

            else if (pInfoType == typeof(DateTime)) //convert  TimeStamp
            {
                var ts = ((DateTime)pVal).ToStep21();
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetIntegerValue));
                entityWriter.Write(ts);
            }
            else if (pInfoType == typeof(Guid)) //convert  Guid string
            {
                if (pVal == null)
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetNonDefinedValue));
                else
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetStringValue));
                    entityWriter.Write((string)pVal);
                }
            }
            else if (pInfoType == typeof(bool?)) //convert  logical
            {
                var b = (bool?)pVal;
                if (!b.HasValue)
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetNonDefinedValue));
                else
                {
                    entityWriter.Write(Convert.ToByte(P21ParseAction.SetBooleanValue));
                    entityWriter.Write(b.Value);
                }
            }
            else if (pInfoType == typeof(byte[])) //convert  binary data represented by byte array
            {
                entityWriter.Write(Convert.ToByte(P21ParseAction.SetHexValue));
                if (pVal == null)
                {
                    entityWriter.Write((Int32)0);
                }
                else
                {
                    var bytes = (byte[])pVal;
                    entityWriter.Write(bytes.Length);
                    if (bytes.Length > 0)
                        entityWriter.Write(bytes);
                }

            }
            else
                throw new ArgumentException(string.Format("Invalid Value Type {0}", pInfoType.Name), "pInfoType");
        }

        /// <summary>
        ///   Writes the in memory data of the entity to a stream
        /// </summary>
        /// <param name = "entityStream"></param>
        /// <param name = "entityWriter"></param>
        /// <param name = "item"></param>
        /// <param name="metadata"></param>
        private static int WriteEntityToSteam(MemoryStream entityStream, BinaryWriter entityWriter, IPersistEntity item, ExpressMetaData metadata)
        {
            entityWriter.Seek(0, SeekOrigin.Begin);
            entityWriter.Write(0);
            item.WriteEntity(entityWriter, metadata);
            var len = Convert.ToInt32(entityStream.Position);
            entityWriter.Seek(0, SeekOrigin.Begin);
            entityWriter.Write(len);
            entityWriter.Seek(0, SeekOrigin.Begin);
            return len;
        }
        #endregion

        #region Functions to read property data
        /// <summary>
        /// Populates an entites properties from the binary stream
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cache"></param>
        /// <param name="br"></param>
        /// <param name="unCached">If true instances inside the properties are not added to the cache</param>
        /// <param name="fromCache"> If true the instance is read from the cache if not present it is created, used during parsing</param>
        internal static void ReadEntityProperties(this IPersistEntity entity, PersistedEntityInstanceCache cache, BinaryReader br, bool unCached = false, bool fromCache = false)
        {
            var action = (P21ParseAction)br.ReadByte();

            var parserState = new XbimParserState(entity);
            while (action != P21ParseAction.EndEntity)
            {
                switch (action)
                {
                    case P21ParseAction.BeginList:
                        parserState.BeginList();
                        break;
                    case P21ParseAction.EndList:
                        parserState.EndList();
                        break;
                    case P21ParseAction.BeginComplex:
                        break;
                    case P21ParseAction.EndComplex:
                        break;
                    case P21ParseAction.SetIntegerValue:
                        parserState.SetIntegerValue(br.ReadInt64());
                        break;
                    case P21ParseAction.SetHexValue:
                        var length = br.ReadInt32();
                        if (length == 0)
                        {
                            parserState.SetHexValue(new byte[0]);
                        }
                        else
                        {
                            parserState.SetHexValue(br.ReadBytes(length));
                        }
                        break;
                    case P21ParseAction.SetFloatValue:
                        parserState.SetFloatValue(br.ReadDouble());
                        break;
                    case P21ParseAction.SetStringValue:
                        parserState.SetStringValue(br.ReadString());
                        break;
                    case P21ParseAction.SetEnumValue:
                        parserState.SetEnumValue(br.ReadString());
                        break;
                    case P21ParseAction.SetBooleanValue:
                        parserState.SetBooleanValue(br.ReadBoolean());
                        break;
                    case P21ParseAction.SetNonDefinedValue:
                        parserState.SetNonDefinedValue();
                        break;
                    case P21ParseAction.SetOverrideValue:
                        parserState.SetOverrideValue();
                        break;
                    case P21ParseAction.SetObjectValueUInt16:
                        if (fromCache)
                        {
                            int label = br.ReadUInt16();
                            IPersistEntity refEntity;
                            if (!parserState.InList && cache.Read.TryGetValue(label, out refEntity)) //if we are in a list then make a forward reference anyway to make sure we maintain list order
                                parserState.SetObjectValue(refEntity);
                            else
                            {
                                cache.AddForwardReference(new StepForwardReference(label, parserState.CurrentPropertyId, entity, parserState.NestedIndex));
                                parserState.SkipProperty();
                            }
                        }
                        else
                            parserState.SetObjectValue(cache.GetInstance(br.ReadUInt16(), false, unCached));
                        break;
                    case P21ParseAction.SetObjectValueInt32:
                        if (fromCache)
                        {
                            var label = br.ReadInt32();
                            IPersistEntity refEntity;
                            if (!parserState.InList && cache.Read.TryGetValue(label, out refEntity)) //if we are in a list then make a forward reference anyway to make sure we maintain list order
                                parserState.SetObjectValue(refEntity);
                            else
                            {
                                cache.AddForwardReference(new StepForwardReference(label, parserState.CurrentPropertyId, entity, parserState.NestedIndex));
                                parserState.SkipProperty();
                            }
                        }
                        else
                            parserState.SetObjectValue(cache.GetInstance(br.ReadInt32(), false, unCached));
                        break;
                    case P21ParseAction.SetObjectValueInt64:
                        throw new XbimException("Entity Label is int64, this is not currently supported");
                    //parserState.SetObjectValue(cache.GetInstance(br.ReadInt64(), false, unCached));
                    //break;
                    case P21ParseAction.BeginNestedType:
                        parserState.BeginNestedType(br.ReadString());
                        break;
                    case P21ParseAction.EndNestedType:
                        parserState.EndNestedType();
                        break;
                    case P21ParseAction.EndEntity:
                        parserState.EndEntity();
                        break;
                    case P21ParseAction.NewEntity:
                        parserState = new XbimParserState(entity);
                        break;
                    default:
                        throw new XbimException("Invalid Property Record #" + entity.EntityLabel + " EntityType: " + entity.GetType().Name);
                }
                action = (P21ParseAction)br.ReadByte();
            }
        }

        #endregion


    }
}
