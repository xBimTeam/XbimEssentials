using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    /// <summary>
    /// This class is used to resolve forward references in the model. It uses configuration from mapping and
    /// row as a data context. Forward reference doesn't keep the reference to the entity so it is possible for 
    /// the IModel to use memory optimizations while this reference exists. It will load the entity only when it is to
    /// be resolved.
    /// </summary>
    internal class ForwardReference
    {
        public ClassMapping CMapping { get; private set; }
        public TableStore Store { get; set; }

        /// <summary>
        /// Handle to the object which will be resolved
        /// </summary>
        private readonly XbimInstanceHandle _handle;

        /// <summary>
        /// Row context of the referenced value
        /// </summary>
        public IRow Row { get; private set; }

        /// <summary>
        /// Model of the entity
        /// </summary>
        public IModel Model { get { return _handle.Model; } }


        public ForwardReference(XbimInstanceHandle handle, IRow row, ClassMapping cMapping, TableStore store)
        {
            CMapping = cMapping;
            Store = store;
            _handle = handle;
            Row = row;
        }

        public ForwardReference(IPersistEntity entity, IRow row, ClassMapping cMapping, TableStore store)
        {
            CMapping = cMapping;
            Store = store;
            _handle = new XbimInstanceHandle(entity);
            Row = row;
        }

        private TextWriter _log;

        /// <summary>
        /// Resolves all references for the entity using configuration from class mapping and data from the row
        /// </summary>
        /// <param name="log"></param>
        public void Resolve(TextWriter log)
        {
            _log = log;

            //resolve parent first if this is not a root mapping
            ResolveParent();

            //resolve all other references
        }

        private void ResolveParent()
        {
            if (CMapping.IsRoot) return;

            var childPath = CMapping.ParentPath;
            if (string.IsNullOrWhiteSpace(childPath))
            {
                _log.WriteLine("It wasn't possible to resolve parent of {0} (label: {1}) because no parent path is defined in the configuration.", _handle.EntityExpressType.ExpressName, _handle.EntityLabel);
                return;
            }

            var parentType = GetExpressType("parent");
            if (parentType == null)
            {
                _log.WriteLine("It wasn't possible to resolve parent type of {0} (label: {1}) using data or configuration.", _handle.EntityExpressType.ExpressName, _handle.EntityLabel);
                return;
            }

            //look up the parent object(s) in the model
            var parents = GetReferencedEntities("parent", parentType).ToList();

            if (!parents.Any())
            {
                _log.WriteLine("No parent of type {0} for {1} was found based on the key values.", parentType.ExpressName, _handle.EntityExpressType.ExpressName);
                return;
            }
            if (parents.Count > 1)
            {
                _log.WriteLine("Multiple parents of type {0} for {1} were found based on the key values. All will be used.", parentType.ExpressName, _handle.EntityExpressType.ExpressName);
            }

            //assign to all parents rather than to loose the relation completely
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            var entity = _handle.GetEntity();
            foreach (var parent in parents)
            {
                var parentEntity = parent;
                //use child path in parent and add this object to the path (possibly using on-path values)
                var parts = childPath.Split('.');
                for (var i = 0; i < parts.Length; i++)
                {
                    var cProp = Store.GetProperty(parentType, parts[i]);
                    if (cProp == null)
                    {
                        _log.WriteLine("Parent type {0} of {1} doesn't have a property {2} used in a parent path.", parentType.ExpressName, _handle.EntityExpressType.ExpressName, parts[i]);
                        return;
                    }
                    if (i == parts.Length - 1)
                    {
                        if (cProp.IsInverse)
                        {
                            var remoteName = cProp.InverseAttributeProperty.RemoteProperty;
                            cProp = Store.GetProperty(_handle.EntityExpressType, remoteName);

                            //swap parent and entity
                            var swap = parent;
                            parentEntity = entity;
                            entity = swap;
                        }
                        //it is an enumerable explicit type so it is ItemSet
                        if (cProp.EnumerableType != null)
                        {
                            var list = cProp.PropertyInfo.GetValue(parentEntity, null) as IList;
                            if (list == null)
                                throw new XbimException("Inconsistent schema. Enumerable explicit attribute should always implement IList");
                            list.Add(entity);
                        }
                        else
                        {
                            cProp.PropertyInfo.SetValue(parentEntity, entity, null);
                        }
                    }
                }

            }
        }

        private IEnumerable<IPersistEntity> GetReferencedEntities(string pathBase, ExpressType type)
        {
            //get parent values to find the parent object
            var propMaps = CMapping.PropertyMappings.Where(m =>
            {
                var path = m.Paths.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(path))
                    return false;
                //all properties except for the ones which are only type helpers ([table], [type])
                return path.StartsWith(pathBase) && !path.Contains('[');
            });

            var keys = new Dictionary<ExpressMetaProperty, object[]>();
            foreach (var map in propMaps)
            {
                var keyCell = Row.GetCell(map.ColumnIndex);
                if (keyCell == null)
                    continue;

                var propName = map.Paths.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(propName))
                    continue;

                //todo: handle multiple values
                //only get the property name
                propName = propName.Split('.').LastOrDefault();
                if (string.IsNullOrWhiteSpace(propName))
                    continue;
                var eProp = Store.GetProperty(type, propName);
                if (eProp == null)
                {
                    _log.WriteLine("Type {0} if {1} doesn't have a property {2}.", type.ExpressName, _handle.EntityExpressType.ExpressName, propName);
                    continue;
                }

                var ePropType = eProp.PropertyInfo.PropertyType;
                if (!ePropType.IsValueType && !(ePropType == typeof(string)))
                {
                    _log.WriteLine("Type {0} in {1} has a property {2} which is not a simple type", type.ExpressName, _handle.EntityExpressType.ExpressName, propName);
                    continue;
                }

                //simple type or express value type
                var keyValue = Store.CreateSimpleValue(ePropType, keyCell);
                if (keyValue == null)
                    continue;
                keys.Add(eProp, new []{ keyValue });
            }

            //look up the parent object(s) in the model
            return keys.Count == 0 ?
                Model.Instances.OfType(type.Name, true).ToList() :
                Model.Instances.OfType(type.Name, true).Where(e => keys.All(kvp =>
                {
                    var prop = kvp.Key.PropertyInfo;
                    var vals = kvp.Value;
                    var eVal = prop.GetValue(e, null);
                    return eVal != null && vals.Any(v => v.Equals(eVal));
                })).ToList();
        }

        /// <summary>
        /// Type might be specified as a type hint with path 'parent.[type]' or with a table hint 'parent.[table]'
        /// or as a parent class in mapping configuration.
        /// </summary>
        /// <returns></returns>
        private ExpressType GetExpressType(string pathBase)
        {
            var typePath = pathBase + ".[type]";
            var typeHintProp =
                CMapping.PropertyMappings.FirstOrDefault(m => m.Paths.FirstOrDefault() == typePath);
            if (typeHintProp != null)
            {
                var typeHintCell = Row.GetCell(typeHintProp.ColumnIndex);
                if (typeHintCell != null && typeHintCell.CellType == CellType.String)
                {
                    var typeHintString = typeHintCell.StringCellValue;
                    if (!string.IsNullOrWhiteSpace(typeHintString))
                    {
                        var typeHint = Model.Metadata.ExpressType(typeHintString.ToUpper());
                        if (typeHint != null)
                            return typeHint;    
                    }
                }
            }

            var tablePath = pathBase + ".[table]";
            var tableHintProp =
                CMapping.PropertyMappings.FirstOrDefault(m => m.Paths.FirstOrDefault() == tablePath);
            if (tableHintProp != null)
            {
                var tableHintCell = Row.GetCell(tableHintProp.ColumnIndex);
                if (tableHintCell != null && tableHintCell.CellType == CellType.String)
                {
                    var tableHintString = tableHintCell.StringCellValue;
                    if (!string.IsNullOrWhiteSpace(tableHintString))
                    {
                        var tableType = Store.GetType(tableHintString);
                        if (tableType != null)
                            return tableType;
                    }
                }
            }

            if (pathBase == "parent")
            {
                var parentClass = CMapping.ParentClass.ToUpper();
                return Model.Metadata.ExpressType(parentClass);    
            }

            return null;
        }
    }
}
