using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.Extensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO;

namespace Xbim.IO.DynamicGrouping
{
    /// <summary>
    /// Class for building dynamic query to get IFC instances which satisfy specified conditions.
    /// Type specified in the constructor is the target type. Use 'AddAttributeCondition' and
    /// 'AddPropertyCondition' to specify conditions. Check LastError and ErrorLog for information
    /// about problems during building the query (even in case of failure).
    /// </summary>
    public class XbimQueryBuilder
    {
        private Type _type;
        /// <summary>
        /// Type for the querry
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        private Expression _allExpressionGroup;
        private Expression _oneOfExpressionGroup;
        private Expression _anyExpressionGroup;
        private Expression _noneExpressionGroup;

        private UnaryExpression input;
        ParameterExpression inParam;

        /// <summary>
        /// Last error in query builder
        /// </summary>
        public string LastError { get; set; }
        private TextWriter _errLog;
        /// <summary>
        /// Error log of querry creation
        /// </summary>
        public TextWriter ErrorLog
        {
            get { return _errLog; }
        }

        /// <summary>
        /// Constructor for query targeting specified type
        /// </summary>
        /// <param name="ifcType">Target type for query</param>
        public XbimQueryBuilder(Type ifcType)
        {
            Init(ifcType);
        }

        /// <summary>
        /// Constructor for query targeting specified type
        /// </summary>
        /// <param name="ifcTypeName">Target type for query (case insensitive)</param>
        public XbimQueryBuilder(string ifcTypeName)
        {
            Type ifcType = IfcMetaData.IfcType(ifcTypeName).Type;
            Init(ifcType);
        }

        private void Init(Type ifcType)
        {
            IfcType ifcTypeLookup;
            if (IfcMetaData.TryGetIfcType(ifcType.Name.ToUpper(), out ifcTypeLookup))
            {
                _type = ifcType;
                _errLog = new StringWriter();

                //create input type from general type of persist entities
                inParam = Expression.Parameter(typeof(IPersistIfcEntity), "input");

                //first condition must be at least type of the input
                input = Expression.Convert(inParam, ifcType);
            }
            else
                FatalErr("Input type is not IFC type");
        }

        public void AddAttributeCondition(string attributName, string value, ValueRule valueRule, GroupRule groupRule)
        {
            if (attributName == null) { Err("Attribute name not specified"); return; }

            //check attribut existence in the specified type
            PropertyInfo attr = Type.GetProperty(attributName);
            if (attr == null) { Err("Attribute name '" + attributName + "' does not exist in specified type '" + Type.Name + "'."); return; }

            //create expression to access attribute
            Expression attrExp = Expression.Property(input, attr);

            //get value from the string
            Expression val = XbimQueryFactory.PromoteToConstant(attr.PropertyType, value);
            if (val == null) { Err("Value '" + value + "' could not be converted to the type of the attribute '" + attributName + "'."); return; }

            //create binary expression according to valueRule
            Expression expression = CreateValueExpression(attrExp, val, valueRule);
            if (expression == null) { Err("Value rule'" + Enum.GetName(typeof(ValueRule), valueRule)+ "' cannot be applied on the attribute '" + attributName + "'."); return; }

            //add resulting expression to the group
            AddToGroup(expression, groupRule);
        }

        public void AddAttributeCondition(string attributName, string value)
        {
            AddAttributeCondition(attributName, value, ValueRule.IS, GroupRule.ALL);
        }

        public void AddPropertyCondition(string pSetName, NameRule pSetNameRule, string propertyName, NameRule propNameRule, string value, ValueRule valueRule, GroupRule groupRule)
        {
            Expression psn = Expression.Constant(pSetName);
            Expression psnRl = Expression.Constant(pSetNameRule);
            Expression pn = Expression.Constant(propertyName);
            Expression pnRl = Expression.Constant(propNameRule);
            Expression valConst = Expression.Constant(value);
            Expression valRul = Expression.Constant(valueRule);

            MethodInfo getPropMeth = typeof(PropertyHelper).GetMethod("EvaluatePropertyValue");
            if (getPropMeth == null) throw new Exception("Wrong method definition.");
            //public static IfcValue GetPropertyValue(IPersistIfcEntity element, string pSetName, NameRule pSetNameRule, string propertyName, NameRule propNameRule)
            Expression property = Expression.Call(getPropMeth, new Expression[] {input ,psn, psnRl, pn, pnRl, valConst, valRul}); //bool result of the test

            AddToGroup(property, groupRule);
        }

        public void AddPropertyCondition(string pSetName, string propertyName, string value)
        {
            AddPropertyCondition(pSetName, NameRule.IS, propertyName, NameRule.IS, value, ValueRule.IS, GroupRule.ALL);
        }

        private Expression CreateValueExpression(Expression left, Expression right, ValueRule valueRule)
        {
            switch (valueRule)
            {
                case ValueRule.IS: return XbimQueryFactory.GenerateEqual(left, right);
                case ValueRule.IS_NOT: return XbimQueryFactory.GenerateNotEqual(left, right);
                case ValueRule.CONTAINS: return XbimQueryFactory.GenerateContains(left, right);
                case ValueRule.NOT_CONTAINS: return Expression.Not(XbimQueryFactory.GenerateContains(left, right));
                case ValueRule.GREATER_THAN: return XbimQueryFactory.GenerateGreaterThan(left, right);
                case ValueRule.LESS_THAN: return XbimQueryFactory.GenerateLessThan(left, right);
                default:
                    FatalErr("Unexpected enumeration value: " + Enum.GetName(typeof(GroupRule), valueRule));
                    break;
            }
            return null;
        }

        private void AddToGroup(Expression expression, GroupRule rule)
        {
            switch (rule)
            {
                case GroupRule.ALL:
                    if (_allExpressionGroup == null) { _allExpressionGroup = expression; return;}
                    _allExpressionGroup = Expression.AndAlso(_allExpressionGroup, expression);
                    break;
                case GroupRule.ONE_OF:
                    if (_oneOfExpressionGroup == null) { _oneOfExpressionGroup = expression; return; }
                    _oneOfExpressionGroup = Expression.ExclusiveOr(_oneOfExpressionGroup, expression);
                    break;
                case GroupRule.ANY:
                    if (_anyExpressionGroup == null) { _anyExpressionGroup = expression; return; }
                    _anyExpressionGroup = Expression.OrElse(_anyExpressionGroup, expression);
                    break;
                case GroupRule.NONE:
                    if (_noneExpressionGroup == null) { _noneExpressionGroup = Expression.Not(expression); return; }
                    _noneExpressionGroup = Expression.AndAlso(_noneExpressionGroup, expression);
                    break;
                default:
                    FatalErr("Unexpected enumeration value: " + Enum.GetName(typeof(GroupRule), rule));
                    break;
            }
        }

        /// <summary>
        /// Builds delegate function from inserted rules which is usable in IModel.Instances.Where(<b>delegate_expression</b>)
        /// </summary>
        /// <returns><b>delegate expression</b> for use in 'Where' function of IEnumerable</returns>
        public Expression<Func<IPersistIfcEntity, bool>> BuildQuery()
        {
            //initial expression
            Expression result = null;

            if (_allExpressionGroup != null) result = _allExpressionGroup;
            if (_anyExpressionGroup != null) 
            {
                if (result == null) result = _anyExpressionGroup;
                else result = Expression.AndAlso(result, _anyExpressionGroup);
            }
            if (_noneExpressionGroup != null)
            {
                if (result == null) result = _noneExpressionGroup;
                else result = Expression.AndAlso(result, _noneExpressionGroup);
            }
            if (_oneOfExpressionGroup != null)
            {
                if (result == null) result = _oneOfExpressionGroup;
                else result = Expression.AndAlso(result, _oneOfExpressionGroup);
            }

            //condition to check type
            MethodInfo getTypeMeth = typeof(object).GetMethod("GetType");
            Expression type = Expression.Call(inParam, getTypeMeth);
            Expression chckType = Expression.Constant(Type, typeof(Type));

            MethodInfo isAssignMeth = typeof(Type).GetMethod("IsAssignableFrom");
            Expression cond = Expression.Call(chckType, isAssignMeth, type);

            if (result == null) result = Expression.Constant(true);
            result = Expression.Condition(cond, result, Expression.Constant(false));

            Expression<Func<IPersistIfcEntity, bool>> exp = Expression.Lambda<Func<IPersistIfcEntity, bool>>(result, inParam);

            return exp;
        }

        private void Err(string msg)
        {
            LastError = msg;
            _errLog.WriteLine(msg);
        }

        private void FatalErr(string msg)
        {
            Err(msg);
            throw new Exception(msg);
        }
    }

    public enum GroupRule
    {
        ALL,
        ONE_OF,
        ANY,
        NONE
    }

    public enum ValueRule
    {
        IS,
        IS_NOT,
        CONTAINS,
        NOT_CONTAINS,
        GREATER_THAN,
        LESS_THAN
    }

    public enum NameRule
    {
        IS,
        IS_NOT,
        CONTAINS,
        NOT_CONTAINS
    }

    public class PropertyHelper
    {
        public static bool EvaluatePropertyValue(IPersistIfcEntity element, string pSetName, NameRule pSetNameRule, string propertyName, NameRule propNameRule, string value, ValueRule valueRule)
        {
            Dictionary<IfcLabel, Dictionary<IfcIdentifier, IfcValue>> allProperties = null;

            if (string.IsNullOrEmpty(propertyName)) return false;

            //properties could be defined in IfcTypeObject as HasProperties
            IfcTypeObject typeObject = element as IfcTypeObject;
            if (typeObject != null)
            {
                allProperties = typeObject.GetAllPropertySingleValues();
            }
            //or properties could be defined in IfcObject in IsDefinedBy
            IfcObject ifcObject = element as IfcObject;
            if (ifcObject != null)
            {
                allProperties = ifcObject.GetAllPropertySingleValues();
            }
            //or properties could be defined for material as ExtendedMaterialProperties
            IfcMaterial material = element as IfcMaterial;
            if (material != null)
            {
                allProperties = material.GetAllPropertySingleValues();
            }

            //getting properties is not supported otherwise
            if (allProperties != null)
            {
                foreach (var p in allProperties)
                {
                    //if pSetName is null all property sets are inspected
                    if (pSetName != null)
                    {
                        if (!IsRightName(pSetName, p.Key, pSetNameRule))
                        {
                            continue;
                        }
                    }
                    foreach (var prop in p.Value)
                    {
                        //if name is not specified all values are returned
                       if (IsRightName(propertyName, prop.Key, propNameRule))
                        {
                           Type t = ((ExpressType)(prop.Value)).UnderlyingSystemType;
                           Expression right = XbimQueryFactory.PromoteToConstant(t, value);
                           Expression left = XbimQueryFactory.PromoteToConstant(t, prop.Value.ToString()); //todo: this should be more sofisticated than 'ToString()'
                           Expression eval = CreateValueExpression(left, right, valueRule);

                           return Expression.Lambda<Func<bool>>(eval).Compile()();
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsRightName(string shouldBe, string isValue, NameRule rule)
        {
            switch (rule)
            {
                case NameRule.IS: return shouldBe == isValue;
                case NameRule.IS_NOT: return shouldBe != isValue;
                case NameRule.CONTAINS: return isValue.Contains(shouldBe);
                case NameRule.NOT_CONTAINS: return !isValue.Contains(shouldBe);
                default:
                    throw new Exception("Unexpected enumeration member");
            }
        }

        private static Expression CreateValueExpression(Expression left, Expression right, ValueRule valueRule)
        {
            switch (valueRule)
            {
                case ValueRule.IS: return XbimQueryFactory.GenerateEqual(left, right);
                case ValueRule.IS_NOT: return XbimQueryFactory.GenerateNotEqual(left, right);
                case ValueRule.CONTAINS: return XbimQueryFactory.GenerateContains(left, right);
                case ValueRule.NOT_CONTAINS: return Expression.Not(XbimQueryFactory.GenerateContains(left, right));
                case ValueRule.GREATER_THAN: return XbimQueryFactory.GenerateGreaterThan(left, right);
                case ValueRule.LESS_THAN: return XbimQueryFactory.GenerateLessThan(left, right);
                default:
                    throw new Exception("Unexpected enumeration value: " + Enum.GetName(typeof(GroupRule), valueRule));
            }
        }
    }
}
