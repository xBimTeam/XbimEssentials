using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    internal class ReferenceContext
    {
        public string Segment { get; set; }
        public List<PropertyMapping> Mappings { get; private set; }
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
        private ClassMapping CMapping { get; set; }
        public object[] KeyValues { get; private set; }
        
        /// <summary>
        /// Only scalar children. These can be used to find an object
        /// </summary>
        public IEnumerable<ReferenceContext> ScalarChildren { get
        {
            return
                Children.Where(
                    c =>
                        c.ContextType == ReferenceContextType.Scalar || c.ContextType == ReferenceContextType.ScalarList);
        }}

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

        public ReferenceContext(TableStore store, ClassMapping cMapping)
        {
            Store = store;
            CMapping = cMapping;
            Mappings = new List<PropertyMapping>();
            Children = new List<ReferenceContext>();
            ContextType = ReferenceContextType.Root;
            PathTypeHint = Store.MetaData.ExpressType(cMapping.Class);

            if (cMapping.PropertyMappings == null || !cMapping.PropertyMappings.Any())
                return;

            var parentPath = cMapping.ParentPath.Split('.').ToList();
            foreach (var mapping in cMapping.PropertyMappings)
            {
                var path = mapping.Paths.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var segments = path.Split('.').ToList();

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


        }

        private ReferenceContext(string segment, ReferenceContext parent)
        {
            //init lists
            Mappings = new List<PropertyMapping>();
            Children = new List<ReferenceContext>();
            ParentContext = parent;

            //try to extract TypeOf part of the path
            var parts = segment.Split('\\');
            segment = parts[0];
            var typeHint = parts.Length > 1 ? parts[1] : null;

            Segment = segment;
            //set up path type hint if it is defined
            if (!string.IsNullOrWhiteSpace(typeHint))
                PathTypeHint = Store.MetaData.ExpressType(typeHint);

            if (segment == "parent")
            {
                PathTypeHint = Store.MetaData.ExpressType(CMapping.ParentClass);
                ContextType = ReferenceContextType.Entity;
                return;
            }

            MetaProperty = Store.GetProperty(parent.SegmentType, segment);
            if (MetaProperty == null)
            {
                Store.Log.WriteLine("Type {0} doesn't have a property {1}.", parent.PathTypeHint.ExpressName, segment);
                return;
            }

            
            PropertyTypeHint = Store.MetaData.ExpressType(MetaProperty.EnumerableType ?? MetaProperty.PropertyInfo.PropertyType);

            //set up type of the context
            var isEnumerable = MetaProperty.EnumerableType != null;
            if (isEnumerable)
            {
                if(MetaProperty.EnumerableType.IsValueType || MetaProperty.EnumerableType == typeof(string))
                    ContextType = ReferenceContextType.ScalarList;
                else
                    ContextType = ReferenceContextType.EntityList;
            }
            else
            {
                if (MetaProperty.PropertyInfo.PropertyType.IsValueType || MetaProperty.PropertyInfo.PropertyType == typeof(string))
                    ContextType = ReferenceContextType.Scalar;
                else
                    ContextType = ReferenceContextType.Entity;
            }

        }

        public void LoadData(IRow row)
        {  
            //clear any old data
            KeyValues = null;
            TableTypeHint = null;
            TypeTypeHint = null;

            //load child data
            foreach (var child in Children)
                child.LoadData(row);
            
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
            
            //if there is any enumeration on the path this needs to be treated as a list of values
            if (HasEnumerationOnPath)
            {
                var map = Mappings.First();
                var cell = row.GetCell(map.ColumnIndex);
                if (cell == null || cell.CellType != CellType.String) 
                    return;

                var strValue = cell.StringCellValue;
                if (!string.IsNullOrWhiteSpace(strValue))
                    KeyValues =
                        strValue.Split(new[] {Store.Mapping.ListSeparator}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => Store.CreateSimpleValue(SegmentType.Type, v.Trim())).ToArray();
            }
            else
            {
                var map = Mappings.First();
                var cell = row.GetCell(map.ColumnIndex);
                if (cell != null)
                    KeyValues = new [] { Store.CreateSimpleValue(SegmentType.Type, cell) };
            }
        }

        private void AddMapping(PropertyMapping pMapping, List<string> segments)
        {
            Mappings.Add(pMapping);
            if (segments == null || !segments.Any())
                return;

            var segment = segments.First();
            var segmentName = segment.Split('\\')[0];

            //handle special cases
            if (segments.Count != 1)
            {
                var nextSegment = segments[1].Split('\\')[0];
                switch (nextSegment)
                {
                    case "[table]":
                        TableHintMapping = pMapping;
                        return;
                    case "[type]":
                        TypeHintMapping = pMapping;
                        return;
                }
            }

            var existChild = Children.FirstOrDefault(c => c.Segment == segmentName);
            if (existChild != null)
                existChild.AddMapping(pMapping, segments.GetRange(1, segments.Count - 1));
            else
            {
                var child = new ReferenceContext(segment, this) { Store = Store, CMapping = CMapping };
                child.AddMapping(pMapping, segments.GetRange(1, segments.Count - 1));
                Children.Add(child);
            }
        }
    }

    internal enum ReferenceContextType
    {
        Root,
        Entity,
        EntityList,
        Scalar,
        ScalarList
    }
}
