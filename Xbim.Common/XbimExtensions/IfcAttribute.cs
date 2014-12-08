#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAttribute.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;

#endregion

namespace Xbim.XbimExtensions
{
    /// <summary>
    ///   The scope of the IFC attribute
    /// </summary>
    public enum IfcAttributeState
    {
        None = 0,
        Optional = 1,
        Mandatory,
        Derived,
        DerivedOverride
    }

    public enum IfcAttributeType
    {
        None = 0,
        Class = 1,
        Set,
        Enum,
        List,
        ListUnique = 100
    }


    public enum IfcEntityType
    {
        None = 0,
        SimpleValue = 1,
        MeasureValue,
        DerivedMeasureValue,
    }

    /// <summary>
    /// Defines that this property can be used to create an identity for the object, but it is not on its own an identity for the object
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,Inherited=true, AllowMultiple = true)]
    public sealed class IdentityComponent : Attribute
    {
    }

    /// <summary>
    /// Defines that this property is an identity property of the obejct, typically a guid or key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class IdentityProperty : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property,Inherited=true, AllowMultiple = false)]
    public sealed class IndexedProperty : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class IfcPersistedEntityAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class IndexedClass : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IfcAttribute : Attribute
    {
        private readonly IfcAttributeState _state;
        private readonly IfcAttributeType _ifcType;
        private readonly int _order;
        private readonly int _maxCardinality = -1;
        private readonly int _minCardinality = -1;
        private readonly IfcAttributeType _memberType = IfcAttributeType.Class;

        public IfcAttributeState State
        {
            get { return _state; }
        }

        public IfcAttributeType IfcType
        {
            get { return _ifcType; }
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

        public IfcAttributeType MemberType
        {
            get { return _memberType; }
        }

        public IfcAttribute()
        {
        }

        public IfcAttribute(int order, IfcAttributeState state)
        {
            _state = state;
            _order = order;
            _ifcType = IfcAttributeType.Class;
        }

        public IfcAttribute(int order, IfcAttributeState state, IfcAttributeType ifcType, IfcAttributeType memberType,
                            int minCardinality, int maxCardinality)
        {
            _state = state;
            _order = order;
            _ifcType = ifcType;
            _memberType = memberType;
            _minCardinality = minCardinality;
            _maxCardinality = maxCardinality;
        }

        public IfcAttribute(int order, IfcAttributeState state, IfcAttributeType ifcType, int minCardinality,
                            int maxCardinality)
        {
            _state = state;
            _order = order;
            _ifcType = ifcType;
            _minCardinality = minCardinality;
            _maxCardinality = maxCardinality;
        }

        public IfcAttribute(int order, IfcAttributeState state, IfcAttributeType ifcType, int minCardinality)
        {
            _state = state;
            _order = order;
            _ifcType = ifcType;
            _minCardinality = minCardinality;
        }

        public IfcAttribute(int order, IfcAttributeState state, IfcAttributeType ifcType, IfcAttributeType memberType,
                            int minCardinality)
        {
            _state = state;
            _order = order;
            _ifcType = ifcType;
            _memberType = memberType;
            _minCardinality = minCardinality;
        }

        public IfcAttribute(int order, IfcAttributeState state, IfcAttributeType ifcType)
        {
            _state = state;
            _order = order;
            _ifcType = ifcType;
        }

        public IfcAttribute(int order, IfcAttributeState state, IfcAttributeType ifcType, IfcAttributeType memberType)
        {
            _state = state;
            _order = order;
            _ifcType = ifcType;
            _memberType = memberType;
        }

        public bool IsEnumerable
        {
            get { return (_ifcType == IfcAttributeType.List || _ifcType == IfcAttributeType.Set); }
        }

        public string ListType
        {
            get
            {
                switch (_ifcType)
                {
                    case IfcAttributeType.Set:
                        return "set";
                    case IfcAttributeType.List:
                        return "list";
                    case IfcAttributeType.ListUnique:
                        return "list-unique";
                    default:
                        return "";
                }
            }
        }

        public bool IsSet
        {
            get { return (_ifcType == IfcAttributeType.Set); }
        }

        public bool IsList
        {
            get { return (_ifcType == IfcAttributeType.List || _ifcType == IfcAttributeType.ListUnique); }
        }

        public bool IsClass
        {
            get { return (_ifcType == IfcAttributeType.Class); }
        }

        public bool IsDerivedOverride
        {
            get { return (_state == IfcAttributeState.DerivedOverride); }
        }

        public bool IsValueType
        {
            get { return (_ifcType > IfcAttributeType.List); }
        }


        public bool IsMemberValueType
        {
            get { return (_memberType > IfcAttributeType.List); }
        }

        public bool IsMemberClass
        {
            get { return (_memberType == IfcAttributeType.Class); }
        }

        public bool IsOptional
        {
            get { return (_state == IfcAttributeState.Optional); }
        }

        public bool IsMandatory
        {
            get { return (_state == IfcAttributeState.Mandatory); }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class IfcEntity : Attribute
    {
        private readonly IfcEntityType _entityType;

        public IfcEntityType EntityType
        {
            get { return _entityType; }
        }

        public IfcEntity(IfcEntityType entityType)
        {
            _entityType = entityType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class IfcWhereRule : Attribute
    {
    }
}