using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Xbim.CobieExpress.IO.TableStore.TableMapping;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.CobieExpress.IO.TableStore
{
    public class TableStore
    {
        public IModel Model { get; private set; }
        public ModelMapping Mapping { get; private set; }
        public ExpressMetaData MetaData { get { return Model.Metadata; } }

        public TableStore(IModel model, ModelMapping mapping)
        {
            Model = model;
            Mapping = mapping;
        }

        public void Store(IWorkbook workbook)
        {
            //if there are no mappings do nothing
            if (Mapping.ClassMappings == null) return;
            _rowNumCache = new Dictionary<string, int>();

            var rootClasses = Mapping.ClassMappings.Where(m => m.IsRoot);
            foreach (var classMapping in rootClasses)
            {
                if (classMapping.PropertyMappings == null) 
                    continue;
                
                var eType = MetaData.ExpressType(classMapping.ClassName);
                if (eType == null)
                {
                    Debug.WriteLine("Type not found: " + classMapping.ClassName);
                    continue;
                }

                var entities = Model.Instances.OfType(eType.Name.ToUpper(), false);
                var tableName = classMapping.TableName ?? "Default";
                var sheet = workbook.GetSheet(tableName) ?? workbook.CreateSheet(tableName);
                
                SetUpHeader(sheet,classMapping);
                Store(sheet, entities, classMapping, eType);
            }

        }

        private void Store(ISheet sheet, IEnumerable<IPersistEntity> entities, ClassMapping mapping, ExpressType expType)
        {
            if (mapping.PropertyMappings == null)
                return;

            foreach (var entity in entities)
            {
                var multiRow = false;
                object multiValue;
                PropertyMapping multiMapping;
                foreach (var propertyMapping in mapping.PropertyMappings)
                {
                    object value = null;
                    foreach (var path in propertyMapping.Paths)
                    {
                        value = GetValue(entity, expType, path);
                        if (value != null) break;
                    }
                    if (value == null && propertyMapping.Required)
                        value = propertyMapping.DefaultValue ?? "n/a";

                    var row = GetRow(sheet, mapping);

                    var isMultiRow = IsMultiRow(value, propertyMapping);
                    multiRow = multiRow || isMultiRow;
                    if (isMultiRow)
                    {
                        var values = new List<string>();
                        var enumerable = value as IEnumerable<string>;
                        if (enumerable != null)
                            values.AddRange(enumerable);

                        //get only first value and store it
                        //set the rest for the processing as multiValue
                        multiMapping = propertyMapping;
                        throw new NotImplementedException();
                    }
                    else
                    {
                        Store(row, value, propertyMapping);    
                    }
                    
                }

                if (multiRow)
                {
                    //sheet.CopyRow()
                }
            }
        }

        private bool IsMultiRow(object value, PropertyMapping mapping)
        {
            if (value == null)
                return false;
            if (mapping.MultiRow == MultiRowRepresentation.None) 
                return false;

            var values = value as IEnumerable<string>;
            if (values == null) 
                return false;

            var strings = values.ToList();
            var count = strings.Count;
            if (count > 1 && mapping.MultiRow == MultiRowRepresentation.Always)
                return true;

            var single = string.Join(",", strings);
            if (single.Length > 255 && mapping.MultiRow == MultiRowRepresentation.IfNecessary)
                return true;

            return false;
        }

        private void Store(IRow row, object value, PropertyMapping mapping)
        {
            var cellIndex = CellReference.ConvertColStringToIndex(mapping.ColumnIndex);
            var cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);

            //set column style to cell
            cell.CellStyle = row.Sheet.GetColumnStyle(cellIndex);

            throw new NotImplementedException();
        }

        private object GetValue(IPersistEntity entity, ExpressType type, string path)
        {
            throw new NotImplementedException();

        }

        private Dictionary<string, int> _rowNumCache = new Dictionary<string, int>();
        private IRow GetRow(ISheet sheet, ClassMapping mappings)
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
            {
                //set up header if this is the very first row in the sheet
                SetUpHeader(sheet, mappings);
                row = sheet.CreateRow(1);
            }

            //cache the latest row index
            _rowNumCache[sheet.SheetName] = row.RowNum;
            return row;
        }

        private IRow GetNextEmptyRow(ISheet sheet)
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

            foreach (var mapping in classMapping.PropertyMappings)
            {
                var cellIndex = CellReference.ConvertColStringToIndex(mapping.ColumnIndex);
                var cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(mapping.Header);

                //set style if defined
                if(string.IsNullOrEmpty(mapping.Colour)) continue;
                var style = workbook.CreateCellStyle();
                style.FillPattern = FillPattern.SolidForeground;
                style.FillForegroundColor = GetClosestColour(mapping.Colour);
                sheet.SetDefaultColumnStyle(cellIndex, style);
            }
        }


        private static readonly Dictionary<string, short> ColourCodeCache = new Dictionary<string, short>(); 
        private short GetClosestColour(string rgb)
        {
            if (string.IsNullOrWhiteSpace(rgb))
                return IndexedColors.Automatic.Index;
            rgb = rgb.Trim('#').Trim();
            short result;
            if (ColourCodeCache.TryGetValue(rgb, out result))
                return result;

            var triplet = rgb.Length == 3;
            var hR = triplet ? rgb.Substring(0, 1) : rgb.Substring(0, 2);
            var hG = triplet ? rgb.Substring(1, 1) : rgb.Substring(2, 2);
            var hB = triplet ? rgb.Substring(2, 1) : rgb.Substring(4, 2);

            var r = Convert.ToByte(hR, 16);
            var g = Convert.ToByte(hG, 16);
            var b = Convert.ToByte(hB, 16);

            var rgbBytes = new[] {r,g,b};
            var distance = double.NaN;
            var colour = IndexedColors.Automatic;
            for (var i = 0; i < 48; i++)
            {
                var col = IndexedColors.ValueOf(i);
                var dist = ColourDistance(rgbBytes, col.RGB);
                if (double.IsNaN(distance)) distance = dist;

                if (!(distance > dist)) continue;
                distance = dist;
                colour = col;
            }

            ColourCodeCache.Add(rgb, colour.Index);
            return colour.Index;
        }

        private double ColourDistance(byte[] a, byte[] b)
        {
            return Math.Sqrt(Math.Pow(a[0] - b[0], 2) + Math.Pow(a[1] - b[1], 2) + Math.Pow(a[2] - b[2], 2));
        }

        #region Reading from Spreadsheet
        private static void SetSimpleValue(PropertyInfo info, object obj, ICell cell, TextWriter log)
        {
            var type = info.PropertyType;
            type = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Nullable.GetUnderlyingType(type)
                : type;

            if (typeof(string).IsAssignableFrom(type))
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
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                info.SetValue(obj, value);
                return;
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
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                info.SetValue(obj, date);
                return;
            }

            if (type == typeof(double))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        info.SetValue(obj, cell.NumericCellValue);
                        break;
                    case CellType.String:
                        double d;
                        if (double.TryParse(cell.StringCellValue, out d))
                            info.SetValue(obj, d);
                        break;
                    default:
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                return;
            }

            if (type == typeof(int))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        info.SetValue(obj, (int)cell.NumericCellValue);
                        break;
                    case CellType.String:
                        int i;
                        if (int.TryParse(cell.StringCellValue, out i))
                            info.SetValue(obj, i);
                        break;
                    default:
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                return;
            }

            if (type == typeof(bool))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        info.SetValue(obj, ((int)cell.NumericCellValue) != 0);
                        break;
                    case CellType.String:
                        bool i;
                        if (bool.TryParse(cell.StringCellValue, out i))
                            info.SetValue(obj, i);
                        else
                        {
                            log.WriteLine("Wrong boolean format of {0} in cell {1}{2}, sheet {3}",
                                cell.StringCellValue, CellReference.ConvertNumToColString(cell.ColumnIndex),
                                cell.RowIndex + 1,
                                cell.Sheet.SheetName);
                        }
                        break;
                    case CellType.Boolean:
                        info.SetValue(obj, cell.BooleanCellValue);
                        break;
                    default:
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                return;
            }

            //enumeration
            if (type.IsEnum)
            {
                if (cell.CellType != CellType.String)
                {
                    log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                        info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                        cell.Sheet.SheetName);
                    return;
                }
                try
                {
                    //if there was no alias try to parse the value
                    var val = Enum.Parse(type, cell.StringCellValue, true);
                    info.SetValue(obj, val);
                    return;
                }
                catch (Exception)
                {
                    log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                        info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                        cell.Sheet.SheetName);
                }
            }

            //if not suitable type was found, report it as a bug
            throw new Exception("Unsupported type " + type.Name + " for value '" + cell + "'");
        }
        #endregion
    }
}
