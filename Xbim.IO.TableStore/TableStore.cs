using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;

namespace Xbim.IO.TableStore
{
    public partial class TableStore
    {
        public IModel Model { get; private set; }
        public ModelMapping Mapping { get; private set; }
        public ExpressMetaData MetaData { get { return Model.Metadata; } }

        #region Writing data out to a spreadsheet
        /// <summary>
        /// Limit of the length of the text in a cell before the row gets repeated if MultiRow == MultiRow.IfNecessary
        /// </summary>
        private const int CellTextLimit = 1024;

        //dictionary of all styles for different data statuses
        private Dictionary<DataStatus, ICellStyle> _styles;

        //cache of latest row number in different sheets
        private Dictionary<string, int> _rowNumCache = new Dictionary<string, int>();

        //cache of class mappings and respective express types
        private readonly Dictionary<ExpressType, ClassMapping> _typeClassMappingsCache = new Dictionary<ExpressType, ClassMapping>(); 

        //cache of meta properties so it doesn't have to look them up in metadata all the time
        private readonly  Dictionary<ExpressType, Dictionary<string, ExpressMetaProperty>> _typePropertyCache = new Dictionary<ExpressType, Dictionary<string, ExpressMetaProperty>>();

        // cache of index column indices for every table in use
        private Dictionary<string, int[]> _multiRowIndicesCache;

        //preprocessed enum aliases to speed things up
        private readonly Dictionary<string, string> _enumAliasesCache;
        private readonly Dictionary<string, string> _aliasesEnumCache;
 
        public TableStore(IModel model, ModelMapping mapping)
        {
            Model = model;
            Mapping = mapping;

            if (mapping.EnumerationMappings != null && mapping.EnumerationMappings.Any())
            {
                _enumAliasesCache = new Dictionary<string, string>();
                _aliasesEnumCache = new Dictionary<string, string>();
                foreach (var enumMapping in mapping.EnumerationMappings)
                {
                    if (enumMapping.Aliases == null || !enumMapping.Aliases.Any())
                        continue;
                    foreach (var alias in enumMapping.Aliases)
                    {
                        _enumAliasesCache.Add(enumMapping.Enumeration + "." + alias.EnumMember, alias.Alias);
                        _aliasesEnumCache.Add(enumMapping.Enumeration + "." + alias.Alias, alias.EnumMember);
                    }
                }
            }
            else
            {
                _enumAliasesCache = null;
                _aliasesEnumCache = null;
            }

            Mapping.Init(MetaData);
        }

        public void Store(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            var ext = Path.GetExtension(path).ToLower().Trim('.');
            if (ext != "xls" && ext != "xlsx")
            {
                //XLSX is Spreadsheet XML representation which is capable of storing more data
                path += ".xlsx";
                ext = "xlsx";
            }
            using (var file = File.Create(path))
            {
                var type = ext == "xlsx" ? ExcelTypeEnum.XLSX : ExcelTypeEnum.XLS;
                Store(file, type);
                file.Close();
            }

            
        }

        public void Store(Stream stream, ExcelTypeEnum type, Stream template = null, bool recalculate = false)
        {
            IWorkbook workbook;
            switch (type)
            {
                case ExcelTypeEnum.XLS:
                    workbook = template != null ? new HSSFWorkbook(template) : new HSSFWorkbook();
                    break;
                case ExcelTypeEnum.XLSX: //this is as it should be according to a standard
                    workbook = template != null ? new XSSFWorkbook(template) : new XSSFWorkbook();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            //create spreadsheet representaion 
            Store(workbook);
            if (!recalculate || template == null)
            {
                workbook.Write(stream);
                return;
            }

            //refresh formulas
            switch (type)
            {
                case ExcelTypeEnum.XLS:
                    HSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
                    break;
                case ExcelTypeEnum.XLSX:
                    XSSFFormulaEvaluator.EvaluateAllFormulaCells(workbook);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            //write to output stream
            workbook.Write(stream);
        }

        public void Store(IWorkbook workbook)
        {
            //if there are no mappings do nothing
            if (Mapping.ClassMappings == null || !Mapping.ClassMappings.Any()) return;

            _rowNumCache = new Dictionary<string, int>();
            _styles = new Dictionary<DataStatus, ICellStyle>();

            //creates tables in defined order if they are not there yet
            SetUpTables(workbook, Mapping);

            //start from root definitions
            var rootClasses = Mapping.ClassMappings.Where(m => m.IsRoot);
            foreach (var classMapping in rootClasses)
            {
                if (classMapping.PropertyMappings == null) 
                    continue;
                
                var eType = classMapping.Type;
                if (eType == null)
                {
                    Debug.WriteLine("Type not found: " + classMapping.Class);
                    continue;
                }

                //root definitions will always have parent == null
                Store(workbook, classMapping, eType, null);
            }
        }

        private void Store(IWorkbook workbook, ClassMapping mapping, ExpressType expType, IPersistEntity parent)
        {
            if (mapping.PropertyMappings == null)
                return;

            var context = parent == null ?
                new EntityContext(Model.Instances.OfType(expType.Name.ToUpper(), false).ToList()){ LeavesDepth = 1 } :
                mapping.GetContext(parent);

            if(!context.Leaves.Any()) return;

            var tableName = mapping.TableName ?? "Default";
            var sheet = workbook.GetSheet(tableName) ?? workbook.CreateSheet(tableName);

            foreach (var leaveContext in context.Leaves)
            {
                Store(sheet, leaveContext.Entity, mapping, expType, leaveContext);

                foreach (var childrenMapping in mapping.ChildrenMappings)
                {
                    Store(workbook, childrenMapping, childrenMapping.Type, leaveContext.Entity);
                }
            }
        }

        private void Store(ISheet sheet, IPersistEntity entity, ClassMapping mapping, ExpressType expType, EntityContext context)
        {
            var multiRow = -1;
            List<string> multiValues = null;
            PropertyMapping multiMapping = null;
            var row = GetRow(sheet);

            foreach (var propertyMapping in mapping.PropertyMappings)
            {
                object value = null;
                foreach (var path in propertyMapping.PathsEnumeration)
                {
                    value = GetValue(entity, expType, path, context);
                    if (value != null) break;
                }
                if (value == null && propertyMapping.Status == DataStatus.Required)
                    value = propertyMapping.DefaultValue ?? "n/a";

                var isMultiRow = IsMultiRow(value, propertyMapping);
                if (isMultiRow)
                {
                    multiRow = row.RowNum;
                    var values = new List<string>();
                    var enumerable = value as IEnumerable<string>;
                    if (enumerable != null)
                        values.AddRange(enumerable);

                    //get only first value and store it
                    var first = values.First();
                    Store(row, first, propertyMapping);

                    //set the rest for the processing as multiValue
                    values.Remove(first);
                    multiValues = values;
                    multiMapping = propertyMapping;
                }
                else
                {
                    Store(row, value, propertyMapping);
                }

            }

            if (row.RowNum == 1 || row.RowNum == 8)
            {
                //adjust width of the columns after the first and the eight row 
                //adjusting fully populated workbook takes ages. This should be almost all right
                AdjustAllColumns(sheet, mapping);
            }

            //add repeated rows if necessary
            if (multiRow <= 0 || multiValues == null || !multiValues.Any()) return;

            foreach (var value in multiValues)
            {
                var rowNum = GetNextRowNum(sheet);
                var copy = sheet.CopyRow(multiRow, rowNum);
                Store(copy, value, multiMapping);    
            }
            
        }


        private bool IsMultiRow(object value, PropertyMapping mapping)
        {
            if (value == null)
                return false;
            if (mapping.MultiRow == MultiRow.None) 
                return false;

            var values = value as IEnumerable<string>;
            if (values == null) 
                return false;

            var strings = values.ToList();
            var count = strings.Count;
            if (count > 1 && mapping.MultiRow == MultiRow.Always)
                return true;

            var single = string.Join(Mapping.ListSeparator, strings);
            return single.Length > CellTextLimit && mapping.MultiRow == MultiRow.IfNecessary;
        }

        private void Store(IRow row, object value, PropertyMapping mapping)
        {
            if (value == null)
                return;

            var cellIndex = CellReference.ConvertColStringToIndex(mapping.Column);
            var cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);

            //set column style to cell
            cell.CellStyle = row.Sheet.GetColumnStyle(cellIndex);

            //simplify any eventual enumeration into a single string
            var enumVal = value as IEnumerable;
            if (enumVal != null && !(value is string))
            {
                var strValue = string.Join(Mapping.ListSeparator, enumVal.Cast<object>());
                cell.SetCellType(CellType.String);
                cell.SetCellValue(strValue);
                return;
            }

            //string value
            var str = value as string;
            if (str != null)
            {
                cell.SetCellType(CellType.String);
                cell.SetCellValue(str);
                return;
            }

            //numeric point types
            if (value is double || value is float || value is int || value is long || value is short || value is byte || value is uint || value is ulong ||
                value is ushort)
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(Convert.ToDouble(value));
                return;
            }

            //boolean value
            if (value is bool)
            {
                cell.SetCellType(CellType.Boolean);
                cell.SetCellValue((bool)value);
                return;
            }

            //enumeration
            if (value is Enum)
            {
                var eType = value.GetType();
                var eValue = Enum.GetName(eType, value);
                    cell.SetCellType(CellType.String);
                //try to get alias from configuration
                var alias = GetEnumAlias(eType, eValue);
                cell.SetCellValue(alias ?? eValue);
                return;
            }

            throw new NotSupportedException("Only base types are supported");
        }

        private string GetEnumAlias(Type type, string value)
        {
            string result;
            return _enumAliasesCache.TryGetValue(type.Name + "." + value, out result) ? result : null;
        }

        private ClassMapping GetTable(ExpressType type)
        {
            ClassMapping mapping;
            if (_typeClassMappingsCache.TryGetValue(type, out mapping))
                return mapping;

            var mappings = Mapping.ClassMappings.Where(m => m.Type == type || m.Type.AllSubTypes.Contains(type)).ToList();
            if (!mappings.Any())
                throw new XbimException("No sheet mapping defined for " + type.Name);

            var root = mappings.FirstOrDefault(m => m.IsRoot);
            if (root != null)
            {
                _typeClassMappingsCache.Add(type, root);
                return root;
            }

            mapping = mappings.FirstOrDefault();
            _typeClassMappingsCache.Add(type, mapping);
            return mapping;
        }

        private object GetValue(IPersistEntity entity, ExpressType type, string path, EntityContext context)
        {
            while (true)
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new XbimException("Path not defined");

                //if it is parent, skip to the root of the context
                //optimization: check first letter before StartsWith() function. 
                if (path[0] == 'p' && path.StartsWith("parent."))
                {
                    if (context == null)
                        return null;

                    path = path.Substring(7); //trim "parent." from the beginning
                    entity = context.RootEntity;
                    type = entity.ExpressType;
                    context = null;
                    continue;
                }

                //one level up in the context hierarchy
                //optimization: check first letter before StartsWith() function. 
                if (path[0] == '(' && path.StartsWith("()."))
                {
                    if (context == null)
                        return null;

                    path = path.Substring(3); //trim "()." from the beginning
                    entity = context.Parent.Entity;
                    type = entity.ExpressType;
                    context = context.Parent;
                    continue;
                }

                if (string.Equals(path, "[table]", StringComparison.Ordinal))
                {
                    var mapping = GetTable(type);
                    return mapping.TableName;
                }

                if (string.Equals(path, "[type]", StringComparison.Ordinal))
                {
                    return entity.ExpressType.ExpressName;
                }

                var parts = path.Split('.');
                var multiResult = new List<string>();
                for (var i = 0; i < parts.Length; i++)
                {
                    var value = GetPropertyValue(parts[i], entity, type);

                    if (value == null)
                        return null;

                    var ent = value as IPersistEntity;
                    if (ent != null)
                    {
                        entity = ent;
                        type = ent.ExpressType;
                        continue;
                    }

                    var expVal = value as IExpressValueType;
                    if (expVal != null)
                    {
                        //if the type of the value is what we want
                        if (i < parts.Length - 1 && parts[parts.Length - 1] == "[type]")
                            return expVal.GetType().Name;
                        //return actual value as an underlying system type
                        return expVal.Value;
                    }

                    var expValEnum = value as IEnumerable<IExpressValueType>;
                    if (expValEnum != null)
                        return expValEnum.Select(v => v.Value);

                    var entEnum = value as IEnumerable<IPersistEntity>;
                    //it must be a simple value
                    if (entEnum == null) return value;

                    //it is a multivalue result
                    var subParts = parts.ToList().GetRange(i + 1, parts.Length - i - 1);
                    var subPath = string.Join(".", subParts);
                    foreach (var persistEntity in entEnum)
                    {
                        var subValue = GetValue(persistEntity, persistEntity.ExpressType, subPath, null);
                        if (subValue == null) continue;
                        var subString = subValue as string;
                        if (subString != null)
                        {
                            multiResult.Add(subString);
                            continue;
                        }
                        multiResult.Add(subValue.ToString());
                    }
                    return multiResult;

                }

                //if there is only entity itself to return, try to get 'Name' or 'Value' property as a fallback
                return GetFallbackValue(entity, type);
            }
        }

        private ExpressMetaProperty GetProperty(ExpressType type, string name)
        {
            ExpressMetaProperty property;
            Dictionary<string, ExpressMetaProperty> properties;
            if (!_typePropertyCache.TryGetValue(type, out properties))
            {
                properties = new Dictionary<string, ExpressMetaProperty>();
                _typePropertyCache.Add(type, properties);
            }
            if (properties.TryGetValue(name, out property))
                return property;

            property = type.Properties.Values.FirstOrDefault(p => p.Name == name) ??
                    type.Inverses.FirstOrDefault(p => p.Name == name) ??
                    type.Derives.FirstOrDefault(p => p.Name == name);
            if (property == null)
                return null;

            properties.Add(name, property);
            return property;
        }

        private object GetPropertyValue(string pathPart, IPersistEntity entity, ExpressType type)
        {
            var propName = pathPart;
            var propIndex = GetPropertyIndex(ref propName);
            var pInfo = GetPropertyInfo(propName, type, propIndex);
            var value = pInfo.GetValue(entity, propIndex == null ? null : new[] { propIndex });
            return value;
        }

        private PropertyInfo GetPropertyInfo(string name, ExpressType type, object index)
        {
            var isIndexed = index != null;
            PropertyInfo pInfo;
            if (isIndexed && string.IsNullOrWhiteSpace(name))
            {
                pInfo = type.Type.GetProperty("Item"); //anonymous index accessors are automatically named 'Item' by compiler
                if (pInfo == null)
                    throw new XbimException(string.Format("{0} doesn't have an index access", type.Name));

                var iParams = pInfo.GetIndexParameters();
                if (iParams.All(p => p.ParameterType != index.GetType()))
                    throw new XbimException(string.Format("{0} doesn't have an index access for type {1}", type.Name, index.GetType().Name));
            }
            else
            {
                var expProp = GetProperty(type, name);
                if (expProp == null)
                    throw new XbimException(string.Format("It wasn't possible to find property {0} in the object of type {1}", name, type.Name));
                pInfo = expProp.PropertyInfo;
                if (isIndexed)
                {
                    var iParams = pInfo.GetIndexParameters();
                    if (iParams.All(p => p.ParameterType != index.GetType()))
                        throw new XbimException(string.Format("Property {0} in the object of type {1} doesn't have an index access for type {2}", name, type.Name, index.GetType().Name));
                }
            }
            return pInfo;
        }

        private static object GetPropertyIndex(ref string pathPart)
        {
            var isIndexed = pathPart.Contains("[") && pathPart.Contains("]");
            if (!isIndexed) return null;

            object propIndex;
            var match = new Regex("((?<name>).+)?\\[(?<index>.+)\\]")
                .Match(pathPart);
            var indexString = match.Groups["index"].Value;
            pathPart = match.Groups["name"].Value;

            if (indexString.Contains("'") || indexString.Contains("\""))
            {
                propIndex = indexString.Trim('\'', '"');
            }
            else
            {
                //try to convert it to integer access
                int indexInt;
                if (int.TryParse(indexString, out indexInt))
                    propIndex = indexInt;
                else
                    propIndex = indexString;
            }

            return propIndex;
        }

        private static object GetFallbackValue(IPersistEntity entity, ExpressType type)
        {
            var nameProp = type.Properties.Values.FirstOrDefault(p => p.Name == "Name");
            var valProp = type.Properties.Values.FirstOrDefault(p => p.Name == "Value");
            if (nameProp == null && valProp == null)
                return entity.ToString();

            if (nameProp != null && valProp != null)
            {
                var nValue = nameProp.PropertyInfo.GetValue(entity, null);
                var vValue = valProp.PropertyInfo.GetValue(entity, null);
                if (nValue != null && vValue != null)
                    return string.Join(":", vValue, nValue);
            }

            if (nameProp != null)
            {
                var nameValue = nameProp.PropertyInfo.GetValue(entity, null);
                if (nameValue != null)
                    return nameValue.ToString();
            }

            if (valProp != null)
            {
                var valValue = valProp.PropertyInfo.GetValue(entity, null);
                if (valValue != null)
                    return valValue.ToString();
            }
            return entity.ToString();
        }

        private IRow GetRow(ISheet sheet)
        {
            //get the next row in rowNumber is less than 1 or use the argument to get or create new row
            int lastIndex;
            if (!_rowNumCache.TryGetValue(sheet.SheetName, out lastIndex))
            {
                lastIndex = -1;
                _rowNumCache.Add(sheet.SheetName, -1);
            }
            var row = lastIndex < 0
                ? GetNextEmptyRow(sheet)
                : (sheet.GetRow(lastIndex + 1) ?? sheet.CreateRow(lastIndex + 1));

            if (row.RowNum == 0)
                row = sheet.CreateRow(1);

            //cache the latest row index
            _rowNumCache[sheet.SheetName] = row.RowNum;
            return row;
        }

        private int GetNextRowNum(ISheet sheet)
        {
            int lastIndex;
            //no raws were created in this sheet so far
            if (!_rowNumCache.TryGetValue(sheet.SheetName, out lastIndex))
                return -1;

            lastIndex++;
            _rowNumCache[sheet.SheetName] = lastIndex;
            return lastIndex;
        }

        private static IRow GetNextEmptyRow(ISheet sheet)
        {
            foreach (IRow row in sheet)
            {
                var isEmpty = true;
                foreach (ICell cell in row)
                {
                    if (cell.CellType == CellType.Blank) continue;

                    isEmpty = false;
                    break;
                }
                if (isEmpty) return row;
            }
            return sheet.CreateRow(sheet.LastRowNum + 1);
        }



        private void SetUpHeader(ISheet sheet, ClassMapping classMapping)
        {
            var workbook = sheet.Workbook;
            var row = sheet.GetRow(0) ?? sheet.CreateRow(0);
            InitMappingColumns(classMapping);

            //create header and column style for every mapped column
            foreach (var mapping in classMapping.PropertyMappings)
            {
                var cellIndex = CellReference.ConvertColStringToIndex(mapping.Column);
                var cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(mapping.Header);
                cell.CellStyle = GetStyle(DataStatus.Header, workbook);

                //set default column style if not defined but available
                var style = GetStyle(mapping.Status, workbook);
                if(mapping.Status == DataStatus.None) continue;
                var existStyle = sheet.GetColumnStyle(cellIndex);
                if (
                    existStyle != null && 
                    existStyle.FillForegroundColor == style.FillForegroundColor &&
                    existStyle.BorderTop == style.BorderTop &&
                    existStyle.TopBorderColor == style.TopBorderColor
                    ) continue;

                //create new style
                sheet.SetDefaultColumnStyle(cellIndex, style);
                sheet.SetColumnHidden(cellIndex, false);
                //set default width
                sheet.SetColumnWidth(cellIndex, 256*15);
                //hide if defined
                if (mapping.Hidden)
                    sheet.SetColumnHidden(cellIndex, true);
            }
        }

        private static void InitMappingColumns(ClassMapping mapping)
        {
            if (mapping.PropertyMappings == null || 
                !mapping.PropertyMappings.Any() ||
                mapping.PropertyMappings.All(m => !string.IsNullOrWhiteSpace(m.Column)))
                return;

            var letter = 'A';
            var number = (int) letter;
            foreach (var pMapping in mapping.PropertyMappings)
            {
                pMapping.Column = ((char)number++).ToString();
            }

        }

        private ICellStyle GetStyle(DataStatus status, IWorkbook workbook)
        {
            if(_styles == null)
                _styles = new Dictionary<DataStatus, ICellStyle>();

            ICellStyle style;
            if (_styles.TryGetValue(status, out style))
                return style;

            style = workbook.CreateCellStyle();
            var representation = Mapping.StatusRepresentations.FirstOrDefault(r => r.Status == status);
            if (representation == null)
            {
                _styles.Add(status, style);
                return style;
            }

            style.FillPattern = FillPattern.SolidForeground;
            style.FillForegroundColor = GetClosestColour(representation.Colour);
            if (representation.Border)
            {
                style.BorderBottom = style.BorderTop = style.BorderLeft = style.BorderRight
                    = BorderStyle.Thin;
                style.BottomBorderColor = style.TopBorderColor = style.LeftBorderColor = style.RightBorderColor
                    = IndexedColors.Black.Index;    
            }
            var font = workbook.CreateFont();
            switch (representation.FontWeight)
            {
                case FontWeight.Normal:
                    break;
                case FontWeight.Bold:
                    font.Boldweight = (short) FontBoldWeight.Bold;
                    break;
                case FontWeight.Italics:
                    font.IsItalic = true;
                    break;
                case FontWeight.BoldItalics:
                    font.Boldweight = (short) FontBoldWeight.Bold;
                    font.IsItalic = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            style.SetFont(font);
            _styles.Add(status, style);
            return style;
        }

        //This operation takes very long time if applied at the end when spreadsheet is fully populated
        private static void AdjustAllColumns(ISheet sheet, ClassMapping mapping)
        {
            foreach (var propertyMapping in mapping.PropertyMappings)
            {
                var colIndex = CellReference.ConvertColStringToIndex(propertyMapping.Column);
                sheet.AutoSizeColumn(colIndex);
            }
        }

        private void SetUpTables(IWorkbook workbook, ModelMapping mapping)
        {
            if (mapping == null || mapping.ClassMappings == null || !mapping.ClassMappings.Any())
                return;

            var i = 0;
            foreach (var classMapping in Mapping.ClassMappings.Where(classMapping => string.IsNullOrWhiteSpace(classMapping.TableName)))
            {
                classMapping.TableName = string.Format("NoName({0})", i++);
            }

            var names = Mapping.ClassMappings.OrderBy(m => m.TableOrder).Select(m => m.TableName).Distinct();
            foreach (var name in names)
            {
                var sheet = workbook.GetSheet(name) ?? workbook.CreateSheet(name);
                var classMapping = Mapping.ClassMappings.First(m => m.TableName == name);
                SetUpHeader(sheet, classMapping);

                ////set colour of the tab: Not implemented exception in NPOI
                //if (classMapping.TableStatus == DataStatus.None) continue;
                //var style = GetStyle(classMapping.TableStatus, workbook);
                //sheet.TabColorIndex = style.FillForegroundColor;
            }
        }

        private static readonly Dictionary<string, short> ColourCodeCache = new Dictionary<string, short>();
        private static readonly List<IndexedColors> IndexedColoursList = new List<IndexedColors>();

        private static short GetClosestColour(string rgb)
        {
            if (!IndexedColoursList.Any())
            {
                var props = typeof (IndexedColors).GetFields(BindingFlags.Static | BindingFlags.Public).Where(p => p.FieldType == typeof (IndexedColors));
                foreach (var info in props)
                {
                    IndexedColoursList.Add((IndexedColors) info.GetValue(null));
                }
            }

            if (string.IsNullOrWhiteSpace(rgb))
                return IndexedColors.Automatic.Index;
            rgb = rgb.Trim('#').Trim();
            short result;
            if (ColourCodeCache.TryGetValue(rgb, out result))
                return result;

            var triplet = rgb.Length == 3;
            var hR = triplet ? rgb.Substring(0, 1) + rgb.Substring(0, 1) : rgb.Substring(0, 2);
            var hG = triplet ? rgb.Substring(1, 1) + rgb.Substring(1, 1) : rgb.Substring(2, 2);
            var hB = triplet ? rgb.Substring(2, 1) + rgb.Substring(1, 1) : rgb.Substring(4, 2);

            var r = Convert.ToByte(hR, 16);
            var g = Convert.ToByte(hG, 16);
            var b = Convert.ToByte(hB, 16);

            var rgbBytes = new[] {r, g, b};
            var distance = double.NaN;
            var colour = IndexedColors.Automatic;
            foreach (var col in IndexedColoursList)
            {
                var dist = ColourDistance(rgbBytes, col.RGB);
                if (double.IsNaN(distance)) distance = dist;

                if (!(distance > dist)) continue;
                distance = dist;
                colour = col;
            }
            ColourCodeCache.Add(rgb, colour.Index);
            return colour.Index;
        }

        private static double ColourDistance(byte[] a, byte[] b)
        {
            return Math.Sqrt(Math.Pow(a[0] - b[0], 2) + Math.Pow(a[1] - b[1], 2) + Math.Pow(a[2] - b[2], 2));
        }
        #endregion
    }

    public enum ExcelTypeEnum
    {
        XLS,
        XLSX
    }
}
