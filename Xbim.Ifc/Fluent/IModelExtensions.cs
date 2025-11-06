using System;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
#nullable enable
    public static class IModelExtensions
    {
        /// <summary>
        /// Adds a filename to the STEP headers
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IModel AddHeaders(this IModel model, string? filename)
        {
            return model.AddHeaders(
                f =>
                {
                    f.Name = filename;
                },
                fd => { }
               );
        }

        /// <summary>
        /// Adds STEP Headers to the <paramref name="model"/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileNameAction"></param>
        /// <param name="fileDescriptionAction"></param>
        /// <returns></returns>
        public static IModel AddHeaders(this IModel model, Action<IStepFileName> fileNameAction, Action<IStepFileDescription> fileDescriptionAction)
        {
            fileDescriptionAction(model.Header.FileDescription);
            fileNameAction(model.Header.FileName);
            return model;
        }

        public static EntityCreator Build(this IModel model)
        {
            return new EntityCreator(model);
        }

        /// <summary>
        /// Set attributes on an entity where the attribute setter value is non-null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        public static T SetAttributes<T>(this T entity, Func<EntityDefaults, EntityDefaults> setter) where T : IIfcRoot
        {
            setter ??= (x) => x;
            var entityDefaults = setter(new EntityDefaults());

            return entity.SetAttributes(entityDefaults);
        }

        /// <summary>
        /// Set attributes on an entity where the attribute setter value is non-null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        public static T SetAttributes<T>(this T entity, EntityDefaults? setter = default) where T : IIfcRoot
        {
            setter ??= new EntityDefaults();
            if(setter.GlobalId != Guid.Empty)
                entity.SetGlobalId(setter.GlobalId);
            if(string.IsNullOrEmpty(setter.Name) == false)
                entity.Name = setter.Name;
            if (string.IsNullOrEmpty(setter.Description) == false)
                entity.Description = setter.Description;
            if(setter.OwnerHistory != null)
                entity.OwnerHistory ??= setter.OwnerHistory;
            if(string.IsNullOrEmpty(setter.PredefinedType) == false)
                SetPredefinedTypeValue(entity, setter);
            return entity;
        }

        /// <summary>
        /// Applies initial default attributes to an entity
        /// </summary>
        /// <remarks>PredefinedType is always set</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static T WithDefaults<T>(this T entity, Func<EntityDefaults, EntityDefaults> initAction) where T : IIfcRoot
        {
            initAction ??= (x) => x;
            var entityDefaults = initAction(new EntityDefaults());

            return entity.WithDefaults(entityDefaults);
        }

        /// <summary>
        /// Applies initial default attributes to an entity
        /// </summary>
        /// <remarks>PredefinedType is always set</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static T WithDefaults<T>(this T entity, EntityDefaults? init = default) where T : IIfcRoot
        {
            init ??= new EntityDefaults();
            if(entity.GlobalId == null)
                entity.SetGlobalId(init.GlobalId);
            entity.Name ??= init.Name;
            entity.Description ??= init.Description;
            entity.OwnerHistory ??= init.OwnerHistory;
            SetPredefinedTypeValue(entity, init);   // Always set because non-null enums have a default value.
            return entity;
        }

        /// <summary>
        /// Creates a PropertySet associated with this entity, if not already existing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertySet"></param>
        /// <returns></returns>
        public static T WithPropertySet<T>(this T entity, string propertySet) where T : IIfcObjectDefinition
        {
            if(entity is IIfcObject obj)
            {
                var pset = obj.GetPropertySet(propertySet);
                pset ??= entity.Model.Build().PropertySet(o => o.Name = propertySet);
                obj.AddPropertySet(pset);
            }
            else if(entity is IIfcTypeObject type)
            {
                var pset = type.GetPropertySet(propertySet);
                pset ??= entity.Model.Build().PropertySet(o => o.Name = propertySet);
                type.AddPropertySet(pset);
            }
            return entity;
        }

        /// <summary>
        /// Creates a PropertySingleValue for an entity in the given PropertySet with the supplied Name and Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertySet"></param>
        /// <param name="propertyName"></param>
        /// <param name="ifcValue"></param>
        /// <returns></returns>
        public static T WithPropertySingle<T>(this T entity, string propertySet, string propertyName, IIfcValue ifcValue) where T : IIfcObjectDefinition
        {
            var type = ifcValue.GetType();
            entity.SetPropertySingleValue(propertySet, propertyName, type).NominalValue = ifcValue;

            return entity;
        }

        /// <summary>
        /// Creates a PropertyEnumeratedValue for an entity in the given PropertySet with the supplied Name and Values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertySet"></param>
        /// <param name="propertyName"></param>
        /// <param name="ifcValues"></param>
        /// <returns></returns>
        public static T WithEnumeratedProperty<T, V>(this T entity, string propertySet, string propertyName, V[] ifcValues) 
            where T : IIfcObject 
            where V : IIfcValue
        {
    
            var prop = entity.SetPropertyEnumeratedValue(propertySet, propertyName, ifcValues);
           
            return entity;
        }

        /// <summary>
        /// Creates a PropertyEnumeratedValue for an entity in the given PropertySet with the supplied Name and Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertySet"></param>
        /// <param name="propertyName"></param>
        /// <param name="lowerValue"></param>
        /// <param name="upperValue"></param>
        /// <param name="setPointVaue"></param>
        /// <returns></returns>
        public static T WithBoundedProperty<T, V>(this T entity, string propertySet, string propertyName, V lowerValue, V upperValue, V? setPointVaue)
            where T : IIfcObject
            where V : IIfcValue
        {
            entity.SetPropertyBoundedValue(propertySet, propertyName, lowerValue, upperValue, setPointVaue);

            return entity;
        }


        /// <summary>
        /// Creates a PropertyListValue for an entity in the given PropertySet with the supplied Name and Values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertySet"></param>
        /// <param name="propertyName"></param>
        /// <param name="ifcValues"></param>
        /// <returns></returns>
        public static T WithListValueProperty<T, V>(this T entity, string propertySet, string propertyName, V[] ifcValues)
            where T : IIfcObject
            where V : IIfcValue
        {

            var prop = entity.SetPropertyListValue(propertySet, propertyName, ifcValues);

            return entity;
        }

        /// <summary>
        /// Creates Physical Simple Quantity for an entity in the given PropertySet with the supplied Name, Value and Unit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertySet"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="quantityType"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static T WithQuantity<T>(this T entity, string propertySet, string propertyName, double value, XbimQuantityTypeEnum quantityType, IIfcNamedUnit unit)
            where T : IIfcObject
        {

            entity.SetElementPhysicalSimpleQuantity(propertySet, propertyName, value, quantityType, unit);

            return entity;
        }



        private static void SetPredefinedTypeValue<T>(T entity, EntityDefaults init) where T : IIfcRoot
        {
            if (entity is IIfcObjectDefinition obj)
            {
                if (string.IsNullOrEmpty(init.PredefinedType))
                {
                    // Set Undefined since the default enum is arbitrary across schemas
                    // See https://github.com/xBimTeam/XbimEssentials/issues/576
                    obj.SetPredefinedTypeValue("NOTDEFINED");
                }
                else
                {

                    if (obj.IsPredefinedTypeEnum(init.PredefinedType))
                    {
                        // Built in standard PDT
                        obj.SetPredefinedTypeValue(init.PredefinedType);
                    }
                    else
                    {
                        // User defined PDT where the actual value is stored in ObjectType/ElementType etc
                        if (obj is IIfcObject o)
                        {
                            o.ObjectType = init.PredefinedType;
                        }
                        else if (obj is IIfcElementType t)
                        {
                            t.ElementType = init.PredefinedType;
                        }
                        else if (obj is IIfcTypeProcess p)
                        {
                            p.ProcessType = init.PredefinedType;
                        }
                        obj.SetPredefinedTypeValue("USERDEFINED");
                    }
                }
            }
        }
        private static void SetGlobalId(this IIfcRoot entity, Guid? guid = default)
        {
            guid ??= Guid.NewGuid();
            entity.GlobalId = Xbim.Ifc2x3.UtilityResource.IfcGloballyUniqueId.ConvertToBase64(guid.Value);
        }


    }
}
