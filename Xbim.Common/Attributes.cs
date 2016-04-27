using System;

namespace Xbim.Common
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class ExpressTypeAttribute : Attribute
    {
        public ExpressTypeAttribute(string name, int id)
        {
            Name = name;
			EntityTypeId = id;
        }

        public string Name { get; private set; }
        public int EntityTypeId { get; private set; }
    }

	/// <summary>
    ///   The scope of the entity attribute
    /// </summary>
    public enum EntityAttributeState
    {
        None = 0,
        Optional = 1,
        Mandatory,
        Derived,
        DerivedOverride
    }

    public enum EntityAttributeType
    {
        None = 0,
        Class = 1,
        Enum,
        Set,
		Bag,
		Array,
		ArrayUnique,
        List,
        ListUnique = 100
    }

    /// <summary>
    /// Indicates that this property plays a role in some inverse relation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IndexedProperty : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InverseProperty : Attribute
    {
        public string RemoteProperty { get; private set; }

        public InverseProperty(string remoteProperty)
        {
            RemoteProperty = remoteProperty;
        }
    }


    [AttributeUsage(AttributeTargets.Struct)]
    public sealed class DefinedTypeAttribute : Attribute
    {
        public Type UnderlyingType { get; private set; }

        public DefinedTypeAttribute(Type underlyingType)
        {
            UnderlyingType = underlyingType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EntityAttributeAttribute : Attribute
    {
        private readonly EntityAttributeState _state;
        private readonly EntityAttributeType _entityType;
        private readonly int _order;
        private readonly int _globalOrder;
        private readonly int _maxCardinality = -1;
        private readonly int _minCardinality = -1;
        private readonly EntityAttributeType _memberType = EntityAttributeType.Class;

        public EntityAttributeState State
        {
            get { return _state; }
        }

        public EntityAttributeType EntityType
        {
            get { return _entityType; }
        }

        public int Order
        {
            get { return _order; }
        }

        public int MinCardinality
        {
            get { return _minCardinality; }
        }

        public int MaxCardinality
        {
            get { return _maxCardinality; }
        }

        public EntityAttributeType MemberType
        {
            get { return _memberType; }
        }

        //public EntityAttributeAttribute()
        //{
        //}

        //public EntityAttributeAttribute(int order, EntityAttributeState state)
        //{
        //    _state = state;
        //    _order = order;
        //    _entityType = EntityAttributeType.Class;
        //}

        public EntityAttributeAttribute(int order, EntityAttributeState state, EntityAttributeType entityType, EntityAttributeType memberType,
                            int minCardinality, int maxCardinality, int globalOrder)
        {
            _state = state;
            _order = order;
            _entityType = entityType;
            _memberType = memberType;
            _minCardinality = minCardinality;
            _maxCardinality = maxCardinality;
            _globalOrder = globalOrder;
        }

        //public EntityAttributeAttribute(int order, EntityAttributeState state, EntityAttributeType entityType, int minCardinality,
        //                    int maxCardinality)
        //{
        //    _state = state;
        //    _order = order;
        //    _entityType = entityType;
        //    _minCardinality = minCardinality;
        //    _maxCardinality = maxCardinality;
        //}

        //public EntityAttributeAttribute(int order, EntityAttributeState state, EntityAttributeType entityType, int minCardinality)
        //{
        //    _state = state;
        //    _order = order;
        //    _entityType = entityType;
        //    _minCardinality = minCardinality;
        //}

        //public EntityAttributeAttribute(int order, EntityAttributeState state, EntityAttributeType entityType, EntityAttributeType memberType,
        //                    int minCardinality)
        //{
        //    _state = state;
        //    _order = order;
        //    _entityType = entityType;
        //    _memberType = memberType;
        //    _minCardinality = minCardinality;
        //}

        //public EntityAttributeAttribute(int order, EntityAttributeState state, EntityAttributeType entityType)
        //{
        //    _state = state;
        //    _order = order;
        //    _entityType = entityType;
        //}

        //public EntityAttributeAttribute(int order, EntityAttributeState state, EntityAttributeType entityType, EntityAttributeType memberType)
        //{
        //    _state = state;
        //    _order = order;
        //    _entityType = entityType;
        //    _memberType = memberType;
        //}

        public bool IsEnumerable
        {
            get { return
                _entityType == EntityAttributeType.Array||
                _entityType == EntityAttributeType.ArrayUnique ||
                _entityType == EntityAttributeType.Bag ||
                _entityType == EntityAttributeType.List ||
                _entityType == EntityAttributeType.ListUnique ||
                _entityType == EntityAttributeType.Set
                ; }
        }

        public string ListType
        {
            get
            {
                switch (_entityType)
                {
                    case EntityAttributeType.Set:
                        return "set";
                    case EntityAttributeType.List:
                        return "list";
                    case EntityAttributeType.ListUnique:
                        return "list-unique";
                    default:
                        return "";
                }
            }
        }

        public bool IsSet
        {
            get { return (_entityType == EntityAttributeType.Set); }
        }

        public bool IsList
        {
            get { return (_entityType == EntityAttributeType.List || _entityType == EntityAttributeType.ListUnique); }
        }

        public bool IsClass
        {
            get { return (_entityType == EntityAttributeType.Class); }
        }

        public bool IsDerivedOverride
        {
            get { return (_state == EntityAttributeState.DerivedOverride); }
        }

		public bool IsDerived
        {
            get { return (_state == EntityAttributeState.DerivedOverride || _state == EntityAttributeState.Derived); }
        }

        public bool IsValueType
        {
            get { return (_entityType > EntityAttributeType.List); }
        }


        public bool IsMemberValueType
        {
            get { return (_memberType > EntityAttributeType.List); }
        }

        public bool IsMemberClass
        {
            get { return (_memberType == EntityAttributeType.Class); }
        }

        public bool IsOptional
        {
            get { return (_state == EntityAttributeState.Optional); }
        }

        public bool IsMandatory
        {
            get { return (_state == EntityAttributeState.Mandatory); }
        }

        public int GlobalOrder
        {
            get { return _globalOrder; }
        }
    }
}