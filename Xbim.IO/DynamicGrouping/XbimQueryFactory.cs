using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using System.Linq.Expressions;
using System.Reflection;
using Xbim.XbimExtensions.SelectTypes;
using System.Collections;

namespace Xbim.IO.DynamicGrouping
{
    class XbimQueryFactory
    {
        /// <summary>
        /// Creates constant expression with promoted value to the same type as the left type.
        /// It is necessary for some of the operations that the types on both sides are same.
        /// If it is not possible to convert left argument null is returned in release compilation
        /// or exception is thrown in debug mode.
        /// </summary>
        /// <param name="leftType">Target type of the parameter</param>
        /// <returns>Expression how to handle input constant value to make it comparable with the type</returns>
        public static Expression PromoteToConstant(Type leftType, string rightValue)
        {
            //string is straight forward
            if (leftType == typeof(string)) return Expression.Constant(rightValue);

            //nullable types must be handled in a different way
            if (IsNullableType(leftType))
            {
                return PromoteToNullable(leftType, rightValue);
            }
            //express types are structures whith known structure
            if (typeof(ExpressType).IsAssignableFrom(leftType))
            {
                return PromoteExpressType(leftType, rightValue);
            }
            //Enumeration
            if (leftType.IsEnum)
            {
                return PromoteToEnumeration(leftType, rightValue);
            }
            //numeric value
            if (numericTypes.Contains(leftType))
            {
                return PromoteToNumericType(leftType, rightValue);
            }
            if (leftType == typeof(bool))
            {
                return PromoteToBool(leftType, rightValue);
            }

            //general structures and classes are not supported
#if DEBUG
            throw new Exception("Not supported type");
#else
            return null;
#endif
        }

        private static Expression PromoteToBool(Type type, string value)
        {
            bool result = false;
            if (bool.TryParse(value, out result))
            {
                return Expression.Constant(result, typeof(bool));
            }
#if DEBUG
            throw new Exception("Specified string does not represent boolean value");
#else
            return null;
#endif
        }

        private static Expression PromoteToNumericType(Type type, string value)
        {
            object number = ParseNumber(value, type);
            return Expression.Constant(number, type);
        }

        /// <summary>
        /// Promote string input value to the enumeration value
        /// </summary>
        /// <param name="type">Enumeration type</param>
        /// <param name="value">String input value</param>
        /// <returns></returns>
        private static Expression PromoteToEnumeration(Type type, string value)
        {
            if (!type.IsEnum) return null;

            object enu = ParseEnum(value, type);
            if (enu == null)
            {
#if DEBUG
                throw new Exception("Specified string does not represent any value trom the enumeration");
#else
                return null;
#endif
            }
            return Expression.Constant(enu, type);
        }

        private static Expression PromoteExpressType(Type type, string value)
        {
            if (!typeof(ExpressType).IsAssignableFrom(type)) return null;

            //create express type instance
            object expresObject = Activator.CreateInstance(type, new object[] { });

            //get underlying system type
            Type underType = typeof(ExpressType).GetProperty("UnderlyingSystemType").GetValue(expresObject, null) as Type;

            //underlying value should be either string or one of numeric types
            if (Type.GetTypeCode(underType) == TypeCode.String)
            {
                expresObject = Activator.CreateInstance(type, new object[] { value });

            }
            else if (numericTypes.Contains(underType))
            {
                object number = ParseNumber(value, underType);
                expresObject = Activator.CreateInstance(type, new object[] { number });
            }

            //it should not happen that value is not known at this point
            else
            {
#if DEBUG
                throw new Exception("Not supported underlying type of the ExpressType");
#else
                return null;
#endif
            }

            return Expression.Constant(expresObject);
        }

        private static Expression PromoteToNullable(Type type, string value)
        {
            Type underType = Nullable.GetUnderlyingType(type);
            ConstantExpression constExp = PromoteToConstant(underType, value) as ConstantExpression;
            if (constExp == null)
            {
#if DEBUG
                throw new Exception("Unexpected underlying type");
#else
                return null;
#endif
            }
            //get value (it can be string, number type or ExpressType)
            object val = constExp.Value;

            //create new nullable object and initialize it
            Type t = typeof(Nullable<>).MakeGenericType(underType);
            object nullable = Activator.CreateInstance(t, new object[] { val });

            if (nullable == null)
            {
#if DEBUG
                throw new Exception("Creation of the nullable type instance has failed.");
#else
                return null;
#endif
            }

            //return constant expression with appropriate nullable value
            return Expression.Constant(nullable, type);
        }

        /// <summary>
        /// List of numeric types to use appropriate parsing function
        /// </summary>
        private static readonly Type[] numericTypes = 
        {
            typeof(int),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(long),
            typeof(double),
            typeof(Double),
            typeof(float),
            typeof(decimal),
            typeof(Decimal),
            typeof(uint),
            typeof(UInt16),
            typeof(UInt32),
            typeof(UInt64),
            typeof(sbyte),
            typeof(SByte)
        };

        /// <summary>
        /// Generates 'Contains' expression for types inheriting from IEnumerable (like string, list, ...) and specific IFC types
        /// </summary>
        /// <param name="left">Left expression</param>
        /// <param name="right">Right expression</param>
        /// <returns>Expression with 'bool' type as result of binary expression</returns>
        public static Expression GenerateContains(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            Type leftType = GetNonNullableType(left.Type);
            Type rightType = GetNonNullableType(right.Type);

            //if both sides are of type assignabe from IEnumerable function is called (it includes string)
            if (typeof(IEnumerable).IsAssignableFrom(leftType) || typeof(ExpressType).IsAssignableFrom(leftType))
            {
                //convert nullable to its value or return false if it does not have any value
                if (leftType != left.Type && rightType != right.Type)
                {
                    // Creating an expression to access value inside 
                    Expression leftValue = Expression.Property(left, left.Type, "Value");
                    Expression rightValue = Expression.Property(right, right.Type, "Value");

                    return //recursive call
                        Expression.Condition(
                            Expression.Equal(Expression.Property(left, "HasValue"), Expression.Constant(true)),
                            GenerateContains(leftValue, rightValue),
                            Expression.Constant(false, typeof(bool))
                        );
                }

                //deal with ExpressType (this should never happen as 'UnwrapExpressType' is used at the beginning of the function)
                //else if (typeof(ExpressType).IsAssignableFrom(leftType))
                //{
                //    // Creating an expression to access value from Express type
                //    Expression leftValue = Expression.Property(left, left.Type, "Value");
                //    Expression rightValue = Expression.Property(right, right.Type, "Value");

                //    //get type and value and pass it recursively
                //    Expression type = Expression.Property(left, typeof(ExpressType), "UnderlyingSystemType");

                //    return //recursive call
                //        Expression.Condition(
                //            Expression.Equal(type, Expression.Constant(typeof(string))),
                //            GenerateContains(
                //                Expression.Convert(leftValue, typeof(string)),
                //                Expression.Convert(rightValue, typeof(string))),
                //            Expression.Constant(false, typeof(bool))
                //        );
                //}

                //base functionality is calling 'Contains' function on objects which support such a functionality (IEnumerable)
                else
                {
                    MethodInfo method = leftType.GetMethod("Contains");
                    return Expression.Call(left, method, right);
                }
            }
            return Expression.Constant(false, typeof(bool));
        }

        /// <summary>
        /// Unwraps ExpressType in the expression so that value is being accessed directly 
        /// and standard methods are than applicable on them. If EspressType is wrapped in 
        /// Nullable&lt;ExpressType&gt; generic function new Nullable instance is created and ExpressType core 
        /// value is used in this new object so that its behavior is the same as if just the 
        /// base types were used. If input is not ExpressType or Nullable&lt;ExpressType&gt;
        /// nothing happens.<br>
        /// This overloaded method is helper for the case when both sides of binary operation
        /// are to be unwrapped at one step. Left and right types should be the same.
        /// </summary>
        /// <param name="left">Left expression</param>
        /// <param name="right">Right expression</param>
        private static void UnwrapExpressType(ref Expression left, ref Expression right)
        {
            left = UnwrapExpressType(left);
            right = UnwrapExpressType(right);
        }

        /// <summary>
        /// Unwraps ExpressType in the expression so that value is being accessed directly 
        /// and standard methods are than applicable on them. If EspressType is wrapped in 
        /// Nullable&lt;ExpressType&gt; generic function new Nullable instance is created and ExpressType core 
        /// value is used in this new object so that its behavior is the same as if just the 
        /// base types were used. If input is not ExpressType or Nullable&lt;ExpressType&gt;
        /// nothing happens.
        /// </summary>
        /// <param name="expression">Expression to be unwrapped</param>
        /// <returns>Unwrapped expression</returns>
        private static Expression UnwrapExpressType(Expression expression)
        {
            if (IsNullableType(expression.Type))
            {
                //process only if underlying type is ExpressType
                if (typeof(ExpressType).IsAssignableFrom(GetNonNullableType(expression.Type)))
                {
                    // Creating an expression to access value inside 
                    Expression value = Expression.Property(expression, expression.Type, "Value");

                    Type expressType = GetNonNullableType(expression.Type);
                    //create dummy express type instance to get the type
                    object expresObject = Activator.CreateInstance(expressType, new object[] { });
                    Type baseType = (expresObject as ExpressType).UnderlyingSystemType as Type;


                    if (baseType != typeof(string)) //string cannot be base type of Nullable
                    {
                        //create nullable null object to return if underlying express type is null
                        Type t = typeof(Nullable<>).MakeGenericType(baseType);
                        object nullable = Activator.CreateInstance(t, new object[] { });

                        //conditional expression
                        value = Expression.Condition(
                        Expression.Equal(Expression.Property(expression, "HasValue"), Expression.Constant(true)),
                            WrapToNullable(UnwrapExpressType(value), baseType), //recursive call to get underlying type of the ExpressType
                            Expression.Constant(nullable, typeof(Nullable<>))
                        );
                    }
                    else
                    { //if base type is string than it is used directly
                        string emptyStr = "";
                        value = Expression.Condition(
                        Expression.Equal(Expression.Property(expression, "HasValue"), Expression.Constant(true)),
                            Expression.Convert(UnwrapExpressType(value), typeof(string)),
                            Expression.Constant(emptyStr, typeof(string))
                        );
                    }
                    return value;
                }

                //if underlying type is not ExpressType nothing happens
                return expression;

            }
            else if (typeof(ExpressType).IsAssignableFrom(expression.Type))
            {
                // Creating an expression to access value from Express type in the expression
                Expression leftValue = Expression.Property(expression, typeof(ExpressType), "Value");  //type 'object'
                Expression underType = Expression.Property(expression, typeof(ExpressType), "UnderlyingSystemType"); //type 'Type'

                //MethodInfo castMethod = typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(BindingFlags) }); 
                //Expression castMethodExpr = Expression.Call(Expression.Constant(typeof(QueryFactory)), castMethod, Expression.Constant("Cast"), Expression.Constant(BindingFlags.Static | BindingFlags.NonPublic)); //Cast method

                ////create Expression Type[]
                //MethodInfo createMeth = typeof(QueryFactory).GetMethod("CreateTypeArray", BindingFlags.NonPublic| BindingFlags.Static);
                //Expression typeParam = Expression.Call(createMeth, underType);

                //MethodInfo castMethMake = typeof(MethodInfo).GetMethod("MakeGenericMethod"); 
                //Expression castMethMakeExpr = Expression.Call(castMethodExpr, castMethMake, typeParam);

                ////create Expression object[]
                //MethodInfo createObjArMeth = typeof(QueryFactory).GetMethod("CreateObjectArray", BindingFlags.NonPublic | BindingFlags.Static);
                //Expression objParam = Expression.Call(createObjArMeth, leftValue);

                //MethodInfo invokeMethod = typeof(MethodInfo).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }); //invoke method
                ////casting of the value (it would be 'object' type otherwise)
                //Expression value = Expression.Call(castMethMakeExpr, invokeMethod, Expression.Constant(null), objParam);


                Type expressType = GetNonNullableType(expression.Type);
                //create dummy express type instance to get the type
                object expresObject = Activator.CreateInstance(expressType, new object[] { });
                Type baseType = (expresObject as ExpressType).UnderlyingSystemType as Type;

                return Expression.Convert(leftValue, baseType);
            }

            //if underlying type is not ExpressType nothing happens
            return expression;
        }

        //helper function for runtime casting
        private static T Cast<T>(object o)
        {
            return (T)o;
        }

        private static Type[] CreateTypeArray(Type t)
        {
            return new Type[] { t };
        }

        private static object[] CreateObjectArray(object t)
        {
            return new object[] { t };
        }

        /// <summary>
        /// Helper function to wrap expression value to the nullable. It is used from 'UnwrapExpressType'
        /// to keep the concept of possibly undefined value.
        /// </summary>
        /// <param name="expObj">Input expression</param>
        /// <param name="baseType">Base type</param>
        /// <returns>Nullable wrapped expression</returns>
        private static Expression WrapToNullable(Expression expObj, Type baseType)
        {
            MethodInfo method = typeof(Type).GetMethod("MakeGenericType");
            Expression type = Expression.Call(Expression.Constant(typeof(Nullable<>)), method, new Expression[] { Expression.Constant(new Type[] { baseType }) });

            MethodInfo method2 = typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type), typeof(object[]) });
            Expression nullable = Expression.Call(method2, new Expression[] { type, Expression.Constant(new object[] { expObj }) });

            //create new nullable object and initialize it
            //Type t = typeof(Nullable<>).MakeGenericType(baseType);
            //object nullable = Activator.CreateInstance(t, new object[] { expObj });
            return nullable;
        }

        #region functions copied from System.Linq.Dynamic.ExpressionParser and modified for Xbim ExpressType

        /// <summary>
        /// Checking if the type is nullable type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>True if the type is nullable, false otherwise</returns>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Function for parsing enumeration values (case insensitive)
        /// </summary>
        /// <param name="name">Name of the enumeration value</param>
        /// <param name="type">Type of the enumeration</param>
        /// <returns>Enumeration value usable for comparison</returns>
        private static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0) return ((FieldInfo)memberInfos[0]).GetValue(null);
            }
            return null;
        }

        /// <summary>
        /// Parsing of the string value to the numeric value.
        /// </summary>
        /// <param name="text">String representing numeric value</param>
        /// <param name="type">Target type</param>
        /// <returns>Numeric value of specified type or NULL if parsing was not successful</returns>
        private static object ParseNumber(string text, Type type)
        {
            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb)) return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s)) return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us)) return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui)) return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul)) return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f)) return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d)) return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e)) return e;
                    break;
            }
            return null;
        }

        /// <summary>
        /// Generates 'equal' expression
        /// </summary>
        /// <param name="left">left expression operand</param>
        /// <param name="right">right expression operand</param>
        /// <returns>Equality expression</returns>
        public static Expression GenerateEqual(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            return Expression.Equal(left, right);
        }

        /// <summary>
        /// Generates 'not-equal' expression
        /// </summary>
        /// <param name="left">left expression operand</param>
        /// <param name="right">right expression operand</param>
        /// <returns>Non equality expression</returns>
        public static Expression GenerateNotEqual(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            return Expression.NotEqual(left, right);
        }

        /// <summary>
        /// Generates 'greater than' expression. 
        /// Function call method is applied on string type of operand 
        /// instead of standard expression function.
        /// </summary>
        /// <param name="left">left expression operand</param>
        /// <param name="right">right expression operand</param>
        /// <returns>Greater than expression</returns>
        public static Expression GenerateGreaterThan(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.GreaterThan(left, right);
        }

        public static Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.GreaterThanOrEqual(left, right);
        }

        public static Expression GenerateLessThan(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            if (left.Type == typeof(string))
            {
                return Expression.LessThan(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.LessThan(left, right);
        }

        public static Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.LessThanOrEqual(left, right);
        }

        public static Expression GenerateAdd(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }

        public static Expression GenerateSubtract(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            return Expression.Subtract(left, right);
        }

        public static Expression GenerateStringConcat(Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            return Expression.Call(
                null,
                typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }),
                new[] { left, right });
        }

        private static MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            MethodInfo method = left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
            return method;
        }

        private static Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            UnwrapExpressType(ref left, ref right);

            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        /// <summary>
        /// Gets non-nullable type underlying the generic nullable type
        /// </summary>
        /// <param name="type">Nullable type</param>
        /// <returns>Non-nullable type</returns>
        private static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Checks compatibility of numeric types
        /// </summary>
        /// <param name="source">Source type to check</param>
        /// <param name="target">Target type to check</param>
        /// <returns>TRUE if types are compatible, FALSE otherwise</returns>
        private static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }
        #endregion------------------------------------functions copied from System.Linq.Dynamic.ExpressionParser-----------------------
    }
}
