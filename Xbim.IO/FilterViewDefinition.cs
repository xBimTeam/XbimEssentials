#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    FilterViewDefinition.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO;

#endregion

namespace Xbim.IO
{
    [DataContract]
    public class FilterViewDefinition
    {
        private readonly IfcFilterDictionary _parseFilter = new IfcFilterDictionary();
        [DataMember] public IfcFilterDictionary Included = new IfcFilterDictionary();
        public IfcFilterDictionary Excluded = new IfcFilterDictionary();

        public void AddInclude(string ifcEntityName, params int[] propertyIndices)
        {
            IfcType ifcType = IfcInstances.IfcTypeLookup[ifcEntityName];
            IfcFilter flt = new IfcFilter(ifcType, propertyIndices);
            Included.Add(flt);
        }

        public void AddInclude(IfcFilter flt)
        {
            Included.Add(flt);
        }

        public void AddExclude(string ifcEntityName)
        {
            IfcType ifcType = IfcInstances.IfcTypeLookup[ifcEntityName];
            AddExclude(ifcType);
        }

        public void AddExclude(IfcType ifcType)
        {
            if (!Excluded.Contains(ifcType))
            {
                IfcFilter flt = new IfcFilter(ifcType);
                Excluded.Add(flt);
            }
            foreach (IfcType subType in ifcType.IfcSubTypes)
            {
                AddExclude(subType);
            }
        }

        public IfcFilterDictionary GetFilter()
        {
            _parseFilter.Clear();
            foreach (IfcFilter ifcFilter in Included)
            {
                AddEntity(ifcFilter.Type, ifcFilter.PropertyIndices);
            }
            return _parseFilter;
        }

        public List<Type> TypeSelection
        {
            get
            {
                List<Type> typeList = new List<Type>();
                foreach (IfcFilter ifcFilter in Included)
                {
                    foreach (Type subType in ifcFilter.Type.NonAbstractSubTypes)
                        typeList.Add(subType);
                }
                return typeList;
            }
        }

        private void AddEntity(IfcType ifcType, params int[] idx)
        {
            int[] includedProperties = idx;

            if (!Excluded.Contains(ifcType) && !_parseFilter.Contains(ifcType)
                && !typeof (ExpressType).IsAssignableFrom(ifcType.Type))
            {
                if (!ifcType.Type.IsAbstract)
                {
                    IfcType superType = ifcType;
                    while (superType != null)
                    {
                        if (Included.Contains(superType))
                        {
                            IfcFilter flt = Included[superType];
                            IfcFilter parseFilter = new IfcFilter(ifcType, flt.PropertyIndices);
                            _parseFilter.Add(parseFilter);
                            includedProperties = parseFilter.PropertyIndices;
                            break;
                        }
                        else
                            superType = superType.IfcSuperType;
                    }

                    if (superType == null)
                        _parseFilter.Add(new IfcFilter(ifcType, idx));
                }

                foreach (Type subType in ifcType.NonAbstractSubTypes)
                {
                    if (subType != ifcType.Type) AddEntity(IfcInstances.IfcEntities[subType]);
                }

                foreach (IfcMetaProperty prop in ifcType.IfcProperties.Values)
                {
                    if (includedProperties == null || includedProperties.Length == 0 ||
                        includedProperties.Contains(prop.IfcAttribute.Order - 1))
                    {
                        Type propType = prop.PropertyInfo.PropertyType;
                        if (typeof (ExpressEnumerable).IsAssignableFrom(propType)) //its a list
                        {
                            Type genType = GetGenericType(propType);
                            if (genType != null && IfcInstances.IfcEntities.Contains(genType))
                                AddEntity(IfcInstances.IfcEntities[genType]);
                            if (genType.IsInterface)
                            {
                                //get the types that are like this
                                foreach (IfcType ent in IfcInstances.IfcEntities)
                                {
                                    if (genType.IsAssignableFrom(ent.Type))
                                        AddEntity(ent);
                                }
                            }
                        }
                        else if (propType.IsInterface) //its a select type
                        {
                            //get the types that are like this
                            foreach (IfcType ent in IfcInstances.IfcEntities)
                            {
                                if (propType.IsAssignableFrom(ent.Type))
                                    AddEntity(ent);
                            }
                        }
                        else if (typeof (IPersistIfc).IsAssignableFrom(propType) &&
                                 IfcInstances.IfcEntities.Contains(propType)) //its a normal entity
                        {
                            IfcType ifcPropType = IfcInstances.IfcEntities[propType];
                            AddEntity(ifcPropType);
                        }
                    }
                }
            }
        }

        private List<Type> GetConcreteTypes(string xbimName, HashSet<string> processed)
        {
            if (processed == null)
                processed = new HashSet<string>();

            if (processed.Contains(xbimName))
                return null;
            else
                processed.Add(xbimName);
            List<Type> concreteTypes = new List<Type>();
            foreach (IfcType ent in IfcInstances.IfcEntities)
            {
                if (IsConcreteClassOf(xbimName, ent))
                    concreteTypes.AddRange(ent.NonAbstractSubTypes);
            }
            return concreteTypes;
        }

        private bool IsConcreteClassOf(string name, IfcType candidate)
        {
            IfcType st = candidate;
            while (st != null)
            {
                if (st.Type.Name == name)
                    return true;
                else
                    st = st.IfcSuperType;
            }
            return false;
        }

        private Type GetGenericType(Type propType)
        {
            Type baseType = propType;
            while (baseType != null)
            {
                if (baseType.IsGenericType)
                {
                    Type[] types = baseType.GetGenericArguments(); //can only be one in Express languge
                    if (types.Length > 0)
                    {
                        return types[0];
                    }
                }
                else
                    baseType = baseType.BaseType;
            }
            return null;
        }
    }
}