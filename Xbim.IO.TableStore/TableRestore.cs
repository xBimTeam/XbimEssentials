using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    public partial class TableStore
    {
        public TextWriter Log { get; private set; }

        #region Reading in from a spreadsheet
        public void LoadFrom(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Log = new StringWriter();

            var ext = Path.GetExtension(path).ToLower().Trim('.');
            if (ext != "xls" && ext != "xlsx")
            {
                //XLSX is Spreadsheet XML representation which is capable of storing more data
                path += ".xlsx";
                ext = "xlsx";
            }
            using (var file = File.OpenRead(path))
            {
                var type = ext == "xlsx" ? ExcelTypeEnum.XLSX : ExcelTypeEnum.XLS;
                LoadFrom(file, type);
                file.Close();
            }


        }

        public void LoadFrom(Stream stream, ExcelTypeEnum type)
        {
            IWorkbook workbook;
            switch (type)
            {
                case ExcelTypeEnum.XLS:
                    workbook = new HSSFWorkbook(stream);
                    break;
                case ExcelTypeEnum.XLSX: //this is as it should be according to a standard
                    workbook = new XSSFWorkbook(stream);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            //refresh cache. This might change in between two loadings
            _multiRowIndicesCache = new Dictionary<string, int[]>();
            _isMultiRowMappingCache = new Dictionary<ClassMapping, bool>();
            _referenceContexts = new Dictionary<ClassMapping, ReferenceContext>();
            _forwardReferences.Clear();
            _globalEntities.Clear();

            //create spreadsheet representaion 
            LoadFromWoorkbook(workbook);
        }

        private void LoadFromWoorkbook(IWorkbook workbook)
        {
            //get all data tables
            if (Mapping.ClassMappings == null || !Mapping.ClassMappings.Any())
                return;

            var sheetsNumber = workbook.NumberOfSheets;
            for (var i = 0; i < sheetsNumber; i++)
            {
                var sheetName = workbook.GetSheetName(i);
                var mapping =
                    Mapping.ClassMappings.FirstOrDefault(
                        m => string.Equals(sheetName, m.TableName, StringComparison.OrdinalIgnoreCase));
                if (mapping == null)
                    continue;
                var sheet = workbook.GetSheet(sheetName);

                if (mapping.IsPartial)
                    continue;
                LoadFromSheet(sheet, mapping);
            }

            //resolve references (don't use foreach as new references might be added to the queue during the processing)
            while (_forwardReferences.Count != 0)
            {
                var reference = _forwardReferences.Dequeue();
                reference.Resolve();
            }

            //todo: load partial tables (also just a references - use scalars to find an object and parent to assign it)
            
            //be happy
        }

        private void LoadFromSheet(ISheet sheet, ClassMapping mapping)
        {
            //if there is only header in a sheet, don't waste resources
            if (sheet.LastRowNum < 1)
                return;

            //adjust mapping to sheet in case columns are in a different order
            AdjustMapping(sheet, mapping);
            CacheColumnIndices(mapping);

            //cache key columns
            CacheMultiRowIndices(mapping);

            //cache contexts
            var context = GetReferenceContext(mapping);

            //iterate over rows (be careful about MultiRow != None, merge values if necessary)
            var enumerator = sheet.GetRowEnumerator();
            IRow lastRow = null;
            IPersistEntity lastEntity = null;
            var emptyCells = 0;
            while (enumerator.MoveNext())
            {
                var row = enumerator.Current as IRow;
                //skip header row
                if (row == null || row.RowNum == 0)
                    continue;

                if (!row.Cells.Any())
                {
                    emptyCells++;
                    if (emptyCells == 3)
                        //break processing if this is third empty row
                        break;
                    //skip empty row
                    continue;
                }
                emptyCells = 0;

                //last row might be used in case this is a MultiRow
                lastEntity = LoadFromRow(row, context, lastRow, lastEntity);
                lastRow = row;
            }
        }

        private ReferenceContext GetReferenceContext(ClassMapping mapping)
        {
            ReferenceContext context;
            if (_referenceContexts.TryGetValue(mapping, out context))
                return context;
            context = new ReferenceContext(this, mapping);
            _referenceContexts.Add(mapping, context);
            return context;
        }

        /// <summary>
        /// All indices should be cached already
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private IEnumerable<int> GetIdentityIndices(ClassMapping mapping)
        {
            return _multiRowIndicesCache[mapping.TableName];
        }

        private void CacheMultiRowIndices(ClassMapping mapping)
        {
            int[] existing;
            //one table might be defined for multiple classes but it has to have the same structure and constrains
            _multiRowIndicesCache.TryGetValue(mapping.TableName, out existing);

            var indices = new int[0];
            if (mapping.PropertyMappings != null && mapping.PropertyMappings.Any())
                indices = mapping.PropertyMappings
                    .Where(p => p.IsMultiRowIdentity)
                    .Select(m => m.ColumnIndex)
                    .ToArray();
           
            if (existing != null)
            {
                //update and check if it is consistent. Report inconsistency.
                _multiRowIndicesCache[mapping.TableName] = indices;
                if (existing.Length != indices.Length || !existing.SequenceEqual(indices))
                    Log.WriteLine("Table {0} is defined in multiple class mappings with different key columns for a multi-value records", mapping.TableName);
            }
            else
                _multiRowIndicesCache.Add(mapping.TableName, indices);

        }

        private IPersistEntity LoadFromRow(IRow row, ReferenceContext context, IRow lastRow, IPersistEntity lastEntity)
        {
            //load data into the context
            context.LoadData(row, true);

            var multirow = IsMultiRow(row, context.CMapping, lastRow);
            if (multirow)
            {
                //only add multivalue to the multivalue properties of last entity
                var subContexts = context.AllScalarChildren
                    .Where(c => c.Mapping.MultiRow != MultiRow.None)
                    .Select(c =>
                    {
                        //get to the first list level up or on the base level if it is a scalar list
                        if (c.ContextType == ReferenceContextType.ScalarList)
                            return c;
                        while (c != null && c.ContextType != ReferenceContextType.EntityList)
                            c = c.ParentContext;
                        return c;
                    })
                    .Where(c => c != null)
                    .Distinct();
                foreach (var ctx in subContexts)
                    ResolveMultiContext(ctx, lastEntity);
                return lastEntity;
            }


            //get type of the coresponding object from ClassMapping or from a type hint, create instance
            return ResolveContext(context, -1, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Reference context of the data</param>
        /// <param name="scalarIndex">Index of value to be used in a value list in case of multi values</param>
        /// <param name="onlyScalar"></param>
        /// <param name="entity">Existing entity which is parent to the context</param>
        /// <returns></returns>
        internal IPersistEntity ResolveContext(ReferenceContext context, int scalarIndex, bool onlyScalar)
        {
            IPersistEntity entity = null;
            var eType = GetConcreteType(context);
            if (IsGlobalType(eType.Type))
            {
                //it is a global type but there are no values to fill in
                if (!context.AllScalarChildren.Any(c => c.Values != null && c.Values.Any()))
                    return null;

                //it is a global entity and it was filled in with the data before
                if (GetOrCreateGlobalEntity(context, out entity, eType, scalarIndex))
                    return entity;
            }

            //create new entity if new global one was not created
            if(entity == null)
                entity = Model.Instances.New(eType.Type);

            //scalar values to be set to the entity
            foreach (var scalar in context.ScalarChildren)
            {
                var values = scalar.Values;
                if (values == null || values.Length == 0)
                    continue;
                if (scalar.ContextType == ReferenceContextType.ScalarList)
                {
                    //is should be ItemSet which is always initialized and inherits from IList
                    var list = scalar.PropertyInfo.GetValue(entity, null) as IList;
                    if(list == null)
                        continue;
                    foreach (var value in values)
                        list.Add(value);
                    continue;
                }

                //it is a single value
                var val = scalarIndex < 0 ? values[0]: (values.Length >= scalarIndex+1 ? values[scalarIndex] : null);
                if(val != null)
                    scalar.PropertyInfo.SetValue(entity, val, scalar.Index != null ? new[] { scalar.Index } : null);
            }

            if (onlyScalar)
                return entity;

            //nested entities (global, local, referenced)
            foreach (var childContext in context.EntityChildren)
            {
                if (childContext.IsReference)
                {
                    _forwardReferences.Enqueue(new ForwardReference(entity, childContext, this));
                    continue;
                }

                if (childContext.ContextType == ReferenceContextType.EntityList)
                {
                        var depth =
                            childContext.ScalarChildren.Where(c => c.Values != null)
                                .Select(c => c.Values.Length)
                                .OrderByDescending(v => v)
                                .FirstOrDefault();
                        for (var i = 0; i < depth; i++)
                        {
                            var child = depth == 1 ? ResolveContext(childContext, -1, false) : ResolveContext(childContext, i, false);
                            AssignEntity(entity, child, childContext);
                        }    
                    continue;
                }

                //it is a single entity
                var cEntity = ResolveContext(childContext, -1, false);
                AssignEntity(entity, cEntity, childContext);
            }

            var parentContext = context.Children.FirstOrDefault(c => c.ContextType == ReferenceContextType.Parent);
            if (parentContext != null)
                _forwardReferences.Enqueue(new ForwardReference(entity, parentContext, this));

            return entity;
        }

        internal void AssignEntity(IPersistEntity parent, IPersistEntity entity, ReferenceContext context)
        {
            if (context.MetaProperty != null && context.MetaProperty.IsDerived)
            {
                Log.WriteLine("It wasn't possible to add entity {0} as a {1} to parent {2} because it is a derived value",
                    entity.ExpressType.ExpressName, context.Segment, parent.ExpressType.ExpressName);
                return;
            }

            var index = context.Index == null ? null : new[] {context.Index};
            //inverse property
            if (context.MetaProperty != null && context.MetaProperty.IsInverse)
            {
                var remotePropName = context.MetaProperty.InverseAttributeProperty.RemoteProperty;
                var entityType = entity.ExpressType;
                var remoteProp = GetProperty(entityType, remotePropName);
                //it is enumerable inverse
                if (remoteProp.EnumerableType != null)
                {
                    var list = remoteProp.PropertyInfo.GetValue(entity, index) as IList;
                    if (list != null)
                    {
                        list.Add(parent);
                        return;
                    }
                }
                //it is a single inverse entity
                else
                {
                    remoteProp.PropertyInfo.SetValue(entity, parent, index);
                    return;
                }
                Log.WriteLine("It wasn't possible to add entity {0} as a {1} to parent {2}", 
                    entity.ExpressType.ExpressName, context.Segment, entityType.ExpressName);
                return;
            }
            //explicit property
            var info = context.PropertyInfo;
            if (context.ContextType == ReferenceContextType.EntityList)
            {
                var list = info.GetValue(parent, index) as IList;
                if (list != null)
                {
                    list.Add(entity);
                    return;
                }
            }
            else
            {
                if ((context.MetaProperty != null && context.MetaProperty.IsExplicit) || info.GetSetMethod() != null)
                {
                    info.SetValue(parent, entity, index);
                    return;
                }
            }
            Log.WriteLine("It wasn't possible to add entity {0} as a {1} to parent {2}",
                entity.ExpressType.ExpressName, context.Segment, parent.ExpressType.ExpressName);
        }

        /// <summary>
        /// This is used for a multi-row instances where only partial context needs to be processed
        /// </summary>
        /// <param name="subContext"></param>
        /// <param name="rootEntity"></param>
        private void ResolveMultiContext(ReferenceContext subContext, IPersistEntity rootEntity)
        {
            //get context path from root entity
            var ctxStack = new Stack<ReferenceContext>();
            var context = subContext;
            while (context != null)
            {
                ctxStack.Push(context);
                context = context.ParentContext;
            }

            //use path to get to the bottom of the stact and add the value to it
            context = ctxStack.Pop();
            var entity = rootEntity;
            //stop one level above the original subcontext
            while (ctxStack.Peek() != subContext)
            {
                //browse to the level of the bottom context and call ResolveContext there
                var index = context.Index != null ? new[] { context.Index } : null;
                var value = context.PropertyInfo.GetValue(rootEntity, index);
                if (value == null)
                {
                    Log.WriteLine("It wasn't possible to browse to the data entry point.");
                    return;
                }

                if (context.ContextType == ReferenceContextType.Entity)
                {
                    entity = value as IPersistEntity;
                    continue;
                }

                var entities = value as IEnumerable;
                if (entities == null)
                {
                    Log.WriteLine("It wasn't possible to browse to the data entry point.");
                    return;
                }
                foreach (var e in entities)
                {
                    if (!IsValidEntity(context, e)) 
                        continue;
                    entity = e as IPersistEntity;
                    break;
                }
            }
            if (subContext.IsReference)
            {
                var reference = new ForwardReference(entity, subContext, this);
                _forwardReferences.Enqueue(reference);
                return;
            }

            if (subContext.ContextType == ReferenceContextType.EntityList)
            {
                var child = ResolveContext(subContext, -1, false);
                AssignEntity(entity, child, subContext);
                return;
            }

            if (subContext.ContextType == ReferenceContextType.ScalarList)
            {
                var list = subContext.PropertyInfo.GetValue(entity, null) as IList;
                if (list != null && subContext.Values != null && subContext.Values.Any())
                    list.Add(subContext.Values[0]);
            }
            
        }

        internal static bool IsValidEntity(ReferenceContext context, object entity)
        {
            if (!context.ScalarChildren.Any())
                return true;

            //if it might have identifiers but doesn't have a one it can't find any
            if (!context.HasData)
                return false;

            return context.ScalarChildren
                .Where(s => s.Values != null && s.Values.Length > 0)
                .All(scalar =>
                {
                    var prop = scalar.PropertyInfo;
                    var vals = scalar.Values;
                    var eVal = prop.GetValue(entity, null);
                    if (scalar.ContextType != ReferenceContextType.ScalarList)
                        return eVal != null && vals.Any(v => v.Equals(eVal));
                    var list = eVal as IEnumerable;
                    return list != null &&
                           //it might be a multivalue
                           list.Cast<object>().All(item => vals.Any(v => v.Equals(item)));
                });
        }

        private bool IsGlobalType(Type type)
        {
            var gt = _globalTypes ??
                     (_globalTypes =
                         Mapping.Scopes.Where(s => s.Scope == ClassScopeEnum.Model)
                             .Select(s => MetaData.ExpressType(s.Class.ToUpper()))
                             .ToList());
            return gt.Any(t => t.Type == type || t.SubTypes.Any(st => st.Type == type));
        }

        private bool IsMultiRow(IRow row, ClassMapping mapping, IRow lastRow)
        {
            if (lastRow == null) return false;

            bool isMultiMapping;
            if (_isMultiRowMappingCache.TryGetValue(mapping, out isMultiMapping))
            {
                if (!isMultiMapping) return false;
            }
            else
            {
                if (mapping.PropertyMappings == null || !mapping.PropertyMappings.Any())
                {
                    _isMultiRowMappingCache.Add(mapping, false);
                    return false;
                }

                var multiRowProperty = mapping.PropertyMappings.FirstOrDefault(m => m.MultiRow != MultiRow.None);
                if (multiRowProperty == null)
                {
                    _isMultiRowMappingCache.Add(mapping, false);
                    return false;
                }

                _isMultiRowMappingCache.Add(mapping, true);
            }

            
            var keyIndices = GetIdentityIndices(mapping);
            foreach (var index in keyIndices)
            {
                var cellA = row.GetCell(index);
                var cellB = lastRow.GetCell(index);

                if(cellA == null || cellB == null)
                    return false;

                if (cellA.CellType == CellType.Blank || cellB.CellType == CellType.Blank)
                    return false;

                if (cellA.CellType != cellB.CellType)
                    return false;

                switch (cellA.CellType)
                {
                    case CellType.Numeric:
                        if (Math.Abs(cellA.NumericCellValue - cellB.NumericCellValue) > 1e-9)
                            return false;
                        break;
                    case CellType.String:
                        if (cellA.StringCellValue != cellB.StringCellValue)
                            return false;
                        break;
                    case CellType.Boolean:
                        if (cellA.BooleanCellValue != cellB.BooleanCellValue)
                            return false;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if it exists, FALSE if new entity fas created and needs to be filled in with data
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <param name="scalarIndex">Index to field of values to be used to create the key. If -1 no index is used and all values are used.</param>
        /// <returns></returns>
        private bool GetOrCreateGlobalEntity(ReferenceContext context, out IPersistEntity entity, ExpressType type, int scalarIndex)
        {
            type = type ?? GetConcreteType(context);
            Dictionary<string, IPersistEntity> entities;
            if (!_globalEntities.TryGetValue(type, out entities))
            {
                entities = new Dictionary<string, IPersistEntity>();
                _globalEntities.Add(type, entities);
            }

            var keys = scalarIndex > -1 ?
                   context.AllScalarChildren.OrderBy(c => c.Segment)
                        .Where(c => c.Values != null)
                        .Select(c =>
                        {
                            if (c.Values.Length == 1) return c.Values[0];
                            return c.Values.Length >= scalarIndex + 1 ? c.Values[scalarIndex] : null;
                        }  ).Where(v => v != null):

                    context.AllScalarChildren.OrderBy(c => c.Segment)
                        .Where(c => c.Values != null)
                        .SelectMany(c => c.Values.Select(v => v.ToString()));
            var key = string.Join(", ", keys);
            if (entities.TryGetValue(key, out entity))
                return true;

            entity = Model.Instances.New(type.Type);
            entities.Add(key, entity);
            return false;
        }

        internal Type GetConcreteType(ReferenceContext context, ICell cell)
        {
            var cType = context.SegmentType;
            if (cType != null && !cType.Type.IsAbstract)
                return cType.Type;

            //use custom type resolver if there is a one which can resolve this type
            if (cType != null && Resolvers != null && Resolvers.Any())
            {
                var resolver = Resolvers.FirstOrDefault(r => r.CanResolve(cType));
                if (resolver != null)
                    return resolver.Resolve(cType.Type, cell, context.CMapping, context.Mapping);
            }

            if (context.PropertyInfo != null)
            {
                var pType = context.PropertyInfo.PropertyType;
                pType = GetNonNullableType(pType);
                if (pType.IsValueType || pType == typeof(string))
                    return pType;

                if (typeof (IEnumerable).IsAssignableFrom(pType))
                {
                    pType = pType.GetGenericArguments()[0];
                    if (pType.IsValueType || pType == typeof (string))
                        return pType;
                }

                if (Resolvers != null && Resolvers.Any())
                {
                    var resolver = Resolvers.FirstOrDefault(r => r.CanResolve(pType));
                    if (resolver != null)
                        return resolver.Resolve(pType, cell, context.CMapping, context.Mapping);
                }
            }

            Log.WriteLine("It wasn't possible to find a non-abstract type for table {0}, class {1}",
                context.CMapping.TableName, context.CMapping.Class);
            return null;
        }

        private ExpressType GetConcreteType(ReferenceContext context)
        {
            var cType = context.SegmentType;
            if (cType != null && !cType.Type.IsAbstract)
                return cType;


            //use fallback to retrieve a non-abstract type (defined in a configuration file?)
            var fbTypeName = context.CMapping.FallBackConcreteType;
            if (context.IsRoot && !string.IsNullOrWhiteSpace(fbTypeName))
            {
                var eType = MetaData.ExpressType(fbTypeName.ToUpper());
                if (eType != null && !eType.Type.IsAbstract)
                    return eType;
            }    
            

            //use custom type resolver if there is a one which can resolve this type
            if (cType != null && Resolvers != null && Resolvers.Any())
            {
                var resolver = Resolvers.FirstOrDefault(r => r.CanResolve(cType));
                if(resolver != null)
                    return resolver.Resolve(cType, context, MetaData);
            }

            Log.WriteLine("It wasn't possible to find a non-abstract type for table {0}, class {1}",
                context.CMapping.TableName, context.CMapping.Class);
            return null;
        }

        private static void CacheColumnIndices(ClassMapping mapping)
        {
            foreach (var pMap in mapping.PropertyMappings)
                pMap.ColumnIndex = CellReference.ConvertColStringToIndex(pMap.Column);
        }

        private static void AdjustMapping(ISheet sheet, ClassMapping mapping)
        {
            //there is only header
            if (sheet.LastRowNum < 1)
                return;

            //get the header row and analyze it
            var headerRow = sheet.GetRow(0);
            if (headerRow == null)
                return;

            var headings = headerRow.Cells.Where(c => c.CellType == CellType.String || !string.IsNullOrWhiteSpace(c.StringCellValue)).ToList();
            if (!headings.Any())
                return;
            var mappings = mapping.PropertyMappings;
            if (mappings == null || !mappings.Any())
                return;

            foreach (var heading in headings)
            {
                var index = heading.ColumnIndex;
                var column = CellReference.ConvertNumToColString(index).ToUpper();
                var header = heading.StringCellValue;

                var pMapping = mappings.FirstOrDefault(m => string.Equals(m.Header, header, StringComparison.OrdinalIgnoreCase));
                //if no mapping is found things might go wrong or it is just renamed
                if (pMapping == null || string.Equals(pMapping.Column, column, StringComparison.OrdinalIgnoreCase))
                    continue;

                //if the letter is not assigned at all, assign this letter
                if (string.IsNullOrWhiteSpace(pMapping.Column))
                {
                    pMapping.Column = column;
                    continue;
                }

                //move the column mapping to the new position
                var current = mappings.FirstOrDefault(m => string.Equals(m.Column, column, StringComparison.OrdinalIgnoreCase));
                if (current != null)
                    current.Column = null;
                pMapping.Column = column;
            }

            var unassigned = mappings.Where(m => string.IsNullOrWhiteSpace(m.Column)).ToList();
            if (!unassigned.Any())
                return;

            //try to assign letters to the unassigned columns
            foreach (var heading in headings)
            {
                var index = heading.ColumnIndex;
                var column = CellReference.ConvertNumToColString(index).ToUpper();
                var pMapping = mappings.FirstOrDefault(m => string.Equals(m.Column, column, StringComparison.OrdinalIgnoreCase));
                if (pMapping != null)
                    continue;

                var first = unassigned.FirstOrDefault();
                if (first == null) break;

                first.Column = column;
                unassigned.Remove(first);
            }

            //remove unassigned mappings
            if (unassigned.Any())
                return;
            foreach (var propertyMapping in unassigned)
                mapping.PropertyMappings.Remove(propertyMapping);
        }

        private static Type GetNonNullableType(Type type)
        {
            //only value types can be nullable
            if (!type.IsValueType) return type;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>) ? Nullable.GetUnderlyingType(type) : type;
        }

        internal object CreateSimpleValue(Type type, string value)
        {
            var underlying = GetNonNullableType(type);

            var propType = underlying;
            var isExpress = false;

            //dig deeper if it is an express value type
            if (underlying.IsValueType && typeof(IExpressValueType).IsAssignableFrom(underlying))
            {
                var eType = MetaData.ExpressType(underlying);
                if (eType != null)
                {
                    isExpress = true;
                    underlying = GetNonNullableType(eType.UnderlyingType);
                }
            }

            //chack base types
            if (typeof(string) == underlying)
            {
                return isExpress ? Activator.CreateInstance(propType, value) : value;
            }
            if (underlying == typeof (double) || underlying == typeof (float))
            {
                double d;
                if (double.TryParse(value, out d))
                    return isExpress
                    ? Activator.CreateInstance(propType, d)
                    : d;
                return null;
            }
            if (underlying == typeof (int) || underlying == typeof (long))
            {
                var l = type == typeof(int) ? Convert.ToInt32(value) : Convert.ToInt64(value);
                return isExpress ? Activator.CreateInstance(propType, l) : l;
            }
            if (underlying == typeof (DateTime))
            {
                DateTime date;
                return !DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out date) ? 
                    DateTime.Parse("1900-12-31T23:59:59", null, DateTimeStyles.RoundtripKind) : 
                    date;
            }
            if (underlying == typeof (bool))
            {
                bool i;
                if (bool.TryParse(value, out i))
                    return isExpress ? Activator.CreateInstance(propType, i) : i;
                return null;
            }
            if (underlying.IsEnum)
            {
                try
                {
                    var eMember = GetAliasEnumName(underlying, value);
                    //if there was no alias try to parse the value
                    var val = Enum.Parse(underlying, eMember ?? value, true);
                    return val;
                }
                catch (Exception)
                {
                    Log.WriteLine("Enumeration {0} doesn't have a member {1}", underlying.Name, value);
                }
            }
            return null;
        }

        internal object CreateSimpleValue(Type type, ICell cell)
        {
            //return if there is no value in she cell
            if (cell.CellType == CellType.Blank) return null;
            type = GetNonNullableType(type);
            
            var propType = type;
            var isExpress = false;

            //dig deeper if it is an express value type
            if (type.IsValueType && typeof(IExpressValueType).IsAssignableFrom(type))
            {
                var eType = MetaData.ExpressType(type);
                if (eType != null)
                {
                    isExpress = true;
                    type = GetNonNullableType(eType.UnderlyingType);
                }
            }

            if (typeof(string) == type)
            {
                string value = null;
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        value = cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                        break;
                    case CellType.String:
                        value = cell.StringCellValue;
                        break;
                    case CellType.Boolean:
                        value = cell.BooleanCellValue.ToString();
                        break;
                    default:
                        Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                        break;
                }
                return isExpress ? Activator.CreateInstance(propType, value) : value;
            }

            if (type == typeof(DateTime))
            {
                var date = default(DateTime);
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        date = cell.DateCellValue;
                        break;
                    case CellType.String:
                        if (!DateTime.TryParse(cell.StringCellValue, null, DateTimeStyles.RoundtripKind, out date))
                            //set to default value according to specification
                            date = DateTime.Parse("1900-12-31T23:59:59", null, DateTimeStyles.RoundtripKind);
                        break;
                    default:
                        Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                        break;
                }
                return date;
            }

            if (type == typeof(double) || type == typeof(float))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        return isExpress
                            ? Activator.CreateInstance(propType, cell.NumericCellValue)
                            : cell.NumericCellValue;
                    case CellType.String:
                        double d;
                        if (double.TryParse(cell.StringCellValue, out d))
                            return isExpress
                            ? Activator.CreateInstance(propType, d)
                            : d;
                        break;
                    default:
                        Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                        break;
                }
                return null;
            }

            if (type == typeof(int) || type == typeof(long))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.String:
                        var l = type == typeof(int) ? Convert.ToInt32(cell.NumericCellValue) : Convert.ToInt64(cell.NumericCellValue);
                        return isExpress ? Activator.CreateInstance(propType, l) : l;
                    default:
                        Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                        break;
                }
                return null;
            }

            if (type == typeof(bool))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        var b = (int)cell.NumericCellValue != 0;
                        return isExpress ? Activator.CreateInstance(propType, b) : b;
                    case CellType.String:
                        bool i;
                        if (bool.TryParse(cell.StringCellValue, out i))
                            return isExpress ? Activator.CreateInstance(propType, i) : i;
                            Log.WriteLine("Wrong boolean format of {0} in cell {1}{2}, sheet {3}", cell.StringCellValue, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                        break;
                    case CellType.Boolean:
                            return isExpress ? Activator.CreateInstance(propType, cell.BooleanCellValue) : cell.BooleanCellValue;
                    default:
                            Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                        break;
                }
                return null;
            }

            //enumeration
            if (type.IsEnum)
            {
                if (cell.CellType != CellType.String)
                {
                    Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                    return null;
                }
                try
                {
                    var eValue = cell.StringCellValue;
                    var eMember = GetAliasEnumName(type, eValue);
                    //if there was no alias try to parse the value
                    var val = Enum.Parse(type, eMember ?? eValue, true);
                    return val;
                }
                catch (Exception)
                {
                    Log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}", propType.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1, cell.Sheet.SheetName);
                }
            }

            //if not suitable type was found, report it 
            throw new Exception("Unsupported type " + type.Name + " for value '" + cell + "'");
        }

        private string GetAliasEnumName(Type type, string alias)
        {
            string result;
            return _aliasesEnumCache.TryGetValue(type.Name + "." + alias, out result) ? result : null;
        }

        #endregion
    }
}
