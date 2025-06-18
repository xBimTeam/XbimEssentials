using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xbim.Common.Exceptions;
using Xbim.Common.Step21;

namespace Xbim.Common.Metadata
{
    [DebuggerDisplay("Name = {Name}, Type = {PropertyInfo.PropertyType.Name}")]
    public class ExpressMetaProperty
    {
        public PropertyInfo PropertyInfo { get; internal set; }
        public EntityAttributeAttribute EntityAttribute { get; internal set; }
        public InverseProperty InverseAttributeProperty { get; internal set; }
        public Type EnumerableType { get; internal set; }
        public string Name { get { return PropertyInfo.Name; } }
        public bool IsInverse { get { return EntityAttribute.Order < 0; } }
        public bool IsDerived { get { return EntityAttribute.State == EntityAttributeState.Derived; } }
        public bool IsExplicit { get { return EntityAttribute.Order > 0; } }
        public bool IsIndexed { get; internal set; }
    }

    /// <summary>
    ///   A collection of IPersistEntity instances, optimised for EXPRESS models
    /// </summary>

    public class ExpressMetaData
    {
        /// <summary>
        /// Module for which this meta data structure is created
        /// </summary>
        public readonly Module Module;

        /// <summary>
        /// Look up for the if of an entity that returns the ExpresType
        /// </summary>
        private readonly Dictionary<short, ExpressType> _typeIdToExpressTypeLookup;
        /// <summary>
        /// Look up the entity Type and return the ExpressType
        /// </summary>
        private readonly ExpressTypeDictionary _typeToExpressTypeLookup;
        /// <summary>
        /// Look up the name of an entity and return the ExpressType
        /// </summary>
        private readonly Dictionary<string, ExpressType> _typeNameToExpressTypeLookup;
        /// <summary>
        /// Look up the name of an entity and return the ExpressType
        /// </summary>
        private readonly Dictionary<string, ExpressType> _persistNameToExpressTypeLookup;
        /// <summary>
        /// Look up ExpressTypes implementing an interface
        /// </summary>
        private readonly Dictionary<Type, List<ExpressType>> _interfaceToExpressTypesLookup;
        /// <summary>
        /// Static cache to avoid multiple creation of the structure
        /// </summary>
        private static readonly ConcurrentDictionary<Module, ExpressMetaData> ModuleCache = new ConcurrentDictionary<Module, ExpressMetaData>();

        /// <summary>
        /// Static cache per schema to avoid multiple instances of the metadata
        /// </summary>
        private static readonly ConcurrentDictionary<XbimSchemaVersion, ExpressMetaData> SchemaCache = new ConcurrentDictionary<XbimSchemaVersion, ExpressMetaData>();

        /// <summary>
        /// This method creates metadata model for a specified module based on reflection and custom attributes.
        /// It only creates ExpressMetaData once for any module. If it already exists it is retrieved from a 
        /// static cache. However, for a performance reasons try to minimize this and rather keep a single instance
        /// reference for your code.
        /// </summary>
        /// <remarks>Prefer <c>GetMetadata(IEntityFactory)</c> overload which permits more control over schema enumeration when assembly 
        /// hosts multiple schemas</remarks>
        /// <param name="module">Assembly module which contains single schema model</param>
        /// <returns>Meta data structure for the schema defined within the module</returns>
        [Obsolete("Prefer ExpressMetaData GetMetadata(IEntityFactory) overload")]
        public static ExpressMetaData GetMetadata(Module module)
        {
            return ModuleCache.GetOrAdd(module, m => new ExpressMetaData(m));
        }

        /// <summary>
        /// Creates an <see cref="ExpressMetaData"/> metadata model for a specified <see cref="IEntityFactory"/> based on reflection and custom attributes.
        /// An ExpressMetaData is creted only once for any EntityFactory.
        /// </summary>
        /// <remarks>Only types sharing the same namespace as the <paramref name="entityFactory"/> are included. This makes it possible
        /// to share multiple schemas in a single Assembly / Module</remarks>
        /// <param name="entityFactory"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ExpressMetaData GetMetadata(IEntityFactory entityFactory)
        {
            if (entityFactory is null)
            {
                throw new ArgumentNullException(nameof(entityFactory));
            }

            return SchemaCache.GetOrAdd(entityFactory.SchemaVersion, f => new ExpressMetaData(entityFactory));
        }

        private ExpressMetaData(Module module)
        {
            Module = module;
            var typesToProcess =
                module.GetTypes().Where(
                    t => IsExpressTypeForSchema(t)).ToList();

            _typeIdToExpressTypeLookup = new Dictionary<short, ExpressType>(typesToProcess.Count);
            _typeNameToExpressTypeLookup = new Dictionary<string, ExpressType>(typesToProcess.Count);
            _persistNameToExpressTypeLookup = new Dictionary<string, ExpressType>(typesToProcess.Count);
            _typeToExpressTypeLookup = new ExpressTypeDictionary();
            _interfaceToExpressTypesLookup = new Dictionary<Type, List<ExpressType>>();

            RegisterTypes(typesToProcess);
        }

        private ExpressMetaData(IEntityFactory factory)
        {
            Module = factory.GetType().Module;
            var schemaNamespace = factory.GetType().Namespace;
            var typesToProcess =
                Module.GetTypes().Where(
                    t => IsExpressTypeForSchema(t, schemaNamespace)).ToList();

            _typeIdToExpressTypeLookup = new Dictionary<short, ExpressType>(typesToProcess.Count);
            _typeNameToExpressTypeLookup = new Dictionary<string, ExpressType>(typesToProcess.Count);
            _persistNameToExpressTypeLookup = new Dictionary<string, ExpressType>(typesToProcess.Count);
            _typeToExpressTypeLookup = new ExpressTypeDictionary();
            _interfaceToExpressTypesLookup = new Dictionary<Type, List<ExpressType>>();

            RegisterTypes(typesToProcess);
        }
        
        internal static bool IsExpressTypeForSchema(Type type, string schemaNamespace = "")
        {
            if (type == null || type.Namespace == null)
                return false;

            if(string.IsNullOrWhiteSpace(schemaNamespace))
                return IsExpressType(type);

            string typeNs = type.Namespace;

            if (!typeNs.StartsWith(schemaNamespace, StringComparison.Ordinal)) 
                return false;

            if (typeNs.Length == schemaNamespace.Length)
                return IsExpressType(type);

            // bounds-checked index access, check if the next char is dot 
            if (typeNs.Length > schemaNamespace.Length && typeNs[schemaNamespace.Length] == '.')
                return IsExpressType(type);

            return false;
        }

       
        private static bool IsExpressType(Type type)
        {
            if (!typeof(IPersist).IsAssignableFrom(type)) 
                return false;
            
            if (!type.IsPublic || type.IsEnum || type.IsInterface)
                return false;

            if (!Attribute.IsDefined(type, typeof(ExpressTypeAttribute), inherit: false))
                return false;

            if (typeof(IExpressHeaderType).IsAssignableFrom(type)) return false;

            return true;
        }

        private void RegisterTypes(IList<Type> typesToProcess)
        {
            // System.Diagnostics.Debug.Write(typesToProcess.Count());
            foreach (var typeToProcess in typesToProcess)
            {
                try
                {
                    // Debug.WriteLine(typeToProcess.ToString());
                    ExpressType expressTypeToProcess;
                    if (!_typeToExpressTypeLookup.TryGetValue(typeToProcess, out expressTypeToProcess))
                        expressTypeToProcess = new ExpressType(typeToProcess);

                    var typeLookup = typeToProcess.Name.ToUpperInvariant();
                    if (!_typeNameToExpressTypeLookup.ContainsKey(typeLookup))
                        _typeNameToExpressTypeLookup.Add(typeLookup, expressTypeToProcess);

                    if (typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(typeToProcess))
                    {
                        _persistNameToExpressTypeLookup.Add(expressTypeToProcess.ExpressNameUpper, expressTypeToProcess);
                        _typeIdToExpressTypeLookup.Add(expressTypeToProcess.TypeId, expressTypeToProcess);
                    }

                    if (!_typeToExpressTypeLookup.ContainsKey(expressTypeToProcess.Type))
                    {
                        _typeToExpressTypeLookup.Add(expressTypeToProcess.Type, expressTypeToProcess);
                        AddParent(expressTypeToProcess);
                    }

                    // populate the dictionary lookup by interface
                    foreach (var interfaceFound in typeToProcess.GetTypeInfo().GetInterfaces())
                    {
                        if (interfaceFound.Namespace != null && !interfaceFound.Namespace.StartsWith("Xbim"))
                            continue;
                        if (!_interfaceToExpressTypesLookup.ContainsKey(interfaceFound))
                        {
                            // add to dictionary
                            _interfaceToExpressTypesLookup.Add(interfaceFound, new List<ExpressType>());
                        }
                        _interfaceToExpressTypesLookup[interfaceFound].Add(expressTypeToProcess);
                    }
                }
                catch (Exception e)
                {
                    throw new XbimException($"Error reading Entity Meta Data for entity {typeToProcess.FullName}", e);
                }
            }
        }

        internal void AddParent(ExpressType child)
        {
            var baseParent = child.Type.GetTypeInfo().BaseType;
            if (baseParent == null || typeof(object) == baseParent || typeof(ValueType) == baseParent || typeof(PersistEntity) == baseParent)
                return;
            ExpressType expressParent;
            if (!_typeToExpressTypeLookup.ContainsKey(baseParent))
            {
                _typeToExpressTypeLookup.Add(baseParent, expressParent = new ExpressType(baseParent));
                var typeLookup = baseParent.Name.ToUpperInvariant();
                if (!_typeNameToExpressTypeLookup.ContainsKey(typeLookup))
                    _typeNameToExpressTypeLookup.Add(typeLookup, expressParent);
                expressParent.SubTypes.Add(child);
                child.SuperType = expressParent;
                AddParent(expressParent);
            }
            else
            {
                expressParent = _typeToExpressTypeLookup[baseParent];
                child.SuperType = expressParent;
                if (!expressParent.SubTypes.Contains(child))
                    expressParent.SubTypes.Add(child);
            }
        }

        public IEnumerable<ExpressType> Types()
        {
            return _typeNameToExpressTypeLookup.Values;
        }

        /// <summary>
        /// Returns the ExpressType with the specified name (name of type or express name)
        /// </summary>
        /// <param name="typeName">The name of the type in uppercase (either type name or persistance name. 
        /// These are not necesarilly the same)</param>
        /// <returns>The foud type (or Null if not found)</returns>
        public ExpressType ExpressType(string typeName)
        {
            ExpressType result;
            if (_typeNameToExpressTypeLookup.TryGetValue(typeName, out result))
                return result;
            if (_persistNameToExpressTypeLookup.TryGetValue(typeName, out result))
                return result;
            return null;
        }

        public IEnumerable<ExpressType> ExpressTypesImplementing(Type type)
        {
            List<ExpressType> result;
            if (_interfaceToExpressTypesLookup.TryGetValue(type, out result))
                return result;
            return Enumerable.Empty<ExpressType>();
        }

        public IEnumerable<ExpressType> TypesImplementing(Type type)
        {
            List<ExpressType> result;
            return _interfaceToExpressTypesLookup.TryGetValue(type, out result) ?
                result :
                Enumerable.Empty<ExpressType>();
        }

        public IEnumerable<ExpressType> TypesImplementing(string stringType)
        {
            var exprType = ExpressType(stringType);
            if (exprType == null) return Enumerable.Empty<ExpressType>();

            List<ExpressType> result;
            return _interfaceToExpressTypesLookup.TryGetValue(exprType.Type, out result) ?
                result :
                Enumerable.Empty<ExpressType>();
        }

        public IEnumerable<short> NonAbstractSubTypes(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                List<ExpressType> result;
                return _interfaceToExpressTypesLookup.TryGetValue(type, out result)
                    ? result.Where(t => !t.Type.GetTypeInfo().IsAbstract).Select(t => t.TypeId)
                    : Enumerable.Empty<short>();
            }

            var eType = ExpressType(type);
            return eType == null ?
                Enumerable.Empty<short>() :
                eType.NonAbstractSubTypes.Select(t => t.TypeId);
        }

        /// <summary>
        /// Returns the ExpressType with the specified type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The foud type (or Null if not found)</returns>
        public ExpressType ExpressType(Type type)
        {
            ExpressType result;
            return _typeToExpressTypeLookup.TryGetValue(type, out result) ? result : null;
        }

        /// <summary>
        /// returns the ExpressType corresponding to the TypeId
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public ExpressType ExpressType(short typeId)
        {
            return _typeIdToExpressTypeLookup[typeId];
        }

        /// <summary>
        /// returns the express type id of the type, if the type is not an entity and excpetion will be thrown
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public short ExpressTypeId(Type type)
        {
            return _typeToExpressTypeLookup[type].TypeId;
        }

        /// <summary>
        /// Returns the typeId for the named type
        /// </summary>
        /// <param name="typeName">the name of the type, this is in uppercase</param>
        /// <returns></returns>
        public short ExpressTypeId(string typeName)
        {
            return ExpressType(typeName).TypeId;
        }

        public short ExpressTypeId(IPersist entity)
        {
            return _typeToExpressTypeLookup[entity.GetType()].TypeId;
        }

        /// <summary>
        /// Returns the Type of the Entity with typeId
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public Type GetType(short typeId)
        {
            return ExpressType(typeId).Type;
        }

        /// <summary>
        /// Returns the ExpressType of the specified entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ExpressType ExpressType(IPersist entity)
        {
            return ExpressType(entity.GetType());
        }


        /// <summary>
        /// Trys to get the specified Type with the typeName, if the ExpressType does not exist false is returned
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="expressType"></param>
        /// <returns></returns>
        public bool TryGetExpressType(string typeName, out ExpressType expressType)
        {
            return _typeNameToExpressTypeLookup.TryGetValue(typeName, out expressType) ||
                _persistNameToExpressTypeLookup.TryGetValue(typeName, out expressType);
        }

        /// <summary>
        /// Returns true if the named entities attribute is indexed
        /// </summary>
        /// <param name="entityTypeName">the name of the Entity</param>
        /// <param name="attributeIndex">the index offset of the attribute to check, nb this is a 1 based index</param>
        /// <returns></returns>
        public bool IsIndexedEntityAttribute(string entityTypeName, int attributeIndex)
        {
            var type = ExpressType(entityTypeName);
            return type.IsIndexedAttribute(attributeIndex);
        }
    }
}