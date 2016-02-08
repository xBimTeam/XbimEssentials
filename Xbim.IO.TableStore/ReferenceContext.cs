using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NPOI.SS.UserModel;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    [DebuggerDisplay("{Segment}")]
    public class ReferenceContext
    {
        public string Segment { get; set; }
        public PropertyMapping Mapping { get; private set; }
        public ExpressMetaProperty MetaProperty { get; private set; }
        public List<ReferenceContext> Children { get; private set; }
        public ReferenceContext ParentContext { get; private set; }
        public PropertyMapping TableHintMapping { get; private set; }
        public PropertyMapping TypeHintMapping { get; private set; }
        public ExpressType PropertyTypeHint { get; private set; }
        public ExpressType PathTypeHint { get; private set; }
        public ExpressType TypeTypeHint { get; private set; }
        public ExpressType TableTypeHint { get; private set; }
        /// <summary>
        /// This should never return null if one of these is specified: 
        /// TypeTypeHint ?? PathTypeHint ?? TableTypeHint ?? PropertyTypeHint
        /// </summary>
        public ExpressType SegmentType { get { return TypeTypeHint ?? PathTypeHint ?? TableTypeHint ?? PropertyTypeHint; } }
        /// <summary>
        /// This will return null if property type is to be used
        /// </summary>
        public ExpressType SegmentTypeOf { get { return TypeTypeHint ?? PathTypeHint ?? TableTypeHint; } }
        public ReferenceContextType ContextType { get; private set; }
        private TableStore Store { get; set; }
        public ClassMapping CMapping { get; private set; }
        public object[] Values { get; private set; }
        public object Index { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public bool IsRoot { get { return ParentContext == null; } }
        public IRow CurrentRow { get; private set; }
        
        /// <summary>
        /// Only scalar children. These can be used to find an object or to fill in the data.
        /// </summary>
        public IEnumerable<ReferenceContext> ScalarChildren { get
        {
            return
                Children.Where(
                    c =>
                        c.ContextType == ReferenceContextType.Scalar || c.ContextType == ReferenceContextType.ScalarList);
        }}

        /// <summary>
        /// Only scalar children. These can be used to find an object or to fill in the data.
        /// </summary>
        public IEnumerable<ReferenceContext> EntityChildren
        {
            get
            {
                return
                    Children.Where(
                        c =>
                            c.ContextType == ReferenceContextType.Entity || c.ContextType == ReferenceContextType.EntityList);
            }
        }

        public IEnumerable<ReferenceContext> AllScalarChildren
        {
            get
            {
                return
                AllChildren.Where(
                    c =>
                        c.ContextType == ReferenceContextType.Scalar || c.ContextType == ReferenceContextType.ScalarList);
            }
        }

        public IEnumerable<ReferenceContext> AllChildren
        {
            get {
                foreach (var child in Children)
                {
                    yield return child;
                    foreach (var allChild in child.AllChildren)
                    {
                        yield return allChild;
                    }
                }
            }
        } 

        public bool HasEnumerationOnPath
        {
            get
            {
                if (ParentContext == null)
                    return false;
                if (ContextType == ReferenceContextType.EntityList || ContextType == ReferenceContextType.ScalarList)
                    return true;
                return ParentContext.HasEnumerationOnPath;
            }
        }

        public ReferenceContext RootContext
        {
            get
            {
                return ParentContext == null ? this : ParentContext.RootContext;
            }
        }

        public bool IsReference { get
        {
            return (Mapping != null && Mapping.Status == DataStatus.Reference) || ScalarChildren.Any(c => c.Mapping != null && c.Mapping.Status == DataStatus.Reference);
        } }

        /// <summary>
        /// Any scalar child of any children has values loaded from a row
        /// </summary>
        public bool HasData
        {
            get { return AllScalarChildren.Any(c => c.Values != null && c.Values.Any()); }
        }

        public ReferenceContext(TableStore store, ClassMapping cMapping)
        {
            Store = store;
            CMapping = cMapping;
            Children = new List<ReferenceContext>();
            ContextType = ReferenceContextType.Root;
            PathTypeHint = Store.MetaData.ExpressType(cMapping.Class.ToUpper());

            if (cMapping.PropertyMappings == null || !cMapping.PropertyMappings.Any())
                return;

            var parentPath = ("parent." + cMapping.ParentPath) .Split(new [] {'.'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var mapping in cMapping.PropertyMappings)
            {
                var path = mapping.Paths.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var segments = path.Split('.').ToList();

                //if path uses upper context of parent, fix the path to be relative to parent
                if (path[0] == '(')
                {
                    var levelCount = path.Count(c => c == '(');
                    segments =
                        parentPath.GetRange(0, parentPath.Count - levelCount)
                            .Concat(segments.GetRange(levelCount - 1, segments.Count - levelCount))
                            .ToList();
                }

                AddMapping(mapping, segments);
            }

            //add special mapping for a parent path
            if (!cMapping.IsRoot)
            {
                var pMap = new PropertyMapping{_Paths = string.Join(".", parentPath), Status = DataStatus.Reference};
                AddMapping(pMap, parentPath);
            }


        }

        private ReferenceContext(string segment, ReferenceContext parent, TableStore store, ClassMapping cMapping)
        {
            Store = store;
            CMapping = cMapping;

            //init lists
            Children = new List<ReferenceContext>();
            ParentContext = parent;

            //try to extract TypeOf part of the path
            var parts = segment.Split('\\');
            segment = parts[0];
            var typeHint = parts.Length > 1 ? parts[1] : null;
            Segment = segment;

            //set up path type hint if it is defined
            if (!string.IsNullOrWhiteSpace(typeHint))
                PathTypeHint = Store.MetaData.ExpressType(typeHint.ToUpper());

            if (segment == "parent")
            {
                PathTypeHint = Store.MetaData.ExpressType(CMapping.ParentClass.ToUpper());
                ContextType = ReferenceContextType.Parent;
                return;
            }

            Index = TableStore.GetPropertyIndex(ref segment);
            PropertyInfo = Store.GetPropertyInfo(segment, parent.SegmentType, Index);
            MetaProperty = Store.GetProperty(parent.SegmentType, segment);
            var info = PropertyInfo != null
                    ? PropertyInfo.PropertyType
                    : (MetaProperty != null
                        ? MetaProperty.EnumerableType ?? MetaProperty.PropertyInfo.PropertyType
                        : null);

            if (info == null)
            {
                Store.Log.WriteLine("Type {0} doesn't have a property {1}.", parent.PathTypeHint.ExpressName, segment);
                return;
            }

            PropertyTypeHint = Store.MetaData.ExpressType(MetaProperty != null ?
                    MetaProperty.EnumerableType ?? MetaProperty.PropertyInfo.PropertyType :
                    (PropertyInfo != null ? PropertyInfo.PropertyType : null) 
                );


            //set up type of the context
            var isEnumerable = MetaProperty != null && MetaProperty.EnumerableType != null;
            if (isEnumerable)
            {
                if(MetaProperty.EnumerableType.IsValueType || 
                    MetaProperty.EnumerableType == typeof(string) || 
                    typeof(IExpressValueType).IsAssignableFrom(MetaProperty.EnumerableType))
                    ContextType = ReferenceContextType.ScalarList;
                else
                    ContextType = ReferenceContextType.EntityList;
            }
            else
            {
                
                if (info.IsValueType || 
                    info == typeof(string) || 
                    typeof(IExpressValueType).IsAssignableFrom(info))
                    ContextType = ReferenceContextType.Scalar;
                else
                    ContextType = ReferenceContextType.Entity;
            }

        }

        public void LoadData(IRow row, bool skipReferences)
        {
            CurrentRow = row;

            //clear any old data
            Values = null;
            TableTypeHint = null;
            TypeTypeHint = null;


            //load child data
            foreach (var child in Children)
                child.LoadData(row, skipReferences);
            
            //load type and table hint values if available
            if (TypeHintMapping != null)
            {
                var typeCell = row.GetCell(TypeHintMapping.ColumnIndex);
                if (typeCell != null && typeCell.CellType == CellType.String && !string.IsNullOrWhiteSpace(typeCell.StringCellValue))
                    TypeTypeHint = Store.MetaData.ExpressType(typeCell.StringCellValue.ToUpper());
            }
            if (TableHintMapping != null)
            {
                var tableCell = row.GetCell(TableHintMapping.ColumnIndex);
                if (tableCell != null && tableCell.CellType == CellType.String && !string.IsNullOrWhiteSpace(tableCell.StringCellValue))
                    TableTypeHint = Store.GetType(tableCell.StringCellValue);
            }

            //return if this is not a leaf
            if (ContextType != ReferenceContextType.Scalar && ContextType != ReferenceContextType.ScalarList)
                return;

            if (skipReferences && IsReference)
                return;
            
            //if there is no mapping it doesn't make a sense to load any data
            if (Mapping == null) return;

            var cell = row.GetCell(Mapping.ColumnIndex);
            var valType = Store.GetConcreteType(this, cell);

            //if there is any enumeration on the path this needs to be treated as a list of values
            if (HasEnumerationOnPath)
            {
                if (cell == null || cell.CellType != CellType.String || string.Equals(cell.StringCellValue, Mapping.DefaultValue, StringComparison.OrdinalIgnoreCase)) 
                    return;

                var strValue = cell.StringCellValue;
                if (!string.IsNullOrWhiteSpace(strValue))
                    Values =
                        strValue.Split(new[] {Store.Mapping.ListSeparator}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => Store.CreateSimpleValue(valType, v.Trim())).ToArray();
            }
            else
            {
                if (cell != null &&
                    (cell.CellType != CellType.String ||
                     !string.Equals(cell.StringCellValue, Mapping.DefaultValue, StringComparison.OrdinalIgnoreCase)))
                {
                    

                    Values = new[] {Store.CreateSimpleValue(valType, cell)};
                }
            }
        }

        private void AddMapping(PropertyMapping pMapping, List<string> segments)
        {
            if (segments == null || !segments.Any())
            {
                //this is a leaf
                Mapping = pMapping;
                return;
            }

            var segment = segments.First();
            var segmentName = segment.Split('\\')[0];

            //handle special cases - type hints
            switch (segment)
            {
                case "[table]":
                    TableHintMapping = pMapping;
                    return;
                case "[type]":
                    TypeHintMapping = pMapping;
                    return;
            }

            var existChild = Children.FirstOrDefault(c => c.Segment == segmentName);
            if (existChild != null)
                existChild.AddMapping(pMapping, segments.GetRange(1, segments.Count - 1));
            else
            {
                var child = new ReferenceContext(segment, this, Store, CMapping);
                child.AddMapping(pMapping, segments.GetRange(1, segments.Count - 1));
                Children.Add(child);
            }
        }
    }

    public enum ReferenceContextType
    {
        Root,
        Parent,
        Entity,
        EntityList,
        Scalar,
        ScalarList
    }
}
