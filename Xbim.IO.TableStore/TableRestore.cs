using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        #region Reading in from a spreadsheet
        public void LoadFrom(string path)
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

                LoadFromSheet(sheet, mapping);
            }
        }

        private void LoadFromSheet(ISheet sheet, ClassMapping mapping)
        {
            //if there is only header in a sheet, don't waste resources
            if (sheet.LastRowNum < 1)
                return;

            //adjust mapping to sheet in case columns are in a different order
            AdjustMapping(sheet, mapping);

            //iterate over rows (be careful about MultiRow != None, merge values if necessary)
            var enumerator = sheet.GetRowEnumerator();
            IRow lastRow = null;
            IPersistEntity lastEntity = null;
            while (enumerator.MoveNext())
            {
                var row = enumerator.Current as IRow;
                //skip header row
                if (row == null || row.RowNum == 0)
                    continue;

                //last row might be used in case this is a MultiRow
                lastEntity = LoadFromRow(row, mapping, lastRow, lastEntity);
                lastRow = row;
            }
        }

        private IPersistEntity LoadFromRow(IRow row, ClassMapping mapping, IRow lastRow, IPersistEntity lastEntity)
        {
            var multirow = mapping.PropertyMappings.Any(p => p.MultiRow != MultiRow.None);

            //get type of the coresponding object from ClassMapping or from a type hint, create instance
            //fill in simple value fields
            //reconstruct references (possibly forward references)
            //be happy

            throw new NotImplementedException();
        }

        private ExpressType GetConcreteType(IRow row, ClassMapping mapping)
        {
            string typeName = null;

            //type hint property has priority
            var hintProperty =
                mapping.PropertyMappings.FirstOrDefault(
                    m => string.Equals(m.Paths, "[type]", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(m.Column));

            if (hintProperty != null)
            {
                var i = CellReference.ConvertColStringToIndex(hintProperty.Column);
                var hintCell = row.GetCell(i);
                if (hintCell != null && hintCell.CellType == CellType.String)
                    typeName = hintCell.StringCellValue;
            }
            else
                typeName = mapping.Class;

            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            var eType = MetaData.ExpressType(typeName.ToUpper());
            if (eType != null && !eType.Type.IsAbstract)
                return eType;

            //use fallback to retrieve a non-abstract type (defined in a configuration file?)
            throw new NotImplementedException();
        }

        private static void AdjustMapping(ISheet sheet, ClassMapping mapping)
        {
            var headerRow = sheet.GetRow(0);
            if (headerRow == null)
                return;

            var headings = headerRow.Cells
                .Where(c => c.CellType == CellType.String || !string.IsNullOrWhiteSpace(c.StringCellValue)).ToList();
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
                pMapping.Column = column;
                var current =
                    mappings.FirstOrDefault(m => string.Equals(m.Column, column, StringComparison.OrdinalIgnoreCase));
                if(current != null)
                    current.Column = null;

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
            if (!unassigned.Any())
                return;
            foreach (var propertyMapping in unassigned)
                mapping.PropertyMappings.Remove(propertyMapping);
        }

        private static Type GetNonNullableType(Type type)
        {
            //only value types can be nullable
            if (!type.IsValueType) return type;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Nullable.GetUnderlyingType(type)
                : type;
        }

        private void SetSimpleValue(PropertyInfo info, object obj, ICell cell, TextWriter log, object index = null)
        {
            //return if there is no value in she cell
            if (cell.CellType == CellType.Blank) return;

            var type = info.PropertyType;
            type = GetNonNullableType(type);

            //dig deeper if it is an express value type
            if (type.IsValueType && typeof (IExpressValueType).IsAssignableFrom(type))
            {
                var eType = MetaData.ExpressType(type);
                if (eType != null)
                    type = GetNonNullableType(eType.UnderlyingType);
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
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                info.SetValue(obj, value, index == null ? null : new []{ index });
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
                info.SetValue(obj, date, index == null ? null : new[] { index });
                return;
            }

            if (type == typeof(double) || type == typeof(float))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        info.SetValue(obj, cell.NumericCellValue, index == null ? null : new[] { index });
                        break;
                    case CellType.String:
                        double d;
                        if (double.TryParse(cell.StringCellValue, out d))
                            info.SetValue(obj, d, index == null ? null : new[] { index });
                        break;
                    default:
                        log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                            info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                            cell.Sheet.SheetName);
                        break;
                }
                return;
            }

            if (type == typeof(int) || type == typeof(long))
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                    case CellType.String:
                        info.SetValue(obj,
                            type == typeof (int)
                                ? Convert.ToInt32(cell.NumericCellValue)
                                : Convert.ToInt64(cell.NumericCellValue),
                            index == null ? null : new[] {index});
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
                        info.SetValue(obj, ((int)cell.NumericCellValue) != 0, index == null ? null : new[] { index });
                        break;
                    case CellType.String:
                        bool i;
                        if (bool.TryParse(cell.StringCellValue, out i))
                            info.SetValue(obj, i, index == null ? null : new[] { index });
                        else
                        {
                            log.WriteLine("Wrong boolean format of {0} in cell {1}{2}, sheet {3}",
                                cell.StringCellValue, CellReference.ConvertNumToColString(cell.ColumnIndex),
                                cell.RowIndex + 1,
                                cell.Sheet.SheetName);
                        }
                        break;
                    case CellType.Boolean:
                        info.SetValue(obj, cell.BooleanCellValue, index == null ? null : new[] { index });
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
                    var eValue = cell.StringCellValue;
                    var eMember = GetAliasEnumName(type, eValue);
                    //if there was no alias try to parse the value
                    var val = Enum.Parse(type, eMember ?? eValue, true);
                    info.SetValue(obj, val, index == null ? null : new[] { index });
                    return;
                }
                catch (Exception)
                {
                    log.WriteLine("There is no suitable value for {0} in cell {1}{2}, sheet {3}",
                        info.Name, CellReference.ConvertNumToColString(cell.ColumnIndex), cell.RowIndex + 1,
                        cell.Sheet.SheetName);
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
