using System;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using Xbim.Common.Metadata;
using Xbim.IO.TableStore;
using Xbim.IO.TableStore.Resolvers;

namespace Xbim.CobieExpress.IO.Resolvers
{
    public class AttributeTypeResolver: ITypeResolver
    {
        public bool CanResolve(Type type)
        {
            return type == typeof (AttributeValue);
        }

        public bool CanResolve(ExpressType type)
        {
            return CanResolve(type.Type); 
        }

        private static readonly Regex DateTimeRegex = new Regex("[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}",
                            RegexOptions.Compiled);
        private static readonly Regex FirstLetterRegex = new Regex("^[0-9].*",
                            RegexOptions.Compiled);


        public Type Resolve(Type type, ICell cell, ClassMapping cMapping, PropertyMapping pMapping)
        {
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    //it might be integer or float
                    var value = cell.NumericCellValue;
                    return Math.Abs(value%1) < 1e-9 ? typeof (IntegerValue) : typeof (FloatValue);
                case CellType.String:
                    //it might be string or datetime
                    var str = cell.StringCellValue;
                    if (str.Length >= 19 && FirstLetterRegex.IsMatch(str[0].ToString())) //2009-06-15T13:45:30
                    {
                        var dStr = str.Substring(0, 19);
                        if (DateTimeRegex.IsMatch(dStr))
                            return typeof (DateTimeValue);
                    }
                    return typeof (StringValue);
                case CellType.Boolean:
                    return typeof (BooleanValue);
            }
            return typeof(StringValue);
        }

        public ExpressType Resolve(ExpressType abstractType, ReferenceContext context, ExpressMetaData metaData)
        {
            return metaData.ExpressType(typeof (StringValue));
        }
    }
}
